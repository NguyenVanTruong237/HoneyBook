using HoneyBook.DataAccess.Repository.IRepository;
using HoneyBook.Models;
using HoneyBook.Models.ViewModels;
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
    [Authorize (Roles =SD.Role_Admin)]
    public class CategoryController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        public CategoryController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IActionResult> Index(int productPage = 1)
        {
            CategoryVM categoryVM = new CategoryVM()
            {
                Categories = await _unitOfWork.Category.GetAllAsync()
            };

            var count = categoryVM.Categories.Count();
            categoryVM.Categories = categoryVM.Categories.OrderBy(p => p.Name)
                .Skip((productPage - 1) * 2).Take(2).ToList();

            categoryVM.PagingInfo = new PagingInfo
            {
                CurrentPage = productPage,
                ItemPerPage = 2,
                TotalItem = count,
                urlParam = "/Admin/Category/Index?productPage=:"
            };

            return View();
        }
        public async Task<IActionResult> Upsert(int? id)
        {
            Category category = new Category();
            if (id == null)
            {
                return View(category);
            }
            category = await _unitOfWork.Category.GetAsync(id.GetValueOrDefault()); //GetValueOrDefault: nếu giá trị id == null sẽ trả về mặc định là null
            if (category == null)
            {
                return NotFound();
            }
            return View(category);
        }
        [HttpPost] // định tuyến: nhận data từ view(upsert)
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Upsert(Category category)
        {
            if (ModelState.IsValid)
            {
                if (category.Id == 0)
                {
                    await _unitOfWork.Category.AddAsync(category);

                }
                else
                {
                    _unitOfWork.Category.Update(category);
                }
                _unitOfWork.save();
                return RedirectToAction(nameof(Index));
            }
            return View(category);
        }
        #region API CALLS
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var allObj = await _unitOfWork.Category.GetAllAsync();
            return Json(new { data = allObj });
        }
        [HttpDelete]
        public async Task<IActionResult> Delete(int id)
        {
            var objInDb = await _unitOfWork.Category.GetAsync(id);
            if (objInDb == null)
            {
                return Json(new { success = false, message = "Xóa thất bại." });
            }
            await _unitOfWork.Category.RemoveAsync(id);
            _unitOfWork.save();
            return Json(new { success = true, message = "Xóa thành công."});
        }
        #endregion
    }
}
