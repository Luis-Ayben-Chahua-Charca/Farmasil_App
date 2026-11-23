namespace FarmasilApp.Modelo.PRV;

/// <summary>
/// Textos que muestra el módulo de Proveedores. Son constantes públicas para que la VistaModelo y las pruebas usen
/// exactamente el mismo texto (DEC-072).
/// </summary>
/// <remarks>
/// Los textos con referencia a una ESP son literales de la especificación y no se cambian sin cambiarla.
/// Los demás no los fija ninguna especificación y se listan en la descripción del PR del #95 para su aprobación.
/// </remarks>
public static class MensajesProveedores
{
    /// <summary>Título de los mensajes de la pantalla.</summary>
    public const string Titulo = "Gestión de Proveedores";

    /// <summary>ESP-0037.</summary>
    public const string ProveedorRegistrado = "Proveedor registrado correctamente";

    /// <summary>ESP-0039.</summary>
    public const string ProveedorActualizado = "Proveedor actualizado correctamente";

    /// <summary>ESP-0040: el proveedor tiene órdenes de devolución pendientes de despacho.</summary>
    public const string OrdenesPendientes = "El proveedor tiene órdenes de devolución pendientes de despacho";

    /// <summary>ESP-0040: texto del modal de confirmación de baja.</summary>
    public const string ConfirmarBaja =
        "¿Está seguro de dar de baja este proveedor? Los productos y devoluciones ya registrados no se verán afectados.";

    /// <summary>ESP-0040.</summary>
    public const string ProveedorDadoDeBaja = "Proveedor dado de baja correctamente";

    /// <summary>ESP-0037 y ESP-0039: <c>VALIDAR ruc COINCIDE CON ^[0-9]{11}$</c>. Texto no fijado por la ESP.</summary>
    public const string RucInvalido = "El RUC debe tener exactamente 11 dígitos numéricos.";

    /// <summary>ESP-0037 y ESP-0039: <c>VALIDAR NO EXISTE(...)</c>. Texto no fijado por la ESP.</summary>
    public const string RucRepetido = "Ya existe un proveedor con ese RUC.";

    /// <summary>ESP-0037 y ESP-0039: <c>VALIDAR razonSocial <> VACIO</c>. Texto no fijado por la ESP.</summary>
    public const string RazonSocialVacia = "Ingrese la razón social del proveedor.";

    /// <summary>ESP-0037 y ESP-0039: <c>VALIDAR LONGITUD(razonSocial) <= 150</c>. Texto no fijado por la ESP.</summary>
    public const string RazonSocialDemasiadoLarga = "La razón social no puede superar los 150 caracteres.";

    /// <summary>ESP-0037 y ESP-0039: <c>VALIDAR LONGITUD(telefono) <= 15</c>. Texto no fijado por la ESP.</summary>
    public const string TelefonoDemasiadoLargo = "El teléfono no puede superar los 15 caracteres.";

    /// <summary>Sesión sin el rol 'Administrador' (RNF-0006, condición 3). Texto no fijado por la ESP.</summary>
    public const string AccesoDenegado = "Su rol no permite gestionar proveedores.";

    /// <summary>Error técnico al consultar. Texto no fijado por la ESP.</summary>
    public const string ErrorConsulta = "No se pudo consultar los proveedores. Inténtelo nuevamente.";

    /// <summary>Error técnico al registrar, actualizar o dar de baja. Texto no fijado por la ESP.</summary>
    public const string ErrorOperacion = "No se pudo completar la operación. Inténtelo nuevamente.";
}
