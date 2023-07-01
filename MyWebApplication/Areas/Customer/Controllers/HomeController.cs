using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PulseDataAccess.Repository.IRepositry;
using PulseModels;
using PulseModels.Models;
using PulseUtility;
using System.Diagnostics;
using System.Security.Claims;

namespace MyWebApplication.Areas.Customer.Controllers
{
	[Area("Customer")]
	public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
		private readonly ICategoryRepositry categoryRepositry;
		private readonly IProductRepositry productRepositry;
        private readonly IShoppingCartRepositry shoppingCartRepositry;
        public HomeController(ILogger<HomeController> logger, ICategoryRepositry _categoryRepositry, IProductRepositry _productRepositry, IShoppingCartRepositry _shoppingCartRepositry)
		{
			_logger = logger;
			categoryRepositry = _categoryRepositry;
			productRepositry = _productRepositry;
            shoppingCartRepositry = _shoppingCartRepositry;
        }

        public IActionResult Index()
        {
            var ClaimsIdentity = (ClaimsIdentity)User.Identity;
			var claim = ClaimsIdentity?.FindFirst(ClaimTypes.NameIdentifier)?.Value; 
			if (claim != null) // user is logged in
           {  
               HttpContext.Session.SetInt32(SD.SesionCart, shoppingCartRepositry.GetAll(u => u.ApplicationUserId == claim).Count());
           }
            
            IEnumerable<Product> productList = productRepositry.GetAll(IncludeProperties:"Category");
			return View(productList);
        }
		public IActionResult Details(int ProductID)
		{
            ShoppingCart cart = new()
            {
                Product = productRepositry.Get(u => u.Id == ProductID, IncludeProperties: "Category"),
                Count = 1,
                ProductId = ProductID
            };
            return View(cart);
        }
        [HttpPost]
        [Authorize]
        public IActionResult Details(ShoppingCart cart)
        {
            var _ClaimsIdentity = (ClaimsIdentity)User.Identity;
            var userId = _ClaimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;
            cart.ApplicationUserId = userId;
            //
            ShoppingCart cartFromDb = shoppingCartRepositry.Get(u => u.ApplicationUserId == userId 
            && u.ProductId == cart.ProductId);
            if (cartFromDb != null)
            {
                cartFromDb.Count += cart.Count;
                shoppingCartRepositry.Update(cartFromDb);
                shoppingCartRepositry.Save();
            }
            else
            {
                //add cart record
                shoppingCartRepositry.Add(cart);
                shoppingCartRepositry.Save(); 

                HttpContext.Session.SetInt32(SD.SesionCart, shoppingCartRepositry.GetAll(u => u.ApplicationUserId == userId).Count());
            }
            
            TempData["Success"] = "Item added to cart successfully";
            return RedirectToAction(nameof(Index));
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}