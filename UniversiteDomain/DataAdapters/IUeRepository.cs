using UniversiteDomain.Entities;

namespace UniversiteDomain.DataAdapters;

public interface IUeRepository : IRepository<Ue>
{
    Task<Ue> AddNoteAsync(long idUe, long idNote);
    Task<Ue> AddNoteAsync(Ue ue, Notes note);
    Task<Ue> AddNoteAsync(Ue? ue, List<Notes> notes);
    Task<Ue> AddNoteAsync(long Idue, long[] Idnote);
    
    public Task<Ue?> FindUeCompletAsync(long idUe);
}