using UniversiteDomain.Entities;

namespace UniversiteDomain.Dtos;

public class UeCompletDto
{
    public long Id { get; set; }
    public string NumeroUe { get; set; }
    public string Intitule { get; set; }
    
    public List<ParcoursDto> Parcourses { get; set; }
    
    public List<NoteAvecEtudiantDto> NoteAvecEtudiants { get; set; }

    public UeCompletDto ToDto(Ue ue)
    {
        this.Id = ue.Id;
        this.NumeroUe = ue.NumeroUe;
        this.Intitule = ue.Intitule;
        if (ue.EnseigneeDans != null)
        {
            this.Parcourses = ParcoursDto.ToDtos(ue.EnseigneeDans);
        }

        if (ue.Notes != null)
        {
            this.NoteAvecEtudiants = NoteAvecEtudiantDto.ToDtos(ue.Notes);
        }

        return this;
    }
    
    public Ue ToEntity()
    {
        return new Ue {Id = this.Id, NumeroUe = this.NumeroUe, Intitule = this.Intitule};
    }
}