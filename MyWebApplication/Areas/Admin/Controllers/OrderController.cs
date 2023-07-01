using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using PulseDataAccess.Repository;
using PulseDataAccess.Repository.IRepositry;
using PulseModels;
using PulseModels.Models;
using PulseModels.ViewModels;
using PulseUtility;
using Stripe;
using Stripe.Checkout;
using System.Diagnostics;

namespace MyWebApplication.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Microsoft.AspNetCore.Authorization.Authorize]
    public class OrderController : Controller
    {
        private readonly IProductRepositry productRepositry;
        private readonly IOrderHeaderRepositry orderHeaderRepositry;
        private readonly IOrderDetailsRepositry orderDetailsRepositry;
        [BindProperty]
        private OrderVM OrderVM { get; set; }

        public OrderController(
            IProductRepositry _productRepositry,
            IOrderHeaderRepositry _orderHeaderRepositr,
            IOrderDetailsRepositry _orderDetailsRepositry)
        {
            productRepositry = _productRepositry;
            orderHeaderRepositry = _orderHeaderRepositr;
            orderDetailsRepositry = _orderDetailsRepositry;
        }
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult Details(int orderId)
        {
            OrderVM = new()
            {
                OrderHeader = orderHeaderRepositry.Get(u => u.Id == orderId, IncludeProperties: "ApplicationUser"),
                OrderDetails = orderDetailsRepositry.GetAll(u => u.OrderId == orderId, IncludeProperties: "Product")
            };

            return View(OrderVM);
        }

        [HttpPost]
        [Microsoft.AspNetCore.Authorization.Authorize(Roles = SD.Role_Admin + "," + SD.Role_Employee)]
        public IActionResult UpdateOrderDetail(int orderId)
        {
            var orderHeaderFromdb = orderHeaderRepositry.Get(u => u.Id == OrderVM.OrderHeader.Id);

            orderHeaderFromdb.Name = OrderVM.OrderHeader.Name;
            orderHeaderFromdb.PhoneNumber = OrderVM.OrderHeader.PhoneNumber;
            orderHeaderFromdb.StreetAddress = OrderVM.OrderHeader.StreetAddress;
            orderHeaderFromdb.City = OrderVM.OrderHeader.City;
            orderHeaderFromdb.State = OrderVM.OrderHeader.State;
            orderHeaderFromdb.PostalCode = OrderVM.OrderHeader.PostalCode;

            if (!string.IsNullOrEmpty(OrderVM.OrderHeader.Carrier))
            {
                orderHeaderFromdb.Carrier = OrderVM.OrderHeader.Carrier;
            }
            if (!string.IsNullOrEmpty(OrderVM.OrderHeader.TrackingNumber))
            {
                orderHeaderFromdb.TrackingNumber = OrderVM.OrderHeader.TrackingNumber;
            }

            orderHeaderRepositry.Update(orderHeaderFromdb);
            orderHeaderRepositry.Save();
            TempData["Success"] = "Order Details Updated Successfully";

            return RedirectToAction(nameof(Details), new
            {
                orderId = orderHeaderFromdb.Id
            });
        }

        [HttpPost]
        [Microsoft.AspNetCore.Authorization.Authorize(Roles = SD.Role_Admin + "," + SD.Role_Employee)]
        public IActionResult StartProcessing()
        {
            orderHeaderRepositry.UpdateStatus(OrderVM.OrderHeader.Id, SD.StatusInProcess);
            orderHeaderRepositry.Save();
            return RedirectToAction(nameof(Details), new
            {
                orderId = OrderVM.OrderHeader.Id
            });
        }
        [HttpPost]
        [Microsoft.AspNetCore.Authorization.Authorize(Roles = SD.Role_Admin + "," + SD.Role_Employee)]
        public IActionResult ShipOrder()
        {
            var OrderHeader = orderHeaderRepositry.Get(u => u.Id == OrderVM.OrderHeader.Id);

            OrderHeader.TrackingNumber = OrderVM.OrderHeader.TrackingNumber;
            OrderHeader.Carrier = OrderVM.OrderHeader.Carrier;
            OrderHeader.OrderStatus = SD.StatusShipped;
            OrderHeader.ShippingDate = System.DateTime.Now;

            if (OrderHeader.PaymentStatus == SD.PaymentStatusDelayedPayment)
            {
                OrderHeader.PaymentDueDate = DateTime.Now.AddDays(30);
            }

            orderHeaderRepositry.Update(OrderHeader);
            orderHeaderRepositry.Save();

            TempData["Success"] = "Order Shipped Successfully";
            return RedirectToAction(nameof(Details), new
            {
                orderId = OrderVM.OrderHeader.Id
            });
        }
        [HttpPost]
        [Microsoft.AspNetCore.Authorization.Authorize(Roles = SD.Role_Admin + "," + SD.Role_Employee)]
        public IActionResult CancelOrder()
        {
            var OrderHeader = orderHeaderRepositry.Get(u => u.Id == OrderVM.OrderHeader.Id);

            if (OrderHeader.PaymentStatus == SD.PaymentStatusApproved)
            {
                var options = new RefundCreateOptions
                {
                    Reason = RefundReasons.RequestedByCustomer,
                    PaymentIntent = OrderHeader.PaymentIntentId,
                };
                var service = new RefundService();
                Refund refund = service.Create(options);

                orderHeaderRepositry.UpdateStatus(OrderVM.OrderHeader.Id, SD.StatusCancelled,SD.StatusRefunded);
            }
            else
            {
                orderHeaderRepositry.UpdateStatus(OrderVM.OrderHeader.Id, SD.StatusCancelled, SD.StatusCancelled);
            }
            
            orderHeaderRepositry.Save();
            TempData["S"] = "";

            return RedirectToAction(nameof(Details), new
            {
                orderId = OrderVM.OrderHeader.Id
            });
        }

        [ActionName("Details")]
        [HttpGet]
        public IActionResult Details_Pay_Now()
        {
            OrderVM.OrderHeader = orderHeaderRepositry.Get(u => u.Id == OrderVM.OrderHeader.Id, IncludeProperties: "ApplicationUser");
            OrderVM.OrderDetails = orderDetailsRepositry.GetAll(u => u.OrderId == OrderVM.OrderHeader.Id, IncludeProperties: "Product");
            var domain = "http://localhost:7197/";
            var option = new SessionCreateOptions
            {
                SuccessUrl = domain + $"/admin/order/PaymentConfirmation?orderId={OrderVM.OrderHeader.Id}",
                CancelUrl = domain + $"admin/order/details?orderId={OrderVM.OrderHeader.Id}",
                LineItems = new List<SessionLineItemOptions>(),
                Mode = "payment",
            };

            foreach (var item in OrderVM.OrderDetails)
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

            orderHeaderRepositry.UpdateStripePaymentID(OrderVM.OrderHeader.Id, session.Id, session.PaymentIntentId);
            orderHeaderRepositry.Save();

            Response.Headers.Add("Location", session.Url);
            return new StatusCodeResult(303);
        }

        public IActionResult PaymentConfirmation(int orderId)
        {
            OrderHeader orderHeader = orderHeaderRepositry.Get(u => u.Id == orderId);
            if (orderHeader.PaymentStatus == SD.PaymentStatusDelayedPayment)
            {
                //ordder by company
                var service = new SessionService();
                Session session = service.Get(orderHeader.SessionId);

                if (session.PaymentStatus.ToLower() == "paid")
                {
                    orderHeaderRepositry.UpdateStripePaymentID(orderId, session.Id, session.PaymentIntentId);
                    orderHeaderRepositry.UpdateStripePaymentID(orderId, session.Id, session.PaymentIntentId);
                    orderHeaderRepositry.UpdateStatus(orderId, orderHeader.OrderStatus, SD.PaymentStatusApproved);
                    orderHeaderRepositry.Save();
                }
            }
           
            return View(orderId);
        }

        #region API CALLS
        [HttpGet]
        public IActionResult GetAll(string status)
        {
            IEnumerable<OrderHeader> objHeadersList;

            if (User.IsInRole(SD.Role_Admin) || User.IsInRole(SD.Role_Employee))
            {
                objHeadersList = orderHeaderRepositry.GetAll(IncludeProperties: "ApplicationUser").ToList();
            }
            else
            {
                var claimsIdentity = (System.Security.Claims.ClaimsIdentity)User.Identity;
                var UserID = claimsIdentity.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier);
                objHeadersList = orderHeaderRepositry.GetAll(u => u.ApplicationUserId == UserID.Value, IncludeProperties: "ApplicationUser").ToList();
            }


            switch (status)
            {
                case "pending":
                    objHeadersList = objHeadersList.Where(o => o.PaymentStatus == SD.PaymentStatusDelayedPayment);
                    break;
                case "inprocess":
                    objHeadersList = objHeadersList.Where(o => o.OrderStatus == SD.StatusInProcess);
                    break;

                case "completed":
                    objHeadersList = objHeadersList.Where(o => o.OrderStatus == SD.StatusShipped);
                    break;

                case "approved":
                    objHeadersList = objHeadersList.Where(o => o.OrderStatus == SD.StatusApproved);
                    break;
                default:
                    break;
            }

            return Json(new { data = objHeadersList });
        }
        #endregion
    }
}
