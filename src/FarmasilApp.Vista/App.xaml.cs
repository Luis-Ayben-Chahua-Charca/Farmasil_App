using System.Windows;
using FarmasilApp.Persistencia;
using FarmasilApp.Vista.Principal;
using FarmasilApp.VistaModelo.Comun;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace FarmasilApp.Vista;

/// <summary>
/// Punto de entrada de la aplicación WPF. Lee la configuración, registra las dependencias una sola vez
/// al arrancar (VP-00) y abre la ventana principal.
/// </summary>
public partial class App : Application
{
    private ServiceProvider? _proveedor;

    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);

        // appsettings.local.json es opcional y no se versiona: permite apuntar a otra instancia o base.
        var configuracion = new ConfigurationBuilder()
            .SetBasePath(AppContext.BaseDirectory)
            .AddJsonFile("appsettings.json", optional: false)
            .AddJsonFile("appsettings.local.json", optional: true)
            .Build();

        var servicios = new ServiceCollection();
        servicios.AgregarPersistencia(configuracion);
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
