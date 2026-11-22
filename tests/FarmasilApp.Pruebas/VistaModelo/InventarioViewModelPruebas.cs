using FarmasilApp.Modelo.Comun;
using FarmasilApp.Modelo.INV;
using FarmasilApp.Modelo.PRV;
using FarmasilApp.Modelo.Seguridad;
using FarmasilApp.Pruebas.INV;
using FarmasilApp.VistaModelo.INV;

namespace FarmasilApp.Pruebas.VistaModelo;

public class InventarioViewModelPruebas
{
    private static InventarioViewModel Crear(
        RepositorioDeInventarioFalso repositorio,
        Rol rol = Rol.Administrador)
    {
        var sesion = new SesionSimulada(rol);

        return new InventarioViewModel(
            new InventarioServicio(
                repositorio,
                new ProveedoresSimulado(),
                sesion),
            new MensajeServicioFalso(),
            sesion);
    }

    private static (
        InventarioViewModel VistaModelo,
        MensajeServicioFalso Mensajes)
        CrearConMensajes(
            RepositorioDeInventarioFalso repositorio,
            Rol rol = Rol.Administrador)
    {
        var sesion = new SesionSimulada(rol);
        var mensajes = new MensajeServicioFalso();

        var vistaModelo = new InventarioViewModel(
            new InventarioServicio(
                repositorio,
                new ProveedoresSimulado(),
                sesion),
            mensajes,
            sesion);

        return (vistaModelo, mensajes);
    }

    [Fact]
    public async Task InicializarCargaElFiltroDeProveedorPorRazonSocial()
    {
        var repositorio = new RepositorioDeInventarioFalso();
        repositorio.IdsDeProveedor.AddRange(new[] { 1, 3 });

        var vistaModelo = Crear(repositorio);

        await vistaModelo.InicializarCommand.ExecuteAsync();

        Assert.Equal(
            new[]
            {
                "Todos",
                "Drogueria Andina SAC",
                "Laboratorios del Pacifico SAC"
            },
            vistaModelo.Proveedores.Select(p => p.Nombre));

        Assert.Same(
            ProveedorFiltro.Todos,
            vistaModelo.ProveedorSeleccionado);
    }

    [Fact]
    public async Task ConTodosSeleccionadoLaConsultaNoFiltraPorProveedor()
    {
        var repositorio = new RepositorioDeInventarioFalso();
        repositorio.IdsDeProveedor.Add(1);

        var vistaModelo = Crear(repositorio);

        await vistaModelo.InicializarCommand.ExecuteAsync();

        Assert.Null(
            repositorio.UltimaConsulta?.IdProveedor);
    }

    [Fact]
    public async Task AlElegirUnProveedorLaConsultaFiltraPorSuIdentificador()
    {
        var repositorio = new RepositorioDeInventarioFalso();
        repositorio.IdsDeProveedor.AddRange(new[] { 1, 3 });

        var vistaModelo = Crear(repositorio);

        await vistaModelo.InicializarCommand.ExecuteAsync();

        vistaModelo.ProveedorSeleccionado =
            vistaModelo.Proveedores.Single(
                p => p.IdProveedor == 3);

        await vistaModelo.BuscarCommand.ExecuteAsync();

        Assert.Equal(
            3,
            repositorio.UltimaConsulta?.IdProveedor);
    }

    [Fact]
    public async Task SiLaListaDeProveedoresSePierdeLaConsultaNoFiltra()
    {
        // Al vaciarse la lista, el ComboBox deja la selección en nulo.
        var repositorio = new RepositorioDeInventarioFalso();

        var vistaModelo = Crear(repositorio);

        await vistaModelo.InicializarCommand.ExecuteAsync();

        vistaModelo.ProveedorSeleccionado = null;

        await vistaModelo.BuscarCommand.ExecuteAsync();

        Assert.Null(
            repositorio.UltimaConsulta?.IdProveedor);
    }

    [Fact]
    public async Task SiFallaLaCargaDeProveedoresQuedaSoloTodosYSeAvisa()
    {
        var repositorio =
            new RepositorioDeInventarioFalso
            {
                FallaAlObtenerProveedores = true
            };

        var vistaModelo = Crear(repositorio);

        await vistaModelo.InicializarCommand.ExecuteAsync();

        Assert.Equal(
            new[] { "Todos" },
            vistaModelo.Proveedores.Select(p => p.Nombre));

        Assert.StartsWith(
            "No se pudieron cargar los filtros del inventario.",
            vistaModelo.Mensaje);
    }

