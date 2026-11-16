namespace FarmasilApp.VistaModelo.Comun;

/// <summary>Datos y comandos del modal común de confirmación de baja.</summary>
public sealed class ConfirmacionDialogViewModel
{
    /// <summary>Título del mockup ART-MKP-CONFIRM-0001, que se usa cuando la especificación no fija otro.</summary>
    public const string TituloPredeterminado = "Confirmación de Acción Crítica";

    public ConfirmacionDialogViewModel(string modulo, string? titulo, string mensaje)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(modulo);
        ArgumentException.ThrowIfNullOrWhiteSpace(mensaje);

        if (modulo.Length != 3 || modulo.Any(caracter => caracter is < 'A' or > 'Z'))
        {
            throw new ArgumentException("El prefijo del módulo debe tener tres letras mayúsculas.", nameof(modulo));
        }

        Titulo = string.IsNullOrWhiteSpace(titulo) ? TituloPredeterminado : titulo;
        Mensaje = mensaje;
        AutomationIdModal = $"{modulo}-MDL-CONFIRMAR-ELIMINACION";
        AutomationIdMensaje = $"{modulo}-TXT-MSJ";
        AutomationIdConfirmar = $"{modulo}-BTN-CONFIRMAR-SI";
        AutomationIdCancelar = $"{modulo}-BTN-CONFIRMAR-NO";

        ConfirmarCommand = new RelayCommand(() => ResultadoSolicitado?.Invoke(true));
        CancelarCommand = new RelayCommand(() => ResultadoSolicitado?.Invoke(false));
    }

    public string Titulo { get; }

    public string Mensaje { get; }

    public string AutomationIdModal { get; }

    public string AutomationIdMensaje { get; }

    public string AutomationIdConfirmar { get; }

    public string AutomationIdCancelar { get; }

    public RelayCommand ConfirmarCommand { get; }

    public RelayCommand CancelarCommand { get; }

    public event Action<bool>? ResultadoSolicitado;
}
