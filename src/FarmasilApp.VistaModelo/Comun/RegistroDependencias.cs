using FarmasilApp.Modelo.Comun;
using FarmasilApp.VistaModelo.ALV;
using FarmasilApp.VistaModelo.INV;
using FarmasilApp.VistaModelo.Principal;
using FarmasilApp.VistaModelo.PAG;
using FarmasilApp.VistaModelo.PRV;
using FarmasilApp.VistaModelo.USR;
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

        // Menú por rol: un módulo por línea, en el orden del catálogo, con los roles que la matriz de permisos
        // le concede para abrirlo. Los demás roles lo ven deshabilitado, con el motivo (DEC-075).
        servicios.AgregarModuloNavegable<AlertasViewModel>("Alertas de vencimiento", Rol.Administrador, Rol.Tecnico);
        servicios.AgregarModuloNavegable<InventarioViewModel>("Inventario", Rol.Administrador, Rol.Tecnico);
        servicios.AgregarModuloNavegable<MetodosPagoViewModel>("Métodos de pago", Rol.Administrador, Rol.Tecnico);
        servicios.AgregarModuloNavegable<UsuariosViewModel>("Usuarios", Rol.Administrador);
        servicios.AgregarModuloNavegable<ProveedoresViewModel>("Proveedores", Rol.Administrador);

        return servicios;
    }

    /// <summary>
    /// Registra la VistaModelo de un módulo como transitoria y su opción del menú principal, con los roles que
    /// pueden abrirlo según la matriz de permisos. No se registra la VistaModelo otra vez con <c>AddTransient</c>.
    /// </summary>
    public static IServiceCollection AgregarModuloNavegable<TViewModel>(
        this IServiceCollection servicios,
        string nombre,
        params Rol[] rolesPermitidos)
        where TViewModel : ViewModelBase
    {
        ArgumentNullException.ThrowIfNull(servicios);

        servicios.AddTransient<TViewModel>();
        servicios.AddSingleton(proveedor =>
            new ModuloMenu(
                nombre,
                () => proveedor.GetRequiredService<INavegacionServicio>().NavegarA<TViewModel>(),
                rolesPermitidos));

        return servicios;
    }
}
