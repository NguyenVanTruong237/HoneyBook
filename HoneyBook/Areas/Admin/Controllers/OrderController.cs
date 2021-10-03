using HoneyBook.DataAccess.Repository.IRepository;
using HoneyBook.Models;
using HoneyBook.Models.ViewModels;
using HoneyBook.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Stripe;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace HoneyBook.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize]
    public class OrderController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        public OrderController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        [BindProperty]
        public OrderDetailsVM OrderDetails { get; set; }
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult Details(int id)
        {
            OrderDetails = new OrderDetailsVM()
            {
                OrderHeader = _unitOfWork.OrderHeader.GetFirstOrDefault(c => c.Id == id, includeProPerties:"ApplicationUser"),
                OrderDetails = _unitOfWork.OrderDetails.GetAll(c => c.OrderId == id, includeProPerties: "Product")
            };
            return View(OrderDetails);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult UpdateOrder (OrderDetailsVM detailsVM)
        {
            var objInDb = _unitOfWork.OrderHeader.GetFirstOrDefault(c => c.Id == detailsVM.OrderHeader.Id);
            if (ModelState.IsValid || objInDb.Carrier ==null || objInDb.TrackingNumber==null)
            {
                objInDb.StreetAddress = detailsVM.OrderHeader.StreetAddress;
                objInDb. City = detailsVM.OrderHeader.City;
                objInDb.State = detailsVM.OrderHeader.State;
                objInDb.PostalCode = detailsVM.OrderHeader.PostalCode;
                _unitOfWork.save();
            }
            return RedirectToAction("Details", "Order", new { id = objInDb.Id });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName("Details")]
        public IActionResult Details(string stripeToken)
        {
            OrderHeader orderHeader = _unitOfWork.OrderHeader.GetFirstOrDefault(c => c.Id == OrderDetails.OrderHeader.Id, includeProPerties:"ApplicationUser");

            if (stripeToken != null)
            {
                var options = new ChargeCreateOptions
                {
                    Amount = Convert.ToInt32(orderHeader.OrderTotal * 100),
                    Currency = "usd",
                    Description = "Order ID : " + orderHeader.Id,
                    Source = stripeToken
                };
                var service = new ChargeService();
                Charge charge = service.Create(options);
                if (charge.BalanceTransactionId == null)
                {
                    orderHeader.PaymentStatus = SD.PaymentStatusRejected;
                }
                else
                {
                    orderHeader.TransactionId = charge.Id;
                }
                if (charge.Status.ToLower() == "succeeded")
                {
                    orderHeader.PaymentStatus = SD.PaymentStatusApproved;
                   orderHeader.PaymentDate = DateTime.Now;
                }
                _unitOfWork.save();
            }
            return RedirectToAction("Details", "Order", new { id = orderHeader.Id });
        }

        [Authorize(Roles =SD.Role_Admin+","+SD.Role_Employee)]
        public IActionResult StartProcessing(int id)
        {
            OrderHeader orderHeader = _unitOfWork.OrderHeader.GetFirstOrDefault(u => u.Id == id);
            orderHeader.OrderStatus = SD.StatusInProcess;
            _unitOfWork.save();
            return RedirectToAction(nameof(Index));
        }
        [HttpPost]
        [Authorize(Roles = SD.Role_Admin + "," + SD.Role_Employee)]
        public IActionResult ShipOrder()
        {
            OrderHeader orderHeader = _unitOfWork.OrderHeader.GetFirstOrDefault(u => u.Id == OrderDetails.OrderHeader.Id);
            orderHeader.TrackingNumber = OrderDetails.OrderHeader.TrackingNumber;
            orderHeader.Carrier = OrderDetails.OrderHeader.Carrier;
            orderHeader.OrderStatus = SD.StatusShipped;
            orderHeader.ShippingDate = DateTime.Now;
            _unitOfWork.save();
            return RedirectToAction(nameof(Index));
        }
        [Authorize(Roles = SD.Role_Admin + "," + SD.Role_Employee)]
        public IActionResult CancelOrder(int id)
        {
            OrderHeader orderHeader = _unitOfWork.OrderHeader.GetFirstOrDefault(u => u.Id == id);
            if (orderHeader.PaymentStatus == SD.StatusApproved)
            {
                var options = new RefundCreateOptions()
                {
                    Amount = Convert.ToInt32(orderHeader.OrderTotal * 100),
                    Reason = RefundReasons.RequestedByCustomer,
                    Charge = orderHeader.TransactionId
                };
                var service = new RefundService();
                Refund refund = service.Create(options);

                orderHeader.OrderStatus = SD.StatusRefunded;
                orderHeader.PaymentStatus = SD.StatusRefunded;
            }
            else
            {
                orderHeader.OrderStatus = SD.StatusCancelled;
                orderHeader.PaymentStatus = SD.StatusCancelled;
            }
            _unitOfWork.save();
            return RedirectToAction(nameof(Index));
        }

        #region API CALLS
        [HttpGet]
        public IActionResult GetOrderList(string status)
        {
            var claimIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimIdentity.FindFirst(ClaimTypes.NameIdentifier);

            IEnumerable<OrderHeader> OrderList;

            if (User.IsInRole(SD.Role_Admin) || User.IsInRole(SD.Role_Employee))
            {
                 OrderList = _unitOfWork.OrderHeader.GetAll(includeProPerties: "ApplicationUser");
            }
            else
            {
                OrderList = _unitOfWork.OrderHeader.GetAll(c => c.ApplicationUserId == claim.Value, includeProPerties: "ApplicationUser");
            }

            switch (status)
            {
                case "pending":
                    OrderList = OrderList.Where(o => o.PaymentStatus == SD.PaymentStatusDelayedPayment);
                    break;
                case "inprocess":
                    OrderList = OrderList.Where(o => o.OrderStatus == SD.StatusApproved ||
                                                o.OrderStatus == SD.StatusInProcess ||
                                                o.OrderStatus == SD.StatusPending);
                    break;
                case "completed":
                    OrderList = OrderList.Where(o => o.OrderStatus == SD.StatusShipped);
                    break;
                case "rejected":
                    OrderList = OrderList.Where(o => o.OrderStatus == SD.StatusCancelled ||
                                                o.OrderStatus == SD.StatusRefunded ||
                                                o.OrderStatus == SD.PaymentStatusRejected);
                    break;
                default:
                    break;
            }

            return Json(new { data = OrderList });
        }
        #endregion
    }
}
