using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PortalApp.Data;
using PortalApp.Models;
using PortalApp.Models.ViewModels;

namespace PortalApp.Controllers.Admin
{
    [Route("Admin/[controller]/[action]/{id?}")]
    [Authorize]
    public class AdminArticleController : Controller
    {
        private static readonly string[] ElevatedRoles = ["Drejtor", "Admin", "Administrator", "Redaktor"];

        private readonly ApplicationDbContext _db;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IWebHostEnvironment _environment;

        public AdminArticleController(
            ApplicationDbContext db,
            UserManager<ApplicationUser> userManager,
            IWebHostEnvironment environment)
        {
            _db = db;
            _userManager = userManager;
            _environment = environment;
        }

        public async Task<IActionResult> Index()
        {
            var userId = _userManager.GetUserId(User);

            if (string.IsNullOrWhiteSpace(userId))
            {
                return Redirect("/Identity/Account/Login");
            }

            var isElevated = await IsElevatedUserAsync();
            var query = _db.Articles
                .AsNoTracking()
                .Include(x => x.ArticleCategory)
                .Include(x => x.CreatedByUser)
                .OrderByDescending(x => x.CreatedAt)
                .AsQueryable();

            if (!isElevated)
            {
                query = query.Where(x => x.CreatedBy == userId);
            }

            var articles = await query.ToListAsync();
            return View(articles);
        }

