using Microsoft.AspNetCore.Mvc;
using Skelbimu_sistema.Models;

namespace Skelbimu_sistema.Controllers
{
    public class AdminController : Controller
    {
        public IActionResult Index()
        {
            //var users = MockUserRepo.GetUsers();

            //// Count the total number of users
            //ViewData["TotalUsers"] = users.Count;

            //// Count the number of blocked users
            //ViewData["BlockedUsers"] = users.Count(u => u.Blocked);

            return View();
        }

        public IActionResult Users(string filter)
        {
            //var users = MockUserRepo.GetUsers();

            //// Apply filtering based on the 'filter' query parameter
            //if (filter == "blocked")
            //{
            //    users = users.Where(u => u.Blocked).ToList();
            //}

            //return View(users); // Pass users to the view
            return View();
        }

        public IActionResult Adverts(string filter, int? sellerId)
        {
            //var adverts = MockAdvertRepo.GetAdverts();
            //var users = MockUserRepo.GetUsers(); // Assuming you have a method to get users

            //// Join adverts with users to get user names
            //var advertsWithSellers = from advert in adverts
            //                         join user in users on advert.User equals user.Id
            //                         select new
            //                         {
            //                             Id = advert.Id,
            //                             Title = advert.Title,
            //                             Suspended = advert.Suspended,
            //                             NumberOfReports = advert.NumberOfReports,
            //                             SellerId = user.Id,
            //                             SellerName = user.FirstName + user.LastName
            //                         };

            //if (filter == "suspended")
            //{
            //    advertsWithSellers = advertsWithSellers.Where(a => a.Suspended).ToList();
            //}
            //else if (filter == "reported")
            //{
            //    advertsWithSellers = advertsWithSellers.Where(a => a.NumberOfReports > 0).ToList();
            //}
            //if (sellerId != null)
            //{
            //    advertsWithSellers = advertsWithSellers.Where(a => a.SellerId == sellerId).ToList();
            //}

            //return View(advertsWithSellers);
            return View();
        }
    }
}
