using FarmasilApp.Modelo.Comun;
using FarmasilApp.Modelo.Seguridad;

namespace FarmasilApp.Modelo.PAG;

/// <summary>
/// Servicio de aplicación para ESP-0017 a ESP-0020.
/// </summary>
public sealed class MetodosPagoServicio : IPagos
{
    public const int LongitudMaximaNombre = 50;

    private readonly IMetodosPagoRepositorio _repositorio;
    private readonly ISesion _sesion;

    public MetodosPagoServicio(IMetodosPagoRepositorio repositorio, ISesion sesion)
    {
        _repositorio = repositorio
            ?? throw new ArgumentNullException(nameof(repositorio));
        _sesion = sesion
            ?? throw new ArgumentNullException(nameof(sesion));
    }

    public IReadOnlyList<MetodoPagoConsulta> Consultar(
        string? nombreMetodo,
        string? estado)
    {
        ExigirRolDeConsulta();

        var nombreNormalizado = Normalizar(nombreMetodo);
        var estadoNormalizado = Normalizar(estado);

        if (estadoNormalizado is not null
            && estadoNormalizado != "Activo"
            && estadoNormalizado != "Inactivo")
        {
            throw new ReglaDeNegocioException(MensajesMetodosPago.EstadoInvalido);
        }

        return _repositorio.Consultar(nombreNormalizado, estadoNormalizado);
    }

    public int Crear(string? nombreMetodo, string? estado)
    {
        _sesion.ExigirRol(Rol.Administrador);

        var nombre = ValidarNombre(nombreMetodo, excluirIdMetodoPago: null);
        var estadoValidado = ValidarEstado(estado);

        return _repositorio.Crear(nombre, estadoValidado);
    }

    public void Actualizar(int idMetodoPago, string? nombreMetodo, string? estado)
    {
        _sesion.ExigirRol(Rol.Administrador);

        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(idMetodoPago);

        var nombre = ValidarNombre(nombreMetodo, excluirIdMetodoPago: idMetodoPago);
        var estadoValidado = ValidarEstado(estado);

        _repositorio.Actualizar(idMetodoPago, nombre, estadoValidado);
    }

    public ResultadoBajaMetodoPago DarDeBaja(int idMetodoPago)
    {
        _sesion.ExigirRol(Rol.Administrador);

        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(idMetodoPago);

        return _repositorio.DarDeBaja(idMetodoPago);
    }

    public IReadOnlyList<MetodoPagoDto> MetodosActivos()
    {
        ExigirRolDeConsulta();

        return _repositorio.Consultar(null, "Activo")
            .Where(metodo => metodo.Estado == "Activo")
            .OrderBy(metodo => metodo.IdMetodoPago)
            .Select(metodo => new MetodoPagoDto
            {
                IdMetodoPago = metodo.IdMetodoPago,
                Nombre = metodo.NombreMetodo,
            })
            .ToArray();
    }

    private string ValidarNombre(string? nombreMetodo, int? excluirIdMetodoPago)
    {
        if (string.IsNullOrWhiteSpace(nombreMetodo))
        {
            throw new ReglaDeNegocioException(MensajesMetodosPago.NombreVacio);
        }

        var nombre = nombreMetodo.Trim();

        if (nombre.Length > LongitudMaximaNombre)
        {
            throw new ReglaDeNegocioException(MensajesMetodosPago.NombreDemasiadoLargo);
        }

        if (_repositorio.ExisteNombre(nombre, excluirIdMetodoPago))
        {
            throw new ReglaDeNegocioException(MensajesMetodosPago.NombreRepetido);
        }

        return nombre;
    }

    private static string ValidarEstado(string? estado)
    {
        if (estado != "Activo" && estado != "Inactivo")
        {
            throw new ReglaDeNegocioException(MensajesMetodosPago.EstadoInvalido);
        }

        return estado;
    }

    private void ExigirRolDeConsulta()
    {
        var rol = _sesion.UsuarioActual.Rol;
        if (rol == Rol.Administrador)
        {
            _sesion.ExigirRol(Rol.Administrador);
            return;
        }

        _sesion.ExigirRol(Rol.Tecnico);
    }

    private static string? Normalizar(string? valor)
    {
        return string.IsNullOrWhiteSpace(valor)
            ? null
            : valor.Trim();
    }
}
