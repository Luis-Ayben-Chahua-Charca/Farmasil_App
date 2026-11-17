namespace FarmasilApp.Modelo.PRV;

/// <summary>
/// Proveedor mostrado por la consulta de Proveedores (ESP-0038).
/// </summary>
public sealed class ProveedorConsulta
{
    public int IdProveedor { get; init; }

    public string Ruc { get; init; } = string.Empty;

    public string RazonSocial { get; init; } = string.Empty;

    /// <summary>Teléfono de contacto; es opcional en el diccionario de datos.</summary>
    public string? Telefono { get; init; }

    public string Estado { get; init; } = string.Empty;
}
