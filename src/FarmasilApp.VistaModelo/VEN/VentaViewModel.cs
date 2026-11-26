using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Globalization;
using FarmasilApp.Modelo.Comun;
using FarmasilApp.Modelo.LOT;
using FarmasilApp.Modelo.PAG;
using FarmasilApp.Modelo.Seguridad;
using FarmasilApp.Modelo.VEN;
using FarmasilApp.VistaModelo.Comun;

namespace FarmasilApp.VistaModelo.VEN;

/// <summary>
/// VistaModelo de la pantalla de venta, <c>ART-MKP-VEN-0001</c> (ESP-0001; DEC-066, DEC-067): catálogo de medicamentos
/// vendibles, detalle del medicamento con la remesa FEFO, y detalle de la venta con su total.
/// </summary>
/// <remarks>
/// <para><b>Qué hace y qué no.</b> Enlaza la pantalla con <see cref="VentaServicio"/> y con el <see cref="CarritoVenta"/>
/// en curso. No contiene reglas de negocio: cada línea la valida el servicio. El detalle no se guarda; salir de la
/// pantalla lo descarta (DEC-067).</para>
/// <para><b>Errores (DEC-072).</b> Una línea rechazada muestra el mensaje de <see cref="ReglaDeNegocioException"/> en
/// <c>VEN-LBL-ADVERTENCIAS</c>. <see cref="AccesoDenegadoException"/> y cualquier otra excepción se registran con
/// <see cref="Trace"/> y muestran un mensaje de <see cref="MensajesVentas"/>, nunca el texto técnico.</para>
/// <para><b>Registrar la venta.</b> <c>VEN-BTN-CREAR-VENTA</c> se muestra deshabilitado con su motivo (RNF-0006,
/// condición 4; DEC-075): el registro todavía no está conectado.</para>
/// </remarks>
public sealed class VentaViewModel : ViewModelBase
{
    /// <summary>Opción del filtro de acción terapéutica que no filtra.</summary>
    public const string TodasLasAcciones = "Todas";

    private readonly VentaServicio _servicio;
    private readonly CarritoVenta _carrito = new();

    private IReadOnlyList<MedicamentoVendibleDto> _catalogo = Array.Empty<MedicamentoVendibleDto>();
    private string? _busqueda;
    private string _accionSeleccionada = TodasLasAcciones;
    private MedicamentoVendibleDto? _medicamentoSeleccionado;
    private RemesaVendibleDto? _remesaSeleccionada;
    private string _cantidad = string.Empty;
    private LineaVenta? _lineaSeleccionada;
    private MetodoPagoDto? _metodoPagoSeleccionado;
    private DateTime _fechaVenta;
    private string _advertencias = string.Empty;

    public VentaViewModel(VentaServicio servicio)
    {
        _servicio = servicio ?? throw new ArgumentNullException(nameof(servicio));

        InicializarCommand = new AsyncRelayCommand(InicializarAsync);
        SeleccionarProductoCommand = new RelayCommand<MedicamentoVendibleDto>(SeleccionarProducto);
        AgregarProductoCommand = new AsyncRelayCommand(AgregarProductoAsync, () => RemesaSeleccionada is not null);
        QuitarProductoCommand = new RelayCommand(QuitarProducto, () => LineaSeleccionada is not null);

        // Deshabilitado hasta que el registro de la venta esté conectado: nunca se puede ejecutar.
        CrearVentaCommand = new RelayCommand(() => { }, () => false);
    }

    /// <summary>Tarjetas de <c>VEN-PNL-CATALOGO</c>: el catálogo con el buscador y el filtro aplicados.</summary>
    public ObservableCollection<MedicamentoVendibleDto> Medicamentos { get; } = new();

    /// <summary>Opciones de <c>VEN-CMB-FILTRO-ACCION-TERAPEUTICA</c>.</summary>
    public ObservableCollection<string> AccionesTerapeuticas { get; } = new() { TodasLasAcciones };

    /// <summary>Opciones de <c>VEN-CMB-LOTE-VENTA</c>: las remesas vendibles del medicamento, la FEFO primero.</summary>
    public ObservableCollection<RemesaVendibleDto> Remesas { get; } = new();

    /// <summary>Filas de <c>VEN-TBL-DETALLE-VENTA</c>.</summary>
    public ObservableCollection<LineaVenta> Lineas { get; } = new();

