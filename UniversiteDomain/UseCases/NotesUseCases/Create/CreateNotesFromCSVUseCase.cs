using System.Globalization;
using System.Text;
using CsvHelper;
using CsvHelper.Configuration;
using UniversiteDomain.DataAdapters.DataAdaptersFactory;
using UniversiteDomain.Dtos;
using UniversiteDomain.Entities;
using UniversiteDomain.Exceptions.CsvExceptions;
using UniversiteDomain.Exceptions.EtudiantExceptions;
using UniversiteDomain.Exceptions.UeExceptions;
using UniversiteDomain.UseCases.NotesUseCases.Delete;
using UniversiteDomain.UseCases.NotesUseCases.Update;

namespace UniversiteDomain.UseCases.NotesUseCases.Create;

public class CreateNotesFromCsvUseCase(IRepositoryFactory factory)
    {
        public async Task ExecuteAsync(string numeroUe, byte[] csvFile){
            var ue = await factory.UeRepository().FindByConditionAsync(e => e.NumeroUe == numeroUe);
            if (ue is { Count: 0 }) throw new UeNotFoundException(numeroUe);
            
            CreateNotesUseCase createNotesUseCase = new CreateNotesUseCase(factory);
            // J'ai ajouter les Use Case pour le changement de note et la suppression de note
            UpdateNoteUseCase updateNoteUseCase = new UpdateNoteUseCase(factory);
            DeleteNoteUseCase deleteNoteUseCase = new DeleteNoteUseCase(factory);
            
            using var stream = new MemoryStream(csvFile);
            using var reader = new StreamReader(stream, Encoding.UTF8);
            using (var csv = new CsvReader(reader, new CsvConfiguration(CultureInfo.InvariantCulture)))  
            {
                
                var records = csv.GetRecords<CsvRecord>().ToList(); 
                if (records == null || !records.Any()) throw new RecordNullException("Une erreur est survenue sur le csv");

                await ValidateRecordsAsync(records);
                
                var NBetudiants = records.Select(r => r.NumEtud).Distinct().ToList();
                var etudiants = await factory.EtudiantRepository().FindByConditionAsync(e => NBetudiants.Contains(e.NumEtud));

                if (etudiants.Count != NBetudiants.Count) {
                    throw new EtudiantNotFoundException("Some students in the CSV are not found in the system.");
                }

                // Step 6: Check existing notes for students in the current UE
                var existingNotes = await factory.NotesRepository()
                    .FindByConditionAsync(n => NBetudiants.Contains(n.Etudiant.NumEtud) && n.Ue.NumeroUe == numeroUe);
                
                foreach (var record in records)
                {
                    var etudiant = etudiants.First(e => e.NumEtud == record.NumEtud);
                    var existingNote = existingNotes.FirstOrDefault(n => n.EtudiantId == etudiant.Id && n.UeId == ue[0].Id);

                    if (existingNote != null) {
                        if (!record.Note.HasValue) {
                            await deleteNoteUseCase.ExecuteAsync(existingNote);
                            Console.WriteLine($"Note deleted for student {record.NumEtud}.");
                            continue;
                        }
                        if (existingNote.Valeur.Equals(record.Note.Value)) {
                            Console.WriteLine($"Une note existe déjà pour l'étudiant {record.NumEtud} et l'UE {numeroUe}. " +
                                              $"Et aucun changement de note, Skipping...");
                            continue;
                        }
                        existingNote.Valeur = record.Note.Value;
                        await updateNoteUseCase.ExecuteAsync(existingNote);
                        continue;
                    }
                    
                    if (!record.Note.HasValue) {
                        continue;
                    }
                    
                    Console.WriteLine($"Création de note pour {record.NumEtud}: {record.Note}");
                    
                    await createNotesUseCase.ExecuteAsync((float) record.Note.Value, etudiant.Id, ue[0].Id);
                    
                    Console.WriteLine($"Création de note réussi pour {record.NumEtud}: {record.Note}");
                }
                await factory.NotesRepository().SaveChangesAsync();
            }

        }
        
        private async Task CheckBusinessRules()
        {
            ArgumentNullException.ThrowIfNull(factory);
            ArgumentNullException.ThrowIfNull(factory.UeRepository);
            ArgumentNullException.ThrowIfNull(factory.EtudiantRepository);
        }
        
        private async Task ValidateRecordsAsync(IEnumerable<CsvRecord> records)
        {
            ArgumentNullException.ThrowIfNull(records);
            
            foreach (var record in records)
            {
                // Validate that the note is between 0 and 20
                if (record.Note < 0 || record.Note > 20)
                {
                    throw new Exception($"Invalid note for student {record.NumEtud}: {record.Note}");
                }

                // Ensure the student exists
                var etudiant = await factory.EtudiantRepository()
                    .FindByConditionAsync(e => e.NumEtud == record.NumEtud);

                if (etudiant is { Count: 0 })
                {
                    throw new EtudiantNotFoundException(record.NumEtud);
                }
            }
        }
    
        public bool IsAuthorized(string role)
        {
            return role.Equals(Roles.Scolarite);
        }
    }