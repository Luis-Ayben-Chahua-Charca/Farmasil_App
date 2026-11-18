using System.Windows;
using System.Windows.Controls;

namespace FarmasilApp.Vista.Comun;

/// <summary>
/// Enlaza <see cref="PasswordBox.Password"/> con una propiedad de la VistaModelo, sin código en el code-behind.
/// </summary>
/// <remarks>
/// WPF no permite enlazar <c>Password</c> porque no es una propiedad de dependencia. Este comportamiento adjunto copia
/// el texto en los dos sentidos: lo que escribe el usuario llega a la VistaModelo, y cuando la VistaModelo limpia la
/// propiedad (por ejemplo, al abrir la edición, ESP-0035), el campo se vacía. El contenido sigue enmascarado.
/// Uso: <c>comun:ContrasenaEnlazada.Contrasena="{Binding Contrasena, Mode=TwoWay}"</c>.
/// </remarks>
public static class ContrasenaEnlazada
{
    public static readonly DependencyProperty ContrasenaProperty =
        DependencyProperty.RegisterAttached(
            "Contrasena",
            typeof(string),
            typeof(ContrasenaEnlazada),
            // El valor inicial es null, no "": así el primer valor enlazado (normalmente "") ya es un cambio y
            // AlCambiarContrasena suscribe el evento PasswordChanged.
            new FrameworkPropertyMetadata(
                null,
                FrameworkPropertyMetadataOptions.BindsTwoWayByDefault,
                AlCambiarContrasena));

    // Marca el PasswordBox mientras se copia su texto hacia la propiedad, para no volver a escribirlo.
    private static readonly DependencyProperty ActualizandoProperty =
        DependencyProperty.RegisterAttached(
            "Actualizando",
            typeof(bool),
            typeof(ContrasenaEnlazada),
            new PropertyMetadata(false));

    public static string? GetContrasena(DependencyObject elemento)
    {
        return (string?)elemento.GetValue(ContrasenaProperty);
    }

    public static void SetContrasena(DependencyObject elemento, string? valor)
    {
        elemento.SetValue(ContrasenaProperty, valor);
    }

    private static void AlCambiarContrasena(DependencyObject objeto, DependencyPropertyChangedEventArgs evento)
    {
        if (objeto is not PasswordBox caja)
        {
            return;
        }

        caja.PasswordChanged -= AlCambiarTexto;

        if (!(bool)caja.GetValue(ActualizandoProperty))
        {
            var nuevo = evento.NewValue as string ?? string.Empty;
            if (caja.Password != nuevo)
            {
                caja.Password = nuevo;
            }
        }

        caja.PasswordChanged += AlCambiarTexto;
    }

    private static void AlCambiarTexto(object sender, RoutedEventArgs e)
    {
        var caja = (PasswordBox)sender;
        caja.SetValue(ActualizandoProperty, true);
        SetContrasena(caja, caja.Password);
        caja.SetValue(ActualizandoProperty, false);
    }
}
