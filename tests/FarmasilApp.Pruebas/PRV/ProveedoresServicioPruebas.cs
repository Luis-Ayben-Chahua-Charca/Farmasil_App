using FarmasilApp.Modelo.Comun;
using FarmasilApp.Modelo.PRV;
using FarmasilApp.Modelo.Seguridad;

namespace FarmasilApp.Pruebas.PRV;

public class ProveedoresServicioPruebas
{
    private static ProveedoresServicio Servicio(
        RepositorioDeProveedoresFalso repositorio,
        Rol rol = Rol.Administrador) =>
        new(repositorio, new SesionSimulada(rol));

    // =============================
    // ESP-0038: Consulta
    // =============================

    [Fact]
    [Trait("Especificacion", "ESP-0038")]
    [Trait("Tecnica", "T-002")]
    public void ConsultarNormalizaLosCriteriosAntesDeLlamarAlRepositorio()
    {
        var repositorio = new RepositorioDeProveedoresFalso();

        Servicio(repositorio).Consultar("  Andina  ", "  Activo  ");

        var consulta = repositorio.UltimaConsulta;
        Assert.NotNull(consulta);
        Assert.Equal("Andina", consulta.Value.Busqueda);
        Assert.Equal("Activo", consulta.Value.Estado);
    }

    [Fact]
    [Trait("Especificacion", "ESP-0038")]
    [Trait("Tecnica", "T-002")]
    public void ConsultarConBusquedaVaciaLaPasaComoNulo()
    {
        var repositorio = new RepositorioDeProveedoresFalso();

        Servicio(repositorio).Consultar("   ", null);

        var consulta = repositorio.UltimaConsulta;
        Assert.NotNull(consulta);
        Assert.Null(consulta.Value.Busqueda);
        Assert.Null(consulta.Value.Estado);
    }

    [Fact]
    [Trait("Especificacion", "ESP-0038")]
    [Trait("Tecnica", "T-002")]
    public void ConsultarConEstadoVacioLoPasaComoNulo()
    {
        var repositorio = new RepositorioDeProveedoresFalso();

        Servicio(repositorio).Consultar(null, "   ");

        var consulta = repositorio.UltimaConsulta;
        Assert.NotNull(consulta);
        Assert.Null(consulta.Value.Estado);
    }

    [Fact]
    [Trait("Especificacion", "ESP-0038")]
    [Trait("Tecnica", "T-001")]
    public void ConsultarSinCriteriosListaTodos()
    {
        var repositorio = new RepositorioDeProveedoresFalso();
        repositorio.Resultados.Add(new ProveedorConsulta
        {
            IdProveedor = 1,
            Ruc = "20100070970",
            RazonSocial = "Drogueria Andina SAC",
            Telefono = "054223344",
            Estado = "Activo"
        });

        var resultados = Servicio(repositorio).Consultar(null, null);

        Assert.Single(resultados);
        Assert.Equal("Drogueria Andina SAC", resultados[0].RazonSocial);

        var consulta = repositorio.UltimaConsulta;
        Assert.NotNull(consulta);
        Assert.Null(consulta.Value.Busqueda);
        Assert.Null(consulta.Value.Estado);
    }

    [Theory]
    [Trait("Especificacion", "ESP-0038")]
    [Trait("Tecnica", "T-001")]
    [InlineData("20100070970", null, "20100070970", null)]
    [InlineData("Andina", null, "Andina", null)]
    [InlineData(null, "Activo", null, "Activo")]
    [InlineData(null, "Inactivo", null, "Inactivo")]
    [InlineData("Salud", "Activo", "Salud", "Activo")]
    public void ConsultarPasaElCriterioAlRepositorio(
        string? busqueda, string? estado,
        string? busquedaEsperada, string? estadoEsperado)
    {
        var repositorio = new RepositorioDeProveedoresFalso();

        Servicio(repositorio).Consultar(busqueda, estado);

        var consulta = repositorio.UltimaConsulta;
        Assert.NotNull(consulta);
        Assert.Equal(busquedaEsperada, consulta.Value.Busqueda);
        Assert.Equal(estadoEsperado, consulta.Value.Estado);
    }

    [Fact]
    [Trait("Especificacion", "ESP-0038")]
    [Trait("Tecnica", "T-001")]
    public void ConsultarSinResultadosDevuelveListaVacia()
    {
        var repositorio = new RepositorioDeProveedoresFalso();

        var resultados = Servicio(repositorio).Consultar("NoExiste", null);

        Assert.Empty(resultados);
    }

