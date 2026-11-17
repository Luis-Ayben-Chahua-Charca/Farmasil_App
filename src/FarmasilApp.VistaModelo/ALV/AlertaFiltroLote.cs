namespace FarmasilApp.VistaModelo.ALV;

public sealed class AlertaFiltroLote
{
    public int? IdLote { get; init; }
    public string Descripcion { get; init; } = string.Empty;
    public static AlertaFiltroLote Todos => new() { Descripcion = "Todos los lotes" };
}
