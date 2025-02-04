using UniversiteDomain.Entities;

namespace UniversiteDomain.Dtos;

public class ParcoursCompletDto
{
    public long Id { get; set; }
    public string NomParcours { get; set; }
    public int AnneeFormation { get; set; }
    
    public List<EtudiantDto> Etudiants { get; set; }
    
    public List<UeDto> Ues { get; set; }

    public ParcoursCompletDto ToDto(Parcours parcours)
    {
        this.Id = parcours.Id;
        this.NomParcours = parcours.NomParcours;
        this.AnneeFormation = parcours.AnneeFormation;

        if (parcours.UesEnseignees != null)
        {
            this.Etudiants = EtudiantDto.ToDtos(parcours.Inscrits);
        }
        
        if (parcours.UesEnseignees != null)
        {
            this.Ues = UeDto.ToDtos(parcours.UesEnseignees);
        }
        
        return this;
    }
    
    public static List<ParcoursCompletDto> ToDtos(List<Parcours> parcours)
    {
        List<ParcoursCompletDto> dtos = new();
        foreach (var parcour in parcours)
        {
            dtos.Add(new ParcoursCompletDto().ToDto(parcour));
        }
        return dtos;
    }
    
    public Parcours ToEntity()
    {
        return new Parcours {Id = this.Id, NomParcours = this.NomParcours, AnneeFormation = this.AnneeFormation};
    }
}