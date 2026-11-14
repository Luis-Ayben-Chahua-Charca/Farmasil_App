using FarmasilApp.Modelo.Comun;

namespace FarmasilApp.Modelo.Seguridad;

/// <summary>
/// Sesión activa. Aspecto transversal, no dependencia de negocio (contexto de arquitectura, §4):
/// da la autoría de las operaciones y el rol (RNF-0006). Firma formal: matriz de interfaces, sección 5 bis (DEC-069).
/// </summary>
public interface ISesion
{
    /// <summary>Usuario de la sesión activa, sin el hash de su contraseña.</summary>
    SesionUsuario UsuarioActual { get; }

    /// <summary>
    /// Exige que la sesión tenga exactamente este rol. Es una defensa adicional: la interfaz
    /// ya oculta o inhabilita lo que el rol no puede usar (DEC-050).
    /// </summary>
    /// <exception cref="AccesoDenegadoException">La sesión tiene otro rol.</exception>
    void ExigirRol(Rol rol);
}
