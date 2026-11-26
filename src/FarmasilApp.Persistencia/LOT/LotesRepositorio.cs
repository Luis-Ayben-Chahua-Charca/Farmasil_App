using System.Data;
using System.Globalization;
using FarmasilApp.Modelo.Comun;
using FarmasilApp.Modelo.LOT;
using Microsoft.Data.SqlClient;

namespace FarmasilApp.Persistencia.LOT;

/// <summary>
/// Persistencia real del módulo LOT para ESP-0041 a ESP-0044.
/// Escribe únicamente TBL_LOTES y realiza las lecturas autorizadas
/// sobre las tablas de devoluciones.
/// </summary>
public sealed class LotesRepositorio : ILotesRepositorio
{
    private const int ErrorConflictoDeRestriccion = 547;
    private const string PuntoDeGuardado = "antes_baja_remesa";

    private readonly OpcionesBaseDatos _opciones;

    public LotesRepositorio(OpcionesBaseDatos opciones)
    {
        _opciones = opciones
            ?? throw new ArgumentNullException(nameof(opciones));
    }

    public IReadOnlyList<RemesaConsulta> Consultar(
        int? idProducto,
        string? numeroLote,
        EstadoLote? estado)
    {
        const string sql = """
            SELECT
                l.id_lote,
                l.id_producto,
                p.nombre AS nombre_medicamento,
                l.numero_lote,
                l.fecha_vencimiento,
                l.stock_actual,
                l.estado_lote,
                l.fecha_ingreso,
                l.id_proveedor
            FROM dbo.TBL_LOTES AS l
            INNER JOIN dbo.TBL_PRODUCTOS AS p
                ON p.id_producto = l.id_producto
            WHERE
                (
                    @idProducto IS NULL
                    OR l.id_producto = @idProducto
                )
                AND
                (
                    @numeroLote IS NULL
                    OR l.numero_lote LIKE '%' + @numeroLoteLike + '%' ESCAPE '\'
                )
                AND
                (
                    @estado IS NULL
                    OR l.estado_lote = @estado
                )
            ORDER BY
                l.fecha_vencimiento,
                l.id_lote;
            """;

        var resultado = new List<RemesaConsulta>();

        using var conexion = Abrir();
        using var comando = new SqlCommand(sql, conexion);

        comando.Parameters.Add(
            new SqlParameter("@idProducto", SqlDbType.Int)
            {
                Value = (object?)idProducto ?? DBNull.Value
            });

        comando.Parameters.Add(
            new SqlParameter("@numeroLote", SqlDbType.NVarChar, 30)
            {
                Value = (object?)numeroLote ?? DBNull.Value
            });

        comando.Parameters.Add(
            new SqlParameter("@numeroLoteLike", SqlDbType.NVarChar, 120)
            {
                Value = numeroLote is null
                    ? DBNull.Value
                    : EscaparLike(numeroLote)
            });

        comando.Parameters.Add(
            new SqlParameter("@estado", SqlDbType.NVarChar, 25)
            {
                Value = estado.HasValue
                    ? EstadoSql(estado.Value)
                    : DBNull.Value
            });

        using var lector = comando.ExecuteReader();

        while (lector.Read())
        {
            resultado.Add(LeerRemesa(lector));
        }

        return resultado;
    }

    public RemesaConsulta? ObtenerPorId(int idLote)
    {
        const string sql = """
            SELECT
                l.id_lote,
                l.id_producto,
                p.nombre AS nombre_medicamento,
                l.numero_lote,
                l.fecha_vencimiento,
                l.stock_actual,
                l.estado_lote,
                l.fecha_ingreso,
                l.id_proveedor
            FROM dbo.TBL_LOTES AS l
            INNER JOIN dbo.TBL_PRODUCTOS AS p
                ON p.id_producto = l.id_producto
            WHERE l.id_lote = @idLote;
            """;

        using var conexion = Abrir();
        using var comando = new SqlCommand(sql, conexion);

        comando.Parameters.Add(
            new SqlParameter("@idLote", SqlDbType.Int)
            {
                Value = idLote
            });

        using var lector = comando.ExecuteReader();

        return lector.Read()
            ? LeerRemesa(lector)
            : null;
    }

