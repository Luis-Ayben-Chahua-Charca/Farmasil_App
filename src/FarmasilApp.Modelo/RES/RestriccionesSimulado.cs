using FarmasilApp.Modelo.Comun;

namespace FarmasilApp.Modelo.RES;

/// <summary>
/// Implementación simulada de <see cref="IRestricciones"/> mientras RES no esté listo (plan, sección 10).
/// Devuelve las restricciones activas de database/002_datos_prueba.sql, con los mismos ids
/// (los que tiene una base recién creada); la restricción 3, inactiva, no se devuelve. No tiene estado.
/// </summary>
public sealed class RestriccionesSimulado : IRestricciones
{
    private static readonly IReadOnlyList<RestriccionDto> Activas = new[]
    {
        new RestriccionDto
        {
            IdRestriccion = 1,
            IdProducto = 1,
            CondicionCliente = "Gestante",
            Tipo = TipoRestriccion.Informativa,
            MotivoAdvertencia = "Consultar dosis recomendada antes de la venta.",
        },
        new RestriccionDto
        {
            IdRestriccion = 2,
            IdProducto = 3,
            CondicionCliente = "Alergia a penicilinas",
            Tipo = TipoRestriccion.Bloqueante,
            MotivoAdvertencia = "No completar la venta si el cliente presenta alergia.",
        },
    };

    public IReadOnlyList<RestriccionDto> RestriccionesActivas(int idProducto)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(idProducto);

        return Activas.Where(restriccion => restriccion.IdProducto == idProducto).ToArray();
    }
}
