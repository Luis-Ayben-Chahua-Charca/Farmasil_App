using System.Collections.ObjectModel;
using System.Diagnostics;
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
    private string _mensaje = "Seleccione los filtros y presione Consultar. También puede consultar sin filtros.";
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
        set => Establecer(ref _fechaEmision, value);
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
            var resultados = _servicio.Consultar(
                TipoComprobanteSeleccionado,
                NumeroComprobante,
                FechaEmision);

            Comprobantes.Clear();
            foreach (var comprobante in resultados)
            {
                Comprobantes.Add(comprobante);
            }

            Mensaje = resultados.Count == 0
                ? "No se encontraron comprobantes con los filtros indicados."
                : $"Se encontraron {resultados.Count} comprobante(s).";
        }
        catch (ReglaDeNegocioException ex)
        {
            Mensaje = ex.Message;
            Comprobantes.Clear();
        }
        catch (Exception ex)
        {
            Trace.TraceError("[DOC] Consulta de comprobantes: {0}", ex);
            Mensaje = "No se pudo realizar la consulta. Verifique la conexión con DB_FARMASIL.";
            Comprobantes.Clear();
        }
        finally
        {
            Consultando = false;
        }
    }
}
