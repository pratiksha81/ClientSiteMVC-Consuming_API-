using ClientSiteMVC.Models;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Globalization;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace ClientSiteMVC.Services
{
    public class ProductServices : IProductServices
    {
        private readonly HttpClient _httpClient;

        public ProductServices(HttpClient httpClient)
        {
            _httpClient = httpClient;
           
            
        }

        // Get all products
        public async Task<IEnumerable<Product>> GetAllProductsAsync(string token)
        {
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            var response = await _httpClient.GetAsync("https://localhost:7055/api/Products");
            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<IEnumerable<Product>>(content);
        }

        // Get a product by ID
        public async Task<Product> GetProductByIdAsync(int id, string token)
        {
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            var response = await _httpClient.GetAsync($"https://localhost:7055/api/Products/{id}");
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<Product>(content);
            }
            return null;
        }

        // Add a product
        public async Task<bool> AddProductAsync(Product product, IFormFile imageFile, string token)

        {

            // Set the Bearer token for authorization if required

            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            // Create multipart form data content

            var formData = new MultipartFormDataContent();

            // Add product properties to the form data

            formData.Add(new StringContent(product.Id.ToString()), "Id");

            formData.Add(new StringContent(product.Name), "Name");

            formData.Add(new StringContent(product.Price.ToString()), "Price");

            // Handle image file if provided

            if (imageFile != null)

            {

                using (var stream = new MemoryStream())

                {

                    await imageFile.CopyToAsync(stream);

                    var imageContent = new ByteArrayContent(stream.ToArray());

                    imageContent.Headers.ContentType = new MediaTypeHeaderValue(imageFile.ContentType);

                    formData.Add(imageContent, "productImage", imageFile.FileName);

                }

            }

            // Send POST request to the API endpoint

            var response = await _httpClient.PostAsync("https://localhost:7055/api/Products", formData);

            // Return whether the API request was successful

            return response.IsSuccessStatusCode;

        }





        // Update a product
        public async Task<string> UpdateProductAsync(Product product, IFormFile imageFile, string token)
        {
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var formData = new MultipartFormDataContent
            {
                { new StringContent(product.Name), "Name" },
                { new StringContent(product.Price.ToString()), "Price" }
            };

            if (imageFile != null && imageFile.Length > 0)
            {
                using (var stream = new MemoryStream())
                {
                    await imageFile.CopyToAsync(stream);
                    var imageContent = new ByteArrayContent(stream.ToArray());
                    imageContent.Headers.ContentType = new MediaTypeHeaderValue(imageFile.ContentType);
                    formData.Add(imageContent, "ProductImage", imageFile.FileName); // Key should match the API parameter
                }
            }

            var response = await _httpClient.PutAsync($"https://localhost:7055/api/Products/{product.Id}", formData);

            return response.IsSuccessStatusCode
                ? "Product updated successfully!"
                : $"Error updating product: {await response.Content.ReadAsStringAsync()}";
        }

        // Delete a product
        public async Task<string> DeleteProductAsync(int id, string token)
        {
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            var response = await _httpClient.DeleteAsync($"https://localhost:7055/api/Products/{id}");
            return response.IsSuccessStatusCode
                ? "Product deleted successfully!"
                : $"Error deleting product: {await response.Content.ReadAsStringAsync()}";
        }
    }
}
