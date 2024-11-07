using UniversiteDomain.DataAdapters.DataAdaptersFactory;
using UniversiteDomain.Entities;
using UniversiteDomain.Exceptions.EtudiantExceptions;
using UniversiteDomain.Exceptions.NotesException;
using UniversiteDomain.Exceptions.UeExceptions;

namespace UniversiteDomain.UseCases.EtudiantUseCases.NotesDansEtudiant;

public class AddNotesDansEtudiantUseCase(IRepositoryFactory repositoryFactory)
{
    public async Task<Etudiant> ExecuteAsync(Etudiant etudiant, Notes note)
    {
        ArgumentNullException.ThrowIfNull(etudiant);
        ArgumentNullException.ThrowIfNull(note);
        return await ExecuteAsync(etudiant.Id, note.Id); 
    }  
    
    public async Task<Etudiant> ExecuteAsync(long idEtudiant, long idNote)
    {
        await CheckBusinessRules(idEtudiant, idNote); 
        return await repositoryFactory.EtudiantRepository().AddNoteAsync(idEtudiant, idNote);
    }
    
    public async Task<Etudiant> ExecuteAsync(Etudiant etudiant, List<Notes> notes)
    {
        ArgumentNullException.ThrowIfNull(etudiant);
        ArgumentNullException.ThrowIfNull(notes);
        long[] idnotes = notes.Select(x => x.Id).ToArray();
        return await ExecuteAsync(etudiant.Id, idnotes);
    }  

    public async Task<Etudiant> ExecuteAsync(long idetudiant, long [] idNotes)
    {
        foreach (var id in idNotes) await CheckBusinessRules(idetudiant, id);
        return await repositoryFactory.EtudiantRepository().AddNoteAsync(idetudiant, idNotes);
    }   

    private async Task CheckBusinessRules(long idEtudiant, long idNote)
    {
        // Vérification des paramètres
        ArgumentNullException.ThrowIfNull(idEtudiant);
        ArgumentNullException.ThrowIfNull(idNote);
        
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(idEtudiant);
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(idNote);
        
        // Vérifions tout d'abord que nous sommes bien connectés aux datasources
        ArgumentNullException.ThrowIfNull(repositoryFactory);
        ArgumentNullException.ThrowIfNull(repositoryFactory.EtudiantRepository());
        ArgumentNullException.ThrowIfNull(repositoryFactory.UeRepository());
        ArgumentNullException.ThrowIfNull(repositoryFactory.NotesRepository());
        
        // On recherche l'étudiant
        List<Etudiant> etudiant = await repositoryFactory.EtudiantRepository().FindByConditionAsync(e=>e.Id.Equals(idEtudiant));
        if (etudiant is { Count: 0 }) throw new EtudiantNotFoundException(idEtudiant.ToString());
        
        // On recherche la note
        List<Notes> note = await repositoryFactory.NotesRepository().FindByConditionAsync(e=>e.Id.Equals(idNote));
        if (note is { Count: 0 }) throw new NotesNotFoundException(idNote.ToString());
        
        // On recherche l'Ue associé a la note && On vérifie si l'ue est dans le parcours de l'etudiant
        List<Ue> ue = await repositoryFactory.UeRepository().FindByConditionAsync(e => e.Id.Equals(note[0].UeId) && e.EnseigneeDans.Equals(etudiant[0].ParcoursSuivi.Id));
        if (ue is { Count: 0 }) throw new UeNotFoundException(note[0].UeId.ToString());
        
        // Vérifie si la note existe déjà dans la liste de l'etudiant
        if (etudiant[0].Notes != null)
        {
            var trouve= etudiant[0].Notes.FindAll(e=>e.Id.Equals(note[0].Id));

            if (trouve.Count() != 0) {
                etudiant[0].Notes.RemoveAll(e => e.Id.Equals(note[0].Id));
                ue[0].Notes.RemoveAll(e => e.Id.Equals(note[0].Id));
            }
        }
    }

}