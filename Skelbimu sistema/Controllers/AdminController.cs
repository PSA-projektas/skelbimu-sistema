using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using Skelbimu_sistema.Models;

namespace Skelbimu_sistema.Controllers
{
    [Authorize]
    [Route("administracija")]
    public class AdminController : Controller
    {
        private readonly DataContext _dataContext;
        private readonly int _maxReportCount;

        public AdminController(DataContext dataContext)
        {
            _dataContext = dataContext;
            _maxReportCount = 5;
        }

        [Authorize(Policy = "IsAdmin")]
        [HttpGet("administracija")]
        public IActionResult Index()
        {
            var users = _dataContext.Users;

            // Count the total number of users
            ViewData["UsersCount"] = users.Count();

			// Count the number of admins
			ViewData["AdminsCount"] = users.Count(u => u.Role == UserRole.Admin);

			// Count the number of blocked users
			ViewData["BlockedCount"] = users.Count(u => u.Blocked);

            var products = _dataContext.Products;

			// Count the total number of users
			ViewData["SuspendedCount"] = products.Where(product => product.State == ProductState.Suspended).Count();

			// Count the number of admins
			ViewData["CorrectedCount"] = products.Include(product => product.Suspension).Where(product => product.State == ProductState.Suspended && product.Suspension!.Corrected).Count();

			// Count the number of blocked users
			ViewData["ReportedCount"] = products.Include(product => product.Reports).Where(product => product.Reports.Count > 0).Count();

			return View();
        }

        [Authorize(Policy = "IsAdmin")]
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

        [Authorize(Policy = "IsAdmin")]
        [HttpGet("administracija/skelbimai")]
        public async Task<IActionResult> Products(string filter, int? sellerId)
        {
            var productsWithAdminInfo = await _dataContext.Products
                .Include(product => product.User)
                .Include(product => product.Reports)
                .Include(product => product.Suspension)
                .ToListAsync();

            if (filter == "reported")
            {
                productsWithAdminInfo = productsWithAdminInfo.Where(product => product.Reports.Count > 0).ToList();
            }

            if (filter == "suspended")
            {
                productsWithAdminInfo = productsWithAdminInfo.Where(product => product.State == ProductState.Suspended).ToList();
            }

            if (filter == "corrected")
            {
				productsWithAdminInfo = productsWithAdminInfo.Where(product => product.State == ProductState.Suspended).ToList();
			}

            if(sellerId != null)
            {
                productsWithAdminInfo = productsWithAdminInfo.Where(product => product.UserId == sellerId).ToList();
            }

            return View(productsWithAdminInfo);
        }

        [Authorize(Policy = "IsAdmin")]
        [HttpGet("skelbimai/{productId}")]
        public async Task<IActionResult> ProductDetails(int productId)
        {
            var productWithAdminInfo = await _dataContext.Products
                .Include(product => product.User)
                .Include(product => product.Reports)
                .Include(product => product.Suspension)
                .FirstOrDefaultAsync(product => product.Id == productId);

            return View(productWithAdminInfo);
        }

        [HttpGet("perspeti/{productId}")]
        public async Task<IActionResult> Report(int productId)
		{
			var product = await _dataContext.Products
		        .Include(product => product.User)
		        .FirstOrDefaultAsync(product => product.Id == productId);

            if (product == null)
            {
                return NotFound(); // TODO: better error handling
            }

            var report = new Report()
            {
                ProductId = productId,
                Product = product
            };

			return View(report);
		}

		[Authorize]
		[HttpPost("perspeti")]
		public async Task<IActionResult> SubmitReport(Report report)
		{
            // TODO: check if current user hasn't already reported this product
            int userId = int.Parse(User.Claims.FirstOrDefault(c => c.Type == "Id")!.Value);
            var user = await _dataContext.Users.FindAsync(userId);            

            report.UserId = userId;
            report.User = user!;

            int productId = report.ProductId;
            var product = await _dataContext.Products.FindAsync(productId); // the view doesn't return the product

            report.Product = product!;

            _dataContext.Reports.Add(report);
            await _dataContext.SaveChangesAsync();

            
            int reportCount = await _dataContext.Reports.Where(report => report.ProductId == productId).CountAsync();
			if(reportCount >= _maxReportCount && product!.State != ProductState.Suspended)
            {
                await AutomaticallySuspend(product);
            }

			return RedirectToAction("Index", "Home"); // TODO: pass a success message
		}

