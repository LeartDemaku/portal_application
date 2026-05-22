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

builder.Services.AddDefaultIdentity<ApplicationUser>(options => options.SignIn.RequireConfirmedAccount = false)
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>();

builder.Services.AddControllersWithViews().AddRazorOptions(options =>
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

    var firstUser = await dbContext.Users.OrderBy(x => x.Email).FirstOrDefaultAsync();
    if (firstUser != null)
    {
        var hasAdminRole = await userManager.IsInRoleAsync(firstUser, "Admin");
        if (!hasAdminRole)
        {
            await userManager.AddToRoleAsync(firstUser, "Admin");
        }
    }

    var fallbackUserId = await dbContext.Users
        .AsNoTracking()
        .OrderBy(x => x.Email)
        .Select(x => x.Id)
        .FirstOrDefaultAsync();

    if (!string.IsNullOrWhiteSpace(fallbackUserId))
    {
        var unpublishedArticles = await dbContext.Articles
            .Where(x => x.ApprovedBy == null)
            .ToListAsync();

        if (unpublishedArticles.Count > 0)
        {
            foreach (var article in unpublishedArticles)
            {
                article.CreatedBy ??= fallbackUserId;
                article.ApprovedBy = article.CreatedBy ?? fallbackUserId;
            }

            await dbContext.SaveChangesAsync();
        }
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
    name: "admin",
    pattern: "Admin/{controller=AdminArticle}/{action=Index}/{id?}");

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}"
    );



app.MapRazorPages();

app.Run();
