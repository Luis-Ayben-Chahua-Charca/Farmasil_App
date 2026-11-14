using FarmasilApp.Modelo.Comun;
using FarmasilApp.Modelo.Seguridad;

namespace FarmasilApp.Pruebas.Seguridad;

public class SesionSimuladaPruebas
{
    [Fact]
    public void SinRolEsLaCuentaAdministradoraDeLosDatosDePrueba()
    {
        // database/002_datos_prueba.sql: admin_prueba es la primera cuenta (id 1), rol Administrador.
        var sesion = new SesionSimulada();

        Assert.Equal(1, sesion.UsuarioActual.IdUsuario);
        Assert.Equal("admin_prueba", sesion.UsuarioActual.NombreUsuario);
        Assert.Equal(Rol.Administrador, sesion.UsuarioActual.Rol);
    }

    [Fact]
    public void ConRolTecnicoEsLaCuentaTecnicaDeLosDatosDePrueba()
    {
        // database/002_datos_prueba.sql: tecnico_prueba es la segunda cuenta (id 2), rol Tecnico.
        var sesion = new SesionSimulada(Rol.Tecnico);

        Assert.Equal(2, sesion.UsuarioActual.IdUsuario);
        Assert.Equal("tecnico_prueba", sesion.UsuarioActual.NombreUsuario);
        Assert.Equal(Rol.Tecnico, sesion.UsuarioActual.Rol);
    }

    [Theory]
    [InlineData(Rol.Administrador)]
    [InlineData(Rol.Tecnico)]
    public void ExigirElRolDeLaSesionNoLanza(Rol rol)
    {
        var sesion = new SesionSimulada(rol);

        var excepcion = Record.Exception(() => sesion.ExigirRol(rol));

        Assert.Null(excepcion);
    }

    [Fact]
    public void ExigirAdministradorConSesionTecnicaLanzaAccesoDenegado()
    {
        var sesion = new SesionSimulada(Rol.Tecnico);

        var excepcion = Assert.Throws<AccesoDenegadoException>(() => sesion.ExigirRol(Rol.Administrador));

        Assert.Contains("Administrador", excepcion.Message);
        Assert.Contains("Tecnico", excepcion.Message);
    }

    [Fact]
    public void ExigirTecnicoConSesionAdministradoraLanzaAccesoDenegado()
    {
        var sesion = new SesionSimulada(Rol.Administrador);

        Assert.Throws<AccesoDenegadoException>(() => sesion.ExigirRol(Rol.Tecnico));
    }

    [Fact]
    public void UnRolFueraDelDominioNoSeAcepta()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => new SesionSimulada((Rol)99));
    }

    [Fact]
    public void ExigirUnRolFueraDelDominioNoSeAcepta()
    {
        var sesion = new SesionSimulada();

        Assert.Throws<ArgumentOutOfRangeException>(() => sesion.ExigirRol((Rol)99));
    }
}
