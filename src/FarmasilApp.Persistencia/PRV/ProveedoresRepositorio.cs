using FarmasilApp.Modelo.Comun;
using FarmasilApp.Modelo.PRV;
using Microsoft.Data.SqlClient;
using System.Data;
using System.Globalization;

namespace FarmasilApp.Persistencia.PRV;

/// <summary>
/// Persistencia del módulo de Proveedores.
/// Todas las entradas se envían a SQL Server mediante parámetros.
/// </summary>
public sealed class ProveedoresRepositorio : IProveedoresRepositorio
{
    private const int ErrorConflictoDeRestriccion = 547;
    private const int ErrorClaveDuplicada2627 = 2627;
    private const int ErrorClaveDuplicada2601 = 2601;
    private const string PuntoDeGuardado = "antes_baja_proveedor";

    private readonly OpcionesBaseDatos _opciones;

    public ProveedoresRepositorio(OpcionesBaseDatos opciones)
    {
        _opciones = opciones
            ?? throw new ArgumentNullException(nameof(opciones));
    }

    public IReadOnlyList<ProveedorConsulta> Consultar(
        string? busqueda,
        string? estado)
    {
        const string sql = """
            SELECT
                p.id_proveedor,
                p.ruc,
                p.razon_social,
                p.telefono,
                p.estado
            FROM dbo.TBL_PROVEEDORES AS p
            WHERE
                (
                    @busqueda IS NULL
                    OR p.ruc = @busqueda
                    OR p.razon_social LIKE '%' + @busquedaLike + '%' ESCAPE '\'
                )
                AND
                (
                    @estado IS NULL
                    OR p.estado = @estado
                )
            ORDER BY p.razon_social, p.id_proveedor;
            """;

        var proveedores = new List<ProveedorConsulta>();

        using var conexion = Abrir();
        using var comando = new SqlCommand(sql, conexion);

        comando.Parameters.Add(
            new SqlParameter("@busqueda", SqlDbType.NVarChar, 150)
            {
                Value = (object?)busqueda ?? DBNull.Value
            });

        comando.Parameters.Add(
            new SqlParameter("@busquedaLike", SqlDbType.NVarChar, 600)
            {
                Value = busqueda is null
                    ? DBNull.Value
                    : EscaparLike(busqueda)
            });

        comando.Parameters.Add(
            new SqlParameter("@estado", SqlDbType.NVarChar, 10)
            {
                Value = (object?)estado ?? DBNull.Value
            });

        using var lector = comando.ExecuteReader();

        while (lector.Read())
        {
            var ordTelefono = lector.GetOrdinal("telefono");

            proveedores.Add(new ProveedorConsulta
            {
                IdProveedor = lector.GetInt32(
                    lector.GetOrdinal("id_proveedor")),

                Ruc = lector.GetString(
                    lector.GetOrdinal("ruc")),

                RazonSocial = lector.GetString(
                    lector.GetOrdinal("razon_social")),

                Telefono = lector.IsDBNull(ordTelefono)
                    ? null
                    : lector.GetString(ordTelefono),

                Estado = lector.GetString(
                    lector.GetOrdinal("estado"))
            });
        }

        return proveedores;
    }

    public IReadOnlyList<ProveedorDto> ObtenerActivos()
    {
        const string sql = """
            SELECT
                id_proveedor,
                ruc,
                razon_social,
                telefono,
                estado
            FROM dbo.TBL_PROVEEDORES
            WHERE estado = N'Activo'
            ORDER BY id_proveedor;
            """;

        var proveedores = new List<ProveedorDto>();

        using var conexion = Abrir();
        using var comando = new SqlCommand(sql, conexion);
        using var lector = comando.ExecuteReader();

        while (lector.Read())
        {
            proveedores.Add(LeerProveedorDto(lector));
        }

        return proveedores;
    }

    public ProveedorDto? ObtenerPorId(int idProveedor)
    {
        const string sql = """
            SELECT
                id_proveedor,
                ruc,
                razon_social,
                telefono,
                estado
            FROM dbo.TBL_PROVEEDORES
            WHERE id_proveedor = @idProveedor;
            """;

        using var conexion = Abrir();
        using var comando = new SqlCommand(sql, conexion);

        comando.Parameters.Add(
            new SqlParameter("@idProveedor", SqlDbType.Int)
            {
                Value = idProveedor
            });

        using var lector = comando.ExecuteReader();

        return lector.Read() ? LeerProveedorDto(lector) : null;
    }

