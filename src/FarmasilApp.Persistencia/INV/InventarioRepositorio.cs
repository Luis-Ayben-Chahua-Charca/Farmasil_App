using FarmasilApp.Modelo.INV;
using Microsoft.Data.SqlClient;
using System.Data;
using System.Globalization;

namespace FarmasilApp.Persistencia.INV;

/// <summary>
/// Persistencia del módulo de Inventario.
/// Todas las entradas se envían a SQL Server mediante parámetros.
/// </summary>
public sealed class InventarioRepositorio : IInventarioRepositorio
{
    private const int ErrorConflictoDeRestriccion = 547;
    private const string PuntoDeGuardado = "antes_baja_producto";

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
                p.id_proveedor,
                p.stock_minimo,
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

        using var conexion = Abrir();
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

                IdProveedor = lector.GetInt32(
                    lector.GetOrdinal("id_proveedor")),

                StockMinimo = lector.GetInt32(
                    lector.GetOrdinal("stock_minimo")),

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

        using var conexion = Abrir();
        using var comando = new SqlCommand(sql, conexion);
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

        using var conexion = Abrir();
        using var comando = new SqlCommand(sql, conexion);
        using var lector = comando.ExecuteReader();

        while (lector.Read())
        {
            acciones.Add(lector.GetString(0));
        }

