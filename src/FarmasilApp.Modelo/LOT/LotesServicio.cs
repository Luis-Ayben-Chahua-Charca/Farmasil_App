using FarmasilApp.Modelo.Comun;
using FarmasilApp.Modelo.INV;
using FarmasilApp.Modelo.PRV;
using FarmasilApp.Modelo.Seguridad;

namespace FarmasilApp.Modelo.LOT;

/// <summary>
/// Servicio de aplicación del módulo LOT.
/// Implementa ESP-0041, ESP-0042, ESP-0043 y ESP-0044.
/// </summary>
public sealed class LotesServicio
{
    private readonly ILotesRepositorio _repositorio;
    private readonly IInventario _inventario;
    private readonly IProveedores _proveedores;
    private readonly ISesion _sesion;
    private readonly TimeProvider _reloj;

    public LotesServicio(
        ILotesRepositorio repositorio,
        IInventario inventario,
        IProveedores proveedores,
        ISesion sesion,
        TimeProvider reloj)
    {
        _repositorio = repositorio
            ?? throw new ArgumentNullException(nameof(repositorio));

        _inventario = inventario
            ?? throw new ArgumentNullException(nameof(inventario));

        _proveedores = proveedores
            ?? throw new ArgumentNullException(nameof(proveedores));

        _sesion = sesion
            ?? throw new ArgumentNullException(nameof(sesion));

        _reloj = reloj
            ?? throw new ArgumentNullException(nameof(reloj));
    }

    /// <summary>
    /// Medicamentos disponibles para los controles del módulo LOT.
    /// </summary>
    public IReadOnlyList<MedicamentoDto> MedicamentosDisponibles()
    {
        ExigirRolConsulta();

        return _inventario.Medicamentos()
            .Where(x => x.Estado == EstadoProducto.Disponible)
            .OrderBy(x => x.Nombre)
            .ThenBy(x => x.IdProducto)
            .ToArray();
    }

    /// <summary>
    /// Proveedores activos disponibles para registrar o modificar una remesa.
    /// </summary>
    public IReadOnlyList<ProveedorDto> ProveedoresActivos()
    {
        ExigirRolConsulta();
        return _proveedores.ProveedoresActivos();
    }

    /// <summary>
    /// ESP-0042. Consulta de remesas.
    /// </summary>
    public IReadOnlyList<RemesaConsulta> Consultar(
        int? idProducto,
        string? numeroLote,
        EstadoLote? estado)
    {
        ExigirRolConsulta();

        if (idProducto is <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(idProducto));
        }

