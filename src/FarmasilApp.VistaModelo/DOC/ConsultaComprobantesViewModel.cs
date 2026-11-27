using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using FarmasilApp.Modelo.Comun;
using FarmasilApp.Modelo.DOC;
using FarmasilApp.Modelo.Seguridad;
using FarmasilApp.Modelo.VEN;
using FarmasilApp.VistaModelo.Comun;

namespace FarmasilApp.VistaModelo.DOC;

/// <summary>VistaModelo de consulta, registro, actualización, anulación y previsualización PDF de DOC.</summary>
public sealed class ComprobantesViewModel : ViewModelBase
{
    private readonly ComprobanteServicio _servicio;
    private readonly IPdfServicio _pdfServicio;
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
    private string _fechaEmisionRegistroTexto = DateTime.Today.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture);
    private VentaPendienteItem? _ventaPrevisualizada;
    private ComprobanteTributario? _comprobanteSeleccionado;
    private ComprobanteTributario? _comprobanteEnEdicion;
    private bool _previsualizacionVisible;
    private bool _editandoComprobante;
    private bool _anulacionVisible;
    private bool _consultando;
    private bool _registrando;
    private string _mensaje = MensajesComprobantes.IndicacionInicial;

    public ComprobantesViewModel(
        ComprobanteServicio servicio,
        IPdfServicio pdfServicio,
        ISesion sesion)
    {
        _servicio = servicio ?? throw new ArgumentNullException(nameof(servicio));
        _pdfServicio = pdfServicio ?? throw new ArgumentNullException(nameof(pdfServicio));
        ArgumentNullException.ThrowIfNull(sesion);
        try
        {
            PuedeAdministrar = sesion.UsuarioActual.Rol == Rol.Administrador;
        }
        catch (AccesoDenegadoException)
        {
            // La pantalla también se construye antes del inicio de sesión durante la composición de dependencias.
            PuedeAdministrar = false;
        }

        TiposComprobante = new ObservableCollection<string> { "Todos", "Boleta", "Factura" };
        TiposComprobanteRegistro = new ObservableCollection<string> { "Boleta", "Factura" };
        Comprobantes = new ObservableCollection<ComprobanteTributario>();
        VentasPendientes = new ObservableCollection<VentaPendienteItem>();

        ConsultarCommand = new RelayCommand(Consultar, () => !Consultando && !Registrando);
        CargarVentasPendientesCommand = new RelayCommand(CargarVentasPendientes, () => !Consultando && !Registrando);
        CrearComprobanteCommand = new RelayCommand(
            CrearComprobante,
            () => !Registrando && !EditandoComprobante && VentaSeleccionada is not null);
        CancelarEmisionCommand = new RelayCommand(
            CancelarEmision,
            () => PrevisualizacionVisible && !Registrando);
        ConfirmarEmisionCommand = new RelayCommand(
            ConfirmarEmision,
            () => PrevisualizacionVisible && VentaPrevisualizada is not null && !Registrando);
        DescargarPdfCommand = new RelayCommand(
            DescargarPdf,
            () => PrevisualizacionVisible && VentaPrevisualizada is not null && !Registrando);
        LimpiarFiltrosVentasCommand = new RelayCommand(LimpiarFiltrosVentas);

        ActualizarComprobanteCommand = new RelayCommand(
            IniciarActualizacion,
            () => PuedeAdministrar && EsEmitido(ComprobanteSeleccionado) && !Registrando && !EditandoComprobante);
        ConfirmarActualizacionCommand = new RelayCommand(
            ConfirmarActualizacion,
            () => PuedeAdministrar && EditandoComprobante && ComprobanteEnEdicion is not null && !Registrando);
        CancelarEdicionCommand = new RelayCommand(
            CancelarEdicion,
            () => EditandoComprobante && !Registrando);
        AnularComprobanteCommand = new RelayCommand(
            SolicitarAnulacion,
            () => PuedeAdministrar && EsEmitido(ComprobanteSeleccionado) && !Registrando && !EditandoComprobante);
        ConfirmarAnulacionCommand = new RelayCommand(
            ConfirmarAnulacion,
            () => PuedeAdministrar && AnulacionVisible && ComprobanteSeleccionado is not null && !Registrando);
        CancelarAnulacionCommand = new RelayCommand(
            CancelarAnulacion,
            () => AnulacionVisible && !Registrando);

        CargarVentasPendientes();
    }

    public ObservableCollection<string> TiposComprobante { get; }
    public ObservableCollection<string> TiposComprobanteRegistro { get; }
    public ObservableCollection<ComprobanteTributario> Comprobantes { get; }
    public ObservableCollection<VentaPendienteItem> VentasPendientes { get; }

    public RelayCommand ConsultarCommand { get; }
    public RelayCommand CargarVentasPendientesCommand { get; }
    public RelayCommand CrearComprobanteCommand { get; }
    public RelayCommand CancelarEmisionCommand { get; }
    public RelayCommand ConfirmarEmisionCommand { get; }
    public RelayCommand DescargarPdfCommand { get; }
    public RelayCommand LimpiarFiltrosVentasCommand { get; }
    public RelayCommand ActualizarComprobanteCommand { get; }
    public RelayCommand ConfirmarActualizacionCommand { get; }
    public RelayCommand CancelarEdicionCommand { get; }
    public RelayCommand AnularComprobanteCommand { get; }
    public RelayCommand ConfirmarAnulacionCommand { get; }
    public RelayCommand CancelarAnulacionCommand { get; }

    public bool PuedeAdministrar { get; }
    public bool PuedeSeleccionarVenta => !EditandoComprobante;
    public string VentaIdEnEdicion
    {
        get
        {
            if (ComprobanteEnEdicion is null)
            {
                return string.Empty;
            }

            return string.IsNullOrWhiteSpace(ComprobanteEnEdicion.CodigoVenta)
                ? $"Venta #{ComprobanteEnEdicion.IdVenta}"
                : ComprobanteEnEdicion.CodigoVenta;
        }
    }

    // Filtros de consulta.
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

    public string FechaEmisionTexto
    {
        get => _fechaEmisionTexto;
        set
        {
            if (Establecer(ref _fechaEmisionTexto, value)
                && _fechaEmision is not null
                && value != _fechaEmision.Value.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture))
            {
                _fechaEmision = null;
                Notificar(nameof(FechaEmision));
            }
        }
    }

    // Filtros de ventas pendientes.
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

    public string CodigoVentaSeleccionada => VentaSeleccionada?.CodigoVenta ?? string.Empty;
    public decimal? MontoTotalSeleccionado => VentaSeleccionada?.MontoTotal;
    public DateTime? FechaVentaSeleccionada => VentaSeleccionada?.FechaHora;
    public IReadOnlyList<DetalleVentaDto> DetalleVentaSeleccionada =>
        VentaSeleccionada?.Venta.Detalle ?? Array.Empty<DetalleVentaDto>();

    // Campos compartidos por emisión y edición.
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

    // Previsualización y PDF.
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
                DescargarPdfCommand.NotificarPuedeEjecutar();
            }
        }
    }

    public string CodigoVentaPrevisualizada => VentaPrevisualizada?.CodigoVenta ?? string.Empty;
    public decimal? MontoTotalPrevisualizado => VentaPrevisualizada?.MontoTotal;
    public DateTime? FechaVentaPrevisualizada => VentaPrevisualizada?.FechaHora;
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
                DescargarPdfCommand.NotificarPuedeEjecutar();
            }
        }
    }

    // Edición de comprobante existente.
    public ComprobanteTributario? ComprobanteSeleccionado
    {
        get => _comprobanteSeleccionado;
        set
        {
            if (Establecer(ref _comprobanteSeleccionado, value))
            {
                ActualizarComprobanteCommand.NotificarPuedeEjecutar();
                AnularComprobanteCommand.NotificarPuedeEjecutar();
            }
        }
    }

    public ComprobanteTributario? ComprobanteEnEdicion
    {
        get => _comprobanteEnEdicion;
        private set
        {
            if (Establecer(ref _comprobanteEnEdicion, value))
            {
                Notificar(nameof(VentaIdEnEdicion));
            }
        }
    }

    public bool EditandoComprobante
    {
        get => _editandoComprobante;
        private set
        {
            if (Establecer(ref _editandoComprobante, value))
            {
                Notificar(nameof(PuedeSeleccionarVenta));
                ActualizarComprobanteCommand.NotificarPuedeEjecutar();
                ConfirmarActualizacionCommand.NotificarPuedeEjecutar();
                CancelarEdicionCommand.NotificarPuedeEjecutar();
                CrearComprobanteCommand.NotificarPuedeEjecutar();
            }
        }
    }

    public bool AnulacionVisible
    {
        get => _anulacionVisible;
        private set
        {
            if (Establecer(ref _anulacionVisible, value))
            {
                ConfirmarAnulacionCommand.NotificarPuedeEjecutar();
                CancelarAnulacionCommand.NotificarPuedeEjecutar();
            }
        }
    }

    // Estado general y mensajes.
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
        DescargarPdfCommand.NotificarPuedeEjecutar();
        ActualizarComprobanteCommand.NotificarPuedeEjecutar();
        ConfirmarActualizacionCommand.NotificarPuedeEjecutar();
        CancelarEdicionCommand.NotificarPuedeEjecutar();
        AnularComprobanteCommand.NotificarPuedeEjecutar();
        ConfirmarAnulacionCommand.NotificarPuedeEjecutar();
        CancelarAnulacionCommand.NotificarPuedeEjecutar();
    }

    private void Consultar()
    {
        if (Consultando || Registrando)
        {
            return;
        }

        try
        {
            Consultando = true;
            var resultados = _servicio.ConsultarConFechaTexto(
                TipoComprobanteSeleccionado,
                NumeroComprobante,
                FechaEmisionTexto);

            ComprobanteSeleccionado = null;
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

    private void CargarVentasPendientes()
    {
        if (Consultando || Registrando)
        {
            return;
        }

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
            Trace.TraceError("[DOC] Carga de ventas pendientes: {0}", ex);
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
            resultados = resultados.Where(venta => venta.CodigoVenta.Contains(
                BuscarCodigoVenta.Trim(), StringComparison.OrdinalIgnoreCase));
        }

        if (FechaFiltroVenta is { } fecha)
        {
            resultados = resultados.Where(venta => venta.FechaHora.Date == fecha.Date);
        }

        var lista = resultados.ToArray();
        VentasPendientes.Clear();
        foreach (var venta in lista)
        {
            VentasPendientes.Add(venta);
        }

        if (VentaSeleccionada is not null && !VentasPendientes.Contains(VentaSeleccionada))
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

    private void CrearComprobante()
    {
        if (Registrando || EditandoComprobante)
        {
            return;
        }

        if (VentaSeleccionada is null)
        {
            Mensaje = "Seleccione una venta pendiente.";
            return;
        }

        try
        {
            _servicio.ValidarDatosParaPrevisualizacion(
                TipoComprobanteRegistro,
                NumeroComprobanteRegistro,
                FechaEmisionRegistroTexto);
            VentaPrevisualizada = VentaSeleccionada;
            PrevisualizacionVisible = true;
        }
        catch (ReglaDeNegocioException ex)
        {
            Mensaje = ex.Message;
        }
    }

    private void CancelarEmision()
    {
        if (Registrando)
        {
            return;
        }

        PrevisualizacionVisible = false;
        VentaPrevisualizada = null;
    }

    private void DescargarPdf()
    {
        if (!PrevisualizacionVisible || VentaPrevisualizada is null || Registrando)
        {
            return;
        }

        try
        {
            var fechaEmision = _servicio.ValidarDatosParaPrevisualizacion(
                TipoComprobanteRegistro,
                NumeroComprobanteRegistro,
                FechaEmisionRegistroTexto);

            var venta = VentaPrevisualizada.Venta;
            var datos = new DatosComprobantePdf
            {
                TipoComprobante = TipoComprobanteRegistro.Trim(),
                NumeroComprobante = NumeroComprobanteRegistro.Trim(),
                FechaEmision = fechaEmision,
                CodigoVenta = VentaPrevisualizada.CodigoVenta,
                FechaVenta = VentaPrevisualizada.FechaHora,
                MontoTotal = venta.MontoTotal,
                Detalle = venta.Detalle.Select(linea => new DetalleComprobantePdf
                {
                    NombreProducto = linea.NombreProducto,
                    NumeroLote = linea.NumeroLote,
                    Cantidad = linea.Cantidad,
                    PrecioUnitario = linea.PrecioUnitario
                }).ToArray()
            };

            var ruta = _pdfServicio.GenerarComprobanteBorrador(datos);
            Mensaje = $"Borrador PDF generado correctamente: {Path.GetFileName(ruta)}. Se guardó en Documentos/FarmasilApp/PDF.";
        }
        catch (ReglaDeNegocioException ex)
        {
            Mensaje = ex.Message;
        }
        catch (Exception ex)
        {
            Trace.TraceError("[DOC] Generación de borrador PDF: {0}", ex);
            Mensaje = MensajesComprobantes.ErrorPdf;
        }
    }

    private void ConfirmarEmision()
{
    if (Registrando || VentaPrevisualizada is null)
    {
        return;
    }

    var registroConfirmado = false;

    try
    {
        Registrando = true;

        _servicio.Registrar(
            TipoComprobanteRegistro,
            NumeroComprobanteRegistro,
            FechaEmisionRegistroTexto,
            VentaPrevisualizada.Venta.IdVenta);

        registroConfirmado = true;

        var ventaRegistrada = VentaPrevisualizada;

        PrevisualizacionVisible = false;
        VentaPrevisualizada = null;

        _todasLasVentas.Remove(ventaRegistrada);
        VentasPendientes.Remove(ventaRegistrada);

        VentaSeleccionada = null;
        NumeroComprobanteRegistro = string.Empty;

        AplicarFiltrosVentas();

        Mensaje = MensajesComprobantes.RegistroCorrecto;
    }
    catch (ReglaDeNegocioException ex)
    {
        Mensaje = ex.Message;
    }
    catch (Exception ex)
    {
        Trace.TraceError("[DOC] Registro de comprobante: {0}", ex);
        Mensaje = MensajesComprobantes.ErrorRegistro;
    }
    finally
    {
        Registrando = false;
    }

    // Actualizar la colección de comprobantes después del registro.
    if (registroConfirmado)
    {
        Consultar();

        if (!string.Equals(
            Mensaje,
            MensajesComprobantes.ErrorConsulta,
            StringComparison.Ordinal))
        {
            Mensaje = MensajesComprobantes.RegistroCorrecto;
        }
    }
}

    private void IniciarActualizacion()
    {
        if (!PuedeAdministrar || ComprobanteSeleccionado is null || Registrando)
        {
            return;
        }

        try
        {
            var comprobante = _servicio.ObtenerPorIdParaActualizar(ComprobanteSeleccionado.IdComprobante);
            ComprobanteEnEdicion = comprobante;
            TipoComprobanteRegistro = comprobante.TipoComprobante;
            NumeroComprobanteRegistro = comprobante.NumeroComprobante;
            FechaEmisionRegistroTexto = comprobante.FechaEmision.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture);
            EditandoComprobante = true;
            Mensaje = "Modifique el tipo, número o fecha. La venta asociada no se puede cambiar.";
        }
        catch (ReglaDeNegocioException ex)
        {
            Mensaje = ex.Message;
        }
        catch (Exception ex)
        {
            Trace.TraceError("[DOC] Inicio de actualización: {0}", ex);
            Mensaje = MensajesComprobantes.ErrorActualizacion;
        }
    }

    private void ConfirmarActualizacion()
    {
        if (!PuedeAdministrar || !EditandoComprobante || ComprobanteEnEdicion is null || Registrando)
        {
            return;
        }

        try
        {
            Registrando = true;
            _servicio.Actualizar(
                ComprobanteEnEdicion.IdComprobante,
                TipoComprobanteRegistro,
                NumeroComprobanteRegistro,
                FechaEmisionRegistroTexto);
            Mensaje = MensajesComprobantes.ActualizacionCorrecta;
            FinalizarEdicion();
        }
        catch (ReglaDeNegocioException ex)
        {
            Mensaje = ex.Message;
        }
        catch (Exception ex)
        {
            Trace.TraceError("[DOC] Actualización de comprobante: {0}", ex);
            Mensaje = MensajesComprobantes.ErrorActualizacion;
        }
        finally
        {
            Registrando = false;
        }

        if (!EditandoComprobante)
        {
            Consultar();
            Mensaje = MensajesComprobantes.ActualizacionCorrecta;
        }
    }

    private void CancelarEdicion()
    {
        if (Registrando)
        {
            return;
        }

        FinalizarEdicion();
        Mensaje = "Se canceló la actualización del comprobante.";
    }

    private void FinalizarEdicion()
    {
        EditandoComprobante = false;
        ComprobanteEnEdicion = null;
        NumeroComprobanteRegistro = string.Empty;
        TipoComprobanteRegistro = "Boleta";
        FechaEmisionRegistroTexto = DateTime.Today.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture);
    }

    private void SolicitarAnulacion()
    {
        if (!PuedeAdministrar || ComprobanteSeleccionado is null || Registrando)
        {
            return;
        }

        try
        {
            _ = _servicio.ObtenerPorIdParaActualizar(ComprobanteSeleccionado.IdComprobante);
            AnulacionVisible = true;
        }
        catch (ReglaDeNegocioException ex)
        {
            Mensaje = ex.Message;
        }
        catch (Exception ex)
        {
            Trace.TraceError("[DOC] Validación de anulación: {0}", ex);
            Mensaje = MensajesComprobantes.ErrorAnulacion;
        }
    }

    private void ConfirmarAnulacion()
    {
        if (!PuedeAdministrar || !AnulacionVisible || ComprobanteSeleccionado is null || Registrando)
        {
            return;
        }

        try
        {
            Registrando = true;
            _servicio.Anular(ComprobanteSeleccionado.IdComprobante);
            AnulacionVisible = false;
            ComprobanteSeleccionado = null;
            Mensaje = MensajesComprobantes.AnulacionCorrecta;
        }
        catch (ReglaDeNegocioException ex)
        {
            Mensaje = ex.Message;
        }
        catch (Exception ex)
        {
            Trace.TraceError("[DOC] Anulación de comprobante: {0}", ex);
            Mensaje = MensajesComprobantes.ErrorAnulacion;
        }
        finally
        {
            Registrando = false;
        }

        if (!AnulacionVisible && Mensaje == MensajesComprobantes.AnulacionCorrecta)
        {
            Consultar();
            CargarVentasPendientes();
            Mensaje = MensajesComprobantes.AnulacionCorrecta;
        }
    }

    private void CancelarAnulacion()
    {
        if (Registrando)
        {
            return;
        }

        AnulacionVisible = false;
    }

    private static bool EsEmitido(ComprobanteTributario? comprobante) =>
        comprobante is not null
        && string.Equals(comprobante.EstadoDocumento, "Emitido", StringComparison.OrdinalIgnoreCase);

    public sealed class VentaPendienteItem
    {
        public VentaPendienteItem(VentaDto venta)
        {
            Venta = venta ?? throw new ArgumentNullException(nameof(venta));
        }

        public VentaDto Venta { get; }
        public int IdVenta => Venta.IdVenta;
        public string CodigoVenta => $"VEN-{Venta.FechaHora:yyyy}-{Venta.IdVenta:000}";
        public DateTime FechaHora => Venta.FechaHora;
        public decimal MontoTotal => Venta.MontoTotal;
    }
}
