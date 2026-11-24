namespace FarmasilApp.Modelo.ALV;

/// <summary>Retiro guardado. Las copias históricas se conservan en las actualizaciones (DEC-035).</summary>
public sealed class AlertaRetiro
{
    public int IdRetiro { get; init; }
    public int? IdLote { get; init; }
    public string NombreProducto { get; init; } = string.Empty;
    public string NumeroLote { get; init; } = string.Empty;
    public DateOnly FechaVencimiento { get; init; }
    public bool EsLoteDefectuoso { get; init; }
    public string? AlertaDigemid { get; init; }
}

public sealed class NuevaAlertaRetiro
{
    public int IdLote { get; init; }
    public string NombreProducto { get; init; } = string.Empty;
    public string NumeroLote { get; init; } = string.Empty;
    public DateOnly FechaVencimiento { get; init; }
    public bool EsLoteDefectuoso { get; init; }
    public string? AlertaDigemid { get; init; }
    public DateOnly FechaRegistro { get; init; }
}
