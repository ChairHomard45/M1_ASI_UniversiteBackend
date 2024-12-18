using UniversiteDomain.DataAdapters;
using UniversiteDomain.DataAdapters.DataAdaptersFactory;
using UniversiteDomain.Entities;
using UniversiteDomain.Exceptions.UeExceptions;

namespace UniversiteDomain.UseCases.UeUseCases.Update;

public class UpdateUeUseCase(IRepositoryFactory factory)
{
    public async Task ExecuteAsync(Ue ue)
    {
        await CheckBusinessRules();
        
        List<Ue> ueToUpdate = await factory.UeRepository().FindByConditionAsync(e => e.Id == ue.Id);
        if (ueToUpdate is { Count: 0 } || ue is null) throw new  UeNotFoundException(ue.Id.ToString());

        ueToUpdate[0].NumeroUe = ue.NumeroUe;
        ueToUpdate[0].Intitule = ue.Intitule;
        
        
        await factory.UeRepository().UpdateAsync(ueToUpdate[0]);
        await factory.UeRepository().SaveChangesAsync();
    }
    private async Task CheckBusinessRules()
    {
        ArgumentNullException.ThrowIfNull(factory);
        IUeRepository ueRepository=factory.UeRepository();
        ArgumentNullException.ThrowIfNull(ueRepository);
    }
    
    public bool IsAuthorized(string role)
    {
        return role.Equals(Roles.Responsable) || role.Equals(Roles.Scolarite);
    }
}