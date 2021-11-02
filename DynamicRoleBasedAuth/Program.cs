using DynamicRoleBasedAuth.Data;
using DynamicRoleBasedAuth.Identity;

using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("AppDbContextConnection");
builder.Services.AddDbContext<AppDbContext>(options =>
{
    options.UseSqlServer(connectionString);
    options.LogTo(Console.WriteLine);
});

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

builder.Services.AddSingleton<ActionDetection>();

var app = builder.Build();

await SeedDatabase(app.Services);

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

static async Task SeedDatabase(IServiceProvider serviceProvider)
{
    using var scope = serviceProvider.CreateScope();
    var services = scope.ServiceProvider;
    var userManager = services.GetRequiredService<UserManager<IdentityUser>>();
    var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();

    var adminUser = new IdentityUser()
    {
        UserName = "admin@site.com",
        Email = "admin@site.com",
        EmailConfirmed = true
    };
    var adminRole = new IdentityRole(Roles.Admin);
    var userRole = new IdentityRole(Roles.User);

    if ((await roleManager.FindByNameAsync(Roles.Admin)) is null)
    {
        await roleManager.CreateAsync(adminRole);
    }

    if ((await roleManager.FindByNameAsync(Roles.User)) is null)
    {
        await roleManager.CreateAsync(userRole);
    }

    if ((await userManager.FindByEmailAsync(adminUser.Email)) is null)
    {
        await userManager.CreateAsync(adminUser, "pass");
        await userManager.AddToRoleAsync(adminUser, Roles.Admin);
    }
}