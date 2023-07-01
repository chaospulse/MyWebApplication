using Microsoft.AspNetCore.Mvc;
using PulseDataAccess.Repository.IRepositry;
using PulseDataAccess.Data;
using PulseModels.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using PulseModels.ViewModels;
using Microsoft.Build.Tasks.Deployment.Bootstrapper;
using PulseDataAccess.Repository;
using Microsoft.AspNetCore.Authorization;
using PulseUtility;

namespace MyWebApplication.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = SD.Role_Admin)]
    public class ProductController : Controller
    {
        private readonly IProductRepositry productRepositry;
        private readonly ICategoryRepositry categoryRepositry;
        private readonly IWebHostEnvironment webHostEnvironment;
        public ProductController(IProductRepositry DBcontext, ICategoryRepositry DBcontext2, IWebHostEnvironment _webHostEnvironment)
        {
            productRepositry = DBcontext;
            categoryRepositry = DBcontext2;
            webHostEnvironment = _webHostEnvironment;
        }

        public IActionResult Index()
        {
            List<PulseModels.Models.Product> objCategoryList = productRepositry.GetAll(IncludeProperties: "Category").ToList();
            return View(objCategoryList);
        }
        public IActionResult Upsert(int? id)
        {
            ProductViewModel productVM = new()
            {
                CategoryList = categoryRepositry.GetAll().Select
                         (u => new SelectListItem
                         {
                             Text = u.Name,
                             Value = u.Id.ToString()
                         }),
                Product = new PulseModels.Models.Product()
            };
            if (id ==0 || id == null) //create 
            {
                return View(productVM);
            }
            else //update
            {
                productVM.Product = productRepositry.Get(U => U.Id == id);
                return View(productVM);
            }
        }

        [HttpPost]
        public IActionResult Upsert(ProductViewModel? obj, IFormFile? file)
        {
            if (ModelState.IsValid)
            {
                string wwwRootPath = webHostEnvironment.WebRootPath;
                if (file != null)
                {
                    string FileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
                    string ProductPath = Path.Combine(wwwRootPath + @"\images\product");
                    
                    if (!string.IsNullOrEmpty(obj.Product.ImageUrl))
                    {
                        var OldImagePath = Path.Combine(wwwRootPath, obj.Product.ImageUrl.TrimStart('\\'));
                        if (System.IO.File.Exists(OldImagePath))
                            System.IO.File.Delete(OldImagePath);
                    }
                    using (var FileStream = new FileStream(Path.Combine(ProductPath, FileName), FileMode.Create))
                    {
                        file.CopyTo(FileStream);
                    }
                    obj.Product.ImageUrl = @"\images\product\" + FileName;
                }

                if (obj.Product.Id == 0)
                    productRepositry.Add(obj.Product);
                else
                    productRepositry.Update(obj.Product);
                
                productRepositry.Save();
                TempData["Product"] = "Product Created Successfully";
                return RedirectToAction("Index");
            }
            else
            {
                obj.CategoryList = productRepositry.GetAll().Select(u => new SelectListItem
                {
                    Text = u.Title,
                    Value = u.Id.ToString()
                });
                return View(obj);
            }
        }

		//delete
		public IActionResult Delete(int? id)
		{
			if (id == null || id == 0)
				return NotFound();
			var obj = productRepositry.Get(U => U.Id == id);
			if (obj == null)
				return NotFound();
			return View(obj);
		}

		#region API CALLS
		[HttpGet]
        public IActionResult GetAll()
        {
            var objProductList = productRepositry.GetAll(IncludeProperties: "Category");
            return Json(new { data = objProductList });
        }

		[HttpPost, ActionName("Delete")]
		public IActionResult DeletePost(int? id)
        {
            var objFromDb = productRepositry.Get(u => u.Id == id);
            if (objFromDb == null)
            {
				//return Json(new { success = false, message = "Error while Deleting" });
				return NotFound();
			}
            var OldImagePath =  Path.Combine(webHostEnvironment.WebRootPath, objFromDb.ImageUrl.TrimStart('\\'));

            if (System.IO.File.Exists(OldImagePath))
                 System.IO.File.Delete(OldImagePath);
                
            productRepositry.Remove(objFromDb);
            productRepositry.Save();
			//return Json(new { success = true, message = "Delete Successful" }); 
			return RedirectToAction("Index");
		}
		#endregion
	}
}
