using FarmasilApp.Modelo.LOT;
using FarmasilApp.Modelo.PAG;
using FarmasilApp.Modelo.RES;
using FarmasilApp.Modelo.Seguridad;
using FarmasilApp.Modelo.VEN;
using FarmasilApp.VistaModelo.VEN;

namespace FarmasilApp.Pruebas.VistaModelo;

/// <summary>
/// Pantalla de venta (ESP-0001) con el servicio real sobre los contratos simulados. Datos de database/002_datos_prueba.sql:
/// Paracetamol (id 1, remesa 1 con 20 unidades, restricción informativa) y Amoxicilina (id 3, restricción bloqueante).
/// </summary>
[Trait("Especificacion", "ESP-0001")]
[Trait("Tecnica", "T-002")]
public class VentaViewModelPruebas
{
    private static readonly DateOnly Hoy = new(2026, 11, 14);

    private static async Task<VentaViewModel> CrearAsync()
    {
        var reloj = new RelojFijo(Hoy);
        var servicio = new VentaServicio(
            new LotesSimulado(reloj), new RestriccionesSimulado(), new PagosSimulado(), new SesionSimulada(), reloj);
        var vistaModelo = new VentaViewModel(servicio);
        await vistaModelo.InicializarCommand.ExecuteAsync();
        return vistaModelo;
    }

    private static MedicamentoVendibleDto Tarjeta(VentaViewModel vm, string nombre) =>
        vm.Medicamentos.Single(m => m.Nombre.StartsWith(nombre, StringComparison.Ordinal));

    [Fact]
    public async Task AlAbrirCargaElCatalogoVendibleLosMetodosDePagoYLaFecha()
    {
        var vm = await CrearAsync();

        Assert.Equal(new[] { 1, 3 }, vm.Medicamentos.Select(m => m.IdProducto));
        Assert.Equal(new[] { "Efectivo", "Yape" }, vm.MetodosPago.Select(m => m.Nombre));
        Assert.Equal("Efectivo", vm.MetodoPagoSeleccionado?.Nombre);
        Assert.Equal(new[] { "Todas", "Analgesico y antipiretico", "Antibiotico" }, vm.AccionesTerapeuticas);
        Assert.Equal(Hoy, DateOnly.FromDateTime(vm.FechaVenta));
        Assert.Empty(vm.Lineas);
        Assert.Equal(0m, vm.MontoTotal);
    }

    [Fact]
    public async Task ElBuscadorYElFiltroReducenElCatalogo()
    {
        var vm = await CrearAsync();

        vm.Busqueda = "amoxi";
        Assert.Equal(3, Assert.Single(vm.Medicamentos).IdProducto);

        vm.Busqueda = null;
        vm.AccionSeleccionada = "Analgesico y antipiretico";
        Assert.Equal(1, Assert.Single(vm.Medicamentos).IdProducto);

        vm.AccionSeleccionada = VentaViewModel.TodasLasAcciones;
        Assert.Equal(2, vm.Medicamentos.Count);
    }

    [Fact]
    public async Task SeleccionarUnaTarjetaPreparaLaRemesaFefoConCantidadUno()
    {
        var vm = await CrearAsync();

        vm.SeleccionarProductoCommand.Execute(Tarjeta(vm, "Paracetamol"));

        Assert.Equal("Paracetamol 500 mg", vm.MedicamentoSeleccionado?.Nombre);
        Assert.Equal(1, vm.RemesaSeleccionada?.IdLote);
        Assert.Equal(new[] { 1 }, vm.Remesas.Select(r => r.IdLote));
        Assert.Equal("1", vm.Cantidad);
        Assert.Equal(Hoy.AddMonths(12).ToString("dd/MM/yyyy"), vm.FechaVencimientoRemesa);
        Assert.Equal(string.Empty, vm.Advertencias);
        Assert.Empty(vm.Lineas);
    }

    [Fact]
    public async Task AgregarProductoDejaLaLineaYActualizaElTotal()
    {
        var vm = await CrearAsync();
        vm.SeleccionarProductoCommand.Execute(Tarjeta(vm, "Paracetamol"));
        vm.Cantidad = "3";

        await vm.AgregarProductoCommand.ExecuteAsync();

        var linea = Assert.Single(vm.Lineas);
        Assert.Equal((3, 2.50m), (linea.Cantidad, linea.PrecioUnitario));
        Assert.Equal(7.50m, vm.MontoTotal);
    }

    [Fact]
    public async Task UnaRestriccionInformativaAgregaLaLineaYMuestraElAviso()
    {
        var vm = await CrearAsync();
        vm.SeleccionarProductoCommand.Execute(Tarjeta(vm, "Paracetamol"));

        await vm.AgregarProductoCommand.ExecuteAsync();

        Assert.Single(vm.Lineas);
        Assert.Equal("Consultar dosis recomendada antes de la venta.", vm.Advertencias);
    }

    [Fact]
    public async Task UnaRestriccionBloqueanteRechazaLaLineaYMuestraSuMotivo()
    {
        var vm = await CrearAsync();
        vm.SeleccionarProductoCommand.Execute(Tarjeta(vm, "Amoxicilina"));

        await vm.AgregarProductoCommand.ExecuteAsync();

        Assert.Empty(vm.Lineas);
        Assert.Equal("No completar la venta si el cliente presenta alergia.", vm.Advertencias);
    }

