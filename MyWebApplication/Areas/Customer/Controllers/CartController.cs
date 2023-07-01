using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PulseDataAccess.Repository;
using PulseDataAccess.Repository.IRepositry;
using PulseModels;
using PulseModels.Models;
using PulseUtility;
using Stripe;
using Stripe.Checkout;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;

namespace MyWebApplication.Areas.Customer.Controllers
{
    [Area("Customer")]
    [Authorize]
    public class CartController : Controller
    {
        private readonly IShoppingCartRepositry shoppingCartRepositry;
        private readonly IApplicationUserRepositry applicationUser;
        private readonly IOrderHeaderRepositry orderHeaderRepositry;
        private readonly IOrderDetailsRepositry orderDetailsRepositry;
        //
        [BindProperty]
        public ShoppingCartVM ShoppingCartVM { get; set; } 
        public CartController(
            IShoppingCartRepositry _shoppingCartRepositry,
            IApplicationUserRepositry _applicationUser,
            IOrderHeaderRepositry _orderHeaderRepositry,
            IOrderDetailsRepositry _orderDetailsRepositry)
        {
            applicationUser = _applicationUser;
            shoppingCartRepositry = _shoppingCartRepositry;
            orderHeaderRepositry =_orderHeaderRepositry;
            orderDetailsRepositry = _orderDetailsRepositry;
        }
        public IActionResult Index()
        {
            var _ClaimsIdentity = (ClaimsIdentity)User.Identity;
            var userId = _ClaimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;
            ShoppingCartVM = new()
            {
                ListCart = shoppingCartRepositry.GetAll(u => u.ApplicationUserId == userId, IncludeProperties: "Product"),
                OrderHeader = new()
            };

            foreach (var cart in ShoppingCartVM.ListCart)
            {
               cart.Price = GetPriceBasedOnQuantity(cart);
                ShoppingCartVM.OrderHeader.OrderTotal += (cart.Price * cart.Count);
            }
            return View(ShoppingCartVM);
        }

        public IActionResult Summary()
        {
            var _ClaimsIdentity = (ClaimsIdentity)User.Identity;
            var userId = _ClaimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;
            ShoppingCartVM = new()
            {
                ListCart = shoppingCartRepositry.GetAll(u => u.ApplicationUserId == userId, IncludeProperties: "Product"),
                OrderHeader = new()
            };
            ShoppingCartVM.OrderHeader.ApplicationUser = applicationUser.Get(u => u.Id == userId);
            
            ShoppingCartVM.OrderHeader.Name = ShoppingCartVM.OrderHeader.ApplicationUser.Name;
            ShoppingCartVM.OrderHeader.PhoneNumber = ShoppingCartVM.OrderHeader.ApplicationUser.PhoneNumber;
            ShoppingCartVM.OrderHeader.StreetAddress = ShoppingCartVM.OrderHeader.ApplicationUser.StreetAddress;
            ShoppingCartVM.OrderHeader.City = ShoppingCartVM.OrderHeader.ApplicationUser.City;
            ShoppingCartVM.OrderHeader.State = ShoppingCartVM.OrderHeader.ApplicationUser.State;
            ShoppingCartVM.OrderHeader.PostalCode = ShoppingCartVM.OrderHeader.ApplicationUser.PostalCode;

            foreach (var cart in ShoppingCartVM.ListCart)
            {
                cart.Price = GetPriceBasedOnQuantity(cart);
                ShoppingCartVM.OrderHeader.OrderTotal += (cart.Price * cart.Count);
            }
            return View(ShoppingCartVM);
        }
        
