using System.Xml.Linq;

namespace FarmasilApp.Pruebas;

/// <summary>
/// Comprueba que las referencias entre proyectos siguen las capas de VL-04 y los paquetes de VL-01:
/// la Vista solo conoce la VistaModelo (y Persistencia para registrar las dependencias al arrancar),
/// la VistaModelo y la Persistencia solo conocen el Modelo, y el Modelo no conoce a ninguna otra capa.
/// Lee los .csproj de la solución, así que no depende de WPF ni de Windows.
/// </summary>
public class EstructuraDeCapasPruebas
{
    private const string Modelo = "FarmasilApp.Modelo";
    private const string Persistencia = "FarmasilApp.Persistencia";
    private const string VistaModelo = "FarmasilApp.VistaModelo";
    private const string Vista = "FarmasilApp.Vista";

    [Theory]
    [InlineData(Modelo)]
    [InlineData(Persistencia)]
    [InlineData(VistaModelo)]
    [InlineData(Vista)]
    public void CadaCapaTieneSuProyecto(string proyecto)
    {
        Assert.True(File.Exists(RutaCsproj(proyecto)), $"Falta el proyecto {proyecto}.");
    }

    [Fact]
    public void ModeloNoReferenciaNingunaOtraCapa()
    {
        Assert.Empty(ReferenciasDe(Modelo));
    }

    [Fact]
    public void PersistenciaSoloReferenciaAlModelo()
    {
        Assert.Equal(new[] { Modelo }, ReferenciasDe(Persistencia));
    }

    [Fact]
    public void VistaModeloSoloReferenciaAlModelo()
    {
        Assert.Equal(new[] { Modelo }, ReferenciasDe(VistaModelo));
    }

    [Fact]
    public void VistaSoloReferenciaALaVistaModeloYAPersistencia()
    {
        // Persistencia solo se referencia para registrar las dependencias al arrancar (VP-00).
        Assert.Equal(new[] { Persistencia, VistaModelo }, ReferenciasDe(Vista));
    }

    [Fact]
    public void NingunProyectoDeCodigoReferenciaALaVista()
    {
        foreach (var proyecto in new[] { Modelo, Persistencia, VistaModelo })
        {
            Assert.DoesNotContain(Vista, ReferenciasDe(proyecto));
        }
    }

    [Fact]
    public void LaSolucionIncluyeLosCincoProyectos()
    {
        var slnx = XDocument.Load(Path.Combine(RaizDelRepositorio(), "FarmasilApp.slnx"));
        var incluidos = slnx.Descendants("Project")
            .Select(p => Path.GetFileNameWithoutExtension((string?)p.Attribute("Path") ?? string.Empty))
            .OrderBy(n => n, StringComparer.Ordinal)
            .ToArray();

        Assert.Equal(
            new[] { Modelo, Persistencia, "FarmasilApp.Pruebas", Vista, VistaModelo }
                .OrderBy(n => n, StringComparer.Ordinal),
            incluidos);
    }

    private static string[] ReferenciasDe(string proyecto)
    {
        var csproj = XDocument.Load(RutaCsproj(proyecto));
        return csproj.Descendants("ProjectReference")
            .Select(r => Path.GetFileNameWithoutExtension(((string?)r.Attribute("Include") ?? string.Empty).Replace('\\', '/')))
            .OrderBy(n => n, StringComparer.Ordinal)
            .ToArray();
    }

    private static string RutaCsproj(string proyecto) =>
        Path.Combine(RaizDelRepositorio(), "src", proyecto, proyecto + ".csproj");

    private static string RaizDelRepositorio()
    {
        var directorio = new DirectoryInfo(AppContext.BaseDirectory);
        while (directorio is not null && !File.Exists(Path.Combine(directorio.FullName, "FarmasilApp.slnx")))
        {
            directorio = directorio.Parent;
        }

        return directorio?.FullName
            ?? throw new InvalidOperationException("No se encontró FarmasilApp.slnx subiendo desde " + AppContext.BaseDirectory);
    }
}
