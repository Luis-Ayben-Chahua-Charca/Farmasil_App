using FarmasilApp.Modelo.Comun;
using FarmasilApp.Modelo.INV;
using FarmasilApp.Modelo.LOT;
using FarmasilApp.Modelo.PRV;
using FarmasilApp.Modelo.Seguridad;

namespace FarmasilApp.Pruebas.LOT;

public class LotesServicioPruebas
{
    private static readonly DateOnly Hoy = new(2026, 11, 25);

    private static LotesServicio Servicio(
        RepositorioLotesFalso repositorio,
        Rol rol = Rol.Administrador,
        InventarioFalso? inventario = null,
        ProveedoresFalso? proveedores = null) =>
        new(
            repositorio,
            inventario ?? new InventarioFalso(),
            proveedores ?? new ProveedoresFalso(),
            new SesionSimulada(rol),
            new RelojFijo(Hoy));

    [Fact]
    [Trait("Especificacion", "ESP-0041")]
    [Trait("Tecnica", "T-002")]
    public void CrearCaminoBasicoRegistraRemesaDisponibleYDevuelveId()
    {
        var repositorio = new RepositorioLotesFalso
        {
            IdCreado = 15
        };

        var id = Servicio(repositorio).Crear(
            idProducto: 1,
            idProveedor: 1,
            numeroLote: "  PAR-NUEVO-01  ",
            fechaVencimiento: Hoy.AddMonths(12),
            stockInicial: 30,
            fechaIngreso: Hoy);

        Assert.Equal(15, id);

        var remesa = Assert.IsType<NuevaRemesa>(
            repositorio.UltimaCreacion);

        Assert.Equal(1, remesa.IdProducto);
        Assert.Equal(1, remesa.IdProveedor);
        Assert.Equal("PAR-NUEVO-01", remesa.NumeroLote);
        Assert.Equal(Hoy.AddMonths(12), remesa.FechaVencimiento);
        Assert.Equal(30, remesa.StockInicial);
        Assert.Equal(Hoy, remesa.FechaIngreso);
    }

    [Fact]
    [Trait("Especificacion", "ESP-0041")]
    [Trait("Tecnica", "T-001")]
    public void CrearRechazaMedicamentoDescontinuado()
    {
        var repositorio = new RepositorioLotesFalso();

        var ex = Assert.Throws<ReglaDeNegocioException>(() =>
            Servicio(repositorio).Crear(
                idProducto: 2,
                idProveedor: 1,
                numeroLote: "IBU-01",
                fechaVencimiento: Hoy.AddMonths(6),
                stockInicial: 10,
                fechaIngreso: Hoy));

        Assert.Equal(
            MensajesLotes.ProductoNoDisponible,
            ex.Message);

        Assert.Null(repositorio.UltimaCreacion);
    }

    [Fact]
    [Trait("Especificacion", "ESP-0041")]
    [Trait("Tecnica", "T-001")]
    public void CrearRechazaNumeroDeLoteDuplicadoParaElMismoMedicamento()
    {
        var repositorio = new RepositorioLotesFalso
        {
            NumeroDuplicado = true
        };

        var ex = Assert.Throws<ReglaDeNegocioException>(() =>
            Servicio(repositorio).Crear(
                idProducto: 1,
                idProveedor: 1,
                numeroLote: "PAR-001",
                fechaVencimiento: Hoy.AddMonths(6),
                stockInicial: 10,
                fechaIngreso: Hoy));

        Assert.Equal(
            MensajesLotes.NumeroLoteDuplicado,
            ex.Message);

        Assert.Null(repositorio.UltimaCreacion);
    }

    [Fact]
    [Trait("Especificacion", "ESP-0041")]
    [Trait("Tecnica", "T-001")]
    public void CrearRechazaVencimientoIgualALaFechaActual()
    {
        var repositorio = new RepositorioLotesFalso();

        var ex = Assert.Throws<ReglaDeNegocioException>(() =>
            Servicio(repositorio).Crear(
                idProducto: 1,
                idProveedor: 1,
                numeroLote: "PAR-001",
                fechaVencimiento: Hoy,
                stockInicial: 10,
                fechaIngreso: Hoy));

        Assert.Equal(
            MensajesLotes.FechaVencimientoInvalida,
            ex.Message);
    }

    [Theory]
    [Trait("Especificacion", "ESP-0041")]
    [Trait("Tecnica", "T-001")]
    [InlineData(0)]
    [InlineData(-1)]
    public void CrearRechazaStockInicialNoPositivo(int stock)
    {
        var repositorio = new RepositorioLotesFalso();

        var ex = Assert.Throws<ReglaDeNegocioException>(() =>
            Servicio(repositorio).Crear(
                idProducto: 1,
                idProveedor: 1,
                numeroLote: "PAR-001",
                fechaVencimiento: Hoy.AddMonths(6),
                stockInicial: stock,
                fechaIngreso: Hoy));

        Assert.Equal(
            MensajesLotes.StockInicialInvalido,
            ex.Message);
    }

