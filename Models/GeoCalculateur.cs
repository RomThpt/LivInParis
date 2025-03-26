namespace LivInParis.Models;

public static class GeoCalculateur
{
    private const double RayonTerreKm = 6371.0;

    /// <summary>
    /// Calcule la distance en kilomètres entre deux points 
    /// géographiques en utilisant la formule de Haversine
    /// </summary>
    public static double CalculerDistanceHaversine(double lat1, double lon1, double lat2, double lon2)
    {
        // Conversion des degrés en radians
        lat1 = ToRadians(lat1);
        lon1 = ToRadians(lon1);
        lat2 = ToRadians(lat2);
        lon2 = ToRadians(lon2);

        // Formule de Haversine
        double dLat = lat2 - lat1;
        double dLon = lon2 - lon1;
        double a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                   Math.Cos(lat1) * Math.Cos(lat2) *
                   Math.Sin(dLon / 2) * Math.Sin(dLon / 2);
        double c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));

        return RayonTerreKm * c;
    }

    private static double ToRadians(double degrees)
    {
        return degrees * Math.PI / 180.0;
    }
}
