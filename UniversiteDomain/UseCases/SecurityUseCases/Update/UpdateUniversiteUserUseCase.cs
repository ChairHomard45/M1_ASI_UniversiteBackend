using UniversiteDomain.DataAdapters;
using UniversiteDomain.DataAdapters.DataAdaptersFactory;
using UniversiteDomain.Entities;

namespace UniversiteDomain.UseCases.SecurityUseCases.Update;

public class UpdateUniversiteUserUseCase(IRepositoryFactory factory)
{
    public async Task ExecuteAsync(Etudiant etudiant)
    {
        await CheckBusinessRules();
        var user = await factory.UniversiteUserRepository().FindByEmailAsync(etudiant.Email);
        if (user == null) throw new NullReferenceException("L'utilisateur n'existe pas");
        
        await factory.UniversiteUserRepository().UpdateAsync(user, etudiant.Email, etudiant.Email);
        //await factory.UniversiteUserRepository().SaveChangesAsync();
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