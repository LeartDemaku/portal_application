using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PortalApp.Data;
using PortalApp.Models;
using PortalApp.Models.ViewModels;
using PortalApp.Services;

namespace PortalApp.Controllers
{
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _db;
        private readonly IPortalQueryService _portalQueryService;

        public HomeController(ApplicationDbContext db, IPortalQueryService portalQueryService)
        {
            _db = db;
            _portalQueryService = portalQueryService;
        }

        public async Task<IActionResult> Index()
        {
            return View(await _portalQueryService.GetHomePageAsync());
        }

        [Route("artikulli/{id:int}")]
        public async Task<IActionResult> Show(int id)
        {
            var article = await _portalQueryService.GetArticleDetailsAsync(id);

            if (article == null)
            {
                return RedirectToAction(nameof(Index));
            }

            return View(article);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Comment(int id, ArticleCommentStoreViewModel data)
        {
            var article = await _db.Articles
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == id && x.ApprovedBy != null);

            if (article == null)
            {
                return RedirectToAction(nameof(Index));
            }

            if (!ModelState.IsValid)
            {
                var pageModel = await _portalQueryService.GetArticleDetailsAsync(id);

                if (pageModel == null)
                {
                    return RedirectToAction(nameof(Index));
                }

                pageModel.CommentForm = data;
                return View("Show", pageModel);
            }

            _db.ArticleComments.Add(new ArticleComment
            {
                ArticleId = article.Id,
                Name = data.Name.Trim(),
                Email = data.Email.Trim(),
                Comment = data.Comment.Trim(),
                CreatedAt = DateTime.Now
            });

            await _db.SaveChangesAsync();

            return RedirectToAction(nameof(Show), new { id = article.Id });
        }

        [Route("kerko")]
        public async Task<IActionResult> Search(string? q)
        {
            return View(await _portalQueryService.SearchAsync(q));
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = System.Diagnostics.Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
