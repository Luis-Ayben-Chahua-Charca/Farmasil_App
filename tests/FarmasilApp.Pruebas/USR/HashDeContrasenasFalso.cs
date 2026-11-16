using FarmasilApp.Modelo.Seguridad;

namespace FarmasilApp.Pruebas.USR;

/// <summary>
/// Hash de prueba, determinista y rápido: <c>HASH(contraseña)</c>. Permite comprobar que al repositorio llega el hash y
/// no la contraseña, sin el costo de PBKDF2. El hash real se prueba en <c>Seguridad/HashContrasenasPbkdf2Pruebas</c>.
/// </summary>
internal sealed class HashDeContrasenasFalso : IHashContrasenas
{
    public static string HashDe(string contrasena) => $"HASH({contrasena})";

    public string Generar(string contrasena) => HashDe(contrasena);

    public bool Verificar(string contrasena, string almacenado) => almacenado == HashDe(contrasena);
}
