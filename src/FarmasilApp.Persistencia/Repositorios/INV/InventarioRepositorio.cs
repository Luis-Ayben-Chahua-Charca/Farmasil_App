using FarmasilApp.Modelo.Entidades.INV;
using FarmasilApp.Modelo.Servicios.INV;
using Microsoft.Data.SqlClient;
using System.Data;

namespace FarmasilApp.Persistencia.Repositorios.INV;

/// <summary>
/// Repositorio de la consulta de Inventario (ESP-0006).
/// Las entradas se envían a SQL Server mediante parámetros.
/// </summary>
public sealed class InventarioRepositorio : IInventarioRepositorio
{
    private readonly OpcionesBaseDatos _opciones;

    public InventarioRepositorio(OpcionesBaseDatos opciones)
    {
        _opciones = opciones
            ?? throw new ArgumentNullException(nameof(opciones));
    }

    public IReadOnlyList<ProductoInventario> Consultar(
        string? busqueda,
        int? idProveedor,
        string? accionTerapeutica)
    {
        const string sql = """
            SELECT
                p.id_producto,
                p.nombre,
                p.accion_terapeutica,
                p.precio_venta,
                p.estado_producto,
                COALESCE(s.stock_fisico, 0) AS stock_fisico
            FROM dbo.TBL_PRODUCTOS AS p
            LEFT JOIN dbo.VW_STOCK_FISICO AS s
                ON s.id_producto = p.id_producto
            WHERE
                (
                    @busqueda IS NULL
                    OR p.nombre LIKE '%' + @busquedaLike + '%' ESCAPE '\'
                    OR CONVERT(nvarchar(20), p.id_producto) = @busqueda
                )
                AND
                (
                    @idProveedor IS NULL
                    OR p.id_proveedor = @idProveedor
                )
                AND
                (
                    @accionTerapeutica IS NULL
                    OR p.accion_terapeutica = @accionTerapeutica
                )
            ORDER BY p.nombre, p.id_producto;
            """;

        var productos = new List<ProductoInventario>();

        using var conexion = new SqlConnection(_opciones.CadenaConexion);
        using var comando = new SqlCommand(sql, conexion);

        comando.Parameters.Add(
            new SqlParameter("@busqueda", SqlDbType.NVarChar, 100)
            {
                Value = (object?)busqueda ?? DBNull.Value
            });

        comando.Parameters.Add(
            new SqlParameter("@busquedaLike", SqlDbType.NVarChar, 400)
            {
                Value = busqueda is null
                    ? DBNull.Value
                    : EscaparLike(busqueda)
            });

        comando.Parameters.Add(
            new SqlParameter("@idProveedor", SqlDbType.Int)
            {
                Value = (object?)idProveedor ?? DBNull.Value
            });

        comando.Parameters.Add(
            new SqlParameter("@accionTerapeutica", SqlDbType.NVarChar, 150)
            {
                Value = (object?)accionTerapeutica ?? DBNull.Value
            });

        conexion.Open();

        using var lector = comando.ExecuteReader();

        while (lector.Read())
        {
            productos.Add(new ProductoInventario
            {
                IdProducto = lector.GetInt32(
                    lector.GetOrdinal("id_producto")),

                Nombre = lector.GetString(
                    lector.GetOrdinal("nombre")),

                AccionTerapeutica = lector.GetString(
                    lector.GetOrdinal("accion_terapeutica")),

                PrecioVenta = lector.GetDecimal(
                    lector.GetOrdinal("precio_venta")),

                EstadoProducto = lector.GetString(
                    lector.GetOrdinal("estado_producto")),

                StockFisico = lector.GetInt32(
                    lector.GetOrdinal("stock_fisico"))
            });
        }

        return productos;
    }

    public IReadOnlyList<int> ObtenerProveedores()
    {
        const string sql = """
            SELECT DISTINCT id_proveedor
            FROM dbo.TBL_PRODUCTOS
            ORDER BY id_proveedor;
            """;

        var proveedores = new List<int>();

        using var conexion = new SqlConnection(_opciones.CadenaConexion);
        using var comando = new SqlCommand(sql, conexion);

        conexion.Open();

        using var lector = comando.ExecuteReader();

        while (lector.Read())
        {
            proveedores.Add(lector.GetInt32(0));
        }

        return proveedores;
    }

    public IReadOnlyList<string> ObtenerAccionesTerapeuticas()
    {
        const string sql = """
            SELECT DISTINCT accion_terapeutica
            FROM dbo.TBL_PRODUCTOS
            WHERE LTRIM(RTRIM(accion_terapeutica)) <> ''
            ORDER BY accion_terapeutica;
            """;

        var acciones = new List<string>();

        using var conexion = new SqlConnection(_opciones.CadenaConexion);
        using var comando = new SqlCommand(sql, conexion);

        conexion.Open();

        using var lector = comando.ExecuteReader();

        while (lector.Read())
        {
            acciones.Add(lector.GetString(0));
        }

        return acciones;
    }

    private static string EscaparLike(string valor)
    {
        return valor
            .Replace(@"\", @"\\")
            .Replace("%", @"\%")
            .Replace("_", @"\_")
            .Replace("[", @"\[");
    }
}