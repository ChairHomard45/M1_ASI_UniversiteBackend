using UniversiteDomain.DataAdapters;
using UniversiteDomain.DataAdapters.DataAdaptersFactory;
using UniversiteDomain.Entities;
using UniversiteDomain.Exceptions.EtudiantExceptions;

namespace UniversiteDomain.UseCases.EtudiantUseCases.Update;

public class UpdateEtudiantUseCase(IRepositoryFactory factory)
{
    public async Task ExecuteAsync(Etudiant etudiant)
    {
        await CheckBusinessRules();
        
        List<Etudiant> etudiantToUpdate = await factory.EtudiantRepository().FindByConditionAsync(e => e.Id == etudiant.Id);
        if (etudiantToUpdate is { Count: 0 } || etudiant is null) throw new EtudiantNotFoundException(etudiant.Id.ToString());

        etudiantToUpdate[0].NumEtud = etudiant.NumEtud;
        etudiantToUpdate[0].Nom = etudiant.Nom;
        etudiantToUpdate[0].Prenom = etudiant.Prenom;
        etudiantToUpdate[0].Email = etudiant.Email;
        
        
        await factory.EtudiantRepository().UpdateAsync(etudiantToUpdate[0]);
        await factory.EtudiantRepository().SaveChangesAsync();
    }
    private async Task CheckBusinessRules()
    {
        ArgumentNullException.ThrowIfNull(factory);
        IEtudiantRepository etudiantRepository=factory.EtudiantRepository();
        ArgumentNullException.ThrowIfNull(etudiantRepository);
    }
    
    public bool IsAuthorized(string role)
    {
        return role.Equals(Roles.Responsable) || role.Equals(Roles.Scolarite);
    }
}