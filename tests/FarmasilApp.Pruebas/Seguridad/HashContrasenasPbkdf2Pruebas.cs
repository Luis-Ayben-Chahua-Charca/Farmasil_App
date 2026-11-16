using System.Text.RegularExpressions;
using FarmasilApp.Modelo.Seguridad;

namespace FarmasilApp.Pruebas.Seguridad;

/// <summary>
/// Pruebas del hash real de contraseñas (RNF-0006, condición 5; DEC-014; formato de DEC-041).
/// </summary>
public partial class HashContrasenasPbkdf2Pruebas
{
    private static readonly HashContrasenasPbkdf2 Hash = new();

    [Fact]
    [Trait("Especificacion", "ESP-0033")]
    [Trait("Tecnica", "T-012")]
    public void VerificarAceptaLosHashesDelScriptDeDatosDePrueba()
    {
        // La contraseña es un dato de prueba: se lee del comentario del script, no se copia al código.
        var script = File.ReadAllText(Repositorio.Ruta("database", "002_datos_prueba.sql"));
        var contrasena = ContrasenaDePrueba().Match(script).Groups[1].Value;
        var hashes = HashDelScript().Matches(script).Select(m => m.Groups[1].Value).ToList();

        Assert.False(string.IsNullOrEmpty(contrasena), "El script no documenta la contraseña de prueba.");
        Assert.Equal(2, hashes.Count);
        Assert.All(hashes, almacenado => Assert.True(Hash.Verificar(contrasena, almacenado)));
        Assert.All(hashes, almacenado => Assert.False(Hash.Verificar(contrasena + "x", almacenado)));
    }

    [Fact]
    [Trait("Especificacion", "ESP-0033")]
    [Trait("Tecnica", "T-012")]
    public void DosHashesDeLaMismaContrasenaSonDistintos()
    {
        var primero = Hash.Generar("Clave-123");
        var segundo = Hash.Generar("Clave-123");

        Assert.NotEqual(primero, segundo);
        Assert.True(Hash.Verificar("Clave-123", primero));
        Assert.True(Hash.Verificar("Clave-123", segundo));
    }

    [Fact]
    [Trait("Especificacion", "ESP-0033")]
    [Trait("Tecnica", "T-004")]
    public void ElHashTieneElFormatoDeDec041YCabeEnLaColumna()
    {
        var almacenado = Hash.Generar("Clave-123");
        var partes = almacenado.Split('$');

        Assert.Equal(4, partes.Length);
        Assert.Equal("PBKDF2-SHA256", partes[0]);
        Assert.True(int.Parse(partes[1]) >= 100_000);
        Assert.Equal(HashContrasenasPbkdf2.BytesSal, Convert.FromBase64String(partes[2]).Length);
        Assert.Equal(HashContrasenasPbkdf2.BytesHash, Convert.FromBase64String(partes[3]).Length);
        Assert.DoesNotContain("Clave-123", almacenado);
        Assert.True(almacenado.Length <= 255, "No cabe en TBL_USUARIOS.contrasena, NVARCHAR(255).");
    }

    [GeneratedRegex(@"Contraseña de prueba para ambas cuentas:\s*(\S+)")]
    private static partial Regex ContrasenaDePrueba();

    [GeneratedRegex(@"N'(PBKDF2-SHA256\$[^']+)'")]
    private static partial Regex HashDelScript();
}
