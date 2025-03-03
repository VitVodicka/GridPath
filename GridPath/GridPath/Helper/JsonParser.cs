using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.Text;
using GridPath.Services.ApiServices;
using GridPath.Controllers;
using GridPath.Models.PolygonParcels;
using GridPath.Models;
using GridPath.Models.Parcels;

namespace GridPath.Helper
{
    public class JsonParser
    {
        public async void ParsePolygonParcelData(Stream stream)
        {
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

                                HomeController.parcelsFromAPIPolygon.Add(parcel);
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
        }

        public async Task<DetailedParcel> ParseDetailedParcelData(Stream stream)
        {
            //TODO
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

                                HomeController.parcelsFromAPIPolygon.Add(parcel);
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
        }

    }
}
