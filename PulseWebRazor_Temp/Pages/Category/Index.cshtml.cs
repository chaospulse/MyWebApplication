using PulseWebRazor_Temp.Data;
using PulseWebRazor_Temp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace PulseWebRazor_Temp.Pages.Category
{
    public class IndexModel : PageModel
    {
		private readonly ApplicationDBContext? DBcontext;
		public List<PulseWebRazor_Temp.Models.Category> MyCategories { get; set; }
		public IndexModel(ApplicationDBContext db)
        {
			DBcontext = db;
		}
		public void OnGet()
        {
			MyCategories = DBcontext.Category.ToList();
		}
    }
}