        return _repositorio.Consultar(
            idProducto,
            Normalizar(numeroLote),
            estado);
    }

    /// <summary>
    /// Obtiene la ficha vigente de una remesa seleccionada.
    /// </summary>
    public RemesaConsulta Obtener(int idLote)
    {
        ExigirRolConsulta();

        if (idLote <= 0)
        {
            throw new ReglaDeNegocioException(
                MensajesLotes.RemesaObligatoria);
        }

        return _repositorio.ObtenerPorId(idLote)
            ?? throw new ReglaDeNegocioException(
                MensajesLotes.RemesaObligatoria);
    }

    /// <summary>
    /// ESP-0041. Registro de remesa.
    /// </summary>
    public int Crear(
        int? idProducto,
        int? idProveedor,
        string? numeroLote,
        DateOnly fechaVencimiento,
        int stockInicial,
        DateOnly fechaIngreso)
    {
        _sesion.ExigirRol(Rol.Administrador);

        var producto = ValidarProductoDisponible(idProducto);
        var proveedor = ValidarProveedor(idProveedor);
        var numero = ValidarNumeroLote(numeroLote);

        if (_repositorio.ExisteNumeroLote(
                producto.IdProducto,
                numero))
        {
            throw new ReglaDeNegocioException(
                MensajesLotes.NumeroLoteDuplicado);
        }

        if (fechaVencimiento <= Hoy)
        {
            throw new ReglaDeNegocioException(
                MensajesLotes.FechaVencimientoInvalida);
        }

        if (stockInicial <= 0)
        {
            throw new ReglaDeNegocioException(
                MensajesLotes.StockInicialInvalido);
        }

        if (fechaIngreso > Hoy)
        {
            throw new ReglaDeNegocioException(
                MensajesLotes.FechaIngresoInvalida);
        }

        return _repositorio.Crear(new NuevaRemesa
        {
            IdProducto = producto.IdProducto,
            IdProveedor = proveedor.IdProveedor,
            NumeroLote = numero,
            FechaVencimiento = fechaVencimiento,
            StockInicial = stockInicial,
            FechaIngreso = fechaIngreso,
        });
    }

    /// <summary>
    /// ESP-0043. Actualización de remesa.
    /// </summary>
    public void Actualizar(
        int idLote,
        int? idProveedor,
        string? numeroLote,
        DateOnly fechaVencimiento,
        int stockActual)
    {
        _sesion.ExigirRol(Rol.Administrador);

        if (idLote <= 0)
        {
            throw new ReglaDeNegocioException(
                MensajesLotes.RemesaObligatoria);
        }

        var actual = _repositorio.ObtenerPorId(idLote)
            ?? throw new ReglaDeNegocioException(
                MensajesLotes.RemesaObligatoria);

        var proveedor = ValidarProveedor(idProveedor);
        var numero = ValidarNumeroLote(numeroLote);

        if (_repositorio.ExisteNumeroLote(
                actual.IdProducto,
                numero,
                idLote))
        {
            throw new ReglaDeNegocioException(
                MensajesLotes.NumeroLoteDuplicado);
        }

        if (stockActual < 0)
        {
            throw new ReglaDeNegocioException(
                MensajesLotes.StockInvalido);
        }

        if (fechaVencimiento != actual.FechaVencimiento
            && fechaVencimiento <= Hoy)
        {
            throw new ReglaDeNegocioException(
                MensajesLotes.FechaVencimientoInvalida);
        }

        if (proveedor.IdProveedor != actual.IdProveedor
            && _repositorio.TieneDetalleDevolucion(idLote))
        {
            throw new ReglaDeNegocioException(
                MensajesLotes.ProveedorConDevolucion);
        }

        var comprometido =
            _repositorio.CantidadComprometidaPendiente(idLote);

        if (stockActual < comprometido)
        {
            throw new ReglaDeNegocioException(
                MensajesLotes.StockMenorQueComprometido);
        }

        var estadoNuevo = DerivarEstado(
            actual.Estado,
            stockActual,
            fechaVencimiento,
            Hoy);

        _repositorio.Actualizar(
            idLote,
            new ActualizacionRemesa
            {
                IdProveedor = proveedor.IdProveedor,
                NumeroLote = numero,
                FechaVencimiento = fechaVencimiento,
                StockActual = stockActual,
            },
            estadoNuevo);
    }

    /// <summary>
    /// Comprobación previa al modal de confirmación de ESP-0044.
    /// </summary>
    public void VerificarBaja(int idLote)
    {
        _sesion.ExigirRol(Rol.Administrador);

        if (idLote <= 0
            || _repositorio.ObtenerPorId(idLote) is null)
        {
            throw new ReglaDeNegocioException(
                MensajesLotes.RemesaObligatoria);
        }

        if (_repositorio.TieneDevolucionPendiente(idLote))
        {
            throw new ReglaDeNegocioException(
                MensajesLotes.DevolucionPendiente);
        }
    }

    /// <summary>
    /// ESP-0044. Baja condicional de la remesa.
    /// La comprobación se repite antes de escribir.
    /// </summary>
    public ResultadoBajaRemesa DarDeBaja(int idLote)
    {
        _sesion.ExigirRol(Rol.Administrador);

        if (idLote <= 0
            || _repositorio.ObtenerPorId(idLote) is null)
        {
            throw new ReglaDeNegocioException(
                MensajesLotes.RemesaObligatoria);
        }

        if (_repositorio.TieneDevolucionPendiente(idLote))
        {
            throw new ReglaDeNegocioException(
                MensajesLotes.DevolucionPendiente);
        }

        return _repositorio.DarDeBaja(idLote);
    }

    /// <summary>
    /// Regla única ESTADO_LOTE_DERIVADO de ESP-0043 / DEC-034.
    /// </summary>
    internal static EstadoLote DerivarEstado(
        EstadoLote estadoActual,
        int stock,
        DateOnly fechaVencimiento,
        DateOnly fechaActual)
    {
        if (stock < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(stock));
        }


        if (estadoActual == EstadoLote.BloqueadoPorDevolucion)
        {
            return EstadoLote.BloqueadoPorDevolucion;
        }

        if (stock == 0)
        {
            return EstadoLote.Agotado;
        }

        if (estadoActual == EstadoLote.Agotado
            && fechaVencimiento > fechaActual)
        {
            return EstadoLote.Disponible;
        }

        return estadoActual;
    }

    private MedicamentoDto ValidarProductoDisponible(
        int? idProducto)
    {
        if (idProducto is null || idProducto <= 0)
        {
            throw new ReglaDeNegocioException(
                MensajesLotes.ProductoObligatorio);
        }

        var producto =
            _inventario.ObtenerMedicamento(idProducto.Value);

        if (producto is null
            || producto.Estado != EstadoProducto.Disponible)
        {
            throw new ReglaDeNegocioException(
                MensajesLotes.ProductoNoDisponible);
        }

        return producto;
    }

    private ProveedorDto ValidarProveedor(
        int? idProveedor)
    {
        if (idProveedor is null || idProveedor <= 0)
        {
            throw new ReglaDeNegocioException(
                MensajesLotes.ProveedorObligatorio);
        }

        return _proveedores.ObtenerProveedor(idProveedor.Value)
            ?? throw new ReglaDeNegocioException(
                MensajesLotes.ProveedorNoExiste);
    }

    private void ExigirRolConsulta()
    {
        var rol = _sesion.UsuarioActual.Rol;

        if (rol is not (Rol.Administrador or Rol.Tecnico))
        {
            throw new AccesoDenegadoException(
                "No tiene permisos para consultar remesas.");
        }
    }

    private DateOnly Hoy =>
        DateOnly.FromDateTime(
            _reloj.GetLocalNow().DateTime);

    private static string ValidarNumeroLote(
        string? numeroLote)
    {
        if (string.IsNullOrWhiteSpace(numeroLote))
        {
            throw new ReglaDeNegocioException(
                MensajesLotes.NumeroLoteObligatorio);
        }

        return numeroLote.Trim();
    }

    private static string? Normalizar(string? valor)
    {
        return string.IsNullOrWhiteSpace(valor)
            ? null
            : valor.Trim();
    }
}
