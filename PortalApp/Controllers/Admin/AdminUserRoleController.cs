using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PortalApp.Data;
using PortalApp.Models.ViewModels;

namespace PortalApp.Controllers.Admin
{
    [Route("Admin/[controller]/[action]/{id?}")]
    [Authorize(Roles = "Drejtor,Admin,Administrator")]
    public class AdminUserRoleController : Controller
    {
        private readonly ApplicationDbContext _db;
        private readonly RoleManager<IdentityRole> _roleManager;

        public AdminUserRoleController(
            ApplicationDbContext db,
            RoleManager<IdentityRole> roleManager)
        {
            _db = db;
            _roleManager = roleManager;
        }

        public async Task<IActionResult> Index()
        {
            var roles = await _db.Roles.AsNoTracking().OrderBy(x => x.Name).ToListAsync();
            return View(roles);
        }

        public IActionResult Create()
        {
            return View(new UserRoleStoreViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(UserRoleStoreViewModel data)
        {
            var roleName = data.RoleName.Trim();

            if (await _roleManager.RoleExistsAsync(roleName))
            {
                ModelState.AddModelError(nameof(data.RoleName), "Ky rol ekziston tashmë.");
            }

            if (!ModelState.IsValid)
            {
                return View(data);
            }

            var result = await _roleManager.CreateAsync(new IdentityRole { Name = roleName });

            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }

                return View(data);
            }

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Edit(string id)
        {
            var role = await _db.Roles.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);

            if (role == null)
            {
                return RedirectToAction(nameof(Index));
            }

            ViewBag.RoleId = role.Id;

            return View(new UserRoleStoreViewModel
            {
                RoleName = role.Name ?? string.Empty
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, UserRoleStoreViewModel data)
        {
            var currentRole = await _roleManager.FindByIdAsync(id);

            if (currentRole == null)
            {
                return RedirectToAction(nameof(Index));
            }

            var roleName = data.RoleName.Trim();
            var existingRole = await _db.Roles.AsNoTracking().FirstOrDefaultAsync(x => x.Name == roleName && x.Id != id);

            if (existingRole != null)
            {
                ModelState.AddModelError(nameof(data.RoleName), "Ky rol ekziston tashmë.");
            }

            if (!ModelState.IsValid)
            {
                ViewBag.RoleId = id;
                return View(data);
            }

            currentRole.Name = roleName;
            var result = await _roleManager.UpdateAsync(currentRole);

            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }

                ViewBag.RoleId = id;
                return View(data);
            }

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(string id)
        {
            var currentRole = await _roleManager.FindByIdAsync(id);

            if (currentRole == null)
            {
                return RedirectToAction(nameof(Index));
            }

            var isInUse = await _db.UserRoles.AnyAsync(x => x.RoleId == id);

            if (isInUse)
            {
                return RedirectToAction(nameof(Index));
            }

            await _roleManager.DeleteAsync(currentRole);

            return RedirectToAction(nameof(Index));
        }
    }
}
