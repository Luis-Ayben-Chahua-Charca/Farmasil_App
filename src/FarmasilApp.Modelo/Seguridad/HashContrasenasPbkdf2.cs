using System.Globalization;
using System.Security.Cryptography;
using System.Text;

namespace FarmasilApp.Modelo.Seguridad;

/// <summary>
/// Hash de contraseñas con PBKDF2-SHA256 de .NET, sin paquetes externos.
/// </summary>
/// <remarks>
/// <para>
/// Formato de DEC-041: <c>PBKDF2-SHA256$&lt;iteraciones&gt;$&lt;sal Base64&gt;$&lt;hash Base64&gt;</c>. La cadena es
/// autodescriptiva: guarda la sal de cada cuenta y las iteraciones con que se calculó, así que no hay columna de sal
/// y se pueden subir las iteraciones de las cuentas nuevas sin invalidar las existentes.
/// </para>
/// <para>
/// <see cref="Verificar"/> lee las iteraciones, la sal y la longitud del hash de la propia cadena. Por eso acepta los
/// hashes de <c>database/002_datos_prueba.sql</c>, que usan 100000 iteraciones y una sal de 17 bytes.
/// </para>
/// </remarks>
public sealed class HashContrasenasPbkdf2 : IHashContrasenas
{
    /// <summary>Primer campo de la cadena: identifica el algoritmo.</summary>
    public const string Algoritmo = "PBKDF2-SHA256";

    /// <summary>
    /// Iteraciones de las cuentas nuevas. 100000 es el mínimo que fija el issue #68 y el valor de los datos de prueba.
    /// Si se sube, las cuentas existentes siguen verificándose con las iteraciones guardadas en su propia cadena.
    /// </summary>
    public const int Iteraciones = 100_000;

    /// <summary>Bytes de la sal aleatoria de cada cuenta nueva.</summary>
    public const int BytesSal = 16;

    /// <summary>Bytes del hash derivado (la salida de SHA-256).</summary>
    public const int BytesHash = 32;

    // Límite de defensa al verificar: una cadena manipulada con un número enorme de iteraciones no bloquea la aplicación.
    private const int IteracionesMaximas = 10_000_000;

    private const char Separador = '$';

    public string Generar(string contrasena)
    {
        ArgumentException.ThrowIfNullOrEmpty(contrasena);

        var sal = RandomNumberGenerator.GetBytes(BytesSal);
        var hash = Derivar(contrasena, sal, Iteraciones, BytesHash);

        return string.Join(
            Separador,
            Algoritmo,
            Iteraciones.ToString(CultureInfo.InvariantCulture),
            Convert.ToBase64String(sal),
            Convert.ToBase64String(hash));
    }

    public bool Verificar(string contrasena, string almacenado)
    {
        ArgumentNullException.ThrowIfNull(contrasena);

        if (string.IsNullOrEmpty(almacenado))
        {
            return false;
        }

        var partes = almacenado.Split(Separador);
        if (partes.Length != 4 || partes[0] != Algoritmo)
        {
            return false;
        }

        if (!int.TryParse(partes[1], NumberStyles.None, CultureInfo.InvariantCulture, out var iteraciones)
            || iteraciones < 1
            || iteraciones > IteracionesMaximas)
        {
            return false;
        }

        byte[] sal;
        byte[] esperado;
        try
        {
            sal = Convert.FromBase64String(partes[2]);
            esperado = Convert.FromBase64String(partes[3]);
        }
        catch (FormatException)
        {
            return false;
        }

        if (sal.Length == 0 || esperado.Length == 0)
        {
            return false;
        }

        var calculado = Derivar(contrasena, sal, iteraciones, esperado.Length);

        // Comparación en tiempo constante: el tiempo no revela cuántos bytes coinciden.
        return CryptographicOperations.FixedTimeEquals(calculado, esperado);
    }

    private static byte[] Derivar(string contrasena, byte[] sal, int iteraciones, int bytes)
    {
        return Rfc2898DeriveBytes.Pbkdf2(
            Encoding.UTF8.GetBytes(contrasena),
            sal,
            iteraciones,
            HashAlgorithmName.SHA256,
            bytes);
    }
}
