using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using DynamicRoleBasedAuth.Data;
var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("AppDbContextConnection");
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(connectionString));

builder.Services.AddIdentity<IdentityUser, IdentityRole>(options =>
 {
     options.SignIn.RequireConfirmedAccount = true;
     options.Password.RequireUppercase = false;
     options.Password.RequireLowercase = false;
     options.Password.RequiredUniqueChars = 0;
     options.Password.RequireDigit = false;
     options.Password.RequiredLength = 3;
     options.Password.RequireNonAlphanumeric = false;
 })
.AddEntityFrameworkStores<AppDbContext>();

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();

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

app.UseAuthentication();
app.UseAuthentication();

app.UseAuthorization();

app.MapControllerRoute(
    name: "areas",
    pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}");


app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.MapRazorPages();

app.Run();