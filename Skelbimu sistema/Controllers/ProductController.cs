using Azure.Core;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Skelbimu_sistema.Models;
using Skelbimu_sistema.ViewModels;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;

namespace Skelbimu_sistema.Controllers
{
    [Route("skelbimas")]
    public class ProductController : Controller
    {
        private readonly DataContext _dataContext;
        private readonly int cookieExpirationTime = 7; // Days
        private readonly int userSearchHistorySize = 256;


        public ProductController(DataContext dataContext)
        {
            _dataContext = dataContext;
        }

        [Authorize]
        [HttpGet("kurimas")]
        public async Task<IActionResult> Create()
        {
            ProductCreationRequest request = new ProductCreationRequest();

            int userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var user = await _dataContext.Users.FindAsync(userId);

            ViewData["UserRole"] = user.Role;

            return View(request);
        }

        [Authorize]
        [HttpPost("kurti")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SubmitCreate(ProductCreationRequest request)
        {
            // Validate the StartDate and EndDate
            if (request.EndDate < request.StartDate)
            {
                ModelState.AddModelError("StartDate", "Pradžios data negali būti vėlesnė nei pabaigos data");
            }

            // Get user and check if not blocked
            int userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            User user = _dataContext.Users.Find(userId)!;
            if (user.Blocked)
            {
                ModelState.AddModelError("Name", "Jūs negalite kurti naujų skelbimų, nes esate užblokuotas");
            }

            if (!ModelState.IsValid)
            {
                return View("Create", request); // Return to the registration view with errors
            }
            
            Product product = new Product();

            product.Name = request.Name;
            product.Description = request.Description;
            product.Price = request.Price;
            product.ImageUrl = request.ImageUrl;
            product.StartDate = request.StartDate.ToString("yyyy-MM-dd"); // Format date
            product.EndDate = request.EndDate.ToString("yyyy-MM-dd");  // Format date
            product.Category = request.Category;
            product.User = user;
            product.State = 0;

            _dataContext.Products.Add(product);
            await _dataContext.SaveChangesAsync();

            // Set success message
            TempData["SuccessMessage"] = "Produktas patalpintas sėkmingai!";

            return RedirectToAction("Index", "Home");
        }

        [Route("product/details")]
        public IActionResult Details(int id)
        {
            // Retrieve the product by id
            var product = _dataContext.Products.Include(product => product.User).FirstOrDefault(product => product.Id == id);

            if (product == null)
            {
                return NotFound();
            }

            return View(product); 
        }

        [Authorize]
        [Route("Product/ViewInventory")]
        public async Task<IActionResult> ViewInventory()
        {
            int userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var user = await _dataContext.Users.FindAsync(userId);

            var userInventory = await _dataContext.Products
                                .Where(product => product.UserId == userId)
                                .ToListAsync();
            ViewData["UserId"] = userId;
            ViewData["UserRole"] = user.Role;

            return View(userInventory);
        }

        [Authorize]
        [HttpPost]
        [Route("Product/ChangeState")]
        public IActionResult ChangeState(int productId)
        {
            var product = _dataContext.Products.FirstOrDefault(p => p.Id == productId);

            if (product == null)
            {
                return NotFound(); 
            }

            product.State = (product.State == ProductState.Active) ? ProductState.Reserved : ProductState.Active;

            _dataContext.SaveChanges();

            return RedirectToAction("ViewInventory");
        }

        [Authorize]
        [HttpPost]
        [Route("Product/Delete/{id}")]
        [ValidateAntiForgeryToken]
        public IActionResult Delete(int id)
        {
            var product = _dataContext.Products.FirstOrDefault(p => p.Id == id);

            if (product == null)
            {
                return NotFound();
            }

            try
            {
                _dataContext.Products.Remove(product);
                _dataContext.SaveChanges();
                return Ok();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error deleting product: " + ex.Message);
                return StatusCode(500); 
            }
        }

        [Authorize]
        [HttpGet]
        [Route("Product/Edit")]
        public IActionResult Edit(int id)
        {
            // Retrieve the product by id
            var product = _dataContext.Products.FirstOrDefault(p => p.Id == id);

            if (product == null)
            {
                return NotFound(); // Product not found
            }

            // Pass the product data to the edit view
            return View(product);
        }

        [Authorize]
        [HttpPost]
        [Route("Product/Edit")]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(Product editedProduct)
        {
            // Retrieve the existing product from the database
            var product = _dataContext.Products.FirstOrDefault(p => p.Id == editedProduct.Id);

            if (product == null)
            {
                return NotFound(); // Product not found
            }
            //check if editedproduct name is not empty
            if (string.IsNullOrEmpty(editedProduct.Name))
            {
                ModelState.AddModelError("Name", "Pavadinimas negali būti tuščias");
            }
            //check if price is not negative
            if (editedProduct.Price < 0)
            {
                ModelState.AddModelError("Price", "Kaina negali būti neigiamas skaičius");
            }
            // Update product details with the edited values
            product.Name = editedProduct.Name;
            product.Description = editedProduct.Description;
            product.Price = editedProduct.Price;
            product.ImageUrl = editedProduct.ImageUrl;
            product.Category = editedProduct.Category;

            // Save changes to the database
            _dataContext.SaveChanges();
            TempData["SuccessMessage"] = "Skelbimas atnaujintas sėkmingai";
            return RedirectToAction("ViewInventory"); // Redirect to the inventory view after editing
        }





        /// <summary>
        /// Filter action when search button is pressed
        /// </summary>
        /// <param name="searchString">User search words</param>
        /// <returns>View</returns>
        [Route("product/filter")]
        public IActionResult Filter(string searchString)
        {
            if (string.IsNullOrEmpty(searchString))
            {
                return View("SearchResults", new List<Product>());
            }
            // Convert the search string to lowercase for case-insensitive comparison
            var searchLower = searchString.ToLower();

            // Get all products from the database
            var allProducts = _dataContext.Products;

            // Filter products in memory
            var filteredProducts = allProducts
                .Where(p =>
                    p.Name.ToLower().Contains(searchLower) ||
                    p.Description.ToLower().Contains(searchLower) ||
                    Enum.GetName(typeof(Category), p.Category)!.ToLower() == searchLower)
                .Include(p => p.Reports)
                .Where(p => p.State == ProductState.Active)
                .ToList();

            // Hide the products that have been reported by logged in user
            if (User.Identity?.IsAuthenticated ?? false)
            {
                var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
                filteredProducts = filteredProducts.Where(p => !p.Reports.Any(r => r.UserId == userId)).ToList();
            }

            if (User.Identity?.IsAuthenticated ?? false)
            {
                // Save search history if user is logged in
                var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
                SaveSearchHistory(searchString, userId);
            }

            return View("SearchResults", filteredProducts);
        }

        /// <summary>
        /// Saves user search to cookie and database
        /// </summary>
        /// <param name="searchString">User search words</param>
        [Authorize]
        private void SaveSearchHistory(string searchString, int userId)
        {
            // Retrieve the search history cookie for the current user
            var searchHistoryCookie = Request.Cookies["SearchHistory_" + userId];
            List<string> searchHistory = new List<string>();

            if (searchHistoryCookie != null && !string.IsNullOrEmpty(searchHistoryCookie))
            {
                // Deserialize search history from cookie
                searchHistory = searchHistoryCookie.Split(',').ToList();
            }

            // Add the current search to the search history
            if (!string.IsNullOrEmpty(searchString) && !searchHistory.Contains(searchString))
            {
                searchHistory.Add(searchString);
            }

            // Update the search history cookie for the current user
            Response.Cookies.Append("SearchHistory_" + userId, string.Join(',', searchHistory), new Microsoft.AspNetCore.Http.CookieOptions
            {
                Expires = DateTimeOffset.Now.AddDays(cookieExpirationTime)
            });

            // Save the search history to the database for the current user
            SaveSearchToDatabase(searchString, userId);
        }

        [Route("Product/SearchByCategory")]
        public IActionResult CategorySearch(Category? selectedCategory)
        {
            if (selectedCategory == null)
            {
                ViewBag.SelectedCategory = null;
                return View("SearchByCategory", new List<Product>());
            }

            ViewBag.SelectedCategory = selectedCategory;
            List<Product> products = _dataContext.Products
                .Where(p => p.Category == selectedCategory)
                .Include(p => p.Reports)
                .Where(p => p.State == ProductState.Active)
                .ToList();

            // Hide the products that have been reported by logged in user
            if (User.Identity?.IsAuthenticated ?? false)
            {
                var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
                products = products.Where(p => !p.Reports.Any(r => r.UserId == userId)).ToList();
            }

            return View("SearchByCategory", products);
        }

        [Route("Product/FilterByPrice")]
        public IActionResult FilterByPrice(double minPrice, double maxPrice, Category? selectedCategory)
        {
            if (selectedCategory == null)
            {
                // Jei kategorija nepasirinkta, grąžinkite tuščius rezultatus
                return View("SearchByCategory", new List<Product>());
            }

            ViewBag.SelectedCategory = selectedCategory;
            var filteredProducts = _dataContext.Products
                .Where(p => p.Price >= minPrice && p.Price <= maxPrice && p.Category == selectedCategory)
                .Include(p => p.Reports)
                .Where(p => p.State == ProductState.Active)
                .ToList();

            // Hide the products that have been reported by logged in user
            if (User.Identity?.IsAuthenticated ?? false)
            {
                var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
                filteredProducts = filteredProducts.Where(p => !p.Reports.Any(r => r.UserId == userId)).ToList();
            }

            return View("SearchByCategory", filteredProducts);
        }

        [Route("Product/FilterByPriceString")]
        public IActionResult FilterByPriceString(double minPrice, double maxPrice, string searchString)
        {
            // Check if the search string is empty
            if (string.IsNullOrEmpty(searchString))
            {
                // If the search string is empty, return an empty list
                return View("SearchResults", new List<Product>());
            }

            // Convert the search string to lowercase for case-insensitive comparison
            var searchLower = searchString.ToLower();

            // Get all products from the database
            var allProducts = _dataContext.Products;

            // Filter products by price range and search string
            var filteredProducts = allProducts
                .Where(p => p.Price >= minPrice && p.Price <= maxPrice &&
                    (p.Name.ToLower().Contains(searchLower) ||
                    p.Description.ToLower().Contains(searchLower) ||
                    Enum.GetName(typeof(Category), p.Category)!.ToLower() == searchLower))
                .Include(p => p.Reports)
                .Where(p => p.State == ProductState.Active)
                .ToList();

            // Hide the products that have been reported by logged in user
            if (User.Identity?.IsAuthenticated ?? false)
            {
                var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
                filteredProducts = filteredProducts.Where(p => !p.Reports.Any(r => r.UserId == userId)).ToList();
            }

            // Return the filtered products to the view
            return View("SearchResults", filteredProducts);
        }

        /// <summary>
        /// Saves user search phrase into database
        /// </summary>
        /// <param name="searchString">User search words</param>
        /// <param name="userId">User id</param>
        [Authorize]
        private void SaveSearchToDatabase(string searchString, int userId)
        {
            var currentUser = _dataContext.Users.FirstOrDefault(p => p.Id == userId);
            if (currentUser != null)
            {
                string newKeywords = currentUser.SearchKeyWords + searchString + ",";

                // Check if the total length exceeds the limit
                if (newKeywords.Length >= userSearchHistorySize)
                {
                    // Find the index of the delimiter within the excess characters
                    int delimiterIndex = newKeywords.IndexOf(',', newKeywords.Length - (newKeywords.Length - userSearchHistorySize));

                    // If delimiterIndex is -1, it means there's no delimiter within the excess characters,
                    // in this case, truncate the string to 'userSearchHistorySize' characters
                    if (delimiterIndex == -1)
                    {
                        newKeywords = newKeywords.Substring(0, 256);
                    }
                    else
                    {
                        // Remove characters from the beginning until the delimiter (including the delimiter)
                        newKeywords = newKeywords.Substring(delimiterIndex + 1);
                    }
                }

                currentUser.SearchKeyWords = newKeywords;
                _dataContext.SaveChanges();
            }
        }


    }

}
