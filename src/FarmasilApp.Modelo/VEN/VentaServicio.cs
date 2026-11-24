using FarmasilApp.Modelo.Comun;
using FarmasilApp.Modelo.LOT;
using FarmasilApp.Modelo.PAG;
using FarmasilApp.Modelo.RES;
using FarmasilApp.Modelo.Seguridad;

namespace FarmasilApp.Modelo.VEN;

/// <summary>
/// Reglas de la pantalla de venta (ESP-0001): catálogo, remesa FEFO, método de pago y validación de cada línea.
/// Trabaja contra <see cref="ILotes"/>, <see cref="IPagos"/> e <see cref="IRestricciones"/>, que hoy son simulados (plan,
/// sección 10). No guarda nada: el registro de la venta es el #99.
/// </summary>
/// <remarks>
/// <para><b>Vendibilidad (DEC-033).</b> La aplica <see cref="ILotes"/>: el catálogo y la remesa FEFO nunca traen un
/// medicamento descontinuado ni una remesa vencida, bloqueada o agotada. Este servicio no consulta Inventario (DEC-044).
/// Como entre mostrar la remesa y agregarla puede cambiar, <see cref="AgregarLinea"/> la vuelve a leer con
/// <see cref="ILotes.DatosRemesas"/>.</para>
/// <para><b>Cómo se informa un rechazo (DEC-072).</b> Toda línea rechazada lanza <see cref="ReglaDeNegocioException"/> con
/// el texto de <see cref="MensajesVentas"/>; la pantalla lo muestra en <c>VEN-LBL-ADVERTENCIAS</c>. Una sesión sin rol de
/// venta recibe <see cref="AccesoDenegadoException"/>.</para>
/// </remarks>
public sealed class VentaServicio
{
    private readonly ILotes _lotes;
    private readonly IRestricciones _restricciones;
    private readonly IPagos _pagos;
    private readonly ISesion _sesion;
    private readonly TimeProvider _reloj;

    public VentaServicio(ILotes lotes, IRestricciones restricciones, IPagos pagos, ISesion sesion, TimeProvider reloj)
    {
        _lotes = lotes ?? throw new ArgumentNullException(nameof(lotes));
        _restricciones = restricciones ?? throw new ArgumentNullException(nameof(restricciones));
        _pagos = pagos ?? throw new ArgumentNullException(nameof(pagos));
        _sesion = sesion ?? throw new ArgumentNullException(nameof(sesion));
        _reloj = reloj ?? throw new ArgumentNullException(nameof(reloj));
    }

    /// <summary>Fecha y hora del sistema para <c>VEN-LBL-FECHA-VENTA</c>.</summary>
    public DateTime FechaHoraSistema() => _reloj.GetLocalNow().DateTime;

    /// <summary>
    /// Medicamentos vendibles del catálogo. <paramref name="busqueda"/> filtra por nombre (contiene, sin distinguir
    /// mayúsculas) y <paramref name="accionTerapeutica"/> por categoría exacta; un criterio vacío no filtra (ESP-0001).
    /// </summary>
    public IReadOnlyList<MedicamentoVendibleDto> MedicamentosVendibles(string? busqueda = null, string? accionTerapeutica = null)
    {
        ExigirAcceso();

        var texto = string.IsNullOrWhiteSpace(busqueda) ? null : busqueda.Trim();
        var accion = string.IsNullOrWhiteSpace(accionTerapeutica) ? null : accionTerapeutica.Trim();

        return _lotes.MedicamentosVendibles()
            .Where(m => texto is null || m.Nombre.Contains(texto, StringComparison.CurrentCultureIgnoreCase))
            .Where(m => accion is null || string.Equals(m.AccionTerapeutica, accion, StringComparison.CurrentCultureIgnoreCase))
            .ToArray();
    }

    /// <summary>Remesas vendibles del medicamento por vencimiento; la primera es la FEFO sugerida.</summary>
    public IReadOnlyList<RemesaVendibleDto> RemesasSugeridas(int idProducto)
    {
        ExigirAcceso();
        return _lotes.SugerirFefo(idProducto);
    }

    /// <summary>Métodos de pago 'Activo' para <c>VEN-CMB-METODO-PAGO</c>.</summary>
    public IReadOnlyList<MetodoPagoDto> MetodosPagoActivos()
    {
        ExigirAcceso();
        return _pagos.MetodosActivos();
    }

    /// <summary>
    /// ESP-0001, agregar producto. Valida en el orden de la especificación y agrega la línea al carrito, acumulando la
    /// cantidad si la remesa ya figura (DEC-036). Devuelve los avisos informativos de las restricciones, que no impiden la
    /// venta.
    /// </summary>
    /// <exception cref="ReglaDeNegocioException">La línea se rechaza; el mensaje es el de <see cref="MensajesVentas"/> o el de la restricción.</exception>
    public IReadOnlyList<string> AgregarLinea(CarritoVenta carrito, RemesaVendibleDto? remesa, int cantidad)
    {
        ArgumentNullException.ThrowIfNull(carrito);
        ExigirAcceso();

        if (remesa is null)
        {
            throw new ReglaDeNegocioException(MensajesVentas.SinRemesa);
        }

        if (cantidad <= 0)
        {
            throw new ReglaDeNegocioException(MensajesVentas.CantidadInvalida);
        }

        // Estado vigente de la remesa: pudo cambiar desde que se mostró.
        var datos = _lotes.DatosRemesas(new[] { remesa.IdLote }).FirstOrDefault()
            ?? throw new ReglaDeNegocioException(MensajesVentas.RemesaNoDisponible);

        var cantidadTotal = cantidad + carrito.CantidadDe(remesa.IdLote);
        if (cantidadTotal > datos.StockActual)
        {
            throw new ReglaDeNegocioException(string.Format(MensajesVentas.StockInsuficiente, datos.StockActual));
        }

        // El medicamento descontinuado lo excluye ILotes al armar el catálogo y la remesa FEFO (DEC-033); aquí no se lee Inventario.
        var hoy = DateOnly.FromDateTime(FechaHoraSistema());
        if (datos.FechaVencimiento <= hoy)
        {
            throw new ReglaDeNegocioException(MensajesVentas.RemesaVencida);
        }

        if (datos.EstadoLote != EstadoLote.Disponible)
        {
            throw new ReglaDeNegocioException(MensajesVentas.RemesaConAlerta);
        }

        var activas = _restricciones.RestriccionesActivas(remesa.IdProducto);
        var bloqueantes = activas.Where(r => r.Tipo == TipoRestriccion.Bloqueante).ToArray();
        if (bloqueantes.Length > 0)
        {
            throw new ReglaDeNegocioException(string.Join(Environment.NewLine, bloqueantes.Select(r => r.MotivoAdvertencia)));
        }

        carrito.Agregar(remesa, cantidad);

        return activas
            .Where(r => r.Tipo == TipoRestriccion.Informativa)
            .Select(r => r.MotivoAdvertencia)
            .ToArray();
    }

    // RNF-0006, condición 3: la venta es de los dos roles; una sesión sin ninguno (o sin sesión) se rechaza.
    private void ExigirAcceso()
    {
        var rol = _sesion.UsuarioActual.Rol;
        if (rol is not (Rol.Administrador or Rol.Tecnico))
        {
            throw new AccesoDenegadoException(MensajesVentas.AccesoDenegado);
        }

        _sesion.ExigirRol(rol);
    }
}
