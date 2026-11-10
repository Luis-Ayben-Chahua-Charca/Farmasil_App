using System.Windows.Input;

namespace FarmasilApp.VistaModelo.Comun;

/// <summary>
/// Comando base para los botones de la Vista (ICommand de VL-04).
/// La VistaModelo lo expone como propiedad y la Vista lo enlaza, sin lógica en el code-behind.
/// </summary>
public class RelayCommand : ICommand
{
    private readonly Action _ejecutar;
    private readonly Func<bool>? _puedeEjecutar;

    public RelayCommand(Action ejecutar, Func<bool>? puedeEjecutar = null)
    {
        _ejecutar = ejecutar ?? throw new ArgumentNullException(nameof(ejecutar));
        _puedeEjecutar = puedeEjecutar;
    }

    public event EventHandler? CanExecuteChanged;

    public bool CanExecute(object? parameter) => _puedeEjecutar?.Invoke() ?? true;

    public void Execute(object? parameter) => _ejecutar();

    /// <summary>Pide a la Vista que vuelva a consultar si el comando puede ejecutarse.</summary>
    public void NotificarPuedeEjecutar() => CanExecuteChanged?.Invoke(this, EventArgs.Empty);
}

/// <summary>Comando base cuyo método recibe el parámetro que envía la Vista (por ejemplo, la fila seleccionada).</summary>
public class RelayCommand<T> : ICommand
{
    private readonly Action<T?> _ejecutar;
    private readonly Func<T?, bool>? _puedeEjecutar;

    public RelayCommand(Action<T?> ejecutar, Func<T?, bool>? puedeEjecutar = null)
    {
        _ejecutar = ejecutar ?? throw new ArgumentNullException(nameof(ejecutar));
        _puedeEjecutar = puedeEjecutar;
    }

    public event EventHandler? CanExecuteChanged;

    public bool CanExecute(object? parameter) => _puedeEjecutar?.Invoke(Convertir(parameter)) ?? true;

    public void Execute(object? parameter) => _ejecutar(Convertir(parameter));

    public void NotificarPuedeEjecutar() => CanExecuteChanged?.Invoke(this, EventArgs.Empty);

    private static T? Convertir(object? parametro) => parametro is T valor ? valor : default;
}
