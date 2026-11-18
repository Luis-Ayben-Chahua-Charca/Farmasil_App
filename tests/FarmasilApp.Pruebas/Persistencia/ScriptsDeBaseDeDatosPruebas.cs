using System.Text.RegularExpressions;

namespace FarmasilApp.Pruebas.Persistencia;

/// <summary>
/// Protege la carga de la base desde <c>database/recrear.ps1</c> (issue #178). La CI no tiene SQL Server, así que esta
/// prueba lee los scripts: comprueba que fijan <c>QUOTED_IDENTIFIER ON</c> y no dependen del cliente que los ejecute.
/// </summary>
/// <remarks>
/// El <c>sqlcmd</c> antiguo (ODBC) abre la sesión con <c>QUOTED_IDENTIFIER OFF</c>. Sin la opción, un <c>INSERT</c> en una
/// tabla con índice filtrado falla con el error 1934 y <c>recrear.ps1</c> deja la base sin datos. El <c>sqlcmd</c> de
/// <c>winget</c> la abre en <c>ON</c>, por lo que el problema solo aparece en algunos equipos.
/// </remarks>
public partial class ScriptsDeBaseDeDatosPruebas
{
    private static string Leer(string archivo) => File.ReadAllText(Repositorio.Ruta("database", archivo));

    /// <summary>Posición de la primera línea que no es un comentario y coincide con el patrón, o -1.</summary>
    private static int Posicion(Regex patron, string script)
    {
        var coincidencia = patron.Match(script);
        return coincidencia.Success ? coincidencia.Index : -1;
    }

    [Fact]
    [Trait("Especificacion", "RNF-0009")]
    [Trait("Tecnica", "T-018")]
    public void ElScriptDeDatosFijaQuotedIdentifierOnAntesDelPrimerInsert()
    {
        var script = Leer("002_datos_prueba.sql");

        var opcion = Posicion(QuotedIdentifierOn(), script);
        var primerInsert = Posicion(PrimerInsert(), script);

        Assert.True(opcion >= 0, "002_datos_prueba.sql no fija SET QUOTED_IDENTIFIER ON.");
        Assert.True(primerInsert >= 0, "002_datos_prueba.sql no tiene ningún INSERT.");
        Assert.True(opcion < primerInsert, "SET QUOTED_IDENTIFIER ON debe ir antes del primer INSERT.");
    }

    [Fact]
    [Trait("Especificacion", "RNF-0009")]
    [Trait("Tecnica", "T-018")]
    public void ElScriptDelEsquemaFijaQuotedIdentifierOnAntesDeCrearLasTablas()
    {
        var script = Leer("001_esquema.sql");

        var opcion = Posicion(QuotedIdentifierOn(), script);
        var primeraTabla = Posicion(PrimeraTabla(), script);

        Assert.True(opcion >= 0, "001_esquema.sql no fija SET QUOTED_IDENTIFIER ON.");
        Assert.True(primeraTabla >= 0, "001_esquema.sql no crea ninguna tabla.");
        Assert.True(opcion < primeraTabla, "SET QUOTED_IDENTIFIER ON debe ir antes del primer CREATE TABLE.");
    }

    [Fact]
    [Trait("Especificacion", "RNF-0009")]
    [Trait("Tecnica", "T-018")]
    public void CadaLlamadaASqlcmdDeRecrearFijaQuotedIdentifierOn()
    {
        var llamadas = LlamadasASqlcmd(Leer("recrear.ps1"));

        // Crear la base, ejecutar los dos scripts y verificar: si cambia el número, hay que revisar esta prueba.
        Assert.Equal(4, llamadas.Count);
        Assert.All(
            llamadas,
            llamada => Assert.True(
                llamada.Contains("-I", StringComparer.Ordinal),
                $"Falta -I en la llamada a sqlcmd: {string.Join(' ', llamada)}"));
    }

    /// <summary>
    /// Cada llamada a <c>sqlcmd</c> de un script de PowerShell: la línea que empieza con <c>sqlcmd</c> y las siguientes
    /// mientras la anterior termine con el carácter de continuación (el acento grave). Devuelve sus argumentos.
    /// </summary>
    private static List<string[]> LlamadasASqlcmd(string script)
    {
        var llamadas = new List<string[]>();
        var lineas = script.Split('\n').Select(linea => linea.TrimEnd('\r')).ToArray();

        for (var i = 0; i < lineas.Length; i++)
        {
            if (!lineas[i].StartsWith("sqlcmd", StringComparison.Ordinal))
            {
                continue;
            }

            var texto = new List<string>();
            var actual = i;
            while (true)
            {
                var continua = lineas[actual].EndsWith('`');
                texto.Add(lineas[actual].TrimEnd('`'));
                if (!continua || actual + 1 >= lineas.Length)
                {
                    break;
                }

                actual++;
            }

            llamadas.Add(string.Join(' ', texto).Split(' ', StringSplitOptions.RemoveEmptyEntries).Skip(1).ToArray());
            i = actual;
        }

        return llamadas;
    }

    // Las líneas de comentario (-- o dentro de /* */ sin mayúsculas iniciales) no coinciden: el patrón exige el
    // comando al inicio de la línea.
    [GeneratedRegex(@"^\s*SET\s+QUOTED_IDENTIFIER\s+ON\s*;", RegexOptions.IgnoreCase | RegexOptions.Multiline)]
    private static partial Regex QuotedIdentifierOn();

    [GeneratedRegex(@"^\s*INSERT\s+INTO\b", RegexOptions.IgnoreCase | RegexOptions.Multiline)]
    private static partial Regex PrimerInsert();

    [GeneratedRegex(@"^\s*CREATE\s+TABLE\b", RegexOptions.IgnoreCase | RegexOptions.Multiline)]
    private static partial Regex PrimeraTabla();
}
