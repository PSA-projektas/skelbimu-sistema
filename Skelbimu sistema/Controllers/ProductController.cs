using Azure.Core;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Skelbimu_sistema.Models;
using Skelbimu_sistema.ViewModels;
using System.Security.Claims;
using System.Security.Cryptography;

namespace Skelbimu_sistema.Controllers
{
    //[Authorize] // Users must be logged in to access ad actions
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
            int userId = GetCurrentUserId();
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
            product.UserId = userId;

            _dataContext.Products.Add(product);
            await _dataContext.SaveChangesAsync();

            return RedirectToAction("Index", "Home");
        }

        [Route("product/details")]
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

        [Route("Product/ViewInventory")]
        public async Task<IActionResult> ViewInventory()
        {
            int userId = GetCurrentUserId();
            if (userId == null)
            {
                return NotFound();
            }

            var userInventory = await _dataContext.Products
                                .Where(p => p.UserId == userId)
                                .ToListAsync();
            ViewData["UserId"] = userId;

            return View(userInventory);
        }


        /// <summary>
        /// Returns logged in user's id
        /// </summary>
        /// <returns>Id</returns>
        public int GetCurrentUserId()
        {
            // Retrieve user cookie
            var userCookie = HttpContext.Request.Cookies["User"];

            if (!string.IsNullOrEmpty(userCookie))
            {
                // Parse user ID from cookie
                var userInfo = JsonConvert.DeserializeObject<dynamic>(userCookie);
                int userId = userInfo.Id;

                return userId;
            }
            else
            {
                // User is not authenticated or user cookie is not found, return -1
                return -1;
            }
        }


        /// <summary>
        /// Filter action when search button is pressed
        /// </summary>
        /// <param name="searchString">User search words</param>
        /// <returns>View</returns>
        [Route("product/filter")]
        public IActionResult Filter(string searchString)
        {
            int userId = GetCurrentUserId();

            if (string.IsNullOrEmpty(searchString))
            {
                return View("SearchResults", new List<Product>());
            }
            // Convert the search string to lowercase for case-insensitive comparison
            var searchLower = searchString.ToLower();

            // Get all products from the database
            var allProducts = _dataContext.Products.ToList();

            // Filter products in memory
            var filteredProducts = allProducts
                .Where(p =>
                    p.Name.ToLower().Contains(searchLower) ||
                    p.Description.ToLower().Contains(searchLower) ||
                    Enum.GetName(typeof(Category), p.Category)?.ToLower() == searchLower)
                .ToList();

            // Check if user is logged in
            if (userId >= 0)
            {
                // Save search history
                SaveSearchHistory(searchString, userId);

                return View("SearchResults", filteredProducts);
            }
            else // Redirect to login page
            {
                return View("SearchResults", filteredProducts);               
            }
        }

        /// <summary>
        /// Saves user search to cookie and database
        /// </summary>
        /// <param name="searchString">User search words</param>
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
            List<Product> products = _dataContext.Products.Where(p => p.Category == selectedCategory).ToList();

            return View("SearchByCategory", products);
        }

        /// <summary>
        /// Saves user search phrase into database
        /// </summary>
        /// <param name="searchString">User search words</param>
        /// <param name="userId">User id</param>
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
