using System.Data;
using FarmasilApp.Modelo.Comun;
using FarmasilApp.Modelo.LOT;
using Microsoft.Data.SqlClient;

namespace FarmasilApp.Persistencia.LOT;

/// <summary>
/// Adaptador temporal de ILotes para la consulta ALV (Issue #87): lee RemesasPorVencer
/// directamente de SQL Server y delega las demás operaciones al ILotes que esté registrado.
/// No reemplaza globalmente ILotes ni modifica otros módulos.
/// Cuando LOT integre su repositorio real, se podrá retirar este adaptador.
/// </summary>
public sealed class LotesSqlVencimientos : ILotes
{
    private readonly OpcionesBaseDatos _opciones;
    private readonly ILotes _resto;

    public LotesSqlVencimientos(OpcionesBaseDatos opciones, ILotes resto)
    {
        _opciones = opciones ?? throw new ArgumentNullException(nameof(opciones));
        _resto = resto ?? throw new ArgumentNullException(nameof(resto));
    }

    public IReadOnlyList<DatosRemesaDto> RemesasPorVencer(DateOnly fechaLimite)
    {
        // Predicado IDENTICO al contrato ILotes y al proceso 1 de RNF-0010:
        // Disponible + stock > 0 + fecha_vencimiento <= fechaLimite (inclusive).
        const string sql = """
            SELECT l.id_lote, l.numero_lote, l.fecha_vencimiento,
                   p.nombre, l.id_proveedor, l.stock_actual
            FROM dbo.TBL_LOTES AS l
            INNER JOIN dbo.TBL_PRODUCTOS AS p ON p.id_producto = l.id_producto
            WHERE l.estado_lote = @estado
              AND l.stock_actual > @stockMinimo
              AND l.fecha_vencimiento <= @fechaLimite
            ORDER BY l.fecha_vencimiento, l.id_lote;
            """;

        var resultado = new List<DatosRemesaDto>();
        using var conexion = new SqlConnection(_opciones.CadenaConexion);
        using var comando = new SqlCommand(sql, conexion);
        comando.Parameters.Add("@estado", SqlDbType.NVarChar, 25).Value = "Disponible";
        comando.Parameters.Add("@stockMinimo", SqlDbType.Int).Value = 0;
        comando.Parameters.Add("@fechaLimite", SqlDbType.Date).Value = fechaLimite.ToDateTime(TimeOnly.MinValue);
        conexion.Open();
        using var lector = comando.ExecuteReader();
        while (lector.Read())
        {
            resultado.Add(new DatosRemesaDto
            {
                IdLote = lector.GetInt32(0),
                NumeroLote = lector.GetString(1),
                FechaVencimiento = DateOnly.FromDateTime(lector.GetDateTime(2)),
                NombreMedicamento = lector.GetString(3),
                IdProveedor = lector.GetInt32(4),
                EstadoLote = EstadoLote.Disponible,
                StockActual = lector.GetInt32(5),
            });
        }
        return resultado;
    }

    // El resto de ILotes conserva su proveedor normal hasta que el equipo de LOT
    // entregue la implementación completa (sprint 3); no se amplía el alcance de #87.
    public IReadOnlyList<MedicamentoVendibleDto> MedicamentosVendibles() => _resto.MedicamentosVendibles();
    public IReadOnlyList<RemesaVendibleDto> SugerirFefo(int idProducto) => _resto.SugerirFefo(idProducto);
    public IReadOnlyList<DatosRemesaDto> DatosRemesas(IReadOnlyCollection<int> idsLote) => _resto.DatosRemesas(idsLote);
    public IReadOnlyList<DatosRemesaDto> RemesasDevolvibles(int idProveedor) => _resto.RemesasDevolvibles(idProveedor);
    public IReadOnlyList<DatosRemesaDto> RemesasDisponibles() => _resto.RemesasDisponibles();
    public void Descontar(int idLote, int cantidad) => _resto.Descontar(idLote, cantidad);
    public void Restituir(int idLote, int cantidad) => _resto.Restituir(idLote, cantidad);
    public void Bloquear(int idLote) => _resto.Bloquear(idLote);
    public void Liberar(int idLote) => _resto.Liberar(idLote);
}
