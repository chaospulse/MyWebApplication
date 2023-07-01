using PulseModels.Models;
using PulseDataAccess.Data;
using PulseDataAccess.Repository.IRepositry;
using PulseModels;

namespace PulseDataAccess.Repository
{
	public class OrderDetailsRepositry : Repositry<OrderDetails>, IOrderDetailsRepositry
    {
		private ApplicationDBContext db;
		public OrderDetailsRepositry(ApplicationDBContext _db) : base(_db) => db = _db; 
		public void Update(OrderDetails orderDetails) => db.OrderDetails.Update(orderDetails);
        public void Save() => db.SaveChanges();
		
    }
}
