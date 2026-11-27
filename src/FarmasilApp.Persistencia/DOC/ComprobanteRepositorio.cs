using System.Data;
using FarmasilApp.Modelo.Comun;
using FarmasilApp.Modelo.DOC;
using Microsoft.Data.SqlClient;

namespace FarmasilApp.Persistencia.DOC;

/// <summary>
/// Repositorio SQL de comprobantes tributarios. Todos los valores ingresados
/// por el usuario se envían como parámetros y las operaciones de escritura
/// se restringen a comprobantes que continúan en estado Emitido.
/// </summary>
public sealed class ComprobanteRepositorio : IComprobanteRepositorio
{
    private readonly OpcionesBaseDatos _opciones;

    public ComprobanteRepositorio(OpcionesBaseDatos opciones)
    {
        _opciones = opciones ?? throw new ArgumentNullException(nameof(opciones));
    }

    public IReadOnlyList<ComprobanteTributario> Consultar(FiltroConsultaComprobantes filtro)
    {
        ArgumentNullException.ThrowIfNull(filtro);

        const string sql = """
            SELECT
                c.id_comprobante,
                c.numero_comprobante,
                c.tipo_comprobante,
                c.fecha_emision,
                c.id_venta,
                c.estado_documento
            FROM dbo.TBL_COMPROBANTES_TRIBUTARIOS AS c
            WHERE
                (@tipoComprobante IS NULL OR c.tipo_comprobante = @tipoComprobante)
                AND (@numeroComprobanteLike IS NULL
                    OR c.numero_comprobante LIKE N'%' + @numeroComprobanteLike + N'%' ESCAPE N'\')
                AND (@fechaEmision IS NULL OR c.fecha_emision = @fechaEmision)
            ORDER BY c.fecha_emision DESC, c.id_comprobante DESC;
            """;

        var comprobantes = new List<ComprobanteTributario>();
        using var conexion = new SqlConnection(_opciones.CadenaConexion);
        using var comando = new SqlCommand(sql, conexion);
        comando.Parameters.Add(new SqlParameter("@tipoComprobante", SqlDbType.NVarChar, 20)
        {
            Value = (object?)filtro.TipoComprobante ?? DBNull.Value
        });
        comando.Parameters.Add(new SqlParameter("@numeroComprobanteLike", SqlDbType.NVarChar, 100)
        {
            Value = filtro.NumeroComprobante is null
                ? DBNull.Value
                : EscaparLike(filtro.NumeroComprobante)
        });
        comando.Parameters.Add(new SqlParameter("@fechaEmision", SqlDbType.Date)
        {
            Value = filtro.FechaEmision is null ? DBNull.Value : filtro.FechaEmision.Value.Date
        });

        conexion.Open();
        using var lector = comando.ExecuteReader();
        while (lector.Read())
        {
            comprobantes.Add(Mapear(lector));
        }

        return comprobantes;
    }

    public ComprobanteTributario? ObtenerPorId(int idComprobante)
    {
        const string sql = """
            SELECT
                id_comprobante,
                numero_comprobante,
                tipo_comprobante,
                fecha_emision,
                id_venta,
                estado_documento
            FROM dbo.TBL_COMPROBANTES_TRIBUTARIOS
            WHERE id_comprobante = @idComprobante;
            """;

        using var conexion = new SqlConnection(_opciones.CadenaConexion);
        using var comando = new SqlCommand(sql, conexion);
        comando.Parameters.Add(new SqlParameter("@idComprobante", SqlDbType.Int)
        {
            Value = idComprobante
        });

        conexion.Open();
        using var lector = comando.ExecuteReader(CommandBehavior.SingleRow);
        return lector.Read() ? Mapear(lector) : null;
    }

    public IReadOnlyList<int> ConsultarIdsVentasConComprobanteEmitido()
    {
        const string sql = """
            SELECT DISTINCT id_venta
            FROM dbo.TBL_COMPROBANTES_TRIBUTARIOS
            WHERE estado_documento = N'Emitido'
            ORDER BY id_venta;
            """;

        var idsVentas = new List<int>();
        using var conexion = new SqlConnection(_opciones.CadenaConexion);
        using var comando = new SqlCommand(sql, conexion);
        conexion.Open();
        using var lector = comando.ExecuteReader();

        while (lector.Read())
        {
            idsVentas.Add(lector.GetInt32(0));
        }

        return idsVentas;
    }

