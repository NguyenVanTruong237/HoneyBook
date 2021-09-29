using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using HoneyBook.DataAccess.Repository.IRepository;
using Microsoft.AspNetCore.Mvc;
namespace HoneyBook.ViewComponents
{
    public class UserNameViewComponent : ViewComponent
    {
        private readonly IUnitOfWork _unitOfWork;
        public UserNameViewComponent(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public async Task<IViewComponentResult> InvokeAsync()
        {
            var claimIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimIdentity.FindFirst(ClaimTypes.NameIdentifier);
            var userInDb = _unitOfWork.ApplicationUser.GetFirstOrDefault(c => c.Id == claim.Value);

            return View(userInDb);
        }
    }
}
