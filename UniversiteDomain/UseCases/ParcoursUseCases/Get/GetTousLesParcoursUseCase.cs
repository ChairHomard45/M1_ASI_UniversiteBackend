using UniversiteDomain.DataAdapters;
using UniversiteDomain.DataAdapters.DataAdaptersFactory;
using UniversiteDomain.Entities;

namespace UniversiteDomain.UseCases.ParcoursUseCases.Get;

public class GetTousLesParcoursUseCase(IRepositoryFactory factory)
{
    public async Task<List<Parcours>?> ExecuteAsync()
    {
        await CheckBusinessRules();
        List<Parcours>? parcours = await factory.ParcoursRepository().FindAllAsync();
        return parcours;
    }

    private async Task CheckBusinessRules()
    {
        ArgumentNullException.ThrowIfNull(factory);
        IParcoursRepository parcoursRepository = factory.ParcoursRepository();
        ArgumentNullException.ThrowIfNull(parcoursRepository);
    }

    public bool IsAuthorized(string role)
    {
        return role.Equals(Roles.Responsable) || role.Equals(Roles.Scolarite);
    }
}