    /// <summary>Opciones de <c>VEN-CMB-METODO-PAGO</c>.</summary>
    public ObservableCollection<MetodoPagoDto> MetodosPago { get; } = new();

    public AsyncRelayCommand InicializarCommand { get; }

    /// <summary><c>VEN-BTN-SELECCIONAR-PRODUCTO</c>: elige el medicamento y prepara la remesa FEFO con cantidad 1.</summary>
    public RelayCommand<MedicamentoVendibleDto> SeleccionarProductoCommand { get; }

    /// <summary><c>VEN-BTN-AGREGAR-PRODUCTO</c>: el único camino que valida y agrega una línea (DEC-067).</summary>
    public AsyncRelayCommand AgregarProductoCommand { get; }

    /// <summary><c>VEN-BTN-QUITAR-PRODUCTO</c>.</summary>
    public RelayCommand QuitarProductoCommand { get; }

    /// <summary><c>VEN-BTN-CREAR-VENTA</c>: deshabilitado; su motivo es <see cref="MotivoRegistro"/>.</summary>
    public RelayCommand CrearVentaCommand { get; }

    /// <summary>Motivo temporal por el que <c>VEN-BTN-CREAR-VENTA</c> está deshabilitado.</summary>
    public string MotivoRegistro => MensajesVentas.RegistroNoDisponible;

    /// <summary><c>VEN-SRH-PRODUCTO</c>: filtra el catálogo por nombre mientras se escribe.</summary>
    public string? Busqueda
    {
        get => _busqueda;
        set
        {
            if (Establecer(ref _busqueda, value))
            {
                AplicarFiltro();
            }
        }
    }

    /// <summary><c>VEN-CMB-FILTRO-ACCION-TERAPEUTICA</c>.</summary>
    public string AccionSeleccionada
    {
        get => _accionSeleccionada;
        set
        {
            if (Establecer(ref _accionSeleccionada, value ?? TodasLasAcciones))
            {
                AplicarFiltro();
            }
        }
    }

    /// <summary>Medicamento elegido: alimenta <c>VEN-PNL-DETALLE-MEDICAMENTO</c>.</summary>
    public MedicamentoVendibleDto? MedicamentoSeleccionado
    {
        get => _medicamentoSeleccionado;
        private set
        {
            if (Establecer(ref _medicamentoSeleccionado, value))
            {
                Notificar(nameof(HayMedicamentoSeleccionado));
            }
        }
    }

    public bool HayMedicamentoSeleccionado => MedicamentoSeleccionado is not null;

    /// <summary><c>VEN-CMB-LOTE-VENTA</c>.</summary>
    public RemesaVendibleDto? RemesaSeleccionada
    {
        get => _remesaSeleccionada;
        set
        {
            if (Establecer(ref _remesaSeleccionada, value))
            {
                Notificar(nameof(FechaVencimientoRemesa));
                AgregarProductoCommand.NotificarPuedeEjecutar();
            }
        }
    }

