using DotSpatial.Projections;

namespace GridPath.Helper
{
    public class CoordinateConversion
    {
        static List<(double x, double y)> polygonCoordinates = new List<(double, double)>();

        private const double MIN_X = 904384;
        private const double MAX_X = 1246155;
        private const double MIN_Y = 403554;
        private const double MAX_Y = 932266;

        private const double RECTANGLE_OFFSET = 100; // 100 metrů


        /// <summary>
        /// Converts from map coordinates(WGS84) to KN API coordinates(EPSG:5514)
        /// </summary>
        /// <param name="longitude"></param>
        /// <param name="latitude"></param>
        /// <returns>
        /// Returns coordinates X and then Y in an array
        /// </returns>
        public static (double x, double y) ConvertCoordinatesFromMapToKNApiv2(double longitude, double latitude)
        {
            double[] xy = { longitude, latitude };

            // Zdrojový souřadnicový systém – WGS84
            ProjectionInfo wgs84 = KnownCoordinateSystems.Geographic.World.WGS1984;

            // Cílový souřadnicový systém – EPSG:5514
            // EPSG:5514 se využívá pro mapové podklady v České republice, např. v katastru nemovitostí.
            // Níže je Esri string definující S‑JTSK / Krovak East North.
            string epsg5514Esri =
                "PROJCS[\"S-JTSK / Krovak East North\", " +
                "GEOGCS[\"S-JTSK\", " +
                "DATUM[\"S_JTSK\", " +
                "SPHEROID[\"Bessel 1841\",6377397.155,299.1528128]], " +
                "PRIMEM[\"Greenwich\",0], " +
                "UNIT[\"degree\",0.0174532925199433]], " +
                "PROJECTION[\"Krovak\"], " +
                "PARAMETER[\"latitude_of_center\",49.5], " +
                "PARAMETER[\"longitude_of_center\",24.83333333333333], " +
                "PARAMETER[\"azimuth\",30.28813972222222], " +
                "PARAMETER[\"scale_factor\",0.9999], " +
                "PARAMETER[\"false_easting\",0], " +
                "PARAMETER[\"false_northing\",0], " +
                "UNIT[\"metre\",1]]";
            ProjectionInfo epsg5514 = ProjectionInfo.FromEsriString(epsg5514Esri);

            // Provedeme převod souřadnic
            Reproject.ReprojectPoints(xy, null, wgs84, epsg5514, 0, 1);
            return (xy[0], xy[1]);
        }
        public static List<(double, double)> ConvertLineToRectangle(double firstPointX, double firstPointY, double secondPointX, double secondPointY)
        {
            polygonCoordinates.Clear();

            // Převedeme první bod
            var point1 = ConvertCoordinatesFromMapToKNApiv2(firstPointX, firstPointY);
            var point2 = ConvertCoordinatesFromMapToKNApiv2(secondPointX, secondPointY);

            // Vytvoříme obdélník pomocí vypočtených bodů
            // Použijeme RECTANGLE_OFFSET pro vytvoření obdélníku
            polygonCoordinates.Add((
                Math.Max(MIN_X, Math.Min(MAX_X, point1.x)),
                Math.Max(MIN_Y, Math.Min(MAX_Y, point1.y))
            ));

            polygonCoordinates.Add((
                Math.Max(MIN_X, Math.Min(MAX_X, point1.x + RECTANGLE_OFFSET)),
                Math.Max(MIN_Y, Math.Min(MAX_Y, point1.y))
            ));

            polygonCoordinates.Add((
                Math.Max(MIN_X, Math.Min(MAX_X, point2.x + RECTANGLE_OFFSET)),
                Math.Max(MIN_Y, Math.Min(MAX_Y, point2.y))
            ));

            polygonCoordinates.Add((
                Math.Max(MIN_X, Math.Min(MAX_X, point2.x)),
                Math.Max(MIN_Y, Math.Min(MAX_Y, point2.y))
            ));

            // Pro debug vypíšeme koordináty
            PrintPolygonCoordinates();

            // Ověříme, že všechny body jsou v povoleném rozsahu
            foreach (var point in polygonCoordinates)
            {
                if (point.Item1 < MIN_X || point.Item1 > MAX_X ||
                    point.Item2 < MIN_Y || point.Item2 > MAX_Y)
                {
                    throw new ArgumentException($"Bod ({point.Item1}, {point.Item2}) je mimo povolený rozsah.");
                }
            }

            return polygonCoordinates;
        }
        public static void PrintPolygonCoordinates()
        {
            Console.WriteLine("Polygon Coordinates:");
            foreach (var coordinate in polygonCoordinates)
            {
                Console.WriteLine($"X: {coordinate.x}, Y: {coordinate.y}");
            }
        }
    }
}
