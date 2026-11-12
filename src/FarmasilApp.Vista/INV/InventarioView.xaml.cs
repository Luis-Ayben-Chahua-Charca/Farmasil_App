using System.Windows;
using System.Windows.Controls;
using FarmasilApp.VistaModelo.INV;

namespace FarmasilApp.Vista.INV;

public partial class InventarioView : UserControl
{
    public InventarioView()
    {
        InitializeComponent();
    }

    private void InventarioView_Loaded(
        object sender,
        RoutedEventArgs e)
    {
        if (DataContext is InventarioViewModel vistaModelo
            && vistaModelo.InicializarCommand.CanExecute(null))
        {
            vistaModelo.InicializarCommand.Execute(null);
        }
    }
}