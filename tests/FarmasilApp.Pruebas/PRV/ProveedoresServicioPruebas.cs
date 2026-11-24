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
    [Trait("Especificacion", "ESP-0038")]
    [Trait("Tecnica", "T-002")]
    public void ElRepositorioEsObligatorio()
    {
        Assert.Throws<ArgumentNullException>(
            () => new ProveedoresServicio(null!, new SesionSimulada()));
    }

    [Fact]
    [Trait("Especificacion", "ESP-0038")]
    [Trait("Tecnica", "T-002")]
    public void LaSesionEsObligatoria()
    {
        Assert.Throws<ArgumentNullException>(
            () => new ProveedoresServicio(new RepositorioDeProveedoresFalso(), null!));
    }

    [Fact]
    [Trait("Especificacion", "ESP-0038")]
    [Trait("Tecnica", "T-002")]
    public void ConsultarComoTecnicoLanzaAccesoDenegado()
    {
        var repositorio = new RepositorioDeProveedoresFalso();

        Assert.Throws<AccesoDenegadoException>(
            () => Servicio(repositorio, Rol.Tecnico).Consultar(null, null));
    }

    // =============================
    // ESP-0037: Crear
    // =============================

    [Fact]
    [Trait("Especificacion", "ESP-0037")]
    [Trait("Tecnica", "T-001")]
    public void CrearConDatosValidosDelegaAlRepositorio()
    {
        var repositorio = new RepositorioDeProveedoresFalso { IdProveedorADevolver = 5 };

        var id = Servicio(repositorio).Crear("20100070970", "Drogueria Andina SAC", "054223344");

        Assert.Equal(5, id);
        Assert.NotNull(repositorio.UltimoCreado);
        Assert.Equal("20100070970", repositorio.UltimoCreado.Ruc);
        Assert.Equal("Drogueria Andina SAC", repositorio.UltimoCreado.RazonSocial);
        Assert.Equal("054223344", repositorio.UltimoCreado.Telefono);
    }

    [Fact]
    [Trait("Especificacion", "ESP-0037")]
    [Trait("Tecnica", "T-006")]
    public void CrearSinIndicarEstadoLoRegistraActivo()
    {
        var repositorio = new RepositorioDeProveedoresFalso();

        Servicio(repositorio).Crear("20100070970", "Drogueria SAC", null);

        Assert.Equal("Activo", repositorio.UltimoCreado!.Estado);
    }

    [Fact]
    [Trait("Especificacion", "ESP-0037")]
    [Trait("Tecnica", "T-006")]
    public void CrearConEstadoInactivoLoPasaAlRepositorio()
    {
        var repositorio = new RepositorioDeProveedoresFalso();

        Servicio(repositorio).Crear("20100070970", "Drogueria SAC", null, "Inactivo");

        Assert.Equal("Inactivo", repositorio.UltimoCreado!.Estado);
    }

    [Theory]
    [Trait("Especificacion", "ESP-0037")]
    [Trait("Tecnica", "T-002")]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("activo")]
    [InlineData("Suspendido")]
    public void CrearConEstadoInvalidoLanzaReglaDeNegocio(string? estado)
    {
        var repositorio = new RepositorioDeProveedoresFalso();

        var excepcion = Assert.Throws<ReglaDeNegocioException>(
            () => Servicio(repositorio).Crear("20100070970", "Drogueria SAC", null, estado));

        Assert.Equal(MensajesProveedores.EstadoInvalido, excepcion.Message);
        Assert.Null(repositorio.UltimoCreado);
    }

    [Fact]
    [Trait("Especificacion", "ESP-0039")]
    [Trait("Tecnica", "T-007")]
    public void ActualizarConEstadoInactivoLoPasaAlRepositorio()
    {
        var repositorio = new RepositorioDeProveedoresFalso();

        Servicio(repositorio).Actualizar(1, "20100070970", "Drogueria SAC", null, "Inactivo");

        Assert.Equal("Inactivo", repositorio.UltimaActualizacion!.Value.Proveedor.Estado);
    }

    [Theory]
    [Trait("Especificacion", "ESP-0039")]
    [Trait("Tecnica", "T-002")]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("Suspendido")]
    public void ActualizarConEstadoInvalidoLanzaReglaDeNegocio(string? estado)
    {
        var repositorio = new RepositorioDeProveedoresFalso();

        var excepcion = Assert.Throws<ReglaDeNegocioException>(
            () => Servicio(repositorio).Actualizar(1, "20100070970", "Drogueria SAC", null, estado));

        Assert.Equal(MensajesProveedores.EstadoInvalido, excepcion.Message);
        Assert.Null(repositorio.UltimaActualizacion);
    }

    [Fact]
    [Trait("Especificacion", "ESP-0037")]
    [Trait("Tecnica", "T-002")]
    public void CrearConTelefonoVacioLoPasaComoNulo()
    {
        var repositorio = new RepositorioDeProveedoresFalso();

        Servicio(repositorio).Crear("20100070970", "Drogueria Andina SAC", "   ");

        Assert.NotNull(repositorio.UltimoCreado);
        Assert.Null(repositorio.UltimoCreado.Telefono);
    }

    [Fact]
    [Trait("Especificacion", "ESP-0037")]
    [Trait("Tecnica", "T-002")]
    public void CrearConTelefonoNuloLoPasaComoNulo()
    {
        var repositorio = new RepositorioDeProveedoresFalso();

        Servicio(repositorio).Crear("20100070970", "Drogueria Andina SAC", null);

        Assert.NotNull(repositorio.UltimoCreado);
        Assert.Null(repositorio.UltimoCreado.Telefono);
    }

    [Theory]
    [Trait("Especificacion", "ESP-0037")]
    [Trait("Tecnica", "T-002")]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("1234567890")]
    [InlineData("123456789012")]
    [InlineData("2010007097A")]
    [InlineData("2010007097 ")]
    [InlineData(" 2010007097")]
    public void CrearConRucInvalidoLanzaReglaDeNegocio(string? ruc)
    {
        var repositorio = new RepositorioDeProveedoresFalso();

        var excepcion = Assert.Throws<ReglaDeNegocioException>(
            () => Servicio(repositorio).Crear(ruc, "Drogueria SAC", null));

        Assert.Equal(MensajesProveedores.RucInvalido, excepcion.Message);
        Assert.Null(repositorio.UltimoCreado);
    }

    [Fact]
    [Trait("Especificacion", "ESP-0037")]
    [Trait("Tecnica", "T-001")]
    public void CrearConRucConCerosALaIzquierdaEsValido()
    {
        var repositorio = new RepositorioDeProveedoresFalso();

        Servicio(repositorio).Crear("00100070970", "Drogueria SAC", null);

        Assert.NotNull(repositorio.UltimoCreado);
        Assert.Equal("00100070970", repositorio.UltimoCreado.Ruc);
    }

    [Fact]
    [Trait("Especificacion", "ESP-0037")]
    [Trait("Tecnica", "T-001")]
    public void CrearConRucDuplicadoLanzaReglaDeNegocio()
    {
        var repositorio = new RepositorioDeProveedoresFalso { RucExistente = "20100070970" };

        var excepcion = Assert.Throws<ReglaDeNegocioException>(
            () => Servicio(repositorio).Crear("20100070970", "Drogueria SAC", null));

        Assert.Equal(MensajesProveedores.RucRepetido, excepcion.Message);
        Assert.Null(repositorio.UltimoCreado);
    }

    [Theory]
    [Trait("Especificacion", "ESP-0037")]
    [Trait("Tecnica", "T-002")]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void CrearConRazonSocialVaciaLanzaReglaDeNegocio(string? razonSocial)
    {
        var repositorio = new RepositorioDeProveedoresFalso();

        var excepcion = Assert.Throws<ReglaDeNegocioException>(
            () => Servicio(repositorio).Crear("20100070970", razonSocial, null));

        Assert.Equal(MensajesProveedores.RazonSocialVacia, excepcion.Message);
        Assert.Null(repositorio.UltimoCreado);
    }

    [Fact]
    [Trait("Especificacion", "ESP-0037")]
    [Trait("Tecnica", "T-002")]
    public void CrearConRazonSocialDemasiadoLargaLanzaReglaDeNegocio()
    {
        var repositorio = new RepositorioDeProveedoresFalso();
        var razonSocialLarga = new string('A', 151);

        var excepcion = Assert.Throws<ReglaDeNegocioException>(
            () => Servicio(repositorio).Crear("20100070970", razonSocialLarga, null));

        Assert.Equal(MensajesProveedores.RazonSocialDemasiadoLarga, excepcion.Message);
        Assert.Null(repositorio.UltimoCreado);
    }

    [Fact]
    [Trait("Especificacion", "ESP-0037")]
    [Trait("Tecnica", "T-002")]
    public void CrearConRazonSocialRecortadaDentroDelLimiteEsValido()
    {
        var repositorio = new RepositorioDeProveedoresFalso();
        var razonSocial = "  " + new string('A', 150) + "  ";

        Servicio(repositorio).Crear("20100070970", razonSocial, null);

        Assert.NotNull(repositorio.UltimoCreado);
        Assert.Equal(new string('A', 150), repositorio.UltimoCreado.RazonSocial);
    }

    [Fact]
    [Trait("Especificacion", "ESP-0037")]
    [Trait("Tecnica", "T-002")]
    public void CrearConTelefonoDemasiadoLargoLanzaReglaDeNegocio()
    {
        var repositorio = new RepositorioDeProveedoresFalso();

        var excepcion = Assert.Throws<ReglaDeNegocioException>(
            () => Servicio(repositorio).Crear("20100070970", "Drogueria SAC", new string('9', 16)));

        Assert.Equal(MensajesProveedores.TelefonoDemasiadoLargo, excepcion.Message);
        Assert.Null(repositorio.UltimoCreado);
    }

    [Fact]
    [Trait("Especificacion", "ESP-0037")]
    [Trait("Tecnica", "T-002")]
    public void CrearComoTecnicoLanzaAccesoDenegado()
    {
        var repositorio = new RepositorioDeProveedoresFalso();

        Assert.Throws<AccesoDenegadoException>(
            () => Servicio(repositorio, Rol.Tecnico).Crear("20100070970", "Drogueria SAC", null));

        Assert.Null(repositorio.UltimoCreado);
    }

    // =============================
    // ESP-0039: Actualizar
    // =============================

    [Fact]
    [Trait("Especificacion", "ESP-0039")]
    [Trait("Tecnica", "T-001")]
    public void ActualizarConDatosValidosDelegaAlRepositorio()
    {
        var repositorio = new RepositorioDeProveedoresFalso();

        Servicio(repositorio).Actualizar(1, "20100070970", "Drogueria Andina SAC", "054223344", "Activo");

        Assert.NotNull(repositorio.UltimaActualizacion);
        Assert.Equal(1, repositorio.UltimaActualizacion.Value.IdProveedor);
        Assert.Equal("20100070970", repositorio.UltimaActualizacion.Value.Proveedor.Ruc);
    }

    [Fact]
    [Trait("Especificacion", "ESP-0039")]
    [Trait("Tecnica", "T-002")]
    public void ActualizarConIdInvalidoLanzaReglaDeNegocio()
    {
        var repositorio = new RepositorioDeProveedoresFalso();

        var excepcion = Assert.Throws<ReglaDeNegocioException>(
            () => Servicio(repositorio).Actualizar(0, "20100070970", "Drogueria SAC", null, "Activo"));

        Assert.Equal("Seleccione un proveedor para actualizar.", excepcion.Message);
        Assert.Null(repositorio.UltimaActualizacion);
    }

    [Fact]
    [Trait("Especificacion", "ESP-0039")]
    [Trait("Tecnica", "T-002")]
    public void ActualizarComoTecnicoLanzaAccesoDenegado()
    {
        var repositorio = new RepositorioDeProveedoresFalso();

        Assert.Throws<AccesoDenegadoException>(
            () => Servicio(repositorio, Rol.Tecnico)
                .Actualizar(1, "20100070970", "Drogueria SAC", null, "Activo"));

        Assert.Null(repositorio.UltimaActualizacion);
    }

    // =============================
    // ESP-0040: VerificarBaja y DarDeBaja
    // =============================

    [Fact]
    [Trait("Especificacion", "ESP-0040")]
    [Trait("Tecnica", "T-001")]
    public void VerificarBajaSinOrdenesPendientesNoLanzaExcepcion()
    {
        var repositorio = new RepositorioDeProveedoresFalso { OrdenesDevPendientes = false };

        Servicio(repositorio).VerificarBaja(1);
    }

    [Fact]
    [Trait("Especificacion", "ESP-0040")]
    [Trait("Tecnica", "T-001")]
    public void VerificarBajaConOrdenesPendientesLanzaReglaDeNegocio()
    {
        var repositorio = new RepositorioDeProveedoresFalso { OrdenesDevPendientes = true };

        var excepcion = Assert.Throws<ReglaDeNegocioException>(
            () => Servicio(repositorio).VerificarBaja(1));

        Assert.Equal(MensajesProveedores.OrdenesPendientes, excepcion.Message);
    }

    [Fact]
    [Trait("Especificacion", "ESP-0040")]
    [Trait("Tecnica", "T-002")]
    public void VerificarBajaConIdInvalidoLanzaReglaDeNegocio()
    {
        var repositorio = new RepositorioDeProveedoresFalso();

        Assert.Throws<ReglaDeNegocioException>(
            () => Servicio(repositorio).VerificarBaja(0));
    }

    [Fact]
    [Trait("Especificacion", "ESP-0040")]
    [Trait("Tecnica", "T-002")]
    public void VerificarBajaComoTecnicoLanzaAccesoDenegado()
    {
        var repositorio = new RepositorioDeProveedoresFalso();

        Assert.Throws<AccesoDenegadoException>(
            () => Servicio(repositorio, Rol.Tecnico).VerificarBaja(1));
    }

    [Fact]
    [Trait("Especificacion", "ESP-0040")]
    [Trait("Tecnica", "T-001")]
    public void DarDeBajaDelegaAlRepositorio()
    {
        var repositorio = new RepositorioDeProveedoresFalso
        {
            ResultadoBajaADevolver = ResultadoBajaProveedor.Eliminado
        };

        var resultado = Servicio(repositorio).DarDeBaja(1);

        Assert.Equal(ResultadoBajaProveedor.Eliminado, resultado);
        Assert.Equal(1, repositorio.UltimaBaja);
    }

    [Fact]
    [Trait("Especificacion", "ESP-0040")]
    [Trait("Tecnica", "T-001")]
    public void DarDeBajaConReferenciaDevuelveDesactivado()
    {
        var repositorio = new RepositorioDeProveedoresFalso
        {
            ResultadoBajaADevolver = ResultadoBajaProveedor.Desactivado
        };

        var resultado = Servicio(repositorio).DarDeBaja(1);

        Assert.Equal(ResultadoBajaProveedor.Desactivado, resultado);
    }

    [Fact]
    [Trait("Especificacion", "ESP-0040")]
    [Trait("Tecnica", "T-002")]
    public void DarDeBajaComoTecnicoLanzaAccesoDenegado()
    {
        var repositorio = new RepositorioDeProveedoresFalso();

        Assert.Throws<AccesoDenegadoException>(
            () => Servicio(repositorio, Rol.Tecnico).DarDeBaja(1));

        Assert.Null(repositorio.UltimaBaja);
    }
}
