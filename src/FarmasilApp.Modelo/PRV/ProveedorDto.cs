using FarmasilApp.Modelo.Comun;

namespace FarmasilApp.Modelo.PRV;

/// <summary>
/// Proveedor que ofrece <see cref="IProveedores"/> (TBL_PROVEEDORES).
/// </summary>
public sealed class ProveedorDto
{
    public int IdProveedor { get; init; }

    public string Ruc { get; init; } = string.Empty;

    public string RazonSocial { get; init; } = string.Empty;

    /// <summary>Teléfono de contacto; es opcional en el diccionario de datos.</summary>
    public string? Telefono { get; init; }

    public EstadoRegistro Estado { get; init; }
}
