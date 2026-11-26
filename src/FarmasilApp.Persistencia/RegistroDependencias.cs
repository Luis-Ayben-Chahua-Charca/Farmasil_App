using FarmasilApp.Modelo.ALV;
using FarmasilApp.Modelo.DOC;
using FarmasilApp.Modelo.INV;
using FarmasilApp.Modelo.LOT;
using FarmasilApp.Modelo.PAG;
using FarmasilApp.Modelo.PRV;
using FarmasilApp.Modelo.USR;
using FarmasilApp.Persistencia.ALV;
using FarmasilApp.Persistencia.DOC;
using FarmasilApp.Persistencia.INV;
using FarmasilApp.Persistencia.LOT;
using FarmasilApp.Persistencia.PAG;
using FarmasilApp.Persistencia.PRV;
using FarmasilApp.Persistencia.USR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace FarmasilApp.Persistencia;

/// <summary>
/// Registro de las dependencias de la capa Persistencia. La aplicación lo invoca una sola vez al arrancar (VP-00).
/// Cada repositorio nuevo se agrega aquí.
/// </summary>
public static class RegistroDependencias
{
    public static IServiceCollection AgregarPersistencia(this IServiceCollection servicios, IConfiguration configuracion)
    {
        ArgumentNullException.ThrowIfNull(configuracion);

        var cadena = configuracion.GetConnectionString(OpcionesBaseDatos.NombreCadena);
        if (string.IsNullOrWhiteSpace(cadena))
        {
            throw new InvalidOperationException(
                $"Falta la cadena de conexión '{OpcionesBaseDatos.NombreCadena}' en la sección ConnectionStrings de appsettings.json.");
        }

        servicios.AddSingleton(new OpcionesBaseDatos(cadena));
        servicios.AddSingleton<ContextoTransaccionSql>();
        servicios.AddTransient<IAlertasRepositorio, AlertasRepositorio>();
        servicios.AddTransient<IComprobanteRepositorio, ComprobanteRepositorio>();
        servicios.AddTransient<IInventarioRepositorio, InventarioRepositorio>();
        servicios.AddTransient<ILotesRepositorio, LotesRepositorio>();
        servicios.AddTransient<IMetodosPagoRepositorio, MetodosPagoRepositorio>();
        servicios.AddTransient<IProveedoresRepositorio, ProveedoresRepositorio>();
        servicios.AddTransient<IUsuariosRepositorio, UsuariosRepositorio>();
        return servicios;
    }
}