    [Fact]
    [Trait("Especificacion", "ESP-0038")]
    [Trait("Tecnica", "T-002")]
    public void ConsultarDevuelveProveedorConTelefonoNulo()
    {
        var repositorio = new RepositorioDeProveedoresFalso();
        repositorio.Resultados.Add(new ProveedorConsulta
        {
            IdProveedor = 4,
            Ruc = "20501112223",
            RazonSocial = "Proveedor Inactivo SAC",
            Telefono = null,
            Estado = "Inactivo"
        });

        var resultados = Servicio(repositorio).Consultar(null, null);

        Assert.Single(resultados);
        Assert.Null(resultados[0].Telefono);
        Assert.Equal("Inactivo", resultados[0].Estado);
    }

    [Fact]
    [Trait("Especificacion", "ESP-0038")]
    [Trait("Tecnica", "T-001")]
    public void ConsultarConEstadoInvalidoLanzaReglaDeNegocioException()
    {
        var repositorio = new RepositorioDeProveedoresFalso();

        var excepcion = Assert.Throws<ReglaDeNegocioException>(
            () => Servicio(repositorio).Consultar(null, "Suspendido"));

        Assert.Equal("El estado debe ser Activo o Inactivo.", excepcion.Message);
        Assert.Null(repositorio.UltimaConsulta);
    }

    [Fact]
    [Trait("Especificacion", "ESP-0037")]
    [Trait("Tecnica", "T-002")]
    public void CrearCaminoBasicoValidaYDevuelveIdCreado()
    {
        var repositorio = new RepositorioDeProveedoresFalso
        {
            IdProveedorADevolver = 7
        };
        var servicio = new ProveedoresServicio(
            repositorio,
            new SesionSimulada(Rol.Administrador));

        var idCreado = servicio.Crear(
            ruc: "20100000001",
            razonSocial: "Distribuidora Farmaceutica del Sur S.A.C.",
            telefono: "958123456",
            estado: ProveedorEscritura.EstadoActivo);

        Assert.Equal(7, idCreado);
        Assert.NotNull(repositorio.UltimoCreado);
        Assert.Equal("20100000001", repositorio.UltimoCreado.Ruc);
        Assert.Equal("Distribuidora Farmaceutica del Sur S.A.C.", repositorio.UltimoCreado.RazonSocial);
        Assert.Equal("958123456", repositorio.UltimoCreado.Telefono);
        Assert.Equal(ProveedorEscritura.EstadoActivo, repositorio.UltimoCreado.Estado);
    }


    [Theory]
    [Trait("Especificacion", "ESP-0037")]
    [Trait("Tecnica", "T-001")]
    [InlineData("2010000000")]      // 10 dígitos
    [InlineData("201000000012")]    // 12 dígitos
    [InlineData("2010000000A")]     // con letras
    [InlineData(" 20100000001")]    // espacio al inicio
    [InlineData("20100000001 ")]    // espacio al final
    [InlineData("20100 000001")]    // espacio intermedio
    public void CrearConRucInvalidoLanzaReglaDeNegocioSinCrear(string rucInvalido)
    {
        var repositorio = new RepositorioDeProveedoresFalso();
        var servicio = new ProveedoresServicio(
            repositorio,
            new SesionSimulada(Rol.Administrador));

        var ex = Assert.Throws<ReglaDeNegocioException>(() =>
            servicio.Crear(
                ruc: rucInvalido,
                razonSocial: "Proveedor Valido S.A.C.",
                telefono: "958123456",
                estado: ProveedorEscritura.EstadoActivo));

        Assert.Equal(MensajesProveedores.RucInvalido, ex.Message);
        Assert.Null(repositorio.UltimoCreado);
    }

    [Fact]
    [Trait("Especificacion", "ESP-0037")]
    [Trait("Tecnica", "T-001")]
    public void CrearConRucDeOnceDigitosValidoCreaProveedor()
    {
        const string rucValido = "20100000001";
        var repositorio = new RepositorioDeProveedoresFalso
        {
            IdProveedorADevolver = 12
        };
        var servicio = new ProveedoresServicio(
            repositorio,
            new SesionSimulada(Rol.Administrador));

        var id = servicio.Crear(
            ruc: rucValido,
            razonSocial: "Proveedor Valido S.A.C.",
            telefono: null,
            estado: ProveedorEscritura.EstadoActivo);

        Assert.Equal(12, id);
        Assert.NotNull(repositorio.UltimoCreado);
        Assert.Equal(rucValido, repositorio.UltimoCreado.Ruc);
    }

