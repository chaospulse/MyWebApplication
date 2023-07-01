using Microsoft.EntityFrameworkCore.Query;
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
	public interface IApplicationUserRepositry : IRepositry<ApplicationUser>
	{
        void Update(ApplicationUser obj);
        void Save();
    }
}
 