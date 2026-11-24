using System.Data;
using FarmasilApp.Modelo.Comun;
using FarmasilApp.Modelo.PAG;
using Microsoft.Data.SqlClient;

namespace FarmasilApp.Persistencia.PAG;

/// <summary>
/// Repositorio de métodos de pago (ESP-0017 a ESP-0020).
/// Las consultas y escrituras se envían a SQL Server mediante parámetros.
/// </summary>
public sealed class MetodosPagoRepositorio : IMetodosPagoRepositorio
{
    private const int ErrorIndiceUnicoDuplicado = 2601;
    private const int ErrorRestriccionUnicaDuplicada = 2627;
    private const int ErrorConflictoDeRestriccion = 547;
    private const string PuntoDeGuardado = "AntesDeEliminarMetodoPago";

    private readonly OpcionesBaseDatos _opciones;

    public MetodosPagoRepositorio(OpcionesBaseDatos opciones)
    {
        _opciones = opciones
            ?? throw new ArgumentNullException(nameof(opciones));
    }

    public IReadOnlyList<MetodoPagoConsulta> Consultar(
        string? nombreMetodo,
        string? estado)
    {
        const string sql = """
            SELECT
                m.id_metodo_pago,
                m.nombre_metodo,
                m.estado
            FROM dbo.TBL_METODOS_PAGO AS m
            WHERE
                (
                    @nombreMetodoLike IS NULL
                    OR m.nombre_metodo LIKE N'%' + @nombreMetodoLike + N'%' ESCAPE '\'
                )
                AND
                (
                    @estado IS NULL
                    OR m.estado = @estado
                )
            ORDER BY m.id_metodo_pago;
            """;

        var metodos = new List<MetodoPagoConsulta>();

        using var conexion = new SqlConnection(_opciones.CadenaConexion);
        using var comando = new SqlCommand(sql, conexion);

        comando.Parameters.Add(
            new SqlParameter("@nombreMetodoLike", SqlDbType.NVarChar, -1)
            {
                Value = nombreMetodo is null
                    ? DBNull.Value
                    : EscaparLike(nombreMetodo)
            });

        comando.Parameters.Add(
            new SqlParameter("@estado", SqlDbType.NVarChar, 10)
            {
                Value = (object?)estado ?? DBNull.Value
            });

        conexion.Open();

        using var lector = comando.ExecuteReader();

        while (lector.Read())
        {
            metodos.Add(new MetodoPagoConsulta
            {
                IdMetodoPago = lector.GetInt32(
                    lector.GetOrdinal("id_metodo_pago")),

                NombreMetodo = lector.GetString(
                    lector.GetOrdinal("nombre_metodo")),

                Estado = lector.GetString(
                    lector.GetOrdinal("estado"))
            });
        }

        return metodos;
    }

    public bool ExisteNombre(string nombreMetodo, int? excluirIdMetodoPago)
    {
        const string sql = """
            SELECT CASE WHEN EXISTS (
                SELECT 1
                FROM dbo.TBL_METODOS_PAGO AS m
                WHERE m.nombre_metodo = @nombreMetodo
                  AND (@excluirId IS NULL OR m.id_metodo_pago <> @excluirId)
            ) THEN 1 ELSE 0 END;
            """;

        using var conexion = Abrir();
        using var comando = new SqlCommand(sql, conexion);
        comando.Parameters.Add(ParametroNombre("@nombreMetodo", nombreMetodo));
        comando.Parameters.Add(
            new SqlParameter("@excluirId", SqlDbType.Int)
            {
                Value = (object?)excluirIdMetodoPago ?? DBNull.Value
            });

        return (int)comando.ExecuteScalar()! == 1;
    }

    public int Crear(string nombreMetodo, string estado)
    {
        const string sql = """
            INSERT INTO dbo.TBL_METODOS_PAGO (nombre_metodo, estado)
            OUTPUT INSERTED.id_metodo_pago
            VALUES (@nombreMetodo, @estado);
            """;

        using var conexion = Abrir();
        using var transaccion = conexion.BeginTransaction();
        using var comando = new SqlCommand(sql, conexion, transaccion);
        comando.Parameters.Add(ParametroNombre("@nombreMetodo", nombreMetodo));
        comando.Parameters.Add(ParametroEstado("@estado", estado));

        try
        {
            var id = (int)comando.ExecuteScalar()!;
            transaccion.Commit();
            return id;
        }
        catch (SqlException ex) when (EsDuplicado(ex))
        {
            throw new ReglaDeNegocioException(MensajesMetodosPago.NombreRepetido);
        }
    }

