using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using PulseDataAccess.Repository;
using PulseDataAccess.Repository.IRepositry;
using PulseModels;
using PulseUtility;

namespace MyWebApplication.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = SD.Role_Admin)]
    public class CompanyController : Controller
    {
        private readonly ICompanyRepositry companyRepositry;
        //
        public CompanyController(ICompanyRepositry _companyRepositry)
        {
            companyRepositry = _companyRepositry;
        }

        public IActionResult Index()
        {
            List<Company> objCategoryList = companyRepositry.GetAll().ToList();
            return View(objCategoryList);
        }
        public IActionResult Create(int? id)
        {
            if (id == 0 || id == null) //create 
            {
                return View(new Company());
            } 
            else //update
            {
                Company companyOBJ = companyRepositry.Get(U => U.Id == id);
                return View(companyOBJ);
            }
        }
        
        public IActionResult Upsert(int? id)
        {
            if (id ==0 || id == null) //create 
            {
                return View(new Company());
            }
            else //update
            {
                Company companyOBJ = companyRepositry.Get(U => U.Id == id);
                return View(companyOBJ);
            }
        }
        [HttpPost]
        public IActionResult Create(Company obj)
        {
            if (ModelState.IsValid)
            {
                companyRepositry.Add(obj);
                companyRepositry.Save();
                TempData["Create"] = "Category Created Successfully";
                return RedirectToAction("Index");
            }
            return View();
        }
        [HttpPost]
        public IActionResult Upsert(Company companyOBJ)
        {
            if (ModelState.IsValid)
            { 
                if (companyOBJ.Id == 0)
                {
                    companyRepositry.Add(companyOBJ);
                }
                else
                {
                    companyRepositry.Update(companyOBJ);
                }
                companyRepositry.Save();
                TempData["Company"] = "Company Created Successfully";
                return RedirectToAction("Index");
            }
            else
            {
                return View(companyOBJ);
            }
        }

        public IActionResult Delete(int? id)
        {
            if (id == null || id == 0)
            {
                return NotFound();
            }
            Company companyOBJ = companyRepositry.Get(U => U.Id == id);
            if (companyOBJ == null)
            {
                return NotFound();
            }
            return View(companyOBJ);
        }

        #region API CALLS
        [HttpGet]
        public IActionResult GetAll()
        {
            var objCompanyList = companyRepositry.GetAll();
            return Json(new { data = objCompanyList });
        }

        [HttpPost, ActionName("Delete")]
        public IActionResult DeletePost(int id)
        {
            Company obj = companyRepositry.Get(u => u.Id == id);
            if (obj == null)
            {
                return NotFound();
            }
            companyRepositry.Remove(obj);
            companyRepositry.Save();
            TempData["Delete"] = "Delete Successfully";
            return RedirectToAction("Index");
        }
        #endregion
    }
}
