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
            //TODO find a way of splitting the parcel into 3 parts based on the length of the parcel 
            List<string> jsonOfSideParcels = new List<string>();
            //TODO return a list of json with coordinates

            //double coordinatesBeginX, double coordinatesBeginY, double coordinatesEndY
            //double firstMiddleCoordingateY = coordinates[0].y + (coordinatesEndY - coordinatesBeginY)/2;
            //double secondMiddleCoordingateY = coordinatesBeginY + ((coordinatesEndY - coordinatesBeginY) / 2)*2;
            for (int i = 0; i < 3; i++)
            {
                switch (i)
                {
                    case 0:
                        #region Recatangle +50
                        double beginningPointX = coordinates[0].x;
                        double beginningPointY = coordinates[0].y;

                        double editedBeginningPointXPlus50 = coordinates[0].x+50;
                        double editedbeginningPointY = coordinates[0].y;


                        double EndPointX = coordinates[2].x;
                        double EndPointY = coordinates[2].y;

                        double editedEndPointXPlus50 = coordinates[0].x + 50;
                        double editedEndPointY = coordinates[0].y;

                        string jsonPlus50 = string.Format(@"[
                {{ ""x"": {0}, ""y"": {1} }},
                {{ ""x"": {2}, ""y"": {3} }},
                {{ ""x"": {4}, ""y"": {5} }},
                {{ ""x"": {6}, ""y"": {7} }}
                ]", beginningPointX, beginningPointY, editedBeginningPointXPlus50, editedbeginningPointY, EndPointX, EndPointY, editedEndPointXPlus50, editedEndPointY);
                        jsonOfSideParcels.Add(jsonPlus50);
                        #endregion

                        #region Recatangle -50
                        double secondBeginningPointX = coordinates[0].x;
                        double secondBeginningPointY = coordinates[0].y;

                        double secondEditedBeginningPointXMinus50 = coordinates[0].x - 50;
                        double secondEditedbeginningPointY = coordinates[0].y;


                        double secondEndPointX = coordinates[2].x;
                        double secondEndPointY = coordinates[2].y;

                        double secondEditedEndPointXMinus50 = coordinates[0].x - 50;
                        double secondEditedEndPointY = coordinates[0].y;

                        string jsonMinus50 = string.Format(@"[
                {{ ""x"": {0}, ""y"": {1} }},
                {{ ""x"": {2}, ""y"": {3} }},
                {{ ""x"": {4}, ""y"": {5} }},
                {{ ""x"": {6}, ""y"": {7} }}
                ]", secondBeginningPointX, secondBeginningPointY, secondEditedBeginningPointXMinus50, secondEditedbeginningPointY, secondEndPointX, secondEndPointY, secondEditedEndPointXMinus50, secondEditedEndPointY);
                        jsonOfSideParcels.Add(jsonMinus50);
                        #endregion



                        //TODO obdelniky od zakladniho Y do 1. stredoveho Y, X je stejně, popřipadně x+50, X-50

                        break;
                    case 1:
                        //TODO obdelniky od 1. stredoveho Y do 2. stredoveho Y, X je stejně, popřipadně x+50, X-50
                        break;
                    case 2:

                        //TODO obdelniky od 2. stredoveho Y do koncoveho Y, X je stejně, popřipadně x+50, X-50
                        break;
                    default:
                        break;
                }
                


            }
            return jsonOfSideParcels;

        }
        
    }
}
