using UniversiteDomain.DataAdapters;
using UniversiteDomain.DataAdapters.DataAdaptersFactory;
using UniversiteDomain.Entities;
using UniversiteDomain.Exceptions.ParcoursExceptions;

namespace UniversiteDomain.UseCases.ParcoursUseCases.Create;

public class CreateParcoursUseCase(IRepositoryFactory factoryRepository)
{
    public async Task<Parcours> ExecuteAsync(string nomPar, int anneeFor)
    {
        var parcours = new Parcours{NomParcours = nomPar, AnneeFormation = anneeFor};
        return await ExecuteAsync(parcours);
    }
    public async Task<Parcours> ExecuteAsync(Parcours parcours)
    {
        await CheckBusinessRules(parcours);
        Parcours pa = await factoryRepository.ParcoursRepository().CreateAsync(parcours);
        factoryRepository.ParcoursRepository().SaveChangesAsync().Wait();
        return pa;
    }
    private async Task CheckBusinessRules(Parcours parcours)
    {
        ArgumentNullException.ThrowIfNull(parcours);
        ArgumentNullException.ThrowIfNull(parcours.NomParcours);
        ArgumentNullException.ThrowIfNull(parcours.AnneeFormation);
        ArgumentNullException.ThrowIfNull(factoryRepository.ParcoursRepository());

        if (parcours.AnneeFormation != 1 && parcours.AnneeFormation != 2) throw new InvalidAnneFormationException(" L'Année de Formation doit être 1 ou 2");
        
        List<Parcours> existe = await factoryRepository.ParcoursRepository().FindByConditionAsync(e=>e.NomParcours.Equals(parcours.NomParcours));
        
        if (existe .Any()) throw new DuplicateInscripionException(parcours.NomParcours+ " - ce numéro d'étudiant est déjà affecté à un étudiant");

    }  
    
    public bool IsAuthorized(string role)
    {
        return role.Equals(Roles.Responsable) || role.Equals(Roles.Scolarite);
    }
}