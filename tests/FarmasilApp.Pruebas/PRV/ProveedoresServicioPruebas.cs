using FarmasilApp.Modelo.PRV;

namespace FarmasilApp.Pruebas.PRV;

public class ProveedoresServicioPruebas
{
    private static ProveedoresServicio Servicio(RepositorioDeProveedoresFalso repositorio) =>
        new(repositorio);

    [Fact]
    [Trait("Especificacion", "ESP-0038")]
    [Trait("Tecnica", "T-002")]
    public void ConsultarNormalizaLosCriteriosAntesDeLlamarAlRepositorio()
    {
        var repositorio = new RepositorioDeProveedoresFalso();

        Servicio(repositorio).Consultar("  Andina  ", "  Activo  ");

        var consulta = repositorio.UltimaConsulta;
        Assert.NotNull(consulta);
        Assert.Equal("Andina", consulta.Value.Busqueda);
        Assert.Equal("Activo", consulta.Value.Estado);
    }

    [Fact]
    [Trait("Especificacion", "ESP-0038")]
    [Trait("Tecnica", "T-002")]
    public void ConsultarConBusquedaVaciaLaPasaComoNulo()
    {
        var repositorio = new RepositorioDeProveedoresFalso();

        Servicio(repositorio).Consultar("   ", null);

        var consulta = repositorio.UltimaConsulta;
        Assert.NotNull(consulta);
        Assert.Null(consulta.Value.Busqueda);
        Assert.Null(consulta.Value.Estado);
    }

    [Fact]
    [Trait("Especificacion", "ESP-0038")]
    [Trait("Tecnica", "T-002")]
    public void ConsultarConEstadoVacioLoPasaComoNulo()
    {
        var repositorio = new RepositorioDeProveedoresFalso();

        Servicio(repositorio).Consultar(null, "   ");

        var consulta = repositorio.UltimaConsulta;
        Assert.NotNull(consulta);
        Assert.Null(consulta.Value.Estado);
    }

    [Fact]
    [Trait("Especificacion", "ESP-0038")]
    [Trait("Tecnica", "T-001")]
    public void ConsultarSinCriteriosListaTodos()
    {
        var repositorio = new RepositorioDeProveedoresFalso();
        repositorio.Resultados.Add(new ProveedorConsulta
        {
            IdProveedor = 1,
            Ruc = "20100070970",
            RazonSocial = "Drogueria Andina SAC",
            Telefono = "054223344",
            Estado = "Activo"
        });

        var resultados = Servicio(repositorio).Consultar(null, null);

        Assert.Single(resultados);
        Assert.Equal("Drogueria Andina SAC", resultados[0].RazonSocial);

        var consulta = repositorio.UltimaConsulta;
        Assert.NotNull(consulta);
        Assert.Null(consulta.Value.Busqueda);
        Assert.Null(consulta.Value.Estado);
    }

    [Theory]
    [Trait("Especificacion", "ESP-0038")]
    [Trait("Tecnica", "T-001")]
    [InlineData("20100070970", null, "20100070970", null)]
    [InlineData("Andina", null, "Andina", null)]
    [InlineData(null, "Activo", null, "Activo")]
    [InlineData(null, "Inactivo", null, "Inactivo")]
    [InlineData("Salud", "Activo", "Salud", "Activo")]
    public void ConsultarPasaElCriterioAlRepositorio(
        string? busqueda, string? estado,
        string? busquedaEsperada, string? estadoEsperado)
    {
        var repositorio = new RepositorioDeProveedoresFalso();

        Servicio(repositorio).Consultar(busqueda, estado);

        var consulta = repositorio.UltimaConsulta;
        Assert.NotNull(consulta);
        Assert.Equal(busquedaEsperada, consulta.Value.Busqueda);
        Assert.Equal(estadoEsperado, consulta.Value.Estado);
    }

    [Fact]
    [Trait("Especificacion", "ESP-0038")]
    [Trait("Tecnica", "T-001")]
    public void ConsultarSinResultadosDevuelveListaVacia()
    {
        var repositorio = new RepositorioDeProveedoresFalso();

        var resultados = Servicio(repositorio).Consultar("NoExiste", null);

        Assert.Empty(resultados);
    }

    [Fact]
    [Trait("Especificacion", "ESP-0038")]
    [Trait("Tecnica", "T-002")]
    public void ConsultarDevuelveProveedorConTelefonoNulo()
    {
        var repositorio = new RepositorioDeProveedoresFalso();
        repositorio.Resultados.Add(new ProveedorConsulta
        {
            IdProveedor = 4,
            Ruc = "20501112223",
            RazonSocial = "Proveedor Inactivo SAC",
            Telefono = null,
            Estado = "Inactivo"
        });

        var resultados = Servicio(repositorio).Consultar(null, null);

        Assert.Single(resultados);
        Assert.Null(resultados[0].Telefono);
        Assert.Equal("Inactivo", resultados[0].Estado);
    }

    [Fact]
    [Trait("Especificacion", "ESP-0038")]
    [Trait("Tecnica", "T-001")]
    public void ConsultarConEstadoInvalidoLanzaArgumentException()
    {
        var repositorio = new RepositorioDeProveedoresFalso();

        var excepcion = Assert.Throws<ArgumentException>(
            () => Servicio(repositorio).Consultar(null, "Suspendido"));

        Assert.Equal("estado", excepcion.ParamName);
        Assert.Null(repositorio.UltimaConsulta);
    }

    [Fact]
    [Trait("Especificacion", "ESP-0038")]
    [Trait("Tecnica", "T-002")]
    public void ElRepositorioEsObligatorio()
    {
        Assert.Throws<ArgumentNullException>(() => new ProveedoresServicio(null!));
    }
}
