using System;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;

namespace GridPath.Services.ApiServices
{
    public class ParcelService
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiUrl;
        private readonly string _apiKey;
        private const double MIN_X = 904384;
        private const double MAX_X = 1246155;
        private const double MIN_Y = 403554;
        private const double MAX_Y = 932266;

        // Zajistíme, že offset vytvoří validní obdélník v rámci povolených hranic
        private const double RECTANGLE_OFFSET = 100; // 100 metrů

        public ParcelService(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _apiUrl = configuration["ApiSettings:ApiUrl"];
            _apiKey = configuration["ApiSettings:ApiKey"];
            _httpClient.DefaultRequestHeaders.Add("ApiKey", _apiKey);
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
            string neighbourParameters = "/Parcely/SousedniParcely/2235132101";
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
                string parcelUri = "/Parcely/Polygon";
                string json = string.Format(@"[
                {{ ""x"": {0}, ""y"": {1} }},
                {{ ""x"": {2}, ""y"": {3} }},
                {{ ""x"": {4}, ""y"": {5} }}
                ]", coordinates[0].x, coordinates[0].y, coordinates[1].x, coordinates[1].y, coordinates[2].x, coordinates[2].y);


                // Zakódování JSON stringu pro použití v query parametru
                string encodedCoordinates = Uri.EscapeDataString(json);

                // Sestavení celé URL s query parametrem
                string requestUrl = $"{_apiUrl}{parcelUri}?SeznamSouradnic={encodedCoordinates}";

                HttpResponseMessage response = await _httpClient.GetAsync(requestUrl);

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

    }

}
