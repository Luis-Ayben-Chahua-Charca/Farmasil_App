using FarmasilApp.VistaModelo.Comun;

namespace FarmasilApp.Pruebas.VistaModelo;

public class RelayCommandPruebas
{
    [Fact]
    public void ExecuteInvocaLaAccion()
    {
        var llamadas = 0;
        var comando = new RelayCommand(() => llamadas++);

        comando.Execute(null);

        Assert.Equal(1, llamadas);
    }

    [Fact]
    public void SinCondicionElComandoSiempreSePuedeEjecutar()
    {
        var comando = new RelayCommand(() => { });

        Assert.True(comando.CanExecute(null));
    }

    [Fact]
    public void CanExecuteRespetaLaCondicion()
    {
        var habilitado = false;
        var comando = new RelayCommand(() => { }, () => habilitado);

        Assert.False(comando.CanExecute(null));

        habilitado = true;

        Assert.True(comando.CanExecute(null));
    }

    [Fact]
    public void NotificarPuedeEjecutarLevantaCanExecuteChanged()
    {
        var comando = new RelayCommand(() => { });
        var levantado = 0;
        comando.CanExecuteChanged += (_, _) => levantado++;

        comando.NotificarPuedeEjecutar();

        Assert.Equal(1, levantado);
    }

    [Fact]
    public void LaAccionEsObligatoria()
    {
        Assert.Throws<ArgumentNullException>(() => new RelayCommand(null!));
    }

    [Fact]
    public void LaVersionGenericaRecibeElParametro()
    {
        int? recibido = null;
        var comando = new RelayCommand<int>(valor => recibido = valor);

        comando.Execute(42);

        Assert.Equal(42, recibido);
    }

    [Fact]
    public void LaVersionGenericaUsaElValorPorDefectoSiElParametroNoEsDelTipo()
    {
        string? recibido = "sin llamar";
        var comando = new RelayCommand<string>(valor => recibido = valor);

        comando.Execute(123);

        Assert.Null(recibido);
    }

    [Fact]
    public void LaVersionGenericaEvaluaLaCondicionConElParametro()
    {
        var comando = new RelayCommand<int>(_ => { }, valor => valor > 0);

        Assert.True(comando.CanExecute(5));
        Assert.False(comando.CanExecute(0));
    }

    [Fact]
    public void LaVersionGenericaLevantaCanExecuteChanged()
    {
        var comando = new RelayCommand<int>(_ => { });
        var levantado = 0;
        comando.CanExecuteChanged += (_, _) => levantado++;

        comando.NotificarPuedeEjecutar();

        Assert.Equal(1, levantado);
    }
}
