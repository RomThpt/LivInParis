using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace LivInParis.Models;

// Version non générique pour la rétrocompatibilité
public class Graphe
{
    public Dictionary<int, Noeud> Noeuds { get; } = new();
    public List<Lien> Liens { get; } = new();
    private readonly RepresentationMode _mode;
    private Dictionary<int, List<int>> _adjListe = new();
    private Dictionary<(int, int), bool> _adjMatrice = new();

    public Graphe(RepresentationMode mode)
    {
        _mode = mode;
    }

    public void AjouterLien(int source, int destination, double poids = 1.0)
    {
        AjouterNoeud(source);
        AjouterNoeud(destination);

        var lien = new Lien(Noeuds[source], Noeuds[destination], poids);
        if (!Liens.Contains(lien))
        {
            Liens.Add(lien);
            Noeuds[source].AjouterLien(lien);

            if (_mode == RepresentationMode.Liste)
            {
                if (!_adjListe.ContainsKey(source))
                    _adjListe[source] = new List<int>();
                _adjListe[source].Add(destination);
            }
            else
            {
                _adjMatrice[(source, destination)] = true;
            }
        }
    }

    public void AjouterNoeud(int id)
    {
        if (!Noeuds.ContainsKey(id))
            Noeuds[id] = new Noeud(id);
    }

    public void ParcoursLargeur(int startId)
    {
        if (!Noeuds.ContainsKey(startId)) return;

        var visites = new HashSet<int>();
        var file = new Queue<int>();
        file.Enqueue(startId);
        visites.Add(startId);

        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine("Parcours en largeur:");
        while (file.Count > 0)
        {
            int current = file.Dequeue();
            Console.Write($"{current} ");
            foreach (var voisin in GetVoisins(current))
                if (visites.Add(voisin))
                    file.Enqueue(voisin);
        }
        Console.WriteLine();
        Console.ResetColor();
    }

    public void ParcoursProfondeur(int startId)
    {
        HashSet<int> visites = new HashSet<int>();
        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine("Parcours en profondeur:");
        ParcoursProfondeurRecursif(startId, visites);
        Console.WriteLine();
        Console.ResetColor();
    }

    private void ParcoursProfondeurRecursif(int noeud, HashSet<int> visites)
    {
        if (!Noeuds.ContainsKey(noeud) || visites.Contains(noeud)) return;
        visites.Add(noeud);
        Console.Write($"{noeud} ");
        foreach (var voisin in GetVoisins(noeud))
            ParcoursProfondeurRecursif(voisin, visites);
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

        foreach (var voisin in GetVoisins(noeud))
        {
            if (DetecteCycleRecursif(voisin, visites, recursionStack))
                return true;
        }

        recursionStack.Remove(noeud);
        return false;
    }

    public void AfficherProprietes()
    {
        Console.ForegroundColor = ConsoleColor.Magenta;
        Console.WriteLine("\nPropriétés du graphe:");
        Console.WriteLine($"Nombre de noeuds: {Noeuds.Count}");
        Console.WriteLine($"Nombre de liens: {Liens.Count}");
        Console.WriteLine($"Mode de représentation: {_mode}");
        Console.ResetColor();
    }

    public static Graphe LoadFromMtxFile(string filePath, RepresentationMode mode)
    {
        var graphe = new Graphe(mode);
        foreach (var line in File.ReadLines(filePath))
        {
            if (line.StartsWith("%")) continue;
            var parts = line.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length < 2) continue;
            if (int.TryParse(parts[0], out int id1) && int.TryParse(parts[1], out int id2))
                graphe.AjouterLien(id1 - 1, id2 - 1);
        }
        return graphe;
    }

    private IEnumerable<int> GetVoisins(int noeudId)
    {
        if (_mode == RepresentationMode.Liste)
        {
            return Noeuds[noeudId].Voisins.Select(v => v.Id);
        }
        else
        {
            List<int> voisins = new List<int>();
            if (_adjMatrice != null && _adjMatrice.ContainsKey((noeudId, noeudId)))
                for (int j = 0; j < _adjMatrice.Count; j++)
                    if (_adjMatrice[(noeudId, j)])
                        voisins.Add(j);
            return voisins;
        }
    }
}

