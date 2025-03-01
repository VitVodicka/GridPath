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

            for (int i = 0; i < 3; i++)
            {
                switch (i)
                {
                    case 0:
                        #region Recatangle +50
                        

                        double editedBeginningPointXPlus50 = coordinates[0].x+50;
                        double editedbeginningPointY = coordinates[0].y;

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

                        double secondEditedBeginningPointXMinus50 = coordinates[0].x - 50;
                        double secondEditedbeginningPointY = coordinates[0].y;

                        double secondEditedEndPointXMinus50 = coordinates[0].x - 50;
                        double secondEditedEndPointY = coordinates[0].y;

                        string jsonMinus50 = string.Format(@"[
                {{ ""x"": {0}, ""y"": {1} }},
                {{ ""x"": {2}, ""y"": {3} }},
                {{ ""x"": {4}, ""y"": {5} }},
                {{ ""x"": {6}, ""y"": {7} }}
                ]", beginningPointX, beginningPointY, secondEditedBeginningPointXMinus50, secondEditedbeginningPointY, EndPointX, EndPointY, secondEditedEndPointXMinus50, secondEditedEndPointY);
                        jsonOfSideParcels.Add(jsonMinus50);
                        #endregion



                        //TODO obdelniky od zakladniho Y do 1. stredoveho Y, X je stejně, popřipadně x+50, X-50

                        break;
                    case 1:
                        double firstMiddleCoordinateY = coordinates[0].y + (coordinates[2].y - coordinates[0].y) / 2;
                        double firstMiddleCoordinateX = coordinates[0].x;

                        #region Recatangle +50(0->0.5)

                        double firstMiddleCoordinateXPlus50 = beginningPointX + 50;
                        double firstMiddleCoordinateYPlus50 = firstMiddleCoordinateY;

                        double firstBeggingMiddleCoordinateXPlus50= beginningPointX + 50;
                        double firstBeggingMiddleCoordinateYPlus50 = beginningPointY ;

                        string jsonHalfPlus50MiddleCoordinateJson = string.Format(@"[
                {{ ""x"": {0}, ""y"": {1} }},
                {{ ""x"": {2}, ""y"": {3} }},
                {{ ""x"": {4}, ""y"": {5} }},
                {{ ""x"": {6}, ""y"": {7} }}
                ]", beginningPointX, beginningPointY, firstBeggingMiddleCoordinateXPlus50, firstBeggingMiddleCoordinateYPlus50, firstMiddleCoordinateX, firstMiddleCoordinateY, firstMiddleCoordinateXPlus50, firstMiddleCoordinateYPlus50);
                        jsonOfSideParcels.Add(jsonHalfPlus50MiddleCoordinateJson);

                        #endregion

                        #region Recatangle -50(0->0.5)

                        double firstMiddleCoordinateXMinus50 = beginningPointX - 50;
                        double firstMiddleCoordinateYMinus50 = firstMiddleCoordinateY;

                        double firstBeggingMiddleCoordinateXMinus50 = beginningPointX - 50;
                        double firstBeggingMiddleCoordinateYMinus50 = beginningPointY;

                        string jsonHalfMinus50MiddleCoordinateJson = string.Format(@"[
                {{ ""x"": {0}, ""y"": {1} }},
                {{ ""x"": {2}, ""y"": {3} }},
                {{ ""x"": {4}, ""y"": {5} }},
                {{ ""x"": {6}, ""y"": {7} }}
                ]", beginningPointX, beginningPointY, firstBeggingMiddleCoordinateXMinus50, firstBeggingMiddleCoordinateYMinus50, firstMiddleCoordinateX, firstMiddleCoordinateY, firstMiddleCoordinateXMinus50, firstMiddleCoordinateYMinus50);
                        jsonOfSideParcels.Add(jsonHalfMinus50MiddleCoordinateJson);

                        #endregion


                        #region Recatangle +50(0.5->1)

                        string jsonSecondHalfPlus50MiddleCoordinateJson = string.Format(@"[
                {{ ""x"": {0}, ""y"": {1} }},
                {{ ""x"": {2}, ""y"": {3} }},
                {{ ""x"": {4}, ""y"": {5} }},
                {{ ""x"": {6}, ""y"": {7} }}
                ]", firstMiddleCoordinateX, firstMiddleCoordinateY, firstMiddleCoordinateXPlus50, firstMiddleCoordinateYPlus50, EndPointX, EndPointY, EndPointX+50, EndPointY);
                        jsonOfSideParcels.Add(jsonSecondHalfPlus50MiddleCoordinateJson);

                        #endregion


                        #region Recatangle -50(0.5->1)


                        string jsonSecondHalfMinus50MiddleCoordinateJson = string.Format(@"[
                {{ ""x"": {0}, ""y"": {1} }},
                {{ ""x"": {2}, ""y"": {3} }},
                {{ ""x"": {4}, ""y"": {5} }},
                {{ ""x"": {6}, ""y"": {7} }}
                ]", firstMiddleCoordinateX, firstMiddleCoordinateY, firstMiddleCoordinateXMinus50, firstMiddleCoordinateYMinus50, EndPointX, EndPointY, EndPointX - 50, EndPointY);
                        jsonOfSideParcels.Add(jsonSecondHalfMinus50MiddleCoordinateJson);

                        #endregion

                        break;
                    default:
                        break;
                }
                


            }
            return jsonOfSideParcels;

        }
        
    }
}
