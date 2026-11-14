using System.Windows;
using FarmasilApp.VistaModelo.Comun;

namespace FarmasilApp.Vista.Comun;

public sealed class MensajeServicio : IMensajeServicio
{
    public bool Confirmar(string modulo, string titulo, string mensaje)
    {
        var vistaModelo = new ConfirmacionDialogViewModel(modulo, titulo, mensaje);
        var dialogo = new ConfirmacionDialog(vistaModelo);
        var propietario = Application.Current?.MainWindow;

        if (propietario is not null)
        {
            dialogo.Owner = propietario;
            dialogo.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            dialogo.Width = propietario.ActualWidth;
            dialogo.Height = propietario.ActualHeight;
        }
        else
        {
            dialogo.WindowStartupLocation = WindowStartupLocation.CenterScreen;
        }

        vistaModelo.ResultadoSolicitado += resultado => dialogo.DialogResult = resultado;

        return dialogo.ShowDialog() == true;
    }

    public void MostrarExito(string titulo, string mensaje)
    {
        MostrarMensaje(titulo, mensaje, MessageBoxImage.Information);
    }

    public void MostrarError(string titulo, string mensaje)
    {
        MostrarMensaje(titulo, mensaje, MessageBoxImage.Error);
    }

    private static void MostrarMensaje(string titulo, string mensaje, MessageBoxImage icono)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(titulo);
        ArgumentException.ThrowIfNullOrWhiteSpace(mensaje);

        var propietario = Application.Current?.MainWindow;

        if (propietario is null)
        {
            MessageBox.Show(mensaje, titulo, MessageBoxButton.OK, icono);
            return;
        }

        MessageBox.Show(propietario, mensaje, titulo, MessageBoxButton.OK, icono);
    }
}
