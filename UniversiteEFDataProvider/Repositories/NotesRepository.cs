using UniversiteDomain.DataAdapters;
using UniversiteDomain.Entities;
using UniversiteEFDataProvider.Data;

namespace UniversiteEFDataProvider.Repositories;

public class NotesRepository(UniversiteDbContext context) : Repository<Notes>(context), INotesRepository
{
    public async Task AffecterEtudiantUeAsync(long idNote,long idEtudiant, long idUe)
    {
        ArgumentNullException.ThrowIfNull(Context.Notes);
        ArgumentNullException.ThrowIfNull(Context.Etudiants);
        ArgumentNullException.ThrowIfNull(Context.Ues);
        Etudiant e = (await Context.Etudiants.FindAsync(idEtudiant))!;
        Ue u = (await Context.Ues.FindAsync(idUe))!;
        Notes p = (await Context.Notes.FindAsync(idNote))!;
        p.Etudiant = e;
        p.Ue = u;
        await Context.SaveChangesAsync();
    }
    
    public async Task AffecterEtudiantUeAsync(Notes note, Etudiant etudiant, Ue ues)
    {
        await AffecterEtudiantUeAsync(note.Id,etudiant.Id, ues.Id); 
    }
}