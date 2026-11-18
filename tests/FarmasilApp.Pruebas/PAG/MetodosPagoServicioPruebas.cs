using FarmasilApp.Modelo.PAG;

namespace FarmasilApp.Pruebas.PAG;

public class MetodosPagoServicioPruebas
{
    [Fact]
    [Trait("Especificacion", "ESP-0018")]
    [Trait("Tecnica", "T-002")]
    public void ConsultarNormalizaNombreYEstadoAntesDeLlamarAlRepositorio()
    {
        var repositorio = new RepositorioDeMetodosPagoFalso();

        Servicio(repositorio).Consultar("  Tarjeta  ", "  Inactivo  ");

        Assert.Equal(("Tarjeta", "Inactivo"), repositorio.UltimaConsulta);
    }

    [Fact]
    [Trait("Especificacion", "ESP-0018")]
    [Trait("Tecnica", "T-001")]
    public void ConsultarSinCriteriosDevuelveMetodosActivosEInactivos()
    {
        var repositorio = new RepositorioDeMetodosPagoFalso();
        repositorio.Resultados.AddRange(new[]
        {
            Metodo(1, "Efectivo", "Activo"),
            Metodo(2, "Tarjeta", "Inactivo")
        });

        var resultados = Servicio(repositorio).Consultar(null, null);

        Assert.Equal(new[] { "Activo", "Inactivo" }, resultados.Select(metodo => metodo.Estado));
        Assert.Equal((null, null), repositorio.UltimaConsulta);
    }

    [Theory]
    [Trait("Especificacion", "ESP-0018")]
    [Trait("Tecnica", "T-001")]
    [InlineData("Tarjeta", null, "Tarjeta", null)]
    [InlineData("arj", null, "arj", null)]
    [InlineData(null, "Activo", null, "Activo")]
    [InlineData(null, "Inactivo", null, "Inactivo")]
    [InlineData("Tar", "Inactivo", "Tar", "Inactivo")]
    public void ConsultarPasaLosCriteriosOpcionalesAlRepositorio(
        string? nombreMetodo,
        string? estado,
        string? nombreEsperado,
        string? estadoEsperado)
    {
        var repositorio = new RepositorioDeMetodosPagoFalso();

        Servicio(repositorio).Consultar(nombreMetodo, estado);

        Assert.Equal((nombreEsperado, estadoEsperado), repositorio.UltimaConsulta);
    }

    [Fact]
    [Trait("Especificacion", "ESP-0018")]
    [Trait("Tecnica", "T-001")]
    public void ConsultarConEstadoNoPermitidoLanzaArgumentException()
    {
        var repositorio = new RepositorioDeMetodosPagoFalso();

        var excepcion = Assert.Throws<ArgumentException>(
            () => Servicio(repositorio).Consultar(null, "Pendiente"));

        Assert.Equal("estado", excepcion.ParamName);
        Assert.Null(repositorio.UltimaConsulta);
    }

    [Fact]
    [Trait("Especificacion", "ESP-0018")]
    [Trait("Tecnica", "T-002")]
    public void ElRepositorioEsObligatorio()
    {
        Assert.Throws<ArgumentNullException>(() => new MetodosPagoServicio(null!));
    }

    private static MetodosPagoServicio Servicio(RepositorioDeMetodosPagoFalso repositorio) =>
        new(repositorio);

    private static MetodoPagoConsulta Metodo(int id, string nombre, string estado) =>
        new()
        {
            IdMetodoPago = id,
            NombreMetodo = nombre,
            Estado = estado
        };
}
