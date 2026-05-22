using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PortalApp.Data;
using PortalApp.Models;
using PortalApp.Models.ViewModels;

namespace PortalApp.Controllers.Admin
{
    [Authorize(Roles = "Drejtor,Admin,Administrator")]
    public class AdminUserController : Controller
    {
        private readonly ApplicationDbContext _db;
        private readonly UserManager<ApplicationUser> _userManager;

        public AdminUserController(
            ApplicationDbContext db,
            UserManager<ApplicationUser> userManager)
        {
            _db = db;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            var users = await _db.Users.AsNoTracking().OrderBy(x => x.FirstName).ThenBy(x => x.LastName).ToListAsync();
            var allUsers = new List<ApplicationUserViewModel>();

            foreach (var user in users)
            {
                var userRoles = await _userManager.GetRolesAsync(user);

                allUsers.Add(new ApplicationUserViewModel
                {
                    Id = user.Id,
                    FirstName = user.FirstName ?? string.Empty,
                    LastName = user.LastName ?? string.Empty,
                    Email = user.Email ?? string.Empty,
                    Roles = userRoles.ToList()
                });
            }

            return View(allUsers);
        }

        public async Task<IActionResult> Create()
        {
            await PopulateRolesAsync();
            return View(new UserStoreViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(UserStoreViewModel data)
        {
            if (string.IsNullOrWhiteSpace(data.Password))
            {
                ModelState.AddModelError(nameof(data.Password), "Fjalëkalimi është i detyrueshëm.");
            }

            var currentRole = await _db.Roles.AsNoTracking().FirstOrDefaultAsync(x => x.Id == data.RoleId);

            if (currentRole == null || string.IsNullOrWhiteSpace(currentRole.Name))
            {
                ModelState.AddModelError(nameof(data.RoleId), "Roli i zgjedhur nuk ekziston.");
            }

            if (!ModelState.IsValid)
            {
                await PopulateRolesAsync();
                return View(data);
            }

            var newUser = new ApplicationUser
            {
                FirstName = data.FirstName.Trim(),
                LastName = data.LastName.Trim(),
                Email = data.Email.Trim(),
                UserName = data.Email.Trim(),
                EmailConfirmed = true
            };

            var result = await _userManager.CreateAsync(newUser, data.Password);

            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }

                await PopulateRolesAsync();
                return View(data);
            }

            await _userManager.AddToRoleAsync(newUser, currentRole!.Name!);

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Edit(string id)
        {
            var user = await _userManager.FindByIdAsync(id);

            if (user == null)
            {
                return RedirectToAction(nameof(Index));
            }

            await PopulateRolesAsync();

            var currentRole = (await _userManager.GetRolesAsync(user)).FirstOrDefault() ?? string.Empty;
            var currentRoleId = await _db.Roles
                .AsNoTracking()
                .Where(x => x.Name == currentRole)
                .Select(x => x.Id)
                .FirstOrDefaultAsync() ?? string.Empty;

            ViewBag.UserId = user.Id;
            ViewBag.CurrentRoleId = currentRoleId;

            return View(new UserStoreViewModel
            {
                FirstName = user.FirstName ?? string.Empty,
                LastName = user.LastName ?? string.Empty,
                Email = user.Email ?? string.Empty,
                RoleId = currentRoleId
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, UserStoreViewModel data)
        {
            var currentUser = await _userManager.FindByIdAsync(id);

            if (currentUser == null)
            {
                return RedirectToAction(nameof(Index));
            }

            var currentRole = await _db.Roles.AsNoTracking().FirstOrDefaultAsync(x => x.Id == data.RoleId);

            if (currentRole == null || string.IsNullOrWhiteSpace(currentRole.Name))
            {
                ModelState.AddModelError(nameof(data.RoleId), "Roli i zgjedhur nuk ekziston.");
            }

            if (!ModelState.IsValid)
            {
                await PopulateRolesAsync();
                ViewBag.UserId = currentUser.Id;
                ViewBag.CurrentRoleId = data.RoleId;
                return View(data);
            }

            currentUser.FirstName = data.FirstName.Trim();
            currentUser.LastName = data.LastName.Trim();
            currentUser.Email = data.Email.Trim();
            currentUser.UserName = data.Email.Trim();

            var updateResult = await _userManager.UpdateAsync(currentUser);

            if (!updateResult.Succeeded)
            {
                foreach (var error in updateResult.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }

                await PopulateRolesAsync();
                ViewBag.UserId = currentUser.Id;
                ViewBag.CurrentRoleId = data.RoleId;
                return View(data);
            }

            if (!string.IsNullOrWhiteSpace(data.Password))
            {
                var token = await _userManager.GeneratePasswordResetTokenAsync(currentUser);
                var resetResult = await _userManager.ResetPasswordAsync(currentUser, token, data.Password);

                if (!resetResult.Succeeded)
                {
                    foreach (var error in resetResult.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }

                    await PopulateRolesAsync();
                    ViewBag.UserId = currentUser.Id;
                    ViewBag.CurrentRoleId = data.RoleId;
                    return View(data);
                }
            }

            var roles = await _userManager.GetRolesAsync(currentUser);
            if (roles.Any())
            {
                await _userManager.RemoveFromRolesAsync(currentUser, roles);
            }

            await _userManager.AddToRoleAsync(currentUser, currentRole!.Name!);

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(string id)
        {
            var currentUser = await _userManager.FindByIdAsync(id);

            if (currentUser == null)
            {
                return RedirectToAction(nameof(Index));
            }

            if (currentUser.Id == _userManager.GetUserId(User))
            {
                return RedirectToAction(nameof(Index));
            }

            var userArticles = await _db.Articles.Where(x => x.CreatedBy == currentUser.Id).ToListAsync();

            foreach (var article in userArticles)
            {
                article.CreatedBy = null;
            }

            await _db.SaveChangesAsync();
            await _userManager.DeleteAsync(currentUser);

            return RedirectToAction(nameof(Index));
        }

        private async Task PopulateRolesAsync()
        {
            ViewBag.Roles = await _db.Roles
                .AsNoTracking()
                .OrderBy(x => x.Name)
                .ToListAsync();
        }
    }
}
