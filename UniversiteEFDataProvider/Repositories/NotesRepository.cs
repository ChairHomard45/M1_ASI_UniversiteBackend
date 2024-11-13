using UniversiteDomain.DataAdapters;
using UniversiteDomain.Entities;
using UniversiteEFDataProvider.Data;

namespace UniversiteEFDataProvider.Repositories;

public class NotesRepository(UniversiteDbContext context) : Repository<Notes>(context), INotesRepository
{
    
}