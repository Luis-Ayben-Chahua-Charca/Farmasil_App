using FarmasilApp.Modelo.Servicios.INV;
using Microsoft.Extensions.DependencyInjection;

namespace FarmasilApp.Modelo.Comun;

/// <summary>
/// Registro de dependencias de la capa Modelo.
/// </summary>
public static class RegistroDependencias
{
    public static IServiceCollection AgregarModelo(
        this IServiceCollection servicios)
    {
        servicios.AddTransient<InventarioServicio>();

        return servicios;
    }
}
