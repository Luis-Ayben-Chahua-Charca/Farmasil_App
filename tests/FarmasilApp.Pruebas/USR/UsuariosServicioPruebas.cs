using FarmasilApp.Modelo.Comun;
using FarmasilApp.Modelo.Seguridad;
using FarmasilApp.Modelo.USR;

namespace FarmasilApp.Pruebas.USR;

/// <summary>
/// Pruebas de ejemplo de <see cref="UsuariosServicio"/>: muestran cómo probar una regla con el repositorio falso.
/// </summary>
/// <remarks>
/// Son solo cinco, a propósito. La suite completa del módulo 9 del plan 09 la escribe R8 en el #80, como verificación
/// independiente de este módulo; la descripción del PR del #68 relaciona cada fila del plan con el método que la cubre.
/// Las sesiones son las simuladas de <c>main</c>: <c>admin_prueba</c> (id 1) y <c>tecnico_prueba</c> (id 2).
/// </remarks>
public class UsuariosServicioPruebas
{
    private static UsuariosServicio Servicio(RepositorioDeUsuariosFalso repositorio, Rol rolDeLaSesion = Rol.Administrador) =>
        new(repositorio, new SesionSimulada(rolDeLaSesion), new HashDeContrasenasFalso());

    [Fact]
    [Trait("Especificacion", "ESP-0033")]
    [Trait("Tecnica", "T-002")]
    public void CrearRegistraLaCuentaConElHashYNuncaConLaContrasena()
    {
        var repositorio = new RepositorioDeUsuariosFalso();
        repositorio.Agregar("admin_prueba", Rol.Administrador);

        Servicio(repositorio).Crear("  tecnico_nuevo  ", "Clave-123", Rol.Tecnico, EstadoRegistro.Activo);

        var creado = repositorio.UltimoCrear;
        Assert.NotNull(creado);
        Assert.Equal("tecnico_nuevo", creado.Value.NombreUsuario);
        Assert.Equal(HashDeContrasenasFalso.HashDe("Clave-123"), creado.Value.Hash);
        Assert.DoesNotContain(repositorio.Cuentas, cuenta => cuenta.Hash == "Clave-123");
        Assert.Equal(Rol.Tecnico, creado.Value.Rol);
        Assert.Equal(EstadoRegistro.Activo, creado.Value.Estado);
    }

    [Fact]
    [Trait("Especificacion", "ESP-0035")]
    [Trait("Tecnica", "T-012")]
    public void ActualizarCancelaSiDegradaALaUnicaCuentaAdministradoraActiva()
    {
        // La sesión es la cuenta 1; la única administradora activa es la cuenta 3, que es la que se degrada.
        var repositorio = new RepositorioDeUsuariosFalso();
        repositorio.Agregar("admin_prueba", Rol.Administrador, EstadoRegistro.Inactivo);
        repositorio.Agregar("tecnico_prueba", Rol.Tecnico);
        var unicaAdministradora = repositorio.Agregar("duena", Rol.Administrador);

        var excepcion = Assert.Throws<ReglaDeNegocioException>(
            () => Servicio(repositorio).Actualizar(unicaAdministradora, "duena", null, Rol.Tecnico, EstadoRegistro.Activo));

        Assert.Equal(MensajesUsuarios.UltimoAdministrador, excepcion.Message);
        Assert.Null(repositorio.UltimoActualizar);
    }

    [Fact]
    [Trait("Especificacion", "ESP-0036")]
    [Trait("Tecnica", "T-012")]
    public void DarDeBajaRechazaLaCuentaConLaQueEstaAbiertaLaSesion()
    {
        var repositorio = new RepositorioDeUsuariosFalso();
        var cuentaDeLaSesion = repositorio.Agregar("admin_prueba", Rol.Administrador);
        repositorio.Agregar("otra_admin", Rol.Administrador);

        var excepcion = Assert.Throws<ReglaDeNegocioException>(
            () => Servicio(repositorio).DarDeBaja(cuentaDeLaSesion));

        Assert.Equal(MensajesUsuarios.CuentaPropiaBaja, excepcion.Message);
        Assert.Null(repositorio.UltimaBaja);
    }

    [Fact]
    [Trait("Especificacion", "RNF-0006")]
    [Trait("Tecnica", "T-012")]
    public void UnaSesionTecnicoNoPuedeCrearCuentas()
    {
        var repositorio = new RepositorioDeUsuariosFalso();

        Assert.Throws<AccesoDenegadoException>(
            () => Servicio(repositorio, Rol.Tecnico).Crear("intrusa", "Clave-123", Rol.Administrador, EstadoRegistro.Activo));

        Assert.Null(repositorio.UltimoCrear);
    }

    [Fact]
    [Trait("Especificacion", "ESP-0035")]
    [Trait("Tecnica", "T-004")]
    public void ActualizarConLaContrasenaVaciaConservaElHashVigente()
    {
        var repositorio = new RepositorioDeUsuariosFalso();
        repositorio.Agregar("admin_prueba", Rol.Administrador);
        var tecnico = repositorio.Agregar("tecnico_prueba", Rol.Tecnico, hash: "HASH-VIGENTE");

        Servicio(repositorio).Actualizar(tecnico, "tecnico_renombrado", "", Rol.Tecnico, EstadoRegistro.Activo);

        Assert.False(repositorio.SePidioCambiarHash);
        Assert.Equal("HASH-VIGENTE", repositorio.Buscar(tecnico)?.Hash);
        Assert.Equal("tecnico_renombrado", repositorio.Buscar(tecnico)?.NombreUsuario);
    }
}
