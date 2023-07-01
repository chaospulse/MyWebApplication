using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.EntityFrameworkCore.Query.SqlExpressions;
using PulseDataAccess.Data;
using PulseDataAccess.Repository.IRepositry;

namespace PulseDataAccess.Repository
{
	public class Repositry<T> : IRepositry<T> where T : class
	{
		private readonly ApplicationDBContext db;
		internal DbSet<T> dbSet;
		public Repositry(ApplicationDBContext _db)
		{
			db = _db;
			dbSet = db.Set<T>();
			db.Products.Include(c => c.Category).Include(c => c.CategoryId);
		}
		public void Add(T entity) => dbSet.Add(entity);

		public T Get(Expression<Func<T, bool>> ?expression, string? IncludeProperties = null, bool tracked = false)
		{
			IQueryable<T> querry;
            if (tracked)
            {
                querry = dbSet.AsQueryable();
              
            }
			else 
			{
                querry = dbSet.AsNoTracking();
            }
            querry = querry.Where(expression);
                if (!string.IsNullOrEmpty(IncludeProperties))
                {
                    foreach (var IncludeProp in IncludeProperties.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                    {
                        querry = querry.Include(IncludeProp);
                    }
                }
            return querry.FirstOrDefault();
        }
           
		public IEnumerable<T> GetAll(Expression<Func<T, bool>> ?expression, string? IncludeProperties = null)
        {
			IQueryable<T> querry = dbSet.AsQueryable();
            if (expression != null)
				querry = querry.Where(expression);

            if (!string.IsNullOrEmpty(IncludeProperties))
			{
				foreach (var IncludeProp in IncludeProperties.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
				{
					querry = querry.Include(IncludeProp);
				}
			}
			return querry.ToList(); //its error
		}
		public void Remove(T entity) => dbSet.Remove(entity);

		public void RemoveRange(IEnumerable<T> entity) => dbSet.RemoveRange(entity);

		
	}
}
