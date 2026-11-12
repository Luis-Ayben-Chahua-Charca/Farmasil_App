using System.Windows;
using System.Windows.Input;

namespace FarmasilApp.Vista.Comun;

/// <summary>
/// Ejecuta un comando cuando el elemento se carga.
/// Permite mantener las vistas sin lógica en el code-behind.
/// </summary>
public static class ComandoAlCargar
{
    public static readonly DependencyProperty CommandProperty =
        DependencyProperty.RegisterAttached(
            "Command",
            typeof(ICommand),
            typeof(ComandoAlCargar),
            new PropertyMetadata(null, AlCambiarCommand));

    public static void SetCommand(
        DependencyObject elemento,
        ICommand? valor)
    {
        elemento.SetValue(CommandProperty, valor);
    }

    public static ICommand? GetCommand(
        DependencyObject elemento)
    {
        return (ICommand?)elemento.GetValue(CommandProperty);
    }

    private static void AlCambiarCommand(
        DependencyObject objeto,
        DependencyPropertyChangedEventArgs evento)
    {
        if (objeto is not FrameworkElement elemento)
        {
            return;
        }

        elemento.Loaded -= AlCargar;

        if (evento.NewValue is ICommand)
        {
            elemento.Loaded += AlCargar;
        }
    }

    private static void AlCargar(
        object sender,
        RoutedEventArgs e)
    {
        if (sender is FrameworkElement elemento
            && GetCommand(elemento) is ICommand comando
            && comando.CanExecute(null))
        {
            comando.Execute(null);
        }
    }
}
