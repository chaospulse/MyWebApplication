using Microsoft.EntityFrameworkCore;
using PulseDataAccess.Data;
using PulseDataAccess.Repository;
using PulseDataAccess.Repository.IRepositry;
using Microsoft.AspNetCore.Identity;
using System.Security.Principal;
using Microsoft.AspNetCore.Identity.UI.Services;
using PulseUtility;
using Stripe;
using PulseDataAccess.DBInitializer;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddDbContext<ApplicationDBContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.Configure<StripeSettings>(builder.Configuration.GetSection("Stripe"));

builder.Services.AddIdentity<IdentityUser, IdentityRole>().AddEntityFrameworkStores<ApplicationDBContext>().AddDefaultTokenProviders();
builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = $"/Identity/Account/Login";
    options.LogoutPath = $"/Identity/Account/Logout";
    options.AccessDeniedPath = $"/Identity/Account/AccessDenied";
});

builder.Services.AddAuthentication().AddFacebook(option =>
{
    option.AppId = "3470383986535345";
    option.AppSecret = "69e9aebb160f2174df3dfea";
});

builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(100);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

builder.Services.AddScoped<IDBInitializer, DBInitializer>();
builder.Services.AddRazorPages();
builder.Services.AddScoped<ICategoryRepositry, CategoryRepositry>();
builder.Services.AddScoped<IProductRepositry, ProductRepositry>();
builder.Services.AddScoped<ICompanyRepositry, CompanyRepositry>();
builder.Services.AddScoped<IEmailSender, EmailSender>();
builder.Services.AddScoped<IApplicationUserRepositry, ApplicationUserRepositry>();
builder.Services.AddScoped<IShoppingCartRepositry, ShoppingCartRepositry>();
builder.Services.AddScoped<IOrderHeaderRepositry, OrderHeaderRepositry>();
builder.Services.AddScoped<IOrderDetailsRepositry, OrderDetailsRepositry>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

StripeConfiguration.ApiKey = builder.Configuration.GetSection("Stripe:SecretKey").Get<string>();

app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.UseSession();
SeedDataBase();
app.MapRazorPages();

app.MapControllerRoute(
    name: "default",
    pattern: "{area=Customer}/{controller=Home}/{action=Index}/{id?}") ;

app.Run();

void SeedDataBase()
{
    using (var scope = app.Services.CreateScope())
    {
        var DB_Initializer = scope.ServiceProvider.GetRequiredService<IDBInitializer>();
        DB_Initializer.Initialize();
    }
}
