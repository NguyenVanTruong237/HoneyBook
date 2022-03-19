using Dapper;
using HoneyBook.DataAccess.Repository.IRepository;
using HoneyBook.Models;
using HoneyBook.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HoneyBook.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = SD.Role_Admin)]
    public class CoverTypeController : Controller
    {
        private IUnitOfWork _unitOfWork;
        public CoverTypeController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        
        public IActionResult Upsert (int? id)
        {
            CoverType coverType = new CoverType();
            if (id == null)
            {
                return View(coverType);
            }

            coverType = _unitOfWork.CoverType.GetFirstOrDefault(c => c.Id == id);
            if (coverType != null)
            {
                return View(coverType);
            }

            return PartialView("~/Views/Shared/Error.cshtml");
        }



        //public IActionResult Upsert(int? id)
        //{
        //    CoverType coverType = new CoverType();
        //    if (id == null)
        //    {
        //        //create
        //        return View(coverType);
        //    }
        //    var parameter = new DynamicParameters();
        //    parameter.Add("@Id", id);
        //    coverType = _unitOfWork.SP_Call.OneRecord<CoverType>(SD.Proc_CoverType_Get, parameter);
        //    return View(coverType);
        //}

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Upsert(CoverType coverType)
        {
            if (ModelState.IsValid)
            {
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
            return View(coverType);
        }

        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public IActionResult Upsert(CoverType coverType)
        //{
        //    if (ModelState.IsValid) 
        //    {
        //        var parameter = new DynamicParameters();
        //        parameter.Add("@Name", coverType.Name);
        //        if (coverType.Id == 0)
        //        {
        //            _unitOfWork.SP_Call.Execute(SD.Proc_CoverType_Create, parameter);
        //        }
        //        else
        //        {
        //            parameter.Add("@Id", coverType.Id);
        //            _unitOfWork.SP_Call.Execute(SD.Proc_CoverType_Update, parameter);
        //        }
        //        _unitOfWork.save();
        //        return RedirectToAction(nameof(Index));
        //    }
        //    return View(coverType);
        //}

        public IActionResult Index()
        {
            return View();
        }


        #region API CALLS
        [HttpGet]
        //public IActionResult GetAll()
        //{
        //    var allObj = _unitOfWork.SP_Call.List<CoverType>(SD.Proc_CoverType_GetAll,null);
        //    return Json(new { data = allObj });
        //}

        [HttpGet]
        public IActionResult GetAll()
        {
            var allObj = _unitOfWork.CoverType.GetAll();

            return Json(new { data = allObj });
        }

        //[HttpDelete]
        //public IActionResult Delete(int id)
        //{
        //    var parameter = new DynamicParameters();
        //    parameter.Add("@Id", id);
        //    var objInDb = _unitOfWork.SP_Call.OneRecord<CoverType>(SD.Proc_CoverType_Get, parameter);
        //    if (objInDb == null)
        //    {
        //        return Json(new { success = false, message = "Không thành công." });
        //    }
        //    _unitOfWork.SP_Call.Execute(SD.Proc_CoverType_Delete, parameter);
        //    _unitOfWork.save();
        //    return Json(new { success = true, message = "Xóa thành công." });
        //}

        [HttpDelete]
        public IActionResult Delete(int id)
        {
            var coverTypeInDb = _unitOfWork.CoverType.GetFirstOrDefault(c => c.Id == id);

            if (coverTypeInDb != null)
            {
                _unitOfWork.CoverType.Remove(id);
                _unitOfWork.save();
                return Json(new { success = true, message = "Xóa thành công." });
            }

            return Json(new { success = false, message = "Không thành công." });
        }
        #endregion
    }
}
