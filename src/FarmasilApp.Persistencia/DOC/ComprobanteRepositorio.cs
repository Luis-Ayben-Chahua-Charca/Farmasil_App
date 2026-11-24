using FarmasilApp.Modelo.DOC;
using Microsoft.Data.SqlClient;
using System.Data;
using FarmasilApp.Modelo.Comun;
namespace FarmasilApp.Persistencia.DOC;

/// <summary>
/// Repositorio de consulta y registro de comprobantes tributarios.
/// Los filtros y datos de escritura SQL se envían mediante parámetros.
/// </summary>
public sealed class ComprobanteRepositorio : IComprobanteRepositorio
{
    private readonly OpcionesBaseDatos _opciones;

    public ComprobanteRepositorio(OpcionesBaseDatos opciones)
    {
        _opciones = opciones
            ?? throw new ArgumentNullException(nameof(opciones));
    }

    public IReadOnlyList<ComprobanteTributario> Consultar(
        FiltroConsultaComprobantes filtro)
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
                (@tipoComprobante IS NULL
                    OR c.tipo_comprobante = @tipoComprobante)
                AND
                (@numeroComprobanteLike IS NULL
                    OR c.numero_comprobante LIKE N'%' + @numeroComprobanteLike + N'%' ESCAPE N'\')
                AND
                (@fechaEmision IS NULL
                    OR c.fecha_emision = @fechaEmision)
            ORDER BY c.fecha_emision DESC, c.id_comprobante DESC;
            """;

        var comprobantes = new List<ComprobanteTributario>();

        using var conexion = new SqlConnection(_opciones.CadenaConexion);
        using var comando = new SqlCommand(sql, conexion);

        comando.Parameters.Add(
            new SqlParameter("@tipoComprobante", SqlDbType.NVarChar, 20)
            {
                Value = (object?)filtro.TipoComprobante ?? DBNull.Value
            });

        comando.Parameters.Add(
            new SqlParameter("@numeroComprobanteLike", SqlDbType.NVarChar, 100)
            {
                Value = filtro.NumeroComprobante is null
                    ? DBNull.Value
                    : EscaparLike(filtro.NumeroComprobante)
            });

        comando.Parameters.Add(
            new SqlParameter("@fechaEmision", SqlDbType.Date)
            {
                Value = filtro.FechaEmision is null
                    ? DBNull.Value
                    : filtro.FechaEmision.Value.Date
            });

        conexion.Open();

        using var lector = comando.ExecuteReader();

        while (lector.Read())
        {
            comprobantes.Add(new ComprobanteTributario
            {
                IdComprobante = lector.GetInt32(
                    lector.GetOrdinal("id_comprobante")),
                NumeroComprobante = lector.GetString(
                    lector.GetOrdinal("numero_comprobante")),
                TipoComprobante = lector.GetString(
                    lector.GetOrdinal("tipo_comprobante")),
                FechaEmision = lector.GetDateTime(
                    lector.GetOrdinal("fecha_emision")),
                IdVenta = lector.GetInt32(
                    lector.GetOrdinal("id_venta")),
                MontoTotal = 0m,
                EstadoDocumento = lector.GetString(
                    lector.GetOrdinal("estado_documento"))
            });
        }

        return comprobantes;
    }
public IReadOnlyList<int> ConsultarIdsVentasConComprobanteEmitido()
{
    const string sql = """
        SELECT DISTINCT c.id_venta
        FROM dbo.TBL_COMPROBANTES_TRIBUTARIOS AS c
        WHERE c.estado_documento = N'Emitido'
        ORDER BY c.id_venta;
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
        (
            numero_comprobante,
            tipo_comprobante,
            fecha_emision,
            id_venta
        )
        VALUES
        (
            @numeroComprobante,
            @tipoComprobante,
            @fechaEmision,
            @idVenta
        );
        """;

    using var conexion = new SqlConnection(_opciones.CadenaConexion);

    conexion.Open();

    using var transaccion = conexion.BeginTransaction();

    try
    {
        using var comando = new SqlCommand(sql, conexion, transaccion);

        comando.Parameters.Add(
            new SqlParameter("@numeroComprobante", SqlDbType.NVarChar, 20)
            {
                Value = numeroComprobante
            });

        comando.Parameters.Add(
            new SqlParameter("@tipoComprobante", SqlDbType.NVarChar, 20)
            {
                Value = tipoComprobante
            });

        comando.Parameters.Add(
            new SqlParameter("@fechaEmision", SqlDbType.Date)
            {
                Value = fechaEmision.Date
            });

        comando.Parameters.Add(
            new SqlParameter("@idVenta", SqlDbType.Int)
            {
                Value = idVenta
            });

        comando.ExecuteNonQuery();

        transaccion.Commit();
    }
    catch (SqlException ex) when (ex.Number is 2601 or 2627)
{
    transaccion.Rollback();

    if (ex.Message.Contains(
        "UQ_TBL_COMPROBANTES_TRIBUTARIOS_numero_comprobante",
        StringComparison.OrdinalIgnoreCase))
    {
        throw new ReglaDeNegocioException(
            MensajesComprobantes.NumeroDuplicado);
    }

    if (ex.Message.Contains(
        "UX_TBL_COMPROBANTES_TRIBUTARIOS_VentaEmitido",
        StringComparison.OrdinalIgnoreCase))
    {
        throw new ReglaDeNegocioException(
            MensajesComprobantes.VentaYaEmitida);
    }

    throw;
}
catch
{
    transaccion.Rollback();
    throw;
}
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
