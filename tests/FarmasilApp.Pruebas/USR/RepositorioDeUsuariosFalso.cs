using FarmasilApp.Modelo.Comun;
using FarmasilApp.Modelo.USR;

namespace FarmasilApp.Pruebas.USR;

/// <summary>
/// Repositorio de Usuarios en memoria para probar <see cref="UsuariosServicio"/> sin base de datos.
/// </summary>
/// <remarks>
/// <para>La prueba carga las cuentas con <see cref="Agregar"/>, cada una con su hash. <see cref="ExisteNombre"/> y
/// <see cref="ContarAdministradoresActivos"/> responden según esa lista, comparando el nombre sin distinguir mayúsculas,
/// como la intercalación de la base (DEC-074).</para>
/// <para>Cada operación anota su última llamada (<c>Ultimo…</c>) para que la prueba compruebe qué llegó al repositorio,
/// por ejemplo, el hash recibido en <see cref="Crear"/> o si <see cref="Actualizar"/> pidió cambiar el hash.</para>
/// <para><see cref="MarcarConReferencias"/> simula una clave foránea: <see cref="DarDeBaja"/> devuelve
/// <see cref="ResultadoBaja.Desactivada"/> y deja la cuenta 'Inactivo', como haría la base (DEC-009).</para>
/// </remarks>
internal sealed class RepositorioDeUsuariosFalso : IUsuariosRepositorio
{
    private readonly List<CuentaFalsa> _cuentas = new();
    private readonly HashSet<int> _conReferencias = new();
    private int _siguienteId = 1;

    /// <summary>Si es <c>true</c>, <see cref="Consultar"/> falla como lo haría una base no disponible.</summary>
    public bool FallaAlConsultar { get; set; }

    /// <summary>Cuentas actuales, con su hash, para inspeccionarlas después de una operación.</summary>
    public IReadOnlyList<CuentaFalsa> Cuentas => _cuentas;

    public (string? NombreUsuario, Rol? Rol)? UltimaConsulta { get; private set; }

    public (string NombreUsuario, int? ExcluirIdUsuario)? UltimoExisteNombre { get; private set; }

    public int? UltimoContarAdministradoresActivos { get; private set; }

    public (string NombreUsuario, string Hash, Rol Rol, EstadoRegistro Estado)? UltimoCrear { get; private set; }

    public (int IdUsuario, string NombreUsuario, Rol Rol, EstadoRegistro Estado, string? HashNuevo)? UltimoActualizar { get; private set; }

    /// <summary>Si la última actualización pidió guardar un hash nuevo; <c>false</c> si conservó el vigente.</summary>
    public bool SePidioCambiarHash => UltimoActualizar?.HashNuevo is not null;

    public int? UltimaBaja { get; private set; }

    /// <summary>Agrega una cuenta y devuelve su id. Los ids empiezan en 1, como en los datos de prueba.</summary>
    public int Agregar(string nombreUsuario, Rol rol, EstadoRegistro estado = EstadoRegistro.Activo, string hash = "HASH-INICIAL")
    {
        var id = _siguienteId++;
        _cuentas.Add(new CuentaFalsa(id, nombreUsuario, hash, rol, estado));
        return id;
    }

    /// <summary>La cuenta tiene operaciones que la referencian (ventas, devoluciones o cotizaciones).</summary>
    public void MarcarConReferencias(int idUsuario) => _conReferencias.Add(idUsuario);

    public bool TieneReferencias(int idUsuario) => _conReferencias.Contains(idUsuario);

    public CuentaFalsa? Buscar(int idUsuario) => _cuentas.Find(c => c.IdUsuario == idUsuario);

    public IReadOnlyList<CuentaUsuario> Consultar(string? nombreUsuario, Rol? rol)
    {
        if (FallaAlConsultar)
        {
            throw new InvalidOperationException("Falla simulada de la base de datos.");
        }

        UltimaConsulta = (nombreUsuario, rol);
        return _cuentas
            .Where(c => nombreUsuario is null || MismoNombre(c.NombreUsuario, nombreUsuario))
            .Where(c => rol is null || c.Rol == rol)
            .Select(c => c.ComoCuenta())
            .ToList();
    }

