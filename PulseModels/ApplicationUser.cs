using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace PulseModels
{
	public class ApplicationUser : IdentityUser
	{
		[Required]
		public string Name { get; set; }
		public string? StreetAddress { get; set; }
		public string? City { get; set; }
		public string? PostalCode { get; set; }
		public string? State { get; set; }
		public int? CompanyId { get; set; }
        public string? Role { get; set; }
        [ForeignKey("CompanyId")]
        [ValidateNever]
        public Company Company { get; set; }
    }
}