    public bool ExisteNumeroLote(
        int idProducto,
        string numeroLote,
        int? excluirIdLote = null)
    {
        const string sql = """
            SELECT CASE
                WHEN EXISTS
                (
                    SELECT 1
                    FROM dbo.TBL_LOTES
                    WHERE id_producto = @idProducto
                      AND numero_lote = @numeroLote
                      AND
                      (
                          @excluirIdLote IS NULL
                          OR id_lote <> @excluirIdLote
                      )
                )
                THEN 1
                ELSE 0
            END;
            """;

        using var conexion = Abrir();
        using var comando = new SqlCommand(sql, conexion);

        comando.Parameters.Add(
            new SqlParameter("@idProducto", SqlDbType.Int)
            {
                Value = idProducto
            });

        comando.Parameters.Add(
            new SqlParameter("@numeroLote", SqlDbType.NVarChar, 30)
            {
                Value = numeroLote
            });

        comando.Parameters.Add(
            new SqlParameter("@excluirIdLote", SqlDbType.Int)
            {
                Value = (object?)excluirIdLote ?? DBNull.Value
            });

        return Convert.ToInt32(
            comando.ExecuteScalar(),
            CultureInfo.InvariantCulture) == 1;
    }

    public int Crear(NuevaRemesa remesa)
    {
        ArgumentNullException.ThrowIfNull(remesa);

        const string sql = """
            INSERT INTO dbo.TBL_LOTES
            (
                id_producto,
                numero_lote,
                fecha_vencimiento,
                stock_actual,
                estado_lote,
                fecha_ingreso,
                id_proveedor
            )
            VALUES
            (
                @idProducto,
                @numeroLote,
                @fechaVencimiento,
                @stockInicial,
                N'Disponible',
                @fechaIngreso,
                @idProveedor
            );

            SELECT CAST(SCOPE_IDENTITY() AS int);
            """;

        using var conexion = Abrir();
        using var comando = new SqlCommand(sql, conexion);

        comando.Parameters.Add(
            new SqlParameter("@idProducto", SqlDbType.Int)
            {
                Value = remesa.IdProducto
            });

        comando.Parameters.Add(
            new SqlParameter("@numeroLote", SqlDbType.NVarChar, 30)
            {
                Value = remesa.NumeroLote
            });

        comando.Parameters.Add(
            new SqlParameter("@fechaVencimiento", SqlDbType.Date)
            {
                Value = remesa.FechaVencimiento.ToDateTime(
                    TimeOnly.MinValue)
            });

        comando.Parameters.Add(
            new SqlParameter("@stockInicial", SqlDbType.Int)
            {
                Value = remesa.StockInicial
            });

        comando.Parameters.Add(
            new SqlParameter("@fechaIngreso", SqlDbType.Date)
            {
                Value = remesa.FechaIngreso.ToDateTime(
                    TimeOnly.MinValue)
            });

        comando.Parameters.Add(
            new SqlParameter("@idProveedor", SqlDbType.Int)
            {
                Value = remesa.IdProveedor
            });

        return Convert.ToInt32(
            comando.ExecuteScalar(),
            CultureInfo.InvariantCulture);
    }

    public bool TieneDetalleDevolucion(int idLote)
    {
        const string sql = """
            SELECT CASE
                WHEN EXISTS
                (
                    SELECT 1
                    FROM dbo.TBL_DETALLE_DEVOLUCION
                    WHERE id_lote = @idLote
                )
                THEN 1
                ELSE 0
            END;
            """;

        using var conexion = Abrir();
        using var comando = new SqlCommand(sql, conexion);

        comando.Parameters.Add(
            new SqlParameter("@idLote", SqlDbType.Int)
            {
                Value = idLote
            });

        return Convert.ToInt32(
            comando.ExecuteScalar(),
            CultureInfo.InvariantCulture) == 1;
    }