    [Fact]
    [Trait("Especificacion", "ESP-0005")]
    [Trait("Tecnica", "T-001")]
    public async Task PrecioNoNumericoMuestraErrorYNoRegistraProducto()
    {
        var repositorio = new RepositorioDeInventarioFalso();
        var (vistaModelo, mensajes) =
            CrearConMensajes(repositorio);

        vistaModelo.NombreProducto =
            "Producto de prueba";

        vistaModelo.AccionTerapeuticaFicha =
            "Prueba";

        vistaModelo.PrecioVentaTexto =
            "abc";

        vistaModelo.ProveedorFicha =
            new ProveedoresSimulado()
                .ProveedoresActivos()
                .First();

        vistaModelo.StockMinimoTexto =
            "2";

        await vistaModelo.CrearCommand.ExecuteAsync();

        Assert.Null(
            repositorio.UltimoProductoCreado);

        var error =
            Assert.Single(mensajes.Errores);

        Assert.Equal(
            "Inventario",
            error.Titulo);

        Assert.Equal(
            "El precio de venta debe ser un número válido.",
            error.Mensaje);
    }

    [Fact]
    [Trait("Especificacion", "ESP-0008")]
    [Trait("Tecnica", "T-003")]
    public async Task ConfirmacionNegativaNoDaDeBajaElProducto()
    {
        var repositorio = new RepositorioDeInventarioFalso();

        var (vistaModelo, mensajes) =
            CrearConMensajes(repositorio);

        mensajes.RespuestaConfirmacion = false;

        vistaModelo.ProductoSeleccionado =
            new ProductoInventario
            {
                IdProducto = 8,
                Nombre = "Producto de prueba",
                AccionTerapeutica = "Prueba",
                PrecioVenta = 5.50m,
                EstadoProducto = "Disponible",
                IdProveedor = 1,
                StockMinimo = 2,
                StockFisico = 0
            };

        await vistaModelo.EliminarCommand.ExecuteAsync();

        Assert.Null(
            repositorio.UltimaBaja);

        var confirmacion =
            Assert.Single(mensajes.Confirmaciones);

        Assert.Equal(
            "INV",
            confirmacion.Modulo);

        Assert.Equal(
            "¿Está seguro de dar de baja este producto?",
            confirmacion.Mensaje);

        Assert.Empty(
            mensajes.Exitos);
    }

    [Fact]
    [Trait("Especificacion", "ESP-0008")]
    [Trait("Tecnica", "T-002")]
    public async Task ConfirmacionPositivaDaDeBajaElProducto()
    {
        var repositorio =
            new RepositorioDeInventarioFalso
            {
                ResultadoBajaADevolver =
                    ResultadoBajaProducto.Descontinuado
            };

        var (vistaModelo, mensajes) =
            CrearConMensajes(repositorio);

        mensajes.RespuestaConfirmacion = true;

        vistaModelo.ProductoSeleccionado =
            new ProductoInventario
            {
                IdProducto = 3,
                Nombre = "Amoxicilina 500 mg",
                AccionTerapeutica = "Antibiotico",
                PrecioVenta = 6.50m,
                EstadoProducto = "Disponible",
                IdProveedor = 1,
                StockMinimo = 5,
                StockFisico = 12
            };

        await vistaModelo.EliminarCommand.ExecuteAsync();

        Assert.Equal(
            3,
            repositorio.UltimaBaja);

        var exito =
            Assert.Single(mensajes.Exitos);

        Assert.Equal(
            "Inventario",
            exito.Titulo);

        Assert.Equal(
            "Producto dado de baja correctamente",
            exito.Mensaje);
    }

    [Fact]
    [Trait("Especificacion", "RNF-0006")]
    [Trait("Tecnica", "T-016")]
    public void TecnicaTieneLasOperacionesDeEscrituraDeshabilitadas()
    {
        var repositorio = new RepositorioDeInventarioFalso();

        var vistaModelo =
            Crear(repositorio, Rol.Tecnico);

        vistaModelo.ProductoSeleccionado =
            new ProductoInventario
            {
                IdProducto = 1,
                Nombre = "Producto",
                AccionTerapeutica = "Prueba",
                PrecioVenta = 1m,
                EstadoProducto = "Disponible",
                IdProveedor = 1,
                StockMinimo = 1,
                StockFisico = 0
            };

        Assert.False(
            vistaModelo.CrearCommand.CanExecute(null));

        Assert.False(
            vistaModelo.ActualizarCommand.CanExecute(null));

        Assert.False(
            vistaModelo.ConfirmarActualizacionCommand.CanExecute(null));

        Assert.False(
            vistaModelo.EliminarCommand.CanExecute(null));

        Assert.Equal(
            "Solo disponible para el Administrador.",
            vistaModelo.MotivoAccionesAdministrador);
    }
}
