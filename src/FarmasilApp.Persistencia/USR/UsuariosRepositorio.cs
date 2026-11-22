using System.Data;
using FarmasilApp.Modelo.Comun;
using FarmasilApp.Modelo.USR;
using Microsoft.Data.SqlClient;

namespace FarmasilApp.Persistencia.USR;

/// <summary>
/// Repositorio de <c>TBL_USUARIOS</c> (ESP-0033 a ESP-0036). Es el repositorio de referencia para los demás módulos.
/// </summary>
/// <remarks>
/// <para><b>Sin reglas.</b> El repositorio solo lee y escribe; las reglas de la especificación están en
/// <see cref="UsuariosServicio"/>.</para>
/// <para><b>Parámetros.</b> Todo valor viaja como <see cref="SqlParameter"/> con su tipo y longitud de columna; ningún
/// texto se concatena al SQL.</para>
/// <para><b>Transacciones.</b> Cada escritura abre su propia conexión y su transacción. Cuando llegue la unidad de trabajo
/// (#99), las escrituras participarán de la transacción de quien llama (DEC-069).</para>
/// <para><b>Literales.</b> Los enums <see cref="Rol"/> y <see cref="EstadoRegistro"/> se guardan con su nombre, que es
/// el literal del <c>CHECK</c> de la tabla, sin tilde (DEC-031).</para>
/// </remarks>
public sealed class UsuariosRepositorio : IUsuariosRepositorio
{
    // Errores de SQL Server que se traducen. 2601 y 2627: fila duplicada en un índice o restricción única.
    // 547: conflicto con una restricción, aquí la clave foránea de otra tabla que referencia la cuenta.
    private const int ErrorIndiceUnicoDuplicado = 2601;
    private const int ErrorRestriccionUnicaDuplicada = 2627;
    private const int ErrorConflictoDeRestriccion = 547;

    private const string PuntoDeGuardado = "AntesDelBorrado";

    private readonly OpcionesBaseDatos _opciones;

    public UsuariosRepositorio(OpcionesBaseDatos opciones)
    {
        _opciones = opciones ?? throw new ArgumentNullException(nameof(opciones));
    }

    public IReadOnlyList<CuentaUsuario> Consultar(string? nombreUsuario, Rol? rol)
    {
        // Nunca se selecciona la columna contrasena (ESP-0034).
        const string sql = """
            SELECT u.id_usuario, u.nombre_usuario, u.rol, u.estado
            FROM dbo.TBL_USUARIOS AS u
            WHERE (@nombre IS NULL OR u.nombre_usuario = @nombre)
              AND (@rol IS NULL OR u.rol = @rol)
            ORDER BY u.nombre_usuario, u.id_usuario;
            """;

        using var conexion = Abrir();
        using var comando = new SqlCommand(sql, conexion);
        comando.Parameters.Add(ParametroNombre("@nombre", nombreUsuario));
        comando.Parameters.Add(ParametroRol(rol));

        var cuentas = new List<CuentaUsuario>();
        using var lector = comando.ExecuteReader();
        while (lector.Read())
        {
            cuentas.Add(LeerCuenta(lector));
        }

        return cuentas;
    }

    public CuentaUsuario? Obtener(int idUsuario)
    {
        const string sql = """
            SELECT u.id_usuario, u.nombre_usuario, u.rol, u.estado
            FROM dbo.TBL_USUARIOS AS u
            WHERE u.id_usuario = @id;
            """;

        using var conexion = Abrir();
        using var comando = new SqlCommand(sql, conexion);
        comando.Parameters.Add(ParametroId(idUsuario));

        using var lector = comando.ExecuteReader();
        return lector.Read() ? LeerCuenta(lector) : null;
    }

    public bool ExisteNombre(string nombreUsuario, int? excluirIdUsuario)
    {
        const string sql = """
            SELECT CASE WHEN EXISTS (
                SELECT 1
                FROM dbo.TBL_USUARIOS AS u
                WHERE u.nombre_usuario = @nombre
                  AND (@excluir IS NULL OR u.id_usuario <> @excluir)
            ) THEN 1 ELSE 0 END;
            """;

        using var conexion = Abrir();
        using var comando = new SqlCommand(sql, conexion);
        comando.Parameters.Add(ParametroNombre("@nombre", nombreUsuario));
        comando.Parameters.Add(new SqlParameter("@excluir", SqlDbType.Int) { Value = (object?)excluirIdUsuario ?? DBNull.Value });

        return (int)comando.ExecuteScalar()! == 1;
    }

    public int ContarAdministradoresActivos(int excluirIdUsuario)
    {
        const string sql = """
            SELECT COUNT(*)
            FROM dbo.TBL_USUARIOS AS u
            WHERE u.rol = N'Administrador'
              AND u.estado = N'Activo'
              AND u.id_usuario <> @excluir;
            """;

        using var conexion = Abrir();
        using var comando = new SqlCommand(sql, conexion);
        comando.Parameters.Add(new SqlParameter("@excluir", SqlDbType.Int) { Value = excluirIdUsuario });

        return (int)comando.ExecuteScalar()!;
    }

