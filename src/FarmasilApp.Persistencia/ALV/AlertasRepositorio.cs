using System.Data;
using FarmasilApp.Modelo.ALV;
using Microsoft.Data.SqlClient;

namespace FarmasilApp.Persistencia.ALV;

/// <summary>Consultas de solo lectura sobre tablas propias de ALV (ESP-0013).</summary>
public sealed class AlertasRepositorio : IAlertasRepositorio
{
    private readonly OpcionesBaseDatos _opciones;
    public AlertasRepositorio(OpcionesBaseDatos opciones) =>
        _opciones = opciones ?? throw new ArgumentNullException(nameof(opciones));

    public int ObtenerUmbralMesesActivo()
    {
        const string sql = """
            SELECT TOP (2) umbral_meses
            FROM dbo.TBL_ALERTAS_VENCIMIENTO
            WHERE estado = @estado
            ORDER BY id_alerta;
            """;
        using var conexion = new SqlConnection(_opciones.CadenaConexion);
        using var comando = new SqlCommand(sql, conexion);
        comando.Parameters.Add("@estado", SqlDbType.NVarChar, 10).Value = "Activo";
        conexion.Open();
        using var lector = comando.ExecuteReader();
        if (!lector.Read())
            throw new InvalidOperationException("No se encontró una regla de vencimiento activa.");
        var umbral = lector.GetInt32(0);
        if (lector.Read())
            throw new InvalidOperationException("Hay varias reglas de vencimiento activas. Resuelva la configuración antes de consultar.");
        return umbral;
    }

    public IReadOnlyList<AlertaConsulta> ConsultarRetiros()
    {
        const string sql = """
            SELECT id_lote, nombre_producto, numero_lote,
                   fecha_vencimiento, es_lote_defectuoso
            FROM dbo.TBL_PRODUCTOS_RETIRADOS
            ORDER BY fecha_vencimiento, nombre_producto, numero_lote, id_retiro;
            """;
        using var conexion = new SqlConnection(_opciones.CadenaConexion);
        using var comando = new SqlCommand(sql, conexion);
        conexion.Open();
        using var lector = comando.ExecuteReader();
        var resultado = new List<AlertaConsulta>();
        while (lector.Read())
        {
            resultado.Add(new AlertaConsulta
            {
                IdLote = lector.IsDBNull(0) ? null : lector.GetInt32(0),
                Medicamento = lector.GetString(1),
                NumeroLote = lector.GetString(2),
                FechaVencimiento = DateOnly.FromDateTime(lector.GetDateTime(3)),
                EsLoteDefectuoso = lector.GetBoolean(4),
                Origen = "Retirado"
            });
        }
        return resultado;
    }
}
