using Azure.Core;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Skelbimu_sistema.Data;
using Skelbimu_sistema.Models;
using Skelbimu_sistema.ViewModels;
using System.Security.Claims;
using System.Security.Cryptography;
using static Microsoft.Extensions.Logging.EventSource.LoggingEventSource;
using Microsoft.AspNetCore.Http;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Text;
using System.Net.Http;
using Microsoft.AspNetCore.Authorization;

namespace Skelbimu_sistema.Controllers
{
    [Route("suggestion")]
    public class WishController : Controller
    {
        private readonly DataContext _dataContext;
        private List<Product> userSuggestionsByWish = new List<Product>();
        private DateTime wishSuggestionsUpdated = new DateTime(1, 1, 1);

        public WishController(DataContext dataContext)
        {
            _dataContext = dataContext;
        }

        /// <summary>
        /// Shows user's products suggestions by search
        /// </summary>
        /// <returns>Page with suggestions</returns>
        [HttpGet("suggestionPage")]
        public IActionResult OpenSuggestedProductsPage()
        {
            List<Product> suggestions = CreateSuggestionsBySearch();
            return View("SuggestionList", suggestions); 
        }

        /// <summary>
        /// Create new wish
        /// </summary>
        /// <returns>View</returns>
        [Authorize]
        [HttpGet("wishCreation")]
        public async Task<IActionResult> Create()
        {
            WishCreationRequest request = new WishCreationRequest();

            int userId = int.Parse(User.Claims.FirstOrDefault(c => c.Type == "Id")!.Value);
            var user = await _dataContext.Users.FindAsync(userId);

            ViewData["UserRole"] = user.Role;

            return View(request);
        }

