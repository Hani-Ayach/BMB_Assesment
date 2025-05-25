using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using BMBAssesmentOT.Web.Data;
using Microsoft.AspNetCore.Identity;

namespace BMBAssesmentOT.Web.Pages
{
    public class CreateModel : PageModel
    {
        private readonly HttpClient _httpClient;
        private readonly UserManager<IdentityUser> _userManager;
        public CreateModel(IHttpClientFactory factory, UserManager<IdentityUser> userManager)
        {
            _httpClient = factory.CreateClient("ApiGatewayClient");
            _userManager = userManager;
        }

        [BindProperty]
        public Order Order { get; set; }

        public List<Client> Clients { get; set; } = new();

        public async Task OnGetAsync()
        {
            Order = new Order { OrderDate = DateTime.Today };

            // Get all clients from Identity API
            Clients = _userManager.Users.Select(u => new Client
            {
                Id = u.Id,
                Email = u.Email
            }).ToList();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
                return Page();

            var response = await _httpClient.PostAsJsonAsync("orders", Order);

            if (response.IsSuccessStatusCode)
                return RedirectToPage("/Orders");

            ModelState.AddModelError(string.Empty, "Failed to create order.");
            return Page();
        }
    }
}
