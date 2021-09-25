using HoneyBook.DataAccess.Repository.IRepository;
using HoneyBook.Models;
using HoneyBook.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
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
        public IActionResult Index()
        {
            return View();
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
