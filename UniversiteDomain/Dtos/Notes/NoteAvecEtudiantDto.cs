using UniversiteDomain.Entities;

namespace UniversiteDomain.Dtos;

public class NoteAvecEtudiantDto
{
    public long EtudiantId { get; set; }
    public long UeId { get; set; }
    public EtudiantDto EtudDto{get; set;}
    public float Valeur { get; set; }

    public NoteAvecEtudiantDto ToDto(Notes note)
    {
        EtudiantId = note.EtudiantId;
        UeId = note.UeId;
        EtudDto = new EtudiantDto().ToDto(note.Etudiant);
        Valeur = note.Valeur;
        return this;
    }
    
    public Notes ToEntity()
    {
        return new Notes {EtudiantId = this.EtudiantId, UeId = this.UeId, Valeur = this.Valeur};
    }
    
    public static List<NoteAvecEtudiantDto> ToDtos(List<Notes> notes)
    {
        List<NoteAvecEtudiantDto> dtos = new();
        foreach (var note in notes)
        {
            dtos.Add(new NoteAvecEtudiantDto().ToDto(note));
        }
        return dtos;
    }

    public static List<Notes> ToEntities(List<NoteAvecEtudiantDto> noteDtos)
    {
        List<Notes> notes = new();
        foreach (var noteDto in noteDtos)
        {
            notes.Add(noteDto.ToEntity());
        }

        return notes;
    }
}