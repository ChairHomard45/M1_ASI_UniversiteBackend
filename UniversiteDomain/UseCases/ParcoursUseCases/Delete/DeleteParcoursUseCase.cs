using UniversiteDomain.DataAdapters;
using UniversiteDomain.DataAdapters.DataAdaptersFactory;
using UniversiteDomain.Entities;

namespace UniversiteDomain.UseCases.ParcoursUseCases.Delete;

public class DeleteParcoursUseCase(IRepositoryFactory factory)
{
    public async Task ExecuteAsync(long idParcours)
    {
        await CheckBusinessRules();
        var etudiantRepository = factory.EtudiantRepository();
        var etudiants = await etudiantRepository.FindByConditionAsync(e => e.ParcoursSuivi.Id == idParcours);

        // Update each Etudiant to set ParcoursSuiviId to NULL
        foreach (var etudiant in etudiants)
        {
            etudiant.ParcoursSuivi = null;
            await etudiantRepository.UpdateAsync(etudiant);
        }
        
        await factory.ParcoursRepository().DeleteAsync(idParcours);
        await factory.ParcoursRepository().SaveChangesAsync();
    }
    private async Task CheckBusinessRules()
    {
        ArgumentNullException.ThrowIfNull(factory);
        IParcoursRepository parcoursRepository=factory.ParcoursRepository();
        IEtudiantRepository etudiantRepository=factory.EtudiantRepository();
        ArgumentNullException.ThrowIfNull(parcoursRepository);
        ArgumentNullException.ThrowIfNull(etudiantRepository);
    }
    
    public bool IsAuthorized(string role)
    {
        return role.Equals(Roles.Responsable) || role.Equals(Roles.Scolarite);
    }
}