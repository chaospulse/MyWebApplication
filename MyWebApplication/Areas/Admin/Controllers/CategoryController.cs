using Microsoft.AspNetCore.Mvc;
using PulseDataAccess.Repository.IRepositry;
using PulseDataAccess.Data;
using PulseModels.Models;
using Microsoft.AspNetCore.Authorization;
using PulseUtility;

namespace MyWebApplication.Areas.Admin.Controllers
{
	[Area("Admin")]
    [Authorize(Roles=SD.Role_Admin)]
    public class CategoryController : Controller
    {
        private readonly ICategoryRepositry categoryRepositry;
        public CategoryController(ICategoryRepositry DBcontext) => categoryRepositry = DBcontext;
        public IActionResult Index()
        {
            List<Category> objCategoryList = categoryRepositry.GetAll().ToList();
            return View(objCategoryList);
        }
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Create(Category obj)
        {
            if (obj.Name == obj.DisplayOrder.ToString())
            {
                ModelState.AddModelError("Name", "Name and Display Order can not be same");
            }

            if (ModelState.IsValid)
            {
                categoryRepositry.Add(obj);
                categoryRepositry.Save();
                TempData["Create"] = "Category Created Successfully";
                return RedirectToAction("Index");
            }
            return View();
        }
        //
        //
        //
        public IActionResult Edit(int id)
        {
            if (id == 0)
            {
                return NotFound();
            }
            Category categoryDB = categoryRepositry.Get(u => u.Id == id);

            if (categoryDB == null)
            {
                return NotFound();
            }
            return View(categoryDB);
        }
        [HttpPost]
        public IActionResult Edit(Category category)
        {
            if (ModelState.IsValid)
            {
                categoryRepositry.Update(category);
                categoryRepositry.Save();
                return RedirectToAction("Index");
            }
            return View();
        }
        //
        //
        //
        public IActionResult Delete(int id)
        {
            if (id == 0)
            {
                return NotFound();
            }
            Category categoryDB = categoryRepositry.Get(u => u.Id == id);
            if (categoryDB == null)
            {
                return NotFound();
            }
            return View(categoryDB);
        }
        [HttpPost, ActionName("Delete")]
        public IActionResult DeletePost(int id)
        {
            Category obj = categoryRepositry.Get(u => u.Id == id);
            if (obj == null)
            {
                return NotFound();
            }
            categoryRepositry.Remove(obj);
            categoryRepositry.Save();
            TempData["Delete"] = "Delete Successfully";
            return RedirectToAction("Index");
        }
        public IActionResult GetAll()
        {
            var obj = categoryRepositry.GetAll();
            return Json(new { data = obj });
        }
    }
}
