using PulseModels.Models;
using PulseDataAccess.Data;
using PulseDataAccess.Repository.IRepositry;
using PulseModels;

namespace PulseDataAccess.Repository
{
	public class CompanyRepositry : Repositry<Company>, ICompanyRepositry
	{
		private ApplicationDBContext db;
		public CompanyRepositry(ApplicationDBContext _db) : base(_db) => db = _db; 
		public void Save() => db.SaveChanges();
		public void Update(Company company) => db.Company.Update(company);
	}
}
