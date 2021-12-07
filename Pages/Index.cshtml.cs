using BitPredictor.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BitPredictor.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;
        public JsonPullService JsonPullService;
        public List<List<double>> Prices { get; private set; }
        public IndexModel(ILogger<IndexModel> logger,
            JsonPullService jsonPullService)
        {
            _logger = logger;
            JsonPullService = jsonPullService;
        }

        public void OnGet()
        {
            //Prices = JsonPullService.getPrices();
        }
    }
}
