using DotSpatial.Projections;

namespace GridPath.Helper
{
    public class CoordinateConversion
    {
        /// <summary>
        /// Converts from map coordinates(WGS84) to KN API coordinates(EPSG:5514)
        /// </summary>
        /// <param name="longitude"></param>
        /// <param name="latitude"></param>
        /// <returns>
        /// Returns coordinates X and then Y in an array
        /// </returns>
        public double[] ConvertCoordinatesFromMapToKNApiv2(double longitude, double latitude)
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
            return xy;
        }
    }
}
