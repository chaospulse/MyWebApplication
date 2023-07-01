using PulseModels.Models;
using PulseDataAccess.Data;
using PulseDataAccess.Repository.IRepositry;
using PulseModels;

namespace PulseDataAccess.Repository
{
    public class OrderHeaderRepositry : Repositry<OrderHeader>, IOrderHeaderRepositry
    {
        private ApplicationDBContext db;
        public OrderHeaderRepositry(ApplicationDBContext _db) : base(_db) => db = _db;
        public void Update(OrderHeader orderHeader) => db.OrderHeader.Update(orderHeader);
        public void Save() => db.SaveChanges();
        //
        //
        public void UpdateStatus(int id, string orderStatus, string? paymentStatus = null)
        {
            var orderFromDb = db.OrderHeader.FirstOrDefault(u => u.Id == id);
            if (orderFromDb != null)
            {
                orderFromDb.OrderStatus = orderStatus;
                if (!string.IsNullOrEmpty(paymentStatus))
                {
                    orderFromDb.PaymentStatus = paymentStatus;
                }
            }
        }

        public void UpdateStripePaymentID(int id,string sessionID,string paymentIntentId)
        {
            var orderFromDb = db.OrderHeader.FirstOrDefault(u => u.Id == id);
            if (!string.IsNullOrEmpty(sessionID))
            {
                orderFromDb.SessionId = sessionID;
            }
            if (!string.IsNullOrEmpty(paymentIntentId))
            {
                orderFromDb.PaymentIntentId = paymentIntentId;
                orderFromDb.PaymentDate = DateTime.Now;
            }
        }
    }
}
