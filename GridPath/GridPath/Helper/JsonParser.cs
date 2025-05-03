using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.Text;
using GridPath.Services.ApiServices;
using GridPath.Controllers;
using GridPath.Models.PolygonParcels;
using GridPath.Models;
using GridPath.Models.Parcels;
using System.ComponentModel;
using System.Collections.Specialized;
using Microsoft.AspNetCore.Server.Kestrel.Core;

namespace GridPath.Helper
{
    public class JsonParser
    {
        public async void ParsePolygonParcelData(Stream stream, bool AddToHomeController=true)
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
                                if(AddToHomeController)
                                    HomeController.parcelsFromAPIPolygon.Add(parcel);
                                else if (AddToHomeController == false)
                                    HomeController.parcelsFromBeginningAndEndPoint.Add(parcel);
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
            try
            {
                using StreamReader reader = new StreamReader(stream, Encoding.UTF8);
                using JsonTextReader jsonReader = new JsonTextReader(reader);
                DetailedParcel parcel = null;
                while (jsonReader.Read())
                {
                    // Hledáme začátek pole "data"
                    if (jsonReader.TokenType == JsonToken.PropertyName && jsonReader.Value?.ToString() == "data")
                    {
                        jsonReader.Read(); // Přesun na začátek pole
                        JObject jsonParcel = JObject.Load(jsonReader);
                        CadastralArea cadastralArea = new CadastralArea(jsonParcel["katastralniUzemi"]["kod"].ToString(), jsonParcel["katastralniUzemi"]["nazev"].ToString());
                        LV lv = new LV(jsonParcel["lv"]["id"].ToString(), jsonParcel["lv"]["cislo"].ToString(), cadastralArea);
                        DruhPozemku fieldType = new DruhPozemku(jsonParcel["druhPozemku"]["kod"].ToString(), jsonParcel["druhPozemku"]["nazev"].ToString());
                        DefinicniBod point = new DefinicniBod(jsonParcel["definicniBod"]["id"].ToString(), jsonParcel["definicniBod"]["x"].ToString(), jsonParcel["definicniBod"]["y"].ToString());

                    ZpusobyOchrany ochrana = null;

                    var zpusobyOchranyArray = jsonParcel["zpusobyOchrany"] as JArray;
                    if (zpusobyOchranyArray != null && zpusobyOchranyArray.Count > 0)
                    {
                        var prvni = zpusobyOchranyArray[0];
                        var kod = prvni["kod"]?.ToString();
                        var nazev = prvni["nazev"]?.ToString();

                        if (!string.IsNullOrEmpty(kod) && !string.IsNullOrEmpty(nazev))
                        {
                            ochrana = new ZpusobyOchrany(kod, nazev);
                        }
                    }


                        parcel = new DetailedParcel(jsonParcel["id"].ToString(), jsonParcel["typParcely"].ToString(), jsonParcel["druhCislovaniParcely"].ToString(),
                        jsonParcel["kmenoveCisloParcely"].ToString(), jsonParcel["poddeleniCislaParcely"].ToString(), cadastralArea,
                        jsonParcel["vymera"].ToString(), lv, fieldType, jsonParcel["stavba"].ToString(), jsonParcel["pravoStavby"].ToString(), point,
                        jsonParcel["zpusobVyuziti"].ToString(), ochrana, jsonParcel["rizeniPlomby"].ToString());

                    }



                }
                return parcel;
            }
            catch (Exception ex)
            {
                throw new Exception($"Chyba při parsování dat: {ex.Message}");

            }
        }
    }
}
