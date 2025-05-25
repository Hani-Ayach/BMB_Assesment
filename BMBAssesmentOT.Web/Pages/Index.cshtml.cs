using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using BMBAssesmentOT.Web.Data;

namespace BMBAssesmentOT.Web.Pages
{
    public class IndexModel : PageModel
    {
        private readonly HttpClient _httpClient;

        public List<Order> Orders { get; set; }

        public IndexModel(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient("ApiGatewayClient");
        }

        public async Task OnGetAsync()
        {
            Orders = await _httpClient.GetFromJsonAsync<List<Order>>("orders");
        }
    }
}