    [Fact]
    [Trait("Especificacion", "ESP-0041")]
    [Trait("Tecnica", "T-001")]
    public void CrearRechazaFechaDeIngresoFutura()
    {
        var repositorio = new RepositorioLotesFalso();

        var ex = Assert.Throws<ReglaDeNegocioException>(() =>
            Servicio(repositorio).Crear(
                idProducto: 1,
                idProveedor: 1,
                numeroLote: "PAR-001",
                fechaVencimiento: Hoy.AddMonths(6),
                stockInicial: 10,
                fechaIngreso: Hoy.AddDays(1)));

        Assert.Equal(
            MensajesLotes.FechaIngresoInvalida,
            ex.Message);
    }

    [Fact]
    [Trait("Especificacion", "ESP-0042")]
    [Trait("Tecnica", "T-002")]
    public void TecnicoPuedeConsultarYElNumeroDeLoteSeNormaliza()
    {
        var repositorio = new RepositorioLotesFalso();

        var resultado = Servicio(
                repositorio,
                Rol.Tecnico)
            .Consultar(
                idProducto: 1,
                numeroLote: "  PAR-001  ",
                estado: EstadoLote.Disponible);

        Assert.NotNull(resultado);
        Assert.NotNull(repositorio.UltimaConsulta);

        var consulta = repositorio.UltimaConsulta.Value;

        Assert.Equal(1, consulta.IdProducto);
        Assert.Equal("PAR-001", consulta.NumeroLote);
        Assert.Equal(
            EstadoLote.Disponible,
            consulta.Estado);
    }

    [Fact]
    [Trait("Especificacion", "ESP-0043")]
    [Trait("Tecnica", "T-001")]
    public void ActualizarPermiteCorregirOtrosCamposSiLaFechaVencidaNoCambio()
    {
        var repositorio = new RepositorioLotesFalso
        {
            RemesaActual = Remesa(
                estado: EstadoLote.Disponible,
                vencimiento: Hoy.AddDays(-10),
                stock: 8)
        };

        Servicio(repositorio).Actualizar(
            idLote: 10,
            idProveedor: 1,
            numeroLote: "PAR-CORREGIDO",
            fechaVencimiento: Hoy.AddDays(-10),
            stockActual: 7);

        Assert.NotNull(repositorio.UltimaActualizacion);

        var actualizacion =
            repositorio.UltimaActualizacion.Value;

        Assert.Equal(10, actualizacion.IdLote);
        Assert.Equal(
            "PAR-CORREGIDO",
            actualizacion.Remesa.NumeroLote);
        Assert.Equal(
            Hoy.AddDays(-10),
            actualizacion.Remesa.FechaVencimiento);
        Assert.Equal(7, actualizacion.Remesa.StockActual);
    }

    [Fact]
    [Trait("Especificacion", "ESP-0043")]
    [Trait("Tecnica", "T-001")]
    public void ActualizarRechazaCambiarLaFechaPorOtraYaVencida()
    {
        var repositorio = new RepositorioLotesFalso
        {
            RemesaActual = Remesa(
                vencimiento: Hoy.AddMonths(3))
        };

        var ex = Assert.Throws<ReglaDeNegocioException>(() =>
            Servicio(repositorio).Actualizar(
                idLote: 10,
                idProveedor: 1,
                numeroLote: "PAR-001",
                fechaVencimiento: Hoy.AddDays(-1),
                stockActual: 8));

        Assert.Equal(
            MensajesLotes.FechaVencimientoInvalida,
            ex.Message);

        Assert.Null(repositorio.UltimaActualizacion);
    }

    [Fact]
    [Trait("Especificacion", "ESP-0043")]
    [Trait("Tecnica", "T-001")]
    public void ActualizarRechazaCambiarProveedorSiLaRemesaFiguraEnUnaDevolucion()
    {
        var repositorio = new RepositorioLotesFalso
        {
            RemesaActual = Remesa(idProveedor: 1),
            TieneDetalle = true
        };

        var ex = Assert.Throws<ReglaDeNegocioException>(() =>
            Servicio(repositorio).Actualizar(
                idLote: 10,
                idProveedor: 2,
                numeroLote: "PAR-001",
                fechaVencimiento: Hoy.AddMonths(6),
                stockActual: 8));

        Assert.Equal(
            "El proveedor no puede cambiarse: la remesa figura en una orden de devolución",
            ex.Message);

        Assert.Null(repositorio.UltimaActualizacion);
    }

