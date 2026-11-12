using System.Windows.Input;

namespace FarmasilApp.VistaModelo.Comun;

/// <summary>
/// Comando asíncrono para operaciones que no deben bloquear la interfaz.
/// Evita ejecuciones simultáneas mientras la tarea está en curso.
/// </summary>
public sealed class AsyncRelayCommand : ICommand
{
    private readonly Func<Task> _ejecutar;
    private readonly Func<bool>? _puedeEjecutar;
    private bool _ejecutando;

    public AsyncRelayCommand(
        Func<Task> ejecutar,
        Func<bool>? puedeEjecutar = null)
    {
        _ejecutar = ejecutar
            ?? throw new ArgumentNullException(nameof(ejecutar));

        _puedeEjecutar = puedeEjecutar;
    }

    public event EventHandler? CanExecuteChanged;

    public bool CanExecute(object? parameter)
    {
        return !_ejecutando
            && (_puedeEjecutar?.Invoke() ?? true);
    }

    public async void Execute(object? parameter)
    {
        await ExecuteAsync();
    }

    public async Task ExecuteAsync()
    {
        if (!CanExecute(null))
        {
            return;
        }

        try
        {
            _ejecutando = true;
            NotificarPuedeEjecutar();

            await _ejecutar();
        }
        finally
        {
            _ejecutando = false;
            NotificarPuedeEjecutar();
        }
    }

    public void NotificarPuedeEjecutar()
    {
        CanExecuteChanged?.Invoke(this, EventArgs.Empty);
    }
}
