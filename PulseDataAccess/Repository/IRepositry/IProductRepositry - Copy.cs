using PulseModels.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace PulseDataAccess.Repository.IRepositry
{
	public interface IProductRepositry : IRepositry<Product>
	{
		void Update(Product category);
		void Save();
	}
}
 