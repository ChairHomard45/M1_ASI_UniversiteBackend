using UniversiteDomain.DataAdapters;
using UniversiteDomain.DataAdapters.DataAdaptersFactory;
using UniversiteDomain.Entities;
using UniversiteDomain.Exceptions.ParcoursExceptions;

namespace UniversiteDomain.UseCases.ParcoursUseCases.Update;

public class UpdateParcoursUseCase(IRepositoryFactory factory)
{
    public async Task ExecuteAsync(Parcours parcours)
    {
        await CheckBusinessRules();
        
        List<Parcours> parcoursToUpdate = await factory.ParcoursRepository().FindByConditionAsync(e => e.Id == parcours.Id);
        if (parcoursToUpdate is { Count: 0 } || parcours is null) throw new ParcoursNotFoundException(parcours.Id.ToString());

        parcoursToUpdate[0].NomParcours = parcours.NomParcours;
        parcoursToUpdate[0].AnneeFormation = parcours.AnneeFormation;
        
        await factory.ParcoursRepository().UpdateAsync(parcoursToUpdate[0]);
        await factory.ParcoursRepository().SaveChangesAsync();
    }
    private async Task CheckBusinessRules()
    {
        ArgumentNullException.ThrowIfNull(factory);
        IParcoursRepository parcoursRepository=factory.ParcoursRepository();
        ArgumentNullException.ThrowIfNull(parcoursRepository);
    }
    
    public bool IsAuthorized(string role)
    {
        return role.Equals(Roles.Responsable) || role.Equals(Roles.Scolarite);
    }
}