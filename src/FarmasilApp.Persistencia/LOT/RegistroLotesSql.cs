using FarmasilApp.Modelo.LOT;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace FarmasilApp.Persistencia.LOT;

/// <summary>
/// Reemplaza la implementación simulada de ILotes por la implementación SQL real (#101).
/// Se invoca después de AgregarModelo: LotesSql vive en Persistencia, que el Modelo no ve (DEC-069).
/// </summary>
public static class RegistroLotesSql
{
    public static IServiceCollection AgregarLotesSql(
        this IServiceCollection servicios)
    {
        ArgumentNullException.ThrowIfNull(servicios);

        servicios.Replace(
            ServiceDescriptor.Singleton<ILotes>(
                proveedor =>
                    new LotesSql(
                        proveedor.GetRequiredService<OpcionesBaseDatos>(),
                        proveedor.GetRequiredService<ContextoTransaccionSql>(),
                        proveedor.GetRequiredService<TimeProvider>())));

        return servicios;
    }
}