    [Fact]
    [Trait("Especificacion", "ESP-0037")]
    [Trait("Tecnica", "T-001")]
    public void CrearConRucDuplicadoLanzaRucRepetidoSinCrear()
    {
        const string rucDuplicado = "20100000001";
        var repositorio = new RepositorioDeProveedoresFalso
        {
            RucExistente = rucDuplicado
        };
        var servicio = new ProveedoresServicio(
            repositorio,
            new SesionSimulada(Rol.Administrador));

        var ex = Assert.Throws<ReglaDeNegocioException>(() =>
            servicio.Crear(
                ruc: rucDuplicado,
                razonSocial: "Otro Proveedor S.A.C.",
                telefono: null,
                estado: ProveedorEscritura.EstadoActivo));

        Assert.Equal(MensajesProveedores.RucRepetido, ex.Message);
        Assert.Null(repositorio.UltimoCreado);
    }


    [Fact]
    [Trait("Especificacion", "ESP-0037")]
    [Trait("Tecnica", "T-004")]
    public void CrearConservaCerosALaIzquierdaEnRuc()
    {
        const string rucConCeros = "00123456789";
        var repositorio = new RepositorioDeProveedoresFalso
        {
            IdProveedorADevolver = 15
        };
        var servicio = new ProveedoresServicio(
            repositorio,
            new SesionSimulada(Rol.Administrador));

        var id = servicio.Crear(
            ruc: rucConCeros,
            razonSocial: "Proveedor Ceros S.A.C.",
            telefono: null,
            estado: ProveedorEscritura.EstadoActivo);

        Assert.Equal(15, id);
        Assert.NotNull(repositorio.UltimoCreado);
        Assert.Equal(rucConCeros, repositorio.UltimoCreado.Ruc);
        Assert.StartsWith("00", repositorio.UltimoCreado.Ruc);
    }

    [Theory]
    [Trait("Especificacion", "ESP-0037")]
    [Trait("Tecnica", "T-001")]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void CrearSinTelefonoCompletaRegistroConTelefonoNulo(string? telefonoOpcional)
    {
        var repositorio = new RepositorioDeProveedoresFalso
        {
            IdProveedorADevolver = 20
        };
        var servicio = new ProveedoresServicio(
            repositorio,
            new SesionSimulada(Rol.Administrador));

        var id = servicio.Crear(
            ruc: "20100000001",
            razonSocial: "Botica Central S.A.C.",
            telefono: telefonoOpcional,
            estado: ProveedorEscritura.EstadoActivo);

        Assert.Equal(20, id);
        Assert.NotNull(repositorio.UltimoCreado);
        Assert.Null(repositorio.UltimoCreado.Telefono);
    }

    [Fact]
    [Trait("Especificacion", "ESP-0038")]
    [Trait("Tecnica", "T-002")]
    public void ConsultarCaminoBasicoNormalizaFiltrosYDevuelveResultados()
    {
        var repositorio = new RepositorioDeProveedoresFalso
        {
            Resultados =
        {
            new ProveedorConsulta
            {
                Ruc = "20100000001",
                RazonSocial = "Distribuidora Farmaceutica del Sur S.A.C.",
                Telefono = "958123456",
                Estado = ProveedorEscritura.EstadoActivo,
            },
        },
        };
        var servicio = new ProveedoresServicio(
            repositorio,
            new SesionSimulada(Rol.Administrador));

        var resultado = servicio.Consultar("  20100000001  ", "  Activo  ");

        Assert.NotNull(repositorio.UltimaConsulta);
        Assert.Equal("20100000001", repositorio.UltimaConsulta!.Value.Busqueda);
        Assert.Equal(ProveedorEscritura.EstadoActivo, repositorio.UltimaConsulta.Value.Estado);
        Assert.Single(resultado);
        Assert.Equal("958123456", resultado[0].Telefono);
    }


