using UniversiteDomain.DataAdapters;
using UniversiteDomain.DataAdapters.DataAdaptersFactory;
using UniversiteDomain.Entities;
using UniversiteDomain.Exceptions.UeExceptions;

namespace UniversiteDomain.UseCases.UeUseCases.Create;

public class CreateUeUseCase(IRepositoryFactory factoryRepository)
{
    public async Task<Ue> ExecuteAsync()
    {
        var ue = new Ue{};
        return await ExecuteAsync(ue);
    }
    public async Task<Ue> ExecuteAsync(Ue uee)
    {
        await CheckBusinessRules(uee);
        Ue ue = await factoryRepository.UeRepository().CreateAsync(uee);
        factoryRepository.UeRepository().SaveChangesAsync().Wait();
        return ue;
    }
    private async Task CheckBusinessRules(Ue ue)
    {
        ArgumentNullException.ThrowIfNull(ue);
        ArgumentNullException.ThrowIfNull(ue.NumeroUe);
        ArgumentNullException.ThrowIfNull(ue.Intitule);
        ArgumentNullException.ThrowIfNull(factoryRepository.UeRepository());
        
        
        // On recherche une ue avec le même numéro
        List<Ue> existe = await factoryRepository.UeRepository().FindByConditionAsync(e=>e.NumeroUe.Equals(ue.NumeroUe));
        
        if (existe .Any()) throw new DuplicateNumeroUeException(ue.NumeroUe+ " - ce numéro d'Ue est déjà affecté à une Ue");
        
        if (ue.Intitule.Length < 3) throw new InvalidUeIntituleException(ue.Intitule +" incorrect - L'intitule doit contenir plus de 3 caractères");
        
    }    
    
    public bool IsAuthorized(string role)
    {
        return role.Equals(Roles.Responsable) || role.Equals(Roles.Scolarite);
    }
}