namespace FarmasilApp.Modelo.PRV;

/// <summary>
/// Datos editables de un proveedor para ESP-0037 y ESP-0039.
/// El identificador lo genera SQL Server; el estado se elige en la ficha.
/// </summary>
public sealed class ProveedorEscritura
{
    public string Ruc { get; init; } = string.Empty;

    public string RazonSocial { get; init; } = string.Empty;

    /// <summary>Teléfono de contacto; es opcional en el diccionario de datos.</summary>
    public string? Telefono { get; init; }

    /// <summary>'Activo' o 'Inactivo', según el combo de la ficha.</summary>
    public string Estado { get; init; } = EstadoActivo;

    public const string EstadoActivo = "Activo";

    public const string EstadoInactivo = "Inactivo";
}
