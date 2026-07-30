using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PortalApp.Data;

namespace PortalApp.Controllers.Admin
{
    [Route("Admin/[controller]/[action]/{id?}")]
    [Authorize(Roles = "Drejtor,Admin,Administrator,Redaktor")]
    public class AdminArticleCommentController : Controller
    {
        private readonly ApplicationDbContext _db;

        public AdminArticleCommentController(ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task<IActionResult> Index()
        {
            var comments = await _db.ArticleComments
                .AsNoTracking()
                .Include(x => x.Article)
                .OrderByDescending(x => x.CreatedAt)
                .ToListAsync();

            return View(comments);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var comment = await _db.ArticleComments.FirstOrDefaultAsync(x => x.Id == id);

            if (comment != null)
            {
                _db.ArticleComments.Remove(comment);
                await _db.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }
    }
}
