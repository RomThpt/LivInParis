1.  """

    You are an expert in C#.

    ***

    This is your mission:
    Implement this to the code:
    Vous proposerez ensuite un outil de visualisation du graphe en C#
    Pour l'outil de visualisation, ou d'autres librairies comme SkiaSharp.(using SkiaSharp;)
    Pour résoudre les problèmes de visualisation, vous pouvez vous aider des outils d'IA générative, vous devrez être en mesure d'expliquer le fonctionnement de l'un ou l'autre des modes et de fournir les prompts soumis.

    ***

    Here is the code:
    Project Path: LivInParis

    Source Tree:

    ```
    LivInParis
    ├── Models
    │   ├── RepresentationMode.cs
    │   ├── Lien.cs
    │   ├── Graphe.cs
    │   └── Noeud.cs
    ├── LivInParis.csproj
    └── Program.cs
    ```

    `Models/RepresentationMode.cs`:

    ```cs
    namespace LivInParis.Models;

    public enum RepresentationMode
    {
        Liste,
        Matrice
    }
    ```

    `Models/Lien.cs`:

    ```cs
    namespace LivInParis.Models;

    public class Lien
    {
        public Noeud Noeud1 { get; private set; }
        public Noeud Noeud2 { get; private set; }

        public Lien(Noeud n1, Noeud n2)
        {
            Noeud1 = n1;
            Noeud2 = n2;
        }

        public override bool Equals(object? obj)
        {
            if (obj is Lien autre)
            {
                return (Noeud1.Id == autre.Noeud1.Id && Noeud2.Id == autre.Noeud2.Id) ||
                    (Noeud1.Id == autre.Noeud2.Id && Noeud2.Id == autre.Noeud1.Id);
            }
            return false;
        }

        public override int GetHashCode()
        {
            int min = Math.Min(Noeud1.Id, Noeud2.Id);
            int max = Math.Max(Noeud1.Id, Noeud2.Id);
            return HashCode.Combine(min, max);
        }
    }
    ```

    `Models/Graphe.cs`:

    ```cs
    namespace LivInParis.Models;

    public class Graphe
    {
        public Dictionary<int, Noeud> Noeuds { get; private set; }
        public List<Lien> Liens { get; private set; }
        public bool[,]? MatriceAdjacence { get; private set; }
        public RepresentationMode Mode { get; private set; }

        public Graphe(RepresentationMode mode)
        {
            Noeuds = new Dictionary<int, Noeud>();
            Liens = new List<Lien>();
            Mode = mode;
        }

        // ... (rest of implementation following the provided code)

        public void AjouterNoeud(int id)
        {
            if (!Noeuds.ContainsKey(id))
            {
                Noeuds.Add(id, new Noeud(id));
            }
        }

        public void AjouterLien(int id1, int id2)
        {
            AjouterNoeud(id1);
            AjouterNoeud(id2);

            Noeud n1 = Noeuds[id1];
            Noeud n2 = Noeuds[id2];

            Lien nouveauLien = new Lien(n1, n2);

            if (!Liens.Contains(nouveauLien))
            {
                Liens.Add(nouveauLien);
                n1.AjouterLien(nouveauLien);
                n2.AjouterLien(nouveauLien);
            }
        }

        public void ParcoursLargeur(int depart)
        {
            if (!Noeuds.ContainsKey(depart))
                return;

            HashSet<int> visites = new HashSet<int>();
            Queue<int> file = new Queue<int>();

            file.Enqueue(depart);
            visites.Add(depart);

            Console.WriteLine("Parcours en largeur:");
            while (file.Count > 0)
            {
                int noeudCourant = file.Dequeue();
                Console.Write($"{noeudCourant} ");

                foreach (var voisin in Noeuds[noeudCourant].Voisins)
                {
                    if (!visites.Contains(voisin.Id))
                    {
                        file.Enqueue(voisin.Id);
                        visites.Add(voisin.Id);
                    }
                }
            }
            Console.WriteLine();
        }

        public void ParcoursProfondeur(int depart)
        {
            HashSet<int> visites = new HashSet<int>();
            Console.WriteLine("Parcours en profondeur:");
            ParcoursProfondeurRecursif(depart, visites);
            Console.WriteLine();
        }

        private void ParcoursProfondeurRecursif(int noeud, HashSet<int> visites)
        {
            if (!Noeuds.ContainsKey(noeud) || visites.Contains(noeud))
                return;

            visites.Add(noeud);
            Console.Write($"{noeud} ");

            foreach (var voisin in Noeuds[noeud].Voisins)
            {
                ParcoursProfondeurRecursif(voisin.Id, visites);
            }
        }

        public bool EstConnexe()
        {
            if (Noeuds.Count == 0)
                return true;

            HashSet<int> visites = new HashSet<int>();
            ParcoursProfondeurRecursif(Noeuds.Keys.First(), visites);

            return visites.Count == Noeuds.Count;
        }

        public bool ContientCycle()
        {
            HashSet<int> visites = new HashSet<int>();
            HashSet<int> recursionStack = new HashSet<int>();

            foreach (var noeud in Noeuds.Keys)
            {
                if (DetecteCycleRecursif(noeud, visites, recursionStack))
                    return true;
            }

            return false;
        }

        private bool DetecteCycleRecursif(int noeud, HashSet<int> visites, HashSet<int> recursionStack)
        {
            if (recursionStack.Contains(noeud))
                return true;

            if (visites.Contains(noeud))
                return false;

            visites.Add(noeud);
            recursionStack.Add(noeud);

            foreach (var voisin in Noeuds[noeud].Voisins)
            {
                if (DetecteCycleRecursif(voisin.Id, visites, recursionStack))
                    return true;
            }

            recursionStack.Remove(noeud);
            return false;
        }

        public void AfficherProprietes()
        {
            Console.WriteLine("\nPropriétés du graphe:");
            Console.WriteLine($"Nombre de noeuds: {Noeuds.Count}");
            Console.WriteLine($"Nombre de liens: {Liens.Count}");
            Console.WriteLine($"Mode de représentation: {Mode}");
        }
    }
    ```

    `Models/Noeud.cs`:

    ```cs
    namespace LivInParis.Models;

    public class Noeud
    {
        public int Id { get; private set; }
        public List<Lien> Liens { get; private set; }

        public Noeud(int id)
        {
            Id = id;
            Liens = new List<Lien>();
        }

        public void AjouterLien(Lien lien)
        {
            if (!Liens.Contains(lien))
            {
                Liens.Add(lien);
            }
        }

        public List<Noeud> Voisins
        {
            get
            {
                List<Noeud> voisins = new List<Noeud>();
                foreach (var lien in Liens)
                {
                    if (lien.Noeud1 == this)
                        voisins.Add(lien.Noeud2);
                    else
                        voisins.Add(lien.Noeud1);
                }
                return voisins;
            }
        }
    }
    ```

    `LivInParis.csproj`:

    ```csproj
    <Project Sdk="Microsoft.NET.Sdk">

    <PropertyGroup>
        <OutputType>Exe</OutputType>
        <TargetFramework>net8.0</TargetFramework>
        <ImplicitUsings>enable</ImplicitUsings>
        <Nullable>enable</Nullable>
    </PropertyGroup>

    </Project>
    ```

    `Program.cs`:

    ```cs
    using LivInParis.Models;

    Console.WriteLine("Graphe en mode Liste d'adjacence:");
    Graphe grapheListe = new Graphe(RepresentationMode.Liste);

    grapheListe.AjouterLien(0, 1);
    grapheListe.AjouterLien(0, 2);
    grapheListe.AjouterLien(1, 2);
    grapheListe.AjouterLien(1, 3);
    grapheListe.AjouterLien(3, 4);

    grapheListe.ParcoursLargeur(0);
    grapheListe.ParcoursProfondeur(0);
    Console.WriteLine($"Graphe connexe ? {grapheListe.EstConnexe()}");
    Console.WriteLine($"Cycle détecté ? {grapheListe.ContientCycle()}");
    grapheListe.AfficherProprietes();
    ```

    """
