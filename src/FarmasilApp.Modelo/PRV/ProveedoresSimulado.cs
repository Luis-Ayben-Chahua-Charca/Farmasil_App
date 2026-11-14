using FarmasilApp.Modelo.Comun;

namespace FarmasilApp.Modelo.PRV;

/// <summary>
/// Implementación simulada de <see cref="IProveedores"/> mientras PRV no esté listo (plan, sección 10).
/// Devuelve siempre los proveedores de database/002_datos_prueba.sql, con los mismos ids
/// (los que tiene una base recién creada). No tiene estado.
/// </summary>
public sealed class ProveedoresSimulado : IProveedores
{
    private static readonly IReadOnlyList<ProveedorDto> Todos = new[]
    {
        new ProveedorDto { IdProveedor = 1, Ruc = "20100070970", RazonSocial = "Drogueria Andina SAC", Telefono = "054223344", Estado = EstadoRegistro.Activo },
        new ProveedorDto { IdProveedor = 2, Ruc = "20481234567", RazonSocial = "Distribuidora Salud Sur SAC", Telefono = "054445566", Estado = EstadoRegistro.Activo },
        new ProveedorDto { IdProveedor = 3, Ruc = "20609876543", RazonSocial = "Laboratorios del Pacifico SAC", Telefono = "054778899", Estado = EstadoRegistro.Activo },
        new ProveedorDto { IdProveedor = 4, Ruc = "20501112223", RazonSocial = "Proveedor Inactivo SAC", Telefono = null, Estado = EstadoRegistro.Inactivo },
    };

    public IReadOnlyList<ProveedorDto> ProveedoresActivos()
    {
        return Todos.Where(proveedor => proveedor.Estado == EstadoRegistro.Activo).ToArray();
    }

    public ProveedorDto? ObtenerProveedor(int idProveedor)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(idProveedor);

        return Todos.FirstOrDefault(proveedor => proveedor.IdProveedor == idProveedor);
    }
}