        public async Task<IActionResult> Create()
        {
            var model = new ArticleStoreViewModel();
            ViewBag.CanApprove = await IsElevatedUserAsync();
            await PopulateArticleFormDataAsync();
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ArticleStoreViewModel data)
        {
            var userId = _userManager.GetUserId(User);

            if (string.IsNullOrWhiteSpace(userId))
            {
                return Redirect("/Identity/Account/Login");
            }

            if (data.CoverImage == null)
            {
                ModelState.AddModelError(nameof(data.CoverImage), "Imazhi i kopertinës është i detyrueshëm.");
            }

            if (!await _db.ArticleCategories.AnyAsync(x => x.Id == data.ArticleCategoryId))
            {
                ModelState.AddModelError(nameof(data.ArticleCategoryId), "Kategoria e zgjedhur nuk ekziston.");
            }

            if (!ModelState.IsValid)
            {
                ViewBag.CanApprove = await IsElevatedUserAsync();
                await PopulateArticleFormDataAsync();
                return View(data);
            }

            var isElevated = await IsElevatedUserAsync();
            var article = new Article
            {
                Title = data.Title.Trim(),
                Content = data.Content.Trim(),
                ArticleCategoryId = data.ArticleCategoryId,
                CoverImage = await SaveImageAsync(data.CoverImage!),
                CreatedAt = DateTime.Now,
                CreatedBy = userId,
                ApprovedBy = isElevated && data.IsApproved ? userId : null
            };

            _db.Articles.Add(article);
            await _db.SaveChangesAsync();

            await SyncArticleTagsAsync(article.Id, data.ArticleTags);
            await _db.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Edit(int id)
        {
            var article = await _db.Articles
                .AsNoTracking()
                .Include(x => x.ArticleTags)
                .ThenInclude(x => x.ArticleTag)
                .FirstOrDefaultAsync(x => x.Id == id);

            if (article == null)
            {
                return RedirectToAction(nameof(Index));
            }

            if (!await CanManageArticleAsync(article))
            {
                return Forbid();
            }

            await PopulateArticleFormDataAsync();
            ViewBag.CanManageCreator = await IsElevatedUserAsync();
            ViewBag.CanApprove = await IsElevatedUserAsync();

            return View(new ArticleEditViewModel
            {
                Title = article.Title,
                Content = article.Content,
                ArticleCategoryId = article.ArticleCategoryId,
                CreatedBy = article.CreatedBy ?? string.Empty,
                CurrentCoverImage = article.CoverImage,
                IsApproved = !string.IsNullOrWhiteSpace(article.ApprovedBy),
                ArticleTags = article.ArticleTags
                    .Select(x => x.ArticleTag.Title)
                    .Where(x => !string.IsNullOrWhiteSpace(x))
                    .Distinct(StringComparer.OrdinalIgnoreCase)
                    .ToList()
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, ArticleEditViewModel data)
        {
            var article = await _db.Articles
                .Include(x => x.ArticleTags)
                .ThenInclude(x => x.ArticleTag)
                .Include(x => x.ArticleComments)
                .FirstOrDefaultAsync(x => x.Id == id);

            if (article == null)
            {
                return RedirectToAction(nameof(Index));
            }

            var isElevated = await IsElevatedUserAsync();

            if (!await CanManageArticleAsync(article))
            {
                return Forbid();
            }

            if (!await _db.ArticleCategories.AnyAsync(x => x.Id == data.ArticleCategoryId))
            {
                ModelState.AddModelError(nameof(data.ArticleCategoryId), "Kategoria e zgjedhur nuk ekziston.");
            }

            if (!ModelState.IsValid)
            {
                data.CurrentCoverImage = article.CoverImage;
                await PopulateArticleFormDataAsync();
                ViewBag.CanManageCreator = isElevated;
                ViewBag.CanApprove = isElevated;
                return View(data);
            }

            article.Title = data.Title.Trim();
            article.Content = data.Content.Trim();
            article.ArticleCategoryId = data.ArticleCategoryId;
            article.UpdatedAt = DateTime.Now;

            if (isElevated && !string.IsNullOrWhiteSpace(data.CreatedBy))
            {
                article.CreatedBy = data.CreatedBy;
            }

            if (isElevated)
            {
                article.ApprovedBy = data.IsApproved ? _userManager.GetUserId(User) : null;
            }
            else
            {
                article.ApprovedBy = null;
            }

            if (data.CoverImage != null)
            {
                DeleteImage(article.CoverImage);
                article.CoverImage = await SaveImageAsync(data.CoverImage);
            }

            _db.ArticleArticleTags.RemoveRange(article.ArticleTags);
            await _db.SaveChangesAsync();

            await SyncArticleTagsAsync(article.Id, data.ArticleTags);
            await _db.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var article = await _db.Articles
                .Include(x => x.ArticleTags)
                .Include(x => x.ArticleComments)
                .FirstOrDefaultAsync(x => x.Id == id);

            if (article == null)
            {
                return RedirectToAction(nameof(Index));
            }

            if (!await CanManageArticleAsync(article))
            {
                return Forbid();
            }

            _db.ArticleArticleTags.RemoveRange(article.ArticleTags);
            _db.ArticleComments.RemoveRange(article.ArticleComments);
            _db.Articles.Remove(article);
            await _db.SaveChangesAsync();

            DeleteImage(article.CoverImage);

            return RedirectToAction(nameof(Index));
        }

        private async Task<bool> CanManageArticleAsync(Article article)
        {
            if (await IsElevatedUserAsync())
            {
                return true;
            }

            var userId = _userManager.GetUserId(User);
            return !string.IsNullOrWhiteSpace(userId) && article.CreatedBy == userId;
        }

        private async Task<bool> IsElevatedUserAsync()
        {
            foreach (var role in ElevatedRoles)
            {
                if (User.IsInRole(role))
                {
                    return true;
                }
            }

            var user = await _userManager.GetUserAsync(User);

            if (user == null)
            {
                return false;
            }

            var roles = await _userManager.GetRolesAsync(user);
            return roles.Any(x => ElevatedRoles.Contains(x, StringComparer.OrdinalIgnoreCase));
        }

        private async Task PopulateArticleFormDataAsync()
        {
            ViewBag.ArticleCategories = await _db.ArticleCategories
                .AsNoTracking()
                .OrderBy(x => x.Name)
                .ToListAsync();

            ViewBag.ArticleTags = await _db.ArticleTags
                .AsNoTracking()
                .OrderBy(x => x.Title)
                .ToListAsync();

            ViewBag.Users = await _db.Users
                .AsNoTracking()
                .OrderBy(x => x.FirstName)
                .ThenBy(x => x.LastName)
                .ToListAsync();
        }

        private async Task SyncArticleTagsAsync(int articleId, IEnumerable<string>? rawTags)
        {
            var existingJoinEntries = await _db.ArticleArticleTags
                .Where(x => x.ArticleId == articleId)
                .ToListAsync();

            if (existingJoinEntries.Any())
            {
                _db.ArticleArticleTags.RemoveRange(existingJoinEntries);
                await _db.SaveChangesAsync();
            }

            var normalizedTags = rawTags?
                .Where(x => !string.IsNullOrWhiteSpace(x))
                .Select(x => x.Trim())
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .ToList() ?? new List<string>();

            foreach (var tagTitle in normalizedTags)
            {
                var existingTag = await _db.ArticleTags
                    .FirstOrDefaultAsync(x => x.Title.ToLower() == tagTitle.ToLower());

                if (existingTag == null)
                {
                    existingTag = new ArticleTag
                    {
                        Title = tagTitle,
                        CreatedAt = DateTime.Now
                    };

                    _db.ArticleTags.Add(existingTag);
                    await _db.SaveChangesAsync();
                }

                _db.ArticleArticleTags.Add(new ArticleTags
                {
                    ArticleId = articleId,
                    ArticleTagId = existingTag.Id,
                    CreatedAt = DateTime.Now
                });
            }
        }

        private async Task<string> SaveImageAsync(IFormFile file)
        {
            var webRoot = _environment.WebRootPath ?? Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
            var uploadsPath = Path.Combine(webRoot, "images", "articles");
            Directory.CreateDirectory(uploadsPath);

            var fileName = $"{Guid.NewGuid()}{Path.GetExtension(file.FileName)}";
            var fullPath = Path.Combine(uploadsPath, fileName);

            await using var stream = new FileStream(fullPath, FileMode.Create);
            await file.CopyToAsync(stream);

            return fileName;
        }

        private void DeleteImage(string? fileName)
        {
            if (string.IsNullOrWhiteSpace(fileName))
            {
                return;
            }

            var webRoot = _environment.WebRootPath ?? Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
            var fullPath = Path.Combine(webRoot, "images", "articles", fileName);

            if (System.IO.File.Exists(fullPath))
            {
                System.IO.File.Delete(fullPath);
            }
        }
    }
}
