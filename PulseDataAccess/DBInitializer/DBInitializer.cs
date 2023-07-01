
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using PulseDataAccess.Data;
using PulseModels;
using PulseUtility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;

namespace PulseDataAccess.DBInitializer
{
    public class DBInitializer : IDBInitializer
    {
        private readonly ApplicationDBContext db;
        private readonly RoleManager<IdentityRole> roleManager;
        private readonly UserManager<IdentityUser> userManager;
        public DBInitializer(ApplicationDBContext _db,
               RoleManager<IdentityRole> _roleManager,
               UserManager<IdentityUser> _userManager)
        {
            db = _db;
            roleManager = _roleManager;
            userManager = _userManager;
        }
        
        public void Initialize()
        {
        
            //migrations if they arent applied
            try
            {
                if (db.Database.GetPendingMigrations().Count() > 0)
                {
                    db.Database.Migrate();
                }
            }
            catch (Exception ex) { }

            //create roles if they are not created
            if (!roleManager.RoleExistsAsync(SD.Role_Customer).GetAwaiter().GetResult())
            {
                roleManager.CreateAsync(new IdentityRole(SD.Role_Customer)).GetAwaiter().GetResult();
                roleManager.CreateAsync(new IdentityRole(SD.Role_Employee)).GetAwaiter().GetResult();
                roleManager.CreateAsync(new IdentityRole(SD.Role_Admin)).GetAwaiter().GetResult();
                roleManager.CreateAsync(new IdentityRole(SD.Role_Company)).GetAwaiter().GetResult();

                //if roles not created, we will create admin user as well
                userManager.CreateAsync(new ApplicationUser
                {
                    UserName = "chaospulse77@gmail.com",
                    Email = "chaospulse77@gmail.com",
                    Name = "Admin",
                    PhoneNumber = "+380502156437",
                    StreetAddress = "York Street",
                    City = "New York",
                    PostalCode = "11111",
                    State = "NY",
                }, "Admin123*").GetAwaiter().GetResult();
                ApplicationUser user = db.ApplicationUsers.FirstOrDefault(u => u.Email == "admin@dotnetmastery.com");
                userManager.AddToRoleAsync(user, SD.Role_Admin).GetAwaiter().GetResult();
            }
            return;
        }
    }
}