// Nouvelle implémentation générique du graphe
public class Graphe<T> where T : IEquatable<T>
{
    public Dictionary<T, Noeud<T>> Nodes { get; } = new();
    public List<Lien<T>> Edges { get; } = new();

    public void AddNode(T id)
    {
        if (!Nodes.ContainsKey(id))
            Nodes[id] = new Noeud<T>(id);
    }

    public void AddEdge(T source, T target, double weight = 1.0)
    {
        AddNode(source);
        AddNode(target);

        var edge = new Lien<T>(Nodes[source], Nodes[target], weight);
        if (!Edges.Contains(edge))
        {
            Edges.Add(edge);
            Nodes[source].AddEdge(edge);
        }
    }

    // Algorithme du plus court chemin de Dijkstra
    public Dictionary<T, double> Dijkstra(T start)
    {
        var distances = new Dictionary<T, double>();
        var priorityQueue = new PriorityQueue<T, double>();

        foreach (var node in Nodes.Keys)
            distances[node] = double.PositiveInfinity;

        distances[start] = 0;
        priorityQueue.Enqueue(start, 0);

        while (priorityQueue.Count > 0)
        {
            var current = priorityQueue.Dequeue();

            foreach (var edge in Nodes[current].OutgoingEdges)
            {
                var newDist = distances[current] + edge.Weight;
                if (newDist < distances[edge.Target.Id])
                {
                    distances[edge.Target.Id] = newDist;
                    priorityQueue.Enqueue(edge.Target.Id, newDist);
                }
            }
        }

        return distances;
    }

    // Algorithme de Floyd-Warshall pour tous les plus courts chemins
    public Dictionary<T, Dictionary<T, double>> FloydWarshall()
    {
        var dist = new Dictionary<T, Dictionary<T, double>>();

        foreach (var u in Nodes.Keys)
        {
            dist[u] = new Dictionary<T, double>();
            foreach (var v in Nodes.Keys)
                dist[u][v] = double.PositiveInfinity;
            dist[u][u] = 0;
        }

        foreach (var edge in Edges)
            dist[edge.Source.Id][edge.Target.Id] = edge.Weight;

        foreach (var k in Nodes.Keys)
            foreach (var i in Nodes.Keys)
                foreach (var j in Nodes.Keys)
                    if (dist[i][k] + dist[k][j] < dist[i][j])
                        dist[i][j] = dist[i][k] + dist[k][j];

        return dist;
    }

    // Obtenir le plus court chemin entre deux nœuds
    public List<T> GetShortestPath(T start, T end)
    {
        var path = new List<T>();
        if (!Nodes.ContainsKey(start) || !Nodes.ContainsKey(end))
            return path;

        var distances = new Dictionary<T, double>();
        Dictionary<T, T> previous = new Dictionary<T, T>();
        var priorityQueue = new PriorityQueue<T, double>();

        foreach (var node in Nodes.Keys)
            distances[node] = double.PositiveInfinity;

        distances[start] = 0;
        priorityQueue.Enqueue(start, 0);

        while (priorityQueue.Count > 0)
        {
            var currentNode = priorityQueue.Dequeue();
            if (currentNode.Equals(end))
                break;

            foreach (var edge in Nodes[currentNode].OutgoingEdges)
            {
                var newDist = distances[currentNode] + edge.Weight;
                if (newDist < distances[edge.Target.Id])
                {
                    distances[edge.Target.Id] = newDist;
                    previous[edge.Target.Id] = currentNode;
                    priorityQueue.Enqueue(edge.Target.Id, newDist);
                }
            }
        }

        // Reconstruire le chemin
        var current = end;
        path.Add(current);

        while (previous.ContainsKey(current))
        {
            current = previous[current];
            path.Add(current);
            if (current.Equals(start))
                break;
        }

        path.Reverse();
        return path;
    }
}
