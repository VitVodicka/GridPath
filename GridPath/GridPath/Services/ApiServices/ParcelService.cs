using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using GridPath.Controllers;
using GridPath.Helper;
using GridPath.Models;
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
            string parcelSearchParameters = "/Parcely/Vyhledani?KodKatastralnihoUzemi=778214&TypParcely=PKN&DruhCislovaniParcely=2&KmenoveCisloParcely=1766&PoddeleniCislaParcely=3";
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
        public async Task<DetailedParcel> GetParcelFromId(string id )
        {
            try {
            string parametersId = "/Parcely/"+id;
            HttpResponseMessage response = await _httpClient.GetAsync(_apiUrl + parametersId);
            response.EnsureSuccessStatusCode();

            using Stream stream = await response.Content.ReadAsStreamAsync();
            return await new JsonParser().ParseDetailedParcelData(stream);
            }
            catch (HttpRequestException ex)
            {
                // Chyba při volání HTTP (např. server nedostupný, 404, atd.)
                Console.WriteLine("Chyba při volání API: " + ex.Message);
                return null;
            }
            catch (TaskCanceledException ex)
            {
                // Timeout požadavku
                Console.WriteLine("Požadavek trval příliš dlouho: " + ex.Message);
                return null;
            }
            catch (JsonException ex)
            {
                // Chyba při parsování JSONu
                Console.WriteLine("Chyba při čtení odpovědi: " + ex.Message);
                return null;
            }
            catch (Exception ex)
            {
                // Vše ostatní
                Console.WriteLine("Neočekávaná chyba: " + ex.Message);
                return null;
            }




        }
        public async Task GetMainParametersOfParcels()
        {
            int limit = 10;
            int count = 0;

            foreach (var parcel in HomeController.parcelsFromAPIPolygon)
            {
                if (count >= limit)
                   break;

                HomeController.parcelsParameters.Add(await GetParcelFromId(parcel.Id));
                count++;
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
        /// <summary>
        /// 
        /// </summary>
        /// <param name="coordinates">list of X and Y locations in KN API coordinates(EPSG:5514)</param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public async Task<string> GetParcelsByPolygon(List<(double x, double y)> coordinates)
        {
            try
            {
                //some changes

                await CalculateApiParcels(_parcelCalculator.CalculateMainParcelAreaPoints(coordinates));
                /*List<string> sideParcels = _parcelCalculator.CalculateSideParcelAreaPoints(coordinates);
                for (int i = 0; i < sideParcels.Count; i++)
                {
                    await CalculateApiParcels(sideParcels[i]);
                }*/
                await GetMainParametersOfParcels();
                //TODO dat tam místa od do
                //var something = await _parcelCalculator.GetGridOfRatedParcels(_parcelCalculator.CalaculateLandPoints());
                /*var (startPoint, endPoint) = await FindBeginningAndEndLandForPoints();

                var gridInt = await _parcelCalculator.GetGridOfRatedParcels(_parcelCalculator.CalaculateLandPoints());
                var gridDouble = gridInt.ToDictionary(k => ((double)k.Key.x, (double)k.Key.y), v => v.Value);
                var path = _parcelCalculator.DijkstraPath(gridDouble, startPoint, endPoint);*/

                /*CoordinateConversion.ConvertCoordinatesFromMapToCoordinatesInGrid(
                    CoordinateConversion.ConvertCoordinatesFromMapToKNApiv2(16.23, 49.29)), 

                CoordinateConversion.ConvertCoordinatesFromMapToCoordinatesInGrid(
                    CoordinateConversion.ConvertCoordinatesFromMapToKNApiv2(16.23, 49.28)));*/

                var value = _parcelCalculator.CalculateLandPoints();
                return "Parcely not implemented";


            }
            catch (NullReferenceException ex)
            {
                throw new Exception($"Chyba: Objekt je null – {ex.Message}");
            }
            catch (KeyNotFoundException ex)
            {
                throw new Exception($"Chyba: Klíč nebyl nalezen ve slovníku – {ex.Message}");
            }
            catch (Exception ex)
            {
                throw new Exception($"Neznámá chyba – {ex.Message}");
            }
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
            //prvně převod na ty body zyčáteční a konečný bod
            //potom dát +0.1 a udělat z toho malý čtvereček
            //zparsovat do JSONu
            //zavolat api
            //dostat z toho pocatecni body

            string startValueJSON = CoordinateConversion.CreateMiniSquareJsonFromPoint(CoordinateConversion.ConvertCoordinatesFromMapToKNApiv2(16.23, 49.29));
            string endValueJSON = CoordinateConversion.CreateMiniSquareJsonFromPoint(CoordinateConversion.ConvertCoordinatesFromMapToKNApiv2(16.23, 49.28));
            await CalculateApiParcels(startValueJSON, false);
            await CalculateApiParcels(startValueJSON, false);

            for (int i = 0; i < HomeController.parcelsFromBeginningAndEndPoint.Count; i++)
            {
                DetailedParcel detailed = await GetParcelFromId(HomeController.parcelsFromBeginningAndEndPoint[i].Id);
                parcelsFromBeginningAndEndPointWithParameters.Add(detailed);

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

    }


}