    public bool ExisteRuc(string ruc, int? excluirIdProveedor = null)
    {
        const string sql = """
            SELECT COUNT(*)
            FROM dbo.TBL_PROVEEDORES
            WHERE ruc = @ruc
              AND (@excluirId IS NULL OR id_proveedor != @excluirId);
            """;

        using var conexion = Abrir();
        using var comando = new SqlCommand(sql, conexion);

        comando.Parameters.Add(
            new SqlParameter("@ruc", SqlDbType.NVarChar, 11)
            {
                Value = ruc
            });

        comando.Parameters.Add(
            new SqlParameter("@excluirId", SqlDbType.Int)
            {
                Value = (object?)excluirIdProveedor ?? DBNull.Value
            });

        var resultado = Convert.ToInt32(
            comando.ExecuteScalar(),
            CultureInfo.InvariantCulture);

        return resultado > 0;
    }

    public bool TieneOrdenesPendientes(int idProveedor)
    {
        const string sql = """
            SELECT COUNT(*)
            FROM dbo.TBL_ORDENES_DEVOLUCION
            WHERE id_proveedor = @idProveedor
              AND estado_orden = N'Pendiente';
            """;

        using var conexion = Abrir();
        using var comando = new SqlCommand(sql, conexion);

        comando.Parameters.Add(
            new SqlParameter("@idProveedor", SqlDbType.Int)
            {
                Value = idProveedor
            });

        var resultado = Convert.ToInt32(
            comando.ExecuteScalar(),
            CultureInfo.InvariantCulture);

        return resultado > 0;
    }

    /// <summary>
    /// ESP-0037. Registra el proveedor con el estado de la ficha y devuelve
    /// el identificador generado por SQL Server.
    /// </summary>
    public int Crear(ProveedorEscritura proveedor)
    {
        ArgumentNullException.ThrowIfNull(proveedor);

        const string sql = """
            INSERT INTO dbo.TBL_PROVEEDORES
                (
                    ruc,
                    razon_social,
                    telefono,
                    estado
                )
            OUTPUT INSERTED.id_proveedor
            VALUES
                (
                    @ruc,
                    @razonSocial,
                    @telefono,
                    @estado
                );
            """;

        using var conexion = Abrir();
        using var transaccion = conexion.BeginTransaction();

        try
        {
            using var comando = new SqlCommand(sql, conexion, transaccion);

            AgregarParametrosProveedor(comando, proveedor);

            var resultado = comando.ExecuteScalar();

            if (resultado is null || resultado is DBNull)
            {
                throw new InvalidOperationException(
                    "SQL Server no devolvió el identificador del proveedor creado.");
            }

            var idProveedor = Convert.ToInt32(
                resultado,
                CultureInfo.InvariantCulture);

            transaccion.Commit();

            return idProveedor;
        }
        catch (SqlException ex)
            when (ex.Number is ErrorClaveDuplicada2627 or ErrorClaveDuplicada2601)
        {
            throw new ReglaDeNegocioException(
                MensajesProveedores.RucRepetido);
        }
    }

    /// <summary>
    /// ESP-0039. Actualiza solamente los datos editables, incluido el estado.
    /// El identificador permanece intacto.
    /// </summary>
    public void Actualizar(
        int idProveedor,
        ProveedorEscritura proveedor)
    {
        ArgumentNullException.ThrowIfNull(proveedor);

        const string sql = """
            UPDATE dbo.TBL_PROVEEDORES
            SET ruc = @ruc,
                razon_social = @razonSocial,
                telefono = @telefono,
                estado = @estado
            WHERE id_proveedor = @idProveedor;
            """;

        using var conexion = Abrir();
        using var transaccion = conexion.BeginTransaction();

        try
        {
            using var comando = new SqlCommand(sql, conexion, transaccion);

            comando.Parameters.Add(
                new SqlParameter("@idProveedor", SqlDbType.Int)
                {
                    Value = idProveedor
                });

            AgregarParametrosProveedor(comando, proveedor);

            var filas = comando.ExecuteNonQuery();

            if (filas != 1)
            {
                throw new InvalidOperationException(
                    $"El proveedor {idProveedor} no existe.");
            }

            transaccion.Commit();
        }
        catch (SqlException ex)
            when (ex.Number is ErrorClaveDuplicada2627 or ErrorClaveDuplicada2601)
        {
            throw new ReglaDeNegocioException(
                MensajesProveedores.RucRepetido);
        }
    }