    public string? ObtenerHash(int idUsuario)
    {
        const string sql = "SELECT u.contrasena FROM dbo.TBL_USUARIOS AS u WHERE u.id_usuario = @id;";

        using var conexion = Abrir();
        using var comando = new SqlCommand(sql, conexion);
        comando.Parameters.Add(ParametroId(idUsuario));

        return comando.ExecuteScalar() as string;
    }

    public CredencialesUsuario? ObtenerCredenciales(string nombreUsuario)
    {
        // La intercalación Modern_Spanish_CI_AS no distingue mayúsculas (DEC-074). El hash se lee solo aquí.
        const string sql = """
            SELECT u.id_usuario, u.nombre_usuario, u.contrasena, u.rol, u.estado
            FROM dbo.TBL_USUARIOS AS u
            WHERE u.nombre_usuario = @nombre;
            """;

        using var conexion = Abrir();
        using var comando = new SqlCommand(sql, conexion);
        comando.Parameters.Add(ParametroNombre("@nombre", nombreUsuario));

        using var lector = comando.ExecuteReader();
        if (!lector.Read())
        {
            return null;
        }

        return new CredencialesUsuario
        {
            IdUsuario = lector.GetInt32(lector.GetOrdinal("id_usuario")),
            NombreUsuario = lector.GetString(lector.GetOrdinal("nombre_usuario")),
            Hash = lector.GetString(lector.GetOrdinal("contrasena")),
            Rol = Enum.Parse<Rol>(lector.GetString(lector.GetOrdinal("rol"))),
            Estado = Enum.Parse<EstadoRegistro>(lector.GetString(lector.GetOrdinal("estado"))),
        };
    }

    public int Crear(string nombreUsuario, string hash, Rol rol, EstadoRegistro estado)
    {
        const string sql = """
            INSERT INTO dbo.TBL_USUARIOS (nombre_usuario, contrasena, rol, estado)
            OUTPUT INSERTED.id_usuario
            VALUES (@nombre, @hash, @rol, @estado);
            """;

        using var conexion = Abrir();
        using var transaccion = conexion.BeginTransaction();
        using var comando = new SqlCommand(sql, conexion, transaccion);
        comando.Parameters.Add(ParametroNombre("@nombre", nombreUsuario));
        comando.Parameters.Add(ParametroHash(hash));
        comando.Parameters.Add(ParametroRol(rol));
        comando.Parameters.Add(ParametroEstado(estado));

        try
        {
            var id = (int)comando.ExecuteScalar()!;
            transaccion.Commit();
            return id;
        }
        catch (SqlException ex) when (EsDuplicado(ex))
        {
            // Si la consulta previa del servicio no vio el duplicado, el índice único tiene la última palabra.
            throw new ReglaDeNegocioException(MensajesUsuarios.NombreRepetido);
        }
    }

    public void Actualizar(int idUsuario, string nombreUsuario, Rol rol, EstadoRegistro estado, string? hashNuevo)
    {
        // Un solo UPDATE: con @hash nulo se conserva el hash vigente (ESP-0035).
        const string sql = """
            UPDATE dbo.TBL_USUARIOS
            SET nombre_usuario = @nombre,
                rol = @rol,
                estado = @estado,
                contrasena = COALESCE(@hash, contrasena)
            WHERE id_usuario = @id;
            """;

        using var conexion = Abrir();
        using var transaccion = conexion.BeginTransaction();
        using var comando = new SqlCommand(sql, conexion, transaccion);
        comando.Parameters.Add(ParametroId(idUsuario));
        comando.Parameters.Add(ParametroNombre("@nombre", nombreUsuario));
        comando.Parameters.Add(ParametroRol(rol));
        comando.Parameters.Add(ParametroEstado(estado));
        comando.Parameters.Add(ParametroHash(hashNuevo));

        int filas;
        try
        {
            filas = comando.ExecuteNonQuery();
        }
        catch (SqlException ex) when (EsDuplicado(ex))
        {
            throw new ReglaDeNegocioException(MensajesUsuarios.NombreRepetido);
        }

        if (filas != 1)
        {
            // Al salir sin Commit, la transacción se revierte al liberarse.
            throw new InvalidOperationException($"La cuenta {idUsuario} no existe.");
        }

        transaccion.Commit();
    }

