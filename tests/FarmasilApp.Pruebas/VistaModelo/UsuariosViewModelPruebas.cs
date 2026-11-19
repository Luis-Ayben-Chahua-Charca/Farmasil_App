using FarmasilApp.Modelo.Comun;
using FarmasilApp.Modelo.Seguridad;
using FarmasilApp.Modelo.USR;
using FarmasilApp.Pruebas.USR;
using FarmasilApp.VistaModelo.USR;

namespace FarmasilApp.Pruebas.VistaModelo;

public class UsuariosViewModelPruebas
{
    private static UsuariosViewModel Crear(RepositorioDeUsuariosFalso repositorio, MensajeServicioFalso mensajes) =>
        new(new UsuariosServicio(repositorio, new SesionSimulada(), new HashDeContrasenasFalso()), mensajes);

    [Fact]
    [Trait("Especificacion", "ESP-0036")]
    [Trait("Tecnica", "T-003")]
    public async Task NegarLaConfirmacionDeBajaConservaLasCuentasYLaSeleccion()
    {
        var repositorio = new RepositorioDeUsuariosFalso();
        repositorio.Agregar("admin_prueba", Rol.Administrador);
        var id = repositorio.Agregar("tecnico", Rol.Tecnico);
        var mensajes = new MensajeServicioFalso { RespuestaConfirmacion = false };
        var vistaModelo = Crear(repositorio, mensajes);
        await vistaModelo.InicializarCommand.ExecuteAsync();
        var seleccionada = vistaModelo.Cuentas.Single(cuenta => cuenta.IdUsuario == id);
        vistaModelo.CuentaSeleccionada = seleccionada;
        var originales = repositorio.Cuentas.ToArray();
        var filas = vistaModelo.Cuentas.ToArray();

        await vistaModelo.EliminarCommand.ExecuteAsync();

        Assert.Equal(("USR", (string?)null, MensajesUsuarios.ConfirmarBaja), Assert.Single(mensajes.Confirmaciones));
        Assert.Null(repositorio.UltimaBaja);
        Assert.Equal(originales, repositorio.Cuentas);
        Assert.Equal(filas, vistaModelo.Cuentas);
        Assert.Same(seleccionada, vistaModelo.CuentaSeleccionada);
        Assert.Empty(mensajes.Exitos);
        Assert.Empty(mensajes.Errores);
    }

    [Fact]
    [Trait("Especificacion", "ESP-0036")]
    [Trait("Tecnica", "T-012")]
    public async Task LaCuentaPropiaSeRechazaAntesDeMostrarElModal()
    {
        var repositorio = new RepositorioDeUsuariosFalso();
        var id = repositorio.Agregar("admin_prueba", Rol.Administrador);
        var mensajes = new MensajeServicioFalso();
        var vistaModelo = Crear(repositorio, mensajes);
        await vistaModelo.InicializarCommand.ExecuteAsync();
        vistaModelo.CuentaSeleccionada = vistaModelo.Cuentas.Single(cuenta => cuenta.IdUsuario == id);

        await vistaModelo.EliminarCommand.ExecuteAsync();

        Assert.Empty(mensajes.Confirmaciones);
        Assert.Equal(MensajesUsuarios.CuentaPropiaBaja, Assert.Single(mensajes.Errores).Mensaje);
        Assert.Empty(mensajes.Exitos);
        Assert.Null(repositorio.UltimaBaja);
        Assert.NotNull(repositorio.Buscar(id));
    }

    [Theory]
    [Trait("Especificacion", "ESP-0036")]
    [Trait("Tecnica", "T-002")]
    [InlineData(false)]
    [InlineData(true)]
    public async Task ConfirmarLaBajaRefrescaLaListaTrasEliminarODesactivar(bool conReferencias)
    {
        var repositorio = new RepositorioDeUsuariosFalso();
        repositorio.Agregar("admin_prueba", Rol.Administrador);
        var id = repositorio.Agregar("tecnico", Rol.Tecnico);
        if (conReferencias) repositorio.MarcarConReferencias(id);
        var mensajes = new MensajeServicioFalso();
        var vistaModelo = Crear(repositorio, mensajes);
        await vistaModelo.InicializarCommand.ExecuteAsync();
        vistaModelo.CuentaSeleccionada = vistaModelo.Cuentas.Single(cuenta => cuenta.IdUsuario == id);

        await vistaModelo.EliminarCommand.ExecuteAsync();

        Assert.Single(mensajes.Confirmaciones);
        Assert.Equal(MensajesUsuarios.CuentaDadaDeBaja, Assert.Single(mensajes.Exitos).Mensaje);
        Assert.Empty(mensajes.Errores);
        Assert.Equal(id, repositorio.UltimaBaja);
        Assert.Null(vistaModelo.CuentaSeleccionada);
        if (conReferencias)
        {
            Assert.Equal(EstadoRegistro.Inactivo, vistaModelo.Cuentas.Single(cuenta => cuenta.IdUsuario == id).Estado);
        }
        else
        {
            Assert.DoesNotContain(vistaModelo.Cuentas, cuenta => cuenta.IdUsuario == id);
        }
    }
}
