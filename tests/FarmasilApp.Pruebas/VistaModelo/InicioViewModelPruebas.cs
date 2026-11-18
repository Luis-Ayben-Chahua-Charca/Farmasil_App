using FarmasilApp.VistaModelo.Principal;

namespace FarmasilApp.Pruebas.VistaModelo;

public class InicioViewModelPruebas
{
    [Fact]
    public void GenerarElPdfInformaElNombreDelArchivoCreado()
    {
        var pdf = new PdfServicioFalso { RutaGenerada = @"C:\Pruebas\Ejemplo-PDF-FarmasilApp-20261117-101500.pdf" };
        var vistaModelo = new InicioViewModel(pdf);

        vistaModelo.GenerarPdfCommand.Execute(null);

        Assert.Equal(1, pdf.Llamadas);
        Assert.Contains("Ejemplo-PDF-FarmasilApp-20261117-101500.pdf", vistaModelo.EstadoPdf);
        Assert.DoesNotContain(@"C:\Pruebas", vistaModelo.EstadoPdf);
    }

    [Fact]
    public void SiFallaElPdfMuestraUnMensajeGenericoSinElTextoTecnico()
    {
        var pdf = new PdfServicioFalso { Excepcion = new IOException("Acceso denegado a C:\\Secreto\\archivo.pdf") };
        var vistaModelo = new InicioViewModel(pdf);

        vistaModelo.GenerarPdfCommand.Execute(null);

        Assert.Equal(InicioViewModel.MensajeError, vistaModelo.EstadoPdf);
        Assert.DoesNotContain("Secreto", vistaModelo.EstadoPdf);
    }

    [Fact]
    public void ElConstructorRechazaUnServicioNulo()
    {
        Assert.Throws<ArgumentNullException>(() => new InicioViewModel(null!));
    }
}
