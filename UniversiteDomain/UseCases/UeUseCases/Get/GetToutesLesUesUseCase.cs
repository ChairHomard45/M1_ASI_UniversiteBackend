using UniversiteDomain.DataAdapters;
using UniversiteDomain.DataAdapters.DataAdaptersFactory;
using UniversiteDomain.Entities;

namespace UniversiteDomain.UseCases.UeUseCases.Get;

public class GetToutesLesUesUseCase(IRepositoryFactory factory)
{
    public async Task<List<Ue>?> ExecuteAsync()
    {
        await CheckBusinessRules();
        List<Ue>? etudiants = await factory.UeRepository().FindAllAsync();
        return etudiants;
    }
    private async Task CheckBusinessRules()
    {
        ArgumentNullException.ThrowIfNull(factory);
        IUeRepository ueRepo =factory.UeRepository();
        ArgumentNullException.ThrowIfNull(ueRepo);
    }
    public bool IsAuthorized(string role)
    {
        return role.Equals(Roles.Responsable) || role.Equals(Roles.Scolarite);
    }
}