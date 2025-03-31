using SkiaSharp;

namespace LivInParis.Models;

public static class GraphVisualizer
{
    public static void Visualize<T>(Graphe<T> graphe, string filePath) where T : IEquatable<T>
    {
        const int width = 1200;
        const int height = 800;
        const float margin = 50;

        using (var surface = SKSurface.Create(new SKImageInfo(width, height)))
        {
            SKCanvas canvas = surface.Canvas;
            canvas.Clear(SKColors.WhiteSmoke);
            
            // Draw background grid
            DrawGrid(canvas, width, height);

            var nodes = graphe.Noeuds.Values.ToList();
            if (nodes.Count == 0) return;

            var positions = CalculateNodePositions(width, height, margin, nodes);
            DrawConnections(canvas, positions, graphe.Liens);
            DrawStations(canvas, positions, nodes);

            // Draw border and title
            DrawMapBorder(canvas, width, height);

            using (var image = surface.Snapshot())
            using (var data = image.Encode(SKEncodedImageFormat.Png, 100))
            using (var stream = File.OpenWrite(filePath))
            {
                data.SaveTo(stream);
            }
        }
    }

    private static void DrawGrid(SKCanvas canvas, int width, int height)
    {
        using var gridPaint = new SKPaint
        {
            Color = SKColors.LightGray.WithAlpha(100),
            StrokeWidth = 0.5f,
            IsAntialias = true
        };
        
        // Draw grid lines
        for (int x = 0; x < width; x += 50)
            canvas.DrawLine(x, 0, x, height, gridPaint);
            
        for (int y = 0; y < height; y += 50)
            canvas.DrawLine(0, y, width, y, gridPaint);
    }

    private static void DrawMapBorder(SKCanvas canvas, int width, int height)
    {
        // Draw border
        using var borderPaint = new SKPaint
        {
            Color = SKColors.Gray,
            StrokeWidth = 2,
            IsAntialias = true,
            Style = SKPaintStyle.Stroke
        };
        canvas.DrawRect(0, 0, width, height, borderPaint);

        // Draw title
        using var titlePaint = new SKPaint
        {
            Color = SKColors.Black,
            TextSize = 28,
            IsAntialias = true,
            TextAlign = SKTextAlign.Center
        };
        canvas.DrawText("Plan du Métro Parisien", width / 2, 30, titlePaint);
    }

    private static Dictionary<T, SKPoint> CalculateNodePositions<T>(
        int width, int height, float margin, List<Noeud<T>> nodes) where T : IEquatable<T>
    {
        var positions = new Dictionary<T, SKPoint>();
        if (nodes.Count == 0)
            return positions;

        var latitudes = nodes.Select(n => n.Latitude).ToList();
        var longitudes = nodes.Select(n => n.Longitude).ToList();

        double minLat = latitudes.Min();
        double maxLat = latitudes.Max();
        double minLon = longitudes.Min();
        double maxLon = longitudes.Max();

        double latRange = maxLat - minLat;
        double lonRange = maxLon - minLon;

        // Adjust for aspect ratio to prevent distortion
        double mapAspect = lonRange / latRange;
        double screenAspect = (width - 2 * margin) / (height - 2 * margin);
        
        float contentWidth, contentHeight;
        
        if (mapAspect > screenAspect)
        {
            // Map is wider than screen
            contentWidth = width - 2 * margin;
            contentHeight = (float)(contentWidth / mapAspect);
        }
        else
        {
            // Map is taller than screen
            contentHeight = height - 2 * margin;
            contentWidth = (float)(contentHeight * mapAspect);
        }

        float centerX = width / 2;
        float centerY = height / 2;
        float mapLeft = centerX - contentWidth / 2;
        float mapTop = centerY - contentHeight / 2;

        foreach (var node in nodes)
        {
            double normalizedLon = (node.Longitude - minLon) / lonRange;
            double normalizedLat = (node.Latitude - minLat) / latRange;

            float x = mapLeft + (float)(normalizedLon * contentWidth);
            float y = mapTop + (float)((1 - normalizedLat) * contentHeight); // Invert Y to match geographic orientation

            positions[node.Id] = new SKPoint(x, y);
        }

        return positions;
    }

    private static void DrawConnections<T>(SKCanvas canvas, Dictionary<T, SKPoint> positions, List<Lien<T>> liens) where T : IEquatable<T>
    {
        using var edgePaint = new SKPaint
        {
            Color = SKColors.DodgerBlue,
            StrokeWidth = 2.5f,
            IsAntialias = true
        };

        foreach (var lien in liens)
        {
            if (positions.TryGetValue(lien.Noeud1.Id, out SKPoint p1) &&
                positions.TryGetValue(lien.Noeud2.Id, out SKPoint p2))
            {
                canvas.DrawLine(p1, p2, edgePaint);
            }
        }
    }

    private static void DrawStations<T>(SKCanvas canvas, Dictionary<T, SKPoint> positions, List<Noeud<T>> nodes) where T : IEquatable<T>
    {
        using var stationPaint = new SKPaint
        {
            Color = SKColors.White,
            IsAntialias = true,
            Style = SKPaintStyle.Fill
        };

        using var stationBorder = new SKPaint
        {
            Color = SKColors.Black,
            IsAntialias = true,
            Style = SKPaintStyle.Stroke,
            StrokeWidth = 1.5f
        };

        using var stationLabelPaint = new SKPaint
        {
            Color = SKColors.Black,
            TextSize = 12,
            IsAntialias = true,
            TextAlign = SKTextAlign.Center
        };

        foreach (var node in nodes)
        {
            if (positions.TryGetValue(node.Id, out SKPoint pos))
            {
                // Draw station marker
                float radius = 6;
                canvas.DrawCircle(pos, radius, stationPaint);
                canvas.DrawCircle(pos, radius, stationBorder);

                // Draw station name for important stations or if we have few stations
                if (nodes.Count < 100 || !string.IsNullOrEmpty(node.Nom))
                {
                    // // Draw white background for better readability
                    // var text = node.Nom ?? $"Station {node.Id}";
                    var textBounds = new SKRect();
                    // stationLabelPaint.MeasureText(text, ref textBounds);
                    
                    using var bgPaint = new SKPaint
                    {
                        Color = SKColors.White.WithAlpha(180),
                        IsAntialias = true
                    };
                    
                    canvas.DrawRect(
                        pos.X - textBounds.Width/2 - 2,
                        pos.Y + radius + 2,
                        textBounds.Width + 4,
                        textBounds.Height + 2,
                        bgPaint);
                        
                    // Draw station name
                    // canvas.DrawText(text, pos.X, pos.Y + radius + 14, stationLabelPaint);
                }
            }
        }
    }
}
