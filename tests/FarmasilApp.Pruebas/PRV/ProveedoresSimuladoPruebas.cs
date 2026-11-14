using FarmasilApp.Modelo.Comun;
using FarmasilApp.Modelo.PRV;

namespace FarmasilApp.Pruebas.PRV;

public class ProveedoresSimuladoPruebas
{
    [Fact]
    public void LosProveedoresActivosSonLosTresDeLosDatosDePrueba()
    {
        // database/002_datos_prueba.sql: ids 1 a 3 activos; el 4 está inactivo.
        IProveedores proveedores = new ProveedoresSimulado();

        var activos = proveedores.ProveedoresActivos();

        Assert.Equal(new[] { 1, 2, 3 }, activos.Select(p => p.IdProveedor));
        Assert.All(activos, p => Assert.Equal(EstadoRegistro.Activo, p.Estado));
    }

    [Fact]
    public void CadaProveedorActivoTraeRucRazonSocialYTelefono()
    {
        var andina = new ProveedoresSimulado().ProveedoresActivos().First();

        Assert.Equal("20100070970", andina.Ruc);
        Assert.Equal("Drogueria Andina SAC", andina.RazonSocial);
        Assert.Equal("054223344", andina.Telefono);
    }

    [Fact]
    public void ObtenerUnProveedorInactivoLoDevuelveConSuEstado()
    {
        var inactivo = new ProveedoresSimulado().ObtenerProveedor(4);

        Assert.NotNull(inactivo);
        Assert.Equal("Proveedor Inactivo SAC", inactivo.RazonSocial);
        Assert.Equal(EstadoRegistro.Inactivo, inactivo.Estado);
        Assert.Null(inactivo.Telefono);
    }

    [Fact]
    public void ObtenerUnProveedorQueNoExisteDevuelveNulo()
    {
        Assert.Null(new ProveedoresSimulado().ObtenerProveedor(99));
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void ObtenerConUnIdentificadorNoPositivoFalla(int idProveedor)
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => new ProveedoresSimulado().ObtenerProveedor(idProveedor));
    }
}
