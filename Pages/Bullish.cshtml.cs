using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using BitPredictor.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace BitPredictor.Pages
{
    public class BullishModel : PageModel
    {

        public JsonPullService JsonPullService;
        public CryptoCalculationService CryptoCalculationService;
        public DateTimeOffset StartDate { get; set; }
        public DateTimeOffset EndDate { get; set; }
        public int LongestTrend { get; set; }
        public BullishModel(JsonPullService jsonPullService, CryptoCalculationService cryptoCalculationService)
        {
            JsonPullService = jsonPullService;
            CryptoCalculationService = cryptoCalculationService;
        }
        public void OnGet()
        {
            var dateStart = Request.Query["start"].ToString();
            var dateEnd = Request.Query["end"].ToString();
            if (dateStart != "")
            {
                StartDate = DateTimeOffset.ParseExact(dateStart, "yyyy-MM-dd", CultureInfo.InvariantCulture);
                EndDate = DateTimeOffset.ParseExact(dateEnd, "yyyy-MM-dd", CultureInfo.InvariantCulture);
                long unixStart = StartDate.ToUnixTimeSeconds();
                long unixEnd = EndDate.ToUnixTimeSeconds();
                LongestTrend = CryptoCalculationService.getLongestBearishTrendForTimes(JsonPullService.GetBTCData(unixStart, unixEnd).prices);
            }

        }

    }
}
