using System.Linq.Expressions;
using Moq;
using UniversiteDomain.DataAdapters;
using UniversiteDomain.DataAdapters.DataAdaptersFactory;
using UniversiteDomain.Entities;
using UniversiteDomain.UseCases.ParcoursUseCases.Create;
using UniversiteDomain.UseCases.ParcoursUseCases.EtudiantDansParcours;
using UniversiteDomain.UseCases.ParcoursUseCases.UeDansParcours;

namespace UniversiteDomainUnitTests;

public class ParcoursUnitTest
{
    [SetUp]
    public void Setup()
    {
    }

    [Test]
    public async Task CreateParcoursUseCase()
    {
        long idParcours = 1;
        String nomParcours = "Ue 1";
        int anneFormation = 2;
        
        // On crée le Parcours qui doit être ajouté en base
        Parcours ParcoursSansId = new Parcours{NomParcours= nomParcours, AnneeFormation = anneFormation};
        
        //  Créons le mock du repository
        // On initialise une fausse datasource qui va simuler un ParcoursRepository
        var mock = new Mock<IParcoursRepository>();
        
        // Il faut ensuite aller dans le use case pour voir quelles fonctions simuler
        // Nous devons simuler FindByCondition et Create
        
        // Simulation de la fonction FindByCondition
        // On dit à ce mock que l'étudiant n'existe pas déjà
        // La réponse à l'appel FindByCondition est donc une liste vide
        var reponseFindByCondition = new List<Parcours>();
        
        // On crée un bouchon dans le mock pour la fonction FindByCondition
        // Quelque soit le paramètre de la fonction FindByCondition, on renvoie la liste vide
        mock
            .Setup(repo=>repo.FindByConditionAsync(It.IsAny<Expression<Func<Parcours, bool>>>()))
            .ReturnsAsync(reponseFindByCondition);
        
        // Simulation de la fonction Create
        // On lui dit que l'ajout d'un Parcours renvoie un Parcours avec l'Id 1
        Parcours parcoursCree =new Parcours{Id=idParcours, NomParcours = nomParcours, AnneeFormation = anneFormation};
        mock.Setup(repoEtudiant=>repoEtudiant.CreateAsync(ParcoursSansId)).ReturnsAsync(parcoursCree);
        
        // On crée le bouchon (un faux etudiantRepository). Il est prêt à être utilisé
        var mockFactory = new Mock<IRepositoryFactory>();
        mockFactory.Setup(facto=>facto.ParcoursRepository()).Returns(mock.Object);
        
        // Création du use case en injectant notre faux repository
        CreateParcoursUseCase useCase=new CreateParcoursUseCase(mockFactory.Object);
        // Appel du use case
        var etudiantTeste=await useCase.ExecuteAsync(ParcoursSansId);
        
        // Vérification du résultat
        Assert.That(etudiantTeste.Id, Is.EqualTo(parcoursCree.Id));
        Assert.That(etudiantTeste.NomParcours, Is.EqualTo(parcoursCree.NomParcours));
        Assert.That(etudiantTeste.AnneeFormation, Is.EqualTo(parcoursCree.AnneeFormation));
    }
    
