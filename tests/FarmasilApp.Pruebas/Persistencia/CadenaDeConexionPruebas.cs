using System.Text.Json;

namespace FarmasilApp.Pruebas.Persistencia;

/// <summary>
/// Protege la cadena de conexión provisional (PEN-035): la que versiona el repositorio usa la cuenta de Windows,
/// no guarda credenciales y está documentada en el README tal como la lee la aplicación.
/// </summary>
public class CadenaDeConexionPruebas
{
    private static string CadenaVersionada()
    {
        var ruta = Repositorio.Ruta("src", "FarmasilApp.Vista", "appsettings.json");
        using var documento = JsonDocument.Parse(File.ReadAllText(ruta));
        return documento.RootElement
            .GetProperty("ConnectionStrings")
            .GetProperty("DB_FARMASIL")
            .GetString()
            ?? throw new InvalidOperationException("La cadena DB_FARMASIL está vacía en appsettings.json.");
    }

    [Fact]
    public void ApuntaALaBaseDB_FARMASILDeLaInstanciaLocalSQLEXPRESS()
    {
        var cadena = CadenaVersionada();

        Assert.Contains("Database=DB_FARMASIL", cadena);
        Assert.Contains(@".\SQLEXPRESS", cadena);
    }

    [Fact]
    public void AutenticaConLaCuentaDeWindows()
    {
        Assert.Contains("Integrated Security=true", CadenaVersionada(), StringComparison.OrdinalIgnoreCase);
    }

    [Theory]
    [InlineData("password")]
    [InlineData("pwd")]
    [InlineData("user id")]
    [InlineData("uid")]
    public void NoGuardaCredenciales(string clave)
    {
        Assert.DoesNotContain(clave, CadenaVersionada(), StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void ElReadmeDocumentaLaMismaCadenaQueLeeLaAplicacion()
    {
        var readme = File.ReadAllText(Repositorio.Ruta("README.md"));

        Assert.Contains(CadenaVersionada(), readme);
    }
}
