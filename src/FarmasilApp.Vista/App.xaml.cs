using System.Windows;
using FarmasilApp.Vista.Principal;
using FarmasilApp.VistaModelo.Comun;
using Microsoft.Extensions.DependencyInjection;

namespace FarmasilApp.Vista;

/// <summary>
/// Punto de entrada de la aplicación WPF. Registra las dependencias una sola vez al arrancar (VP-00)
/// y abre la ventana principal.
/// </summary>
public partial class App : Application
{
    private ServiceProvider? _proveedor;

    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);

        var servicios = new ServiceCollection();
        servicios.AgregarVistaModelo();
        servicios.AddSingleton<MainWindow>();
        _proveedor = servicios.BuildServiceProvider();

        _proveedor.GetRequiredService<MainWindow>().Show();
    }

    protected override void OnExit(ExitEventArgs e)
    {
        _proveedor?.Dispose();
        base.OnExit(e);
    }
}
