using PulseModels.Models;
using PulseDataAccess.Data;
using PulseDataAccess.Repository.IRepositry;
using PulseModels;

namespace PulseDataAccess.Repository
{
	public class ShoppingCartRepositry : Repositry<ShoppingCart>, IShoppingCartRepositry
    {
		private ApplicationDBContext db;
		public ShoppingCartRepositry(ApplicationDBContext _db) : base(_db) => db = _db; 
		public void Save() => db.SaveChanges();
		public void Update(ShoppingCart shoppingCart) => db.ShoppingCart.Update(shoppingCart);
	}
}