    [Theory]
    [Trait("Especificacion", "ESP-0038")]
    [Trait("Tecnica", "T-001")]
    [InlineData("20100000001", null, "20100000001", null)]                                // RUC exacto
    [InlineData("  Farmaceutica  ", null, "Farmaceutica", null)]                         // Razón social parcial (recortada)
    [InlineData(null, ProveedorEscritura.EstadoActivo, null, ProveedorEscritura.EstadoActivo)] // Estado 'Activo'
    [InlineData(null, ProveedorEscritura.EstadoInactivo, null, ProveedorEscritura.EstadoInactivo)] // Estado 'Inactivo'
    [InlineData("  Botica  ", ProveedorEscritura.EstadoActivo, "Botica", ProveedorEscritura.EstadoActivo)] // Búsqueda + estado
    [InlineData(null, null, null, null)]                                                 // Sin criterios (null)
    [InlineData("   ", "   ", null, null)]                                               // Sin criterios (espacios -> null)
    public void ConsultarParticionesDeCriteriosNormalizaYDelegaAlRepositorio(
    string? busquedaEntrada,
    string? estadoEntrada,
    string? busquedaEsperada,
    string? estadoEsperado)
    {
        var repositorio = new RepositorioDeProveedoresFalso
        {
            Resultados =
        {
            new ProveedorConsulta
            {
                Ruc = "20100000001",
                RazonSocial = "Distribuidora Farmaceutica del Sur S.A.C.",
                Telefono = "958123456",
                Estado = ProveedorEscritura.EstadoActivo,
            },
        },
        };
        var servicio = new ProveedoresServicio(
            repositorio,
            new SesionSimulada(Rol.Administrador));

        var resultado = servicio.Consultar(busquedaEntrada, estadoEntrada);

        Assert.NotNull(repositorio.UltimaConsulta);
        Assert.Equal(busquedaEsperada, repositorio.UltimaConsulta!.Value.Busqueda);
        Assert.Equal(estadoEsperado, repositorio.UltimaConsulta.Value.Estado);
        Assert.Single(resultado);
    }

    [Fact]
    [Trait("Especificacion", "ESP-0038")]
    [Trait("Tecnica", "T-001")]
    public void ConsultarSinResultadosDevuelveColeccionVacia()
    {
        var repositorio = new RepositorioDeProveedoresFalso(); // Resultados inicia vacía
        var servicio = new ProveedoresServicio(
            repositorio,
            new SesionSimulada(Rol.Administrador));

        var resultado = servicio.Consultar("99999999999", ProveedorEscritura.EstadoInactivo);

        Assert.NotNull(repositorio.UltimaConsulta);
        Assert.Equal("99999999999", repositorio.UltimaConsulta!.Value.Busqueda);
        Assert.Equal(ProveedorEscritura.EstadoInactivo, repositorio.UltimaConsulta.Value.Estado);
        Assert.Empty(resultado);
    }


    [Fact]
    [Trait("Especificacion", "ESP-0039")]
    [Trait("Tecnica", "T-002")]
    public void ActualizarCaminoBasicoValidaYDelegaAlRepositorio()
    {
        const int idProveedor = 5;
        var repositorio = new RepositorioDeProveedoresFalso();
        var servicio = new ProveedoresServicio(
            repositorio,
            new SesionSimulada(Rol.Administrador));

        servicio.Actualizar(
            idProveedor: idProveedor,
            ruc: "20100000001",
            razonSocial: "  Distribuidora Farmaceutica Actualizada S.A.C.  ",
            telefono: "  958987654  ",
            estado: ProveedorEscritura.EstadoInactivo);

        Assert.NotNull(repositorio.UltimaActualizacion);
        Assert.Equal(idProveedor, repositorio.UltimaActualizacion!.Value.IdProveedor);

        var editado = repositorio.UltimaActualizacion.Value.Proveedor;
        Assert.Equal("20100000001", editado.Ruc);
        Assert.Equal("Distribuidora Farmaceutica Actualizada S.A.C.", editado.RazonSocial);
        Assert.Equal("958987654", editado.Telefono);
        Assert.Equal(ProveedorEscritura.EstadoInactivo, editado.Estado);
    }

    [Fact]
    [Trait("Especificacion", "ESP-0039")]
    [Trait("Tecnica", "T-001")]
    public void ActualizarConRucDeOtroProveedorLanzaRucRepetidoSinActualizar()
    {
        const int idProveedorAEditar = 5;
        const string rucDeOtroProveedor = "20999999999";

        var repositorio = new RepositorioDeProveedoresFalso
        {
            RucExistente = rucDeOtroProveedor,
        };
        var servicio = new ProveedoresServicio(
            repositorio,
            new SesionSimulada(Rol.Administrador));

        var ex = Assert.Throws<ReglaDeNegocioException>(() =>
            servicio.Actualizar(
                idProveedor: idProveedorAEditar,
                ruc: rucDeOtroProveedor,
                razonSocial: "Razon Social Modificada S.A.C.",
                telefono: "958123456",
                estado: ProveedorEscritura.EstadoActivo));

        Assert.Equal(MensajesProveedores.RucRepetido, ex.Message);
        Assert.Null(repositorio.UltimaActualizacion);
    }

