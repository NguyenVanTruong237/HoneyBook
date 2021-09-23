using HoneyBook.DataAccess.Repository.IRepository;
using HoneyBook.Models.ViewModels;
using HoneyBook.Utility;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;

namespace HoneyBook.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class CartController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IEmailSender _emailSender;
        private UserManager<IdentityUser> _userManager;

        public CartController(IUnitOfWork unitOfWork, IEmailSender emailSender, UserManager<IdentityUser> userManager)
        {
            _unitOfWork = unitOfWork;
            _emailSender = emailSender;
            _userManager = userManager;
        }

        public IActionResult Index()
        {
            var claimIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimIdentity.FindFirst(ClaimTypes.NameIdentifier);

            var shoppingCartVM = new ShoppingCartVM()
            {
                OrderHeader = new Models.OrderHeader(),
                ShoppingCarts = _unitOfWork.ShoppingCart.GetAll(u => u.ApplicationUserId == claim.Value, includeProPerties:"Product")
            };

            shoppingCartVM.OrderHeader.OrderTotal = 0;
            shoppingCartVM.OrderHeader.ApplicationUser = _unitOfWork.ApplicationUser.GetFirstOrDefault(u => u.Id == claim.Value, includeProPerties: "Company");

            foreach (var list in shoppingCartVM.ShoppingCarts)
            {
                list.Price = SD.GetPriceBaseOnQuantity(list.Count, list.Product.Price, list.Product.Price50, list.Product.Price100);
                shoppingCartVM.OrderHeader.OrderTotal += (list.Price * list.Count);
                list.Product.Description = SD.ConvertToRawHtml(list.Product.Description);

                if (list.Product.Description.Length > 100)
                {
                    list.Product.Description = list.Product.Description.Substring(0, 99) + " ...";
                }
            }
            return View(shoppingCartVM);
        }
        [HttpPost]
        [ActionName("Index")]
        public async Task<IActionResult> IndexPOST()
        {
            var claimIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimIdentity.FindFirst(ClaimTypes.NameIdentifier);
            var user = _unitOfWork.ApplicationUser.GetFirstOrDefault(c => c.Id == claim.Value);
            if (user == null)
            {
                ModelState.AddModelError(string.Empty, "Verification email is empty!");
            }

            //Confirm email
            var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
            var callbackUrl = Url.Page(
                "/Account/ConfirmEmail",
                pageHandler: null,
                values: new { area = "Identity", userId = user.Id, code = code },
                protocol: Request.Scheme);

            await _emailSender.SendEmailAsync(user.Email, "Confirm your email",
                $"Please confirm your account by <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicking here</a>.");

            ModelState.AddModelError(string.Empty, "Xác thực mail của bạn đã được gửi, vui lòng check mail!");
            return RedirectToAction(nameof(Index));
        }

        public IActionResult Plus(int cartid)
        {
            var cart = _unitOfWork.ShoppingCart.GetFirstOrDefault(c => c.Id == cartid,includeProPerties: "Product");

            cart.Count += 1;
            cart.Price = SD.GetPriceBaseOnQuantity(cart.Count, cart.Product.Price
                , cart.Product.Price50, cart.Product.Price100);

            _unitOfWork.save();
            return RedirectToAction(nameof(Index));
        }
        public IActionResult Minus (int cartId)
        {
            var cart = _unitOfWork.ShoppingCart.GetFirstOrDefault(c => c.Id == cartId, includeProPerties: "Product");

            if (cart.Count == 1)
            {
                var cnt = _unitOfWork.ShoppingCart.GetAll(c => c.ApplicationUserId == cart.ApplicationUserId).Count();
                _unitOfWork.ShoppingCart.Remove(cart);
                _unitOfWork.save();
                HttpContext.Session.SetInt32(SD.ssShoppingCart, cnt - 1);
            }
            else
            {
                cart.Count += -1;
                cart.Price= SD.GetPriceBaseOnQuantity(cart.Count, cart.Product.Price
                , cart.Product.Price50, cart.Product.Price100);
                _unitOfWork.save();
            }
            return RedirectToAction(nameof(Index));
        }
        public IActionResult Remove (int cartId)
        {
            var cart = _unitOfWork.ShoppingCart.GetFirstOrDefault(c => c.Id == cartId, includeProPerties: "Product");
            var cnt = _unitOfWork.ShoppingCart.GetAll(c => c.ApplicationUserId == cart.ApplicationUserId).Count();

            _unitOfWork.ShoppingCart.Remove(cart);
            _unitOfWork.save();
            HttpContext.Session.SetInt32(SD.ssShoppingCart, cnt - 1);

            return RedirectToAction(nameof(Index));
        }
        public IActionResult Summary()
        {
            var claimIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimIdentity.FindFirst(ClaimTypes.NameIdentifier);

            var shoppingCartVM = new ShoppingCartVM()
            {
                OrderHeader = new Models.OrderHeader(),
                ShoppingCarts = _unitOfWork.ShoppingCart.GetAll(c => c.ApplicationUserId == claim.Value, includeProPerties: "Product")
            };
            shoppingCartVM.OrderHeader.ApplicationUser = _unitOfWork.ApplicationUser.GetFirstOrDefault(c => c.Id == claim.Value, includeProPerties: "Company");

            foreach (var list in shoppingCartVM.ShoppingCarts)
            {
                list.Price = SD.GetPriceBaseOnQuantity(list.Count, list.Product.Price,list.Product.Price50,list.Product.Price100);
                shoppingCartVM.OrderHeader.OrderTotal += list.Price * list.Count;
            }
            shoppingCartVM.OrderHeader.Name = shoppingCartVM.OrderHeader.ApplicationUser.Name;
            shoppingCartVM.OrderHeader.PhoneNumber = shoppingCartVM.OrderHeader.ApplicationUser.PhoneNumber;
            shoppingCartVM.OrderHeader.StreetAddress = shoppingCartVM.OrderHeader.ApplicationUser.StreetAddress;
            shoppingCartVM.OrderHeader.City = shoppingCartVM.OrderHeader.ApplicationUser.City;
            shoppingCartVM.OrderHeader.State = shoppingCartVM.OrderHeader.ApplicationUser.State;
            shoppingCartVM.OrderHeader.PostalCode = shoppingCartVM.OrderHeader.ApplicationUser.PostalCode;

            return View(shoppingCartVM);
        }
    }
}
