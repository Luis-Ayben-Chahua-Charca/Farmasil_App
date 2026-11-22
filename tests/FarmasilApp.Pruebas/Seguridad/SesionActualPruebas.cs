using FarmasilApp.Modelo.Comun;
using FarmasilApp.Modelo.Seguridad;
using FarmasilApp.Pruebas.USR;

namespace FarmasilApp.Pruebas.Seguridad;

/// <summary>
/// Inicio y cierre de sesión (#88; RNF-0006, condición 1). Se prueban con un repositorio en memoria y un hash de
/// prueba, salvo la prueba que verifica el hash real de los datos de prueba.
/// </summary>
[Trait("Especificacion", "RNF-0006")]
[Trait("Tecnica", "T-012")]
public class SesionActualPruebas
{
    private const string Contrasena = "Farmasil2026!";

    private readonly RepositorioDeUsuariosFalso _repositorio = new();
    private readonly HashContadorFalso _hash = new();

    private SesionActual CrearSesion() => new(_repositorio, _hash);

    private int AgregarCuenta(string nombre, Rol rol, EstadoRegistro estado = EstadoRegistro.Activo) =>
        _repositorio.Agregar(nombre, rol, estado, HashDeContrasenasFalso.HashDe(Contrasena));

    [Fact]
    public void SinIniciarSesionNoHaySesionYElUsuarioActualSeRechaza()
    {
        var sesion = CrearSesion();

        Assert.False(sesion.HaySesion);
        var excepcion = Assert.Throws<AccesoDenegadoException>(() => sesion.UsuarioActual);
        Assert.Equal(MensajesSesion.SinSesion, excepcion.Message);
        Assert.Throws<AccesoDenegadoException>(() => sesion.ExigirRol(Rol.Administrador));
    }

    [Fact]
    public void ConCredencialesValidasLaSesionExponeUsuarioYRol()
    {
        var id = AgregarCuenta("ana", Rol.Administrador);
        var sesion = CrearSesion();

        sesion.IniciarSesion("ana", Contrasena);

        Assert.True(sesion.HaySesion);
        Assert.Equal(id, sesion.UsuarioActual.IdUsuario);
        Assert.Equal("ana", sesion.UsuarioActual.NombreUsuario);
        Assert.Equal(Rol.Administrador, sesion.UsuarioActual.Rol);
        sesion.ExigirRol(Rol.Administrador);
        Assert.Throws<AccesoDenegadoException>(() => sesion.ExigirRol(Rol.Tecnico));
    }

    [Fact]
    public void ElTecnicoIniciaSesionConSuRol()
    {
        AgregarCuenta("tomas", Rol.Tecnico);
        var sesion = CrearSesion();

        sesion.IniciarSesion("tomas", Contrasena);

        Assert.Equal(Rol.Tecnico, sesion.UsuarioActual.Rol);
        sesion.ExigirRol(Rol.Tecnico);
        Assert.Throws<AccesoDenegadoException>(() => sesion.ExigirRol(Rol.Administrador));
    }

    [Fact]
    public void ElNombreSeRecortaYNoDistingueMayusculas()
    {
        var id = AgregarCuenta("Ana", Rol.Administrador);
        var sesion = CrearSesion();

        sesion.IniciarSesion("  ANA ", Contrasena);

        Assert.Equal(id, sesion.UsuarioActual.IdUsuario);
    }

    [Fact]
    public void ConContrasenaIncorrectaSeRechazaYNoSeAbreSesion()
    {
        AgregarCuenta("ana", Rol.Administrador);
        var sesion = CrearSesion();

        var excepcion = Assert.Throws<ReglaDeNegocioException>(() => sesion.IniciarSesion("ana", "otra-contrasena"));

        Assert.Equal(MensajesSesion.CredencialesIncorrectas, excepcion.Message);
        Assert.False(sesion.HaySesion);
    }

    [Fact]
    public void ConUsuarioInexistenteDaElMismoMensajeYTambienCalculaElHash()
    {
        AgregarCuenta("ana", Rol.Administrador);
        var sesion = CrearSesion();

        var excepcion = Assert.Throws<ReglaDeNegocioException>(() => sesion.IniciarSesion("nadie", Contrasena));

        Assert.Equal(MensajesSesion.CredencialesIncorrectas, excepcion.Message);
        Assert.False(sesion.HaySesion);
        // El tiempo no revela si el usuario existe: se verifica contra un hash ficticio.
        Assert.Equal(1, _hash.Verificaciones);
    }

    [Fact]
    public void UnaCuentaInactivaConLaContrasenaCorrectaNoIniciaSesion()
    {
        AgregarCuenta("baja", Rol.Tecnico, EstadoRegistro.Inactivo);
        var sesion = CrearSesion();

        var excepcion = Assert.Throws<ReglaDeNegocioException>(() => sesion.IniciarSesion("baja", Contrasena));

        Assert.Equal(MensajesSesion.CuentaInactiva, excepcion.Message);
        Assert.False(sesion.HaySesion);
    }

