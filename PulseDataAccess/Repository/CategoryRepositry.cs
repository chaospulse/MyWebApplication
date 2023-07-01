using PulseModels.Models;
using PulseDataAccess.Data;
using PulseDataAccess.Repository.IRepositry;

namespace PulseDataAccess.Repository
{
	public class CategoryRepositry : Repositry<Category>, ICategoryRepositry
	{
		private ApplicationDBContext db;
		public CategoryRepositry(ApplicationDBContext _db) : base(_db) => db = _db; 
		public void Save() => db.SaveChanges();
		public void Update(Category category) => db.Category.Update(category);
	}
}
