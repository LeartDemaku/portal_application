using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;
using PortalApp.Data;
using PortalApp.Models;
using PortalApp.Services;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    options.UseSqlServer(connectionString);
});
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options => options.SignIn.RequireConfirmedAccount = false)
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders()
    .AddDefaultUI();

builder.Services.AddControllersWithViews()
    .AddRazorOptions(options =>
    {
        options.ViewLocationFormats.Add("/Views/Admin/{1}/{0}.cshtml");
    });

builder.Services.AddTransient<IEmailSender, EmailSender>();
builder.Services.AddScoped<IPortalQueryService, PortalQueryService>();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
    var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();

    var roles = new[] { "Drejtor", "Admin", "Administrator", "Gazetar", "Redaktor" };
    foreach (var role in roles)
    {
        if (!await roleManager.RoleExistsAsync(role))
        {
            await roleManager.CreateAsync(new IdentityRole(role));
        }
    }

    var categoriesToSeed = new[] { "Sport", "Siguri", "Teknologji", "Ekonomi", "Shëndetësi", "Globale", "Politikë" };
    foreach (var catName in categoriesToSeed)
    {
        if (!await dbContext.ArticleCategories.AnyAsync(x => x.Name == catName))
        {
            dbContext.ArticleCategories.Add(new ArticleCategory
            {
                Name = catName,
                CreatedAt = DateTime.Now
            });
        }
    }
    await dbContext.SaveChangesAsync();

    var tagsToSeed = new[] { "Teknologji", "Sport", "Siguri", "Ekonomi", "Robotikë", "Shëndetësi", "Globale", "Politikë" };
    foreach (var tagName in tagsToSeed)
    {
        if (!await dbContext.ArticleTags.AnyAsync(x => x.Title == tagName))
        {
            dbContext.ArticleTags.Add(new ArticleTag
            {
                Title = tagName,
                CreatedAt = DateTime.Now
            });
        }
    }
    await dbContext.SaveChangesAsync();

    var defaultUsers = new (string Email, string Role)[]
    {
        ("admin@portal.com", "Admin"),
        ("drejtor@portal.com", "Drejtor"),
        ("gazetar@portal.com", "Gazetar"),
        ("redaktor@portal.com", "Redaktor")
    };

    foreach (var defaultUser in defaultUsers)
    {
        var existingUser = await userManager.FindByEmailAsync(defaultUser.Email);
        if (existingUser == null)
        {
            var user = new ApplicationUser
            {
                FirstName = defaultUser.Role,
                LastName = "Portal",
                Email = defaultUser.Email,
                UserName = defaultUser.Email,
                EmailConfirmed = true
            };
            var result = await userManager.CreateAsync(user, "Password123!");
            if (result.Succeeded)
            {
                await userManager.AddToRoleAsync(user, defaultUser.Role);
            }
        }
        else
        {
            var hasRole = await userManager.IsInRoleAsync(existingUser, defaultUser.Role);
            if (!hasRole)
            {
                await userManager.AddToRoleAsync(existingUser, defaultUser.Role);
            }
        }
    }

    var adminUser = await userManager.FindByEmailAsync("admin@portal.com");
    if (adminUser != null)
    {
        var token = await userManager.GeneratePasswordResetTokenAsync(adminUser);
        await userManager.ResetPasswordAsync(adminUser, token, "Password123!");
    }
}

if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.MapRazorPages();

app.Run();