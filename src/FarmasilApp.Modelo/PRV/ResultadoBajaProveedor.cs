namespace FarmasilApp.Modelo.PRV;

/// <summary>
/// Resultado de la baja condicional de un proveedor
/// según VP-10 y DEC-029.
/// </summary>
public enum ResultadoBajaProveedor
{
    /// <summary>
    /// El proveedor no tenía referencias y fue eliminado físicamente.
    /// </summary>
    Eliminado,

    /// <summary>
    /// Una referencia impidió el borrado y el proveedor quedó Inactivo.
    /// </summary>
    Desactivado,
}
