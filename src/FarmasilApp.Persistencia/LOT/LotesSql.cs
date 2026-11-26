using System.Data;
using FarmasilApp.Modelo.Comun;
using FarmasilApp.Modelo.LOT;
using Microsoft.Data.SqlClient;

namespace FarmasilApp.Persistencia.LOT;

/// <summary>
/// Implementación SQL real del contrato ILotes.
/// Las consultas abren su propia conexión.
/// Las escrituras deben participar en la transacción del módulo llamador (PEN-039).
/// </summary>
public sealed class LotesSql : ILotes
{
    private readonly OpcionesBaseDatos _opciones;
    private readonly ContextoTransaccionSql _contextoTransaccion;
    private readonly TimeProvider _reloj;

    public LotesSql(
        OpcionesBaseDatos opciones,
        ContextoTransaccionSql contextoTransaccion,
        TimeProvider reloj)
    {
        _opciones = opciones
            ?? throw new ArgumentNullException(nameof(opciones));

        _contextoTransaccion = contextoTransaccion
            ?? throw new ArgumentNullException(nameof(contextoTransaccion));

        _reloj = reloj
            ?? throw new ArgumentNullException(nameof(reloj));
    }

    private DateOnly Hoy =>
        DateOnly.FromDateTime(
            _reloj.GetLocalNow().DateTime);

    public IReadOnlyList<MedicamentoVendibleDto> MedicamentosVendibles()
    {
        const string sql = """
            SELECT p.id_producto,
                   p.nombre,
                   p.accion_terapeutica,
                   p.precio_venta,
                   SUM(l.stock_actual) AS stock_vendible
            FROM dbo.TBL_PRODUCTOS AS p
            INNER JOIN dbo.TBL_LOTES AS l
                ON l.id_producto = p.id_producto
            WHERE p.estado_producto = N'Disponible'
              AND l.estado_lote = N'Disponible'
              AND l.stock_actual > 0
              AND l.fecha_vencimiento > @hoy
            GROUP BY p.id_producto,
                     p.nombre,
                     p.accion_terapeutica,
                     p.precio_venta
            ORDER BY p.id_producto;
            """;

        using var conexion =
            new SqlConnection(
                _opciones.CadenaConexion);

        using var comando =
            new SqlCommand(
                sql,
                conexion);

        comando.Parameters
            .Add("@hoy", SqlDbType.Date)
            .Value =
                Hoy.ToDateTime(
                    TimeOnly.MinValue);

        conexion.Open();

        using var lector =
            comando.ExecuteReader();

        var resultado =
            new List<MedicamentoVendibleDto>();

        while (lector.Read())
        {
            resultado.Add(
                new MedicamentoVendibleDto
                {
                    IdProducto =
                        lector.GetInt32(0),

                    Nombre =
                        lector.GetString(1),

                    AccionTerapeutica =
                        lector.GetString(2),

                    PrecioVenta =
                        lector.GetDecimal(3),

                    StockVendible =
                        lector.GetInt32(4),
                });
        }

        return resultado;
    }

