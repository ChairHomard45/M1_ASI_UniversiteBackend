using UniversiteDomain.DataAdapters;
using UniversiteDomain.DataAdapters.DataAdaptersFactory;
using UniversiteDomain.Entities;

namespace UniversiteDomain.UseCases.SecurityUseCases.Update;

public class UpdateUniversiteUserUseCase(IRepositoryFactory factory)
{
    public async Task ExecuteAsync(Etudiant etudiant)
    {
        await CheckBusinessRules(etudiant);
    }
    private async Task CheckBusinessRules(Etudiant etudiant)
    {
        ArgumentNullException.ThrowIfNull(factory);
        IUniversiteUserRepository userRepository=factory.UniversiteUserRepository();
        ArgumentNullException.ThrowIfNull(userRepository);
        
        var user = await userRepository.FindByEmailAsync(etudiant.Email);
        if (user == null) throw new NullReferenceException("User does not exist");
        
        await userRepository.UpdateAsync(user, etudiant.Email, etudiant.Email);
        await userRepository.SaveChangesAsync();
    }
    
    public bool IsAuthorized(string role)
    {
        return role.Equals(Roles.Responsable) || role.Equals(Roles.Scolarite);
    }
}