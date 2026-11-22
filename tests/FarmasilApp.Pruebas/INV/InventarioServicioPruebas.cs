using FarmasilApp.Modelo.Comun;
using FarmasilApp.Modelo.INV;
using FarmasilApp.Modelo.PRV;
using FarmasilApp.Modelo.Seguridad;

namespace FarmasilApp.Pruebas.INV;

public class InventarioServicioPruebas
{
    private static InventarioServicio Servicio(
        RepositorioDeInventarioFalso repositorio,
        Rol rol = Rol.Administrador) =>
        new(
            repositorio,
            new ProveedoresSimulado(),
            new SesionSimulada(rol));

    [Fact]
    public void LosProveedoresDelFiltroSeMuestranPorRazonSocial()
    {
        var repositorio = new RepositorioDeInventarioFalso();
        repositorio.IdsDeProveedor.AddRange(new[] { 1, 2, 3 });

        var proveedores =
            Servicio(repositorio).ObtenerProveedores();

        Assert.Equal(
            new[]
            {
                "Drogueria Andina SAC",
                "Distribuidora Salud Sur SAC",
                "Laboratorios del Pacifico SAC"
            },
            proveedores.Select(p => p.Nombre));

        Assert.Equal(
            new int?[] { 1, 2, 3 },
            proveedores.Select(p => p.IdProveedor));
    }

    [Fact]
    public void UnProveedorInactivoConProductosTambienSeMuestraConSuNombre()
    {
        var repositorio = new RepositorioDeInventarioFalso();
        repositorio.IdsDeProveedor.Add(4);

        var proveedor =
            Assert.Single(
                Servicio(repositorio)
                    .ObtenerProveedores());

        Assert.Equal(
            "Proveedor Inactivo SAC",
            proveedor.Nombre);
    }

    [Fact]
    public void UnProveedorQueIProveedoresNoConoceSeMuestraPorSuIdentificador()
    {
        var repositorio = new RepositorioDeInventarioFalso();
        repositorio.IdsDeProveedor.Add(99);

        var proveedor =
            Assert.Single(
                Servicio(repositorio)
                    .ObtenerProveedores());

        Assert.Equal(99, proveedor.IdProveedor);
        Assert.Equal(
            "Proveedor 99",
            proveedor.Nombre);
    }

    [Fact]
    public void SinProductosNoHayProveedoresQueFiltrar()
    {
        Assert.Empty(
            Servicio(
                new RepositorioDeInventarioFalso())
            .ObtenerProveedores());
    }

    [Fact]
    public void TodosNoFiltraPorProveedor()
    {
        Assert.Null(
            ProveedorFiltro.Todos.IdProveedor);

        Assert.Equal(
            "Todos",
            ProveedorFiltro.Todos.Nombre);
    }

    [Fact]
    public void ConsultarNormalizaLosCriteriosAntesDeLlamarAlRepositorio()
    {
        var repositorio =
            new RepositorioDeInventarioFalso();

        Servicio(repositorio)
            .Consultar(
                "  para  ",
                3,
                "   ");

        var consulta = repositorio.UltimaConsulta;

        Assert.NotNull(consulta);
        Assert.Equal(
            "para",
            consulta.Value.Busqueda);

        Assert.Equal(
            3,
            consulta.Value.IdProveedor);

        Assert.Null(
            consulta.Value.Accion);
    }

    [Fact]
    public void ElRepositorioLosProveedoresYLaSesionSonObligatorios()
    {
        Assert.Throws<ArgumentNullException>(
            () => new InventarioServicio(
                null!,
                new ProveedoresSimulado(),
                new SesionSimulada()));

        Assert.Throws<ArgumentNullException>(
            () => new InventarioServicio(
                new RepositorioDeInventarioFalso(),
                null!,
                new SesionSimulada()));

        Assert.Throws<ArgumentNullException>(
            () => new InventarioServicio(
                new RepositorioDeInventarioFalso(),
                new ProveedoresSimulado(),
                null!));
    }

    [Fact]
    [Trait("Especificacion", "ESP-0005")]
    [Trait("Tecnica", "T-002")]
    public void CrearNormalizaLosDatosAntesDePersistir()
    {
        var repositorio =
            new RepositorioDeInventarioFalso();

        var servicio = Servicio(repositorio);

        servicio.Crear(
            "  Paracetamol 500 mg  ",
            "  Analgesico  ",
            2.50m,
            1,
            10);

        var producto =
            Assert.IsType<ProductoEscritura>(
                repositorio.UltimoProductoCreado);

        Assert.Equal(
            "Paracetamol 500 mg",
            producto.Nombre);

        Assert.Equal(
            "Analgesico",
            producto.AccionTerapeutica);

        Assert.Equal(
            2.50m,
            producto.PrecioVenta);

        Assert.Equal(
            1,
            producto.IdProveedor);

        Assert.Equal(
            10,
            producto.StockMinimo);
    }

    [Fact]
    [Trait("Especificacion", "ESP-0005")]
    [Trait("Tecnica", "T-004")]
    public void CrearDevuelveElIdentificadorGeneradoPorPersistencia()
    {
        var repositorio =
            new RepositorioDeInventarioFalso
            {
                IdProductoCreado = 88
            };

        var id = Servicio(repositorio)
            .Crear(
                "Paracetamol",
                "Analgesico",
                2.50m,
                1,
                10);

        Assert.Equal(88, id);
    }

