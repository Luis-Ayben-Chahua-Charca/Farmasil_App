using System.Data;
using FarmasilApp.Modelo.PAG;
using Microsoft.Data.SqlClient;

namespace FarmasilApp.Persistencia.PAG;

/// <summary>
/// Repositorio de la consulta de métodos de pago (ESP-0018).
/// Los criterios se envían a SQL Server mediante parámetros.
/// </summary>
public sealed class MetodosPagoRepositorio : IMetodosPagoRepositorio
{
    private readonly OpcionesBaseDatos _opciones;

    public MetodosPagoRepositorio(OpcionesBaseDatos opciones)
    {
        _opciones = opciones
            ?? throw new ArgumentNullException(nameof(opciones));
    }

    public IReadOnlyList<MetodoPagoConsulta> Consultar(
        string? nombreMetodo,
        string? estado)
    {
        const string sql = """
            SELECT
                m.id_metodo_pago,
                m.nombre_metodo,
                m.estado
            FROM dbo.TBL_METODOS_PAGO AS m
            WHERE
                (
                    @nombreMetodoLike IS NULL
                    OR m.nombre_metodo LIKE N'%' + @nombreMetodoLike + N'%' ESCAPE '\'
                )
                AND
                (
                    @estado IS NULL
                    OR m.estado = @estado
                )
            ORDER BY m.id_metodo_pago;
            """;

        var metodos = new List<MetodoPagoConsulta>();

        using var conexion = new SqlConnection(_opciones.CadenaConexion);
        using var comando = new SqlCommand(sql, conexion);

        comando.Parameters.Add(
            new SqlParameter("@nombreMetodoLike", SqlDbType.NVarChar, -1)
            {
                Value = nombreMetodo is null
                    ? DBNull.Value
                    : EscaparLike(nombreMetodo)
            });

        comando.Parameters.Add(
            new SqlParameter("@estado", SqlDbType.NVarChar, 10)
            {
                Value = (object?)estado ?? DBNull.Value
            });

        conexion.Open();

        using var lector = comando.ExecuteReader();

        while (lector.Read())
        {
            metodos.Add(new MetodoPagoConsulta
            {
                IdMetodoPago = lector.GetInt32(
                    lector.GetOrdinal("id_metodo_pago")),

                NombreMetodo = lector.GetString(
                    lector.GetOrdinal("nombre_metodo")),

                Estado = lector.GetString(
                    lector.GetOrdinal("estado"))
            });
        }

        return metodos;
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