    public int CantidadComprometidaPendiente(int idLote)
    {
        const string sql = """
            SELECT COALESCE(
                SUM(d.cantidad_devolver),
                0
            )
            FROM dbo.TBL_DETALLE_DEVOLUCION AS d
            INNER JOIN dbo.TBL_ORDENES_DEVOLUCION AS o
                ON o.id_devolucion = d.id_devolucion
            WHERE d.id_lote = @idLote
              AND o.estado_orden = N'Pendiente';
            """;

        using var conexion = Abrir();
        using var comando = new SqlCommand(sql, conexion);

        comando.Parameters.Add(
            new SqlParameter("@idLote", SqlDbType.Int)
            {
                Value = idLote
            });

        return Convert.ToInt32(
            comando.ExecuteScalar(),
            CultureInfo.InvariantCulture);
    }

    public void Actualizar(
        int idLote,
        ActualizacionRemesa remesa,
        EstadoLote estadoNuevo)
    {
        ArgumentNullException.ThrowIfNull(remesa);

        const string sql = """
            UPDATE dbo.TBL_LOTES
            SET
                numero_lote = @numeroLote,
                fecha_vencimiento = @fechaVencimiento,
                stock_actual = @stockActual,
                estado_lote = @estadoLote,
                id_proveedor = @idProveedor
            WHERE id_lote = @idLote;
            """;

        using var conexion = Abrir();
        using var comando = new SqlCommand(sql, conexion);

        comando.Parameters.Add(
            new SqlParameter("@numeroLote", SqlDbType.NVarChar, 30)
            {
                Value = remesa.NumeroLote
            });

        comando.Parameters.Add(
            new SqlParameter("@fechaVencimiento", SqlDbType.Date)
            {
                Value = remesa.FechaVencimiento.ToDateTime(
                    TimeOnly.MinValue)
            });

        comando.Parameters.Add(
            new SqlParameter("@stockActual", SqlDbType.Int)
            {
                Value = remesa.StockActual
            });

        comando.Parameters.Add(
            new SqlParameter("@estadoLote", SqlDbType.NVarChar, 25)
            {
                Value = EstadoSql(estadoNuevo)
            });

        comando.Parameters.Add(
            new SqlParameter("@idProveedor", SqlDbType.Int)
            {
                Value = remesa.IdProveedor
            });

        comando.Parameters.Add(
            new SqlParameter("@idLote", SqlDbType.Int)
            {
                Value = idLote
            });

        if (comando.ExecuteNonQuery() != 1)
        {
            throw new InvalidOperationException(
                $"La remesa {idLote} no existe.");
        }
    }

    public bool TieneDevolucionPendiente(int idLote)
    {
        const string sql = """
            SELECT CASE
                WHEN EXISTS
                (
                    SELECT 1
                    FROM dbo.TBL_DETALLE_DEVOLUCION AS d
                    INNER JOIN dbo.TBL_ORDENES_DEVOLUCION AS o
                        ON o.id_devolucion = d.id_devolucion
                    WHERE d.id_lote = @idLote
                      AND o.estado_orden = N'Pendiente'
                )
                THEN 1
                ELSE 0
            END;
            """;

        using var conexion = Abrir();
        using var comando = new SqlCommand(sql, conexion);

        comando.Parameters.Add(
            new SqlParameter("@idLote", SqlDbType.Int)
            {
                Value = idLote
            });

        return Convert.ToInt32(
            comando.ExecuteScalar(),
            CultureInfo.InvariantCulture) == 1;
    }

