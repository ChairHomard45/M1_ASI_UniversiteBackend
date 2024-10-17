using UniversiteDomain.DataAdapters;
using UniversiteDomain.Entities;
using UniversiteDomain.Exceptions.ParcoursExceptions;

namespace UniversiteDomain.UseCases.ParcoursUseCases.Create;

public class CreateParcoursUseCase(IParcoursRepository parcoursRepository)
{
    public async Task<Parcours> ExecuteAsync(string nomPar, int anneeFor)
    {
        var parcours = new Parcours{NomParcours = nomPar, AnneeFormation = anneeFor};
        return await ExecuteAsync(parcours);
    }
    public async Task<Parcours> ExecuteAsync(Parcours parcours)
    {
        await CheckBusinessRules(parcours);
        Parcours pa = await parcoursRepository.CreateAsync(parcours);
        parcoursRepository.SaveChangesAsync().Wait();
        return pa;
    }
    private async Task CheckBusinessRules(Parcours parcours)
    {
        ArgumentNullException.ThrowIfNull(parcours);
        ArgumentNullException.ThrowIfNull(parcours.NomParcours);
        ArgumentNullException.ThrowIfNull(parcours.AnneeFormation);
        ArgumentNullException.ThrowIfNull(parcoursRepository);
        
        List<Parcours> existe = await parcoursRepository.FindByConditionAsync(e=>e.NomParcours.Equals(parcours.NomParcours));
        
        if (existe .Any()) throw new DuplicateInscripionException(parcours.NomParcours+ " - ce numéro d'étudiant est déjà affecté à un étudiant");

    }
}