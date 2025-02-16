using UniversiteDomain.DataAdapters;
using UniversiteDomain.DataAdapters.DataAdaptersFactory;
using UniversiteDomain.Entities;
using UniversiteDomain.Exceptions.EtudiantExceptions;
using UniversiteDomain.Exceptions.NotesException;

namespace UniversiteDomain.UseCases.NotesUseCases.Update;

public class UpdateNoteUseCase(IRepositoryFactory factory) {
        public async Task ExecuteAsync(Notes note)
        {
            await CheckBusinessRules();
        
            List<Notes> notesToUpdate = await factory.NotesRepository().FindByConditionAsync(n => n.EtudiantId == note.EtudiantId && n.UeId == note.UeId);
            if (notesToUpdate is { Count: 0 } || note is null) throw new NotesNotFoundException(note.Id.ToString());

            notesToUpdate[0].EtudiantId = note.EtudiantId;
            notesToUpdate[0].Etudiant = note.Etudiant;
            notesToUpdate[0].UeId = note.UeId;
            notesToUpdate[0].Ue = note.Ue;
            notesToUpdate[0].Valeur = note.Valeur;
            
            await factory.NotesRepository().UpdateAsync(notesToUpdate[0]);
        }
        private async Task CheckBusinessRules()
        {
            ArgumentNullException.ThrowIfNull(factory);
            INotesRepository notesRepository = factory.NotesRepository();
            ArgumentNullException.ThrowIfNull(notesRepository);
        }
    
        public bool IsAuthorized(string role)
        {
            return role.Equals(Roles.Scolarite);
        }
    }