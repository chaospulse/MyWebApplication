using PulseModels;
using PulseModels.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace PulseDataAccess.Repository.IRepositry
{
	public interface IOrderHeaderRepositry : IRepositry<OrderHeader>
	{
        void Update(OrderHeader orderHeader);
        void UpdateStatus(int id, string orderStatus, string? paymentStatus = null);
        void UpdateStripePaymentID(int id, string sessionID, string paymentIntentId);
        void Save();
    }
}
 