using System.Data;
using FarmasilApp.Modelo.ALV;
using Microsoft.Data.SqlClient;

namespace FarmasilApp.Persistencia.ALV;

/// <summary>Solo escribe dbo.TBL_PRODUCTOS_RETIRADOS. El estado de LOT es responsabilidad de ILotes.</summary>
public sealed class AlertasRepositorio : IAlertasRepositorio
{
    private readonly OpcionesBaseDatos _opciones;
    private readonly ContextoTransaccionSql _contextoTransaccion;

    public AlertasRepositorio(OpcionesBaseDatos opciones)
        : this(opciones, new ContextoTransaccionSql())
    {
    }

    public AlertasRepositorio(
        OpcionesBaseDatos opciones,
        ContextoTransaccionSql contextoTransaccion)
    {
        _opciones = opciones
            ?? throw new ArgumentNullException(nameof(opciones));

        _contextoTransaccion = contextoTransaccion
            ?? throw new ArgumentNullException(nameof(contextoTransaccion));
    }

    public int ObtenerUmbralMesesActivo()
    {
        const string sql = """
            SELECT TOP (2) umbral_meses FROM dbo.TBL_ALERTAS_VENCIMIENTO
            WHERE estado = @estado ORDER BY id_alerta;
            """;
        using var conexion = new SqlConnection(_opciones.CadenaConexion);
        using var comando = new SqlCommand(sql, conexion);
        comando.Parameters.Add("@estado", SqlDbType.NVarChar, 10).Value = "Activo";
        conexion.Open();
        using var lector = comando.ExecuteReader();
        if (!lector.Read()) throw new InvalidOperationException("No hay regla activa de vencimiento.");
        var umbral = lector.GetInt32(0);
        if (lector.Read()) throw new InvalidOperationException("Hay más de una regla activa.");
        return umbral;
    }

    public IReadOnlyList<AlertaConsulta> ConsultarRetiros()
    {
        const string sql = """
            SELECT id_retiro, id_lote, nombre_producto, numero_lote,
                   fecha_vencimiento, es_lote_defectuoso, alerta_digemid
            FROM dbo.TBL_PRODUCTOS_RETIRADOS
            ORDER BY fecha_vencimiento, nombre_producto, numero_lote, id_retiro;
            """;
        using var conexion = new SqlConnection(_opciones.CadenaConexion);
        using var comando = new SqlCommand(sql, conexion);
        conexion.Open();
        using var lector = comando.ExecuteReader();
        var resultado = new List<AlertaConsulta>();
        while (lector.Read())
        {
            resultado.Add(new AlertaConsulta
            {
                IdRetiro = lector.GetInt32(0),
                IdLote = lector.IsDBNull(1) ? null : lector.GetInt32(1),
                Medicamento = lector.GetString(2),
                NumeroLote = lector.GetString(3),
                FechaVencimiento = DateOnly.FromDateTime(lector.GetDateTime(4)),
                EsLoteDefectuoso = lector.GetBoolean(5),
                AlertaDigemid = lector.IsDBNull(6) ? null : lector.GetString(6),
                Origen = "Retirado"
            });
        }
        return resultado;
    }

    public T EjecutarTransaccion<T>(Func<IAlertasUnidadTrabajo, T> operacion)
    {
        ArgumentNullException.ThrowIfNull(operacion);
        using var conexion = new SqlConnection(_opciones.CadenaConexion);
        conexion.Open();
        // SERIALIZABLE protege el conteo de retiros ante borrados/altas concurrentes.
        using var transaccion =
            conexion.BeginTransaction(
                IsolationLevel.Serializable);

        using var alcance =
            _contextoTransaccion.Establecer(
                conexion,
                transaccion);

        try
        {
            var unidad =
                new UnidadTrabajo(
                    conexion,
                    transaccion);
            var resultado = operacion(unidad);
            transaccion.Commit();
            return resultado;
        }
        catch
        {
            try { transaccion.Rollback(); } catch (SqlException) { /* La conexión puede haber fallado. */ }
            throw;
        }
    }

    private sealed class UnidadTrabajo : IAlertasUnidadTrabajo
    {
        private readonly SqlConnection _conexion;
        private readonly SqlTransaction _tx;
        internal UnidadTrabajo(SqlConnection conexion, SqlTransaction tx) { _conexion = conexion; _tx = tx; }
        private SqlCommand Comando(string sql) => new(sql, _conexion, _tx);

