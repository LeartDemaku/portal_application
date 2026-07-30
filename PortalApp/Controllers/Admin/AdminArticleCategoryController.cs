using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PortalApp.Data;
using PortalApp.Models;

namespace PortalApp.Controllers.Admin
{
    [Route("Admin/[controller]/[action]/{id?}")]
    [Authorize(Roles = "Drejtor,Admin,Administrator,Redaktor")]
    public class AdminArticleCategoryController : Controller
    {
        private readonly ApplicationDbContext _db;

        public AdminArticleCategoryController(ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task<IActionResult> Index()
        {
            var categories = await _db.ArticleCategories
                .AsNoTracking()
                .OrderBy(x => x.Name)
                .ToListAsync();

            return View(categories);
        }

        public IActionResult Create()
        {
            return View(new ArticleCategory());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ArticleCategory category)
        {
            category.Name = category.Name.Trim();

            var exists = await _db.ArticleCategories.AnyAsync(x => x.Name.ToLower() == category.Name.ToLower());
            if (exists)
            {
                ModelState.AddModelError(nameof(category.Name), "Kjo kategori ekziston tashmë.");
            }

            if (!ModelState.IsValid)
            {
                return View(category);
            }

            category.CreatedAt = DateTime.Now;
            category.UpdatedAt = null;
            _db.ArticleCategories.Add(category);
            await _db.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Edit(int id)
        {
            var category = await _db.ArticleCategories.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);

            if (category == null)
            {
                return RedirectToAction(nameof(Index));
            }

            return View(category);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, ArticleCategory category)
        {
            var currentCategory = await _db.ArticleCategories.FirstOrDefaultAsync(x => x.Id == id);

            if (currentCategory == null)
            {
                return RedirectToAction(nameof(Index));
            }

            category.Name = category.Name.Trim();

            var exists = await _db.ArticleCategories.AnyAsync(x => x.Id != id && x.Name.ToLower() == category.Name.ToLower());
            if (exists)
            {
                ModelState.AddModelError(nameof(category.Name), "Kjo kategori ekziston tashmë.");
            }

            if (!ModelState.IsValid)
            {
                category.Id = id;
                category.CreatedAt = currentCategory.CreatedAt;
                return View(category);
            }

            currentCategory.Name = category.Name;
            currentCategory.UpdatedAt = DateTime.Now;
            await _db.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var category = await _db.ArticleCategories.FirstOrDefaultAsync(x => x.Id == id);

            if (category == null)
            {
                return RedirectToAction(nameof(Index));
            }

            var hasArticles = await _db.Articles.AnyAsync(x => x.ArticleCategoryId == id);

            if (hasArticles)
            {
                TempData["ErrorMessage"] = "Kjo kategori përmban artikuj dhe nuk mund të fshihet.";
                return RedirectToAction(nameof(Index));
            }

            _db.ArticleCategories.Remove(category);
            await _db.SaveChangesAsync();

            TempData["SuccessMessage"] = "Kategoria u fshi me sukses.";
            return RedirectToAction(nameof(Index));
        }
    }
}
