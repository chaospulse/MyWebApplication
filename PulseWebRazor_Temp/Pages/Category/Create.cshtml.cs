using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using PulseWebRazor_Temp.Data;

namespace PulseWebRazor_Temp.Pages.Category
{
	[BindProperties]
	public class CreateModel : PageModel
    {
		private readonly ApplicationDBContext? DBcontext;
		[BindProperty]
		public PulseWebRazor_Temp.Models.Category MyCategories { get; set; }
		public CreateModel(ApplicationDBContext db)
		{
			DBcontext = db;
		}
		public void OnGet() { }
		public IActionResult OnPost()
		{
			DBcontext?.Add(MyCategories);
			DBcontext?.SaveChanges();
			TempData["Message"] = "Category created successfully!";
			return RedirectToPage("Index");
		}
    }
}
