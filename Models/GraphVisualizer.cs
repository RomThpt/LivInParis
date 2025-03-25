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

    // Méthode pour visualiser un graphe avec des coordonnées géographiques
    public static void VisualizeGeo<T>(
        Graphe<T> graph,
        string filePath,
        Dictionary<T, (double Lat, double Lon)> positions,
        List<T>? path = null) where T : IEquatable<T>
    {
        const int width = 1200;
        const int height = 800;
        const float margin = 30;

        using (var surface = SKSurface.Create(new SKImageInfo(width, height)))
        {
            SKCanvas canvas = surface.Canvas;
            canvas.Clear(SKColors.White);

            if (positions.Count == 0) return;

            // Calcul des bornes géographiques
            var minLat = positions.Values.Min(p => p.Lat);
            var maxLat = positions.Values.Max(p => p.Lat);
            var minLon = positions.Values.Min(p => p.Lon);
            var maxLon = positions.Values.Max(p => p.Lon);

            // Conversion des coordonnées géo en positions canvas
            SKPoint ConvertToPoint(double lat, double lon)
            {
                var x = (float)((lon - minLon) / (maxLon - minLon) * (width - 2 * margin) + margin);
                var y = (float)(height - ((lat - minLat) / (maxLat - minLat) * (height - 2 * margin) + margin));
                return new SKPoint(x, y);
            }

            // Dessin des connexions
            // Find min and max weights for normalization
            double minWeight = graph.Edges.Min(e => e.Weight);
            double maxWeight = graph.Edges.Max(e => e.Weight);
            double range = maxWeight - minWeight;

            foreach (var edge in graph.Edges)
            {
                if (positions.TryGetValue(edge.Source.Id, out var sPos) &&
                    positions.TryGetValue(edge.Target.Id, out var tPos))
                {
                    var p1 = ConvertToPoint(sPos.Lat, sPos.Lon);
                    var p2 = ConvertToPoint(tPos.Lat, tPos.Lon);

                    // Normalize the weight to determine line thickness
                    float normalizedWeight = (float)((edge.Weight - minWeight) / (range > 0 ? range : 1));
                    float thickness = 3 * (1 - (normalizedWeight * 0.7f)) + 0.5f;

                    // Create a gradient color
                    byte colorValue = (byte)(180 + (normalizedWeight * 75));

                    using var edgePaint = new SKPaint
                    {
                        Color = new SKColor(colorValue, colorValue, colorValue),
                        StrokeWidth = thickness,
                        IsAntialias = true
                    };

                    canvas.DrawLine(p1, p2, edgePaint);
                }
            }

            // Dessin des stations
            using var nodePaint = new SKPaint
            {
                Color = SKColors.DarkBlue,
                IsAntialias = true
            };

            using var textPaint = new SKPaint
            {
                Color = SKColors.Black,
                TextSize = 10,
                IsAntialias = true
            };

            foreach (var (id, pos) in positions)
            {
                var point = ConvertToPoint(pos.Lat, pos.Lon);
                canvas.DrawCircle(point, 6, nodePaint);
                canvas.DrawText(id.ToString(), point.X + 8, point.Y + 4, textPaint);
            }

            // Dessin du chemin optimal
            if (path != null)
            {
                using var pathPaint = new SKPaint
                {
                    Color = SKColors.Red,
                    StrokeWidth = 4,
                    IsAntialias = true
                };

                for (int i = 0; i < path.Count - 1; i++)
                {
                    if (positions.TryGetValue(path[i], out var p1) &&
                        positions.TryGetValue(path[i + 1], out var p2))
                    {
                        canvas.DrawLine(
                            ConvertToPoint(p1.Lat, p1.Lon),
                            ConvertToPoint(p2.Lat, p2.Lon),
                            pathPaint
                        );
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
        // Find min and max weights for normalization
        double minWeight = edges.Min(e => e.Weight);
        double maxWeight = edges.Max(e => e.Weight);
        double range = maxWeight - minWeight;

        foreach (var edge in edges)
        {
            if (positions.TryGetValue(edge.Source.Id, out var source) &&
                positions.TryGetValue(edge.Target.Id, out var target))
            {
                // Normalize the weight to a value between 0 and 1
                float normalizedWeight = (float)((edge.Weight - minWeight) / (range > 0 ? range : 1));

                // Use the normalized weight to determine line thickness (inverse - thinner for longer distances)
                float thickness = 3 * (1 - (normalizedWeight * 0.7f)) + 0.5f;  // Between 0.5 and 3

                // Create a gradient color from black (short) to light gray (long distance)
                byte colorValue = (byte)(180 + (normalizedWeight * 75));  // Between 180 and 255

                using var paint = new SKPaint
                {
                    Color = new SKColor(colorValue, colorValue, colorValue),
                    StrokeWidth = thickness,
                    IsAntialias = true,
                };

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