    public IReadOnlyList<RemesaVendibleDto> SugerirFefo(
        int idProducto)
    {
        ArgumentOutOfRangeException
            .ThrowIfNegativeOrZero(
                idProducto);

        const string sql = """
            SELECT l.id_lote,
                   p.id_producto,
                   p.nombre,
                   p.precio_venta,
                   l.numero_lote,
                   l.fecha_vencimiento,
                   l.stock_actual
            FROM dbo.TBL_LOTES AS l
            INNER JOIN dbo.TBL_PRODUCTOS AS p
                ON p.id_producto = l.id_producto
            WHERE p.id_producto = @producto
              AND p.estado_producto = N'Disponible'
              AND l.estado_lote = N'Disponible'
              AND l.stock_actual > 0
              AND l.fecha_vencimiento > @hoy
            ORDER BY l.fecha_vencimiento,
                     l.id_lote;
            """;

        using var conexion =
            new SqlConnection(
                _opciones.CadenaConexion);

        using var comando =
            new SqlCommand(
                sql,
                conexion);

        comando.Parameters
            .Add("@producto", SqlDbType.Int)
            .Value =
                idProducto;

        comando.Parameters
            .Add("@hoy", SqlDbType.Date)
            .Value =
                Hoy.ToDateTime(
                    TimeOnly.MinValue);

        conexion.Open();

        using var lector =
            comando.ExecuteReader();

        var resultado =
            new List<RemesaVendibleDto>();

        while (lector.Read())
        {
            resultado.Add(
                new RemesaVendibleDto
                {
                    IdLote =
                        lector.GetInt32(0),

                    IdProducto =
                        lector.GetInt32(1),

                    NombreMedicamento =
                        lector.GetString(2),

                    PrecioVenta =
                        lector.GetDecimal(3),

                    NumeroLote =
                        lector.GetString(4),

                    FechaVencimiento =
                        DateOnly.FromDateTime(
                            lector.GetDateTime(5)),

                    StockActual =
                        lector.GetInt32(6),
                });
        }

        return resultado;
    }

    public IReadOnlyList<DatosRemesaDto> DatosRemesas(
        IReadOnlyCollection<int> idsLote)
    {
        ArgumentNullException
            .ThrowIfNull(
                idsLote);

        foreach (var id in idsLote)
        {
            ArgumentOutOfRangeException
                .ThrowIfNegativeOrZero(
                    id);
        }

        var ids =
            idsLote
                .Distinct()
                .ToArray();

        if (ids.Length == 0)
        {
            return Array.Empty<DatosRemesaDto>();
        }

        var parametros =
            ids
                .Select(
                    (_, indice) =>
                        $"@id{indice}")
                .ToArray();

        var sql = $"""
            SELECT l.id_lote,
                   l.numero_lote,
                   l.fecha_vencimiento,
                   p.nombre,
                   l.id_proveedor,
                   l.estado_lote,
                   l.stock_actual
            FROM dbo.TBL_LOTES AS l
            INNER JOIN dbo.TBL_PRODUCTOS AS p
                ON p.id_producto = l.id_producto
            WHERE l.id_lote IN ({string.Join(", ", parametros)});
            """;

        using var conexion =
            new SqlConnection(
                _opciones.CadenaConexion);

        using var comando =
            new SqlCommand(
                sql,
                conexion);

        for (var indice = 0;
             indice < ids.Length;
             indice++)
        {
            comando.Parameters
                .Add(
                    parametros[indice],
                    SqlDbType.Int)
                .Value =
                    ids[indice];
        }

        conexion.Open();

        using var lector =
            comando.ExecuteReader();

        var encontrados =
            new Dictionary<int, DatosRemesaDto>();

        while (lector.Read())
        {
            var remesa =
                LeerDatosRemesa(
                    lector);

            encontrados[remesa.IdLote] =
                remesa;
        }

        var resultado =
            new List<DatosRemesaDto>();

        foreach (var id in ids)
        {
            if (encontrados.TryGetValue(
                    id,
                    out var remesa))
            {
                resultado.Add(
                    remesa);
            }
        }

        return resultado;
    }

    public IReadOnlyList<DatosRemesaDto> RemesasDevolvibles(
        int idProveedor)
    {
        ArgumentOutOfRangeException
            .ThrowIfNegativeOrZero(
                idProveedor);

        const string sql = """
            SELECT l.id_lote,
                   l.numero_lote,
                   l.fecha_vencimiento,
                   p.nombre,
                   l.id_proveedor,
                   l.estado_lote,
                   l.stock_actual
            FROM dbo.TBL_LOTES AS l
            INNER JOIN dbo.TBL_PRODUCTOS AS p
                ON p.id_producto = l.id_producto
            WHERE l.id_proveedor = @proveedor
              AND l.estado_lote IN (
                    N'Disponible',
                    N'Bloqueado por devolucion')
              AND l.stock_actual > 0
            ORDER BY l.fecha_vencimiento,
                     l.id_lote;
            """;

        using var conexion =
            new SqlConnection(
                _opciones.CadenaConexion);

        using var comando =
            new SqlCommand(
                sql,
                conexion);

        comando.Parameters
            .Add("@proveedor", SqlDbType.Int)
            .Value =
                idProveedor;

        conexion.Open();

        return LeerDatosRemesas(
            comando);
    }

