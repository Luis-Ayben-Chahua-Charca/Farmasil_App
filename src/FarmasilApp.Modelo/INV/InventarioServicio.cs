using FarmasilApp.Modelo.Comun;
using FarmasilApp.Modelo.PRV;
using FarmasilApp.Modelo.Seguridad;

namespace FarmasilApp.Modelo.INV;

/// <summary>
/// Servicio de aplicación de Inventario.
/// Implementa ESP-0005, ESP-0006, ESP-0007 y ESP-0008.
/// </summary>
public sealed class InventarioServicio
{
    public const int LongitudMaximaNombre = 100;
    public const int LongitudMaximaAccionTerapeutica = 150;

    private readonly IInventarioRepositorio _repositorio;
    private readonly IProveedores _proveedores;
    private readonly ISesion _sesion;

    public InventarioServicio(
        IInventarioRepositorio repositorio,
        IProveedores proveedores,
        ISesion sesion)
    {
        _repositorio = repositorio
            ?? throw new ArgumentNullException(nameof(repositorio));

        _proveedores = proveedores
            ?? throw new ArgumentNullException(nameof(proveedores));

        _sesion = sesion
            ?? throw new ArgumentNullException(nameof(sesion));
    }

    public IReadOnlyList<ProductoInventario> Consultar(
        string? busqueda,
        int? idProveedor,
        string? accionTerapeutica)
    {
        return _repositorio.Consultar(
            Normalizar(busqueda),
            idProveedor,
            Normalizar(accionTerapeutica));
    }

    /// <summary>
    /// Proveedores usados por el filtro de consulta.
    /// Los identificadores salen de Inventario y el nombre de IProveedores.
    /// </summary>
    public IReadOnlyList<ProveedorFiltro> ObtenerProveedores()
    {
        return _repositorio.ObtenerProveedores()
            .Select(idProveedor => new ProveedorFiltro
            {
                IdProveedor = idProveedor,
                Nombre = _proveedores.ObtenerProveedor(idProveedor)?.RazonSocial
                    ?? $"Proveedor {idProveedor}",
            })
            .ToArray();
    }

    /// <summary>
    /// Proveedores disponibles para registrar o modificar un producto.
    /// Inventario consume IProveedores y no consulta TBL_PROVEEDORES directamente.
    /// </summary>
    public IReadOnlyList<ProveedorDto> ObtenerProveedoresActivos()
    {
        return _proveedores.ProveedoresActivos();
    }

    public IReadOnlyList<string> ObtenerAccionesTerapeuticas()
    {
        return _repositorio.ObtenerAccionesTerapeuticas();
    }

    /// <summary>
    /// ESP-0005 - Creación del producto.
    /// </summary>
    public int Crear(
        string? nombre,
        string? accionTerapeutica,
        decimal precioVenta,
        int? idProveedor,
        int stockMinimo)
    {
        _sesion.ExigirRol(Rol.Administrador);

        var producto = ValidarProducto(
            nombre,
            accionTerapeutica,
            precioVenta,
            idProveedor,
            stockMinimo);

        return _repositorio.Crear(producto);
    }

    /// <summary>
    /// ESP-0007 - Actualización del producto.
    /// </summary>
    public void Actualizar(
        int idProducto,
        string? nombre,
        string? accionTerapeutica,
        decimal precioVenta,
        int? idProveedor,
        int stockMinimo)
    {
        _sesion.ExigirRol(Rol.Administrador);

        if (idProducto <= 0)
        {
            throw new ReglaDeNegocioException(
                "Seleccione un producto para actualizar.");
        }

        var producto = ValidarProducto(
            nombre,
            accionTerapeutica,
            precioVenta,
            idProveedor,
            stockMinimo);

        _repositorio.Actualizar(idProducto, producto);
    }

    /// <summary>
    /// ESP-0008 - Baja condicional.
    /// La base de datos decide si el registro se elimina físicamente
    /// o debe quedar Descontinuado.
    /// </summary>
    public ResultadoBajaProducto DarDeBaja(int idProducto)
    {
        _sesion.ExigirRol(Rol.Administrador);

        if (idProducto <= 0)
        {
            throw new ReglaDeNegocioException(
                "Seleccione un producto para dar de baja.");
        }

        return _repositorio.DarDeBaja(idProducto);
    }

    private ProductoEscritura ValidarProducto(
        string? nombre,
        string? accionTerapeutica,
        decimal precioVenta,
        int? idProveedor,
        int stockMinimo)
    {
        var nombreNormalizado = ValidarTexto(
            nombre,
            LongitudMaximaNombre,
            "El nombre del producto es obligatorio.",
            "El nombre del producto supera la longitud permitida.");

        var accionNormalizada = ValidarTexto(
            accionTerapeutica,
            LongitudMaximaAccionTerapeutica,
            "La acción terapéutica es obligatoria.",
            "La acción terapéutica supera la longitud permitida.");

        if (precioVenta < 0)
        {
            throw new ReglaDeNegocioException(
                "El precio de venta debe ser mayor o igual a cero.");
        }

        if (stockMinimo < 0)
        {
            throw new ReglaDeNegocioException(
                "El stock mínimo debe ser mayor o igual a cero.");
        }

        if (idProveedor is null || idProveedor <= 0)
        {
            throw new ReglaDeNegocioException(
                "Seleccione un proveedor.");
        }

        var proveedorActivo = _proveedores.ProveedoresActivos()
            .Any(p => p.IdProveedor == idProveedor.Value);

        if (!proveedorActivo)
        {
            throw new ReglaDeNegocioException(
                "El proveedor seleccionado no está activo.");
        }

        return new ProductoEscritura
        {
            Nombre = nombreNormalizado,
            AccionTerapeutica = accionNormalizada,
            PrecioVenta = precioVenta,
            IdProveedor = idProveedor.Value,
            StockMinimo = stockMinimo,
        };
    }

    private static string ValidarTexto(
        string? valor,
        int longitudMaxima,
        string mensajeVacio,
        string mensajeLongitud)
    {
        if (string.IsNullOrWhiteSpace(valor))
        {
            throw new ReglaDeNegocioException(mensajeVacio);
        }

        var normalizado = valor.Trim();

        if (normalizado.Length > longitudMaxima)
        {
            throw new ReglaDeNegocioException(mensajeLongitud);
        }

        return normalizado;
    }

    private static string? Normalizar(string? valor)
    {
        return string.IsNullOrWhiteSpace(valor)
            ? null
            : valor.Trim();
    }
}