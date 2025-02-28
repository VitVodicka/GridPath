using DotSpatial.Projections;

namespace GridPath.Helper
{
    public class CoordinateConversion
    {
        static List<(double x, double y)> polygonCoordinates = new List<(double, double)>();


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
            return (Math.Round(xy[0],0), Math.Round(xy[1],0));
        }
        /// <summary>
        /// returns a polygon of positions
        /// </summary>
        /// <param name="firstPointX"></param>
        /// <param name="firstPointY"></param>
        /// <param name="secondPointX"></param>
        /// <param name="secondPointY"></param>
        /// <returns> Returns four points in the following order:
        /// 1. Start point (pointBegin)
        /// 2. Shifted start point (pointBeginRectangle)
        /// 3. End point (pointEnd)
        /// 4. Shifted end point (pointEndRectangle) </returns>
        public static List<(double, double)> ConvertLineToRectangle(double firstPointX, double firstPointY, double secondPointX, double secondPointY)
        {
            //chyba v čárkách nebo v typu zadávání nebo převodu
            polygonCoordinates.Clear();

            var pointBegin = ConvertCoordinatesFromMapToKNApiv2(firstPointX, firstPointY);
            var pointBeginRectangle = (pointBegin.x + 1, pointBegin.y);
            var pointEnd = ConvertCoordinatesFromMapToKNApiv2(secondPointX, secondPointY);
            var pointEndRectangle = (pointEnd.x + 1, pointEnd.y);

            polygonCoordinates.Add(pointBegin);
            polygonCoordinates.Add(pointBeginRectangle);
            polygonCoordinates.Add(pointEnd);
            polygonCoordinates.Add(pointEndRectangle);
            // Pro debug vypíšeme koordináty
            PrintPolygonCoordinates();

            // Ověříme, že všechny body jsou v povoleném rozsahu
            /*foreach (var point in polygonCoordinates)
            {
                if (point.Item1 < MIN_X || point.Item1 > MAX_X ||
                    point.Item2 < MIN_Y || point.Item2 > MAX_Y)
                {
                    throw new ArgumentException($"Bod ({point.Item1}, {point.Item2}) je mimo povolený rozsah.");
                }
            }*/

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
