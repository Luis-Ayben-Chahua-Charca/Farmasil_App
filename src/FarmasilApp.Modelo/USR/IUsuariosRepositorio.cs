using FarmasilApp.Modelo.Comun;

namespace FarmasilApp.Modelo.USR;

/// <summary>
/// Persistencia de <c>TBL_USUARIOS</c>. Lo implementa la capa Persistencia y, en las pruebas, un repositorio falso.
/// </summary>
/// <remarks>
/// Patrón del módulo de referencia: el repositorio solo ofrece operaciones pequeñas y sin reglas, y
/// <see cref="UsuariosServicio"/> las combina para aplicar las reglas de la especificación. Así cada regla se prueba
/// con un repositorio en memoria, sin base de datos ni interfaz. Las comparaciones de nombre no distinguen mayúsculas,
/// como la intercalación <c>Modern_Spanish_CI_AS</c> de la base (DEC-074).
/// </remarks>
public interface IUsuariosRepositorio
{
    /// <summary>Cuentas cuyo nombre es igual a <paramref name="nombreUsuario"/> y cuyo rol es <paramref name="rol"/>; un criterio nulo no filtra.</summary>
    IReadOnlyList<CuentaUsuario> Consultar(string? nombreUsuario, Rol? rol);

    /// <summary>La cuenta con ese identificador, o <c>null</c> si no existe.</summary>
    CuentaUsuario? Obtener(int idUsuario);

    /// <summary>Si existe una cuenta con ese nombre, sin contar la cuenta <paramref name="excluirIdUsuario"/>.</summary>
    bool ExisteNombre(string nombreUsuario, int? excluirIdUsuario);

    /// <summary>Cuentas 'Administrador' y 'Activo', sin contar la cuenta <paramref name="excluirIdUsuario"/>.</summary>
    int ContarAdministradoresActivos(int excluirIdUsuario);

    /// <summary>
    /// Cadena del hash de la cuenta, o <c>null</c> si no existe. No la usa ninguna operación de Usuarios: la consumirá el
    /// inicio de sesión (#88) y nunca sale hacia la interfaz.
    /// </summary>
    string? ObtenerHash(int idUsuario);

    /// <summary>Registra la cuenta y devuelve su identificador. Un nombre repetido lanza <see cref="ReglaDeNegocioException"/>.</summary>
    int Crear(string nombreUsuario, string hash, Rol rol, EstadoRegistro estado);

    /// <summary>
    /// Actualiza nombre, rol y estado en una transacción. Si <paramref name="hashNuevo"/> es <c>null</c>, conserva el hash
    /// vigente. Un nombre repetido lanza <see cref="ReglaDeNegocioException"/>.
    /// </summary>
    void Actualizar(int idUsuario, string nombreUsuario, Rol rol, EstadoRegistro estado, string? hashNuevo);

    /// <summary>Baja condicional (DEC-009, DEC-029): borra la cuenta o, si otra tabla la referencia, la desactiva.</summary>
    ResultadoBaja DarDeBaja(int idUsuario);
}
