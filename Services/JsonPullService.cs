using BitPredictor.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace BitPredictor.Services
{

    public class JsonPullService
    {
        private static readonly HttpClient client = new HttpClient();
        static string apiURL = "https://api.coingecko.com/api/v3/coins/bitcoin/market_chart/range?vs_currency=eur";
        public static async Task<BTCPricesData> getStringFromSite(long from, long to)
        {

            //Get async http response, use content to deserialize the JSON to make the object for site usage
            //An hour should be added to ensure that the to day gets read correctly by the API
            HttpResponseMessage response = await client.GetAsync(string.Format(apiURL+"&from={0}&to={1}",from,to+3600));
            response.EnsureSuccessStatusCode();
            BTCPricesData data = JsonSerializer.Deserialize<BTCPricesData>(await response.Content.ReadAsStringAsync());
            return data;
        }

        public BTCPricesData GetBTCData(long from, long to)
        {
            var t = Task.Run(() => getStringFromSite(from, to));
            t.Wait();
            return t.Result;
        }

    }

}
