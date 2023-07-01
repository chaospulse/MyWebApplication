
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.Build.Tasks.Deployment.Bootstrapper;
using Microsoft.EntityFrameworkCore;
using PulseModels;
using PulseModels.Models;
using System;

namespace PulseDataAccess.Data
{
    public class ApplicationDBContext : IdentityDbContext<IdentityUser>
	{
        public ApplicationDBContext(DbContextOptions<ApplicationDBContext> options) : base(options)
        { }
        public DbSet<Category> Category { get; set;  }
		public DbSet<PulseModels.Models.Product> Products { get; set; }
		public DbSet<ApplicationUser> ApplicationUsers { get; set; }
        public DbSet<Company> Company { get; set; }
		public DbSet<ShoppingCart> ShoppingCart { get; set; }
        public DbSet<OrderHeader> OrderHeader { get; set; }
        public DbSet<OrderDetails> OrderDetails { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
			base.OnModelCreating(modelBuilder);
			
			modelBuilder.Entity<Category>().HasData(
                new Category { Id = 1, Name = "Action", DisplayOrder = 1 },
                new Category { Id = 2, Name = "SciFi", DisplayOrder = 2 },
                new Category { Id = 3, Name = "History", DisplayOrder = 3 });

            modelBuilder.Entity<Company>().HasData(
                new Company
				{
					Id = 1,
					Name = "Tech Solution",
					StreetAddress = "123 Main St",
					City = "New York",
					State = "NY",
					PostalCode = "10001",
					PhoneNumber = "212-555-1212"
				},
                 new Company
                 {
                     Id = 2,
                     Name = "Pulse Inc.",
                     StreetAddress = "123 Tech St",
                     City = "New York",
                     State = "NY",
                     PostalCode = "10001",
                     PhoneNumber = "212-555-1212"
                 },
                 new Company
                 {
                     Id = 3,
                     Name = "Zloy Inc.",
                     StreetAddress = "999 Work St",
                     City = "New York",
                     State = "NY",
                     PostalCode = "10001",
                     PhoneNumber = "212-555-1212"
                 }




                );

            modelBuilder.Entity<PulseModels.Models.Product>().HasData(
				new PulseModels.Models.Product
				{ 
                    Id = 1,
                    Title = "The Lord of the Rings",
                    Author = "J.R.R. Tolkien", 
                    ISBN = "9780544003415", 
                    Description = "The Lord of the Rings is an epic high-fantasy novel written by English author and scholar J. R. R. Tolkien.",
					ListPrice = 20,
					Price = 18,
					Price50 = 15,
					Price100 = 12,
					CategoryId = 1,
					ImageUrl = ""

				},
				new PulseModels.Models.Product {
					Id = 2,
					Title = "The Hobbit",
					Author = "J.R.R. Tolkien",
					ISBN = "9780544003415",
					Description = "The Hobbit, or There and Back Again is a children's fantasy novel by English author J. R. R. Tolkien.",
					ListPrice = 10,
					Price = 8,
					Price50 = 7,
					Price100 = 6,
					CategoryId = 2,
					ImageUrl = ""

				},
                new PulseModels.Models.Product
                {
					Id = 3,
					Title = "Rock in the Ocean",
					Author = "Ron Parker",
					ISBN = "SOTJ1111111101",
					Description = "Preasent vitae sodales libero. Praesent mole.",
					ListPrice = 30,
					Price = 27,
					Price50 = 25,
					Price100 = 20,
					CategoryId = 3,
					ImageUrl = ""
				});
		}
    }
}