    /// <summary><c>VEN-LBL-FECHA-VENCIMIENTO</c>: vencimiento de la remesa elegida, solo lectura.</summary>
    public string FechaVencimientoRemesa =>
        RemesaSeleccionada is null
            ? string.Empty
            : RemesaSeleccionada.FechaVencimiento.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture);

    /// <summary><c>VEN-NUM-CANTIDAD-VENTA</c>. Es texto: la validación de entero positivo es del servicio.</summary>
    public string Cantidad
    {
        get => _cantidad;
        set => Establecer(ref _cantidad, value ?? string.Empty);
    }

    /// <summary>Línea elegida en <c>VEN-TBL-DETALLE-VENTA</c>.</summary>
    public LineaVenta? LineaSeleccionada
    {
        get => _lineaSeleccionada;
        set
        {
            if (Establecer(ref _lineaSeleccionada, value))
            {
                QuitarProductoCommand.NotificarPuedeEjecutar();
            }
        }
    }

    /// <summary><c>VEN-CMB-METODO-PAGO</c>.</summary>
    public MetodoPagoDto? MetodoPagoSeleccionado
    {
        get => _metodoPagoSeleccionado;
        set => Establecer(ref _metodoPagoSeleccionado, value);
    }

    /// <summary><c>VEN-LBL-MONTO-TOTAL</c>.</summary>
    public decimal MontoTotal => _carrito.MontoTotal;

    /// <summary><c>VEN-LBL-FECHA-VENTA</c>: fecha y hora del sistema al abrir la pantalla.</summary>
    public DateTime FechaVenta
    {
        get => _fechaVenta;
        private set => Establecer(ref _fechaVenta, value);
    }

    /// <summary><c>VEN-LBL-ADVERTENCIAS</c>: motivo del último rechazo o avisos informativos; vacío si no hay.</summary>
    public string Advertencias
    {
        get => _advertencias;
        private set => Establecer(ref _advertencias, value);
    }

    private async Task InicializarAsync()
    {
        await EjecutarAsync("Carga", MensajesVentas.ErrorCarga, async () =>
        {
            var (catalogo, metodos, fecha) = await Task.Run(() =>
                (_servicio.MedicamentosVendibles(), _servicio.MetodosPagoActivos(), _servicio.FechaHoraSistema()));

            _catalogo = catalogo;
            AccionesTerapeuticas.Clear();
            AccionesTerapeuticas.Add(TodasLasAcciones);
            foreach (var accion in catalogo.Select(m => m.AccionTerapeutica).Distinct().Order())
            {
                AccionesTerapeuticas.Add(accion);
            }

            MetodosPago.Clear();
            foreach (var metodo in metodos)
            {
                MetodosPago.Add(metodo);
            }

            MetodoPagoSeleccionado = metodos.FirstOrDefault();
            FechaVenta = fecha;
            AplicarFiltro();
        });
    }

    private void AplicarFiltro()
    {
        var accion = AccionSeleccionada == TodasLasAcciones ? null : AccionSeleccionada;

        Medicamentos.Clear();
        foreach (var medicamento in VentaServicio.Filtrar(_catalogo, Busqueda, accion))
        {
            Medicamentos.Add(medicamento);
        }
    }

    private void SeleccionarProducto(MedicamentoVendibleDto? medicamento)
    {
        if (medicamento is null)
        {
            return;
        }

        try
        {
            var remesas = _servicio.RemesasSugeridas(medicamento.IdProducto);

            MedicamentoSeleccionado = medicamento;
            Remesas.Clear();
            foreach (var remesa in remesas)
            {
                Remesas.Add(remesa);
            }

            // La primera es la FEFO. Sin remesas vendibles no se puede agregar nada.
            RemesaSeleccionada = remesas.FirstOrDefault();
            Cantidad = "1";
            Advertencias = remesas.Count == 0 ? MensajesVentas.SinRemesa : string.Empty;
        }
        catch (Exception ex)
        {
            Registrar("Selección", ex);
            Advertencias = ex is AccesoDenegadoException ? MensajesVentas.AccesoDenegado : MensajesVentas.ErrorCarga;
        }
    }

    private async Task AgregarProductoAsync()
    {
        var (remesa, texto) = (RemesaSeleccionada, Cantidad);

        await EjecutarAsync("Agregar producto", MensajesVentas.ErrorOperacion, async () =>
        {
            // «Agregar producto» es el único camino que valida; un texto que no es entero positivo es cantidad inválida.
            var cantidad = int.TryParse(texto?.Trim(), NumberStyles.None, CultureInfo.InvariantCulture, out var valor) ? valor : 0;

            var avisos = await Task.Run(() => _servicio.AgregarLinea(_carrito, remesa, cantidad));

            Advertencias = string.Join(Environment.NewLine, avisos);
            ActualizarDetalle();
        });
    }

    private void QuitarProducto()
    {
        if (LineaSeleccionada is not { } linea)
        {
            return;
        }

        _carrito.Quitar(linea.IdLote);
        Advertencias = string.Empty;
        ActualizarDetalle();
    }

    private void ActualizarDetalle()
    {
        Lineas.Clear();
        foreach (var linea in _carrito.Lineas)
        {
            Lineas.Add(linea);
        }

        LineaSeleccionada = null;
        Notificar(nameof(MontoTotal));
    }

    private async Task EjecutarAsync(string operacion, string mensajeError, Func<Task> accion)
    {
        try
        {
            await accion();
        }
        catch (ReglaDeNegocioException ex)
        {
            Advertencias = ex.Message;
        }
        catch (AccesoDenegadoException ex)
        {
            Registrar(operacion, ex);
            Advertencias = MensajesVentas.AccesoDenegado;
        }
        catch (Exception ex)
        {
            Registrar(operacion, ex);
            Advertencias = mensajeError;
        }
    }

    private static void Registrar(string operacion, Exception excepcion)
    {
        Trace.TraceError("[Ventas] {0}: {1}", operacion, excepcion);
    }
}
