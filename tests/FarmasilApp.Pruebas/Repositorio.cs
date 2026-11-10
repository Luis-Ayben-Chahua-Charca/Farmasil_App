namespace FarmasilApp.Pruebas;

/// <summary>Ubica los archivos del repositorio para las pruebas que leen proyectos, configuración o documentos.</summary>
internal static class Repositorio
{
    /// <summary>Carpeta raíz del repositorio: la primera que contiene FarmasilApp.slnx, subiendo desde la carpeta de las pruebas.</summary>
    public static string Raiz()
    {
        var directorio = new DirectoryInfo(AppContext.BaseDirectory);
        while (directorio is not null && !File.Exists(Path.Combine(directorio.FullName, "FarmasilApp.slnx")))
        {
            directorio = directorio.Parent;
        }

        return directorio?.FullName
            ?? throw new InvalidOperationException("No se encontró FarmasilApp.slnx subiendo desde " + AppContext.BaseDirectory);
    }

    public static string Ruta(params string[] partes) => Path.Combine(new[] { Raiz() }.Concat(partes).ToArray());
}
