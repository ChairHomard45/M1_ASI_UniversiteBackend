using UniversiteDomain.DataAdapters;
using UniversiteDomain.Entities;
using UniversiteDomain.Exceptions.UeExceptions;

namespace UniversiteDomain.UseCases.UeUseCases.Create;

public class CreateUeUseCase(IUeRepository ueRepository)
{
    public async Task<Ue> ExecuteAsync()
    {
        var ue = new Ue{};
        return await ExecuteAsync(ue);
    }
    public async Task<Ue> ExecuteAsync(Ue uee)
    {
        await CheckBusinessRules(uee);
        Ue ue = await ueRepository.CreateAsync(uee);
        ueRepository.SaveChangesAsync().Wait();
        return ue;
    }
    private async Task CheckBusinessRules(Ue ue)
    {
        ArgumentNullException.ThrowIfNull(ue);
        ArgumentNullException.ThrowIfNull(ue.NumeroUe);
        ArgumentNullException.ThrowIfNull(ue.Intitule);
        ArgumentNullException.ThrowIfNull(ueRepository);
        
        
        // On recherche une ue avec le même numéro
        List<Ue> existe = await ueRepository.FindByConditionAsync(e=>e.NumeroUe.Equals(ue.NumeroUe));
        
        if (existe .Any()) throw new DuplicateNumeroUeException(ue.NumeroUe+ " - ce numéro d'étudiant est déjà affecté à un étudiant");
        
        if (ue.Intitule.Length < 3) throw new InvalidUeIntituleException(ue.Intitule +" incorrect - L'intitule doit contenir plus de 3 caractères");
        
    }
}