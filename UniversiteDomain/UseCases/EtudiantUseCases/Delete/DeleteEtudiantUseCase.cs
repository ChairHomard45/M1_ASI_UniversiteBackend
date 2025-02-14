using UniversiteDomain.DataAdapters;
using UniversiteDomain.DataAdapters.DataAdaptersFactory;
using UniversiteDomain.Entities;
using UniversiteDomain.Exceptions.EtudiantExceptions;
using UniversiteDomain.UseCases.NotesUseCases.Delete;

namespace UniversiteDomain.UseCases.EtudiantUseCases.Delete;

public class DeleteEtudiantUseCase(IRepositoryFactory factory)
{
    public async Task ExecuteAsync(long idEtudiant)
    {
        DeleteNoteUseCase deleteNoteUseCase = new DeleteNoteUseCase(factory);
        await CheckBusinessRules();
        
        Etudiant? etud = await factory.EtudiantRepository().FindEtudiantCompletAsync(idEtudiant);
        if (etud == null) throw new EtudiantNotFoundException("Etudiant not found");

        etud.ParcoursSuivi = null;
        
        var tempNotes = etud.NotesObtenues.ToList();
        foreach (var note in tempNotes)
        {
            await deleteNoteUseCase.ExecuteAsync(note);
        }
        await factory.EtudiantRepository().UpdateAsync(etud);
        await factory.EtudiantRepository().SaveChangesAsync();

        await factory.EtudiantRepository().DeleteAsync(idEtudiant);
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