    [Test]
    public async Task AddEtudiantDansParcoursUseCase()
    {
        long idEtudiant = 1;
        long idParcours = 3;
        
        Etudiant etudiant= new Etudiant { Id = 1, NumEtud = "1", Nom = "nom1", Prenom = "prenom1", Email = "1" };
        Parcours parcours = new Parcours{Id=3, NomParcours = "Ue 3", AnneeFormation = 1};
        
        // On initialise des faux repositories
        var mockEtudiant = new Mock<IEtudiantRepository>();
        var mockParcours = new Mock<IParcoursRepository>();
        
        List<Etudiant> etudiants = new List<Etudiant>();
        
        etudiants.Add(new Etudiant{Id=1});
        
        mockEtudiant
            .Setup(repo=>repo.FindByConditionAsync(e=>e.Id.Equals(idEtudiant)))
            .ReturnsAsync(etudiants);

        List<Parcours> parcourses = new List<Parcours>();
        parcourses.Add(parcours);
        
        List<Parcours> parcoursFinaux = new List<Parcours>();
        Parcours parcoursFinal = parcours;
        
        parcoursFinal.Inscrits.Add(etudiant);
        parcoursFinaux.Add(parcours);
        
        mockParcours
            .Setup(repo=>repo.FindByConditionAsync(e=>e.Id.Equals(idParcours)))
            .ReturnsAsync(parcourses);
        mockParcours
            .Setup(repo => repo.AddEtudiantAsync(idParcours, idEtudiant))
            .ReturnsAsync(parcoursFinal);
        
        Console.WriteLine(parcoursFinal.Inscrits.Count);
        
        // Création d'une fausse factory qui contient les faux repositories
        var mockFactory = new Mock<IRepositoryFactory>();
        mockFactory.Setup(facto=>facto.EtudiantRepository()).Returns(mockEtudiant.Object);
        mockFactory.Setup(facto=>facto.ParcoursRepository()).Returns(mockParcours.Object);
        
        // Création du use case en utilisant le mock comme datasource
        AddEtudiantDansParcoursUseCase useCase=new AddEtudiantDansParcoursUseCase(mockFactory.Object);
        
        // Appel du use case
        var parcoursTest=await useCase.ExecuteAsync(idParcours, idEtudiant);
        // Vérification du résultat
        Assert.That(parcoursTest.Id, Is.EqualTo(parcoursFinal.Id));
        Assert.That(parcoursTest.Inscrits, Is.Not.Null);
        Assert.That(parcoursTest.Inscrits.Count, Is.EqualTo(1));
        Assert.That(parcoursTest.Inscrits[0].Id, Is.EqualTo(idEtudiant));
    }

    [Test]
    public async Task AddUeDansParcoursUseCase()
    {
        long idUe = 1;
        long idParcours = 3;
        
        Ue ue = new Ue { Id = idUe, NumeroUe = "14", Intitule = "15"};
        Parcours parcours = new Parcours{Id=idParcours, NomParcours = "Ue 3", AnneeFormation = 1};

        
        // On initialise des faux repositories
        var mockUe = new Mock<IUeRepository>();
        var mockParcours = new Mock<IParcoursRepository>();
        
        
        mockUe
            .Setup(repo=>repo.FindByConditionAsync(e => e.Id.Equals(idUe)))
            .ReturnsAsync(new List<Ue> { ue });

        mockParcours
            .Setup(repo => repo.FindByConditionAsync(e => e.Id.Equals(idParcours)))
            .ReturnsAsync(new List<Parcours> { parcours });
        
        Parcours parcoursFinal = new Parcours{Id=idParcours, NomParcours = "Ue 3", AnneeFormation = 1};
        
        parcoursFinal.UesEnseignees = new List<Ue> { ue };
        

        mockParcours
            .Setup(repo => repo.AddUeAsync(idParcours, idUe))
            .ReturnsAsync(parcoursFinal);
        
        var mockFactory = new Mock<IRepositoryFactory>();
        mockFactory.Setup(facto=>facto.UeRepository()).Returns(mockUe.Object);
        mockFactory.Setup(facto=>facto.ParcoursRepository()).Returns(mockParcours.Object);
        
        AddUeDansParcoursUseCase useCase=new AddUeDansParcoursUseCase(mockFactory.Object);
        
        var parcoursTest=await useCase.ExecuteAsync(idParcours,  idUe);
        
        Assert.That(parcoursTest.Id, Is.EqualTo(parcoursFinal.Id));
        Assert.That(parcoursTest.UesEnseignees, Is.Not.Null);
        Assert.That(parcoursTest.UesEnseignees.Count, Is.EqualTo(1));
        Assert.That(parcoursTest.UesEnseignees[0].Id, Is.EqualTo(idUe));

    }
}