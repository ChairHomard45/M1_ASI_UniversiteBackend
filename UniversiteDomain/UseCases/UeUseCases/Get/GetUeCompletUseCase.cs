using UniversiteDomain.DataAdapters;
using UniversiteDomain.DataAdapters.DataAdaptersFactory;
using UniversiteDomain.Entities;

namespace UniversiteDomain.UseCases.UeUseCases.Get;

public class GetUeCompletUseCase(IRepositoryFactory factory)
{
    public async Task<Ue?> ExecuteAsync(long idUe)
    {
        await CheckBusinessRules();
        Ue? ues = await factory.UeRepository().FindUeCompletAsync(idUe);
        //Etudiant? etudiant=await factory.EtudiantRepository().FindAsync(idEtudiant);
        return ues;
    }
    private async Task CheckBusinessRules()
    {
        ArgumentNullException.ThrowIfNull(factory);
        IUeRepository ueRepository=factory.UeRepository();
        ArgumentNullException.ThrowIfNull(ueRepository);
    }
    public bool IsAuthorized(string role, IUniversiteUser user, long idEtudiant)
    {
        if (role.Equals(Roles.Scolarite) || role.Equals(Roles.Responsable)) return true;
        // Si c'est un étudiant qui est connecté,
        // il ne peut consulter que ses notes
        return user.Etudiant!=null && role.Equals(Roles.Etudiant) && user.Etudiant.Id==idEtudiant;
    }
}