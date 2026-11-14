using FarmasilApp.Modelo.DEV;
using FarmasilApp.Modelo.INV;
using FarmasilApp.Modelo.LOT;
using FarmasilApp.Modelo.PAG;
using FarmasilApp.Modelo.PRV;
using FarmasilApp.Modelo.REA;
using FarmasilApp.Modelo.RES;
using FarmasilApp.Modelo.Seguridad;
using FarmasilApp.Modelo.VEN;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

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

        // Contratos entre módulos (DEC-069): una línea por interfaz. Cuando el módulo dueño esté listo,
        // se cambia solo la implementación de su línea por la real; las simuladas no tienen estado.
        servicios.TryAddSingleton(TimeProvider.System);
        servicios.AddSingleton<ISesion, SesionSimulada>();
        servicios.AddSingleton<IPagos, PagosSimulado>();
        servicios.AddSingleton<IRestricciones, RestriccionesSimulado>();
        servicios.AddSingleton<IProveedores, ProveedoresSimulado>();
        servicios.AddSingleton<IInventario, InventarioSimulado>();
        servicios.AddSingleton<ILotes, LotesSimulado>();
        servicios.AddSingleton<IVentas, VentasSimulado>();
        servicios.AddSingleton<IDevoluciones, DevolucionesSimulado>();
        servicios.AddSingleton<IReabastecimiento, ReabastecimientoSimulado>();

        return servicios;
    }
}
