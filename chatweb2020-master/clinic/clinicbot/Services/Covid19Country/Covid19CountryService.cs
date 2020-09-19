using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace clinicbot.Services.Covid19Country
{       // SERVICIO
    public class Covid19CountryService: ICovid19CountryService
    {
        /// <summary>
        /// Servicio https://corona.lmao.ninja documentatción
        /// </summary>
        /// <param name="country"></param>
        /// <returns></returns>
        public async Task<Covid19CountryModel> Execute(string country)
        {
            string uri = "https://corona.lmao.ninja/v2/countries";
            try
            {
                using (var client = new HttpClient())
                using (var request = new HttpRequestMessage())
                {
                    request.Method = HttpMethod.Get;
                    request.RequestUri = new Uri(uri);
                    var response = await client.SendAsync(request);
                    var responseBody = await response.Content.ReadAsStringAsync();
                    var modelTemp = JsonConvert.DeserializeObject<List<Covid19CountryModel>>(responseBody);

                    var model = modelTemp.FirstOrDefault(x => x.country.ToLower() == country.ToLower());
                    return model;
                }
            }
            catch (Exception e)
            {
                return new Covid19CountryModel();
            }
        }
    }
}
