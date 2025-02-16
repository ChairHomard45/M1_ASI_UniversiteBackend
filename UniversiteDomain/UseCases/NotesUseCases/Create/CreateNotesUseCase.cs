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
        var existingNotes = await repositoryFactory.NotesRepository()
            .FindByConditionAsync(n => n.EtudiantId == EtudiantId && n.UeId == UeId);

        if (existingNotes.Any()) // If a note already exists, update instead of creating
        {
            var existingNote = existingNotes.First();
            existingNote.Valeur = valeur;
            await repositoryFactory.NotesRepository().UpdateAsync(existingNote);
            return existingNote;
        }

        // If no existing note, proceed with creating a new one
        var ue = await repositoryFactory.UeRepository().FindByConditionAsync(e => e.Id == UeId);
        if (!ue.Any()) throw new UeNotFoundException(UeId.ToString());

        var etudiant = await repositoryFactory.EtudiantRepository().FindByConditionAsync(e => e.Id == EtudiantId);
        if (!etudiant.Any()) throw new EtudiantNotFoundException(EtudiantId.ToString());

        var note = new Notes
        {
            Valeur = valeur,
            EtudiantId = EtudiantId,
            UeId = UeId,
            Etudiant = etudiant.First(),
            Ue = ue.First()
        };
        
        await repositoryFactory.NotesRepository().CreateAsync(note);
        return note;
    }

    public async Task<Notes> ExecuteAsync(Notes note)
    {
        await CheckBusinessRules(note);
        Notes et = await repositoryFactory.NotesRepository().CreateAsync(note);
        
        var ue = await repositoryFactory.UeRepository().FindByConditionAsync(e => e.Id == note.UeId);
        var etudiant = await repositoryFactory.EtudiantRepository().FindByConditionAsync(e => e.Id == note.EtudiantId);

        if (ue.Any() && etudiant.Any())
        {
            var ueEntity = ue.First();
            var etudiantEntity = etudiant.First();

            ueEntity.Notes.Add(et);
            etudiantEntity.NotesObtenues.Add(et);

            await repositoryFactory.UeRepository().UpdateAsync(ueEntity);
            await repositoryFactory.EtudiantRepository().UpdateAsync(etudiantEntity);
        }
        
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
    
    public bool IsAuthorized(string role)
    {
        return role.Equals(Roles.Scolarite);
    }
}