    /// <summary>
    /// Baja condicional según DEC-009 y DEC-029 (VP-B). Es el patrón que copian los demás módulos con baja condicional.
    /// </summary>
    /// <remarks>
    /// <list type="number">
    /// <item><c>SET XACT_ABORT OFF</c> explícito: con <c>ON</c>, el error 547 dejaría la transacción no confirmable y no se
    /// podría volver al punto de guardado.</item>
    /// <item>Se abre la transacción y se fija un punto de guardado antes del <c>DELETE</c>.</item>
    /// <item>Si el <c>DELETE</c> borra la fila, se confirma: <see cref="ResultadoBaja.Eliminada"/>.</item>
    /// <item>Si una clave foránea lo rechaza (error 547) y <c>XACT_STATE()</c> vale 1, se vuelve al punto de guardado y se
    /// aplica <c>estado = 'Inactivo'</c> en la misma transacción: <see cref="ResultadoBaja.Desactivada"/>.</item>
    /// <item>Si <c>XACT_STATE()</c> vale -1, o falla cualquier otra cosa, se revierte todo y se informa el error, sin
    /// reintentar en otra transacción.</item>
    /// </list>
    /// El módulo no consulta las tablas que lo referencian (ventas, devoluciones, cotizaciones): la clave foránea decide,
    /// y cubre también las referencias que se agreguen en el futuro (RG-011).
    /// </remarks>
    public ResultadoBaja DarDeBaja(int idUsuario)
    {
        const string sqlBorrar = "DELETE FROM dbo.TBL_USUARIOS WHERE id_usuario = @id;";
        const string sqlDesactivar = "UPDATE dbo.TBL_USUARIOS SET estado = N'Inactivo' WHERE id_usuario = @id;";

        using var conexion = Abrir();

        using (var xactAbort = new SqlCommand("SET XACT_ABORT OFF;", conexion))
        {
            xactAbort.ExecuteNonQuery();
        }

        using var transaccion = conexion.BeginTransaction();
        transaccion.Save(PuntoDeGuardado);

        try
        {
            using var borrar = new SqlCommand(sqlBorrar, conexion, transaccion);
            borrar.Parameters.Add(ParametroId(idUsuario));

            if (borrar.ExecuteNonQuery() != 1)
            {
                throw new InvalidOperationException($"La cuenta {idUsuario} no existe.");
            }

            transaccion.Commit();
            return ResultadoBaja.Eliminada;
        }
        catch (SqlException ex) when (ex.Number == ErrorConflictoDeRestriccion)
        {
            if (EstadoDeLaTransaccion(conexion, transaccion) != 1)
            {
                transaccion.Rollback();
                throw new InvalidOperationException(
                    $"La baja de la cuenta {idUsuario} no se pudo completar: la transacción quedó no confirmable.", ex);
            }

            transaccion.Rollback(PuntoDeGuardado);

            using var desactivar = new SqlCommand(sqlDesactivar, conexion, transaccion);
            desactivar.Parameters.Add(ParametroId(idUsuario));

            if (desactivar.ExecuteNonQuery() != 1)
            {
                throw new InvalidOperationException($"La cuenta {idUsuario} no existe.");
            }

            transaccion.Commit();
            return ResultadoBaja.Desactivada;
        }
    }

    private SqlConnection Abrir()
    {
        var conexion = new SqlConnection(_opciones.CadenaConexion);
        conexion.Open();
        return conexion;
    }

    private static int EstadoDeLaTransaccion(SqlConnection conexion, SqlTransaction transaccion)
    {
        using var comando = new SqlCommand("SELECT XACT_STATE();", conexion, transaccion);
        return Convert.ToInt32(comando.ExecuteScalar(), System.Globalization.CultureInfo.InvariantCulture);
    }

    private static bool EsDuplicado(SqlException ex) =>
        ex.Number is ErrorIndiceUnicoDuplicado or ErrorRestriccionUnicaDuplicada;

    private static CuentaUsuario LeerCuenta(SqlDataReader lector) => new()
    {
        IdUsuario = lector.GetInt32(lector.GetOrdinal("id_usuario")),
        NombreUsuario = lector.GetString(lector.GetOrdinal("nombre_usuario")),
        Rol = Enum.Parse<Rol>(lector.GetString(lector.GetOrdinal("rol"))),
        Estado = Enum.Parse<EstadoRegistro>(lector.GetString(lector.GetOrdinal("estado"))),
    };

    private static SqlParameter ParametroId(int idUsuario) =>
        new("@id", SqlDbType.Int) { Value = idUsuario };

    // El tamaño nunca es menor que el texto: SqlClient truncaría en silencio una búsqueda más larga que la columna,
    // y el texto truncado podría coincidir con otra cuenta.
    private static SqlParameter ParametroNombre(string nombreParametro, string? nombreUsuario) =>
        new(nombreParametro, SqlDbType.NVarChar, Math.Max(UsuariosServicio.LongitudMaximaNombre, nombreUsuario?.Length ?? 0))
        {
            Value = (object?)nombreUsuario ?? DBNull.Value,
        };

    private static SqlParameter ParametroHash(string? hash) =>
        new("@hash", SqlDbType.NVarChar, 255) { Value = (object?)hash ?? DBNull.Value };

    private static SqlParameter ParametroRol(Rol? rol) =>
        new("@rol", SqlDbType.NVarChar, 20) { Value = rol is null ? DBNull.Value : rol.Value.ToString() };

    private static SqlParameter ParametroEstado(EstadoRegistro estado) =>
        new("@estado", SqlDbType.NVarChar, 10) { Value = estado.ToString() };
}
