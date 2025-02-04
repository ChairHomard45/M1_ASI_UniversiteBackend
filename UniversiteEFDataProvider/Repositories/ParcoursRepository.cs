using Microsoft.EntityFrameworkCore;
using UniversiteDomain.DataAdapters;
using UniversiteDomain.Entities;
using UniversiteEFDataProvider.Data;

namespace UniversiteEFDataProvider.Repositories;

public class ParcoursRepository(UniversiteDbContext context) : Repository<Parcours>(context), IParcoursRepository
{
    
    /* ------ Normal Version (AddEtudiant) ------*/
    public async Task<Parcours> AddEtudiantAsync(long idParcours, long idEtudiant)
    {
        ArgumentNullException.ThrowIfNull(Context.Etudiants);
        ArgumentNullException.ThrowIfNull(Context.Parcours);
        Etudiant e = (await Context.Etudiants.FindAsync(idEtudiant))!;
        Parcours p = (await Context.Parcours.FindAsync(idParcours))!;
        p.Inscrits.Add(e);
        await Context.SaveChangesAsync();
        return p;
    }
    
    public async Task<Parcours> AddEtudiantAsync(Parcours parcours, Etudiant etudiant)
    {
        return await AddEtudiantAsync(parcours.Id, etudiant.Id);
    }
    
    /* ------ Lists Version (AddEtudiant) ------*/
    public async Task<Parcours> AddEtudiantAsync(long idParcours, long[] idEtudiants)
    {
        ArgumentNullException.ThrowIfNull(Context.Etudiants);
        ArgumentNullException.ThrowIfNull(Context.Parcours);

        Parcours p = (await Context.Parcours.FindAsync(idParcours))!;
        List<Etudiant> notes = new List<Etudiant>();

        foreach (var id in idEtudiants)
        {
            Etudiant etud = (await Context.Etudiants.FindAsync(id))!;
            notes.Add(etud);
        }

        p.Inscrits.AddRange(notes);
        await Context.SaveChangesAsync();
        return p;
    }
    
    public async Task<Parcours> AddEtudiantAsync(Parcours? parcours, List<Etudiant> etudiants)
    {
        long[] idEtudiants = etudiants.Select(n => n.Id).ToArray();
        return await AddEtudiantAsync(parcours.Id, idEtudiants);
    }
    
    /* ------ Normal Version (AddUe) ------*/
    public async Task<Parcours> AddUeAsync(long idParcours, long idUe)
    {
        ArgumentNullException.ThrowIfNull(Context.Parcours);
        ArgumentNullException.ThrowIfNull(Context.Ues);
        Parcours p = (await Context.Parcours.FindAsync(idParcours))!;
        Ue ue = (await Context.Ues.FindAsync(idUe))!;
        
        p.UesEnseignees.Add(ue);
        await Context.SaveChangesAsync();
        return p;
    }

    public async Task<Parcours> AddUeAsync(Parcours parcours, Ue ue)
    {
        return await AddUeAsync(parcours.Id, ue.Id);
    }

    /* ------ Lists Version (AddUe) ------*/
    public async Task<Parcours> AddUeAsync(long idParcours, long[] idUe)
    {
        ArgumentNullException.ThrowIfNull(Context.Parcours);
        ArgumentNullException.ThrowIfNull(Context.Ues);

        Parcours e = (await Context.Parcours.FindAsync(idParcours))!;
        List<Ue> ues = new List<Ue>();

        foreach (var id in idUe)
        {
            Ue ue = (await Context.Ues.FindAsync(id))!;
            ues.Add(ue);
        }

        e.UesEnseignees.AddRange(ues);
        await Context.SaveChangesAsync();
        return e;
    }
    
    public async Task<Parcours> AddUeAsync(Parcours? parcours, List<Ue> ues)
    {
        long[] ueIds = ues.Select(n => n.Id).ToArray();
        return await AddUeAsync(parcours.Id, ueIds);
    }
    
    
    public async Task<Parcours?> FindParcoursCompletAsync(long idParcours)
    {
        ArgumentNullException.ThrowIfNull(Context.Parcours);
        return await Context.Parcours.Include(p => p.Inscrits).Include(t => t.UesEnseignees).FirstOrDefaultAsync(p => p.Id == idParcours);
    }
}