        return acciones;
    }

    /// <summary>
    /// ESP-0005. Registra el producto como Disponible y devuelve
    /// el identificador generado por SQL Server.
    /// </summary>
    public int Crear(ProductoEscritura producto)
    {
        ArgumentNullException.ThrowIfNull(producto);

        const string sql = """
            INSERT INTO dbo.TBL_PRODUCTOS
                (
                    nombre,
                    accion_terapeutica,
                    precio_venta,
                    estado_producto,
                    id_proveedor,
                    stock_minimo
                )
            OUTPUT INSERTED.id_producto
            VALUES
                (
                    @nombre,
                    @accionTerapeutica,
                    @precioVenta,
                    N'Disponible',
                    @idProveedor,
                    @stockMinimo
                );
            """;

        using var conexion = Abrir();
        using var transaccion = conexion.BeginTransaction();
        using var comando = new SqlCommand(sql, conexion, transaccion);

        AgregarParametrosProducto(comando, producto);

        var resultado = comando.ExecuteScalar();

        if (resultado is null || resultado is DBNull)
        {
            throw new InvalidOperationException(
                "SQL Server no devolvió el identificador del producto creado.");
        }

        var idProducto = Convert.ToInt32(
            resultado,
            CultureInfo.InvariantCulture);

        transaccion.Commit();

        return idProducto;
    }

    /// <summary>
    /// ESP-0007. Actualiza solamente los datos editables.
    /// El identificador y las referencias existentes permanecen intactos.
    /// </summary>
    public void Actualizar(
        int idProducto,
        ProductoEscritura producto)
    {
        ArgumentNullException.ThrowIfNull(producto);

        const string sql = """
            UPDATE dbo.TBL_PRODUCTOS
            SET nombre = @nombre,
                accion_terapeutica = @accionTerapeutica,
                precio_venta = @precioVenta,
                id_proveedor = @idProveedor,
                stock_minimo = @stockMinimo
            WHERE id_producto = @idProducto;
            """;

        using var conexion = Abrir();
        using var transaccion = conexion.BeginTransaction();
        using var comando = new SqlCommand(sql, conexion, transaccion);

        comando.Parameters.Add(
            new SqlParameter("@idProducto", SqlDbType.Int)
            {
                Value = idProducto
            });

        AgregarParametrosProducto(comando, producto);

        var filas = comando.ExecuteNonQuery();

        if (filas != 1)
        {
            throw new InvalidOperationException(
                $"El producto {idProducto} no existe.");
        }

        transaccion.Commit();
    }

    /// <summary>
    /// ESP-0008. Baja condicional según DEC-009 y DEC-029 (VP-B).
    /// Primero intenta el borrado físico. Si una clave foránea lo impide
    /// mediante el error 547 y la transacción sigue confirmable, vuelve
    /// al punto de guardado y marca el producto como Descontinuado.
    /// </summary>
    public ResultadoBajaProducto DarDeBaja(int idProducto)
    {
        const string sqlBorrar = """
            DELETE FROM dbo.TBL_PRODUCTOS
            WHERE id_producto = @idProducto;
            """;

        const string sqlDescontinuar = """
            UPDATE dbo.TBL_PRODUCTOS
            SET estado_producto = N'Descontinuado'
            WHERE id_producto = @idProducto;
            """;

        using var conexion = Abrir();

        using (var xactAbort =
               new SqlCommand("SET XACT_ABORT OFF;", conexion))
        {
            xactAbort.ExecuteNonQuery();
        }

        using var transaccion = conexion.BeginTransaction();

        transaccion.Save(PuntoDeGuardado);

        try
        {
            using var borrar =
                new SqlCommand(sqlBorrar, conexion, transaccion);

            borrar.Parameters.Add(
                new SqlParameter("@idProducto", SqlDbType.Int)
                {
                    Value = idProducto
                });

            if (borrar.ExecuteNonQuery() != 1)
            {
                throw new InvalidOperationException(
                    $"El producto {idProducto} no existe.");
            }

            transaccion.Commit();

            return ResultadoBajaProducto.Eliminado;
        }
        catch (SqlException ex)
            when (ex.Number == ErrorConflictoDeRestriccion)
        {
            if (EstadoDeLaTransaccion(
                    conexion,
                    transaccion) != 1)
            {
                transaccion.Rollback();

                throw new InvalidOperationException(
                    $"La baja del producto {idProducto} no se pudo completar: " +
                    "la transacción quedó no confirmable.",
                    ex);
            }

            transaccion.Rollback(PuntoDeGuardado);

            using var descontinuar =
                new SqlCommand(
                    sqlDescontinuar,
                    conexion,
                    transaccion);

            descontinuar.Parameters.Add(
                new SqlParameter("@idProducto", SqlDbType.Int)
                {
                    Value = idProducto
                });

            if (descontinuar.ExecuteNonQuery() != 1)
            {
                throw new InvalidOperationException(
                    $"El producto {idProducto} no existe.");
            }

            transaccion.Commit();

            return ResultadoBajaProducto.Descontinuado;
        }
    }

    private SqlConnection Abrir()
    {
        var conexion =
            new SqlConnection(_opciones.CadenaConexion);

        conexion.Open();

        return conexion;
    }

    private static void AgregarParametrosProducto(
        SqlCommand comando,
        ProductoEscritura producto)
    {
        comando.Parameters.Add(
            new SqlParameter(
                "@nombre",
                SqlDbType.NVarChar,
                InventarioServicio.LongitudMaximaNombre)
            {
                Value = producto.Nombre
            });

        comando.Parameters.Add(
            new SqlParameter(
                "@accionTerapeutica",
                SqlDbType.NVarChar,
                InventarioServicio.LongitudMaximaAccionTerapeutica)
            {
                Value = producto.AccionTerapeutica
            });

        var precio =
            new SqlParameter(
                "@precioVenta",
                SqlDbType.Decimal)
            {
                Precision = 10,
                Scale = 2,
                Value = producto.PrecioVenta
            };

        comando.Parameters.Add(precio);

        comando.Parameters.Add(
            new SqlParameter(
                "@idProveedor",
                SqlDbType.Int)
            {
                Value = producto.IdProveedor
            });

        comando.Parameters.Add(
            new SqlParameter(
                "@stockMinimo",
                SqlDbType.Int)
            {
                Value = producto.StockMinimo
            });
    }

    private static int EstadoDeLaTransaccion(
        SqlConnection conexion,
        SqlTransaction transaccion)
    {
        using var comando =
            new SqlCommand(
                "SELECT XACT_STATE();",
                conexion,
                transaccion);

        return Convert.ToInt32(
            comando.ExecuteScalar(),
            CultureInfo.InvariantCulture);
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
