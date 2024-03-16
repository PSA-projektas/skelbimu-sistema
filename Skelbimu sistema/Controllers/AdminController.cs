using Microsoft.AspNetCore.Mvc;
using Skelbimu_sistema.Models;

namespace Skelbimu_sistema.Controllers
{
    public class AdminController : Controller
    {
        private readonly DataContext _dataContext;

        public AdminController(DataContext dataContext)
        {
            _dataContext = dataContext;
        }

        [HttpGet("administracija")]
        public IActionResult Index()
        {
            var users = _dataContext.Users;

            // Count the total number of users
            ViewData["UsersCount"] = users.Count();

			// Count the number of admins
			ViewData["AdminsCount"] = users.Count(u => u.Role == 2);

			// Count the number of blocked users
			ViewData["BlockedCount"] = users.Count(u => u.Blocked);

            return View();
        }

        [HttpGet("administracija/naudotojai")]
        public IActionResult Users(string filter)
        {
            var users = _dataContext.Users.ToList();

            // Apply filtering based on the 'filter' query parameter
            if (filter == "blocked")
            {
                users = users.Where(u => u.Blocked).ToList();
            }

            return View(users);
        }

        [HttpGet("administracija/skelbimai")]
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
