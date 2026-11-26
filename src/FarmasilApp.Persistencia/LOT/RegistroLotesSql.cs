using FarmasilApp.Modelo.INV;
using FarmasilApp.Modelo.LOT;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace FarmasilApp.Persistencia.LOT;

/// <summary>
/// Reemplaza la implementación simulada de ILotes por la implementación SQL real
/// cuando el módulo LOT está disponible.
/// </summary>
public static class RegistroLotesSql
{
    public static IServiceCollection AgregarLotesSql(
        this IServiceCollection servicios)
    {
        ArgumentNullException.ThrowIfNull(servicios);

        servicios.Replace(
            ServiceDescriptor.Transient<IInventario, InventarioContrato>());

        servicios.Replace(
            ServiceDescriptor.Singleton<ILotes>(
                proveedor =>
                    new LotesSql(
                        proveedor.GetRequiredService<OpcionesBaseDatos>(),
                        proveedor.GetRequiredService<ContextoTransaccionSql>(),
                        proveedor.GetRequiredService<TimeProvider>())));

        servicios.AddTransient<LotesServicio>();

        return servicios;
    }
}