    /// <summary>
    /// ESP-0040. Baja condicional según VP-10 y DEC-029.
    /// Primero intenta el borrado físico. Si una clave foránea lo impide
    /// mediante el error 547 y la transacción sigue confirmable, vuelve
    /// al punto de guardado y marca el proveedor como Inactivo.
    /// </summary>
    public ResultadoBajaProveedor DarDeBaja(int idProveedor)
    {
        const string sqlBorrar = """
            DELETE FROM dbo.TBL_PROVEEDORES
            WHERE id_proveedor = @idProveedor;
            """;

        const string sqlDesactivar = """
            UPDATE dbo.TBL_PROVEEDORES
            SET estado = N'Inactivo'
            WHERE id_proveedor = @idProveedor;
            """;

        using var conexion = Abrir();

        using (var xactAbort =
               new SqlCommand("SET XACT_ABORT OFF;", conexion))
        {
            xactAbort.ExecuteNonQuery();
        }

        using var transaccion = conexion.BeginTransaction();

        transaccion.Save(PuntoDeGuardado);

        try
        {
            using var borrar =
                new SqlCommand(sqlBorrar, conexion, transaccion);

            borrar.Parameters.Add(
                new SqlParameter("@idProveedor", SqlDbType.Int)
                {
                    Value = idProveedor
                });

            if (borrar.ExecuteNonQuery() != 1)
            {
                throw new InvalidOperationException(
                    $"El proveedor {idProveedor} no existe.");
            }

            transaccion.Commit();

            return ResultadoBajaProveedor.Eliminado;
        }
        catch (SqlException ex)
            when (ex.Number == ErrorConflictoDeRestriccion)
        {
            if (EstadoDeLaTransaccion(
                    conexion,
                    transaccion) != 1)
            {
                transaccion.Rollback();

                throw new InvalidOperationException(
                    $"La baja del proveedor {idProveedor} no se pudo completar: " +
                    "la transacción quedó no confirmable.",
                    ex);
            }

            transaccion.Rollback(PuntoDeGuardado);

            using var desactivar =
                new SqlCommand(
                    sqlDesactivar,
                    conexion,
                    transaccion);

            desactivar.Parameters.Add(
                new SqlParameter("@idProveedor", SqlDbType.Int)
                {
                    Value = idProveedor
                });

            if (desactivar.ExecuteNonQuery() != 1)
            {
                throw new InvalidOperationException(
                    $"El proveedor {idProveedor} no existe.");
            }

            transaccion.Commit();

            return ResultadoBajaProveedor.Desactivado;
        }
    }

    private SqlConnection Abrir()
    {
        var conexion =
            new SqlConnection(_opciones.CadenaConexion);

        conexion.Open();

        return conexion;
    }

    private static void AgregarParametrosProveedor(
        SqlCommand comando,
        ProveedorEscritura proveedor)
    {
        comando.Parameters.Add(
            new SqlParameter(
                "@ruc",
                SqlDbType.NVarChar,
                11)
            {
                Value = proveedor.Ruc
            });

        comando.Parameters.Add(
            new SqlParameter(
                "@razonSocial",
                SqlDbType.NVarChar,
                ProveedoresServicio.LongitudMaximaRazonSocial)
            {
                Value = proveedor.RazonSocial
            });

        comando.Parameters.Add(
            new SqlParameter(
                "@telefono",
                SqlDbType.NVarChar,
                ProveedoresServicio.LongitudMaximaTelefono)
            {
                Value = (object?)proveedor.Telefono ?? DBNull.Value
            });

        comando.Parameters.Add(
            new SqlParameter(
                "@estado",
                SqlDbType.NVarChar,
                10)
            {
                Value = proveedor.Estado
            });
    }

    private static ProveedorDto LeerProveedorDto(SqlDataReader lector)
    {
        var ordTelefono = lector.GetOrdinal("telefono");
        var estadoTexto = lector.GetString(lector.GetOrdinal("estado"));

        return new ProveedorDto
        {
            IdProveedor = lector.GetInt32(
                lector.GetOrdinal("id_proveedor")),

            Ruc = lector.GetString(
                lector.GetOrdinal("ruc")),

            RazonSocial = lector.GetString(
                lector.GetOrdinal("razon_social")),

            Telefono = lector.IsDBNull(ordTelefono)
                ? null
                : lector.GetString(ordTelefono),

            Estado = Enum.Parse<EstadoRegistro>(estadoTexto),
        };
    }

    private static int EstadoDeLaTransaccion(
        SqlConnection conexion,
        SqlTransaction transaccion)
    {
        using var comando =
            new SqlCommand(
                "SELECT XACT_STATE();",
                conexion,
                transaccion);

        return Convert.ToInt32(
            comando.ExecuteScalar(),
            CultureInfo.InvariantCulture);
    }

    private static string EscaparLike(string valor)
    {
        return valor
            .Replace(@"\", @"\\")
            .Replace("%", @"\%")
            .Replace("_", @"\_")
            .Replace("[", @"\[");
    }
}
