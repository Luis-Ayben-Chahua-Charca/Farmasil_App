using FarmasilApp.Modelo.PAG;
using FarmasilApp.VistaModelo.PAG;

namespace FarmasilApp.Pruebas.PAG;

public class MetodosPagoViewModelPruebas
{
    [Fact]
    [Trait("Especificacion", "ESP-0018")]
    [Trait("Tecnica", "T-001")]
    public async Task InicializarConsultaTodosLosMetodosYPreparaElFiltroDeEstado()
    {
        var repositorio = new RepositorioDeMetodosPagoFalso();
        repositorio.Resultados.AddRange(new[]
        {
            Metodo(1, "Efectivo", "Activo"),
            Metodo(2, "Yape", "Activo"),
            Metodo(3, "Tarjeta", "Inactivo")
        });
        var vistaModelo = new MetodosPagoViewModel(new MetodosPagoServicio(repositorio));

        await vistaModelo.InicializarCommand.ExecuteAsync();

        Assert.Equal(
            new[] { "-- Filtrar por Estado --", "Activo", "Inactivo" },
            vistaModelo.FiltrosEstado);
        Assert.Equal(new[] { "Activo", "Inactivo" }, vistaModelo.EstadosMetodo);
        Assert.Equal(new[] { "Efectivo", "Yape", "Tarjeta" }, vistaModelo.Metodos.Select(metodo => metodo.NombreMetodo));
        Assert.Equal((null, null), repositorio.UltimaConsulta);
    }

    [Fact]
    [Trait("Especificacion", "ESP-0018")]
    [Trait("Tecnica", "T-002")]
    public async Task BuscarPasaElNombreParcialYElEstadoSeleccionados()
    {
        var repositorio = new RepositorioDeMetodosPagoFalso();
        repositorio.Resultados.Add(Metodo(3, "Tarjeta", "Inactivo"));
        var vistaModelo = new MetodosPagoViewModel(new MetodosPagoServicio(repositorio))
        {
            Busqueda = "arj",
            EstadoFiltro = "Inactivo"
        };

        await vistaModelo.BuscarCommand.ExecuteAsync();

        Assert.Equal(("arj", "Inactivo"), repositorio.UltimaConsulta);
        Assert.Equal("Tarjeta", Assert.Single(vistaModelo.Metodos).NombreMetodo);
    }

    [Fact]
    [Trait("Especificacion", "ESP-0018")]
    [Trait("Tecnica", "T-001")]
    public async Task UnErrorDeConsultaSeRegistraYSeMuestraSinDejarResultadosAntiguos()
    {
        var repositorio = new RepositorioDeMetodosPagoFalso();
        repositorio.Resultados.Add(Metodo(1, "Efectivo", "Activo"));
        var vistaModelo = new MetodosPagoViewModel(new MetodosPagoServicio(repositorio));
        await vistaModelo.BuscarCommand.ExecuteAsync();
        repositorio.FallaAlConsultar = true;

        await vistaModelo.BuscarCommand.ExecuteAsync();

        Assert.Empty(vistaModelo.Metodos);
        Assert.Null(vistaModelo.MetodoSeleccionado);
        Assert.Equal(
            "No se pudieron consultar los métodos de pago. Inténtelo nuevamente.",
            vistaModelo.Mensaje);
    }

    private static MetodoPagoConsulta Metodo(int id, string nombre, string estado) =>
        new()
        {
            IdMetodoPago = id,
            NombreMetodo = nombre,
            Estado = estado
        };
}
