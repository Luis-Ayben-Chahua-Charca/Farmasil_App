using System.ComponentModel;
using FarmasilApp.Modelo.Comun;
using FarmasilApp.Modelo.Seguridad;
using FarmasilApp.VistaModelo.Comun;
using FarmasilApp.VistaModelo.INV;
using FarmasilApp.VistaModelo.Principal;

namespace FarmasilApp.Pruebas.VistaModelo;

public class MainViewModelPruebas
{
    [Theory]
    [InlineData(Rol.Administrador)]
    [InlineData(Rol.Tecnico)]
    public void ElMenuIncluyeInventarioParaLosRolesPermitidos(Rol rol)
    {
        var principal = CrearPrincipal(rol, out _);

        var modulo = Assert.Single(principal.Modulos);

        Assert.Equal("Inventario", modulo.Nombre);
    }

    [Fact]
    public void LaOpcionDeInventarioNavegaASuVistaModelo()
    {
        var principal = CrearPrincipal(Rol.Administrador, out var navegacion);

        Assert.Single(principal.Modulos).AbrirCommand.Execute(null);

        Assert.Equal(typeof(InventarioViewModel), navegacion.Destino);
    }

    private static MainViewModel CrearPrincipal(Rol rol, out NavegacionFalsa navegacion)
    {
        navegacion = new NavegacionFalsa();
        return new MainViewModel(navegacion, new SesionSimulada(rol));
    }

    private sealed class NavegacionFalsa : INavegacionServicio
    {
        public ViewModelBase? Actual { get; private set; }

        public Type? Destino { get; private set; }

        public event PropertyChangedEventHandler? PropertyChanged;

        public void NavegarA<TViewModel>() where TViewModel : ViewModelBase
        {
            Destino = typeof(TViewModel);
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Actual)));
        }
    }
}
