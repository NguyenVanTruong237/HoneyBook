using HoneyBook.DataAccess.Repository.IRepository;
using HoneyBook.Models;
using HoneyBook.Models.ViewModels;
using HoneyBook.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace HoneyBook.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = SD.Role_Admin)]
    public class ProductController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IWebHostEnvironment _hostEnviroment;

        public ProductController(IUnitOfWork unitOfWork, IWebHostEnvironment hostEnvironment)
        {
            _unitOfWork = unitOfWork;
            _hostEnviroment = hostEnvironment;
        }

        public async Task<IActionResult> Upsert(int? id)
        {
            IEnumerable<Category> cateList = await _unitOfWork.Category.GetAllAsync();
            ProductVM productVM = new ProductVM()
            {
                Product = new Product(),
                CategoryList = cateList.Select(i => new SelectListItem
                {
                    Text = i.Name,
                    Value = i.Id.ToString()
                }),
                CoverTypeList = _unitOfWork.CoverType.GetAll().Select(i => new SelectListItem
                {
                    Text = i.Name,
                    Value = i.Id.ToString()
                })
            };
            if (id == null)
            {
                return View(productVM);
            }
            productVM.Product = _unitOfWork.Product.Get(id.GetValueOrDefault());
            if (productVM == null)
            {
                return NotFound();
            }
            return View(productVM);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Upsert(ProductVM productVM)
        {
            if (ModelState.IsValid)
            {
                string webRootPath = _hostEnviroment.WebRootPath;
                var files = HttpContext.Request.Form.Files;
                if (files.Count > 0)
                {
                    string filename = Guid.NewGuid().ToString(); //tạo ra số guid vd số guid : 0f8fad5b-d9cb-469f-a165-70867728950e
                    var uploads = Path.Combine(webRootPath, @"images\products"); //tới đường dẫn nơi lưu file
                    var extenstion = Path.GetExtension(files[0].FileName); //get ra đuôi file vd: png, exe
                    if (productVM.Product.ImageUrl != null)
                    {
                        var imagePath = Path.Combine(webRootPath, productVM.Product.ImageUrl.TrimStart('\\'));
                        if (System.IO.File.Exists(imagePath))
                        {
                            System.IO.File.Delete(imagePath);
                        }
                    }
                    using (var filestreams = new FileStream(Path.Combine(uploads, filename + extenstion), FileMode.Create))
                    {
                        files[0].CopyTo(filestreams);
                    }
                    productVM.Product.ImageUrl = @"\images\products\" + filename + extenstion;
                }
                else
                {
                    if (productVM.Product.Id != 0)
                    {
                        Product objInDb = _unitOfWork.Product.Get(productVM.Product.Id);
                        productVM.Product.ImageUrl = objInDb.ImageUrl;
                    }
                }

                if (productVM.Product.Id == 0)
                {
                    _unitOfWork.Product.Add(productVM.Product);
                }
                else
                {
                    _unitOfWork.Product.update(productVM.Product);
                }
                _unitOfWork.save();
                return RedirectToAction(nameof(Index));
            }
            return View(productVM);
        }
        public IActionResult Index()
        {
            return View();
        }

        #region API CALLs

        public IActionResult GetAll()
        {
            var ObjInDb = _unitOfWork.Product.GetAll(includeProPerties: "Category,CoverType");
            return Json(new { data = ObjInDb });
        }
        public IActionResult Delete(int id)
        {
            var objInDb = _unitOfWork.Product.Get(id);
            if (objInDb == null)
            {
                return Json(new { success = false, message = "Thất bại." });
            }
            string webRootPath = _hostEnviroment.WebRootPath;
            var imagePath = Path.Combine(webRootPath, objInDb.ImageUrl.TrimStart('\\'));
            if (System.IO.File.Exists(imagePath))
            {
                System.IO.File.Delete(imagePath);
            }
            _unitOfWork.Product.Remove(objInDb);
            _unitOfWork.save();
            return Json(new { success = true, message= "Xóa thành công."});
        }

        #endregion
    }
}