namespace FarmasilApp.Pruebas;

/// <summary>Reloj de prueba: siempre marca el mismo día, a mediodía y en UTC.</summary>
internal sealed class RelojFijo : TimeProvider
{
    private readonly DateTimeOffset _ahora;

    public RelojFijo(DateOnly dia)
    {
        _ahora = new DateTimeOffset(dia.ToDateTime(new TimeOnly(12, 0)), TimeSpan.Zero);
    }

    public override TimeZoneInfo LocalTimeZone => TimeZoneInfo.Utc;

    public override DateTimeOffset GetUtcNow() => _ahora;
}
