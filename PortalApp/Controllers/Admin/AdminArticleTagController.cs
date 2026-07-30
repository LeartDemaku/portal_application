using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PortalApp.Data;
using PortalApp.Models;

namespace PortalApp.Controllers.Admin
{
    [Route("Admin/[controller]/[action]/{id?}")]
    [Authorize(Roles = "Drejtor,Admin,Administrator,Redaktor")]
    public class AdminArticleTagController : Controller
    {
        private readonly ApplicationDbContext _db;

        public AdminArticleTagController(ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task<IActionResult> Index()
        {
            var tags = await _db.ArticleTags
                .AsNoTracking()
                .OrderBy(x => x.Title)
                .ToListAsync();

            return View(tags);
        }

        public IActionResult Create()
        {
            return View(new ArticleTag());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ArticleTag tag)
        {
            tag.Title = tag.Title.Trim();

            var exists = await _db.ArticleTags.AnyAsync(x => x.Title.ToLower() == tag.Title.ToLower());
            if (exists)
            {
                ModelState.AddModelError(nameof(tag.Title), "Ky tag ekziston tashmë.");
            }

            if (!ModelState.IsValid)
            {
                return View(tag);
            }

            tag.CreatedAt = DateTime.Now;
            _db.ArticleTags.Add(tag);
            await _db.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var tag = await _db.ArticleTags
                .Include(x => x.ArticleArticleTags)
                .FirstOrDefaultAsync(x => x.Id == id);

            if (tag == null)
            {
                return RedirectToAction(nameof(Index));
            }

            _db.ArticleArticleTags.RemoveRange(tag.ArticleArticleTags);
            _db.ArticleTags.Remove(tag);
            await _db.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }
    }
}
