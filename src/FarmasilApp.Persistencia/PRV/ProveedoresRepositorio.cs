using FarmasilApp.Modelo.PRV;
using Microsoft.Data.SqlClient;
using System.Data;

namespace FarmasilApp.Persistencia.PRV;

/// <summary>
/// Repositorio de la consulta de Proveedores (ESP-0038).
/// Las entradas se envían a SQL Server mediante parámetros.
/// </summary>
public sealed class ProveedoresRepositorio : IProveedoresRepositorio
{
    private readonly OpcionesBaseDatos _opciones;

    public ProveedoresRepositorio(OpcionesBaseDatos opciones)
    {
        _opciones = opciones
            ?? throw new ArgumentNullException(nameof(opciones));
    }

    public IReadOnlyList<ProveedorConsulta> Consultar(
        string? busqueda,
        string? estado)
    {
        const string sql = """
            SELECT
                p.id_proveedor,
                p.ruc,
                p.razon_social,
                p.telefono,
                p.estado
            FROM dbo.TBL_PROVEEDORES AS p
            WHERE
                (
                    @busqueda IS NULL
                    OR p.ruc = @busqueda
                    OR p.razon_social LIKE '%' + @busquedaLike + '%' ESCAPE '\'
                )
                AND
                (
                    @estado IS NULL
                    OR p.estado = @estado
                )
            ORDER BY p.razon_social, p.id_proveedor;
            """;

        var proveedores = new List<ProveedorConsulta>();

        using var conexion = new SqlConnection(_opciones.CadenaConexion);
        using var comando = new SqlCommand(sql, conexion);

        comando.Parameters.Add(
            new SqlParameter("@busqueda", SqlDbType.NVarChar, 150)
            {
                Value = (object?)busqueda ?? DBNull.Value
            });

        comando.Parameters.Add(
            new SqlParameter("@busquedaLike", SqlDbType.NVarChar, 600)
            {
                Value = busqueda is null
                    ? DBNull.Value
                    : EscaparLike(busqueda)
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
            var ordTelefono = lector.GetOrdinal("telefono");

            proveedores.Add(new ProveedorConsulta
            {
                IdProveedor = lector.GetInt32(
                    lector.GetOrdinal("id_proveedor")),

                Ruc = lector.GetString(
                    lector.GetOrdinal("ruc")),

                RazonSocial = lector.GetString(
                    lector.GetOrdinal("razon_social")),

                Telefono = lector.IsDBNull(ordTelefono)
                    ? null
                    : lector.GetString(ordTelefono),

                Estado = lector.GetString(
                    lector.GetOrdinal("estado"))
            });
        }

        return proveedores;
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
