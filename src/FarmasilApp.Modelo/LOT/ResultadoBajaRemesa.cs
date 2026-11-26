namespace FarmasilApp.Modelo.LOT;

/// <summary>
/// Resultado de la baja condicional de una remesa
/// según DEC-009 y DEC-029.
/// </summary>
public enum ResultadoBajaRemesa
{
    /// <summary>
    /// La remesa no tenía referencias y fue eliminada físicamente.
    /// </summary>
    Eliminada,

    /// <summary>
    /// Una referencia impidió el borrado y la remesa quedó Agotada
    /// con stock igual a cero.
    /// </summary>
    Agotada,
}
