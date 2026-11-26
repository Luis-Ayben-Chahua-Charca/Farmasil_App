using FarmasilApp.Modelo.Comun;
using FarmasilApp.Modelo.PRV;

namespace FarmasilApp.Pruebas.PRV;

/// <summary>
/// <see cref="ProveedoresContrato"/>, la implementación real de <see cref="IProveedores"/>, con el repositorio falso.
/// El filtro por estado 'Activo' está en el SQL de <c>ObtenerActivos</c>: se prueba en la integración (S4).
/// </summary>
public sealed class ProveedoresContratoPruebas
{
    [Fact]
    [Trait("Especificacion", "ESP-0037")]
    [Trait("Tecnica", "T-006")]
    public void ProveedoresActivosDevuelveLosQueEntregaElRepositorioEnSuOrden()
    {
        var repositorio = new RepositorioDeProveedoresFalso();
        repositorio.ProveedoresActivos.Add(Proveedor(1, EstadoRegistro.Activo));
        repositorio.ProveedoresActivos.Add(Proveedor(3, EstadoRegistro.Activo));
        IProveedores proveedores = new ProveedoresContrato(repositorio);

        var activos = proveedores.ProveedoresActivos();

        Assert.Equal(new[] { 1, 3 }, activos.Select(p => p.IdProveedor));
    }

    [Fact]
    [Trait("Especificacion", "ESP-0039")]
    [Trait("Tecnica", "T-007")]
    public void ObtenerProveedorDevuelveUnInactivoConSuEstado()
    {
        var repositorio = new RepositorioDeProveedoresFalso();
        repositorio.ProveedoresPorId[4] = Proveedor(4, EstadoRegistro.Inactivo);
        IProveedores proveedores = new ProveedoresContrato(repositorio);

        var inactivo = proveedores.ObtenerProveedor(4);

        Assert.NotNull(inactivo);
        Assert.Equal(EstadoRegistro.Inactivo, inactivo.Estado);
        Assert.Equal("Proveedor 4", inactivo.RazonSocial);
    }

    [Fact]
    [Trait("Especificacion", "ESP-0039")]
    [Trait("Tecnica", "T-007")]
    public void ObtenerProveedorInexistenteDevuelveNulo()
    {
        IProveedores proveedores = new ProveedoresContrato(new RepositorioDeProveedoresFalso());

        Assert.Null(proveedores.ObtenerProveedor(99));
    }

    [Theory]
    [Trait("Especificacion", "ESP-0039")]
    [Trait("Tecnica", "T-001")]
    [InlineData(0)]
    [InlineData(-1)]
    public void ObtenerProveedorConIdentificadorNoPositivoLanzaExcepcion(int idProveedor)
    {
        IProveedores proveedores = new ProveedoresContrato(new RepositorioDeProveedoresFalso());

        Assert.Throws<ArgumentOutOfRangeException>(() => proveedores.ObtenerProveedor(idProveedor));
    }

    [Fact]
    [Trait("Especificacion", "ESP-0038")]
    [Trait("Tecnica", "T-002")]
    public void ElRepositorioEsObligatorio()
    {
        Assert.Throws<ArgumentNullException>(() => new ProveedoresContrato(null!));
    }

    private static ProveedorDto Proveedor(int id, EstadoRegistro estado) => new()
    {
        IdProveedor = id,
        Ruc = $"2010007097{id}",
        RazonSocial = $"Proveedor {id}",
        Estado = estado
    };
}
