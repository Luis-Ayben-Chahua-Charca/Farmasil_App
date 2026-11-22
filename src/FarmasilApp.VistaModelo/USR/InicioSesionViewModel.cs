using System.Diagnostics;
using FarmasilApp.Modelo.Comun;
using FarmasilApp.Modelo.Seguridad;
using FarmasilApp.VistaModelo.Comun;

namespace FarmasilApp.VistaModelo.USR;

/// <summary>
/// VistaModelo de la pantalla de inicio de sesión (RNF-0006, condición 1; DEC-012; VP-00). La pantalla es mínima y no
/// tiene mockup aprobado (DEC-080).
/// </summary>
/// <remarks>
/// <para><b>Qué hace.</b> Toma el usuario y la contraseña, llama a <see cref="IAutenticacion.IniciarSesion"/> y, si la
/// sesión se abre, avisa con <see cref="SesionIniciada"/>; la ventana principal arma entonces el menú del rol. No
/// contiene reglas: verificar la contraseña y el estado de la cuenta es del Modelo.</para>
/// <para><b>Errores (DEC-072).</b> <see cref="ReglaDeNegocioException"/>: se muestra su mensaje. Cualquier otra excepción
/// se registra con <see cref="Trace"/> y se muestra <see cref="MensajesSesion.ErrorInicio"/>.</para>
/// <para><b>Contraseña (RNF-0006, condición 5).</b> Nunca se escribe en <see cref="Trace"/> ni en ningún registro, y se
/// borra de la VistaModelo en cuanto termina el intento, sea correcto o no.</para>
/// </remarks>
public sealed class InicioSesionViewModel : ViewModelBase
{
    private readonly IAutenticacion _autenticacion;

    private string _nombreUsuario = string.Empty;
    private string _contrasena = string.Empty;
    private string _mensaje = string.Empty;

    public InicioSesionViewModel(IAutenticacion autenticacion)
    {
        _autenticacion = autenticacion ?? throw new ArgumentNullException(nameof(autenticacion));

        IniciarSesionCommand = new AsyncRelayCommand(IniciarSesionAsync);
    }

    /// <summary>Se produce cuando la sesión se abrió. Lo escucha la ventana principal.</summary>
    public event EventHandler? SesionIniciada;

    public string Titulo => MensajesSesion.Titulo;

    /// <summary>USR-TXT-USUARIO-SESION.</summary>
    public string NombreUsuario
    {
        get => _nombreUsuario;
        set => Establecer(ref _nombreUsuario, value ?? string.Empty);
    }

    /// <summary>USR-PWD-CONTRASENA-SESION. La Vista la enlaza con el <c>PasswordBox</c> enmascarado.</summary>
    public string Contrasena
    {
        get => _contrasena;
        set => Establecer(ref _contrasena, value ?? string.Empty);
    }

    /// <summary>USR-LBL-ERROR-SESION: motivo del último rechazo; vacío si no hay.</summary>
    public string Mensaje
    {
        get => _mensaje;
        private set => Establecer(ref _mensaje, value);
    }

    /// <summary>USR-BTN-INICIAR-SESION.</summary>
    public AsyncRelayCommand IniciarSesionCommand { get; }

    private async Task IniciarSesionAsync()
    {
        // Se copian los datos antes de salir del hilo de la interfaz: el intento no depende de lo que se escriba después.
        var (nombre, contrasena) = (NombreUsuario, Contrasena);
        Mensaje = string.Empty;

        try
        {
            await Task.Run(() => _autenticacion.IniciarSesion(nombre, contrasena));
        }
        catch (ReglaDeNegocioException ex)
        {
            Mensaje = ex.Message;
            Contrasena = string.Empty;
            return;
        }
        catch (Exception ex)
        {
            Trace.TraceError("[Sesión] Inicio de sesión: {0}", ex);
            Mensaje = MensajesSesion.ErrorInicio;
            Contrasena = string.Empty;
            return;
        }

        Contrasena = string.Empty;
        SesionIniciada?.Invoke(this, EventArgs.Empty);
    }
}
