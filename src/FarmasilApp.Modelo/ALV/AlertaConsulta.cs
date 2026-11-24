namespace FarmasilApp.Modelo.ALV;

/// <summary>Fila consolidada de ESP-0013: próxima a vencer o retiro registrado.</summary>
public sealed class AlertaConsulta
{
    public int? IdLote { get; init; }
    public int? IdRetiro { get; init; }
    public string? AlertaDigemid { get; init; }
    public string Medicamento { get; init; } = string.Empty;
    public string NumeroLote { get; init; } = string.Empty;
    public DateOnly FechaVencimiento { get; init; }
    public bool? EsLoteDefectuoso { get; init; }
    public string Origen { get; init; } = string.Empty;
}
