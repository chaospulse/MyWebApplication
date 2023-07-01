using PulseModels.Models;
using PulseDataAccess.Data;
using PulseDataAccess.Repository.IRepositry;
using PulseModels;
using System.Linq.Expressions;

namespace PulseDataAccess.Repository
{
    public class ApplicationUserRepositry : Repositry<ApplicationUser>, IApplicationUserRepositry
    {
        private ApplicationDBContext db;
        public ApplicationUserRepositry(ApplicationDBContext _db) : base(_db) => db = _db;

        public void Update(ApplicationUser obj) => db.ApplicationUsers.Update(obj);
       
        public void Save() => db.SaveChanges();
    }
}
