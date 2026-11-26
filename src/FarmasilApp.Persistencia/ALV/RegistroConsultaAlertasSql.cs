using FarmasilApp.Modelo.ALV;
using FarmasilApp.Modelo.DEV;
using FarmasilApp.Modelo.LOT;
using FarmasilApp.Modelo.Seguridad;
using FarmasilApp.Persistencia.LOT;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace FarmasilApp.Persistencia.ALV;

/// <summary>
/// Registra el servicio ALV utilizando lotes SQL para la consulta,
/// conservando los contratos ILotes e IDevoluciones.
/// </summary>
public static class RegistroConsultaAlertasSql
{
    public static IServiceCollection AgregarConsultaAlertasSql(
        this IServiceCollection servicios)
    {
        ArgumentNullException.ThrowIfNull(servicios);

        servicios.Replace(
            ServiceDescriptor.Transient<AlertasServicio>(proveedor =>
                new AlertasServicio(
                    proveedor.GetRequiredService<IAlertasRepositorio>(),
                    proveedor.GetRequiredService<ILotes>(),
                    proveedor.GetRequiredService<ISesion>(),
                    proveedor.GetRequiredService<TimeProvider>(),
                    proveedor.GetRequiredService<IDevoluciones>()
                )
            )
        );

        return servicios;
    }
}
