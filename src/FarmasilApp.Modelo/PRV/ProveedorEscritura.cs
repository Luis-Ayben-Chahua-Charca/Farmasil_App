namespace FarmasilApp.Modelo.PRV;

/// <summary>
/// Datos editables de un proveedor para ESP-0037 y ESP-0039.
/// El identificador y el estado no se capturan manualmente.
/// </summary>
public sealed class ProveedorEscritura
{
    public string Ruc { get; init; } = string.Empty;

    public string RazonSocial { get; init; } = string.Empty;

    /// <summary>Teléfono de contacto; es opcional en el diccionario de datos.</summary>
    public string? Telefono { get; init; }
}