    /// <summary>
    /// ESP-0044 / DEC-029.
    /// Intenta primero el DELETE físico. Si SQL Server devuelve 547
    /// por una referencia y la transacción continúa confirmable,
    /// vuelve al SAVEPOINT y realiza la baja lógica.
    /// </summary>
    public ResultadoBajaRemesa DarDeBaja(int idLote)
    {
        const string sqlBorrar = """
            DELETE FROM dbo.TBL_LOTES
            WHERE id_lote = @idLote;
            """;

        const string sqlAgotar = """
            UPDATE dbo.TBL_LOTES
            SET
                estado_lote = N'Agotado',
                stock_actual = 0
            WHERE id_lote = @idLote;
            """;

        using var conexion = Abrir();

        using (var xactAbort =
               new SqlCommand("SET XACT_ABORT OFF;", conexion))
        {
            xactAbort.ExecuteNonQuery();
        }

        using var transaccion =
            conexion.BeginTransaction();

        transaccion.Save(PuntoDeGuardado);

        try
        {
            using var borrar =
                new SqlCommand(
                    sqlBorrar,
                    conexion,
                    transaccion);

            borrar.Parameters.Add(
                new SqlParameter("@idLote", SqlDbType.Int)
                {
                    Value = idLote
                });

            if (borrar.ExecuteNonQuery() != 1)
            {
                throw new InvalidOperationException(
                    $"La remesa {idLote} no existe.");
            }

            transaccion.Commit();

            return ResultadoBajaRemesa.Eliminada;
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
                    $"La baja de la remesa {idLote} no se pudo completar: " +
                    "la transacción quedó no confirmable.",
                    ex);
            }

            transaccion.Rollback(PuntoDeGuardado);

            using var agotar =
                new SqlCommand(
                    sqlAgotar,
                    conexion,
                    transaccion);

            agotar.Parameters.Add(
                new SqlParameter("@idLote", SqlDbType.Int)
                {
                    Value = idLote
                });

            if (agotar.ExecuteNonQuery() != 1)
            {
                throw new InvalidOperationException(
                    $"La remesa {idLote} no existe.");
            }

            transaccion.Commit();

            return ResultadoBajaRemesa.Agotada;
        }
    }

    private SqlConnection Abrir()
    {
        var conexion =
            new SqlConnection(_opciones.CadenaConexion);

        conexion.Open();

        return conexion;
    }

    private static RemesaConsulta LeerRemesa(
        SqlDataReader lector)
    {
        return new RemesaConsulta
        {
            IdLote = lector.GetInt32(
                lector.GetOrdinal("id_lote")),

            IdProducto = lector.GetInt32(
                lector.GetOrdinal("id_producto")),

            NombreMedicamento = lector.GetString(
                lector.GetOrdinal("nombre_medicamento")),

            NumeroLote = lector.GetString(
                lector.GetOrdinal("numero_lote")),

            FechaVencimiento = DateOnly.FromDateTime(
                lector.GetDateTime(
                    lector.GetOrdinal("fecha_vencimiento"))),

            StockActual = lector.GetInt32(
                lector.GetOrdinal("stock_actual")),

            Estado = ConvertirEstado(
                lector.GetString(
                    lector.GetOrdinal("estado_lote"))),

            FechaIngreso = DateOnly.FromDateTime(
                lector.GetDateTime(
                    lector.GetOrdinal("fecha_ingreso"))),

            IdProveedor = lector.GetInt32(
                lector.GetOrdinal("id_proveedor")),
        };
    }

    private static EstadoLote ConvertirEstado(string estado)
    {
        return estado switch
        {
            "Disponible" =>
                EstadoLote.Disponible,

            "Bloqueado por devolucion" =>
                EstadoLote.BloqueadoPorDevolucion,

            "Agotado" =>
                EstadoLote.Agotado,

            _ => throw new InvalidOperationException(
                $"Estado de remesa desconocido: '{estado}'."),
        };
    }

    private static string EstadoSql(EstadoLote estado)
    {
        return estado switch
        {
            EstadoLote.Disponible =>
                "Disponible",

            EstadoLote.BloqueadoPorDevolucion =>
                "Bloqueado por devolucion",

            EstadoLote.Agotado =>
                "Agotado",

            _ => throw new ArgumentOutOfRangeException(
                nameof(estado)),
        };
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
