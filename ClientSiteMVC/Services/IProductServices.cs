using ClientSiteMVC.Models;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ClientSiteMVC.Services
{
    public interface IProductServices
    {
        Task<IEnumerable<Product>> GetAllProductsAsync(string token);
        Task<Product> GetProductByIdAsync(int id, string token);
        Task<bool> AddProductAsync(Product product, IFormFile imageFile, string token);
        Task<string> UpdateProductAsync(Product product, IFormFile imageFile, string token);
        Task<string> DeleteProductAsync(int id, string token);
    }
}
