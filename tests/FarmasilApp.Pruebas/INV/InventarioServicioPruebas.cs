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
    [Trait("Especificacion", "ESP-0006")]
    [Trait("Tecnica", "T-002")]
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
    [Trait("Especificacion", "ESP-0006")]
    [Trait("Tecnica", "T-001")]
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
    [Trait("Especificacion", "ESP-0006")]
    [Trait("Tecnica", "T-001")]
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
    [Trait("Especificacion", "ESP-0006")]
    [Trait("Tecnica", "T-001")]
    public void SinProductosNoHayProveedoresQueFiltrar()
    {
        Assert.Empty(
            Servicio(
                new RepositorioDeInventarioFalso())
            .ObtenerProveedores());
    }

    [Fact]
    [Trait("Especificacion", "ESP-0006")]
    [Trait("Tecnica", "T-001")]
    public void TodosNoFiltraPorProveedor()
    {
        Assert.Null(
            ProveedorFiltro.Todos.IdProveedor);

        Assert.Equal(
            "Todos",
            ProveedorFiltro.Todos.Nombre);
    }

    [Fact]
    [Trait("Especificacion", "ESP-0006")]
    [Trait("Tecnica", "T-002")]
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
    [Trait("Especificacion", "ESP-0006")]
    [Trait("Tecnica", "T-002")]
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


    [Fact]
    [Trait("Especificacion", "ESP-0005")]
    [Trait("Tecnica", "T-002")]
    public void CrearProductoCaminoBasicoRegistraDatosCorrectosYDevuelveId()
    {
        var repositorio = new RepositorioDeInventarioFalso
        {
            IdProductoCreado = 15
        };

        var idCreado = Servicio(repositorio)
            .Crear(
                "Paracetamol 500 mg Tableta",
                "Analgesico y antipiretico",
                1.50m,
                1,
                20);

        Assert.Equal(15, idCreado);

        var producto = Assert.IsType<ProductoEscritura>(
            repositorio.UltimoProductoCreado);

        Assert.Equal("Paracetamol 500 mg Tableta", producto.Nombre);
        Assert.Equal("Analgesico y antipiretico", producto.AccionTerapeutica);
        Assert.Equal(1.50m, producto.PrecioVenta);
        Assert.Equal(1, producto.IdProveedor);
        Assert.Equal(20, producto.StockMinimo);
    }


    [Fact]
    [Trait("Especificacion", "ESP-0005")]
    [Trait("Tecnica", "T-001")]
    public void CrearAceptaPrecioPositivo()
    {
        var repositorio = new RepositorioDeInventarioFalso();

        Servicio(repositorio)
            .Crear(
                "Ibuprofeno 400 mg",
                "Antiinflamatorio",
                4.20m,
                1,
                15);

        var producto = Assert.IsType<ProductoEscritura>(
            repositorio.UltimoProductoCreado);

        Assert.Equal(4.20m, producto.PrecioVenta);
    }


    [Fact]
    [Trait("Especificacion", "ESP-0005")]
    [Trait("Tecnica", "T-004")]
    public void CrearGeneraIdentificadorAutomaticamenteSinRecibirloComoEntrada()
    {
        var repositorio = new RepositorioDeInventarioFalso
        {
            IdProductoCreado = 104
        };

        // Crear no recibe IdProducto: el identificador es asignado por persistencia y retornado
        var idGenerado = Servicio(repositorio)
            .Crear(
                "Amoxicilina 500 mg Capsula",
                "Antibiotico",
                3.20m,
                1,
                25);

        Assert.Equal(104, idGenerado);

        var producto = Assert.IsType<ProductoEscritura>(
            repositorio.UltimoProductoCreado);

        Assert.Equal("Amoxicilina 500 mg Capsula", producto.Nombre);
        Assert.Equal("Antibiotico", producto.AccionTerapeutica);
        Assert.Equal(3.20m, producto.PrecioVenta);
        Assert.Equal(1, producto.IdProveedor);
        Assert.Equal(25, producto.StockMinimo);
    }

    [Fact]
    [Trait("Especificacion", "ESP-0006")]
    [Trait("Tecnica", "T-002")]
    public void ConsultarCaminoBasicoEnviaCriteriosNormalizadosAlRepositorio()
    {
        var repositorio = new RepositorioDeInventarioFalso();

        var resultado = Servicio(repositorio)
            .Consultar(
                busqueda: "  Paracetamol  ",
                idProveedor: 1,
                accionTerapeutica: "  Analgésico  ");

        Assert.NotNull(resultado);
        Assert.NotNull(repositorio.UltimaConsulta);

        var consulta = repositorio.UltimaConsulta.Value;
        Assert.Equal("Paracetamol", consulta.Busqueda);
        Assert.Equal(1, consulta.IdProveedor);
        Assert.Equal("Analgésico", consulta.Accion);
    }

    [Fact]
    [Trait("Especificacion", "ESP-0006")]
    [Trait("Tecnica", "T-002")]
    public void ConsultarCaminoBasicoSinFiltrosOpcionalesPasaCriteriosNulos()
    {
        var repositorio = new RepositorioDeInventarioFalso();

        var resultado = Servicio(repositorio)
            .Consultar(
                busqueda: "12",
                idProveedor: null,
                accionTerapeutica: "   ");

        Assert.NotNull(resultado);
        Assert.NotNull(repositorio.UltimaConsulta);

        var consulta = repositorio.UltimaConsulta.Value;
        Assert.Equal("12", consulta.Busqueda);
        Assert.Null(consulta.IdProveedor);
        Assert.Null(consulta.Accion);
    }


    [Theory]
    [Trait("Especificacion", "ESP-0006")]
    [Trait("Tecnica", "T-001")]
    [InlineData("15", null, null, "15", null, null)]
    [InlineData("99999", null, null, "99999", null, null)]
    [InlineData("  para  ", null, null, "para", null, null)]
    [InlineData(null, 2, null, null, 2, null)]
    [InlineData(null, null, "  Analgesico  ", null, null, "Analgesico")]
    [InlineData("  ibu  ", 1, "  Antiinflamatorio  ", "ibu", 1, "Antiinflamatorio")]
    [InlineData("   ", null, "  \t  ", null, null, null)]
    [InlineData(null, null, null, null, null, null)]
    public void ConsultarParticionesDeCriteriosNormalizaYDelegaAlRepositorio(
    string? busquedaEntrada,
    int? proveedorEntrada,
    string? accionEntrada,
    string? busquedaEsperada,
    int? proveedorEsperado,
    string? accionEsperada)
    {
        var repositorio = new RepositorioDeInventarioFalso();

        var resultado = Servicio(repositorio)
            .Consultar(
                busquedaEntrada,
                proveedorEntrada,
                accionEntrada);

        Assert.NotNull(resultado);
        Assert.NotNull(repositorio.UltimaConsulta);

        var consulta = repositorio.UltimaConsulta.Value;
        Assert.Equal(busquedaEsperada, consulta.Busqueda);
        Assert.Equal(proveedorEsperado, consulta.IdProveedor);
        Assert.Equal(accionEsperada, consulta.Accion);
    }

    [Fact]
    [Trait("Especificacion", "ESP-0007")]
    [Trait("Tecnica", "T-002")]
    public void ActualizarCaminoBasicoModificaCamposYEnviaAlRepositorio()
    {
        var repositorio = new RepositorioDeInventarioFalso();

        Servicio(repositorio)
            .Actualizar(
                idProducto: 12,
                nombre: "  Omeprazol 20 mg Capsula  ",
                accionTerapeutica: "  Antiulceroso  ",
                precioVenta: 5.50m,
                idProveedor: 2,
                stockMinimo: 20);

        Assert.NotNull(repositorio.UltimaActualizacion);

        var (idActualizado, producto) = repositorio.UltimaActualizacion.Value;
        Assert.Equal(12, idActualizado);
        Assert.Equal("Omeprazol 20 mg Capsula", producto.Nombre);
        Assert.Equal("Antiulceroso", producto.AccionTerapeutica);
        Assert.Equal(5.50m, producto.PrecioVenta);
        Assert.Equal(2, producto.IdProveedor);
        Assert.Equal(20, producto.StockMinimo);
    }

    [Fact]
    [Trait("Especificacion", "ESP-0008")]
    [Trait("Tecnica", "T-002")]
    public void DarDeBajaCaminoBasicoAplicaBajaYDevuelveResultado()
    {
        const int idProducto = 25;
        var repositorio = new RepositorioDeInventarioFalso
        {
            ResultadoBajaADevolver = ResultadoBajaProducto.Descontinuado
        };

        var resultado = Servicio(repositorio).DarDeBaja(idProducto);

        Assert.Equal(idProducto, repositorio.UltimaBaja);
        Assert.Equal(ResultadoBajaProducto.Descontinuado, resultado);
    }

    [Theory]
    [Trait("Especificacion", "ESP-0008")]
    [Trait("Tecnica", "T-003")]
    [InlineData(ResultadoBajaProducto.Descontinuado)]
    public void DarDeBajaCubreRamaDeBajaEnRepositorio(
    ResultadoBajaProducto resultadoEsperado)
    {
        const int idProducto = 31;
        var repositorio = new RepositorioDeInventarioFalso
        {
            ResultadoBajaADevolver = resultadoEsperado
        };

        var resultado = Servicio(repositorio).DarDeBaja(idProducto);

        Assert.Equal(idProducto, repositorio.UltimaBaja);
        Assert.Equal(resultadoEsperado, resultado);
    }

    [Fact]
    [Trait("Especificacion", "ESP-0008")]
    [Trait("Tecnica", "T-001")]
    public void DarDeBajaMedicamentoSinRestriccionesDelegaAlRepositorio()
    {
        const int idProducto = 18;
        var repositorio = new RepositorioDeInventarioFalso
        {
            ResultadoBajaADevolver = ResultadoBajaProducto.Descontinuado
        };

        var resultado = Servicio(repositorio).DarDeBaja(idProducto);

        Assert.Equal(idProducto, repositorio.UltimaBaja);
        Assert.Equal(ResultadoBajaProducto.Descontinuado, resultado);
    }

    [Fact]
    [Trait("Especificacion", "RNF-0006")]
    [Trait("Tecnica", "T-012")]
    public void SesionTecnicoPermiteConsultarYRechazaOperacionesDeEscritura()
    {
        var repositorio = new RepositorioDeInventarioFalso();
        var servicio = Servicio(repositorio, Rol.Tecnico);

        // ESP-0006 (Consulta / Leer): permitido
        var productos = servicio.Consultar(null, null, null);
        Assert.NotNull(productos);

        // Operaciones de escritura/baja restringidas: rechazadas sin tocar el repositorio
        Assert.Throws<AccesoDenegadoException>(() =>
            servicio.Actualizar(
                idProducto: 1,
                nombre: "Paracetamol 500 mg",
                accionTerapeutica: "Analgesico",
                precioVenta: 3.00m,
                idProveedor: 1,
                stockMinimo: 10));

        Assert.Throws<AccesoDenegadoException>(() =>
            servicio.DarDeBaja(idProducto: 1));

        Assert.Null(repositorio.UltimaActualizacion);
        Assert.Null(repositorio.UltimaBaja);
    }

}
