using System.Threading;
using Microsoft.Data.SqlClient;

namespace FarmasilApp.Persistencia;

/// <summary>
/// Expone temporalmente la conexión y transacción SQL abiertas por el módulo
/// que coordina una operación. Permite que otros repositorios participen
/// en la misma unidad atómica sin abrir una segunda conexión (PEN-039).
/// </summary>
public sealed class ContextoTransaccionSql
{
    private readonly AsyncLocal<Estado?> _actual = new();

    /// <summary>Indica si el flujo actual ya participa en una transacción SQL.</summary>
    public bool HayTransaccion => _actual.Value is not null;

    /// <summary>
    /// Establece la conexión y transacción del coordinador durante el alcance indicado.
    /// El contexto no es propietario de ellas: no realiza Commit, Rollback ni Dispose.
    /// </summary>
    public IDisposable Establecer(
        SqlConnection conexion,
        SqlTransaction transaccion)
    {
        ArgumentNullException.ThrowIfNull(conexion);
        ArgumentNullException.ThrowIfNull(transaccion);

        if (transaccion.Connection is null
            || !ReferenceEquals(transaccion.Connection, conexion))
        {
            throw new ArgumentException(
                "La transacción debe pertenecer a la conexión indicada.",
                nameof(transaccion));
        }

        if (_actual.Value is not null)
        {
            throw new InvalidOperationException(
                "Ya existe una transacción SQL activa en el flujo actual.");
        }

        _actual.Value = new Estado(
            conexion,
            transaccion);

        return new Alcance(this);
    }

    /// <summary>
    /// Devuelve la conexión y transacción activas.
    /// Las escrituras coordinadas de ILotes deben usar este método
    /// en lugar de abrir una conexión propia.
    /// </summary>
    public (
        SqlConnection Conexion,
        SqlTransaction Transaccion
    ) ExigirActual()
    {
        var estado = _actual.Value
            ?? throw new InvalidOperationException(
                "La operación requiere una transacción SQL activa del módulo llamador.");

        if (estado.Transaccion.Connection is null)
        {
            throw new InvalidOperationException(
                "La transacción SQL del módulo llamador ya no está activa.");
        }

        return (
            estado.Conexion,
            estado.Transaccion);
    }

    private void Limpiar()
    {
        _actual.Value = null;
    }

    private sealed record Estado(
        SqlConnection Conexion,
        SqlTransaction Transaccion);

    private sealed class Alcance : IDisposable
    {
        private ContextoTransaccionSql? _dueno;

        public Alcance(ContextoTransaccionSql dueno)
        {
            _dueno = dueno;
        }

        public void Dispose()
        {
            Interlocked.Exchange(
                    ref _dueno,
                    null)
                ?.Limpiar();
        }
    }
}
