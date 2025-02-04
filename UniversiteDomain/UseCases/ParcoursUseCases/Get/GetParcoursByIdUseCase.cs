using UniversiteDomain.DataAdapters;
using UniversiteDomain.DataAdapters.DataAdaptersFactory;
using UniversiteDomain.Entities;

namespace UniversiteDomain.UseCases.EtudiantUseCases.Get;

public class GetParcoursByIdUseCase(IRepositoryFactory factory)
{
    public async Task<Parcours?> ExecuteAsync(long idParcours)
    {
        await CheckBusinessRules();
        Parcours? parcour = await factory.ParcoursRepository().FindAsync(idParcours);
        return parcour;
    }
    private async Task CheckBusinessRules()
    {
        ArgumentNullException.ThrowIfNull(factory);
        IParcoursRepository parcoursRep=factory.ParcoursRepository();
        ArgumentNullException.ThrowIfNull(parcoursRep);
    }
    public bool IsAuthorized(string role, IUniversiteUser user, long idEtudiant)
    {
        if (role.Equals(Roles.Scolarite) || role.Equals(Roles.Responsable)) return true;
        // Si c'est un étudiant qui est connecté,
        // il ne peut consulter que ses notes
        return user.Etudiant!=null && role.Equals(Roles.Etudiant) && user.Etudiant.Id==idEtudiant;
    }
}