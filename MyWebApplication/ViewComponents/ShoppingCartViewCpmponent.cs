using Microsoft.AspNetCore.Mvc;
using PulseDataAccess.Repository;
using PulseDataAccess.Repository.IRepositry;
using PulseUtility;
using System.Security.Claims;

namespace MyWebApplication.ViewComponents
{
    public class ShoppingCartViewComponent : ViewComponent
    {
        private readonly IShoppingCartRepositry shoppingCart;
        //
        public ShoppingCartViewComponent(IShoppingCartRepositry _shoppingCart)
        {
            shoppingCart = _shoppingCart;
        }
        //

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var _ClaimsIdentity = (ClaimsIdentity)User.Identity;
            var claim = _ClaimsIdentity.FindFirst(ClaimTypes.NameIdentifier);

            if (claim != null)
            {
                //user is logged in
                if (HttpContext.Session.GetInt32 == null)
                {
                    HttpContext.Session.SetInt32(SD.SesionCart, shoppingCart.GetAll(u => u.ApplicationUserId == claim.Value).Count());
                } 
                return View(HttpContext.Session.GetInt32(SD.SesionCart));
            }
            else
            {
                HttpContext.Session.Clear();
                return View(0);
            }
        }
    }
}
