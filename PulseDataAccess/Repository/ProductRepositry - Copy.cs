using PulseModels.Models;
using PulseDataAccess.Data;
using PulseDataAccess.Repository.IRepositry;

namespace PulseDataAccess.Repository
{
	public class ProductRepositry : Repositry<Product>, IProductRepositry
	{
		private ApplicationDBContext db;
		public ProductRepositry(ApplicationDBContext _db) : base(_db) => db = _db;
		public void Save() => db.SaveChanges();
		public void Update(Product product)
		{
			var objFromDB = db.Products.FirstOrDefault(u => u.Id == product.Id);
			if (objFromDB != null)
			{
				objFromDB.Title = product.Title;
				objFromDB.ISBN = product.ISBN;
				objFromDB.Price =  product.Price;
				objFromDB.Price50 = product.Price50;
				objFromDB.Price100 = product.Price100;
				objFromDB.ListPrice = product.ListPrice;
				objFromDB.Description = product.Description;
				objFromDB.CategoryId = product.CategoryId;
				objFromDB.Author = product.Author;
				
				if (product != null)
					objFromDB.ImageUrl = product.ImageUrl;
			}


		}
	}
}
