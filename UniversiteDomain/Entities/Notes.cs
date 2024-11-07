namespace UniversiteDomain.Entities;

public class Notes
{
    public long Id { get; set; }
    public float Valeur { get; set; }
    
    // Clés étrangères
    public long EtudiantId { get; set; }
    public long UeId { get; set; }
    
    // Propriétés de navigation
    public Etudiant? Etudiant { get; set; }
    public Ue? Ue { get; set; }
    
    public override string ToString()
    {
        return $"Value {Valeur} ";
    }
}