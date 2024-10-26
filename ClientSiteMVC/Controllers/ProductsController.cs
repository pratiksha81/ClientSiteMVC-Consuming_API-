using ClientSiteMVC.Models;
using ClientSiteMVC.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace ClientSiteMVC.Controllers
{
    public class ProductsController : Controller
    {
        private readonly IProductServices _productService;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public ProductsController(IProductServices productService, IHttpContextAccessor httpContextAccessor)
        {
            _productService = productService;
            _httpContextAccessor = httpContextAccessor;
        }

        // GET: Dashboard
        [HttpGet]
        public async Task<IActionResult> Dashboard()
        {
            var token = _httpContextAccessor.HttpContext.Session.GetString("JWToken");
            if (string.IsNullOrEmpty(token)) return RedirectToAction("Login", "Account");

            var products = await _productService.GetAllProductsAsync(token);
            return View(products);
        }

        // POST: Add Product
        
        [HttpPost]
        public async Task<IActionResult> AddProduct(Product product, IFormFile productImage)
        {
            var token = _httpContextAccessor.HttpContext.Session.GetString("JWToken");

            if (string.IsNullOrEmpty(token))
            {
                return RedirectToAction("Login", "Account");
            }

            var result = await _productService.AddProductAsync(product, productImage, token);

            if (result == null)
            {
                ModelState.AddModelError("", "Error adding product.");
                return RedirectToAction("Dashboard");
            }
            return RedirectToAction("Dashboard");

        }
        // POST: Edit Product
        [HttpPost]
        public async Task<IActionResult> EditProduct(Product model, IFormFile imageFile)
        {
            var token = _httpContextAccessor.HttpContext.Session.GetString("JWToken");
            if (string.IsNullOrEmpty(token)) return RedirectToAction("Login", "Account");

            if (ModelState.IsValid)
            {
                var result = await _productService.UpdateProductAsync(model, imageFile, token);
                if (string.IsNullOrEmpty(result))
                {
                    return RedirectToAction("Dashboard"); // Success: Return to the Dashboard
                }

                // Display error message
                ModelState.AddModelError(string.Empty, result);
            }

            ViewBag.Error = "Failed to update product.";
            return View("Dashboard", await _productService.GetAllProductsAsync(token));
        }

        // POST: Delete Product
        [HttpPost]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            var token = _httpContextAccessor.HttpContext.Session.GetString("JWToken");
            if (string.IsNullOrEmpty(token)) return RedirectToAction("Login", "Account");

            var result = await _productService.DeleteProductAsync(id, token);
            if (string.IsNullOrEmpty(result))
            {
                return RedirectToAction("Dashboard"); // Success: Return to the Dashboard
            }

            // Display error message
            ModelState.AddModelError(string.Empty, result);
            ViewBag.Error = "Failed to delete product.";
            return View("Dashboard", await _productService.GetAllProductsAsync(token));
        }
    }
}
