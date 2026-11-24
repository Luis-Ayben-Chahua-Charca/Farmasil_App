
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Globalization;
using FarmasilApp.Modelo.Comun;
using FarmasilApp.Modelo.DOC;
using FarmasilApp.Modelo.VEN;
using FarmasilApp.VistaModelo.Comun;

namespace FarmasilApp.VistaModelo.DOC;

/// <summary>
/// VistaModelo de consulta y registro de comprobantes tributarios
/// (ESP-0010 y ESP-0009).
/// </summary>
public sealed class ComprobantesViewModel : ViewModelBase
{
    private readonly ComprobanteServicio _servicio;
    private readonly List<VentaPendienteItem> _todasLasVentas = new();

    private string _tipoComprobanteSeleccionado = "Todos";
    private string _numeroComprobante = string.Empty;
    private DateTime? _fechaEmision;
    private string _fechaEmisionTexto = string.Empty;

    private string _buscarCodigoVenta = string.Empty;
    private DateTime? _fechaFiltroVenta;
    private VentaPendienteItem? _ventaSeleccionada;

    private string _tipoComprobanteRegistro = "Boleta";
    private string _numeroComprobanteRegistro = string.Empty;
    private string _fechaEmisionRegistroTexto =
        DateTime.Today.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture);

    private VentaPendienteItem? _ventaPrevisualizada;
    private bool _previsualizacionVisible;
    private bool _consultando;
    private bool _registrando;

    private string _mensaje = MensajesComprobantes.IndicacionInicial;

    public ComprobantesViewModel(ComprobanteServicio servicio)
    {
        _servicio = servicio
            ?? throw new ArgumentNullException(nameof(servicio));

        TiposComprobante = new ObservableCollection<string>
        {
            "Todos",
            "Boleta",
            "Factura"
        };

        TiposComprobanteRegistro = new ObservableCollection<string>
        {
            "Boleta",
            "Factura"
        };

        Comprobantes = new ObservableCollection<ComprobanteTributario>();
        VentasPendientes = new ObservableCollection<VentaPendienteItem>();

        ConsultarCommand = new RelayCommand(
            Consultar,
            () => !Consultando && !Registrando);

        CargarVentasPendientesCommand = new RelayCommand(
            CargarVentasPendientes,
            () => !Consultando && !Registrando);

        CrearComprobanteCommand = new RelayCommand(
            CrearComprobante,
            () => !Registrando && VentaSeleccionada is not null);

        CancelarEmisionCommand = new RelayCommand(
            CancelarEmision,
            () => PrevisualizacionVisible && !Registrando);

        ConfirmarEmisionCommand = new RelayCommand(
            ConfirmarEmision,
            () => PrevisualizacionVisible
                  && VentaPrevisualizada is not null
                  && !Registrando);

        LimpiarFiltrosVentasCommand = new RelayCommand(
            LimpiarFiltrosVentas);

        CargarVentasPendientes();
    }

    // =========================================================
    // COLECCIONES
    // =========================================================

    public ObservableCollection<string> TiposComprobante { get; }

    public ObservableCollection<string> TiposComprobanteRegistro { get; }

    public ObservableCollection<ComprobanteTributario> Comprobantes { get; }

    public ObservableCollection<VentaPendienteItem> VentasPendientes { get; }

    // =========================================================
    // COMANDOS
    // =========================================================

    public RelayCommand ConsultarCommand { get; }

    public RelayCommand CargarVentasPendientesCommand { get; }

    public RelayCommand CrearComprobanteCommand { get; }

    public RelayCommand CancelarEmisionCommand { get; }

    public RelayCommand ConfirmarEmisionCommand { get; }

    public RelayCommand LimpiarFiltrosVentasCommand { get; }

    // =========================================================
    // FILTROS DE CONSULTA DE COMPROBANTES
    // =========================================================

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
                FechaEmisionTexto = value?.ToString(
                    "dd/MM/yyyy",
                    CultureInfo.InvariantCulture) ?? string.Empty;
            }
        }
    }

    public string FechaEmisionTexto
    {
        get => _fechaEmisionTexto;
        set
        {
            if (Establecer(ref _fechaEmisionTexto, value)
                && _fechaEmision is not null
                && value != _fechaEmision.Value.ToString(
                    "dd/MM/yyyy",
                    CultureInfo.InvariantCulture))
            {
                _fechaEmision = null;
                Notificar(nameof(FechaEmision));
            }
        }
    }

    // =========================================================
    // FILTROS DE VENTAS PENDIENTES
    // =========================================================

    public string BuscarCodigoVenta
    {
        get => _buscarCodigoVenta;
        set
        {
            if (Establecer(ref _buscarCodigoVenta, value))
            {
                AplicarFiltrosVentas();
            }
        }
    }

    public DateTime? FechaFiltroVenta
    {
        get => _fechaFiltroVenta;
        set
        {
            if (Establecer(ref _fechaFiltroVenta, value))
            {
                AplicarFiltrosVentas();
            }
        }
    }

    public VentaPendienteItem? VentaSeleccionada
    {
        get => _ventaSeleccionada;
        set
        {
            if (Establecer(ref _ventaSeleccionada, value))
            {
                Notificar(nameof(CodigoVentaSeleccionada));
                Notificar(nameof(MontoTotalSeleccionado));
                Notificar(nameof(FechaVentaSeleccionada));
                Notificar(nameof(DetalleVentaSeleccionada));

                CrearComprobanteCommand.NotificarPuedeEjecutar();
            }
        }
    }

    public string CodigoVentaSeleccionada =>
        VentaSeleccionada?.CodigoVenta ?? string.Empty;

    public decimal? MontoTotalSeleccionado =>
        VentaSeleccionada?.MontoTotal;

    public DateTime? FechaVentaSeleccionada =>
        VentaSeleccionada?.FechaHora;

    public IReadOnlyList<DetalleVentaDto> DetalleVentaSeleccionada =>
        VentaSeleccionada?.Venta.Detalle ?? Array.Empty<DetalleVentaDto>();

    // =========================================================
    // DATOS DEL NUEVO COMPROBANTE
    // =========================================================

    public string TipoComprobanteRegistro
    {
        get => _tipoComprobanteRegistro;
        set => Establecer(ref _tipoComprobanteRegistro, value);
    }

    public string NumeroComprobanteRegistro
    {
        get => _numeroComprobanteRegistro;
        set => Establecer(ref _numeroComprobanteRegistro, value);
    }

    public string FechaEmisionRegistroTexto
    {
        get => _fechaEmisionRegistroTexto;
        set => Establecer(ref _fechaEmisionRegistroTexto, value);
    }

    // =========================================================
    // PREVISUALIZACIÓN DEL COMPROBANTE
    // =========================================================

    public VentaPendienteItem? VentaPrevisualizada
    {
        get => _ventaPrevisualizada;
        private set
        {
            if (Establecer(ref _ventaPrevisualizada, value))
            {
                Notificar(nameof(CodigoVentaPrevisualizada));
                Notificar(nameof(MontoTotalPrevisualizado));
                Notificar(nameof(FechaVentaPrevisualizada));
                Notificar(nameof(DetalleVentaPrevisualizada));

                ConfirmarEmisionCommand.NotificarPuedeEjecutar();
            }
        }
    }

    public string CodigoVentaPrevisualizada =>
        VentaPrevisualizada?.CodigoVenta ?? string.Empty;

    public decimal? MontoTotalPrevisualizado =>
        VentaPrevisualizada?.MontoTotal;

    public DateTime? FechaVentaPrevisualizada =>
        VentaPrevisualizada?.FechaHora;

    public IReadOnlyList<DetalleVentaDto> DetalleVentaPrevisualizada =>
        VentaPrevisualizada?.Venta.Detalle ?? Array.Empty<DetalleVentaDto>();

    public bool PrevisualizacionVisible
    {
        get => _previsualizacionVisible;
        private set
        {
            if (Establecer(ref _previsualizacionVisible, value))
            {
                CancelarEmisionCommand.NotificarPuedeEjecutar();
                ConfirmarEmisionCommand.NotificarPuedeEjecutar();
            }
        }
    }

    // =========================================================
    // ESTADO Y MENSAJES
    // =========================================================

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
                NotificarComandos();
            }
        }
    }

    public bool Registrando
    {
        get => _registrando;
        private set
        {
            if (Establecer(ref _registrando, value))
            {
                NotificarComandos();
            }
        }
    }

    private void NotificarComandos()
    {
        ConsultarCommand.NotificarPuedeEjecutar();
        CargarVentasPendientesCommand.NotificarPuedeEjecutar();
        CrearComprobanteCommand.NotificarPuedeEjecutar();
        CancelarEmisionCommand.NotificarPuedeEjecutar();
        ConfirmarEmisionCommand.NotificarPuedeEjecutar();
    }

    // =========================================================
    // CONSULTAR COMPROBANTES
    // =========================================================

    private void Consultar()
    {
        if (Consultando || Registrando)
            return;

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
                : string.Format(
                    CultureInfo.CurrentCulture,
                    MensajesComprobantes.CantidadEncontrada,
                    resultados.Count);
        }
        catch (ReglaDeNegocioException ex)
        {
            Mensaje = ex.Message;
            Comprobantes.Clear();
        }
        catch (Exception ex)
        {
            Trace.TraceError(
                "[DOC] Consulta de comprobantes: {0}",
                ex);

            Mensaje = MensajesComprobantes.ErrorConsulta;
            Comprobantes.Clear();
        }
        finally
        {
            Consultando = false;
        }
    }

    // =========================================================
    // CARGAR VENTAS PENDIENTES
    // =========================================================

    private void CargarVentasPendientes()
    {
        if (Consultando || Registrando)
            return;

        try
        {
            Consultando = true;

            var resultados = _servicio.ConsultarVentasPendientes();

            _todasLasVentas.Clear();

            foreach (var venta in resultados)
            {
                _todasLasVentas.Add(new VentaPendienteItem(venta));
            }

            AplicarFiltrosVentas();

            Mensaje = resultados.Count == 0
                ? "No hay ventas pendientes de comprobante."
                : $"Se encontraron {resultados.Count} venta(s) pendiente(s).";
        }
        catch (Exception ex)
        {
            Trace.TraceError(
                "[DOC] Carga de ventas pendientes: {0}",
                ex);

            Mensaje = "No se pudieron cargar las ventas pendientes. Verifique la conexión con DB_FARMASIL.";

            _todasLasVentas.Clear();
            VentasPendientes.Clear();
            VentaSeleccionada = null;
        }
        finally
        {
            Consultando = false;
        }
    }

    private void AplicarFiltrosVentas()
    {
        IEnumerable<VentaPendienteItem> resultados = _todasLasVentas;

        if (!string.IsNullOrWhiteSpace(BuscarCodigoVenta))
        {
            resultados = resultados.Where(venta =>
                venta.CodigoVenta.Contains(
                    BuscarCodigoVenta.Trim(),
                    StringComparison.OrdinalIgnoreCase));
        }

        if (FechaFiltroVenta is { } fecha)
        {
            resultados = resultados.Where(venta =>
                venta.FechaHora.Date == fecha.Date);
        }

        var lista = resultados.ToArray();

        VentasPendientes.Clear();

        foreach (var venta in lista)
        {
            VentasPendientes.Add(venta);
        }

        if (VentaSeleccionada is not null
            && !VentasPendientes.Contains(VentaSeleccionada))
        {
            VentaSeleccionada = null;
        }
    }

    private void LimpiarFiltrosVentas()
    {
        BuscarCodigoVenta = string.Empty;
        FechaFiltroVenta = null;
        VentaSeleccionada = null;

        AplicarFiltrosVentas();
    }

    // =========================================================
    // ABRIR PREVISUALIZACIÓN
    // =========================================================

    private void CrearComprobante()
    {
        if (Registrando)
            return;

        if (VentaSeleccionada is null)
        {
            Mensaje = "Seleccione una venta pendiente.";
            return;
        }

        VentaPrevisualizada = VentaSeleccionada;
        PrevisualizacionVisible = true;
    }

    // =========================================================
    // CANCELAR EMISIÓN
    // =========================================================

    private void CancelarEmision()
    {
        if (Registrando)
            return;

        PrevisualizacionVisible = false;
        VentaPrevisualizada = null;
    }

    // =========================================================
    // CONFIRMAR Y REGISTRAR
    // =========================================================

    private void ConfirmarEmision()
    {
        if (Registrando || VentaPrevisualizada is null)
            return;

        try
        {
            Registrando = true;

            _servicio.Registrar(
                TipoComprobanteRegistro,
                NumeroComprobanteRegistro,
                FechaEmisionRegistroTexto,
                VentaPrevisualizada.Venta.IdVenta);

            var ventaRegistrada = VentaPrevisualizada;

            PrevisualizacionVisible = false;
            VentaPrevisualizada = null;

            _todasLasVentas.Remove(ventaRegistrada);
            VentasPendientes.Remove(ventaRegistrada);

            VentaSeleccionada = null;
            NumeroComprobanteRegistro = string.Empty;

            AplicarFiltrosVentas();

            // ESP-0009: tras registrar se actualiza también DOC-TBL-COMPROBANTES con los filtros vigentes.
            Registrando = false;
            Consultar();

            Mensaje = MensajesComprobantes.RegistroCorrecto;
        }
        catch (ReglaDeNegocioException ex)
        {
            Mensaje = ex.Message;
        }
        catch (Exception ex)
        {
            Trace.TraceError(
                "[DOC] Registro de comprobante: {0}",
                ex);

            Mensaje = "No se pudo registrar el comprobante. Verifique la conexión con DB_FARMASIL.";
        }
        finally
        {
            Registrando = false;
        }
    }

    // =========================================================
    // ELEMENTO DE VENTA PARA LA VISTA
    // =========================================================

    public sealed class VentaPendienteItem
    {
        public VentaPendienteItem(VentaDto venta)
        {
            Venta = venta
                ?? throw new ArgumentNullException(nameof(venta));
        }

        public VentaDto Venta { get; }

        public int IdVenta => Venta.IdVenta;

        public string CodigoVenta =>
            $"VEN-{Venta.FechaHora:yyyy}-{Venta.IdVenta:000}";

        public DateTime FechaHora => Venta.FechaHora;

        public decimal MontoTotal => Venta.MontoTotal;
    }
}
