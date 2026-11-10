using FarmasilApp.VistaModelo.Comun;

namespace FarmasilApp.Pruebas.VistaModelo;

public class ViewModelBasePruebas
{
    private class VistaModeloDePrueba : ViewModelBase
    {
        private string _nombre = "inicial";

        public string Nombre
        {
            get => _nombre;
            set => Establecer(ref _nombre, value);
        }

        public void NotificarSinNombre() => Notificar();

        public void NotificarConNombre(string propiedad) => Notificar(propiedad);
    }

    [Fact]
    public void EstablecerUnValorNuevoCambiaLaPropiedadYNotificaConSuNombre()
    {
        var vm = new VistaModeloDePrueba();
        var notificadas = new List<string?>();
        vm.PropertyChanged += (_, e) => notificadas.Add(e.PropertyName);

        vm.Nombre = "otro";

        Assert.Equal("otro", vm.Nombre);
        Assert.Equal(new[] { nameof(VistaModeloDePrueba.Nombre) }, notificadas);
    }

    [Fact]
    public void EstablecerElMismoValorNoNotifica()
    {
        var vm = new VistaModeloDePrueba();
        var notificaciones = 0;
        vm.PropertyChanged += (_, _) => notificaciones++;

        vm.Nombre = "inicial";

        Assert.Equal(0, notificaciones);
    }

    [Fact]
    public void NotificarSinArgumentoUsaElNombreDeQuienLlama()
    {
        var vm = new VistaModeloDePrueba();
        var notificadas = new List<string?>();
        vm.PropertyChanged += (_, e) => notificadas.Add(e.PropertyName);

        vm.NotificarSinNombre();

        Assert.Equal(new[] { nameof(VistaModeloDePrueba.NotificarSinNombre) }, notificadas);
    }

    [Fact]
    public void NotificarConNombreUsaEseNombre()
    {
        var vm = new VistaModeloDePrueba();
        var notificadas = new List<string?>();
        vm.PropertyChanged += (_, e) => notificadas.Add(e.PropertyName);

        vm.NotificarConNombre("MontoTotal");

        Assert.Equal(new[] { "MontoTotal" }, notificadas);
    }

    [Fact]
    public void NotificarSinSuscriptoresNoFalla()
    {
        var vm = new VistaModeloDePrueba();

        var excepcion = Record.Exception(() => vm.Nombre = "otro");

        Assert.Null(excepcion);
    }
}
