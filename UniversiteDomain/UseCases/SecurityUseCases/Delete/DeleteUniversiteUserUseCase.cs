using UniversiteDomain.DataAdapters;
using UniversiteDomain.DataAdapters.DataAdaptersFactory;
using UniversiteDomain.Entities;

namespace UniversiteDomain.UseCases.SecurityUseCases.Delete;

public class DeleteUniversiteUserUseCase(IRepositoryFactory factory)
{
    public async Task ExecuteAsync(long idEtudiant)
    {
        await CheckBusinessRules();
        await factory.UniversiteUserRepository().DeleteAsync(idEtudiant);
        await factory.UniversiteUserRepository().SaveChangesAsync();
    }
    private async Task CheckBusinessRules()
    {
        ArgumentNullException.ThrowIfNull(factory);
        IUniversiteUserRepository userRepository=factory.UniversiteUserRepository();
        ArgumentNullException.ThrowIfNull(userRepository);
    }

    
    public bool IsAuthorized(string role)
    {
        return role.Equals(Roles.Responsable) || role.Equals(Roles.Scolarite);
    }
    
}