    public void Registrar(
        string numeroComprobante,
        string tipoComprobante,
        DateTime fechaEmision,
        int idVenta)
    {
        const string sql = """
            INSERT INTO dbo.TBL_COMPROBANTES_TRIBUTARIOS
                (numero_comprobante, tipo_comprobante, fecha_emision, id_venta)
            VALUES
                (@numeroComprobante, @tipoComprobante, @fechaEmision, @idVenta);
            """;

        using var conexion = new SqlConnection(_opciones.CadenaConexion);
        conexion.Open();
        using var transaccion = conexion.BeginTransaction();

        try
        {
            using var comando = new SqlCommand(sql, conexion, transaccion);
            AgregarDatosComprobante(comando, numeroComprobante, tipoComprobante, fechaEmision);
            comando.Parameters.Add(new SqlParameter("@idVenta", SqlDbType.Int) { Value = idVenta });
            comando.ExecuteNonQuery();
            transaccion.Commit();
        }
        catch (SqlException ex) when (ex.Number is 2601 or 2627)
        {
            transaccion.Rollback();
            throw TraducirDuplicado(ex);
        }
        catch
        {
            transaccion.Rollback();
            throw;
        }
    }

    public bool Actualizar(
        int idComprobante,
        string numeroComprobante,
        string tipoComprobante,
        DateTime fechaEmision)
    {
        const string sql = """
            UPDATE dbo.TBL_COMPROBANTES_TRIBUTARIOS
            SET numero_comprobante = @numeroComprobante,
                tipo_comprobante = @tipoComprobante,
                fecha_emision = @fechaEmision
            WHERE id_comprobante = @idComprobante
              AND estado_documento = N'Emitido';
            """;

        using var conexion = new SqlConnection(_opciones.CadenaConexion);
        conexion.Open();
        using var transaccion = conexion.BeginTransaction();

        try
        {
            using var comando = new SqlCommand(sql, conexion, transaccion);
            AgregarDatosComprobante(comando, numeroComprobante, tipoComprobante, fechaEmision);
            comando.Parameters.Add(new SqlParameter("@idComprobante", SqlDbType.Int)
            {
                Value = idComprobante
            });

            var filas = comando.ExecuteNonQuery();
            transaccion.Commit();
            return filas == 1;
        }
        catch (SqlException ex) when (ex.Number is 2601 or 2627)
        {
            transaccion.Rollback();
            throw TraducirDuplicado(ex);
        }
        catch
        {
            transaccion.Rollback();
            throw;
        }
    }

    public bool Anular(int idComprobante)
    {
        const string sql = """
            UPDATE dbo.TBL_COMPROBANTES_TRIBUTARIOS
            SET estado_documento = N'Anulado'
            WHERE id_comprobante = @idComprobante
              AND estado_documento = N'Emitido';
            """;

        using var conexion = new SqlConnection(_opciones.CadenaConexion);
        conexion.Open();
        using var transaccion = conexion.BeginTransaction();

        try
        {
            using var comando = new SqlCommand(sql, conexion, transaccion);
            comando.Parameters.Add(new SqlParameter("@idComprobante", SqlDbType.Int)
            {
                Value = idComprobante
            });

            var filas = comando.ExecuteNonQuery();
            transaccion.Commit();
            return filas == 1;
        }
        catch
        {
            transaccion.Rollback();
            throw;
        }
    }

    private static ComprobanteTributario Mapear(SqlDataReader lector) => new()
    {
        IdComprobante = lector.GetInt32(lector.GetOrdinal("id_comprobante")),
        NumeroComprobante = lector.GetString(lector.GetOrdinal("numero_comprobante")),
        TipoComprobante = lector.GetString(lector.GetOrdinal("tipo_comprobante")),
        FechaEmision = lector.GetDateTime(lector.GetOrdinal("fecha_emision")),
        IdVenta = lector.GetInt32(lector.GetOrdinal("id_venta")),
        EstadoDocumento = lector.GetString(lector.GetOrdinal("estado_documento"))
    };

    private static void AgregarDatosComprobante(
        SqlCommand comando,
        string numeroComprobante,
        string tipoComprobante,
        DateTime fechaEmision)
    {
        comando.Parameters.Add(new SqlParameter("@numeroComprobante", SqlDbType.NVarChar, 20)
        {
            Value = numeroComprobante
        });
        comando.Parameters.Add(new SqlParameter("@tipoComprobante", SqlDbType.NVarChar, 20)
        {
            Value = tipoComprobante
        });
        comando.Parameters.Add(new SqlParameter("@fechaEmision", SqlDbType.Date)
        {
            Value = fechaEmision.Date
        });
    }

    private static ReglaDeNegocioException TraducirDuplicado(SqlException excepcion)
    {
        if (excepcion.Message.Contains(
            "UQ_TBL_COMPROBANTES_TRIBUTARIOS_numero_comprobante",
            StringComparison.OrdinalIgnoreCase))
        {
            return new ReglaDeNegocioException(MensajesComprobantes.NumeroDuplicado);
        }

        if (excepcion.Message.Contains(
            "UX_TBL_COMPROBANTES_TRIBUTARIOS_VentaEmitido",
            StringComparison.OrdinalIgnoreCase))
        {
            return new ReglaDeNegocioException(MensajesComprobantes.VentaYaEmitida);
        }

        return new ReglaDeNegocioException(MensajesComprobantes.NumeroDuplicado);
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