    [Fact]
    [Trait("Especificacion", "ESP-0043")]
    [Trait("Tecnica", "T-001")]
    public void ActualizarRechazaStockInferiorALasUnidadesComprometidas()
    {
        var repositorio = new RepositorioLotesFalso
        {
            RemesaActual = Remesa(stock: 10),
            CantidadComprometida = 6
        };

        var ex = Assert.Throws<ReglaDeNegocioException>(() =>
            Servicio(repositorio).Actualizar(
                idLote: 10,
                idProveedor: 1,
                numeroLote: "PAR-001",
                fechaVencimiento: Hoy.AddMonths(6),
                stockActual: 5));

        Assert.Equal(
            "Las existencias no pueden ser inferiores a las unidades comprometidas en devoluciones pendientes",
            ex.Message);

        Assert.Null(repositorio.UltimaActualizacion);
    }

    [Fact]
    [Trait("Especificacion", "ESP-0043")]
    [Trait("Tecnica", "T-002")]
    public void ActualizarMantieneBloqueadaUnaRemesaAunqueElStockQuedeEnCero()
    {
        var repositorio = new RepositorioLotesFalso
        {
            RemesaActual = Remesa(
                estado: EstadoLote.BloqueadoPorDevolucion,
                stock: 5)
        };

        Servicio(repositorio).Actualizar(
            idLote: 10,
            idProveedor: 1,
            numeroLote: "PAR-001",
            fechaVencimiento: Hoy.AddMonths(6),
            stockActual: 0);

        Assert.NotNull(repositorio.UltimaActualizacion);

        Assert.Equal(
            EstadoLote.BloqueadoPorDevolucion,
            repositorio.UltimaActualizacion.Value.Estado);
    }

    [Fact]
    [Trait("Especificacion", "ESP-0043")]
    [Trait("Tecnica", "T-002")]
    public void ActualizarReactivaUnaRemesaAgotadaSiRecuperaStockYNoEstaVencida()
    {
        var repositorio = new RepositorioLotesFalso
        {
            RemesaActual = Remesa(
                estado: EstadoLote.Agotado,
                vencimiento: Hoy.AddMonths(6),
                stock: 0)
        };

        Servicio(repositorio).Actualizar(
            idLote: 10,
            idProveedor: 1,
            numeroLote: "PAR-001",
            fechaVencimiento: Hoy.AddMonths(6),
            stockActual: 10);

        Assert.NotNull(repositorio.UltimaActualizacion);

        Assert.Equal(
            EstadoLote.Disponible,
            repositorio.UltimaActualizacion.Value.Estado);
    }

    [Fact]
    [Trait("Especificacion", "ESP-0044")]
    [Trait("Tecnica", "T-001")]
    public void VerificarBajaRechazaUnaRemesaComprometidaEnDevolucionPendiente()
    {
        var repositorio = new RepositorioLotesFalso
        {
            RemesaActual = Remesa(),
            TienePendiente = true
        };

        var ex = Assert.Throws<ReglaDeNegocioException>(() =>
            Servicio(repositorio).VerificarBaja(10));

        Assert.Equal(
            "La remesa está comprometida en una orden de devolución pendiente",
            ex.Message);
    }

    [Fact]
    [Trait("Especificacion", "ESP-0044")]
    [Trait("Tecnica", "T-002")]
    public void DarDeBajaCaminoBasicoDelegaAlRepositorioYDevuelveResultado()
    {
        var repositorio = new RepositorioLotesFalso
        {
            RemesaActual = Remesa(),
            ResultadoBaja = ResultadoBajaRemesa.Agotada
        };

        var resultado =
            Servicio(repositorio).DarDeBaja(10);

        Assert.Equal(
            ResultadoBajaRemesa.Agotada,
            resultado);

        Assert.Equal(10, repositorio.UltimaBaja);
    }

    [Fact]
    [Trait("Especificacion", "ESP-0042")]
    [Trait("Tecnica", "T-012")]
    public void TecnicoPuedeConsultarPeroNoPuedeCrearActualizarNiDarDeBaja()
    {
        var repositorio = new RepositorioLotesFalso
        {
            RemesaActual = Remesa()
        };

        var servicio =
            Servicio(repositorio, Rol.Tecnico);

        var consulta =
            servicio.Consultar(null, null, null);

        Assert.NotNull(consulta);

        Assert.Throws<AccesoDenegadoException>(() =>
            servicio.Crear(
                1,
                1,
                "PAR-001",
                Hoy.AddMonths(6),
                10,
                Hoy));

        Assert.Throws<AccesoDenegadoException>(() =>
            servicio.Actualizar(
                10,
                1,
                "PAR-001",
                Hoy.AddMonths(6),
                10));

        Assert.Throws<AccesoDenegadoException>(() =>
            servicio.DarDeBaja(10));

        Assert.Null(repositorio.UltimaCreacion);
        Assert.Null(repositorio.UltimaActualizacion);
        Assert.Null(repositorio.UltimaBaja);
    }

