using UniversiteDomain.DataAdapters;
using UniversiteDomain.DataAdapters.DataAdaptersFactory;
using UniversiteDomain.Entities;
using UniversiteDomain.Exceptions.EtudiantExceptions;
using UniversiteDomain.Exceptions.NotesException;
using UniversiteDomain.Exceptions.UeExceptions;

namespace UniversiteDomain.UseCases.NotesUseCases.Create;

public class CreateNotesUseCase(IRepositoryFactory repositoryFactory)
{
    public async Task<Notes> ExecuteAsync(float valeur, long EtudiantId, long UeId)
    {
        List<Ue> ue = await repositoryFactory.UeRepository().FindByConditionAsync(e=>e.Id.Equals(UeId));
        if (ue is { Count: 0 }) throw new UeNotFoundException(UeId.ToString());
        
        List<Etudiant> etudiant = await repositoryFactory.EtudiantRepository().FindByConditionAsync(e => e.Id.Equals(EtudiantId));
        if (etudiant is { Count: 0 }) throw new EtudiantNotFoundException(EtudiantId.ToString());
        
        var note = new Notes{ Valeur = valeur, EtudiantId = EtudiantId, UeId = UeId, Etudiant = etudiant[0] , Ue = ue[0]};
        return await ExecuteAsync(note);
    }
    public async Task<Notes> ExecuteAsync(Notes note)
    {
        await CheckBusinessRules(note);
        Notes et = await repositoryFactory.NotesRepository().CreateAsync(note);
        repositoryFactory.NotesRepository().SaveChangesAsync().Wait();
        return et;
    }

    private async Task CheckBusinessRules(Notes note)
    {
        ArgumentNullException.ThrowIfNull(note);
        ArgumentNullException.ThrowIfNull(note.Valeur);
        ArgumentNullException.ThrowIfNull(note.EtudiantId);
        ArgumentNullException.ThrowIfNull(note.UeId);
        ArgumentNullException.ThrowIfNull(note.Etudiant);
        ArgumentNullException.ThrowIfNull(note.Ue);
        ArgumentNullException.ThrowIfNull(repositoryFactory);
        
        
        // Valeur note entre 0 et 20
        if (note.Valeur < 0 || note.Valeur > 20) throw new InvalidValueNoteException(note.Valeur + " n'est pas entre 0 et 20");
        
    }
}