    public IReadOnlyList<DatosRemesaDto> RemesasPorVencer(
        DateOnly fechaLimite)
    {
        const string sql = """
            SELECT l.id_lote,
                   l.numero_lote,
                   l.fecha_vencimiento,
                   p.nombre,
                   l.id_proveedor,
                   l.estado_lote,
                   l.stock_actual
            FROM dbo.TBL_LOTES AS l
            INNER JOIN dbo.TBL_PRODUCTOS AS p
                ON p.id_producto = l.id_producto
            WHERE l.estado_lote = N'Disponible'
              AND l.stock_actual > 0
              AND l.fecha_vencimiento <= @limite
            ORDER BY l.fecha_vencimiento,
                     l.id_lote;
            """;

        using var conexion =
            new SqlConnection(
                _opciones.CadenaConexion);

        using var comando =
            new SqlCommand(
                sql,
                conexion);

        comando.Parameters
            .Add("@limite", SqlDbType.Date)
            .Value =
                fechaLimite.ToDateTime(
                    TimeOnly.MinValue);

        conexion.Open();

        return LeerDatosRemesas(
            comando);
    }

    public IReadOnlyList<DatosRemesaDto> RemesasDisponibles()
    {
        const string sql = """
            SELECT l.id_lote,
                   l.numero_lote,
                   l.fecha_vencimiento,
                   p.nombre,
                   l.id_proveedor,
                   l.estado_lote,
                   l.stock_actual
            FROM dbo.TBL_LOTES AS l
            INNER JOIN dbo.TBL_PRODUCTOS AS p
                ON p.id_producto = l.id_producto
            WHERE l.estado_lote IN (
                    N'Disponible',
                    N'Bloqueado por devolucion')
              AND l.stock_actual > 0
            ORDER BY l.fecha_vencimiento,
                     l.id_lote;
            """;

        using var conexion =
            new SqlConnection(
                _opciones.CadenaConexion);

        using var comando =
            new SqlCommand(
                sql,
                conexion);

        conexion.Open();

        return LeerDatosRemesas(
            comando);
    }

    public void Descontar(
        int idLote,
        int cantidad)
    {
        ArgumentOutOfRangeException
            .ThrowIfNegativeOrZero(
                idLote);

        ArgumentOutOfRangeException
            .ThrowIfNegativeOrZero(
                cantidad);

        const string sql = """
            UPDATE dbo.TBL_LOTES
            SET stock_actual = stock_actual - @cantidad,
                estado_lote =
                    CASE
                        WHEN estado_lote = N'Bloqueado por devolucion'
                            THEN N'Bloqueado por devolucion'
                        WHEN stock_actual - @cantidad = 0
                            THEN N'Agotado'
                        ELSE estado_lote
                    END
            WHERE id_lote = @id
              AND stock_actual >= @cantidad;
            """;

        var actual =
            _contextoTransaccion.ExigirActual();

        using var comando =
            new SqlCommand(
                sql,
                actual.Conexion,
                actual.Transaccion);

        comando.Parameters
            .Add("@id", SqlDbType.Int)
            .Value =
                idLote;

        comando.Parameters
            .Add("@cantidad", SqlDbType.Int)
            .Value =
                cantidad;

        if (comando.ExecuteNonQuery() != 1)
        {
            throw new InvalidOperationException(
                "La remesa no existe o no tiene existencias suficientes.");
        }
    }

