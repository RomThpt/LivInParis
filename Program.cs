﻿using LivInParis.Models;

string filePath = Path.Combine("public", "Association-soc-karate", "soc-karate.mtx");

Console.ForegroundColor = ConsoleColor.Cyan;
Console.WriteLine("***************************");
Console.WriteLine("********  Analyse  ********");
Console.WriteLine("***************************");
Console.ResetColor();

var grapheListe = Graphe.LoadFromMtxFile(filePath, RepresentationMode.Liste);
AnalyzeGraph(grapheListe);


// Visualization
GraphVisualizer.Visualize(grapheListe, "karate_liste.png");
Console.WriteLine("\nkarate_liste.png genéré");

void AnalyzeGraph(Graphe g)
{
    g.ParcoursLargeur(0);
    g.ParcoursProfondeur(0);

    Console.ForegroundColor = ConsoleColor.Yellow;
    Console.WriteLine($"\nConnexe: {g.EstConnexe()}");
    Console.WriteLine($"Contient cycle: {g.ContientCycle()}");
    Console.ResetColor();

    g.AfficherProprietes();
}
