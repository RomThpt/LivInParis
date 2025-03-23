using SkiaSharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace LivInParis.Models;

public static class GraphVisualizer
{
    // Conserver la méthode existante pour la rétrocompatibilité
    public static void Visualize(Graphe graphe, string filePath)
    {
        const int width = 800;
        const int height = 600;
        const float margin = 50;

        using (var surface = SKSurface.Create(new SKImageInfo(width, height)))
        {
            SKCanvas canvas = surface.Canvas;
            canvas.Clear(SKColors.White);

            var nodes = graphe.Noeuds.Keys.OrderBy(id => id).ToList();
            if (nodes.Count == 0) return;

            var positions = CalculateNodePositions(width, height, margin, nodes);
            DrawConnections(canvas, positions, graphe.Liens);
            DrawNodes(canvas, positions);

            using (var image = surface.Snapshot())
            using (var data = image.Encode(SKEncodedImageFormat.Png, 100))
            using (var stream = File.OpenWrite(filePath))
            {
                data.SaveTo(stream);
            }
        }
    }

    // Ajouter une nouvelle méthode pour les graphes génériques
    public static void Visualize<T>(Graphe<T> graph, string filePath, List<T>? path = null) where T : IEquatable<T>
    {
        const int width = 800;
        const int height = 600;
        const float margin = 50;

        using (var surface = SKSurface.Create(new SKImageInfo(width, height)))
        {
            SKCanvas canvas = surface.Canvas;
            canvas.Clear(SKColors.White);

            var nodes = graph.Nodes.Keys.ToList();
            if (nodes.Count == 0) return;

            var positions = CalculateGenericNodePositions(width, height, margin, nodes);
            DrawGenericConnections(canvas, positions, graph.Edges);
            DrawGenericNodes(canvas, positions);

            // Mettre en évidence le chemin si fourni
            if (path != null && path.Count > 1)
            {
                using var pathPaint = new SKPaint
                {
                    Color = SKColors.Red,
                    StrokeWidth = 3,
                    IsAntialias = true,
                    StrokeCap = SKStrokeCap.Round
                };

                for (int i = 0; i < path.Count - 1; i++)
                {
                    if (positions.TryGetValue(path[i], out var p1) &&
                        positions.TryGetValue(path[i + 1], out var p2))
                    {
                        canvas.DrawLine(p1, p2, pathPaint);
                    }
                }
            }

            using (var image = surface.Snapshot())
            using (var data = image.Encode(SKEncodedImageFormat.Png, 100))
            using (var stream = File.OpenWrite(filePath))
            {
                data.SaveTo(stream);
            }
        }
    }

    // Conserver les méthodes auxiliaires existantes
    private static Dictionary<int, SKPoint> CalculateNodePositions(int width, int height, float margin, List<int> nodes)
    {
        var positions = new Dictionary<int, SKPoint>();
        float centerX = width / 2f;
        float centerY = height / 2f;
        float radius = (Math.Min(width, height) / 2f) - margin;

        for (int i = 0; i < nodes.Count; i++)
        {
            float angle = (float)(2 * Math.PI * i / nodes.Count);
            float x = centerX + radius * (float)Math.Cos(angle);
            float y = centerY + radius * (float)Math.Sin(angle);
            positions[nodes[i]] = new SKPoint(x, y);
        }
        return positions;
    }

    private static void DrawConnections(SKCanvas canvas, Dictionary<int, SKPoint> positions, List<Lien> liens)
    {
        using var edgePaint = new SKPaint
        {
            Color = SKColors.Black,
            StrokeWidth = 2,
            IsAntialias = true
        };

        foreach (var lien in liens)
        {
            if (positions.TryGetValue(lien.Source.Id, out SKPoint p1) &&
                positions.TryGetValue(lien.Destination.Id, out SKPoint p2))
            {
                canvas.DrawLine(p1, p2, edgePaint);
            }
        }
    }

    private static void DrawNodes(SKCanvas canvas, Dictionary<int, SKPoint> positions)
    {
        using var nodePaint = new SKPaint
        {
            Color = SKColors.LightBlue,
            IsAntialias = true
        };

        using var textPaint = new SKPaint
        {
            Color = SKColors.Black,
            TextSize = 20,
            TextAlign = SKTextAlign.Center,
            IsAntialias = true
        };

        foreach (var (id, pos) in positions)
        {
            canvas.DrawCircle(pos, 20, nodePaint);
            canvas.DrawText(id.ToString(), pos.X, pos.Y + 7, textPaint);
        }
    }

    // Ajouter de nouvelles méthodes auxiliaires pour la visualisation générique
    private static Dictionary<T, SKPoint> CalculateGenericNodePositions<T>(int width, int height, float margin, List<T> nodes) where T : IEquatable<T>
    {
        var positions = new Dictionary<T, SKPoint>();
        float centerX = width / 2f;
        float centerY = height / 2f;
        float radius = (Math.Min(width, height) / 2f) - margin;

        for (int i = 0; i < nodes.Count; i++)
        {
            float angle = (float)(2 * Math.PI * i / nodes.Count);
            float x = centerX + radius * (float)Math.Cos(angle);
            float y = centerY + radius * (float)Math.Sin(angle);
            positions[nodes[i]] = new SKPoint(x, y);
        }
        return positions;
    }

    private static void DrawGenericConnections<T>(SKCanvas canvas, Dictionary<T, SKPoint> positions, List<Lien<T>> edges) where T : IEquatable<T>
    {
        using var paint = new SKPaint
        {
            Color = SKColors.Gray,
            StrokeWidth = 1,
            IsAntialias = true,
        };

        foreach (var edge in edges)
        {
            if (positions.TryGetValue(edge.Source.Id, out var source) &&
                positions.TryGetValue(edge.Target.Id, out var target))
            {
                canvas.DrawLine(source, target, paint);
            }
        }
    }

    private static void DrawGenericNodes<T>(SKCanvas canvas, Dictionary<T, SKPoint> positions) where T : IEquatable<T>
    {
        using var paint = new SKPaint
        {
            Color = SKColors.Blue,
            IsAntialias = true,
        };

        using var textPaint = new SKPaint
        {
            Color = SKColors.Black,
            TextSize = 12,
            IsAntialias = true,
        };

        const float radius = 15;

        foreach (var position in positions)
        {
            canvas.DrawCircle(position.Value, radius, paint);

            string label = position.Key.ToString() ?? string.Empty;
            var textBounds = new SKRect();
            textPaint.MeasureText(label, ref textBounds);

            float x = position.Value.X - textBounds.Width / 2;
            float y = position.Value.Y + textBounds.Height / 2;

            canvas.DrawText(label, x, y, textPaint);
        }
    }
}
