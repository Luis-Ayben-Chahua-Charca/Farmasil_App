namespace FarmasilApp.Modelo.INV;

/// <summary>
/// Resultado de la baja condicional de un producto
/// según DEC-009 y DEC-029.
/// </summary>
public enum ResultadoBajaProducto
{
    /// <summary>
    /// El producto no tenía referencias y fue eliminado físicamente.
    /// </summary>
    Eliminado,

    /// <summary>
    /// Una referencia impidió el borrado y el producto quedó Descontinuado.
    /// </summary>
    Descontinuado,
}
