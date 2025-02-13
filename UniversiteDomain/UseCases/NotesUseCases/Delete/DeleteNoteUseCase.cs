using UniversiteDomain.DataAdapters;
using UniversiteDomain.DataAdapters.DataAdaptersFactory;
using UniversiteDomain.Entities;

namespace UniversiteDomain.UseCases.NotesUseCases.Delete;

public class DeleteNoteUseCase(IRepositoryFactory factory)
{
    public async Task ExecuteAsync(Notes note)
    {
        await CheckBusinessRules(note);
        await factory.NotesRepository().DeleteAsync(note);
        await factory.NotesRepository().SaveChangesAsync();
    }
    private async Task CheckBusinessRules(Notes note)
    {
        ArgumentNullException.ThrowIfNull(note);
        ArgumentNullException.ThrowIfNull(factory);
        INotesRepository notesRepository = factory.NotesRepository();
        ArgumentNullException.ThrowIfNull(notesRepository);
    }
    
    public bool IsAuthorized(string role)
    {
        return role.Equals(Roles.Scolarite);
    }
}