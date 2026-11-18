using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Globalization;
using FarmasilApp.Modelo.Comun;
using FarmasilApp.Modelo.DOC;
using FarmasilApp.VistaModelo.Comun;

namespace FarmasilApp.VistaModelo.DOC;

/// <summary>VistaModelo de la consulta de comprobantes tributarios (ESP-0010).</summary>
public sealed class ComprobantesViewModel : ViewModelBase
{
    private readonly ComprobanteServicio _servicio;
    private string _tipoComprobanteSeleccionado = "Todos";
    private string _numeroComprobante = string.Empty;
    private DateTime? _fechaEmision;
    private string _fechaEmisionTexto = string.Empty;
    private string _mensaje = MensajesComprobantes.IndicacionInicial;
    private bool _consultando;

    public ComprobantesViewModel(ComprobanteServicio servicio)
    {
        _servicio = servicio ?? throw new ArgumentNullException(nameof(servicio));
        TiposComprobante = new ObservableCollection<string> { "Todos", "Boleta", "Factura" };
        Comprobantes = new ObservableCollection<ComprobanteTributario>();
        ConsultarCommand = new RelayCommand(Consultar, () => !Consultando);
    }

    public ObservableCollection<string> TiposComprobante { get; }
    public ObservableCollection<ComprobanteTributario> Comprobantes { get; }
    public RelayCommand ConsultarCommand { get; }

    public string TipoComprobanteSeleccionado
    {
        get => _tipoComprobanteSeleccionado;
        set => Establecer(ref _tipoComprobanteSeleccionado, value);
    }

    public string NumeroComprobante
    {
        get => _numeroComprobante;
        set => Establecer(ref _numeroComprobante, value);
    }

    public DateTime? FechaEmision
    {
        get => _fechaEmision;
        set
        {
            if (Establecer(ref _fechaEmision, value))
            {
                FechaEmisionTexto = value?.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture) ?? string.Empty;
            }
        }
    }

    /// <summary>Entrada sin convertir: una fecha inválida no se confunde con el filtro vacío.</summary>
    public string FechaEmisionTexto
    {
        get => _fechaEmisionTexto;
        set
        {
            if (Establecer(ref _fechaEmisionTexto, value)
                && _fechaEmision is not null
                && value != _fechaEmision.Value.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture))
            {
                // Permite volver a elegir la misma fecha después de editar o borrar el texto.
                _fechaEmision = null;
                Notificar(nameof(FechaEmision));
            }
        }
    }

    public string Mensaje
    {
        get => _mensaje;
        private set => Establecer(ref _mensaje, value);
    }

    public bool Consultando
    {
        get => _consultando;
        private set
        {
            if (Establecer(ref _consultando, value))
            {
                ConsultarCommand.NotificarPuedeEjecutar();
            }
        }
    }

    private void Consultar()
    {
        if (Consultando) return;

        try
        {
            Consultando = true;
            var resultados = _servicio.ConsultarConFechaTexto(
                TipoComprobanteSeleccionado,
                NumeroComprobante,
                FechaEmisionTexto);

            Comprobantes.Clear();
            foreach (var comprobante in resultados)
            {
                Comprobantes.Add(comprobante);
            }

            Mensaje = resultados.Count == 0
                ? MensajesComprobantes.SinResultados
                : string.Format(CultureInfo.CurrentCulture, MensajesComprobantes.CantidadEncontrada, resultados.Count);
        }
        catch (ReglaDeNegocioException ex)
        {
            Mensaje = ex.Message;
            Comprobantes.Clear();
        }
        catch (Exception ex)
        {
            Trace.TraceError("[DOC] Consulta de comprobantes: {0}", ex);
            Mensaje = MensajesComprobantes.ErrorConsulta;
            Comprobantes.Clear();
        }
        finally
        {
            Consultando = false;
        }
    }
}