    public CuentaUsuario? Obtener(int idUsuario) => Buscar(idUsuario)?.ComoCuenta();

    public bool ExisteNombre(string nombreUsuario, int? excluirIdUsuario)
    {
        UltimoExisteNombre = (nombreUsuario, excluirIdUsuario);
        return _cuentas.Exists(c => MismoNombre(c.NombreUsuario, nombreUsuario) && c.IdUsuario != excluirIdUsuario);
    }

    public int ContarAdministradoresActivos(int excluirIdUsuario)
    {
        UltimoContarAdministradoresActivos = excluirIdUsuario;
        return _cuentas.Count(c => c.Rol == Rol.Administrador && c.Estado == EstadoRegistro.Activo && c.IdUsuario != excluirIdUsuario);
    }

    public string? ObtenerHash(int idUsuario) => Buscar(idUsuario)?.Hash;

    /// <summary>Si es <c>true</c>, <see cref="ObtenerCredenciales"/> falla como lo haría una base no disponible.</summary>
    public bool FallaAlObtenerCredenciales { get; set; }

    /// <summary>Cuántas veces se consultaron las credenciales: una entrada vacía no debe llegar a la base.</summary>
    public int ConsultasDeCredenciales { get; private set; }

    public CredencialesUsuario? ObtenerCredenciales(string nombreUsuario)
    {
        ConsultasDeCredenciales++;
        if (FallaAlObtenerCredenciales)
        {
            throw new InvalidOperationException("Falla simulada de la base de datos.");
        }

        var cuenta = _cuentas.Find(c => MismoNombre(c.NombreUsuario, nombreUsuario));
        return cuenta is null
            ? null
            : new CredencialesUsuario
            {
                IdUsuario = cuenta.IdUsuario,
                NombreUsuario = cuenta.NombreUsuario,
                Hash = cuenta.Hash,
                Rol = cuenta.Rol,
                Estado = cuenta.Estado,
            };
    }

    public int Crear(string nombreUsuario, string hash, Rol rol, EstadoRegistro estado)
    {
        UltimoCrear = (nombreUsuario, hash, rol, estado);
        return Agregar(nombreUsuario, rol, estado, hash);
    }

    public void Actualizar(int idUsuario, string nombreUsuario, Rol rol, EstadoRegistro estado, string? hashNuevo)
    {
        UltimoActualizar = (idUsuario, nombreUsuario, rol, estado, hashNuevo);

        var indice = _cuentas.FindIndex(c => c.IdUsuario == idUsuario);
        if (indice < 0)
        {
            throw new InvalidOperationException($"La cuenta {idUsuario} no existe.");
        }

        var actual = _cuentas[indice];
        _cuentas[indice] = actual with
        {
            NombreUsuario = nombreUsuario,
            Rol = rol,
            Estado = estado,
            Hash = hashNuevo ?? actual.Hash,
        };
    }

    public ResultadoBaja DarDeBaja(int idUsuario)
    {
        UltimaBaja = idUsuario;

        var indice = _cuentas.FindIndex(c => c.IdUsuario == idUsuario);
        if (indice < 0)
        {
            throw new InvalidOperationException($"La cuenta {idUsuario} no existe.");
        }

        if (TieneReferencias(idUsuario))
        {
            _cuentas[indice] = _cuentas[indice] with { Estado = EstadoRegistro.Inactivo };
            return ResultadoBaja.Desactivada;
        }

        _cuentas.RemoveAt(indice);
        return ResultadoBaja.Eliminada;
    }

    private static bool MismoNombre(string a, string b) => string.Equals(a, b, StringComparison.OrdinalIgnoreCase);

    /// <summary>Fila de <c>TBL_USUARIOS</c> en memoria, con el hash que el tipo público <see cref="CuentaUsuario"/> no tiene.</summary>
    internal sealed record CuentaFalsa(int IdUsuario, string NombreUsuario, string Hash, Rol Rol, EstadoRegistro Estado)
    {
        public CuentaUsuario ComoCuenta() => new()
        {
            IdUsuario = IdUsuario,
            NombreUsuario = NombreUsuario,
            Rol = Rol,
            Estado = Estado,
        };
    }
}