        [HttpPost]
        [ActionName("Summary")]
        public IActionResult SummaryPost()
        {
            var _ClaimsIdentity = (ClaimsIdentity)User.Identity;
            var userId = _ClaimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;
           
            ShoppingCartVM.ListCart = shoppingCartRepositry.GetAll(u => u.ApplicationUserId == userId, IncludeProperties: "Product");

            ShoppingCartVM.OrderHeader.OrderDate = DateTime.Now;
            ShoppingCartVM.OrderHeader.ApplicationUserId = userId;

            ApplicationUser appUser = applicationUser.Get(u => u.Id == userId);
            
            foreach (var cart in ShoppingCartVM.ListCart)
            {
                cart.Price = GetPriceBasedOnQuantity(cart);
                ShoppingCartVM.OrderHeader.OrderTotal += (cart.Price * cart.Count);
            }
            if (appUser.CompanyId.GetValueOrDefault()==0 )
            {
                //its a regular customer account
                ShoppingCartVM.OrderHeader.PaymentStatus = SD.PaymentStatusPending;
                ShoppingCartVM.OrderHeader.OrderStatus = SD.StatusPending;
            }
            else //its a company user
            {
                ShoppingCartVM.OrderHeader.PaymentStatus = SD.PaymentStatusApproved;
                ShoppingCartVM.OrderHeader.OrderStatus = SD.StatusApproved;
            }

            orderHeaderRepositry.Add(ShoppingCartVM.OrderHeader);
            orderHeaderRepositry.Save();

            foreach (var item in ShoppingCartVM.ListCart)
            {
                OrderDetails order = new()
                {
                    ProductId = item.ProductId,
                    OrderId = ShoppingCartVM.OrderHeader.Id,
                    Price = item.Price,
                    Count = item.Count
                };
                orderDetailsRepositry.Add(order);
                orderDetailsRepositry.Save();
            }
            
            if (appUser.CompanyId.GetValueOrDefault()==0)
            {
                //its a regular customer account
                //stripe logic
                var domain = "http://localhost:7197/";
                var option = new SessionCreateOptions
                {
                    SuccessUrl = domain + $"/customer/cart/OrderConfirmation?id={ShoppingCartVM.OrderHeader.Id}",
                    CancelUrl = domain + $"customer/cart/index",
                    LineItems = new List<SessionLineItemOptions>(),
                    Mode = "payment",
                };

                foreach (var item in ShoppingCartVM.ListCart)
                {
                    var sessionLineItem = new SessionLineItemOptions
                    {
                        PriceData = new SessionLineItemPriceDataOptions
                        {
                            UnitAmount = (long)(item.Price*100), // 20.50 --> 2050
                            Currency = "USD",
                            ProductData = new SessionLineItemPriceDataProductDataOptions
                            {
                                Name = item.Product.Title
                            }
                        }, 
                        Quantity = item.Count
                    };
                    option.LineItems.Add(sessionLineItem);
                }

                var service = new SessionService();
                Session session = service.Create(option);
                
                orderHeaderRepositry.UpdateStripePaymentID(ShoppingCartVM.OrderHeader.Id, session.Id, session.PaymentIntentId);
                orderHeaderRepositry.Save();

                Response.Headers.Add("Location", session.Url);
                return new StatusCodeResult(303);

            }
               
            return RedirectToAction(nameof(OrderConfirmation),
                new{ id = ShoppingCartVM.OrderHeader.Id });
        }
        
        public IActionResult OrderConfirmation(int id)
        {
            OrderHeader orderHeader = orderHeaderRepositry.Get(u => u.Id == id, IncludeProperties: "ApplicationUser");
            if (orderHeader.PaymentStatus != SD.PaymentStatusDelayedPayment)
            {
                //order by customer
                var service = new SessionService();
                Session session = service.Get(orderHeader.SessionId);

                if (session.PaymentStatus.ToLower() == "paid")
                {
                    orderHeaderRepositry.UpdateStripePaymentID(id, session.Id, session.PaymentIntentId);
                    orderHeaderRepositry.UpdateStatus(id, SD.StatusApproved,SD.PaymentStatusApproved);
                    orderHeaderRepositry.Save();
                }
                HttpContext.Session.Clear();
            }
            List<ShoppingCart> shoppingCarts = shoppingCartRepositry.GetAll(u => u.ApplicationUserId == orderHeader.ApplicationUserId).ToList();

            shoppingCartRepositry.RemoveRange(shoppingCarts);

            return View(id);
        }
        
        public IActionResult Plus(int cartId)
        {
            var cartFromDB= shoppingCartRepositry.Get(u => u.Id == cartId);
            cartFromDB.Count += 1;
            shoppingCartRepositry.Update(cartFromDB);
            shoppingCartRepositry.Save();
            return RedirectToAction(nameof(Index));
        }

        public IActionResult Minus(int cartId)
        {
            var cartFromDB = shoppingCartRepositry.Get(u => u.Id == cartId);
            if (cartFromDB.Count <= 1)
            {
                HttpContext.Session.SetInt32(SD.SesionCart, shoppingCartRepositry.GetAll(u => u.ApplicationUserId == cartFromDB.ApplicationUserId).Count()-1);
                shoppingCartRepositry.Remove(cartFromDB);
            }
            else
            {
                cartFromDB.Count -= 1;
                shoppingCartRepositry.Update(cartFromDB);
            }
            shoppingCartRepositry.Save();
            return RedirectToAction(nameof(Index));
        }

        public IActionResult Remove(int cartId)
        {
            var cartFromDB = shoppingCartRepositry.Get(u => u.Id == cartId, tracked: true);
            HttpContext.Session.SetInt32(SD.SesionCart, shoppingCartRepositry.GetAll(u => u.ApplicationUserId == cartFromDB.ApplicationUserId).Count()-1);
            shoppingCartRepositry.Remove(cartFromDB);
         
            shoppingCartRepositry.Save();
           
            return RedirectToAction(nameof(Index));
        }

        private double GetPriceBasedOnQuantity(ShoppingCart shoppingCart)
        {
            if (shoppingCart.Count <= 50)
                 return shoppingCart.Product.Price;
            else if (shoppingCart.Count <= 100)
                return shoppingCart.Product.Price50;
            else if (shoppingCart.Count > 100)
                return shoppingCart.Product.Price100;
            else
                return shoppingCart.Product.Price;
        }
    }
}
