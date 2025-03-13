using GridPath.Models;
using System.Globalization;

namespace GridPath.Helper
{
    public class ParcelCalculator
    {
        List<string> jsonOfSideParcels = new List<string>();
        public string CalculateMainParcelAreaPoints(List<(double x, double y)> coordinates)
        {
            string json = string.Format(@"[
                {{ ""x"": {0}, ""y"": {1} }},
                {{ ""x"": {2}, ""y"": {3} }},
                {{ ""x"": {4}, ""y"": {5} }}
                ]", coordinates[0].x, coordinates[0].y, coordinates[1].x, coordinates[1].y, coordinates[2].x, coordinates[2].y);
            return json;
        }

        public List<string> CalculateSideParcelAreaPoints(List<(double x, double y)> coordinates)
        {


            double beginningPointX = coordinates[0].x;
            double beginningPointY = coordinates[0].y;
            double EndPointX = coordinates[2].x;
            double EndPointY = coordinates[2].y;


            #region Recatangle +50

            double editedBeginningPointXPlus50 = coordinates[0].x + 200;
            double editedbeginningPointY = coordinates[0].y;
            double editedEndPointXPlus50 = EndPointX + 200;
            double editedEndPointY = coordinates[0].y;

            List<(double, double)> pointsConvertedToListPlus = CoordinateConversion.ConvertPointsToList(editedBeginningPointXPlus50, editedbeginningPointY, beginningPointX, beginningPointY, EndPointX, EndPointY, editedEndPointXPlus50, EndPointY);
            DivideRectangleIntoSmallerParcels(CalculateRecatangleArea(pointsConvertedToListPlus), pointsConvertedToListPlus);
            #endregion

            #region Recatangle -50
            double editedBeginningPointXMinus50 = coordinates[0].x - 200;
            double editedEndPointXMinus50 = EndPointX - 200;

            List<(double, double)> pointsConvertedToListMinus = CoordinateConversion.ConvertPointsToList(editedBeginningPointXMinus50, coordinates[0].y, beginningPointX, beginningPointY, EndPointX, EndPointY, editedEndPointXMinus50, EndPointY);
            DivideRectangleIntoSmallerParcels(CalculateRecatangleArea(pointsConvertedToListMinus), pointsConvertedToListMinus);
            #endregion


            return jsonOfSideParcels;

        }
        public double CalculateRecatangleArea(List<(double x, double y)> points)
        {
            //using shaon therem
            int n = points.Count;
            double area = 0;

            for (int i = 0; i < n; i++)
            {
                double x1 = points[i].x, y1 = points[i].y;
                double x2 = points[(i + 1) % n].x, y2 = points[(i + 1) % n].y; // Cykluje zpět na začátek
                area += x1 * y2 - y1 * x2;
            }

            double resault = Math.Abs(area) / 2.0;
            double numberOfDevision = Math.Ceiling(resault / 5000)+1;
            return numberOfDevision;

        }
        public void DivideRectangleIntoSmallerParcels(double numberOfDivision, List<(double, double)> points)
        {
            //dostat body 1-4
            List<(double, double)> pointsFromFirstLine = DivideLineIntoPoints(points[0].Item1, points[0].Item2, points[3].Item1, points[3].Item2, numberOfDivision);
            //dostat body 2-3
            List<(double, double)> pointsFromSecondLine = DivideLineIntoPoints(points[1].Item1, points[1].Item2, points[2].Item1, points[2].Item2, numberOfDivision);
            
            for (int i = 0; i < numberOfDivision-1; i++)
            {
                string json = string.Format(CultureInfo.InvariantCulture, @"[
                {{ ""x"": {0}, ""y"": {1} }},
                {{ ""x"": {2}, ""y"": {3} }},
                {{ ""x"": {4}, ""y"": {5} }},
                {{ ""x"": {6}, ""y"": {7} }}
                ]",
                pointsFromFirstLine[i].Item1, pointsFromFirstLine[i].Item2,
                pointsFromSecondLine[i].Item1, pointsFromSecondLine[i].Item2,
                pointsFromSecondLine[i + 1].Item1, pointsFromSecondLine[i + 1].Item2,
                pointsFromFirstLine[i + 1].Item1, pointsFromFirstLine[i + 1].Item2);


                jsonOfSideParcels.Add(json);
            }
        }
        public List<(double x, double y)> DivideLineIntoPoints(double x1, double y1, double x2, double y2, double numberOfDivision)
        {
            List<(double x, double y)> body = new List<(double x, double y)>();

            for (int i = 0; i < numberOfDivision; i++)
            {
                double x =Math.Round(x1 + i * (x2 - x1) / (numberOfDivision - 1),2);
                double y =Math.Round( y1 + i * (y2 - y1) / (numberOfDivision - 1),2); 
                body.Add((x, y));
            }
            return body;
        }



    }
}