        [Authorize(Policy = "IsAdmin")]
        [HttpPost("perspėjimai/pašalinti")]
        public IActionResult DeleteReport(int reportId, int productId)
        {
            var report = _dataContext.Reports.Find(reportId);
            if (report != null)
            {
                _dataContext.Reports.Remove(report);
                _dataContext.SaveChanges();
            }
            return RedirectToAction(nameof(ProductDetails), new { productId });
        }

        [Authorize(Policy = "IsAdmin")]
        [HttpGet("skelbimai/{productId}/suspenduoti")]
        public IActionResult Suspend(int productId)
        {
            var suspension = new Suspension()
            {
                ProductId = productId
            };

            return View(suspension);
        }

        [Authorize(Policy = "IsAdmin")]
        [HttpPost("skelbimai/suspenduoti")]
        public async Task<IActionResult> SubmitSuspension(Suspension suspension)
        {
            var productId = suspension.ProductId;
            var product = await _dataContext.Products.Include(product => product.User).FirstOrDefaultAsync(product => product.Id == productId);

            suspension.Product = product!;
            suspension.Reviewed = true;
            _dataContext.Suspensions.Add(suspension);
            product!.State = ProductState.Suspended;

            var user = product!.User;
            if (!user.Blocked) 
            {
                user.Blocked = true;
            }

            await _dataContext.SaveChangesAsync();

            return RedirectToAction("ProductDetails", new { productId });
        }

        [Authorize(Policy = "IsAdmin")]
        [HttpPost("suspendavimai/atnaujinti")]
        public IActionResult UpdateSuspension(int suspensionId, string reason, int productId)
        {
            var suspension = _dataContext.Suspensions.Find(suspensionId);
            if (suspension != null)
            {
                suspension.Reason = reason;
                suspension.Reviewed = true;
                _dataContext.SaveChanges();
            }
            return RedirectToAction(nameof(ProductDetails), new { productId });
        }

        [Authorize(Policy = "IsAdmin")]
        [HttpPost("suspendavimai/pašalinti")]
        public async Task<IActionResult> DeleteSuspension(int suspensionId)
        {
            var suspension = await _dataContext.Suspensions
                .Include(suspension => suspension.Product)
                    .ThenInclude(product => product.User)
                        .ThenInclude(user => user.Products)
                    .ThenInclude(product => product.Reports)
                .FirstOrDefaultAsync(suspension => suspension.Id == suspensionId);

            if (suspension != null)
            {
                _dataContext.Suspensions.Remove(suspension);
                var product = suspension.Product;
                if (product != null)
                {
                    product.State = ProductState.Closed;
                    foreach(var report in product.Reports)
                    {
                        _dataContext.Reports.Remove(report);
                    }

                    var user = product.User;
                    if(user != null)
                    {
                        var suspendedCount = user.Products.Where(product => product.State == ProductState.Suspended).Count();
                        if (suspendedCount == 0)
                        {
                            user.Blocked = false;
                        }
                    }
                }

                await _dataContext.SaveChangesAsync();
            }

            return RedirectToAction("ProductDetails", new { suspension!.ProductId });
        }

        private async Task AutomaticallySuspend(Product product)
        {
            Suspension suspension = new Suspension()
            {
                Reason = "Automatiškai suspenduotas dėl per didelio perspėjimų skaičiaus.",
                ProductId = product.Id,
                Product = product
            };
            _dataContext.Suspensions.Add(suspension);
            product.State = ProductState.Suspended;
            var user = product.User;
            if (!user.Blocked)
            {
                user.Blocked = true;
            }
            await _dataContext.SaveChangesAsync();
        }
	}
}
