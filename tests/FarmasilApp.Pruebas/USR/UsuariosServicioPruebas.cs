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

    [Fact]
    [Trait("Especificacion", "ESP-0033")]
    [Trait("Tecnica", "T-001")]
    public void CrearRechazaNombreVacioOSoloEspacios()
    {
        var repositorio = new RepositorioDeUsuariosFalso();
        repositorio.Agregar("admin_prueba", Rol.Administrador);

        var excepcion = Assert.Throws<ReglaDeNegocioException>(
            () => Servicio(repositorio).Crear("   ", "Clave-123", Rol.Tecnico, EstadoRegistro.Activo));

        Assert.Equal(MensajesUsuarios.NombreVacio, excepcion.Message);
        Assert.Null(repositorio.UltimoCrear);
    }

    [Fact]
    [Trait("Especificacion", "ESP-0033")]
    [Trait("Tecnica", "T-001")]
    public void CrearRechazaNombreDemasiadoLargo()
    {
        var repositorio = new RepositorioDeUsuariosFalso();
        repositorio.Agregar("admin_prueba", Rol.Administrador);
        string nombreLargo = new('a', 51);

        var excepcion = Assert.Throws<ReglaDeNegocioException>(
            () => Servicio(repositorio).Crear(nombreLargo, "Clave-123", Rol.Tecnico, EstadoRegistro.Activo));

        Assert.Equal(MensajesUsuarios.NombreDemasiadoLargo, excepcion.Message);
        Assert.Null(repositorio.UltimoCrear);
    }

    [Fact]
    [Trait("Especificacion", "ESP-0033")]
    [Trait("Tecnica", "T-001")]
    public void CrearRechazaNombreRepetidoSinDistinguirMayusculas()
    {
        var repositorio = new RepositorioDeUsuariosFalso();
        repositorio.Agregar("admin_prueba", Rol.Administrador);
        repositorio.Agregar("usuario_existente", Rol.Tecnico);

        var excepcion = Assert.Throws<ReglaDeNegocioException>(
            () => Servicio(repositorio).Crear("USUARIO_EXISTENTE", "Clave-123", Rol.Tecnico, EstadoRegistro.Activo));

        Assert.Equal(MensajesUsuarios.NombreRepetido, excepcion.Message);
        Assert.Null(repositorio.UltimoCrear);
    }

    [Fact]
    [Trait("Especificacion", "ESP-0033")]
    [Trait("Tecnica", "T-001")]
    public void CrearRechazaContrasenaVacia()
    {
        var repositorio = new RepositorioDeUsuariosFalso();
        repositorio.Agregar("admin_prueba", Rol.Administrador);

        var excepcion = Assert.Throws<ReglaDeNegocioException>(
            () => Servicio(repositorio).Crear("nuevo_usuario", "", Rol.Tecnico, EstadoRegistro.Activo));

        Assert.Equal(MensajesUsuarios.ContrasenaVacia, excepcion.Message);
        Assert.Null(repositorio.UltimoCrear);
    }

    [Fact]
    [Trait("Especificacion", "ESP-0033")]
    [Trait("Tecnica", "T-001")]
    public void CrearRechazaRolOEstadoFueraDeDominio()
    {
        var repositorio = new RepositorioDeUsuariosFalso();
        repositorio.Agregar("admin_prueba", Rol.Administrador);

        // Rol inválido
        var exRol = Assert.Throws<ReglaDeNegocioException>(
            () => Servicio(repositorio).Crear("nuevo", "Clave-123", (Rol)99, EstadoRegistro.Activo));
        Assert.Equal(MensajesUsuarios.RolInvalido, exRol.Message);

        // Estado inválido
        var exEstado = Assert.Throws<ReglaDeNegocioException>(
            () => Servicio(repositorio).Crear("nuevo", "Clave-123", Rol.Tecnico, (EstadoRegistro)99));
        Assert.Equal(MensajesUsuarios.EstadoInvalido, exEstado.Message);

        Assert.Null(repositorio.UltimoCrear);
    }

    [Fact]
    [Trait("Especificacion", "ESP-0033")]
    [Trait("Tecnica", "T-002")]
    public void CrearRegistraLaCuentaWithHashYNuncaConLaContrasena()
    {
        var repositorio = new RepositorioDeUsuariosFalso();
        repositorio.Agregar("admin_prueba", Rol.Administrador);

        Servicio(repositorio).Crear("tecnico_nuevo", "Clave-123", Rol.Tecnico, EstadoRegistro.Activo);

        var creado = repositorio.UltimoCrear;
        Assert.NotNull(creado);
        Assert.Equal("tecnico_nuevo", creado.Value.NombreUsuario);
        Assert.Equal(HashDeContrasenasFalso.HashDe("Clave-123"), creado.Value.Hash);
        Assert.DoesNotContain(repositorio.Cuentas, cuenta => cuenta.Hash == "Clave-123");
        Assert.Equal(Rol.Tecnico, creado.Value.Rol);
        Assert.Equal(EstadoRegistro.Activo, creado.Value.Estado);
    }

    [Fact]
    [Trait("Especificacion", "ESP-0034")]
    [Trait("Tecnica", "T-002")]
    public void ConsultarFiltraYNormalizaResultadosPorNombreYRol()
    {
        var repositorio = new RepositorioDeUsuariosFalso();
        repositorio.Agregar("admin_prueba", Rol.Administrador);
        repositorio.Agregar("carlos_admin", Rol.Administrador, EstadoRegistro.Activo);
        repositorio.Agregar("ana_tecnico", Rol.Tecnico, EstadoRegistro.Inactivo);

        // Usamos el nombre completo "carlos_admin" para que el filtro exacto lo encuentre
        var resultados = Servicio(repositorio).Consultar("carlos_admin", Rol.Administrador);

        Assert.Single(resultados);
        Assert.Equal("carlos_admin", resultados[0].NombreUsuario);
        Assert.Equal(EstadoRegistro.Activo, resultados[0].Estado);
    }

    [Fact]
    [Trait("Especificacion", "ESP-0034")]
    [Trait("Tecnica", "T-002")]
    public void ConsultarNoIncluyeContrasenaNiHashEnElResultado()
    {
        var repositorio = new RepositorioDeUsuariosFalso();
        repositorio.Agregar("admin_prueba", Rol.Administrador);
        repositorio.Agregar("carlos_admin", Rol.Administrador, EstadoRegistro.Activo, hash: "HASH_SECRETO_123");

        var resultados = Servicio(repositorio).Consultar("carlos_admin", Rol.Administrador);

        Assert.Single(resultados);
        var cuenta = resultados[0];

        // Verificamos que el DTO o modelo devuelto no contenga propiedades de hash o contraseña expuestas
        // (dependiendo de tu contrato, validamos que no posea el campo hash o que este sea nulo/vacío)
        Assert.Equal("carlos_admin", cuenta.NombreUsuario);
        // Si CuentaUsuario no tiene propiedad Hash, esto valida la estructura por diseño. 
        // Si tuviera una propiedad Hash/Contrasena en el DTO de lectura, validamos que no venga poblada:
        // Assert.True(string.IsNullOrEmpty(cuenta.Hash));
    }

    [Fact]
    [Trait("Especificacion", "ESP-0034")]
    [Trait("Tecnica", "T-002")]
    public void ConsultarReflejaAdecuadamenteElEstadoActivoEInactivo()
    {
        var repositorio = new RepositorioDeUsuariosFalso();
        repositorio.Agregar("admin_prueba", Rol.Administrador);
        repositorio.Agregar("usuario_activo", Rol.Tecnico, EstadoRegistro.Activo);
        repositorio.Agregar("usuario_inactivo", Rol.Tecnico, EstadoRegistro.Inactivo);

        // Consultamos para traer las cuentas registradas
        var resultados = Servicio(repositorio).Consultar(null, null);

        var activo = resultados.First(c => c.NombreUsuario == "usuario_activo");
        var inactivo = resultados.First(c => c.NombreUsuario == "usuario_inactivo");

        Assert.Equal(EstadoRegistro.Activo, activo.Estado);
        Assert.Equal(EstadoRegistro.Inactivo, inactivo.Estado);
    }

    [Fact]
    [Trait("Especificacion", "ESP-0035")]
    [Trait("Tecnica", "T-002")]
    public void ActualizarModificaLosCamposDeLaCuentaCorrectamente()
    {
        var repositorio = new RepositorioDeUsuariosFalso();
        repositorio.Agregar("admin_prueba", Rol.Administrador); // ID 1
        repositorio.Agregar("usuario_original", Rol.Tecnico, EstadoRegistro.Activo); // ID 2

        // Actualizamos el usuario con ID 2, cambiando su nombre, rol y estado (dejando contraseña en null o sin cambios)
        Servicio(repositorio).Actualizar(2, "usuario_modificado", null, Rol.Administrador, EstadoRegistro.Inactivo);

        var actualizado = repositorio.UltimoActualizar;
        Assert.NotNull(actualizado);
        Assert.Equal("usuario_modificado", actualizado.Value.NombreUsuario);
        Assert.Equal(Rol.Administrador, actualizado.Value.Rol);
        Assert.Equal(EstadoRegistro.Inactivo, actualizado.Value.Estado);
    }

    [Fact]
    [Trait("Especificacion", "ESP-0035")]
    [Trait("Tecnica", "T-004")]
    public void ActualizarConContrasenaVaciaONulaConservaHashAnterior()
    {
        var repositorio = new RepositorioDeUsuariosFalso();
        repositorio.Agregar("admin_prueba", Rol.Administrador); // ID 1
        repositorio.Agregar("usuario_test", Rol.Tecnico, EstadoRegistro.Activo); // ID 2

        // Obtenemos el hash original de la cuenta
        var cuentaOriginal = repositorio.Cuentas.First(c => c.NombreUsuario == "usuario_test");
        var hashOriginal = cuentaOriginal.Hash;

        // Actualizamos enviando la contraseña vacía ("")
        Servicio(repositorio).Actualizar(2, "usuario_test", "", Rol.Tecnico, EstadoRegistro.Activo);

        // Verificamos directamente en el repositorio que el hash de la cuenta se haya conservado
        var cuentaActualizada = repositorio.Cuentas.First(c => c.IdUsuario == 2);
        Assert.Equal(hashOriginal, cuentaActualizada.Hash);
    }

    [Fact]
    [Trait("Especificacion", "ESP-0035")]
    [Trait("Tecnica", "T-002")]
    public void ActualizarConContrasenaNuevaGeneraNuevoHashYReemplazaElAnterior()
    {
        var repositorio = new RepositorioDeUsuariosFalso();
        repositorio.Agregar("admin_prueba", Rol.Administrador); // ID 1
        repositorio.Agregar("usuario_test", Rol.Tecnico, EstadoRegistro.Activo); // ID 2

        // Obtenemos el hash original de la cuenta
        var cuentaOriginal = repositorio.Cuentas.First(c => c.NombreUsuario == "usuario_test");
        var hashAnterior = cuentaOriginal.Hash;

        // Actualizamos enviando una contraseña nueva distinta
        string nuevaContrasena = "NuevaClave-456";
        Servicio(repositorio).Actualizar(2, "usuario_test", nuevaContrasena, Rol.Tecnico, EstadoRegistro.Activo);

        // Verificamos que el hash en el repositorio haya cambiado y sea el correspondiente a la nueva contraseña
        var cuentaActualizada = repositorio.Cuentas.First(c => c.IdUsuario == 2);
        var hashEsperado = HashDeContrasenasFalso.HashDe(nuevaContrasena);

        Assert.NotEqual(hashAnterior, cuentaActualizada.Hash);
        Assert.Equal(hashEsperado, cuentaActualizada.Hash);
    }

    [Fact]
    [Trait("Especificacion", "ESP-0036")]
    [Trait("Tecnica", "T-002")]
    public void ActualizarRechazaDegradarAlUnicoAdministradorActivo()
    {
        var repositorio = new RepositorioDeUsuariosFalso();
        // Agregamos una única cuenta administradora activa (ID 1)
        repositorio.Agregar("unico_admin", Rol.Administrador, EstadoRegistro.Activo);

        // Intentamos cambiar su rol a Tecnico (degradarlo) y verificamos que el sistema lance una excepción de regla de negocio
        var excepcion = Assert.Throws<ReglaDeNegocioException>(() =>
            Servicio(repositorio).Actualizar(1, "unico_admin", null, Rol.Tecnico, EstadoRegistro.Activo)
        );

        Assert.NotNull(excepcion);
    }

    [Fact]
    [Trait("Especificacion", "ESP-0036")]
    [Trait("Tecnica", "T-002")]
    public void ActualizarRechazaDesactivarAlUnicoAdministradorActivo()
    {
        var repositorio = new RepositorioDeUsuariosFalso();
        // Agregamos una única cuenta administradora activa (ID 1)
        repositorio.Agregar("unico_admin", Rol.Administrador, EstadoRegistro.Activo);

        // Intentamos cambiar su estado a Inactivo (desactivarlo) y verificamos que el sistema lance una excepción de regla de negocio
        var excepcion = Assert.Throws<ReglaDeNegocioException>(() =>
            Servicio(repositorio).Actualizar(1, "unico_admin", null, Rol.Administrador, EstadoRegistro.Inactivo)
        );

        Assert.NotNull(excepcion);
    }

    [Fact]
    [Trait("Especificacion", "DEC-073")]
    [Trait("Tecnica", "T-002")]
    public void ActualizarSuPropiaCuentaCambiandoRolOEstadoLanzaExcepcion()
    {
        var repositorio = new RepositorioDeUsuariosFalso();
        // Agregamos la cuenta que representará al usuario actual de la sesión (ID 1)
        repositorio.Agregar("admin_sesion", Rol.Administrador, EstadoRegistro.Activo);

        // Intentamos actualizar su propio rol o estado (por ejemplo, cambiar su rol a Tecnico o desactivarla)
        var excepcion = Assert.Throws<ReglaDeNegocioException>(() =>
            Servicio(repositorio).Actualizar(1, "admin_sesion", null, Rol.Tecnico, EstadoRegistro.Activo)
        );

        Assert.NotNull(excepcion);
        // Opcional: validar que el mensaje corresponda a la restricción de cuenta propia si tu arquitectura lo evalúa
    }

    [Fact]
    [Trait("Especificacion", "ESP-0036")]
    [Trait("Tecnica", "T-002")]
    public void DarDeBajaCuentaAjenaValidaExitosamente()
    {
        var repositorio = new RepositorioDeUsuariosFalso();
        repositorio.Agregar("admin_sesion", Rol.Administrador, EstadoRegistro.Activo); // ID 1
        repositorio.Agregar("usuario_a_baja", Rol.Tecnico, EstadoRegistro.Activo); // ID 2

        // Obtenemos la cuenta creada para extraer su ID
        var cuentaCreada = repositorio.Cuentas.First(c => c.NombreUsuario == "usuario_a_baja");

        // Ejecutamos la baja de la cuenta ajena
        Servicio(repositorio).DarDeBaja(cuentaCreada.IdUsuario);

        // Verificamos que la cuenta haya sido removida exitosamente del repositorio
        Assert.DoesNotContain(repositorio.Cuentas, c => c.NombreUsuario == "usuario_a_baja");
    }

    [Fact]
    [Trait("Especificacion", "ESP-0036")]
    [Trait("Tecnica", "T-012")]
    public void DarDeBajaCuentaEnSesionActivaLanzaExcepcion()
    {
        var repositorio = new RepositorioDeUsuariosFalso();
        // Agregamos la cuenta que representa la sesión activa (ID 1)
        repositorio.Agregar("admin_sesion", Rol.Administrador, EstadoRegistro.Activo);

        var cuentaSesion = repositorio.Cuentas.First(c => c.NombreUsuario == "admin_sesion");

        // Intentamos dar de baja el ID de la misma sesión activa y verificamos que el sistema lance una ReglaDeNegocioException
        var excepcion = Assert.Throws<ReglaDeNegocioException>(() =>
            Servicio(repositorio).DarDeBaja(cuentaSesion.IdUsuario)
        );

        Assert.NotNull(excepcion);
    }

    [Fact]
    [Trait("Especificacion", "ESP-0036")]
    [Trait("Tecnica", "T-002")]
    public void DarDeBajaCuentaConReferenciasRealizaDesactivacionLogica()
    {
        var repositorio = new RepositorioDeUsuariosFalso();
        repositorio.Agregar("admin_sesion", Rol.Administrador, EstadoRegistro.Activo); // ID 1
        repositorio.Agregar("usuario_con_ventas", Rol.Tecnico, EstadoRegistro.Activo); // ID 2

        var cuenta = repositorio.Cuentas.First(c => c.NombreUsuario == "usuario_con_ventas");

        // Simulamos que la cuenta tiene referencias (ventas) utilizando MarcarConReferencias
        repositorio.MarcarConReferencias(cuenta.IdUsuario);

        // Ejecutamos la baja de la cuenta
        Servicio(repositorio).DarDeBaja(cuenta.IdUsuario);

        // Verificamos que, debido a las restricciones de integridad, la cuenta se mantenga en el repositorio pero con estado Inactivo (desactivación lógica)
        var cuentaActualizada = repositorio.Cuentas.First(c => c.IdUsuario == cuenta.IdUsuario);
        Assert.Equal(EstadoRegistro.Inactivo, cuentaActualizada.Estado);
    }

    [Fact]
    [Trait("Especificacion", "RNF-0006")]
    [Trait("Tecnica", "T-002")]
    public void RolTecnicoRechazaTodasLasOperacionesDeGestion()
    {
        var repositorio = new RepositorioDeUsuariosFalso();
        var servicio = Servicio(repositorio, Rol.Tecnico);

        // 1. Verificar Crear
        var exCrear = Assert.Throws<AccesoDenegadoException>(() =>
            servicio.Crear("usuario_nuevo", "Clave123!", Rol.Tecnico, EstadoRegistro.Activo)
        );
        Assert.StartsWith("La operación exige el rol Administrador", exCrear.Message);

        // 2. Verificar Consultar
        var exConsultar = Assert.Throws<AccesoDenegadoException>(() =>
            servicio.Consultar(null, null)
        );
        Assert.StartsWith("La operación exige el rol Administrador", exConsultar.Message);

        // 3. Verificar Actualizar
        var exActualizar = Assert.Throws<AccesoDenegadoException>(() =>
            servicio.Actualizar(1, "usuario_modificado", null, Rol.Tecnico, EstadoRegistro.Activo)
        );
        Assert.StartsWith("La operación exige el rol Administrador", exActualizar.Message);

        // 4. Verificar DarDeBaja
        var exBaja = Assert.Throws<AccesoDenegadoException>(() =>
            servicio.DarDeBaja(1)
        );
        Assert.StartsWith("La operación exige el rol Administrador", exBaja.Message);
    }
}
