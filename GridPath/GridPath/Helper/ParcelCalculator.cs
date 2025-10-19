using GridPath.Controllers;
using GridPath.Helper.Dictionaries;
using GridPath.Models;
using GridPath.Models.Grid;
using GridPath.Models.Parcels;
using System.ComponentModel;
using System.Globalization;

namespace GridPath.Helper
{
    public class ParcelCalculator
    {
        List<string> jsonOfSideParcels = new List<string>();
        public string ConvertMainParcelAreaIntoJsonPoints(List<(double x, double y)> coordinates)
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

            double editedBeginningPointXPlus50 = coordinates[0].x + 100;
            double editedbeginningPointY = coordinates[0].y;
            double editedEndPointXPlus50 = EndPointX + 100;
            double editedEndPointY = coordinates[0].y;

            List<(double, double)> pointsConvertedToListPlus = CoordinateConversion.ConvertPointsToList(editedBeginningPointXPlus50, editedbeginningPointY, beginningPointX, beginningPointY, EndPointX, EndPointY, editedEndPointXPlus50, EndPointY);
            DivideRectangleIntoSmallerParcels(CalculateRecatangleArea(pointsConvertedToListPlus), pointsConvertedToListPlus);
            #endregion

            #region Recatangle -50
            double editedBeginningPointXMinus50 = coordinates[0].x - 100;
            double editedEndPointXMinus50 = EndPointX - 100;

            List<(double, double)> pointsConvertedToListMinus = CoordinateConversion.ConvertPointsToList(editedBeginningPointXMinus50, coordinates[0].y, beginningPointX, beginningPointY, EndPointX, EndPointY, editedEndPointXMinus50, EndPointY);
            DivideRectangleIntoSmallerParcels(CalculateRecatangleArea(pointsConvertedToListMinus), pointsConvertedToListMinus);
            #endregion


