using SkiaSharp;

namespace LivInParis.Models;

public static class GraphVisualizer
{
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
            if (positions.TryGetValue(lien.Noeud1.Id, out SKPoint p1) &&
                positions.TryGetValue(lien.Noeud2.Id, out SKPoint p2))
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
}
