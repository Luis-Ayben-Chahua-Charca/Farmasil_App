using FarmasilApp.VistaModelo.Principal;
using Microsoft.Extensions.DependencyInjection;

namespace FarmasilApp.VistaModelo.Comun;

/// <summary>
/// Registro de las dependencias de la capa VistaModelo. La aplicación lo invoca una sola vez al arrancar (VP-00).
/// Cada VistaModelo nueva se agrega aquí.
/// </summary>
public static class RegistroDependencias
{
    public static IServiceCollection AgregarVistaModelo(this IServiceCollection servicios)
    {
        servicios.AddSingleton<INavegacionServicio, NavegacionServicio>();

        servicios.AddSingleton<MainViewModel>();
        servicios.AddTransient<InicioViewModel>();

        return servicios;
    }
}
