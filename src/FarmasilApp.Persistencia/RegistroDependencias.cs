using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace FarmasilApp.Persistencia;

/// <summary>
/// Registro de las dependencias de la capa Persistencia. La aplicación lo invoca una sola vez al arrancar (VP-00).
/// Cada repositorio nuevo se agrega aquí.
/// </summary>
public static class RegistroDependencias
{
    public static IServiceCollection AgregarPersistencia(this IServiceCollection servicios, IConfiguration configuracion)
    {
        ArgumentNullException.ThrowIfNull(configuracion);

        var cadena = configuracion.GetConnectionString(OpcionesBaseDatos.NombreCadena);
        if (string.IsNullOrWhiteSpace(cadena))
        {
            throw new InvalidOperationException(
                $"Falta la cadena de conexión '{OpcionesBaseDatos.NombreCadena}' en la sección ConnectionStrings de appsettings.json.");
        }

        servicios.AddSingleton(new OpcionesBaseDatos(cadena));
        return servicios;
    }
}