    [Theory]
    [InlineData("0")]
    [InlineData("-2")]
    [InlineData("abc")]
    [InlineData("")]
    [InlineData("1.5")]
    public async Task UnaCantidadQueNoEsEnteroPositivoSeRechazaConSuMensaje(string cantidad)
    {
        var vm = await CrearAsync();
        vm.SeleccionarProductoCommand.Execute(Tarjeta(vm, "Paracetamol"));
        vm.Cantidad = cantidad;

        await vm.AgregarProductoCommand.ExecuteAsync();

        Assert.Empty(vm.Lineas);
        Assert.Equal(MensajesVentas.CantidadInvalida, vm.Advertencias);
    }

    [Fact]
    public async Task LaCantidadAcumuladaNoPuedeSuperarElStock()
    {
        var vm = await CrearAsync();
        vm.SeleccionarProductoCommand.Execute(Tarjeta(vm, "Paracetamol"));
        vm.Cantidad = "15";
        await vm.AgregarProductoCommand.ExecuteAsync();

        vm.Cantidad = "6";
        await vm.AgregarProductoCommand.ExecuteAsync();

        Assert.Equal(15, Assert.Single(vm.Lineas).Cantidad);
        Assert.Equal(string.Format(MensajesVentas.StockInsuficiente, 20), vm.Advertencias);
    }

    [Fact]
    public async Task AgregarLaMismaRemesaAcumulaEnUnaSolaLinea()
    {
        var vm = await CrearAsync();
        vm.SeleccionarProductoCommand.Execute(Tarjeta(vm, "Paracetamol"));
        vm.Cantidad = "2";
        await vm.AgregarProductoCommand.ExecuteAsync();

        vm.Cantidad = "3";
        await vm.AgregarProductoCommand.ExecuteAsync();

        Assert.Equal(5, Assert.Single(vm.Lineas).Cantidad);
        Assert.Equal(12.50m, vm.MontoTotal);
    }

    [Fact]
    public async Task QuitarProductoRetiraLaLineaSeleccionadaYRecalculaElTotal()
    {
        var vm = await CrearAsync();
        vm.SeleccionarProductoCommand.Execute(Tarjeta(vm, "Paracetamol"));
        await vm.AgregarProductoCommand.ExecuteAsync();
        Assert.False(vm.QuitarProductoCommand.CanExecute(null));

        vm.LineaSeleccionada = vm.Lineas.Single();
        Assert.True(vm.QuitarProductoCommand.CanExecute(null));
        vm.QuitarProductoCommand.Execute(null);

        Assert.Empty(vm.Lineas);
        Assert.Equal(0m, vm.MontoTotal);
        Assert.Null(vm.LineaSeleccionada);
    }

    [Fact]
    public async Task SinRemesaElegidaNoSePuedeAgregar()
    {
        var vm = await CrearAsync();

        Assert.False(vm.AgregarProductoCommand.CanExecute(null));
    }

    [Fact]
    public async Task RegistrarVentaQuedaDeshabilitadoConSuMotivo()
    {
        var vm = await CrearAsync();
        vm.SeleccionarProductoCommand.Execute(Tarjeta(vm, "Paracetamol"));
        await vm.AgregarProductoCommand.ExecuteAsync();

        Assert.False(vm.CrearVentaCommand.CanExecute(null));
        Assert.Equal(MensajesVentas.RegistroNoDisponible, vm.MotivoRegistro);
    }

    [Fact]
    public async Task UnErrorTecnicoMuestraUnMensajeGenericoSinElTextoTecnico()
    {
        var reloj = new RelojFijo(Hoy);
        var servicio = new VentaServicio(
            new LotesFalla(), new RestriccionesSimulado(), new PagosSimulado(), new SesionSimulada(), reloj);
        var vm = new VentaViewModel(servicio);

        await vm.InicializarCommand.ExecuteAsync();

        Assert.Equal(MensajesVentas.ErrorCarga, vm.Advertencias);
        Assert.DoesNotContain("Secreto", vm.Advertencias);
    }

    [Fact]
    public void ElConstructorRechazaUnServicioNulo()
    {
        Assert.Throws<ArgumentNullException>(() => new VentaViewModel(null!));
    }

    /// <summary>Lotes que falla como una base no disponible, con un texto técnico que no debe llegar a la pantalla.</summary>
    private sealed class LotesFalla : ILotes
    {
        private static Exception Falla() => new InvalidOperationException("Conexión rechazada a Secreto");

        public IReadOnlyList<MedicamentoVendibleDto> MedicamentosVendibles() => throw Falla();

        public IReadOnlyList<RemesaVendibleDto> SugerirFefo(int idProducto) => throw Falla();

        public IReadOnlyList<DatosRemesaDto> DatosRemesas(IReadOnlyCollection<int> idsLote) => throw Falla();

        public IReadOnlyList<DatosRemesaDto> RemesasDevolvibles(int idProveedor) => throw Falla();

        public IReadOnlyList<DatosRemesaDto> RemesasPorVencer(DateOnly fechaLimite) => throw Falla();

        public IReadOnlyList<DatosRemesaDto> RemesasDisponibles() => throw Falla();

        public void Descontar(int idLote, int cantidad) => throw Falla();

        public void Restituir(int idLote, int cantidad) => throw Falla();

        public void Bloquear(int idLote) => throw Falla();

        public void Liberar(int idLote) => throw Falla();
    }
}