        /// <summary>
        /// Submit wish creation
        /// </summary>
        /// <param name="request">Request of creation</param>
        /// <returns>Success message</returns>
        [Authorize]
        [HttpPost("wishCreateSubmit")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SubmitCreate(WishCreationRequest request)
        {
            //check if editedwish name is not empty
            if (string.IsNullOrEmpty(request.Name))
            {
                ModelState.AddModelError("Name", "Pavadinimas negali būti tuščias");
            }
            if (string.IsNullOrEmpty(request.SearchKeyWords))
            {
                ModelState.AddModelError("SearchKeyWords", "Reikalingas bent vienas raktažodis");
            }
            //check if price is not negative
            if (request.PriceLow < 0)
            {
                ModelState.AddModelError("PriceLow", "Kaina negali būti neigiamas skaičius");
            }
            if (request.PriceHigh < 0)
            {
                ModelState.AddModelError("PriceHigh", "Kaina negali būti neigiamas skaičius");
            }
            if (request.PriceHigh < request.PriceLow)
            {
                ModelState.AddModelError("PriceHigh", "Maksimali kaina negali būti žemesnė už minimalią");
            }
            if (request.PriceLow > request.PriceHigh)
            {
                ModelState.AddModelError("PriceLow", "Minimali kaina negali būti aukštensė už maksimalią");
            }
            // Get user and check if not blocked
            int userId = int.Parse(User.Claims.FirstOrDefault(c => c.Type == "Id")!.Value);
            User user = _dataContext.Users.Find(userId)!;

            if (!ModelState.IsValid)
            {
                return View("Create", request); // Return to the registration view with errors
            }

            Wish wish = new Wish();
            wish.Name = request.Name;
            wish.SearchKeyWords = request.SearchKeyWords;
            wish.PriceLow = request.PriceLow;
            wish.PriceHigh = request.PriceHigh;
            wish.PaymentMethod = request.PaymentType;
            wish.Category = request.Category;
            wish.User = user;

            _dataContext.UserWishes.Add(wish);  
            await _dataContext.SaveChangesAsync();

            // Set success message
            //TempData["SuccessMessage"] = "Noras sukurtas sėkmingai!";

            return RedirectToAction("WishListPage");
        }

        /// <summary>
        /// Returns logged in user's id
        /// </summary>
        /// <returns>Id</returns>
        public int GetCurrentUserId()
        {
            var user = User;

            if (user.Identity != null && user.Identity.IsAuthenticated)
            {
                var userIdClaim = user.Claims.FirstOrDefault(c => c.Type == "Id");

                if (userIdClaim != null && int.TryParse(userIdClaim.Value, out int userId))
                {
                    return userId;
                }

                // Couldn't get user id from User
                return -1;
            }

            // User not logged in
            return -1;
        }

        /// <summary>
        /// Make a list of user's search key words
        /// </summary>
        /// <returns>list of words</returns>
        public List<string> SelectKeyWords()
        {                    
            // Get the current user ID
            int userId = GetCurrentUserId();

            // If user is not logged in, return an empty list
            if (userId == -1)
            {
                return new List<string>();
            }

            // Retrieve the search history cookie for the current user
            /*
            var searchHistoryCookie = Request.Cookies["SearchHistory_" + userId];

            List<string> searchHistory = new List<string>();

            if (searchHistoryCookie != null && !string.IsNullOrEmpty(searchHistoryCookie))
            {
                // Deserialize search history from cookie
                searchHistory = searchHistoryCookie.Split(',').ToList();
            }
            */

            // Retrieve data from user's search history attribute
            var currentUser = _dataContext.Users.FirstOrDefault(p => p.Id == userId);
            List<string> searchWordsList = new List<string>();
            if (currentUser != null)
            {
                string searchWords = currentUser.SearchKeyWords;
                string[] searchWordsArray = searchWords.Split(new char[] { ',', ' ' }, 
                                            StringSplitOptions.RemoveEmptyEntries);
                searchWordsList = new List<string>(searchWordsArray);
            }
            return searchWordsList;
        }


        /// <summary>
        /// Creates product list based on user's search
        /// </summary>
        /// <returns>List of products</returns>
        public List<Product> CreateSuggestionsBySearch()
        {
            List<Product> products = _dataContext.Products.ToList();
            List<string> keywords = SelectKeyWords();

            var suggestions = new List<Product>();

            foreach (var product in products)
            {          
                if (ValidateProductForSuggestion(product, keywords))
                {
                    suggestions.Add(product);
                }
            }

            return suggestions;
        }

        /// <summary>
        /// Decides whether the product is suggestion
        /// </summary>
        /// <param name="product">Product</param>
        /// <param name="keywords">Keywords list</param>
        /// <returns>Decision</returns>
        private static bool ValidateProductForSuggestion(Product product, List<string> keywords)
        {
            // Normalize keywords to handle variations like 'e' and 'ė' and convert to lowercase
            List<string> normalizedKeywords = keywords.Select(keyword => keyword.Normalize(NormalizationForm.FormKD).ToLower()).ToList();

            // Combine product name and description and normalize to lowercase
            string combinedProductInfo = (product.Name + " " + product.Description).Normalize(NormalizationForm.FormKD).ToLower();

            // Check if any normalized keyword is contained in the combined product info
            foreach (var keyword in normalizedKeywords)
            {
                if (combinedProductInfo.Contains(keyword))
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Open wish list page
        /// </summary>
        /// <returns>Page of user wishes</returns>
        [Authorize]
        [HttpGet("WishListPage")]
        [Route("/Wish/WishListPage")]
        public async Task<IActionResult> WishListPage()
        {
            int userId = int.Parse(User.Claims.FirstOrDefault(c => c.Type == "Id")!.Value);
            var user = await _dataContext.Users.FindAsync(userId);

            var userWishes = await _dataContext.UserWishes
                                .Where(wish => wish.UserId == userId)
                                .ToListAsync();
            ViewData["UserId"] = userId;
            ViewData["UserRole"] = user.Role;

            return View(userWishes);
        }

        /// <summary>
        /// Delete user wish
        /// </summary>
        /// <param name="id">Wish id</param>
        /// <returns>Success message</returns>
        [Authorize]
        [HttpPost]
        [Route("/Wish/Delete/{id}")]
        [ValidateAntiForgeryToken]
        public IActionResult Delete(int id)
        {
            var wish = _dataContext.UserWishes.FirstOrDefault(w => w.Id == id);

            if (wish == null)
            {
                return NotFound();
            }

            try
            {
                _dataContext.UserWishes.Remove(wish);
                _dataContext.SaveChanges();
                return Ok();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error deleting wish: " + ex.Message);
                return StatusCode(500);
            }
        }

        /// <summary>
        /// Edit user wish
        /// </summary>
        /// <param name="id">Wish id</param>
        /// <returns>Success message</returns>
        [Authorize]
        [HttpGet]
        [Route("/Wish/Edit")]
        public IActionResult Edit(int id)
        {
            // Retrieve the wish by id
            var wish = _dataContext.UserWishes.FirstOrDefault(w => w.Id == id);

            if (wish == null)
            {
                return NotFound();
            }

            // Pass the wish data to the edit view
            return View(wish);
        }

        /// <summary>
        /// Edit wish
        /// </summary>
        /// <param name="editedWish">Wish to edit</param>
        /// <returns>Success message</returns>
        [Authorize]
        [HttpPost]
        [Route("/Wish/Edit")]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(Wish editedWish)
        {
            var wish = _dataContext.UserWishes.FirstOrDefault(w => w.Id == editedWish.Id);

            if (wish == null)
            {
                return NotFound();
            }
            if (string.IsNullOrEmpty(editedWish.Name))
            {
                ModelState.AddModelError("Name", "Pavadinimas negali būti tuščias");
            }
            if (string.IsNullOrEmpty(editedWish.SearchKeyWords))
            {
                ModelState.AddModelError("SearchKeyWords", "Reikalingas bent vienas raktažodis");
            }
            //check if price is not negative
            if (editedWish.PriceLow < 0)
            {
                ModelState.AddModelError("PriceLow", "Kaina negali būti neigiamas skaičius");
            }
            if (editedWish.PriceHigh < 0)
            {
                ModelState.AddModelError("PriceHigh", "Kaina negali būti neigiamas skaičius");
            }
            if (editedWish.PriceHigh < editedWish.PriceLow)
            {
                ModelState.AddModelError("PriceHigh", "Maksimali kaina negali būti žemesnė už minimalią");
            }
            if (editedWish.PriceLow > editedWish.PriceHigh)
            {
                ModelState.AddModelError("PriceLow", "Minimali kaina negali būti aukštensė už maksimalią");
            }

            wish.Name = editedWish.Name;
            wish.SearchKeyWords = editedWish.SearchKeyWords;
            wish.PriceLow = editedWish.PriceLow;
            wish.PriceHigh = editedWish.PriceHigh;
            wish.PaymentMethod = editedWish.PaymentMethod;
            wish.Category = editedWish.Category;

            // Save changes to the database
            _dataContext.SaveChanges();
            //TempData["SuccessMessageWish"] = "Noras redaguotas sėkmingai!";
            return RedirectToAction("WishListPage"); // Redirect to the wishlist view after editing
        }

        /// <summary>
        /// Opens wish suggestions (products)
        /// </summary>
        /// <param name="id">Wish id</param>
        /// <returns>Page of suggestions</returns>
        [Authorize]
        [HttpGet]
        [Route("/Wish/OpenWishSuggestionsPage")]
        public IActionResult OpenWishSuggestionsPage(int id)
        {
            var wish = _dataContext.UserWishes.FirstOrDefault(w => w.Id == id);
            List<Product> suggestions = FindProductsByWish(wish);
            return View("SuggestionList", suggestions);
        }

        /// <summary>
        /// Makes a list of products based on wish keywords and informs user via email
        /// </summary>
        /// <param name="wish">User wish</param>
        /// <returns>List of products</returns>
        public List<Product> FindProductsByWish(Wish wish)
        {
            if (wishSuggestionsUpdated == new DateTime(1, 1, 1) || 
                wishSuggestionsUpdated.AddHours(1) <= DateTime.Now)
            {
                wishSuggestionsUpdated = DateTime.Now;
                userSuggestionsByWish = UpdateWishList(wish);
                InformUserWishUpdate(wish); // send email
            }     
            return userSuggestionsByWish;
        }

        /// <summary>
        /// Get keywords of wish
        /// </summary>
        /// <param name="wish">User wish</param>
        /// <returns>List of keywords</returns>
        private List<string> GetWishKeyWords(Wish wish)
        {
            List<string> keywords = new List<string>();
            char[] delimiters = { ' ', ',', ';' };
            string[] words = wish.SearchKeyWords.Split(delimiters, StringSplitOptions.RemoveEmptyEntries);

            foreach (string word in words)
            {
                keywords.Add(word);
            }
            return keywords;
        }

        /// <summary>
        /// Updates user's suggestion list of products based on wish keywords every hour
        /// </summary>
        /// <param name="wish">User wish</param>
        /// <returns>List of products</returns>
        private List<Product> UpdateWishList(Wish wish)
        {
            List<Product> products = _dataContext.Products.ToList();
            List<string> keywords = GetWishKeyWords(wish);
            List<Product> suggestions = new List<Product>();

            foreach (var product in products)
            {
                if (ValidateProductForSuggestion(product, keywords) &&
                    ValidateProductDetails(product, wish) &&
                    ValidateProductPaymentType(product, wish))
                {
                    suggestions.Add(product);
                }
            }
            return suggestions;
        }

        /// <summary>
        /// Send email to user to inform about wish suggestions update
        /// </summary>
        /// <param name="wish">Wish</param>
        private void InformUserWishUpdate(Wish wish)
        {
            int userId = GetCurrentUserId();          
            var user = _dataContext.Users.FirstOrDefault(p => p.Id == userId);
            string email = user.Email;
            string emailText = "Jūsų noro, pavadinimu " + wish.Name + ", pasiūlymų sąrašas atnaujintas.";
            if (email != null) 
            { 
                // send email
            }
        }

        /// <summary>
        /// Validate product by wish price and category
        /// </summary>
        /// <param name="product">Product to validate</param>
        /// <param name="wish">Wish to check</param>
        /// <returns>Condition</returns>
        private bool ValidateProductDetails(Product product, Wish wish)
        {
            if (product.Price >= wish.PriceLow && product.Price <= wish.PriceHigh &&
                product.Category == wish.Category || wish.Category.ToString().Equals("All"))
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// Validate product by wish payment type
        /// </summary>
        /// <param name="product">Product to validate</param>
        /// <param name="wish">Wish to check</param>
        /// <returns>Condition</returns>
        private bool ValidateProductPaymentType(Product product, Wish wish)
        {
            if ((int)product.PaymentType == (int)wish.PaymentMethod ||
                wish.PaymentMethod.Equals("All"))
            {
                return true;
            }
            return false;
        }

    }
}
