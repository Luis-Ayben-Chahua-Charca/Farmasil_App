using Microsoft.Extensions.DependencyInjection;

namespace FarmasilApp.VistaModelo.Comun;

/// <inheritdoc cref="INavegacionServicio"/>
public class NavegacionServicio : ViewModelBase, INavegacionServicio
{
    private readonly IServiceProvider _proveedor;
    private ViewModelBase? _actual;

    public NavegacionServicio(IServiceProvider proveedor)
    {
        _proveedor = proveedor ?? throw new ArgumentNullException(nameof(proveedor));
    }

    public ViewModelBase? Actual
    {
        get => _actual;
        private set => Establecer(ref _actual, value);
    }

    public void NavegarA<TViewModel>() where TViewModel : ViewModelBase
    {
        Actual = _proveedor.GetRequiredService<TViewModel>();
    }
}