    private static RemesaConsulta Remesa(
        EstadoLote estado = EstadoLote.Disponible,
        DateOnly? vencimiento = null,
        int stock = 10,
        int idProveedor = 1) =>
        new()
        {
            IdLote = 10,
            IdProducto = 1,
            NombreMedicamento = "Paracetamol 500 mg",
            NumeroLote = "PAR-001",
            FechaVencimiento =
                vencimiento ?? Hoy.AddMonths(6),
            StockActual = stock,
            Estado = estado,
            FechaIngreso = Hoy.AddMonths(-2),
            IdProveedor = idProveedor,
        };

    private sealed class RepositorioLotesFalso
        : ILotesRepositorio
    {
        public int IdCreado { get; set; } = 1;

        public bool NumeroDuplicado { get; set; }

        public bool TieneDetalle { get; set; }

        public int CantidadComprometida { get; set; }

        public bool TienePendiente { get; set; }

        public ResultadoBajaRemesa ResultadoBaja { get; set; } =
            ResultadoBajaRemesa.Eliminada;

        public RemesaConsulta? RemesaActual { get; set; }

        public NuevaRemesa? UltimaCreacion { get; private set; }

        public (
            int? IdProducto,
            string? NumeroLote,
            EstadoLote? Estado
        )? UltimaConsulta { get; private set; }

        public (
            int IdLote,
            ActualizacionRemesa Remesa,
            EstadoLote Estado
        )? UltimaActualizacion { get; private set; }

        public int? UltimaBaja { get; private set; }

        public IReadOnlyList<RemesaConsulta> Consultar(
            int? idProducto,
            string? numeroLote,
            EstadoLote? estado)
        {
            UltimaConsulta =
                (idProducto, numeroLote, estado);

            return RemesaActual is null
                ? Array.Empty<RemesaConsulta>()
                : new[] { RemesaActual };
        }

        public RemesaConsulta? ObtenerPorId(int idLote) =>
            RemesaActual;

        public bool ExisteNumeroLote(
            int idProducto,
            string numeroLote,
            int? excluirIdLote = null) =>
            NumeroDuplicado;

        public int Crear(NuevaRemesa remesa)
        {
            UltimaCreacion = remesa;
            return IdCreado;
        }

        public bool TieneDetalleDevolucion(int idLote) =>
            TieneDetalle;

        public int CantidadComprometidaPendiente(int idLote) =>
            CantidadComprometida;

        public void Actualizar(
            int idLote,
            ActualizacionRemesa remesa,
            EstadoLote estadoNuevo)
        {
            UltimaActualizacion =
                (idLote, remesa, estadoNuevo);
        }

        public bool TieneDevolucionPendiente(int idLote) =>
            TienePendiente;

        public ResultadoBajaRemesa DarDeBaja(int idLote)
        {
            UltimaBaja = idLote;
            return ResultadoBaja;
        }
    }

    private sealed class InventarioFalso : IInventario
    {
        private readonly MedicamentoDto[] _medicamentos =
        {
            new()
            {
                IdProducto = 1,
                Nombre = "Paracetamol 500 mg",
                AccionTerapeutica =
                    "Analgesico y antipiretico",
                PrecioVenta = 2.50m,
                Estado = EstadoProducto.Disponible,
                IdProveedor = 1,
                StockMinimo = 10,
            },
            new()
            {
                IdProducto = 2,
                Nombre = "Ibuprofeno 400 mg",
                AccionTerapeutica =
                    "Antiinflamatorio",
                PrecioVenta = 3.50m,
                Estado = EstadoProducto.Descontinuado,
                IdProveedor = 1,
                StockMinimo = 10,
            },
        };

        public IReadOnlyList<MedicamentoDto> Medicamentos() =>
            _medicamentos;

        public MedicamentoDto? ObtenerMedicamento(
            int idProducto) =>
            _medicamentos.SingleOrDefault(
                x => x.IdProducto == idProducto);
    }

    private sealed class ProveedoresFalso : IProveedores
    {
        private readonly ProveedorDto[] _proveedores =
        {
            new()
            {
                IdProveedor = 1,
                Ruc = "20100000001",
                RazonSocial = "Drogueria Andina SAC",
            },
            new()
            {
                IdProveedor = 2,
                Ruc = "20100000002",
                RazonSocial =
                    "Distribuidora Salud Sur SAC",
            },
        };

        public IReadOnlyList<ProveedorDto>
            ProveedoresActivos() =>
            _proveedores;

        public ProveedorDto? ObtenerProveedor(
            int idProveedor) =>
            _proveedores.SingleOrDefault(
                x => x.IdProveedor == idProveedor);
    }
}
