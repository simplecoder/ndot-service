using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Ndot.Models;
using Newtonsoft.Json.Linq;

namespace Ndot.Helpers
{
    public interface IEdmundsApiAgent
    {
        VinApiData GetVinData(string vin);
    }

    public class EdmundsApiAgent : IEdmundsApiAgent
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiKey = ConfigurationManager.AppSettings["EdmundsApiKey"];

        public EdmundsApiAgent()
        {
            _httpClient = new HttpClient
                {
                    BaseAddress = new Uri("https://api.edmunds.com/"),
                    
                };
            _httpClient.DefaultRequestHeaders.Add("Accept", "application/json");

        }

        public VinApiData GetVinData(string vin)
        {
            var response = _httpClient.GetAsync("v1/api/toolsrepository/vindecoder?fmt=json&vin=" + vin + "&api_key=" + _apiKey).Result;
            var str = response.Content.ReadAsStringAsync().Result;
            var obj = JObject.Parse(str);
            return new VinApiData
                {
                    Year = (string) obj["styleHolder"][0]["year"],
                    Make = (string)obj["styleHolder"][0]["modelId"],
                    BodyType = (string)obj["styleHolder"][0]["categories"]["PRIMARY_BODY_TYPE"][0]
                };
        }
    }
}
