namespace FarmasilApp.Modelo.Seguridad;

/// <summary>
/// Genera y verifica el hash con sal de una contraseña (RNF-0006, condición 5; DEC-014). Lo usa Usuarios al
/// crear y actualizar cuentas, y lo usará el inicio de sesión (#88). Se inyecta para que las pruebas del servicio
/// puedan sustituirlo.
/// </summary>
public interface IHashContrasenas
{
    /// <summary>Devuelve la cadena que se guarda en <c>TBL_USUARIOS.contrasena</c>, con una sal nueva.</summary>
    string Generar(string contrasena);

    /// <summary>
    /// Indica si <paramref name="contrasena"/> corresponde a la cadena <paramref name="almacenado"/>.
    /// Una cadena con formato inválido no corresponde a ninguna contraseña.
    /// </summary>
    bool Verificar(string contrasena, string almacenado);
}
