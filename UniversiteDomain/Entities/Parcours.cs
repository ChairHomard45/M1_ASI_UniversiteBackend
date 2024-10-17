namespace UniversiteDomain.Entities;

public class Parcours
{
    public long Id { get; set; }
    public string NomParcours { get; set; } = string.Empty;
    public int AnneeFormation { get; set; } = 1;
    
    public override string ToString()
    {
        return $"ID {Id} : {NomParcours} - {AnneeFormation}"/*+ParcoursSuivi*/;
    }
}