using LivInParis.Models;

// Get paths to data files
string mtxFilePath = Path.Combine("public", "Association-soc-karate", "soc-karate.mtx");
string xmlPath = Path.Combine("public", "metro.xml");

// Ensure directories exist
EnsureDirectoryExists(Path.GetDirectoryName(mtxFilePath));
EnsureDirectoryExists(Path.GetDirectoryName(xmlPath));

// Try to analyze the Karate club network if the file exists
if (File.Exists(mtxFilePath))
{
    Console.ForegroundColor = ConsoleColor.Cyan;
    Console.WriteLine("***************************");
    Console.WriteLine("********  Analyse du réseau social  ********");
    Console.WriteLine("***************************");
    Console.ResetColor();

    var grapheListe = Graphe<int>.LoadFromMtxFile(mtxFilePath, RepresentationMode.Liste);
    AnalyzeGraph(grapheListe);
    GraphVisualizer.Visualize(grapheListe, "karate_liste.png");
    Console.WriteLine("\nkarate_liste.png genéré");
}
else
{
    Console.ForegroundColor = ConsoleColor.Yellow;
    Console.WriteLine("Fichier MTX non trouvé: Le réseau Karate club ne sera pas analysé.");
    Console.WriteLine($"Chemin recherché: {Path.GetFullPath(mtxFilePath)}");
    Console.ResetColor();
}

// Paris Metro analysis from XML
Console.ForegroundColor = ConsoleColor.Cyan;
Console.WriteLine("\n***************************");
Console.WriteLine("********  Analyse du métro parisien (XML)  ********");
Console.WriteLine("***************************");
Console.ResetColor();

Console.WriteLine($"Chargement du réseau métro depuis: {xmlPath}");

try 
{
    if (!File.Exists(xmlPath))
    {
        throw new FileNotFoundException($"Le fichier n'existe pas: {xmlPath}");
    }
    
    var metroGraph = Graphe<string>.LoadFromXmlFile(xmlPath, RepresentationMode.Liste);
    
    Console.ForegroundColor = ConsoleColor.Green;
    Console.WriteLine($"Réseau chargé avec succès: {metroGraph.Noeuds.Count} stations et {metroGraph.Liens.Count} connexions");
    Console.ResetColor();
    
    AnalyzeGraph(metroGraph);
    
    Console.WriteLine("\nGénération de la visualisation du réseau métro...");
    GraphVisualizer.Visualize(metroGraph, "paris_metro.png");
    Console.WriteLine("paris_metro.png généré avec succès!");
    
    // Demonstrate path-finding with the metro network
    if (metroGraph.Noeuds.Count > 0)
    {
        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine("\n***************************");
        Console.WriteLine("**** Calcul d'itinéraires métro ****");
        Console.WriteLine("***************************");
        Console.ResetColor();
        
        // Choose source and destination stations
        var sourceStation = metroGraph.Noeuds.Keys.First();
        var destStation = metroGraph.Noeuds.Keys.Skip(10).FirstOrDefault() ?? metroGraph.Noeuds.Keys.Last();
        
        Console.WriteLine($"Calcul d'itinéraire depuis {metroGraph.Noeuds[sourceStation].Nom} vers {metroGraph.Noeuds[destStation].Nom}");
        
        // Compute shortest path using Dijkstra
        var distances = metroGraph.Dijkstra(sourceStation);
        Console.WriteLine($"Distance: {distances[destStation]:F2} km");
        
        // Show other example distances
        Console.WriteLine("\nExemples de distances:");
        foreach (var (station, distance) in distances.OrderBy(kvp => kvp.Value).Take(5))
        {
            Console.WriteLine($"  Vers {metroGraph.Noeuds[station].Nom}: {distance:F2} km");
        }
    }
}
catch (Exception ex)
{
    Console.ForegroundColor = ConsoleColor.Red;
    Console.WriteLine($"Erreur lors du chargement du fichier XML: {ex.Message}");
    Console.WriteLine("Vérifiez que le fichier metro.xml existe dans le dossier public/");
    
    // Create a sample metro.xml file to help the user
    TryCreateSampleMetroFile();
    Console.ResetColor();
}

void AnalyzeGraph<T>(Graphe<T> g) where T : IEquatable<T>
{
    if (g.Noeuds.Count > 0)
    {
        T startNode = g.Noeuds.Keys.First();
        
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.WriteLine($"\nPropriétés du graphe:");
        Console.WriteLine($"Nombre de noeuds: {g.Noeuds.Count}");
        Console.WriteLine($"Nombre de liens: {g.Liens.Count}");
        Console.WriteLine($"Mode de représentation: {g.Mode}");
        Console.WriteLine($"Connexe: {g.EstConnexe()}");
        Console.WriteLine($"Contient cycle: {g.ContientCycle()}");
        Console.ResetColor();
    }
}

void EnsureDirectoryExists(string? directory)
{
    if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
    {
        try
        {
            Directory.CreateDirectory(directory);
            Console.WriteLine($"Répertoire créé: {directory}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Impossible de créer le répertoire {directory}: {ex.Message}");
        }
    }
}

void TryCreateSampleMetroFile()
{
    try
    {
        string sampleDir = Path.GetDirectoryName(xmlPath)!;
        if (!Directory.Exists(sampleDir))
        {
            Directory.CreateDirectory(sampleDir);
        }
        
        string samplePath = Path.Combine(sampleDir, "metro_sample.xml");
        
        // Create a simple sample XML with a few stations
        string sampleXml = @"<?xml version=""1.0"" encoding=""UTF-8""?>
<metro>
  <stations>
    <station id=""1"">
      <nom>Châtelet</nom>
      <latitude>48.8583</latitude>
      <longitude>2.3469</longitude>
    </station>
    <station id=""2"">
      <nom>Gare du Nord</nom>
      <latitude>48.8809</latitude>
      <longitude>2.3553</longitude>
    </station>
    <station id=""3"">
      <nom>Nation</nom>
      <latitude>48.8483</latitude>
      <longitude>2.3962</longitude>
    </station>
  </stations>
  <services>
    <service>
      <trajet>
        <station ref-id=""1""/>
        <station ref-id=""2""/>
        <station ref-id=""3""/>
      </trajet>
    </service>
  </services>
</metro>";

        File.WriteAllText(samplePath, sampleXml);
        Console.WriteLine($"\nUn exemple de fichier metro.xml a été créé: {samplePath}");
        Console.WriteLine("Vous pouvez le renommer en 'metro.xml' et relancer le programme.");
    }
    catch (Exception)
    {
        Console.WriteLine("Impossible de créer un fichier d'exemple.");
    }
}
