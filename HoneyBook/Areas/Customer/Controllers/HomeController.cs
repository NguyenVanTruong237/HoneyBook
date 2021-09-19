using HoneyBook.DataAccess.Repository.IRepository;
using HoneyBook.Models;
using HoneyBook.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace HoneyBook.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IUnitOfWork _unitOfWork;

        public HomeController(ILogger<HomeController> logger, IUnitOfWork unitOfWork)
        { 
            _logger = logger;
            _unitOfWork = unitOfWork;
        }

        public IActionResult Index()
        {
            IEnumerable<Product> products = _unitOfWork.Product.GetAll(includeProPerties: "Category,CoverType");
            return View(products);
        }

        public IActionResult Details(int id)
        {
            var objInDb = _unitOfWork.Product.GetFirstOrDefault(c => c.Id == id, includeProPerties: "Category,CoverType");
            ShoppingCart shoppingCart = new ShoppingCart()
            {
                ProductId = objInDb.Id,
                Product = objInDb
            };
            return View(shoppingCart);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public IActionResult Details(ShoppingCart cartObject)
        {
            cartObject.Id = 0;
            if (ModelState.IsValid)
            {
                //Get User Id
                var claimIdentity = (ClaimsIdentity)User.Identity;
                var claim = claimIdentity.FindFirst(ClaimTypes.NameIdentifier);
                cartObject.ApplicationUserId = claim.Value;

                ShoppingCart cartInDb = _unitOfWork.ShoppingCart.GetFirstOrDefault(
                    u => u.ApplicationUserId == cartObject.ApplicationUserId && u.ProductId == cartObject.ProductId
                    , includeProPerties: "Product");

                if (cartInDb == null)
                {
                    _unitOfWork.ShoppingCart.Add(cartObject);
                }
                else
                {
                    cartInDb.Count += cartObject.Count;
                   // _unitOfWork.ShoppingCart.Update(cartObject);
                }
                _unitOfWork.save();
                return RedirectToAction(nameof(Index));
            }
            else
            {
                var productInDb = _unitOfWork.Product.GetFirstOrDefault(c => c.Id == cartObject.ProductId, includeProPerties: "Category,CoverType");
                ShoppingCart cartObj = new ShoppingCart()
                {
                    Product = productInDb,
                    ProductId = productInDb.Id
                };
                return View(cartObj);
            }
        }
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