            return jsonOfSideParcels;

        }
        public double CalculateRecatangleArea(List<(double x, double y)> points)
        {
            //pomocí shaon therem
            int n = points.Count;
            double area = 0;

            for (int i = 0; i < n; i++)
            {
                double x1 = points[i].x, y1 = points[i].y;
                double x2 = points[(i + 1) % n].x, y2 = points[(i + 1) % n].y; 
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
        public List<DetailRatedParcel> CalculateLandPoints()
        {
            List<DetailRatedParcel> ratedParcels = new List<DetailRatedParcel>();
            double points;
            
            foreach (var parcel in HomeController.parcelsParameters)
            {
                points = 100;
                string warning = "";
                var LandKey = (parcel.DruhPozemku.Nazev, int.Parse(parcel.DruhPozemku.Kod));

                string ochranyKod = parcel.ZpusobyOchrany?.Kod; int parsedKod = int.TryParse(ochranyKod, out var tempKod) ? tempKod : 0;
                var LandKeyProtection = parcel.ZpusobyOchrany != null ? (parcel.ZpusobyOchrany.Nazev, parsedKod) : (null, 0);

                if (LandDictionaries.NotGoodLandTypes.TryGetValue(LandKey, out int value))
                {
                    if(parcel.DruhPozemku.Nazev == "ostatní plocha")
                    {
                        if (LandDictionaries.NotPreferedLandUsages.TryGetValue(LandKey, out int valueUsages))
                        {
                            points += valueUsages;
                        }
                        else
                        { points += 100;
                        }
                    }
                    else
                    { points += value;
                    }
                    
                }
                else
                { points += 100;
                }
                //not sure jestli to je správně
                if (parcel.ZpusobyOchrany != null && !string.IsNullOrEmpty(parcel.ZpusobyOchrany.Nazev) &&
                    LandDictionaries.ProtectionScores.TryGetValue(LandKeyProtection, out int valueProtection))
                {
                    points += valueProtection;
                }
                else
                {
                    points += 100;
                }

                if(parcel.Stavba != "")
                {
                    points = 0;
                }
                if(parcel.PravoStavby!= "")
                {
                    warning += "Právo stavby";
                }
                if(parcel.RizeniPlomby != "" || parcel.RizeniPlomby != "[]")
                {
                    warning+= " " + parcel.RizeniPlomby;
                }
                DetailRatedParcel detailedParcel = new DetailRatedParcel(parcel, points,warning);
                ratedParcels.Add(detailedParcel);

            }
            return ratedParcels;
        }
        public async Task<Dictionary<(double x, double y), BunkaVGridu>>  GetGridOfRatedParcels(List<DetailRatedParcel> ratedParcels)
        {
            Dictionary<(double x, double y), BunkaVGridu> grid = new Dictionary<(double x, double y), BunkaVGridu>();
            double gridCellSize = 5.0;

            foreach (var ratedParcel in ratedParcels)
            {

                double xGrid = Math.Floor(Double.Parse(ratedParcel.DetailedParcel.DefinicniBod.X) / gridCellSize);
                double yGrid = Math.Floor(Double.Parse(ratedParcel.DetailedParcel.DefinicniBod.Y) / gridCellSize);

                var key = (xGrid, yGrid);

                if (!grid.ContainsKey(key))
                {
                    grid[key] = new BunkaVGridu();
                }

                grid[key].Pozemky.Add(ratedParcel);
                grid[key].UpdateStredniHodnota();
            }
            return grid;
            
        }
        //TODO od 15. prvku tak zkusit pnajít jaký by byl nejbližší neighbour
        //15. prvek (-124001, -230032)
        // start 49.2668956N, 16.2672894E
        //end 49.2603700N, 16.2984031E
        //zkontrolovat, jestli to jde i do hůř hodnot, který můžou mít jediný value
        //proč se tam připočítává distance
        //ono když je tam dobrá distance, tak i když střední hodnota u souseda tak je tam 0, tak to tam i přidá
        //hledá to a pak se to zastaví, ted u 15. vistied
        //je to fixnuty, ale pak to může nenajit dalšho souseda
        //zda nebude lepší, když je dám do jinačího
        //zkontrolovat co tato tak má za properties: [(-124001, -230032)]
        //u méně pozemků tak to nenajde souseda

        //checknout, když okolo to nenajde pozemky podle osové bitýšky, pokud tam je pravděpodobně stejný definicni bod
        //from 49.3047700N, 16.2236231E to 49.3133600N, 16.2166278E
        public List<(double x, double y)> DijkstraPath(
    Dictionary<(double x, double y), BunkaVGridu> grid,
    (double x, double y) start,
    (double x, double y) goal)
        {
            const double EPS = 1e-9;

            // 1) Validace vstupů a průchodnosti
            if (grid.Count == 0) return new();
            if (!grid.ContainsKey(start) || !grid.ContainsKey(goal)) return new();

            // 2) Hranice gridu (pro stop podmínku při hledání souseda)
            double minX = double.PositiveInfinity, maxX = double.NegativeInfinity;
            double minY = double.PositiveInfinity, maxY = double.NegativeInfinity;
            foreach (var (gx, gy) in grid.Keys)
            {
                if (gx < minX) minX = gx;
                if (gx > maxX) maxX = gx;
                if (gy < minY) minY = gy;
                if (gy > maxY) maxY = gy;
            }

            // 3) Najdi nejbližší existující buňku v daném směru (0,+1), (0,-1), (+1,0), (-1,0)
            (double x, double y)? FindNeighbour((double x, double y) u, int dx, int dy)
            {
                double cx = u.x;
                double cy = u.y;

                // Lokální maximum kroků do hrany podle směru (rychlejší než globální limit)
                int StepsToEdge((double x, double y) p, int ddx, int ddy)
                {
                    if (ddx == 1) return (int)(maxX - p.x);
                    if (ddx == -1) return (int)(p.x - minX);
                    if (ddy == 1) return (int)(maxY - p.y);
                    if (ddy == -1) return (int)(p.y - minY);
                    return 0;
                }

                int maxSteps = StepsToEdge(u, dx, dy) + 1;

                for (int s = 0; s < maxSteps; s++)
                {
                    cx += dx;
                    cy += dy;

                    if (cx < minX || cx > maxX || cy < minY || cy > maxY)
                        return null;

                    var cand = (cx, cy);
                    if (grid.ContainsKey(cand))
                        return cand;
                }
                return null;
            }

            // 4) Dijkstrovy struktury
            var distance = new Dictionary<(double x, double y), double>(grid.Count);
            var predecessors = new Dictionary<(double x, double y), (double x, double y)?>(grid.Count);
            var queue = new PriorityQueue<(double x, double y), double>();
            var visited = new HashSet<(double x, double y)>();

            foreach (var k in grid.Keys) { distance[k] = double.MaxValue; predecessors[k] = null; }
            distance[start] = 0;
            queue.Enqueue(start, 0);

            var neighbours = new (int dx, int dy)[] { (0, 1), (0, -1), (1, 0), (-1, 0) };

            // 5) Hlavní smyčka

            while (queue.Count > 0)
            {
                // 1) Vezmi vrchol s nejnižší známou vzdáleností
                if (!queue.TryDequeue(out var u, out var du))
                    break;

                // 2) Pokud je to zastaralý záznam → přeskoč
                if (du > distance[u])
                    continue;

                // 3) Pokud už je zpracovaný → přeskoč
                if (!visited.Add(u))
                    continue;

                // 4) Pokud jsme v cíli → ukonči
                if (u == goal)
                    break;

                // 5) Projdi sousedy
                foreach (var (dx, dy) in neighbours)
                {
                    var nb = FindNeighbour(u, dx, dy);
                    if (nb is null) continue;

                    var v = nb.Value;

                    // Pokud buňka není v gridu, přeskoč
                    if (!grid.TryGetValue(v, out var vCell))
                        continue;

                    // Zakázané nebo vadné buňky (neprůchodné)
                    double w = vCell.StredniHodnota;
                    if (double.IsNaN(w) || double.IsInfinity(w) || w <= EPS)
                        continue;

                    // Cena cesty do souseda
                    double newDist = distance[u] + w;
                    if (double.IsNaN(newDist) || double.IsInfinity(newDist))
                        continue;




                    //u 12. souseda tak newDist 2590 < 2010
                    // Relaxace: pokud je nová cesta lepší, aktualizuj
                    if (newDist+ EPS < distance[v])
                    {
                        distance[v] = newDist;
                        predecessors[v] = u;
                        queue.Enqueue(v, newDist);
                    }
                }
            }


            // 6) Rekonstrukce cesty
            if (!visited.Contains(goal)) return new();

            var path = new List<(double x, double y)>();
            for (var cur = goal; ;)
            {
                path.Add(cur);
                var p = predecessors[cur];
                if (p is null) break;
                cur = p.Value;
            }
            path.Reverse();
            return path;
        }



    }
}
