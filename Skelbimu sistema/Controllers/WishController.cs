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
using System.Globalization;
using System.Text.RegularExpressions;

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
        /// Make a list of user's search key words
        /// </summary>
        /// <returns>list of words</returns>
        public List<string> SelectKeyWords()
        {
            /*
            // Get the current user's ID from HttpContext
            string userId = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            // Assuming you have a way to retrieve user data based on their ID
            User user = null;
            if (int.TryParse(userId, out int userIdInt))
            {
                user = _dataContext.Users.FirstOrDefault(u => u.Id == userIdInt);
            }

            if (user != null)
            {
                // Assuming SearchKeyWords is a property of the User model
                List<string> keyWords = user.SearchKeyWords.Split(' ').ToList();               
                return keyWords;
            }

            // If user is not found or doesn't have search keywords, return an empty list
            return new List<string>();
            */

            return new List<string> { "Pele"};
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
        /// Adds search input string to key words list
        /// </summary>
        /// <param name="searchString">Input string</param>
        /// <returns>Success message</returns>
        [HttpPost]
        public IActionResult SaveSearchToKeyWordsList(string searchString)
        {
            string userId = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            User user = null;
            if (int.TryParse(userId, out int userIdInt))
            {
                user = _dataContext.Users.FirstOrDefault(u => u.Id == userIdInt);
            }

            if (user != null)
            {
                user.SearchKeyWords = user.SearchKeyWords + " " + searchString;
                _dataContext.SaveChanges();
            }

            return RedirectToAction("OpenSuggestedProductsPage"); 
        }


    }
}
