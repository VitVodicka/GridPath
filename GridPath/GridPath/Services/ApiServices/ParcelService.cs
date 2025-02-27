using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
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

        
        // Zajistíme, že offset vytvoří validní obdélník v rámci povolených hranic
        public static HashSet<Parcel> parcelsFromAPI = new HashSet<Parcel>();
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
                response.EnsureSuccessStatusCode();

                using Stream stream = await response.Content.ReadAsStreamAsync();
                using StreamReader reader = new StreamReader(stream, Encoding.UTF8);
                using JsonTextReader jsonReader = new JsonTextReader(reader);


                while (jsonReader.Read())
                {
                    // Hledáme začátek pole "data"
                    if (jsonReader.TokenType == JsonToken.PropertyName && jsonReader.Value?.ToString() == "data")
                    {
                        jsonReader.Read(); // Přesun na začátek pole

                        if (jsonReader.TokenType == JsonToken.StartArray)
                        {
                            while (jsonReader.Read())
                            {
                                if (jsonReader.TokenType == JsonToken.StartObject)
                                {
                                    JObject jsonParcel = JObject.Load(jsonReader);
                                    CadastralArea cadastralArea = new CadastralArea(jsonParcel["katastralniUzemi"]["kod"].ToString(), jsonParcel["katastralniUzemi"]["nazev"].ToString());
                                    Parcel parcel = new Parcel(jsonParcel["id"].ToString(), jsonParcel["typParcely"].ToString(), jsonParcel["druhCislovaniParcely"].ToString(),
                                        jsonParcel["kmenoveCisloParcely"].ToString(), jsonParcel["poddeleniCislaParcely"].ToString(), cadastralArea);
                                    

                                    parcelsFromAPI.Add(parcel);
                                    Console.WriteLine(parcel.ToString()); // Výpis jednotlivé parcely
                                }
                                else if (jsonReader.TokenType == JsonToken.EndArray)
                                {
                                    break;
                                }
                            }
                        }
                    }
                }
                return "asdasda";
            }
            catch (Exception ex)
            {
                throw new Exception($"Chyba připojení: {ex.Message}");
            }
        }

    }

}
