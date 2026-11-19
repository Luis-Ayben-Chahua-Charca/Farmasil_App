using FarmasilApp.Modelo.Comun;
using FarmasilApp.Modelo.Seguridad;
using FarmasilApp.Modelo.USR;

namespace FarmasilApp.Pruebas.USR;

/// <summary>Pruebas unitarias de ESP-0033 a ESP-0036 y RNF-0006 con los contratos reales de Usuarios.</summary>
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

    [Theory]
    [Trait("Especificacion", "ESP-0033")]
    [Trait("Tecnica", "T-001")]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void CrearRechazaNombreVacioOSoloEspacios(string? nombre)
    {
        var repositorio = new RepositorioDeUsuariosFalso();
        var error = Assert.Throws<ReglaDeNegocioException>(
            () => Servicio(repositorio).Crear(nombre, "Clave-123", Rol.Tecnico, EstadoRegistro.Activo));
        Assert.Equal(MensajesUsuarios.NombreVacio, error.Message);
        Assert.Null(repositorio.UltimoCrear);
    }

    [Fact]
    [Trait("Especificacion", "ESP-0033")]
    [Trait("Tecnica", "T-001")]
    public void CrearRechazaNombreDemasiadoLargo()
    {
        var repositorio = new RepositorioDeUsuariosFalso();
        var error = Assert.Throws<ReglaDeNegocioException>(
            () => Servicio(repositorio).Crear(new string('a', 51), "Clave-123", Rol.Tecnico, EstadoRegistro.Activo));
        Assert.Equal(MensajesUsuarios.NombreDemasiadoLargo, error.Message);
        Assert.Null(repositorio.UltimoCrear);
    }

    [Fact]
    [Trait("Especificacion", "ESP-0033")]
    [Trait("Tecnica", "T-001")]
    public void CrearAceptaElNombreEnElLimiteDeCincuentaCaracteres()
    {
        var repositorio = new RepositorioDeUsuariosFalso();
        var nombre = new string('a', 50);
        var id = Servicio(repositorio).Crear(nombre, "Clave-123", Rol.Tecnico, EstadoRegistro.Activo);
        Assert.Equal(nombre, repositorio.Buscar(id)!.NombreUsuario);
    }

    [Fact]
    [Trait("Especificacion", "ESP-0033")]
    [Trait("Tecnica", "T-001")]
    public void CrearRechazaNombreRepetidoSinDistinguirMayusculas()
    {
        var repositorio = new RepositorioDeUsuariosFalso();
        repositorio.Agregar("usuario_existente", Rol.Tecnico);
        var error = Assert.Throws<ReglaDeNegocioException>(
            () => Servicio(repositorio).Crear("USUARIO_EXISTENTE", "Clave-123", Rol.Tecnico, EstadoRegistro.Activo));
        Assert.Equal(MensajesUsuarios.NombreRepetido, error.Message);
        Assert.Null(repositorio.UltimoCrear);
    }

    [Theory]
    [Trait("Especificacion", "ESP-0033")]
    [Trait("Tecnica", "T-001")]
    [InlineData(null)]
    [InlineData("")]
    public void CrearRechazaContrasenaVacia(string? contrasena)
    {
        var repositorio = new RepositorioDeUsuariosFalso();
        var error = Assert.Throws<ReglaDeNegocioException>(
            () => Servicio(repositorio).Crear("nuevo", contrasena, Rol.Tecnico, EstadoRegistro.Activo));
        Assert.Equal(MensajesUsuarios.ContrasenaVacia, error.Message);
        Assert.Null(repositorio.UltimoCrear);
    }

    [Theory]
    [Trait("Especificacion", "ESP-0033")]
    [Trait("Tecnica", "T-001")]
    [InlineData(null, EstadoRegistro.Activo, MensajesUsuarios.RolInvalido)]
    [InlineData((Rol)99, EstadoRegistro.Activo, MensajesUsuarios.RolInvalido)]
    [InlineData(Rol.Tecnico, null, MensajesUsuarios.EstadoInvalido)]
    [InlineData(Rol.Tecnico, (EstadoRegistro)99, MensajesUsuarios.EstadoInvalido)]
    public void CrearRechazaRolOEstadoFueraDeDominio(Rol? rol, EstadoRegistro? estado, string mensaje)
    {
        var repositorio = new RepositorioDeUsuariosFalso();
        var error = Assert.Throws<ReglaDeNegocioException>(
            () => Servicio(repositorio).Crear("nuevo", "Clave-123", rol, estado));
        Assert.Equal(mensaje, error.Message);
        Assert.Null(repositorio.UltimoCrear);
    }

    [Fact]
    [Trait("Especificacion", "ESP-0034")]
    [Trait("Tecnica", "T-002")]
    public void ConsultarNormalizaElNombreYConservaElFiltroDeRol()
    {
        var repositorio = new RepositorioDeUsuariosFalso();
        repositorio.Agregar("admin_prueba", Rol.Administrador);
        repositorio.Agregar("carlos_admin", Rol.Administrador);
        repositorio.Agregar("ana_tecnico", Rol.Tecnico, EstadoRegistro.Inactivo);

        var cuenta = Assert.Single(Servicio(repositorio).Consultar("  carlos_admin  ", Rol.Administrador));

        Assert.Equal("carlos_admin", cuenta.NombreUsuario);
        Assert.Equal(EstadoRegistro.Activo, cuenta.Estado);
        Assert.Equal("carlos_admin", repositorio.UltimaConsulta!.Value.NombreUsuario);
        Assert.Equal(Rol.Administrador, repositorio.UltimaConsulta.Value.Rol);
    }

    [Theory]
    [Trait("Especificacion", "ESP-0034")]
    [Trait("Tecnica", "T-001")]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void ConsultarSinNombreDevuelveTodasLasCuentas(string? nombre)
    {
        var repositorio = new RepositorioDeUsuariosFalso();
        repositorio.Agregar("admin_prueba", Rol.Administrador);
        repositorio.Agregar("tecnico_prueba", Rol.Tecnico, EstadoRegistro.Inactivo);
        var cuentas = Servicio(repositorio).Consultar(nombre, null);
        Assert.Equal(2, cuentas.Count);
        Assert.Null(repositorio.UltimaConsulta!.Value.NombreUsuario);
        Assert.Null(repositorio.UltimaConsulta.Value.Rol);
    }

    [Theory]
    [Trait("Especificacion", "ESP-0034")]
    [Trait("Tecnica", "T-001")]
    [InlineData(Rol.Administrador)]
    [InlineData(Rol.Tecnico)]
    public void ConsultarPorRolExcluyeLasCuentasDelOtroRol(Rol rol)
    {
        var repositorio = new RepositorioDeUsuariosFalso();
        repositorio.Agregar("admin_prueba", Rol.Administrador);
        repositorio.Agregar("tecnico_prueba", Rol.Tecnico);
        var cuenta = Assert.Single(Servicio(repositorio).Consultar(null, rol));
        Assert.Equal(rol, cuenta.Rol);
        Assert.Equal(rol, repositorio.UltimaConsulta!.Value.Rol);
    }

    [Fact]
    [Trait("Especificacion", "ESP-0034")]
    [Trait("Tecnica", "T-001")]
    public void ConsultarSinCoincidenciasDevuelveUnaListaVacia()
    {
        var repositorio = new RepositorioDeUsuariosFalso();
        repositorio.Agregar("admin_prueba", Rol.Administrador);
        Assert.Empty(Servicio(repositorio).Consultar("inexistente", null));
    }

    [Fact]
    [Trait("Especificacion", "ESP-0034")]
    [Trait("Tecnica", "T-012")]
    public void ConsultarNoExponeMiembrosPublicosDeContrasenaNiHash()
    {
        var repositorio = new RepositorioDeUsuariosFalso();
        repositorio.Agregar("carlos_admin", Rol.Administrador, hash: "HASH_SECRETO_123");
        var cuenta = Assert.Single(Servicio(repositorio).Consultar("carlos_admin", Rol.Administrador));
        var tipo = cuenta.GetType();
        var nombres = tipo.GetProperties().Select(propiedad => propiedad.Name)
            .Concat(tipo.GetFields().Select(campo => campo.Name));

        Assert.DoesNotContain(nombres, nombre =>
            nombre.Contains("contras", StringComparison.OrdinalIgnoreCase)
            || nombre.Contains("hash", StringComparison.OrdinalIgnoreCase)
            || nombre.Contains("password", StringComparison.OrdinalIgnoreCase));
        Assert.Equal("carlos_admin", cuenta.NombreUsuario);
    }

    [Fact]
    [Trait("Especificacion", "ESP-0034")]
    [Trait("Tecnica", "T-004")]
    public void ConsultarDistingueElEstadoActivoEInactivo()
    {
        var repositorio = new RepositorioDeUsuariosFalso();
        repositorio.Agregar("activo", Rol.Tecnico);
        repositorio.Agregar("inactivo", Rol.Tecnico, EstadoRegistro.Inactivo);
        var cuentas = Servicio(repositorio).Consultar(null, null);
        Assert.Equal(EstadoRegistro.Activo, cuentas.Single(c => c.NombreUsuario == "activo").Estado);
        Assert.Equal(EstadoRegistro.Inactivo, cuentas.Single(c => c.NombreUsuario == "inactivo").Estado);
    }

    [Fact]
    [Trait("Especificacion", "ESP-0035")]
    [Trait("Tecnica", "T-002")]
    public void ActualizarModificaLosCamposDeUnaCuentaAjena()
    {
        var repositorio = new RepositorioDeUsuariosFalso();
        repositorio.Agregar("admin_prueba", Rol.Administrador);
        var id = repositorio.Agregar("usuario_original", Rol.Tecnico);
        Servicio(repositorio).Actualizar(id, "usuario_modificado", null, Rol.Administrador, EstadoRegistro.Inactivo);
        var cuenta = repositorio.Buscar(id)!;
        Assert.Equal(id, repositorio.UltimoActualizar!.Value.IdUsuario);
        Assert.Equal("usuario_modificado", cuenta.NombreUsuario);
        Assert.Equal(Rol.Administrador, cuenta.Rol);
        Assert.Equal(EstadoRegistro.Inactivo, cuenta.Estado);
    }

    [Fact]
    [Trait("Especificacion", "ESP-0035")]
    [Trait("Tecnica", "T-004")]
    public void ActualizarConContrasenaNulaConservaElHashAnterior()
    {
        var repositorio = new RepositorioDeUsuariosFalso();
        repositorio.Agregar("admin_prueba", Rol.Administrador);
        var id = repositorio.Agregar("tecnico", Rol.Tecnico, hash: "HASH-VIGENTE");
        Servicio(repositorio).Actualizar(id, "tecnico", null, Rol.Tecnico, EstadoRegistro.Activo);
        Assert.False(repositorio.SePidioCambiarHash);
        Assert.Equal("HASH-VIGENTE", repositorio.Buscar(id)!.Hash);
    }

    [Fact]
    [Trait("Especificacion", "ESP-0035")]
    [Trait("Tecnica", "T-012")]
    public void ActualizarConContrasenaNuevaReemplazaElHashAnterior()
    {
        var repositorio = new RepositorioDeUsuariosFalso();
        repositorio.Agregar("admin_prueba", Rol.Administrador);
        var id = repositorio.Agregar("tecnico", Rol.Tecnico, hash: HashDeContrasenasFalso.HashDe("Anterior-123"));
        Servicio(repositorio).Actualizar(id, "tecnico", "Nueva-456", Rol.Tecnico, EstadoRegistro.Activo);
        var hash = repositorio.Buscar(id)!.Hash;
        Assert.True(repositorio.SePidioCambiarHash);
        Assert.Equal(HashDeContrasenasFalso.HashDe("Nueva-456"), hash);
        var verificador = new HashDeContrasenasFalso();
        Assert.True(verificador.Verificar("Nueva-456", hash));
        Assert.False(verificador.Verificar("Anterior-123", hash));
    }

    [Fact]
    [Trait("Especificacion", "ESP-0035")]
    [Trait("Tecnica", "T-012")]
    public void ActualizarRechazaDesactivarALaUltimaAdministradoraAjenaALaSesion()
    {
        var repositorio = new RepositorioDeUsuariosFalso();
        repositorio.Agregar("admin_prueba", Rol.Administrador, EstadoRegistro.Inactivo);
        var id = repositorio.Agregar("unica_admin", Rol.Administrador);
        var error = Assert.Throws<ReglaDeNegocioException>(
            () => Servicio(repositorio).Actualizar(id, "unica_admin", null, Rol.Administrador, EstadoRegistro.Inactivo));
        Assert.Equal(MensajesUsuarios.UltimoAdministrador, error.Message);
        Assert.Equal(id, repositorio.UltimoContarAdministradoresActivos);
        Assert.Null(repositorio.UltimoActualizar);
        Assert.Equal(EstadoRegistro.Activo, repositorio.Buscar(id)!.Estado);
    }

    [Fact]
    [Trait("Especificacion", "ESP-0035")]
    [Trait("Tecnica", "T-003")]
    public void ActualizarPermiteDegradarUnaCuentaSiQuedaOtraAdministradoraActiva()
    {
        var repositorio = new RepositorioDeUsuariosFalso();
        repositorio.Agregar("admin_prueba", Rol.Administrador);
        var id = repositorio.Agregar("otra_admin", Rol.Administrador);
        Servicio(repositorio).Actualizar(id, "otra_admin", null, Rol.Tecnico, EstadoRegistro.Activo);
        Assert.Equal(Rol.Tecnico, repositorio.Buscar(id)!.Rol);
    }

    [Theory]
    [Trait("Especificacion", "ESP-0035")]
    [Trait("Tecnica", "T-012")]
    [InlineData(Rol.Tecnico, EstadoRegistro.Activo)]
    [InlineData(Rol.Administrador, EstadoRegistro.Inactivo)]
    public void ActualizarRechazaCambiarRolOEstadoDeLaCuentaPropiaAunqueExistaOtraAdministradora(Rol rol, EstadoRegistro estado)
    {
        var repositorio = new RepositorioDeUsuariosFalso();
        var id = repositorio.Agregar("admin_prueba", Rol.Administrador);
        repositorio.Agregar("otra_admin", Rol.Administrador);
        var original = repositorio.Buscar(id);
        var error = Assert.Throws<ReglaDeNegocioException>(
            () => Servicio(repositorio).Actualizar(id, "admin_prueba", null, rol, estado));
        Assert.Equal(MensajesUsuarios.CuentaPropiaRolEstado, error.Message);
        Assert.Null(repositorio.UltimoActualizar);
        Assert.Equal(original, repositorio.Buscar(id));
    }

    [Fact]
    [Trait("Especificacion", "ESP-0035")]
    [Trait("Tecnica", "T-003")]
    public void ActualizarLaCuentaPropiaPermiteCambiarNombreYContrasena()
    {
        var repositorio = new RepositorioDeUsuariosFalso();
        var id = repositorio.Agregar("admin_prueba", Rol.Administrador);
        Servicio(repositorio).Actualizar(id, "admin_renombrado", "Nueva-456", Rol.Administrador, EstadoRegistro.Activo);
        Assert.Equal("admin_renombrado", repositorio.Buscar(id)!.NombreUsuario);
        Assert.Equal(HashDeContrasenasFalso.HashDe("Nueva-456"), repositorio.Buscar(id)!.Hash);
    }

    [Fact]
    [Trait("Especificacion", "ESP-0036")]
    [Trait("Tecnica", "T-002")]
    public void DarDeBajaEliminaUnaCuentaAjenaSinReferencias()
    {
        var repositorio = new RepositorioDeUsuariosFalso();
        repositorio.Agregar("admin_prueba", Rol.Administrador);
        var id = repositorio.Agregar("tecnico", Rol.Tecnico);
        Assert.Equal(ResultadoBaja.Eliminada, Servicio(repositorio).DarDeBaja(id));
        Assert.Equal(id, repositorio.UltimaBaja);
        Assert.Null(repositorio.Buscar(id));
    }

    [Fact]
    [Trait("Especificacion", "ESP-0036")]
    [Trait("Tecnica", "T-012")]
    public void VerificarBajaRechazaLaCuentaPropiaAntesDePersistir()
    {
        var repositorio = new RepositorioDeUsuariosFalso();
        var id = repositorio.Agregar("admin_prueba", Rol.Administrador);
        var error = Assert.Throws<ReglaDeNegocioException>(() => Servicio(repositorio).VerificarBaja(id));
        Assert.Equal(MensajesUsuarios.CuentaPropiaBaja, error.Message);
        Assert.Null(repositorio.UltimaBaja);
    }

    [Fact]
    [Trait("Especificacion", "ESP-0036")]
    [Trait("Tecnica", "T-003")]
    public void DarDeBajaConReferenciasDevuelveDesactivadaYConservaLaCuenta()
    {
        var repositorio = new RepositorioDeUsuariosFalso();
        repositorio.Agregar("admin_prueba", Rol.Administrador);
        var id = repositorio.Agregar("tecnico", Rol.Tecnico, hash: "HASH-VIGENTE");
        repositorio.MarcarConReferencias(id);
        Assert.Equal(ResultadoBaja.Desactivada, Servicio(repositorio).DarDeBaja(id));
        var cuenta = repositorio.Buscar(id)!;
        Assert.Equal(EstadoRegistro.Inactivo, cuenta.Estado);
        Assert.Equal("tecnico", cuenta.NombreUsuario);
        Assert.Equal("HASH-VIGENTE", cuenta.Hash);
    }

    [Theory]
    [Trait("Especificacion", "RNF-0006")]
    [Trait("Tecnica", "T-012")]
    [InlineData("Crear")]
    [InlineData("Consultar")]
    [InlineData("Actualizar")]
    [InlineData("VerificarBaja")]
    [InlineData("DarDeBaja")]
    public void RolTecnicoRechazaCadaEntradaDeGestionAntesDeAccederAlRepositorio(string operacion)
    {
        var repositorio = new RepositorioDeUsuariosFalso();
        repositorio.Agregar("admin_prueba", Rol.Administrador);
        repositorio.Agregar("tecnico_sesion", Rol.Tecnico);
        var id = repositorio.Agregar("cuenta_ajena", Rol.Tecnico);
        var originales = repositorio.Cuentas.ToArray();
        var servicio = Servicio(repositorio, Rol.Tecnico);
        Action ejecutar = operacion switch
        {
            "Crear" => () => servicio.Crear("nueva", "Clave-123", Rol.Tecnico, EstadoRegistro.Activo),
            "Consultar" => () => servicio.Consultar(null, null),
            "Actualizar" => () => servicio.Actualizar(id, "cuenta_ajena", null, Rol.Tecnico, EstadoRegistro.Activo),
            "VerificarBaja" => () => servicio.VerificarBaja(id),
            "DarDeBaja" => () => servicio.DarDeBaja(id),
            _ => throw new ArgumentOutOfRangeException(nameof(operacion)),
        };
        Assert.Throws<AccesoDenegadoException>(ejecutar);
        Assert.Null(repositorio.UltimaConsulta);
        Assert.Null(repositorio.UltimoExisteNombre);
        Assert.Null(repositorio.UltimoCrear);
        Assert.Null(repositorio.UltimoActualizar);
        Assert.Null(repositorio.UltimaBaja);
        Assert.Equal(originales, repositorio.Cuentas);
    }
}
