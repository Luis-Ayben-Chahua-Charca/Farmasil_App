namespace FarmasilApp.Modelo.USR;

/// <summary>
/// Resultado de la baja condicional (DEC-009, DEC-029): lo decide la base de datos, no el módulo.
/// </summary>
public enum ResultadoBaja
{
    /// <summary>Ninguna clave foránea referenciaba la cuenta y se borró físicamente.</summary>
    Eliminada,

    /// <summary>Una clave foránea impidió el borrado (error 547) y se aplicó <c>estado = 'Inactivo'</c>.</summary>
    Desactivada,
}
