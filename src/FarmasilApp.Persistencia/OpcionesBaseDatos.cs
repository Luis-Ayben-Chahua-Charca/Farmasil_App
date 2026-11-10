namespace FarmasilApp.Persistencia;

/// <summary>
/// Datos para llegar a DB_FARMASIL. Los repositorios y la unidad de trabajo reciben esta clase por inyección
/// para abrir sus conexiones, de modo que ninguno lea la configuración por su cuenta.
/// </summary>
public sealed class OpcionesBaseDatos
{
    /// <summary>Nombre de la cadena en la sección <c>ConnectionStrings</c> de <c>appsettings.json</c>.</summary>
    public const string NombreCadena = "DB_FARMASIL";

    public OpcionesBaseDatos(string cadenaConexion)
    {
        if (string.IsNullOrWhiteSpace(cadenaConexion))
        {
            throw new ArgumentException("La cadena de conexión no puede estar vacía.", nameof(cadenaConexion));
        }

        CadenaConexion = cadenaConexion;
    }

    public string CadenaConexion { get; }
}
