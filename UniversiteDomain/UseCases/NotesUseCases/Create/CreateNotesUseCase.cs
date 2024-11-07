using UniversiteDomain.DataAdapters;
using UniversiteDomain.Entities;
using UniversiteDomain.Exceptions.NotesException;

namespace UniversiteDomain.UseCases.NotesUseCases.Create;

public class CreateNotesUseCase(INotesRepository notesRepository)
{
    public async Task<Notes> ExecuteAsync(float valeur, long EtudiantId, long UeId, Etudiant etudiant, Ue ue)
    {
        var note = new Notes{ Valeur = valeur, EtudiantId = EtudiantId, UeId = UeId, Etudiant = etudiant, Ue = ue };
        return await ExecuteAsync(note);
    }
    public async Task<Notes> ExecuteAsync(Notes note)
    {
        await CheckBusinessRules(note);
        Notes et = await notesRepository.CreateAsync(note);
        notesRepository.SaveChangesAsync().Wait();
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
        ArgumentNullException.ThrowIfNull(notesRepository);
        
        // Valeur note entre 0 et 20
        if (note.Valeur < 0 || note.Valeur > 20) throw new InvalidValueNoteException(note.Valeur + " n'est pas entre 0 et 20");
        
    }
}