using UniversiteDomain.DataAdapters;
using UniversiteDomain.DataAdapters.DataAdaptersFactory;
using UniversiteDomain.Entities;
using UniversiteDomain.Exceptions.EtudiantExceptions;

namespace UniversiteDomain.UseCases.SecurityUseCases.Delete;

public class DeleteUniversiteUserUseCase(IRepositoryFactory factory)
{
    public async Task ExecuteAsync(long idEtudiant)
    {
        await CheckBusinessRules(idEtudiant);
    }
    private async Task CheckBusinessRules(long idEtudiant)
    {
        ArgumentNullException.ThrowIfNull(factory);
        IUniversiteUserRepository userRepository=factory.UniversiteUserRepository();
        ArgumentNullException.ThrowIfNull(userRepository);
        ArgumentNullException.ThrowIfNull(factory.EtudiantRepository());
        
        List<Etudiant> etud = await factory.EtudiantRepository().FindByConditionAsync(e => e.Id == idEtudiant);
        if (etud == null) throw new EtudiantNotFoundException(idEtudiant.ToString());
        
        var user = await factory.UniversiteUserRepository().FindByEmailAsync(etud[0].Email);
        if (user == null) throw new NullReferenceException("User does not exist");
        
        await userRepository.DeleteAsync(user);
        await factory.UniversiteUserRepository().SaveChangesAsync();
    }

    
    public bool IsAuthorized(string role)
    {
        return role.Equals(Roles.Responsable) || role.Equals(Roles.Scolarite);
    }
    
}