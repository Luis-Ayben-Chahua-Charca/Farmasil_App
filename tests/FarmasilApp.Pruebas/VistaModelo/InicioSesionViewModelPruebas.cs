using System.Diagnostics;
using FarmasilApp.Modelo.Comun;
using FarmasilApp.Modelo.Seguridad;
using FarmasilApp.VistaModelo.USR;

namespace FarmasilApp.Pruebas.VistaModelo;

/// <summary>Pantalla de inicio de sesión (#88; RNF-0006, condiciones 1 y 5).</summary>
[Trait("Especificacion", "RNF-0006")]
[Trait("Tecnica", "T-012")]
public class InicioSesionViewModelPruebas
{
    [Fact]
    public async Task ConCredencialesValidasAvisaQueLaSesionSeIniciaYBorraLaContrasena()
    {
        var autenticacion = new AutenticacionFalsa();
        var vistaModelo = new InicioSesionViewModel(autenticacion) { NombreUsuario = "ana", Contrasena = "Secreta1" };
        var avisos = 0;
        vistaModelo.SesionIniciada += (_, _) => avisos++;

        await vistaModelo.IniciarSesionCommand.ExecuteAsync();

        Assert.Equal(("ana", "Secreta1"), autenticacion.UltimoIntento);
        Assert.Equal(1, avisos);
        Assert.Equal(string.Empty, vistaModelo.Mensaje);
        Assert.Equal(string.Empty, vistaModelo.Contrasena);
    }

    [Theory]
    [InlineData(MensajesSesion.CredencialesIncorrectas)]
    [InlineData(MensajesSesion.CuentaInactiva)]
    public async Task UnRechazoMuestraElMensajeDelModeloBorraLaContrasenaYNoAvisa(string mensaje)
    {
        var autenticacion = new AutenticacionFalsa { Fallo = new ReglaDeNegocioException(mensaje) };
        var vistaModelo = new InicioSesionViewModel(autenticacion) { NombreUsuario = "ana", Contrasena = "Secreta1" };
        var avisos = 0;
        vistaModelo.SesionIniciada += (_, _) => avisos++;

        await vistaModelo.IniciarSesionCommand.ExecuteAsync();

        Assert.Equal(mensaje, vistaModelo.Mensaje);
        Assert.Equal(string.Empty, vistaModelo.Contrasena);
        Assert.Equal("ana", vistaModelo.NombreUsuario);
        Assert.Equal(0, avisos);
    }

    [Fact]
    public async Task UnErrorTecnicoMuestraUnMensajeGenericoYNoEscribeLaContrasenaEnElRegistro()
    {
        var autenticacion = new AutenticacionFalsa { Fallo = new IOException("Acceso denegado al archivo Secreto.mdf") };
        var vistaModelo = new InicioSesionViewModel(autenticacion) { NombreUsuario = "ana", Contrasena = "ClaveSecreta9" };
        var registro = new RegistroDeTrazas();
        Trace.Listeners.Add(registro);
        try
        {
            await vistaModelo.IniciarSesionCommand.ExecuteAsync();
        }
        finally
        {
            Trace.Listeners.Remove(registro);
        }

        Assert.Equal(MensajesSesion.ErrorInicio, vistaModelo.Mensaje);
        Assert.DoesNotContain("Secreto", vistaModelo.Mensaje);
        Assert.Equal(string.Empty, vistaModelo.Contrasena);
        Assert.Contains("[Sesión]", registro.Texto);
        Assert.DoesNotContain("ClaveSecreta9", registro.Texto);
    }

    [Fact]
    public async Task UnNuevoIntentoLimpiaElMensajeAnterior()
    {
        var autenticacion = new AutenticacionFalsa { Fallo = new ReglaDeNegocioException(MensajesSesion.CredencialesIncorrectas) };
        var vistaModelo = new InicioSesionViewModel(autenticacion) { NombreUsuario = "ana", Contrasena = "x" };
        await vistaModelo.IniciarSesionCommand.ExecuteAsync();

        autenticacion.Fallo = null;
        vistaModelo.Contrasena = "Secreta1";
        await vistaModelo.IniciarSesionCommand.ExecuteAsync();

        Assert.Equal(string.Empty, vistaModelo.Mensaje);
    }

    [Fact]
    public async Task MientrasSeVerificaLosCamposQuedanInhabilitadosYDespuesSeHabilitan()
    {
        var autenticacion = new AutenticacionFalsa();
        var vistaModelo = new InicioSesionViewModel(autenticacion) { NombreUsuario = "ana", Contrasena = "Secreta1" };
        autenticacion.AlIniciar = () => (vistaModelo.EstaVerificando, vistaModelo.PuedeEditar);

        Assert.True(vistaModelo.PuedeEditar);
        await vistaModelo.IniciarSesionCommand.ExecuteAsync();

        Assert.Equal((true, false), autenticacion.VistoDuranteElIntento);
        Assert.False(vistaModelo.EstaVerificando);
        Assert.True(vistaModelo.PuedeEditar);
    }

    [Fact]
    public async Task SiElIntentoFallaLosCamposVuelvenAHabilitarse()
    {
        var autenticacion = new AutenticacionFalsa { Fallo = new IOException("Base no disponible") };
        var vistaModelo = new InicioSesionViewModel(autenticacion) { NombreUsuario = "ana", Contrasena = "x" };

        await vistaModelo.IniciarSesionCommand.ExecuteAsync();

        Assert.False(vistaModelo.EstaVerificando);
        Assert.True(vistaModelo.PuedeEditar);
    }

    [Fact]
    public async Task LaContrasenaSeOcultaPorDefectoYVuelveAOcultarseTrasCadaIntento()
    {
        var vistaModelo = new InicioSesionViewModel(new AutenticacionFalsa()) { NombreUsuario = "ana", Contrasena = "x" };
        Assert.False(vistaModelo.ContrasenaVisible);

        vistaModelo.ContrasenaVisible = true;
        await vistaModelo.IniciarSesionCommand.ExecuteAsync();

        Assert.False(vistaModelo.ContrasenaVisible);
    }

    [Fact]
    public void ElConstructorRechazaUnServicioNulo()
    {
        Assert.Throws<ArgumentNullException>(() => new InicioSesionViewModel(null!));
    }

    private sealed class AutenticacionFalsa : IAutenticacion
    {
        public Exception? Fallo { get; set; }

        public Func<(bool, bool)>? AlIniciar { get; set; }

        public (bool, bool)? VistoDuranteElIntento { get; private set; }

        public (string? Usuario, string? Contrasena)? UltimoIntento { get; private set; }

        public bool HaySesion { get; private set; }

        public void IniciarSesion(string? nombreUsuario, string? contrasena)
        {
            UltimoIntento = (nombreUsuario, contrasena);
            VistoDuranteElIntento = AlIniciar?.Invoke();
            if (Fallo is not null)
            {
                throw Fallo;
            }

            HaySesion = true;
        }

        public void CerrarSesion() => HaySesion = false;
    }

    private sealed class RegistroDeTrazas : TraceListener
    {
        private readonly System.Text.StringBuilder _texto = new();

        public string Texto => _texto.ToString();

        public override void Write(string? mensaje) => _texto.Append(mensaje);

        public override void WriteLine(string? mensaje) => _texto.AppendLine(mensaje);
    }
}
