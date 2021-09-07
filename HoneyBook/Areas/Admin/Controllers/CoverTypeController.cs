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
    public class CoverTypeController : Controller
    {
        private IUnitOfWork _unitOfWork;
        public CoverTypeController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        
        public IActionResult Upsert(int? id)
        {
            CoverType coverType = new CoverType();
            if (id == null)
            {
                //create
                return View(coverType);
            }
            coverType = _unitOfWork.CoverType.Get(id.GetValueOrDefault());
            return View(coverType);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Upsert(CoverType coverType)
        {
            if (ModelState.IsValid) 
            {
                var objInDb = _unitOfWork.CoverType.Get(coverType.Id);
                if (coverType.Id == 0)
                {
                    _unitOfWork.CoverType.Add(coverType);
                }
                else
                {
                    _unitOfWork.CoverType.update(coverType);
                }
                _unitOfWork.save();
                return RedirectToAction(nameof(Index));
            }
            return View();
        }


        public IActionResult Index()
        {
            return View();
        }
        #region API CALLS
        [HttpGet]
        public IActionResult GetAll()
        {
           var allObj = _unitOfWork.CoverType.GetAll();
            return Json(new { data = allObj });
        }
        [HttpDelete]
        public IActionResult Delete(int id)
        {
            var objInDb = _unitOfWork.CoverType.Get(id);
            if (objInDb == null)
            {
                return Json(new { success = false, message = "Không thành công." });
            }
            _unitOfWork.CoverType.Remove(id);
            _unitOfWork.save();
            return Json(new { success = true, message = "Xóa thành công." });
        }
        #endregion
    }
}
