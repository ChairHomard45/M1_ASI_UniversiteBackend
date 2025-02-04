using UniversiteDomain.DataAdapters.DataAdaptersFactory;
using UniversiteDomain.Entities;
using UniversiteDomain.Exceptions.UeExceptions;

namespace UniversiteDomain.UseCases.NotesUseCases.Create;

public class CreateCSVNotesUseCase(IRepositoryFactory factory)
{
    public async Task<byte[]> ExecuteAsync(string NumeroUE)
    {
        await CheckBusinessRules();
        var ue = await factory.UeRepository().FindByConditionAsync(e => e.NumeroUe == NumeroUE);
        if (ue is { Count: 0 }) throw new UeNotFoundException(NumeroUE);
        
        var etudiants = await factory.EtudiantRepository().FindEtudiantsByNumUeAsync(NumeroUE);
        
        using var stream = new MemoryStream();
        using var writer = new StreamWriter(stream);
        using var csv = new CsvHelper.CsvWriter(writer, System.Globalization.CultureInfo.InvariantCulture);
        
        // Écriture des colonnes
        csv.WriteField("NumEtud");
        csv.WriteField("Nom");
        csv.WriteField("Prenom");
        csv.WriteField("NumeroUe");
        csv.WriteField("Intitule");
        csv.WriteField("Note");
        csv.NextRecord();

        foreach (var etudiant in etudiants)
        {
            csv.WriteField(etudiant.NumEtud);
            csv.WriteField(etudiant.Nom);
            csv.WriteField(etudiant.Prenom);
            csv.WriteField(ue[0].NumeroUe);
            csv.WriteField(ue[0].Intitule);
            var note = etudiant.NotesObtenues.FirstOrDefault(e => e.Ue != null 
                                                                        && e.Ue.NumeroUe == NumeroUE)?.Valeur;
            
            csv.WriteField(note.HasValue ? note.Value.ToString() : ""); 
            //Console.WriteLine($"Entering the student {etudiant.Nom} {etudiant.Prenom}");
            csv.NextRecord();
        }
        
        writer.Flush();
        stream.Seek(0, SeekOrigin.Begin);
        return stream.ToArray();
    
    }

    private async Task CheckBusinessRules()
    {
        ArgumentNullException.ThrowIfNull(factory);
        ArgumentNullException.ThrowIfNull(factory.UeRepository);
        ArgumentNullException.ThrowIfNull(factory.EtudiantRepository);
    }
    
    public bool IsAuthorized(string role)
    {
        return role.Equals(Roles.Scolarite);
    }
}