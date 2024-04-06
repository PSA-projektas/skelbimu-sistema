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

namespace Skelbimu_sistema.Controllers
{
    [Route("suggestion")]
    public class WishController : Controller
    {
        private readonly DataContext _dataContext;

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

    }
}