    [Fact]
    [Trait("Especificacion", "ESP-0040")]
    [Trait("Tecnica", "T-002")]
    public void DarDeBajaCaminoBasicoDelegaYDevuelveResultado()
    {
        const int idProveedor = 9;
        var repositorio = new RepositorioDeProveedoresFalso
        {
            ResultadoBajaADevolver = ResultadoBajaProveedor.Eliminado,
        };
        var servicio = new ProveedoresServicio(
            repositorio,
            new SesionSimulada(Rol.Administrador));

        var resultado = servicio.DarDeBaja(idProveedor);

        Assert.Equal(idProveedor, repositorio.UltimaBaja);
        Assert.Equal(ResultadoBajaProveedor.Eliminado, resultado);
    }


    [Fact]
    [Trait("Especificacion", "ESP-0040")]
    [Trait("Tecnica", "T-003")]
    public void VerificarBajaConOrdenesPendientesLanzaReglaDeNegocio()
    {
        const int idProveedor = 4;
        var repositorio = new RepositorioDeProveedoresFalso
        {
            OrdenesDevPendientes = true,
        };
        var servicio = new ProveedoresServicio(
            repositorio,
            new SesionSimulada(Rol.Administrador));

        var ex = Assert.Throws<ReglaDeNegocioException>(() =>
            servicio.VerificarBaja(idProveedor));

        Assert.Equal(MensajesProveedores.OrdenesPendientes, ex.Message);
    }

    [Theory]
    [Trait("Especificacion", "ESP-0040")]
    [Trait("Tecnica", "T-003")]
    [InlineData(ResultadoBajaProveedor.Desactivado)] // Con registros asociados
    [InlineData(ResultadoBajaProveedor.Eliminado)]   // Sin registros asociados
    public void DarDeBajaRamasConYSinRegistrosDevuelveResultadoEsperado(
        ResultadoBajaProveedor resultadoEsperado)
    {
        const int idProveedor = 4;
        var repositorio = new RepositorioDeProveedoresFalso
        {
            ResultadoBajaADevolver = resultadoEsperado,
        };
        var servicio = new ProveedoresServicio(
            repositorio,
            new SesionSimulada(Rol.Administrador));

        var resultado = servicio.DarDeBaja(idProveedor);

        Assert.Equal(idProveedor, repositorio.UltimaBaja);
        Assert.Equal(resultadoEsperado, resultado);
    }

    [Fact]
    [Trait("Especificacion", "ESP-0040")]
    [Trait("Tecnica", "T-003")]
    public void ConfirmacionNegativaNoInvocaDarDeBaja()
    {
        const int idProveedor = 4;
        var repositorio = new RepositorioDeProveedoresFalso();
        var servicio = new ProveedoresServicio(
            repositorio,
            new SesionSimulada(Rol.Administrador));

        servicio.VerificarBaja(idProveedor);

        Assert.Null(repositorio.UltimaBaja);
    }

    [Fact]
    [Trait("Modulo", "10")]
    [Trait("Tecnica", "T-012")]
    public void SesionTecnicoRechazaLasCuatroOperacionesDeProveedores()
    {
        var repositorio = new RepositorioDeProveedoresFalso();
        var servicio = new ProveedoresServicio(
            repositorio,
            new SesionSimulada(Rol.Tecnico));

        Assert.Throws<AccesoDenegadoException>(() =>
            servicio.Crear(
                ruc: "20100000001",
                razonSocial: "Proveedor de Prueba S.A.C.",
                telefono: "958123456",
                estado: ProveedorEscritura.EstadoActivo));

        Assert.Throws<AccesoDenegadoException>(() =>
            servicio.Consultar(null, null));

        Assert.Throws<AccesoDenegadoException>(() =>
            servicio.Actualizar(
                idProveedor: 1,
                ruc: "20100000001",
                razonSocial: "Proveedor Actualizado S.A.C.",
                telefono: "958123456",
                estado: ProveedorEscritura.EstadoActivo));

        Assert.Throws<AccesoDenegadoException>(() =>
            servicio.DarDeBaja(idProveedor: 1));

        Assert.Null(repositorio.UltimaActualizacion);
        Assert.Null(repositorio.UltimaBaja);
    }
}
