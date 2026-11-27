using System.Windows;
using System.Windows.Threading;
using FarmasilApp.Vista.Comun;
using FarmasilApp.Modelo.ALV;
using FarmasilApp.Modelo.Comun;
using FarmasilApp.Persistencia;
using FarmasilApp.Persistencia.ALV;
using FarmasilApp.Persistencia.LOT;
using FarmasilApp.Vista.Principal;
using FarmasilApp.VistaModelo.Comun;
using FarmasilApp.VistaModelo.Principal;
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
    private CancellationTokenSource? _cancelacionEvaluacion;
    private Task? _tareaEvaluacion;

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
        servicios.AgregarModelo();
        servicios.AgregarLotesSql();
        servicios.AgregarConsultaAlertasSql();
        servicios.AgregarVistaModelo();
        servicios.AddSingleton<IMensajeServicio, MensajeServicio>();
        // Issue #84: la interfaz pertenece a VistaModelo; la implementación WPF usa MigraDoc.
        servicios.AddSingleton<IPdfServicio, PdfServicio>();
        servicios.AddSingleton<MainWindow>();
        _proveedor = servicios.BuildServiceProvider();

        _proveedor.GetRequiredService<MainWindow>().Show();
        _proveedor.GetRequiredService<MainViewModel>()
            .ConfigurarContextoAvisos(new DispatcherSynchronizationContext(Dispatcher));

        // Issue #109: se lanza después de crear la ventana, sin bloquear el hilo WPF.
        // La evaluación no requiere que se haya iniciado sesión (VP-00).
        _cancelacionEvaluacion = new CancellationTokenSource();
        _tareaEvaluacion = _proveedor.GetRequiredService<ProcesoDiarioVencimientos>()
            .EjecutarAsync(_cancelacionEvaluacion.Token);
    }

    protected override void OnExit(ExitEventArgs e)
    {
        // Al cerrar se cancela el retraso periódico; no esperamos SQL bloqueando la interfaz.
        _cancelacionEvaluacion?.Cancel();
        _cancelacionEvaluacion?.Dispose();
        _proveedor?.Dispose();
        base.OnExit(e);
    }
}
