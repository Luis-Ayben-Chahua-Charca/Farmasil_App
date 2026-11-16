using System.Windows;
using FarmasilApp.VistaModelo.Comun;

namespace FarmasilApp.Vista.Comun;

public partial class ConfirmacionDialog : Window
{
    public ConfirmacionDialog(ConfirmacionDialogViewModel vistaModelo)
    {
        InitializeComponent();
        DataContext = vistaModelo ?? throw new ArgumentNullException(nameof(vistaModelo));
    }
}