        public AlertaRetiro? ObtenerRetiro(int idRetiro)
        {
            const string sql = """
                SELECT id_retiro, id_lote, nombre_producto, numero_lote,
                       fecha_vencimiento, es_lote_defectuoso, alerta_digemid
                FROM dbo.TBL_PRODUCTOS_RETIRADOS WITH (UPDLOCK, HOLDLOCK)
                WHERE id_retiro = @id;
                """;
            using var cmd = Comando(sql);
            cmd.Parameters.Add("@id", SqlDbType.Int).Value = idRetiro;
            using var lector = cmd.ExecuteReader();
            if (!lector.Read()) return null;
            return new AlertaRetiro
            {
                IdRetiro = lector.GetInt32(0),
                IdLote = lector.IsDBNull(1) ? null : lector.GetInt32(1),
                NombreProducto = lector.GetString(2),
                NumeroLote = lector.GetString(3),
                FechaVencimiento = DateOnly.FromDateTime(lector.GetDateTime(4)),
                EsLoteDefectuoso = lector.GetBoolean(5),
                AlertaDigemid = lector.IsDBNull(6) ? null : lector.GetString(6)
            };
        }

        public int Insertar(NuevaAlertaRetiro retiro)
        {
            const string sql = """
                INSERT INTO dbo.TBL_PRODUCTOS_RETIRADOS
                (id_lote, nombre_producto, numero_lote, fecha_vencimiento,
                 es_lote_defectuoso, alerta_digemid, fecha_registro)
                OUTPUT INSERTED.id_retiro
                VALUES (@lote, @producto, @numero, @vence, @defecto, @digemid, @registrado);
                """;
            using var cmd = Comando(sql);
            cmd.Parameters.Add("@lote", SqlDbType.Int).Value = retiro.IdLote;
            cmd.Parameters.Add("@producto", SqlDbType.NVarChar, 100).Value = retiro.NombreProducto;
            cmd.Parameters.Add("@numero", SqlDbType.NVarChar, 30).Value = retiro.NumeroLote;
            cmd.Parameters.Add("@vence", SqlDbType.Date).Value = retiro.FechaVencimiento.ToDateTime(TimeOnly.MinValue);
            cmd.Parameters.Add("@defecto", SqlDbType.Bit).Value = retiro.EsLoteDefectuoso;
            cmd.Parameters.Add("@digemid", SqlDbType.NVarChar, 100).Value = (object?)retiro.AlertaDigemid ?? DBNull.Value;
            cmd.Parameters.Add("@registrado", SqlDbType.Date).Value = retiro.FechaRegistro.ToDateTime(TimeOnly.MinValue);
            return Convert.ToInt32(cmd.ExecuteScalar());
        }

        public bool Actualizar(int idRetiro, bool esDefectuoso, string? codigoDigemid)
        {
            const string sql = """
                UPDATE dbo.TBL_PRODUCTOS_RETIRADOS
                SET es_lote_defectuoso = @defecto, alerta_digemid = @digemid
                WHERE id_retiro = @id;
                """;
            using var cmd = Comando(sql);
            cmd.Parameters.Add("@id", SqlDbType.Int).Value = idRetiro;
            cmd.Parameters.Add("@defecto", SqlDbType.Bit).Value = esDefectuoso;
            cmd.Parameters.Add("@digemid", SqlDbType.NVarChar, 100).Value = (object?)codigoDigemid ?? DBNull.Value;
            return cmd.ExecuteNonQuery() == 1;
        }

        public bool Eliminar(int idRetiro)
        {
            const string sql = "DELETE FROM dbo.TBL_PRODUCTOS_RETIRADOS WHERE id_retiro = @id;";
            using var cmd = Comando(sql);
            cmd.Parameters.Add("@id", SqlDbType.Int).Value = idRetiro;
            return cmd.ExecuteNonQuery() == 1;
        }

        public int ContarRetiros(int idLote)
        {
            const string sql = """
                SELECT COUNT(*) FROM dbo.TBL_PRODUCTOS_RETIRADOS WITH (UPDLOCK, HOLDLOCK)
                WHERE id_lote = @lote;
                """;
            using var cmd = Comando(sql);
            cmd.Parameters.Add("@lote", SqlDbType.Int).Value = idLote;
            return Convert.ToInt32(cmd.ExecuteScalar());
        }
    }
}
