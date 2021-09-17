using HoneyBook.DataAccess.Data;
using HoneyBook.Models;
using HoneyBook.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HoneyBook.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = SD.Role_Admin + "," + SD.Role_Employee)]
    public class UserController : Controller
    {
        private readonly ApplicationDbContext _db;
        public UserController(ApplicationDbContext db)
        {
            _db = db;
        }

        public IActionResult Index()
        {
            return View();
        }
        #region API CALLS 
        [HttpGet]
        public IActionResult GetAll()
        {
            var userList = _db.ApplicationUsers.Include(c => c.Company).ToList(); 
            var userRole = _db.UserRoles.ToList();
            var roleList = _db.Roles.ToList();
            foreach (var user in userList)
            {
                var roleId = userRole.FirstOrDefault(c => c.UserId == user.Id).RoleId;
                user.Role = roleList.FirstOrDefault(c => c.Id == roleId).Name;
                if (user.Company == null)
                {
                    user.Company = new Company()
                    {
                        Name = "",
                    };
                }
            }
            return Json(new { data = userList });
        }
        [HttpPost]
        public IActionResult LockUnlock([FromBody] string id)
        {
            var objInDb = _db.ApplicationUsers.FirstOrDefault(c => c.Id == id);
            if (objInDb == null)
            {
                return Json(new { success = false, message = "Oop lỗi rồi fen!" });
            }
            if (objInDb.LockoutEnd!=null&&objInDb.LockoutEnd>DateTime.Now)
            {
                objInDb.LockoutEnd = DateTime.Now;
            }
            else
            {
                objInDb.LockoutEnd = DateTime.Now.AddYears(1000);
            }
            _db.SaveChanges();
            return Json(new { success = true, message = "Thành công!" });
        }
        #endregion
    }
}
