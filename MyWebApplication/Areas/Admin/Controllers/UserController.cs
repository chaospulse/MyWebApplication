using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using PulseDataAccess.Repository.IRepositry;
using PulseModels;
using PulseUtility;
using System.Data;
using PulseModels.Models.ViewModels;
using System.Linq.Expressions;

namespace MyWebApplication.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = SD.Role_Admin)]
    public class UserController : Controller
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IApplicationUserRepositry applicationUserRepositry;
        private readonly ICompanyRepositry companyRepositry;

        public UserController(UserManager<IdentityUser> userManager,
               RoleManager<IdentityRole> roleManager,
               IApplicationUserRepositry _applicationUserRepositry,
               ICompanyRepositry _companyRepositry)
        {
            applicationUserRepositry = _applicationUserRepositry;
            companyRepositry = _companyRepositry;
            _roleManager = roleManager;
            _userManager = userManager;
        }
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult RoleManagment(string userId)
        {

            RoleManagmentVM RoleVM = new RoleManagmentVM()
            {
                ApplicationUser = applicationUserRepositry.Get(u => u.Id == userId, IncludeProperties: "Company"),
                RoleList = _roleManager.Roles.Select(i => new SelectListItem
                {
                    Text = i.Name,
                    Value = i.Name
                }),
                CompanyList = companyRepositry.GetAll().Select(i => new SelectListItem
                {
                    Text = i.Name,
                    Value = i.Id.ToString()
                }),
            };

            RoleVM.ApplicationUser.Role = _userManager.GetRolesAsync(applicationUserRepositry.Get(u => u.Id==userId))
                    .GetAwaiter().GetResult().FirstOrDefault();
            return View(RoleVM);
        }

        [HttpPost]
        public IActionResult RoleManagment(RoleManagmentVM roleManagmentVM)
        {
            string oldRole = _userManager.GetRolesAsync(applicationUserRepositry.Get(u => u.Id == roleManagmentVM.ApplicationUser.Id))
                    .GetAwaiter().GetResult().FirstOrDefault();

            ApplicationUser applicationUser = applicationUserRepositry.Get(u => u.Id == roleManagmentVM.ApplicationUser.Id);


            if (!(roleManagmentVM.ApplicationUser.Role == oldRole))
            {
                //a role was updated
                if (roleManagmentVM.ApplicationUser.Role == SD.Role_Company)
                    applicationUser.CompanyId = roleManagmentVM.ApplicationUser.CompanyId;
                
                if (oldRole == SD.Role_Company)
                    applicationUser.CompanyId = null;

                applicationUserRepositry.Update(applicationUser);
                applicationUserRepositry.Save();

                _userManager.RemoveFromRoleAsync(applicationUser, oldRole).GetAwaiter().GetResult();
                _userManager.AddToRoleAsync(applicationUser, roleManagmentVM.ApplicationUser.Role).GetAwaiter().GetResult();
            }
            else
            {
                if (oldRole==SD.Role_Company && applicationUser.CompanyId != roleManagmentVM.ApplicationUser.CompanyId)
                {
                    applicationUser.CompanyId = roleManagmentVM.ApplicationUser.CompanyId;
                    applicationUserRepositry.Update(applicationUser);
                    applicationUserRepositry.Save();
                }
            }
            return RedirectToAction("Index");
        }

        #region API CALLS
        [HttpGet]
        public IActionResult GetAll()
        {
            List<ApplicationUser> objUserList = applicationUserRepositry.GetAll(IncludeProperties: "Company").ToList();
            foreach (var user in objUserList)
            {
                user.Role = _userManager.GetRolesAsync(user).GetAwaiter().GetResult().FirstOrDefault();
                if (user.Company == null)
                {
                    user.Company = new Company() 
                    {
                        Name = ""
                    };
                }
            }
            return Json(new { data = objUserList });
        }

        [HttpPost]
        public IActionResult LockUnlock([FromBody] string id)
        {
            var objFromDb = applicationUserRepositry.Get(u => u.Id == id);
            if (objFromDb == null)
                return Json(new { success = false, message = "Error while Locking/Unlocking" });

            if (objFromDb.LockoutEnd!=null && objFromDb.LockoutEnd > DateTime.Now)  //user is currently locked and we need to unlock them
                objFromDb.LockoutEnd = DateTime.Now;
            else
                objFromDb.LockoutEnd = DateTime.Now.AddYears(1000);

            applicationUserRepositry.Update(objFromDb);
            applicationUserRepositry.Save();
            return Json(new { success = true, message = "Operation Successful" });
        }
        #endregion
    }
}