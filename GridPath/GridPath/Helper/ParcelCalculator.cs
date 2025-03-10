using GridPath.Models;

namespace GridPath.Helper
{
    public class ParcelCalculator
    {
        public string CalculateMainParcelAreaPoints(List<(double x, double y)> coordinates)
        {
            string json = string.Format(@"[
                {{ ""x"": {0}, ""y"": {1} }},
                {{ ""x"": {2}, ""y"": {3} }},
                {{ ""x"": {4}, ""y"": {5} }}
                ]", coordinates[0].x, coordinates[0].y, coordinates[1].x, coordinates[1].y, coordinates[2].x, coordinates[2].y);
            return json;
        }
       
        public List<string> CalculateSideParcelAreaPoints(List<(double x, double y) >coordinates)
        {
            List<string> jsonOfSideParcels = new List<string>();

            double beginningPointX = coordinates[0].x;
            double beginningPointY = coordinates[0].y;

            double EndPointX = coordinates[2].x;
            double EndPointY = coordinates[2].y;

            
                        #region Recatangle +50
                        

                        double editedBeginningPointXPlus50 = coordinates[0].x+200;
                        double editedbeginningPointY = coordinates[0].y;

                        double editedEndPointXPlus50 = EndPointX + 200;
                        double editedEndPointY = coordinates[0].y;

                        string jsonPlus50 = string.Format(@"[
                {{ ""x"": {0}, ""y"": {1} }},
                {{ ""x"": {2}, ""y"": {3} }},
                {{ ""x"": {4}, ""y"": {5} }},
                {{ ""x"": {6}, ""y"": {7} }}
                ]",  editedBeginningPointXPlus50, editedbeginningPointY, beginningPointX, beginningPointY, EndPointX, EndPointY, editedEndPointXPlus50, EndPointY);
                        CalculateRecatangleArea(EndPointX, editedEndPointXPlus50, EndPointY, beginningPointY);
                        jsonOfSideParcels.Add(jsonPlus50);
                        #endregion

                        #region Recatangle -50
                        double editedBeginningPointXMinus50 = coordinates[0].x - 200;

                        double editedEndPointXMinus50 = EndPointX - 200;

                        string jsonMinus50 = string.Format(@"[
                {{ ""x"": {0}, ""y"": {1} }},
                {{ ""x"": {2}, ""y"": {3} }},
                {{ ""x"": {4}, ""y"": {5} }},
                {{ ""x"": {6}, ""y"": {7} }}
                ]", editedBeginningPointXMinus50, coordinates[0].y, beginningPointX, beginningPointY, EndPointX, EndPointY, editedEndPointXMinus50, EndPointY);
                        CalculateRecatangleArea(EndPointX, editedEndPointXPlus50, EndPointY, beginningPointY);
                        jsonOfSideParcels.Add(jsonMinus50);
                        #endregion


            return jsonOfSideParcels;

        }
        public void CalculateRecatangleArea(double x4, double x3, double y4, double y1)
        {
            double plusx4 = Math.Abs(x4);
            double plusx3 = Math.Abs(x3);
            double plusy4 = Math.Abs(y4);
            double plusy1 = Math.Abs(y1);
                
            double x    = Math.Abs(plusx4 - plusx3);
            double y = Math.Abs(plusy4 - plusy1);
            double obsah = x * y;
            double pocetRozdeleni = Math.Ceiling((obsah / 5000));
            Console.WriteLine($"Obsah obdelniku je {x * y}");
            Console.WriteLine($"Pocet rozdeleni je {pocetRozdeleni}");

        }
        
    }
}
