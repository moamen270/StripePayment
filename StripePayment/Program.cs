using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Stripe;
using StripePayment.Data;
using StripePayment.Models;
using StripePayment.Setting;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddIdentityCore<User>(options => options.SignIn.RequireConfirmedEmail = false).AddEntityFrameworkStores<ApplicationDbContext>().AddUserManager<UserManager<User>>().AddSignInManager<SignInManager<User>>();
// Add services to the container.
builder.Services.AddAuthentication().AddCookie("Identity.Application");
builder.Services.AddHttpContextAccessor();
builder.Services.AddControllersWithViews();

builder.Services.Configure<StripeSetting>(builder.Configuration.GetSection("Stripe"));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
	app.UseExceptionHandler("/Home/Error");
	// The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
	app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
StripeConfiguration.ApiKey = builder.Configuration.GetSection("Stripe:SecretKey").Get<string>();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
	name: "default",
	pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();