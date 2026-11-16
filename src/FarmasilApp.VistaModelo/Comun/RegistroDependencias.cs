using FarmasilApp.Modelo.Comun;
using FarmasilApp.VistaModelo.INV;
using FarmasilApp.VistaModelo.Principal;
using FarmasilApp.VistaModelo.PRV;
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
        servicios.AgregarModuloNavegable<InventarioViewModel>(
            "Inventario",
            Rol.Administrador,
            Rol.Tecnico);
        servicios.AgregarModuloNavegable<ProveedoresViewModel>(
            "Proveedores",
            Rol.Administrador);

        return servicios;
    }

    public static IServiceCollection AgregarModuloNavegable<TViewModel>(
        this IServiceCollection servicios,
        string nombre,
        params Rol[] rolesPermitidos)
        where TViewModel : ViewModelBase
    {
        ArgumentNullException.ThrowIfNull(servicios);

        servicios.AddTransient<TViewModel>();
        servicios.AddTransient<ModuloMenuViewModel>(proveedor =>
            new ModuloMenuViewModel(
                nombre,
                new RelayCommand(() =>
                    proveedor.GetRequiredService<INavegacionServicio>().NavegarA<TViewModel>()),
                rolesPermitidos));

        return servicios;
    }
}
