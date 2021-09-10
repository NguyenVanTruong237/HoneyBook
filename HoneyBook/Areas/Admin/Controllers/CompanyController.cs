using HoneyBook.DataAccess.Repository.IRepository;
using HoneyBook.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HoneyBook.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class CompanyController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        public CompanyController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public IActionResult Index()
        {
            return View();

        }
        public IActionResult Upsert(int? id)
        {
            Company company = new Company();
            if (id == null)
            {
                return View(company);
            }
            var objInDb = _unitOfWork.Company.Get(id.GetValueOrDefault());
            if (objInDb == null)
            {
                return NotFound();
            }
            return View(objInDb);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Upsert(Company company)
        {
            if (ModelState.IsValid)
            {
                if (company.Id == 0)
                {
                    _unitOfWork.Company.Add(company);
                }
                else
                {
                    _unitOfWork.Company.Update(company);
                }
                _unitOfWork.save();
                return RedirectToAction(nameof(Index));
            }
            return View(company);
        }
        #region API CALLS
        public IActionResult GetAll()
        {
            var companies = _unitOfWork.Company.GetAll();
            return Json(new { data = companies });
        }
        public IActionResult Delete(int id)
        {
            var ObjInDb = _unitOfWork.Company.Get(id);
            if (ObjInDb == null)
            {
                return Json(new { success = false, message = "Thất bại." });
            }
            _unitOfWork.Company.Remove(ObjInDb);
            _unitOfWork.save();
            return Json(new { success = true, message = "Xóa thành công." });
        }
        #endregion

    }
}
