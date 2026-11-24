using FarmasilApp.Modelo.PAG;

namespace FarmasilApp.Pruebas.PAG;

internal sealed class RepositorioDeMetodosPagoFalso : IMetodosPagoRepositorio
{
    private readonly HashSet<int> _conReferencias = new();
    private int _siguienteId = 1;

    public List<MetodoPagoConsulta> Resultados { get; } = new();

    public List<int> VentasHistoricas { get; } = new();

    public (string? NombreMetodo, string? Estado)? UltimaConsulta { get; private set; }

    public (string NombreMetodo, int? ExcluirIdMetodoPago)? UltimaExistenciaNombre { get; private set; }

    public (string NombreMetodo, string Estado)? UltimoCrear { get; private set; }

    public (int IdMetodoPago, string NombreMetodo, string Estado)? UltimoActualizar { get; private set; }

    public int? UltimaBaja { get; private set; }

    public ResultadoBajaMetodoPago? UltimoResultadoBaja { get; private set; }

    public bool FallaAlConsultar { get; set; }

    public void Agregar(MetodoPagoConsulta metodo)
    {
        Resultados.Add(metodo);
        _siguienteId = Math.Max(_siguienteId, metodo.IdMetodoPago + 1);
    }

    public void MarcarConVentas(int idMetodoPago)
    {
        _conReferencias.Add(idMetodoPago);
        VentasHistoricas.Add(idMetodoPago);
    }

    public IReadOnlyList<MetodoPagoConsulta> Consultar(string? nombreMetodo, string? estado)
    {
        if (FallaAlConsultar)
        {
            throw new InvalidOperationException("Falla simulada de la base de datos.");
        }

        UltimaConsulta = (nombreMetodo, estado);
        return Resultados
            .Where(metodo => nombreMetodo is null
                || metodo.NombreMetodo.Contains(nombreMetodo, StringComparison.OrdinalIgnoreCase))
            .Where(metodo => estado is null || metodo.Estado == estado)
            .OrderBy(metodo => metodo.IdMetodoPago)
            .ToArray();
    }

    public bool ExisteNombre(string nombreMetodo, int? excluirIdMetodoPago)
    {
        UltimaExistenciaNombre = (nombreMetodo, excluirIdMetodoPago);
        return Resultados.Any(metodo =>
            metodo.IdMetodoPago != excluirIdMetodoPago
            && string.Equals(metodo.NombreMetodo, nombreMetodo, StringComparison.OrdinalIgnoreCase));
    }

    public int Crear(string nombreMetodo, string estado)
    {
        UltimoCrear = (nombreMetodo, estado);
        var id = _siguienteId++;
        Resultados.Add(new MetodoPagoConsulta
        {
            IdMetodoPago = id,
            NombreMetodo = nombreMetodo,
            Estado = estado
        });
        return id;
    }

    public void Actualizar(int idMetodoPago, string nombreMetodo, string estado)
    {
        UltimoActualizar = (idMetodoPago, nombreMetodo, estado);
        var indice = Resultados.FindIndex(metodo => metodo.IdMetodoPago == idMetodoPago);
        if (indice < 0)
        {
            throw new InvalidOperationException($"El método de pago {idMetodoPago} no existe.");
        }

        Resultados[indice] = new MetodoPagoConsulta
        {
            IdMetodoPago = idMetodoPago,
            NombreMetodo = nombreMetodo,
            Estado = estado
        };
    }

    public ResultadoBajaMetodoPago DarDeBaja(int idMetodoPago)
    {
        UltimaBaja = idMetodoPago;
        var indice = Resultados.FindIndex(metodo => metodo.IdMetodoPago == idMetodoPago);
        if (indice < 0)
        {
            throw new InvalidOperationException($"El método de pago {idMetodoPago} no existe.");
        }

        if (_conReferencias.Contains(idMetodoPago))
        {
            var metodo = Resultados[indice];
            Resultados[indice] = new MetodoPagoConsulta
            {
                IdMetodoPago = metodo.IdMetodoPago,
                NombreMetodo = metodo.NombreMetodo,
                Estado = "Inactivo"
            };
            UltimoResultadoBaja = ResultadoBajaMetodoPago.Desactivado;
            return UltimoResultadoBaja.Value;
        }

        Resultados.RemoveAt(indice);
        UltimoResultadoBaja = ResultadoBajaMetodoPago.Eliminado;
        return UltimoResultadoBaja.Value;
    }
}