    public void Actualizar(int idMetodoPago, string nombreMetodo, string estado)
    {
        const string sql = """
            UPDATE dbo.TBL_METODOS_PAGO
            SET nombre_metodo = @nombreMetodo,
                estado = @estado
            WHERE id_metodo_pago = @idMetodoPago;
            """;

        using var conexion = Abrir();
        using var transaccion = conexion.BeginTransaction();
        using var comando = new SqlCommand(sql, conexion, transaccion);
        comando.Parameters.Add(ParametroId("@idMetodoPago", idMetodoPago));
        comando.Parameters.Add(ParametroNombre("@nombreMetodo", nombreMetodo));
        comando.Parameters.Add(ParametroEstado("@estado", estado));

        try
        {
            if (comando.ExecuteNonQuery() != 1)
            {
                throw new InvalidOperationException($"El método de pago {idMetodoPago} no existe.");
            }

            transaccion.Commit();
        }
        catch (SqlException ex) when (EsDuplicado(ex))
        {
            throw new ReglaDeNegocioException(MensajesMetodosPago.NombreRepetido);
        }
    }

    /// <summary>
    /// Intenta la baja física y, si una referencia produce el error 547, restaura el punto de guardado y desactiva
    /// el método en la misma transacción. La clave foránea decide si existen referencias; no se consultan tablas ajenas.
    /// </summary>
    public ResultadoBajaMetodoPago DarDeBaja(int idMetodoPago)
    {
        const string sqlEliminar = """
            DELETE FROM dbo.TBL_METODOS_PAGO
            WHERE id_metodo_pago = @idMetodoPago;
            """;

        const string sqlDesactivar = """
            UPDATE dbo.TBL_METODOS_PAGO
            SET estado = N'Inactivo'
            WHERE id_metodo_pago = @idMetodoPago;
            """;

        using var conexion = Abrir();

        using (var xactAbort = new SqlCommand("SET XACT_ABORT OFF;", conexion))
        {
            xactAbort.ExecuteNonQuery();
        }

        using var transaccion = conexion.BeginTransaction();
        transaccion.Save(PuntoDeGuardado);

        try
        {
            using var eliminar = new SqlCommand(sqlEliminar, conexion, transaccion);
            eliminar.Parameters.Add(ParametroId("@idMetodoPago", idMetodoPago));

            if (eliminar.ExecuteNonQuery() != 1)
            {
                throw new InvalidOperationException($"El método de pago {idMetodoPago} no existe.");
            }

            transaccion.Commit();
            return ResultadoBajaMetodoPago.Eliminado;
        }
        catch (SqlException ex) when (ex.Number == ErrorConflictoDeRestriccion)
        {
            if (EstadoDeLaTransaccion(conexion, transaccion) != 1)
            {
                transaccion.Rollback();
                throw new InvalidOperationException(
                    $"La baja del método de pago {idMetodoPago} no se pudo completar: la transacción quedó no confirmable.",
                    ex);
            }

            transaccion.Rollback(PuntoDeGuardado);

            using var desactivar = new SqlCommand(sqlDesactivar, conexion, transaccion);
            desactivar.Parameters.Add(ParametroId("@idMetodoPago", idMetodoPago));

            if (desactivar.ExecuteNonQuery() != 1)
            {
                throw new InvalidOperationException($"El método de pago {idMetodoPago} no existe.");
            }

            transaccion.Commit();
            return ResultadoBajaMetodoPago.Desactivado;
        }
    }

    private static string EscaparLike(string valor)
    {
        return valor
            .Replace(@"\", @"\\")
            .Replace("%", @"\%")
            .Replace("_", @"\_")
            .Replace("[", @"\[");
    }

    private SqlConnection Abrir()
    {
        var conexion = new SqlConnection(_opciones.CadenaConexion);
        conexion.Open();
        return conexion;
    }

    private static int EstadoDeLaTransaccion(SqlConnection conexion, SqlTransaction transaccion)
    {
        using var comando = new SqlCommand("SELECT XACT_STATE();", conexion, transaccion);
        return Convert.ToInt32(comando.ExecuteScalar(), System.Globalization.CultureInfo.InvariantCulture);
    }

    private static bool EsDuplicado(SqlException ex) =>
        ex.Number is ErrorIndiceUnicoDuplicado or ErrorRestriccionUnicaDuplicada;

    private static SqlParameter ParametroId(string nombre, int id) =>
        new(nombre, SqlDbType.Int) { Value = id };

    private static SqlParameter ParametroNombre(string nombreParametro, string nombreMetodo) =>
        new(nombreParametro, SqlDbType.NVarChar, Math.Max(MetodosPagoServicio.LongitudMaximaNombre, nombreMetodo.Length))
        {
            Value = nombreMetodo
        };

    private static SqlParameter ParametroEstado(string nombre, string estado) =>
        new(nombre, SqlDbType.NVarChar, 10) { Value = estado };
}
