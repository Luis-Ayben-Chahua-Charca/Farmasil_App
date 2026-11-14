namespace FarmasilApp.Modelo.Seguridad;

/// <summary>
/// La sesión activa no tiene el rol que exige la operación (<see cref="ISesion.ExigirRol"/>).
/// </summary>
public sealed class AccesoDenegadoException : Exception
{
    public AccesoDenegadoException(string mensaje)
        : base(mensaje)
    {
    }
}
