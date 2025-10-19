using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using GridPath.Controllers;
using GridPath.Helper;
using GridPath.Models;
using GridPath.Models.Grid;
using GridPath.Models.Parcels;
using GridPath.Models.PolygonParcels;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace GridPath.Services.ApiServices
{
    public class ParcelService
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiUrl;
        private readonly string _apiKey;
        private ParcelCalculator _parcelCalculator;
        



        public ParcelService(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _apiUrl = configuration["ApiSettings:ApiUrl"];
            _apiKey = configuration["ApiSettings:ApiKey"];
            _httpClient.DefaultRequestHeaders.Add("ApiKey", _apiKey);
            _parcelCalculator = new ParcelCalculator();
        }

        public async Task<string> GetParcelFromParameters()
        {
            string parcelSearchParameters = "/Parcely/Vyhledani?KodKatastralnihoUzemi=778214&TypParcely=PKN&DruhCislovaniParcely=2&" +
                "KmenoveCisloParcely=1766&PoddeleniCislaParcely=3";
            try
            {
                HttpResponseMessage response = await _httpClient.GetAsync(_apiUrl+parcelSearchParameters);

                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadAsStringAsync();
                }
                else
                {
                    string responseContent = await response.Content.ReadAsStringAsync();
                    throw new Exception($"Chyba: {response.StatusCode}, Detailní odpověď: {responseContent}");
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Chyba připojení: {ex.Message}");
            }
        }
        public async Task<DetailedParcel> GetParcelFromId(string id)
        {
            try
            {
                string parametersId = "/Parcely/" + id;
                HttpResponseMessage response = await _httpClient.GetAsync(_apiUrl + parametersId);
                response.EnsureSuccessStatusCode();

                using Stream stream = await response.Content.ReadAsStreamAsync();
                return await new JsonParser().ParseDetailedParcelData(stream);
            }
            catch (HttpRequestException ex)
            {
                throw new Exception("Chyba při volání API: " + ex.Message, ex);
            }
            catch (TaskCanceledException ex)
            {
                throw new Exception("Požadavek trval příliš dlouho: " + ex.Message, ex);
            }
            catch (JsonException ex)
            {
                throw new Exception("Chyba při čtení odpovědi: " + ex.Message, ex);
            }
            catch (Exception ex)
            {
                throw new Exception("Neočekávaná chyba: " + ex.Message, ex);
            }
        }

        public async Task GetMainParametersOfParcels()
        {

            foreach (var parcel in HomeController.parcelsFromAPIPolygon)
            {

                HomeController.parcelsParameters.Add(await GetParcelFromId(parcel.Id));
            }

        }
        public async Task<string> GetNeighbourParcels()
        {
            string neighbourParameters = "/Parcely/SousedniParcely/459212744";
            try
            {
                HttpResponseMessage response = await _httpClient.GetAsync(_apiUrl+ neighbourParameters);

                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadAsStringAsync();
                }
                else
                {
                    string responseContent = await response.Content.ReadAsStringAsync();
                    throw new Exception($"Chyba: {response.StatusCode}, Detailní odpověď: {responseContent}");
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Chyba připojení: {ex.Message}");
            }
        }
        public async void ReturnNumberOfPossibleCalling()
        {
            string healthParameter = "/AplikacniSluzby/StavUctu";
             
            try
            {
                HttpResponseMessage response = await _httpClient.GetAsync(_apiUrl + healthParameter);

                if (response.IsSuccessStatusCode)
                {
                    var numberOfCallingJson= await response.Content.ReadAsStringAsync();
                    JObject obj = JObject.Parse(numberOfCallingJson);
                    HomeController.numberOfPossibleCalling = 500 - (int)obj["provedenoVolani"];
                }
                else
                {
                    string responseContent = await response.Content.ReadAsStringAsync();
                    throw new Exception($"Chyba: {response.StatusCode}, Detailní odpověď: {responseContent}");
                }
                
            }
            catch (Exception ex)
            {
                throw new Exception($"Chyba připojení: {ex.Message}");
            }
        }
        
        public async Task<string> GetParcelsByPolygon(List<(double x, double y)> coordinates)
        {
            //working

            try
            {
                
                if(HomeController.numberOfPossibleCalling > 1)
                    await CalculateApiParcels(_parcelCalculator.ConvertMainParcelAreaIntoJsonPoints(coordinates));

                List<string> sideParcels = _parcelCalculator.CalculateSideParcelAreaPoints(coordinates);

                if(HomeController.numberOfPossibleCalling < sideParcels.Count)
                for (int i = 0; i < sideParcels.Count; i++)
                {
                    await CalculateApiParcels(sideParcels[i]);
                }
                ReturnNumberOfPossibleCalling();
                
                if ((HomeController.parcelsFromAPIPolygon.Count+ sideParcels.Count) < HomeController.numberOfPossibleCalling)
                {
                await GetMainParametersOfParcels();

                ReturnNumberOfPossibleCalling();
                    //null error when from 49.3025594N, 16.1784547E to 49.2964586N, 16.1947625E
                    var (startPoint, endPoint) = await FindBeginningAndEndLandForPoints();

                    //ended here
                    var ratedParcels = _parcelCalculator.CalculateLandPoints();
                    var distinctRatedParcels = ratedParcels
                        .Where(parcel => parcel.DetailedParcel != null)
                        .GroupBy(parcel => parcel.DetailedParcel.Id)
                        .Select(group => group.First())
                        .ToList();

                    Dictionary<(double x, double y), BunkaVGridu> grid = await _parcelCalculator.GetGridOfRatedParcels(distinctRatedParcels);

                    var path = _parcelCalculator.DijkstraPath(grid, startPoint, endPoint);


                return string.Join(" -> ", path.Select(p => $"({(p.x*5):F2}, {(p.y*5):F2})"));
                }
                else
                {
                    throw new Exception("Překročený limit volání API");
                }
                
            }
            catch (NullReferenceException ex)
            { throw new Exception($"Chyba: Objekt je null – {ex.Message}");
            }
            catch (KeyNotFoundException ex)
            { throw new Exception($"Chyba: Klíč nebyl nalezen ve slovníku – {ex.Message}");
            }
            catch (Exception ex)
            { throw new Exception($"Neznámá chyba – {ex.Message}"); 
            }
            return "Parcely not implemented";
        }
        public async Task CalculateApiParcels(string json, bool isCalculatingFromPolygon=true)
        {
            try
            {
                string parcelUri = "/Parcely/Polygon";

                // Správné zakódování JSON do URL parametru
                string encodedCoordinates = Uri.EscapeDataString(json);

                HttpResponseMessage response = await _httpClient.GetAsync($"{_apiUrl}{parcelUri}?SeznamSouradnic={json}");

                if (!response.IsSuccessStatusCode)
                {
                    string error = await response.Content.ReadAsStringAsync();
                    Console.WriteLine($"❌ Chyba API: {response.StatusCode} - {error}");
                    return;
                }
                var responseContent = await response.Content.ReadAsStringAsync();
                using Stream stream = await response.Content.ReadAsStreamAsync();
                new JsonParser().ParsePolygonParcelData(stream,isCalculatingFromPolygon );
            }
            catch (Exception ex)
            {
                throw new Exception($"Chyba připojení: {ex.Message}");
            }
        }
        public async Task<((double x, double y) start, (double x, double y) end)> FindBeginningAndEndLandForPoints()
        {
            List<DetailedParcel> parcelsFromBeginningAndEndPointWithParameters = new List<DetailedParcel>();
            (double x, double y) definicniBodyStartPoint;
            (double x, double y) definicniBodyEndPointPoint;

            //implement a values from HomeController
            //error in endValueJSON using separate function
            string startValueJSON = CoordinateConversion.CreateMiniSquareJsonFromPoint(CoordinateConversion.ConvertCoordinatesFromMapToKNApiv2(HomeController.pointA[0], HomeController.pointA[1]));
            string endValueJSON = CoordinateConversion.CreateMiniSquareJsonFromPoint(CoordinateConversion.ConvertCoordinatesFromMapToKNApiv2(HomeController.pointB[0], HomeController.pointB[1]));
            
            if(HomeController.numberOfPossibleCalling > HomeController.parcelsFromBeginningAndEndPoint.Count + 2) { 
                
            HomeController.parcelsFromBeginningAndEndPoint.Clear();
            HomeController.parcelsFromBeginningAndEndPoint.Add(HomeController.parcelsFromAPIPolygon.ElementAt(0));
            HomeController.parcelsFromBeginningAndEndPoint.Add(HomeController.parcelsFromAPIPolygon.Last());
            
            //this also
            //await CalculateApiParcels(startValueJSON, false);
            //await CalculateApiParcels(endValueJSON, false);


                for (int i = 0; i < HomeController.parcelsFromBeginningAndEndPoint.Count; i++)
            {
                DetailedParcel detailed = await GetParcelFromId(HomeController.parcelsFromBeginningAndEndPoint[i].Id);
                parcelsFromBeginningAndEndPointWithParameters.Add(detailed);

            }
            }
            definicniBodyStartPoint.x = Double.Parse(parcelsFromBeginningAndEndPointWithParameters[0].DefinicniBod.X);
            definicniBodyStartPoint.y = Double.Parse(parcelsFromBeginningAndEndPointWithParameters[0].DefinicniBod.Y);

            definicniBodyEndPointPoint.x = Double.Parse(parcelsFromBeginningAndEndPointWithParameters[1].DefinicniBod.X);
            definicniBodyEndPointPoint.y = Double.Parse(parcelsFromBeginningAndEndPointWithParameters[1].DefinicniBod.Y);

            definicniBodyStartPoint.x = (int)Math.Floor(definicniBodyStartPoint.x / 5);
            definicniBodyStartPoint.y = (int)Math.Floor(definicniBodyStartPoint.y / 5);

            definicniBodyEndPointPoint.x = (int)Math.Floor(definicniBodyEndPointPoint.x / 5);
            definicniBodyEndPointPoint.y = (int)Math.Floor(definicniBodyEndPointPoint.y / 5);


            return (definicniBodyStartPoint, definicniBodyEndPointPoint);

        }
        public async void testingFunction()
        {//neukazuje přensý odkud kam
            string startValueJSON = CoordinateConversion.CreateMiniSquareJsonFromPoint(CoordinateConversion.ConvertCoordinatesFromMapToKNApiv2(HomeController.pointA[0], HomeController.pointA[1]));
            string endValueJSON = CoordinateConversion.CreateMiniSquareJsonFromPoint(CoordinateConversion.ConvertCoordinatesFromMapToKNApiv2(HomeController.pointB[0], HomeController.pointB[1]));

            await CalculateApiParcels(startValueJSON, false);
            await CalculateApiParcels(endValueJSON, false);
        }

    }


}