    [Fact]
    [Trait("Especificacion", "ESP-0005")]
    [Trait("Tecnica", "T-001")]
    public void CrearRechazaNombreVacio()
    {
        var excepcion =
            Assert.Throws<ReglaDeNegocioException>(
                () => Servicio(
                        new RepositorioDeInventarioFalso())
                    .Crear(
                        "   ",
                        "Analgesico",
                        2.50m,
                        1,
                        10));

        Assert.Contains(
            "nombre",
            excepcion.Message,
            StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    [Trait("Especificacion", "ESP-0005")]
    [Trait("Tecnica", "T-001")]
    public void CrearRechazaPrecioNegativo()
    {
        Assert.Throws<ReglaDeNegocioException>(
            () => Servicio(
                    new RepositorioDeInventarioFalso())
                .Crear(
                    "Paracetamol",
                    "Analgesico",
                    -0.01m,
                    1,
                    10));
    }

    [Fact]
    [Trait("Especificacion", "ESP-0005")]
    [Trait("Tecnica", "T-001")]
    public void CrearAceptaPrecioCero()
    {
        var repositorio =
            new RepositorioDeInventarioFalso();

        Servicio(repositorio)
            .Crear(
                "Producto muestra",
                "Prueba",
                0m,
                1,
                0);

        Assert.NotNull(
            repositorio.UltimoProductoCreado);
    }

    [Fact]
    [Trait("Especificacion", "ESP-0005")]
    [Trait("Tecnica", "T-001")]
    public void CrearRechazaStockMinimoNegativo()
    {
        Assert.Throws<ReglaDeNegocioException>(
            () => Servicio(
                    new RepositorioDeInventarioFalso())
                .Crear(
                    "Paracetamol",
                    "Analgesico",
                    2.50m,
                    1,
                    -1));
    }

    [Fact]
    [Trait("Especificacion", "ESP-0005")]
    [Trait("Tecnica", "T-001")]
    public void CrearRechazaProveedorInactivo()
    {
        Assert.Throws<ReglaDeNegocioException>(
            () => Servicio(
                    new RepositorioDeInventarioFalso())
                .Crear(
                    "Paracetamol",
                    "Analgesico",
                    2.50m,
                    4,
                    10));
    }

    [Fact]
    [Trait("Especificacion", "ESP-0007")]
    [Trait("Tecnica", "T-002")]
    public void ActualizarConservaElIdentificadorYEnviaLosNuevosDatos()
    {
        var repositorio =
            new RepositorioDeInventarioFalso();

        Servicio(repositorio)
            .Actualizar(
                7,
                "  Omeprazol 40 mg ",
                " Gastroprotector ",
                8.50m,
                2,
                12);

        Assert.NotNull(
            repositorio.UltimaActualizacion);

        var actualizacion =
            repositorio.UltimaActualizacion.Value;

        Assert.Equal(
            7,
            actualizacion.IdProducto);

        Assert.Equal(
            "Omeprazol 40 mg",
            actualizacion.Producto.Nombre);

        Assert.Equal(
            "Gastroprotector",
            actualizacion.Producto.AccionTerapeutica);

        Assert.Equal(
            8.50m,
            actualizacion.Producto.PrecioVenta);

        Assert.Equal(
            2,
            actualizacion.Producto.IdProveedor);

        Assert.Equal(
            12,
            actualizacion.Producto.StockMinimo);
    }

    [Fact]
    [Trait("Especificacion", "ESP-0008")]
    [Trait("Tecnica", "T-002")]
    public void DarDeBajaDelegaLaBajaCondicionalAlRepositorio()
    {
        var repositorio =
            new RepositorioDeInventarioFalso
            {
                ResultadoBajaADevolver =
                    ResultadoBajaProducto.Descontinuado
            };

        var resultado =
            Servicio(repositorio)
                .DarDeBaja(3);

        Assert.Equal(
            3,
            repositorio.UltimaBaja);

        Assert.Equal(
            ResultadoBajaProducto.Descontinuado,
            resultado);
    }

    [Fact]
    [Trait("Especificacion", "ESP-0005")]
    [Trait("Tecnica", "T-012")]
    public void TecnicoNoPuedeCrearProductos()
    {
        Assert.Throws<AccesoDenegadoException>(
            () => Servicio(
                    new RepositorioDeInventarioFalso(),
                    Rol.Tecnico)
                .Crear(
                    "Paracetamol",
                    "Analgesico",
                    2.50m,
                    1,
                    10));
    }

    [Fact]
    [Trait("Especificacion", "ESP-0007")]
    [Trait("Tecnica", "T-012")]
    public void TecnicoNoPuedeActualizarProductos()
    {
        Assert.Throws<AccesoDenegadoException>(
            () => Servicio(
                    new RepositorioDeInventarioFalso(),
                    Rol.Tecnico)
                .Actualizar(
                    1,
                    "Paracetamol",
                    "Analgesico",
                    2.50m,
                    1,
                    10));
    }

    [Fact]
    [Trait("Especificacion", "ESP-0008")]
    [Trait("Tecnica", "T-012")]
    public void TecnicoNoPuedeDarDeBajaProductos()
    {
        Assert.Throws<AccesoDenegadoException>(
            () => Servicio(
                    new RepositorioDeInventarioFalso(),
                    Rol.Tecnico)
                .DarDeBaja(1));
    }
}