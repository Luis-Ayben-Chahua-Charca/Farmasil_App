using FarmasilApp.VistaModelo.DOC;
using System.Windows.Controls;

namespace FarmasilApp.Vista.DOC;

/// <summary>
/// Vista WPF. La lógica de consulta permanece en VistaModelo y Modelo.
/// </summary>
public partial class ConsultaComprobantesView : UserControl
{
    public ConsultaComprobantesView(ConsultaComprobantesViewModel viewModel)
    {
        InitializeComponent();
        DataContext = viewModel
            ?? throw new ArgumentNullException(nameof(viewModel));
    }
}
