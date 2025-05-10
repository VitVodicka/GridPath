using DotSpatial.Projections;
using System.Globalization;
using System.Security.Cryptography.Xml;

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
            polygonCoordinates.Clear();

            var pointBegin = ConvertCoordinatesFromMapToKNApiv2(firstPointX, firstPointY);
            var pointBeginRectangle = (pointBegin.x + 1, pointBegin.y);
            var pointEnd = ConvertCoordinatesFromMapToKNApiv2(secondPointX, secondPointY);
            var pointEndRectangle = (pointEnd.x + 1, pointEnd.y);

            polygonCoordinates.Add(pointBegin);
            polygonCoordinates.Add(pointBeginRectangle);
            polygonCoordinates.Add(pointEnd);
            polygonCoordinates.Add(pointEndRectangle);

            PrintPolygonCoordinates();

            return polygonCoordinates;
        }
        private static void PrintPolygonCoordinates()
        {
            Console.WriteLine("Polygon Coordinates:");
            foreach (var coordinate in polygonCoordinates)
            {
                Console.WriteLine($"X: {coordinate.x}, Y: {coordinate.y}");
            }
        }
        public static string CreateMiniSquareJsonFromPoint((double x, double y) coordinates , double offset = 10)
        {
            var square = new List<(double, double)>
        {
        (coordinates.x - offset, coordinates.y - offset),
        (coordinates.x + offset, coordinates.y - offset),
        (coordinates.x + offset, coordinates.y + offset),
        (coordinates.x - offset, coordinates.y + offset)
        };

            string json = string.Format(CultureInfo.InvariantCulture, @"[
        {{ ""x"": {0}, ""y"": {1} }},
        {{ ""x"": {2}, ""y"": {3} }},
        {{ ""x"": {4}, ""y"": {5} }},
        {{ ""x"": {6}, ""y"": {7} }}
        ]",
            square[0].Item1, square[0].Item2,
            square[1].Item1, square[1].Item2,
            square[2].Item1, square[2].Item2,
            square[3].Item1, square[3].Item2);

            return json;
        }

        public static List<(double, double)> ConvertPointsToList(double x1, double y1, double x2, double y2, double x3, double y3, double x4, double y4)
        {
            List<(double x, double y)> points = new List<(double x, double y)>();
            points.Add((x1, y1));
            points.Add((x2, y2));
            points.Add((x3, y3));
            points.Add((x4, y4));
            return points;
        }
        public static (int, int) ConvertCoordinatesFromMapToCoordinatesInGrid((double x, double y) coordinates)
        {
            return ((int)Math.Floor(coordinates.x / 5), (int)Math.Floor(coordinates.y / 5));
        }

    }
}
