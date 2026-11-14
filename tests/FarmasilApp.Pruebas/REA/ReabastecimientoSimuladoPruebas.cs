using FarmasilApp.Modelo.Comun;
using FarmasilApp.Modelo.REA;

namespace FarmasilApp.Pruebas.REA;

/// <summary>
/// Los datos esperados son los de database/002_datos_prueba.sql: tres ítems incluidos hoy, para ibuprofeno (2, automático y urgente),
/// loratadina (4, manual y normal) y omeprazol (6, automático y urgente).
/// </summary>
public class ReabastecimientoSimuladoPruebas
{
    private static readonly DateOnly Hoy = new(2026, 11, 14);

    private static IReabastecimiento Reabastecimiento() => new ReabastecimientoSimulado(new RelojFijo(Hoy));

    [Fact]
    public void LaListaTieneLosTresItemsDeLosDatosDePrueba()
    {
        var lista = Reabastecimiento().ListaReabastecimiento();

        Assert.Equal(new[] { 2, 4, 6 }, lista.Select(i => i.IdProducto));
    }

    [Fact]
    public void CadaItemTraeSuOrigenSuPrioridadYSuFecha()
    {
        var lista = Reabastecimiento().ListaReabastecimiento();

        Assert.Equal(OrigenReabastecimiento.Automatico, lista[0].Origen);
        Assert.Equal(Prioridad.Urgente, lista[0].Prioridad);
        Assert.Equal(OrigenReabastecimiento.Manual, lista[1].Origen);
        Assert.Equal(Prioridad.Normal, lista[1].Prioridad);
        Assert.All(lista, item => Assert.Equal(Hoy, item.FechaInclusion));
    }

    [Fact]
    public void ElRelojEsObligatorio()
    {
        Assert.Throws<ArgumentNullException>(() => new ReabastecimientoSimulado(null!));
    }
}
