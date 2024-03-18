using Azure.Core;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Skelbimu_sistema.Models;
using Skelbimu_sistema.ViewModels;
using System.Security.Cryptography;

namespace Skelbimu_sistema.Controllers
{
    //[Authorize] // Users must be logged in to access ad actions
    [Route("skelbimas")]
    public class ProductController : Controller
    {
        private readonly DataContext _dataContext;

        public ProductController(DataContext dataContext)
        {
            _dataContext = dataContext;
        }

        [HttpGet("kurimas")]
        public IActionResult Create()
        {
            ProductCreationRequest request = new ProductCreationRequest();

            return View(request);
        }



        [HttpPost("kurti")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SubmitCreate(ProductCreationRequest request)
        {
            // Validate the StartDate and EndDate
            if (request.EndDate < request.StartDate)
            {
                ModelState.AddModelError("StartDate", "Pradžios data negali būti vėlesnė nei pabaigos data");
            }
            if (!ModelState.IsValid)
            {
                return View("Create", request); // Return to the registration view with errors
            }
                // Create a new Product object
                Product product = new Product();

                // Map properties from the ViewModel to the Model
                product.Name = request.Name;
                product.Description = request.Description;
                product.Price = request.Price;
                product.ImageUrl = request.ImageUrl;
                product.StartDate = request.StartDate.ToString("yyyy-MM-dd"); // Format date
                product.EndDate = request.EndDate.ToString("yyyy-MM-dd");  // Format date
                product.Category = request.Category;

                _dataContext.Products.Add(product);
                await _dataContext.SaveChangesAsync();

                return RedirectToAction("Index", "Home");
        }

        //write me a method that will return one product by id
        public IActionResult Details(int id)
        {
            // Retrieve the product by id
            var product = _dataContext.Products.FirstOrDefault(p => p.Id == id);

            if (product == null)
            {
                return NotFound(); // Return a 404 Not Found if the product is not found
            }

            return View(product); // Return the product details view
        }

        // Helper method to get the current user's ID (adapt this based on your auth)
        private int GetCurrentUserId()
        {
            // Implementation depending on your authentication system 
            // Ex: return int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            throw new NotImplementedException("Update this with your user ID retrieval logic");
        }
        
    }
}