    [Fact]
    public void UnaCuentaInactivaConContrasenaErroneaNoRevelaQueEstaInactiva()
    {
        AgregarCuenta("baja", Rol.Tecnico, EstadoRegistro.Inactivo);
        var sesion = CrearSesion();

        var excepcion = Assert.Throws<ReglaDeNegocioException>(() => sesion.IniciarSesion("baja", "incorrecta"));

        Assert.Equal(MensajesSesion.CredencialesIncorrectas, excepcion.Message);
    }

    [Theory]
    [InlineData(null, null)]
    [InlineData("", Contrasena)]
    [InlineData("   ", Contrasena)]
    [InlineData("ana", null)]
    [InlineData("ana", "")]
    public void LosDatosVaciosSeRechazanSinConsultarLaBase(string? usuario, string? contrasena)
    {
        AgregarCuenta("ana", Rol.Administrador);
        var sesion = CrearSesion();

        var excepcion = Assert.Throws<ReglaDeNegocioException>(() => sesion.IniciarSesion(usuario, contrasena));

        Assert.Equal(MensajesSesion.CredencialesIncorrectas, excepcion.Message);
        Assert.Equal(0, _repositorio.ConsultasDeCredenciales);
    }

    [Fact]
    public void UnInicioFallidoNoCierraLaSesionVigente()
    {
        AgregarCuenta("ana", Rol.Administrador);
        var sesion = CrearSesion();
        sesion.IniciarSesion("ana", Contrasena);

        Assert.Throws<ReglaDeNegocioException>(() => sesion.IniciarSesion("ana", "incorrecta"));

        Assert.True(sesion.HaySesion);
        Assert.Equal("ana", sesion.UsuarioActual.NombreUsuario);
    }

    [Fact]
    public void UnNuevoInicioReemplazaLaSesionVigente()
    {
        AgregarCuenta("ana", Rol.Administrador);
        AgregarCuenta("tomas", Rol.Tecnico);
        var sesion = CrearSesion();
        sesion.IniciarSesion("ana", Contrasena);

        sesion.IniciarSesion("tomas", Contrasena);

        Assert.Equal("tomas", sesion.UsuarioActual.NombreUsuario);
    }

    [Fact]
    public void CerrarSesionBorraLaSesionYLosModulosVuelvenARechazar()
    {
        AgregarCuenta("ana", Rol.Administrador);
        var sesion = CrearSesion();
        sesion.IniciarSesion("ana", Contrasena);

        sesion.CerrarSesion();

        Assert.False(sesion.HaySesion);
        Assert.Throws<AccesoDenegadoException>(() => sesion.UsuarioActual);
        sesion.CerrarSesion(); // Sin sesión no hace nada.
    }

    [Fact]
    public void UnErrorDeLaBaseSePropagaSinAbrirSesion()
    {
        AgregarCuenta("ana", Rol.Administrador);
        _repositorio.FallaAlObtenerCredenciales = true;
        var sesion = CrearSesion();

        Assert.Throws<InvalidOperationException>(() => sesion.IniciarSesion("ana", Contrasena));

        Assert.False(sesion.HaySesion);
    }

    [Theory]
    [InlineData("admin_prueba", Rol.Administrador)]
    [InlineData("tecnico_prueba", Rol.Tecnico)]
    public void LosHashesDeLosDatosDePruebaSeVerificanConElHashReal(string usuario, Rol rol)
    {
        // Formato y valores de database/002_datos_prueba.sql, cuya contraseña documentada es Farmasil2026!.
        var hashes = new Dictionary<string, string>
        {
            ["admin_prueba"] = "PBKDF2-SHA256$100000$RkFSTUFTSUwtQURNSU4tMDE=$+LkYPDp3ujISJg57fcX4TI4mONdCJVKbJKmrKWl5nzo=",
            ["tecnico_prueba"] = "PBKDF2-SHA256$100000$RkFSTUFTSUwtVEVDTklDTzE=$nrqRBgiwG5d0X78IdfcJNAFb2Pnc75NJ8jOAf1l3rBs=",
        };
        _repositorio.Agregar(usuario, rol, EstadoRegistro.Activo, hashes[usuario]);
        var sesion = new SesionActual(_repositorio, new HashContrasenasPbkdf2());

        sesion.IniciarSesion(usuario, Contrasena);

        Assert.Equal(rol, sesion.UsuarioActual.Rol);
        Assert.Throws<ReglaDeNegocioException>(() => sesion.IniciarSesion(usuario, "otra"));
    }

    [Fact]
    public void ElMensajeDeRechazoNoIncluyeLaContrasena()
    {
        AgregarCuenta("ana", Rol.Administrador);
        var sesion = CrearSesion();

        var excepcion = Assert.Throws<ReglaDeNegocioException>(() => sesion.IniciarSesion("ana", "SecretoXYZ"));

        Assert.DoesNotContain("SecretoXYZ", excepcion.Message);
    }

    /// <summary>Hash de prueba que cuenta las verificaciones para comprobar el hash ficticio del usuario inexistente.</summary>
    private sealed class HashContadorFalso : IHashContrasenas
    {
        public int Verificaciones { get; private set; }

        public string Generar(string contrasena) => HashDeContrasenasFalso.HashDe(contrasena);

        public bool Verificar(string contrasena, string almacenado)
        {
            Verificaciones++;
            return almacenado == HashDeContrasenasFalso.HashDe(contrasena);
        }
    }
}