    public void Restituir(
        int idLote,
        int cantidad)
    {
        ArgumentOutOfRangeException
            .ThrowIfNegativeOrZero(
                idLote);

        ArgumentOutOfRangeException
            .ThrowIfNegativeOrZero(
                cantidad);

        const string sql = """
            UPDATE dbo.TBL_LOTES
            SET stock_actual = stock_actual + @cantidad,
                estado_lote =
                    CASE
                        WHEN estado_lote = N'Bloqueado por devolucion'
                            THEN N'Bloqueado por devolucion'
                        WHEN estado_lote = N'Agotado'
                             AND fecha_vencimiento > @hoy
                            THEN N'Disponible'
                        ELSE estado_lote
                    END
            WHERE id_lote = @id;
            """;

        var actual =
            _contextoTransaccion.ExigirActual();

        using var comando =
            new SqlCommand(
                sql,
                actual.Conexion,
                actual.Transaccion);

        comando.Parameters
            .Add("@id", SqlDbType.Int)
            .Value =
                idLote;

        comando.Parameters
            .Add("@cantidad", SqlDbType.Int)
            .Value =
                cantidad;

        comando.Parameters
            .Add("@hoy", SqlDbType.Date)
            .Value =
                Hoy.ToDateTime(
                    TimeOnly.MinValue);

        if (comando.ExecuteNonQuery() != 1)
        {
            throw new InvalidOperationException(
                "La remesa indicada no existe.");
        }
    }

    public void Bloquear(
        int idLote)
    {
        ArgumentOutOfRangeException
            .ThrowIfNegativeOrZero(
                idLote);

        const string sql = """
            UPDATE dbo.TBL_LOTES
            SET estado_lote = N'Bloqueado por devolucion'
            WHERE id_lote = @id;
            """;

        var actual =
            _contextoTransaccion.ExigirActual();

        using var comando =
            new SqlCommand(
                sql,
                actual.Conexion,
                actual.Transaccion);

        comando.Parameters
            .Add("@id", SqlDbType.Int)
            .Value =
                idLote;

        if (comando.ExecuteNonQuery() != 1)
        {
            throw new InvalidOperationException(
                "La remesa indicada no existe.");
        }
    }

    public void Liberar(
        int idLote)
    {
        ArgumentOutOfRangeException
            .ThrowIfNegativeOrZero(
                idLote);

        const string sql = """
            UPDATE dbo.TBL_LOTES
            SET estado_lote =
                CASE
                    WHEN stock_actual > 0
                        THEN N'Disponible'
                    ELSE N'Agotado'
                END
            WHERE id_lote = @id;
            """;

        var actual =
            _contextoTransaccion.ExigirActual();

        using var comando =
            new SqlCommand(
                sql,
                actual.Conexion,
                actual.Transaccion);

        comando.Parameters
            .Add("@id", SqlDbType.Int)
            .Value =
                idLote;

        if (comando.ExecuteNonQuery() != 1)
        {
            throw new InvalidOperationException(
                "La remesa indicada no existe.");
        }
    }

    private static IReadOnlyList<DatosRemesaDto> LeerDatosRemesas(
        SqlCommand comando)
    {
        using var lector =
            comando.ExecuteReader();

        var resultado =
            new List<DatosRemesaDto>();

        while (lector.Read())
        {
            resultado.Add(
                LeerDatosRemesa(
                    lector));
        }

        return resultado;
    }

    private static DatosRemesaDto LeerDatosRemesa(
        SqlDataReader lector)
    {
        return new DatosRemesaDto
        {
            IdLote =
                lector.GetInt32(0),

            NumeroLote =
                lector.GetString(1),

            FechaVencimiento =
                DateOnly.FromDateTime(
                    lector.GetDateTime(2)),

            NombreMedicamento =
                lector.GetString(3),

            IdProveedor =
                lector.GetInt32(4),

            EstadoLote =
                EstadoDesdeBase(
                    lector.GetString(5)),

            StockActual =
                lector.GetInt32(6),
        };
    }

    private static EstadoLote EstadoDesdeBase(
        string estado)
    {
        return estado switch
        {
            "Disponible" =>
                EstadoLote.Disponible,

            "Bloqueado por devolucion" =>
                EstadoLote.BloqueadoPorDevolucion,

            "Agotado" =>
                EstadoLote.Agotado,

            _ =>
                throw new InvalidOperationException(
                    $"Estado de remesa desconocido: '{estado}'."),
        };
    }
}
