using FarmasilApp.Modelo.DOC;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;

namespace FarmasilApp.VistaModelo.DOC;

/// <summary>
/// VistaModelo de la consulta de comprobantes tributarios (ESP-0010).
/// </summary>
public sealed class ConsultaComprobantesViewModel : INotifyPropertyChanged
{
    private readonly ComprobanteServicio _servicio;
    private string _tipoComprobanteSeleccionado = "Todos";
    private string _numeroComprobante = string.Empty;
    private DateTime? _fechaEmision;
    private string _mensaje = "Seleccione los filtros y presione Consultar. También puede consultar sin filtros.";
    private bool _consultando;

    public ConsultaComprobantesViewModel(ComprobanteServicio servicio)
    {
        _servicio = servicio ?? throw new ArgumentNullException(nameof(servicio));

        TiposComprobante = new ObservableCollection<string>
        {
            "Todos",
            "Boleta",
            "Factura"
        };

        Comprobantes = new ObservableCollection<ComprobanteTributario>();
        ConsultarCommand = new RelayCommand(Consultar);
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    public ObservableCollection<string> TiposComprobante { get; }

    public ObservableCollection<ComprobanteTributario> Comprobantes { get; }

    public ICommand ConsultarCommand { get; }

    public string TipoComprobanteSeleccionado
    {
        get => _tipoComprobanteSeleccionado;
        set => SetProperty(ref _tipoComprobanteSeleccionado, value);
    }

    public string NumeroComprobante
    {
        get => _numeroComprobante;
        set => SetProperty(ref _numeroComprobante, value);
    }

    public DateTime? FechaEmision
    {
        get => _fechaEmision;
        set => SetProperty(ref _fechaEmision, value);
    }

    public string Mensaje
    {
        get => _mensaje;
        private set => SetProperty(ref _mensaje, value);
    }

    public bool Consultando
    {
        get => _consultando;
        private set => SetProperty(ref _consultando, value);
    }

    private void Consultar()
    {
        if (Consultando)
        {
            return;
        }

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
        catch (Exception)
        {
            Mensaje = "No se pudo realizar la consulta. Verifique la conexión con DB_FARMASIL.";
            Comprobantes.Clear();
        }
        finally
        {
            Consultando = false;
        }
    }

    private bool SetProperty<T>(ref T campo, T valor, [CallerMemberName] string? nombre = null)
    {
        if (EqualityComparer<T>.Default.Equals(campo, valor))
        {
            return false;
        }

        campo = valor;
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nombre));
        return true;
    }

    private sealed class RelayCommand : ICommand
    {
        private readonly Action _ejecutar;

        public RelayCommand(Action ejecutar)
        {
            _ejecutar = ejecutar ?? throw new ArgumentNullException(nameof(ejecutar));
        }

        public event EventHandler? CanExecuteChanged;

        public bool CanExecute(object? parameter) => true;

        public void Execute(object? parameter) => _ejecutar();
    }
}
