using FarmasilApp.Modelo.ALV;
using FarmasilApp.Modelo.DEV;
using FarmasilApp.Modelo.DOC;
using FarmasilApp.Modelo.INV;
using FarmasilApp.Modelo.LOT;
using FarmasilApp.Modelo.PAG;
using FarmasilApp.Modelo.PRV;
using FarmasilApp.Modelo.REA;
using FarmasilApp.Modelo.RES;
using FarmasilApp.Modelo.Seguridad;
using FarmasilApp.Modelo.USR;
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
        servicios.AddTransient<AlertasServicio>();
        servicios.AddTransient<ComprobanteServicio>();
        servicios.AddTransient<InventarioServicio>();
        servicios.AddTransient<MetodosPagoServicio>();
        servicios.AddTransient<ProveedoresServicio>();
        servicios.AddTransient<UsuariosServicio>();

        // El hash no tiene estado: una sola instancia sirve a Usuarios y al inicio de sesión (#88).
        servicios.AddSingleton<IHashContrasenas, HashContrasenasPbkdf2>();

        // Contratos entre módulos (DEC-069): una línea por interfaz. Cuando el módulo dueño esté listo,
        // se cambia solo la implementación de su línea por la real; las simuladas no tienen estado.
        servicios.TryAddSingleton(TimeProvider.System);
        // Sesión real (#88): una sola instancia para ISesion (los módulos) e IAutenticacion (inicio de sesión y ventana
        // principal). SesionSimulada ya no se registra: queda solo en las pruebas.
        servicios.AddSingleton<SesionActual>();
        servicios.AddSingleton<ISesion>(proveedor => proveedor.GetRequiredService<SesionActual>());
        servicios.AddSingleton<IAutenticacion>(proveedor => proveedor.GetRequiredService<SesionActual>());
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
