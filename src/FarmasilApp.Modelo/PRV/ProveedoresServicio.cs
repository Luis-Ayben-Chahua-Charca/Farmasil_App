using System.Text.RegularExpressions;
using FarmasilApp.Modelo.Comun;
using FarmasilApp.Modelo.Seguridad;

namespace FarmasilApp.Modelo.PRV;

/// <summary>
/// Servicio de aplicación de Proveedores.
/// Implementa ESP-0037, ESP-0038, ESP-0039 y ESP-0040.
/// </summary>
/// <remarks>
/// <para><b>Dónde viven las reglas.</b> Toda regla de la especificación está en este servicio, no en la VistaModelo ni en
/// el SQL. El servicio recibe por inyección el repositorio y la sesión, así que cada regla se prueba con
/// dobles en memoria.</para>
/// <para><b>Cómo se informa un fallo.</b>
/// <list type="bullet">
/// <item>Cada operación empieza con <see cref="ISesion.ExigirRol"/>: una sesión 'Tecnico' recibe
/// <see cref="AccesoDenegadoException"/> (RNF-0006, condición 3; DEC-069).</item>
/// <item>Una validación o una guarda fallida lanza <see cref="ReglaDeNegocioException"/> con el texto de
/// <see cref="MensajesProveedores"/>, antes de escribir nada (DEC-072).</item>
/// <item>Cualquier otra excepción es un error técnico del repositorio.</item>
/// </list></para>
/// </remarks>
public sealed class ProveedoresServicio
{
    /// <summary>Longitud de <c>TBL_PROVEEDORES.razon_social</c>, <c>NVARCHAR(150)</c>.</summary>
    public const int LongitudMaximaRazonSocial = 150;

    /// <summary>Longitud de <c>TBL_PROVEEDORES.telefono</c>, <c>NVARCHAR(15)</c>.</summary>
    public const int LongitudMaximaTelefono = 15;

    private static readonly Regex PatronRuc = new(@"^[0-9]{11}$", RegexOptions.Compiled);

    private readonly IProveedoresRepositorio _repositorio;
    private readonly ISesion _sesion;

    public ProveedoresServicio(IProveedoresRepositorio repositorio, ISesion sesion)
    {
        _repositorio = repositorio
            ?? throw new ArgumentNullException(nameof(repositorio));

        _sesion = sesion
            ?? throw new ArgumentNullException(nameof(sesion));
    }

    /// <summary>
    /// ESP-0038. Todo el módulo es solo para el Administrador, incluyendo la consulta (DEC-075).
    /// </summary>
    public IReadOnlyList<ProveedorConsulta> Consultar(
        string? busqueda,
        string? estado)
    {
        _sesion.ExigirRol(Rol.Administrador);

        var busquedaNormalizada = Normalizar(busqueda);
        var estadoNormalizado = Normalizar(estado);

        if (estadoNormalizado is not null
            && estadoNormalizado != "Activo"
            && estadoNormalizado != "Inactivo")
        {
            throw new ReglaDeNegocioException(
                "El estado debe ser Activo o Inactivo.");
        }

        return _repositorio.Consultar(busquedaNormalizada, estadoNormalizado);
    }

    /// <summary>
    /// ESP-0037. Valida en el orden de la especificación y devuelve el id del proveedor creado.
    /// </summary>
    public int Crear(
        string? ruc,
        string? razonSocial,
        string? telefono)
    {
        _sesion.ExigirRol(Rol.Administrador);

        var proveedor = ValidarProveedor(ruc, razonSocial, telefono, excluirIdProveedor: null);

        return _repositorio.Crear(proveedor);
    }

    /// <summary>
    /// ESP-0039. Actualiza los datos editables del proveedor.
    /// </summary>
    public void Actualizar(
        int idProveedor,
        string? ruc,
        string? razonSocial,
        string? telefono)
    {
        _sesion.ExigirRol(Rol.Administrador);

        if (idProveedor <= 0)
        {
            throw new ReglaDeNegocioException(
                "Seleccione un proveedor para actualizar.");
        }

        var proveedor = ValidarProveedor(ruc, razonSocial, telefono, excluirIdProveedor: idProveedor);

        _repositorio.Actualizar(idProveedor, proveedor);
    }

    /// <summary>
    /// ESP-0040, antes del modal: rechaza la baja si el proveedor tiene órdenes de devolución pendientes.
    /// La VistaModelo la llama antes de pedir la confirmación.
    /// </summary>
    public void VerificarBaja(int idProveedor)
    {
        _sesion.ExigirRol(Rol.Administrador);

        if (idProveedor <= 0)
        {
            throw new ReglaDeNegocioException(
                "Seleccione un proveedor para dar de baja.");
        }

        if (_repositorio.TieneOrdenesPendientes(idProveedor))
        {
            throw new ReglaDeNegocioException(
                MensajesProveedores.OrdenesPendientes);
        }
    }

    /// <summary>
    /// ESP-0040, después de la confirmación. Delega la baja condicional en el repositorio.
    /// </summary>
    public ResultadoBajaProveedor DarDeBaja(int idProveedor)
    {
        _sesion.ExigirRol(Rol.Administrador);

        if (idProveedor <= 0)
        {
            throw new ReglaDeNegocioException(
                "Seleccione un proveedor para dar de baja.");
        }

        return _repositorio.DarDeBaja(idProveedor);
    }

    private ProveedorEscritura ValidarProveedor(
        string? ruc,
        string? razonSocial,
        string? telefono,
        int? excluirIdProveedor)
    {
        // RUC: exactamente 11 dígitos, sin recortar (los espacios causan rechazo).
        if (ruc is null || !PatronRuc.IsMatch(ruc))
        {
            throw new ReglaDeNegocioException(MensajesProveedores.RucInvalido);
        }

        // Unicidad del RUC (antes de llamar al repositorio como segunda línea de defensa).
        if (_repositorio.ExisteRuc(ruc, excluirIdProveedor))
        {
            throw new ReglaDeNegocioException(MensajesProveedores.RucRepetido);
        }

        // Razón social: obligatoria, recortada, máximo 150 caracteres.
        if (string.IsNullOrWhiteSpace(razonSocial))
        {
            throw new ReglaDeNegocioException(MensajesProveedores.RazonSocialVacia);
        }

        var razonSocialNormalizada = razonSocial.Trim();

        if (razonSocialNormalizada.Length > LongitudMaximaRazonSocial)
        {
            throw new ReglaDeNegocioException(MensajesProveedores.RazonSocialDemasiadoLarga);
        }

        // Teléfono: opcional, vacío → null, máximo 15 caracteres.
        var telefonoNormalizado = string.IsNullOrWhiteSpace(telefono) ? null : telefono.Trim();

        if (telefonoNormalizado is not null && telefonoNormalizado.Length > LongitudMaximaTelefono)
        {
            throw new ReglaDeNegocioException(MensajesProveedores.TelefonoDemasiadoLargo);
        }

        return new ProveedorEscritura
        {
            Ruc = ruc,
            RazonSocial = razonSocialNormalizada,
            Telefono = telefonoNormalizado,
        };
    }

    private static string? Normalizar(string? valor)
    {
        return string.IsNullOrWhiteSpace(valor)
            ? null
            : valor.Trim();
    }
}
