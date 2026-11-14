using FarmasilApp.Modelo.DOC;
using Microsoft.Data.SqlClient;
using System.Data;

namespace FarmasilApp.Persistencia.DOC;

/// <summary>
/// Repositorio de consulta de comprobantes (ESP-0010 v03.00).
/// Solo lectura; todos los filtros SQL se envían mediante parámetros.
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
                v.monto_total,
                c.estado_documento
            FROM dbo.TBL_COMPROBANTES_TRIBUTARIOS AS c
            INNER JOIN dbo.TBL_REGISTRO_VENTAS AS v
                ON v.id_venta = c.id_venta
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
                MontoTotal = lector.GetDecimal(
                    lector.GetOrdinal("monto_total")),
                EstadoDocumento = lector.GetString(
                    lector.GetOrdinal("estado_documento"))
            });
        }

        return comprobantes;
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
