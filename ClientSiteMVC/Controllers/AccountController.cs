using ClientSiteMVC.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Text;

namespace ClientSiteMVC.Controllers
{
    public class AccountController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public AccountController(IHttpClientFactory httpClientFactory, IHttpContextAccessor httpContextAccessor)
        {
            _httpClientFactory = httpClientFactory;
            _httpContextAccessor = httpContextAccessor;
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var client = _httpClientFactory.CreateClient();
            var loginRequest = new StringContent(JsonConvert.SerializeObject(model), Encoding.UTF8, "application/json");

            // Make a POST request to the API for login
            var response = await client.PostAsync("https://localhost:7055/api/Auth/login", loginRequest);

            if (response.IsSuccessStatusCode)
            {
                var responseContent = await response.Content.ReadAsStringAsync();
                var tokenResponse = JsonConvert.DeserializeObject<JwtTokenResponse>(responseContent);

                // Store the JWT token in the session instead of a cookie
                _httpContextAccessor.HttpContext.Session.SetString("JWToken", tokenResponse.Token);

                // Redirect to the Products Dashboard on successful login
                return RedirectToAction("Dashboard", "Products");
            }
            else
            {
                // Add error message if login fails
                ModelState.AddModelError(string.Empty, "Invalid login attempt. Please check your credentials.");
            }

            // Return to the login view if the login fails
            return View(model);
        }
    }
}
