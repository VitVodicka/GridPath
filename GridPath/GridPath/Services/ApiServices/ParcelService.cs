using System;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

namespace GridPath.Services.ApiServices
{
    public class ParcelService
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiUrl;
        private readonly string _apiKey;

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
        public async Task<string> GetParcelsByPolygon()
        {
            //TODO: Implementovat metodu pro získání parcel podle polygonu
            try
            {
                HttpResponseMessage response = await _httpClient.GetAsync(_apiUrl);

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
