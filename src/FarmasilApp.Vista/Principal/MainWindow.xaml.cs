using System.Windows;
using FarmasilApp.VistaModelo.Principal;

namespace FarmasilApp.Vista.Principal;

/// <summary>Ventana principal. Sin lógica: se enlaza a su VistaModelo por DataContext.</summary>
public partial class MainWindow : Window
{
    public MainWindow(MainViewModel vistaModelo)
    {
        InitializeComponent();
        DataContext = vistaModelo;
    }
}
