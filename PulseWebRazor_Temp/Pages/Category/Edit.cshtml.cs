using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using PulseWebRazor_Temp.Data;
using PulseWebRazor_Temp.Models;

namespace PulseWebRazor_Temp.Pages.Category
{
	[BindProperties]
	public class EditModel : PageModel
    {
		private readonly ApplicationDBContext? DBcontext;
		[BindProperty]
		public PulseWebRazor_Temp.Models.Category MyCategories { get; set; }
		public EditModel(ApplicationDBContext db) { DBcontext = db; }
		public void OnGet(int? id)
		{
            if (id != null && id != 0)
			{
				MyCategories = DBcontext.Category.Find(id);
			}
		}
		public IActionResult OnPost()
		{
			if (ModelState.IsValid)
			{
				DBcontext?.Update(MyCategories);
				DBcontext?.SaveChanges();
				TempData["Message"] = "Category updated successfully!";
				return RedirectToPage("Index");
			}
			return Page();
		}
	}
}
