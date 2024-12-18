using Microsoft.EntityFrameworkCore;
using UniversiteDomain.DataAdapters;
using UniversiteDomain.Entities;
using UniversiteEFDataProvider.Data;

namespace UniversiteEFDataProvider.Repositories;

public class UeRepository(UniversiteDbContext context) : Repository<Ue>(context), IUeRepository
{
    /* ------ Normal Version ------*/
    public async Task<Ue> AddNoteAsync(long idUe, long idNote)
    {
        ArgumentNullException.ThrowIfNull(Context.Ues);
        ArgumentNullException.ThrowIfNull(Context.Notes);
        Ue u = (await Context.Ues.FindAsync(idUe))!;
        Notes n = (await Context.Notes.FindAsync(idNote))!;
        u.Notes.Add(n);
        await Context.SaveChangesAsync();
        return u;
    }

    public async Task<Ue> AddNoteAsync(Ue ue, Notes note)
    {
        return await AddNoteAsync(ue.Id, note.Id);
    }
    
    /* ------ Lists Version ------*/

    public async Task<Ue> AddNoteAsync(long Idue, long[] Idnote)
    {
        ArgumentNullException.ThrowIfNull(Context.Ues);
        ArgumentNullException.ThrowIfNull(Context.Notes);
        
        Ue e = (await Context.Ues.FindAsync(Idue))!;
        List<Notes> notes = new List<Notes>();

        foreach (var id in Idnote)
        {
            Notes note = (await Context.Notes.FindAsync(id))!;
            notes.Add(note);
        }

        e.Notes.AddRange(notes);
        await Context.SaveChangesAsync();
        return e;
    }
    
    public async Task<Ue> AddNoteAsync(Ue? ue, List<Notes> notes)
    {
        long[] noteIds = notes.Select(n => n.Id).ToArray();
        return await AddNoteAsync(ue.Id, noteIds);
    }
    
    public async Task<Ue?> FindUeCompletAsync(long idUe)
    {
        ArgumentNullException.ThrowIfNull(Context.Ues);
        return await Context.Ues.Include(e => e.EnseigneeDans).Include(e => e.Notes).ThenInclude(ue => ue.Etudiant).FirstOrDefaultAsync(e => e.Id == idUe);
    }
}