using UniversiteDomain.DataAdapters.DataAdaptersFactory;
using UniversiteDomain.Entities;
using UniversiteDomain.Exceptions.EtudiantExceptions;
using UniversiteDomain.Exceptions.NotesException;
using UniversiteDomain.Exceptions.ParcoursExceptions;
using UniversiteDomain.Exceptions.UeExceptions;

namespace UniversiteDomain.UseCases.UeUseCases.NotesDansUe;

public class AddNotesDansUeUseCase(IRepositoryFactory repositoryFactory)
{
    public async Task<Ue> ExecuteAsync(Ue ue, Notes note)
    {
        ArgumentNullException.ThrowIfNull(ue);
        ArgumentNullException.ThrowIfNull(note);
        return await ExecuteAsync(ue.Id, note.Id); 
    }  
    
    public async Task<Ue> ExecuteAsync(long idUe, long idNote)
    {
        await CheckBusinessRules(idUe, idNote); 
        return await repositoryFactory.UeRepository().AddNoteAsync(idUe, idNote);
    }
    
    public async Task<Ue> ExecuteAsync(Ue ue, List<Notes> notes)
    {
        ArgumentNullException.ThrowIfNull(ue);
        ArgumentNullException.ThrowIfNull(notes);
        long[] idnotes = notes.Select(x => x.Id).ToArray();
        return await ExecuteAsync(ue.Id, idnotes);
    }  

    public async Task<Ue> ExecuteAsync(long idetudiant, long [] idNotes)
    {
        foreach(var id in idNotes) await CheckBusinessRules(idetudiant, id);
        return await repositoryFactory.UeRepository().AddNoteAsync(idetudiant, idNotes);
    }   

    private async Task CheckBusinessRules(long idUe, long idNote)
    {
        // Vérification des paramètres
        ArgumentNullException.ThrowIfNull(idUe);
        ArgumentNullException.ThrowIfNull(idNote);
        
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(idUe);
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(idNote);
        
        // Vérifions tout d'abord que nous sommes bien connectés aux datasources
        ArgumentNullException.ThrowIfNull(repositoryFactory);
        ArgumentNullException.ThrowIfNull(repositoryFactory.EtudiantRepository());
        ArgumentNullException.ThrowIfNull(repositoryFactory.UeRepository());
        ArgumentNullException.ThrowIfNull(repositoryFactory.NotesRepository());
        ArgumentNullException.ThrowIfNull(repositoryFactory.ParcoursRepository());
        
        // On recherche l'Ue
        List<Ue> ue = await repositoryFactory.UeRepository().FindByConditionAsync(e=>e.Id.Equals(idUe));
        if (ue is { Count: 0 }) throw new UeNotFoundException(idUe.ToString());
        
        // On recherche la note
        List<Notes> note = await repositoryFactory.NotesRepository().FindByConditionAsync(e=>e.Id.Equals(idNote));
        if (note is { Count: 0 }) throw new NotesNotFoundException(idNote.ToString());
        
        // On recherche l'Etudiant associé a la note 
        List<Etudiant> etudiant = await repositoryFactory.EtudiantRepository().FindByConditionAsync(e => e.Id.Equals(note[0].EtudiantId));
        if (etudiant is { Count: 0 }) throw new EtudiantNotFoundException(note[0].EtudiantId.ToString());
        
        // On vérifie si L'ue est dans le parcours de l'etudiant
        List<Parcours> parcours = await repositoryFactory.ParcoursRepository().FindByConditionAsync(e => e.UesEnseignees.Equals(idUe) && e.Id.Equals(etudiant[0].ParcoursSuivi.Id));
        if (parcours is { Count: 0 }) throw new ParcoursNotFoundException(idUe.ToString());
        
        // Vérifie si la note existe déjà dans la liste de l'ue
        /*
        if (ue[0].Notes != null)
        {
            var trouve= ue[0].Notes.FindAll(e=>e.Id.Equals(note[0].Id));

            if (trouve.Count() != 0) {
                etudiant[0].Notes.RemoveAll(e => e.Id.Equals(note[0].Id));
                ue[0].Notes.RemoveAll(e => e.Id.Equals(note[0].Id));
            }
        }*/
        
    }

}