using FarmasilApp.Modelo.ALV;
using FarmasilApp.Modelo.LOT;
using FarmasilApp.Modelo.Seguridad;
using FarmasilApp.Persistencia.LOT;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace FarmasilApp.Persistencia.ALV;

/// <summary>Enlaza ALV a la lectura SQL de lotes, sin sustituir globalmente ILotes.</summary>
public static class RegistroConsultaAlertasSql
{
    public static IServiceCollection AgregarConsultaAlertasSql(this IServiceCollection servicios)
    {
        ArgumentNullException.ThrowIfNull(servicios);
        servicios.Replace(ServiceDescriptor.Transient<AlertasServicio>(proveedor =>
            new AlertasServicio(
                proveedor.GetRequiredService<IAlertasRepositorio>(),
                new LotesSqlVencimientos(
                    proveedor.GetRequiredService<OpcionesBaseDatos>(),
                    proveedor.GetRequiredService<ILotes>()),
                proveedor.GetRequiredService<ISesion>(),
                proveedor.GetRequiredService<TimeProvider>())));
        return servicios;
    }
}
