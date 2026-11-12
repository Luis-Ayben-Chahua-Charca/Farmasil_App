using FarmasilApp.Modelo.INV;
using Microsoft.Extensions.DependencyInjection;

namespace FarmasilApp.Modelo.Comun;

/// <summary>
/// Registro de las dependencias de la capa Modelo. La aplicación lo invoca una sola vez al arrancar (VP-00).
/// </summary>
public static class RegistroDependencias
{
    /// <summary>
    /// Cada servicio del Modelo y cada interfaz entre módulos (<c>ILotes</c>, <c>IPagos</c>, …) se registra aquí,
    /// con su implementación simulada mientras el módulo dueño no esté listo (plan, sección 10).
    /// </summary>
    public static IServiceCollection AgregarModelo(this IServiceCollection servicios)
    {
        servicios.AddTransient<InventarioServicio>();

        return servicios;
    }
}
