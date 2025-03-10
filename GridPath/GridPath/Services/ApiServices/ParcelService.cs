using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using GridPath.Helper;
using GridPath.Models;
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
        public async Task<string> GetParcelFromId()
        {
            string parametersId = "/Parcely/2235132101";
            try
            {
                
                HttpResponseMessage response = await _httpClient.GetAsync(_apiUrl+parametersId);

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
                
                await CalculateApiParcels(_parcelCalculator.CalculateMainParcelAreaPoints(coordinates));            

                //await CalculateApiParcels(_parcelCalculator.CalculateMainParcelAreaPoints(coordinates));
                List<string> sideParcels = _parcelCalculator.CalculateSideParcelAreaPoints(coordinates);
               // await CalculateApiParcels(sideParcels[0]);
                for (int i = 0; i < sideParcels.Count; i++)
                {
                    await CalculateApiParcels(sideParcels[i]);
                }

                return "Parcely not implemented";


            }
            catch (Exception ex)
            {
                throw new Exception($"Chyba připojení: {ex.Message}");
            }
        }
        public async Task CalculateApiParcels(string json)
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
                new JsonParser().ParseParcelData(stream);
            }
            catch (Exception ex)
            {
                throw new Exception($"Chyba připojení: {ex.Message}");
            }
        }

    }

}
