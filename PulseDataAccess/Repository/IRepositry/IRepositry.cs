using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Linq.Expressions;

namespace PulseDataAccess.Repository.IRepositry
{
	public interface IRepositry<T> where T : class
	{
		IEnumerable<T> GetAll(Expression<Func<T, bool>> ?expression = null, string? IncludeProperties = null);
        T Get(Expression<Func<T, bool>> ?expression, string? IncludeProperties = null, bool tracked = false);
        void Add(T entity);
		void Remove(T entity);
		void RemoveRange(IEnumerable<T> entity);
	}
}
