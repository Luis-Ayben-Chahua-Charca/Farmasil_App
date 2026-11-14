using FarmasilApp.VistaModelo.Comun;

namespace FarmasilApp.Pruebas.VistaModelo;

public class ConfirmacionDialogViewModelPruebas
{
    [Fact]
    public void UsaLosIdentificadoresDelModuloSegunLaGuia()
    {
        var vistaModelo = new ConfirmacionDialogViewModel(
            "PAG",
            "Confirmar baja",
            "¿Dar de baja el método?");

        Assert.Equal("PAG-MDL-CONFIRMAR-ELIMINACION", vistaModelo.AutomationIdModal);
        Assert.Equal("PAG-TXT-MSJ", vistaModelo.AutomationIdMensaje);
        Assert.Equal("PAG-BTN-CONFIRMAR-SI", vistaModelo.AutomationIdConfirmar);
        Assert.Equal("PAG-BTN-CONFIRMAR-NO", vistaModelo.AutomationIdCancelar);
    }

    [Fact]
    public void ConfirmaCuandoSeEjecutaElComandoAfirmativo()
    {
        var vistaModelo = CrearVistaModelo();
        bool? resultado = null;
        vistaModelo.ResultadoSolicitado += valor => resultado = valor;

        vistaModelo.ConfirmarCommand.Execute(null);

        Assert.True(resultado);
    }

    [Fact]
    public void CancelaCuandoSeEjecutaElComandoNegativo()
    {
        var vistaModelo = CrearVistaModelo();
        bool? resultado = null;
        vistaModelo.ResultadoSolicitado += valor => resultado = valor;

        vistaModelo.CancelarCommand.Execute(null);

        Assert.False(resultado);
    }

    [Theory]
    [InlineData("")]
    [InlineData("pag")]
    [InlineData("PAG1")]
    public void RechazaUnPrefijoDeModuloQueNoSigueLaGuia(string modulo)
    {
        Assert.Throws<ArgumentException>(
            () => new ConfirmacionDialogViewModel(modulo, "Título", "Mensaje"));
    }

    private static ConfirmacionDialogViewModel CrearVistaModelo()
    {
        return new ConfirmacionDialogViewModel("PAG", "Confirmar baja", "¿Dar de baja el método?");
    }
}
