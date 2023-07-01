using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using PulseWebRazor_Temp.Data;
using PulseWebRazor_Temp.Models;

namespace PulseWebRazor_Temp.Pages.Category
{
	[BindProperties]
	public class DeleteModel : PageModel
    {
		private readonly ApplicationDBContext? DBcontext;
		public PulseWebRazor_Temp.Models.Category MyCategories { get; set; }
		public DeleteModel(ApplicationDBContext db)
		{
			DBcontext = db;
		}
		public void OnGet(int? id)
		{
			if (id != null && id != 0)
			{
				MyCategories = DBcontext.Category.Find(id);
			}
		}
		public IActionResult OnPost()
		{
			PulseWebRazor_Temp.Models.Category? category = DBcontext?.Category.Find(MyCategories.Id);
			if (category == null)
				NotFound();
			
			DBcontext.Category.Remove(category);
			DBcontext?.SaveChanges();
			TempData["Message"] = "Category deleted successfully!";
			return RedirectToPage("Index");
		}
	}
}
