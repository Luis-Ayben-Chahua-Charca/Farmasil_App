using FarmasilApp.Modelo.Servicios.INV;
using FarmasilApp.Persistencia.Repositorios.INV;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace FarmasilApp.Persistencia;

/// <summary>
/// Registro de dependencias de Persistencia.
/// </summary>
public static class RegistroDependencias
{
    public static IServiceCollection AgregarPersistencia(
        this IServiceCollection servicios,
        IConfiguration configuracion)
    {
        ArgumentNullException.ThrowIfNull(configuracion);

        var cadena = configuracion.GetConnectionString(
            OpcionesBaseDatos.NombreCadena);

        if (string.IsNullOrWhiteSpace(cadena))
        {
            throw new InvalidOperationException(
                $"Falta la cadena de conexión '{OpcionesBaseDatos.NombreCadena}' " +
                "en la sección ConnectionStrings de appsettings.json.");
        }

        servicios.AddSingleton(new OpcionesBaseDatos(cadena));

        servicios.AddTransient<
            IInventarioRepositorio,
            InventarioRepositorio>();

        return servicios;
    }
}
