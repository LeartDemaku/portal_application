using Microsoft.EntityFrameworkCore;
using PortalApp.Data;
using PortalApp.Models;
using PortalApp.Models.ViewModels;

namespace PortalApp.Services
{
    public interface IPortalQueryService
    {
        Task<List<PortalNavigationItemViewModel>> GetNavigationItemsAsync();
        Task<PortalHomeViewModel> GetHomePageAsync();
        Task<PortalListingPageViewModel?> GetSectionPageAsync(string slug);
        Task<PortalListingPageViewModel> GetTagPageAsync(string name);
        Task<PortalListingPageViewModel> SearchAsync(string? query);
        Task<PortalArticleDetailsViewModel?> GetArticleDetailsAsync(int id);
    }

    public class PortalQueryService : IPortalQueryService
    {
        private const string FallbackImage = "/images/articles/lajmi_kryesor.png";
        private readonly ApplicationDbContext _db;

        public PortalQueryService(ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task<List<PortalNavigationItemViewModel>> GetNavigationItemsAsync()
        {
            var categories = await _db.ArticleCategories
                .AsNoTracking()
                .OrderBy(category => category.Name)
                .ToListAsync();

            return categories
                .Select(category => new PortalNavigationItemViewModel
                {
                    Slug = PortalSectionRegistry.ToSlug(category.Name),
                    Title = category.Name
                })
                .ToList();
        }

        public async Task<PortalHomeViewModel> GetHomePageAsync()
        {
            var articles = await PublicArticleQuery()
                .Take(32)
                .ToListAsync();

            var ordered = articles
                .OrderByDescending(x => x.CreatedAt)
                .ToList();

            var sections = ordered
                .GroupBy(article => article.ArticleCategoryId)
                .OrderByDescending(group => group.Max(article => article.CreatedAt))
                .Select(group =>
                {
                    var definition = ResolveSection(group.First().ArticleCategory?.Name);

                    return new PortalSectionPreviewViewModel
                    {
                        Slug = definition.Slug,
                        Title = definition.Name,
                        Description = definition.Description,
                        AccentColor = definition.AccentColor,
                        Articles = group
                            .OrderByDescending(article => article.CreatedAt)
                            .Take(4)
                            .Select(MapCard)
                            .ToList()
                    };
                })
                .ToList();

            return new PortalHomeViewModel
            {
                FeaturedArticle = ordered.Select(MapCard).FirstOrDefault(),
                HeadlineArticles = ordered.Skip(1).Take(3).Select(MapCard).ToList(),
                LatestArticles = ordered.Skip(4).Take(12).Select(MapCard).ToList(),
                Sections = sections
            };
        }

        public async Task<PortalListingPageViewModel?> GetSectionPageAsync(string slug)
        {
            var normalizedSlug = PortalSectionRegistry.ToSlug(slug);
            var category = (await _db.ArticleCategories
                .AsNoTracking()
                .ToListAsync())
                .FirstOrDefault(item => PortalSectionRegistry.ToSlug(item.Name) == normalizedSlug);

            if (category == null)
            {
                return null;
            }

            var section = ResolveSection(category.Name);
            var ordered = await PublicArticleQuery()
                .Where(article => article.ArticleCategoryId == category.Id)
                .OrderByDescending(x => x.CreatedAt)
                .ToListAsync();

            return new PortalListingPageViewModel
            {
                Title = section.Name,
                Description = section.Description,
                AccentColor = section.AccentColor,
                CategorySlug = section.Slug,
                FeaturedArticle = ordered.Select(MapCard).FirstOrDefault(),
                HeadlineArticles = ordered.Skip(1).Take(3).Select(MapCard).ToList(),
                Articles = ordered.Skip(4).Select(MapCard).ToList()
            };
        }

        public async Task<PortalListingPageViewModel> GetTagPageAsync(string name)
        {
            var cleanName = name.Trim();
            var lowered = cleanName.ToLower();
            var articles = await PublicArticleQuery()
                .Where(article => article.ArticleTags.Any(tag => tag.ArticleTag.Title.ToLower() == lowered))
                .ToListAsync();

            var ordered = articles
                .OrderByDescending(x => x.CreatedAt)
                .ToList();

            return new PortalListingPageViewModel
            {
                Title = $"Tag: #{cleanName}",
                Description = "Artikuj te filtruar sipas temes dhe fjaleve kyce.",
                AccentColor = "#7c3aed",
                TagName = cleanName,
                FeaturedArticle = ordered.Select(MapCard).FirstOrDefault(),
                HeadlineArticles = ordered.Skip(1).Take(3).Select(MapCard).ToList(),
                Articles = ordered.Skip(4).Select(MapCard).ToList()
            };
        }

        public async Task<PortalListingPageViewModel> SearchAsync(string? query)
        {
            var cleanQuery = query?.Trim() ?? string.Empty;

            if (string.IsNullOrWhiteSpace(cleanQuery))
            {
                return new PortalListingPageViewModel
                {
                    Title = "Kerko",
                    Description = "Gjeni lajme sipas titullit, permbajtjes, kategorise ose tagut.",
                    AccentColor = "#334155",
                    SearchQuery = cleanQuery
                };
            }

            var pattern = $"%{cleanQuery}%";
            var articles = await PublicArticleQuery()
                .Where(article =>
                    EF.Functions.Like(article.Title, pattern) ||
                    EF.Functions.Like(article.Content, pattern) ||
                    EF.Functions.Like(article.ArticleCategory.Name, pattern) ||
                    article.ArticleTags.Any(tag => EF.Functions.Like(tag.ArticleTag.Title, pattern)))
                .ToListAsync();

            var ordered = articles
                .OrderByDescending(x => x.CreatedAt)
                .ToList();

            return new PortalListingPageViewModel
            {
                Title = $"Rezultatet per \"{cleanQuery}\"",
                Description = $"{ordered.Count} artikuj te gjetur nga kerkimi editorial.",
                AccentColor = "#334155",
                SearchQuery = cleanQuery,
                FeaturedArticle = ordered.Select(MapCard).FirstOrDefault(),
                HeadlineArticles = ordered.Skip(1).Take(3).Select(MapCard).ToList(),
                Articles = ordered.Skip(4).Select(MapCard).ToList()
            };
        }

        public async Task<PortalArticleDetailsViewModel?> GetArticleDetailsAsync(int id)
        {
            var article = await _db.Articles
                .AsNoTracking()
                .Include(x => x.ArticleCategory)
                .Include(x => x.CreatedByUser)
                .Include(x => x.ArticleTags)
                .ThenInclude(x => x.ArticleTag)
                .Include(x => x.ArticleComments.OrderByDescending(comment => comment.CreatedAt))
                .FirstOrDefaultAsync(x => x.Id == id && x.ApprovedBy != null);

            if (article == null)
            {
                return null;
            }

            var related = await PublicArticleQuery()
                .Where(x => x.Id != id && x.ArticleCategoryId == article.ArticleCategoryId)
                .Take(3)
                .ToListAsync();

            var section = ResolveSection(article.ArticleCategory?.Name);

            return new PortalArticleDetailsViewModel
            {
                Id = article.Id,
                Title = article.Title,
                Content = article.Content,
                CoverImageUrl = GetCoverImage(article.CoverImage),
                CategoryName = section.Name,
                CategorySlug = section.Slug,
                AuthorName = ResolveAuthor(article.CreatedByUser),
                CreatedAt = article.CreatedAt,
                ReadTimeMinutes = EstimateReadTime(article.Content),
                Paragraphs = SplitParagraphs(article.Content),
                Tags = article.ArticleTags
                    .Select(x => x.ArticleTag.Title)
                    .Where(x => !string.IsNullOrWhiteSpace(x))
                    .Distinct(StringComparer.OrdinalIgnoreCase)
                    .ToList(),
                Comments = article.ArticleComments
                    .OrderByDescending(x => x.CreatedAt)
                    .ToList(),
                RelatedArticles = related
                    .OrderByDescending(x => x.CreatedAt)
                    .Select(MapCard)
                    .ToList()
            };
        }

        private IQueryable<Article> PublicArticleQuery()
        {
            return _db.Articles
                .AsNoTracking()
                .Where(x => x.ApprovedBy != null)
                .Include(x => x.ArticleCategory)
                .Include(x => x.CreatedByUser)
                .Include(x => x.ArticleTags)
                .ThenInclude(x => x.ArticleTag)
                .OrderByDescending(x => x.CreatedAt);
        }

        private PortalArticleCardViewModel MapCard(Article article)
        {
            var section = ResolveSection(article.ArticleCategory?.Name);

            return new PortalArticleCardViewModel
            {
                Id = article.Id,
                Title = article.Title,
                Summary = BuildSummary(article.Content),
                CoverImageUrl = GetCoverImage(article.CoverImage),
                CategoryName = section.Name,
                CategorySlug = section.Slug,
                AuthorName = ResolveAuthor(article.CreatedByUser),
                CreatedAt = article.CreatedAt,
                ReadTimeMinutes = EstimateReadTime(article.Content),
                Tags = article.ArticleTags
                    .Select(x => x.ArticleTag.Title)
                    .Where(x => !string.IsNullOrWhiteSpace(x))
                    .Distinct(StringComparer.OrdinalIgnoreCase)
                    .Take(3)
                    .ToList()
            };
        }

        private static string ResolveAuthor(ApplicationUser? user)
        {
            var fullName = string.Join(" ", new[] { user?.FirstName, user?.LastName }.Where(x => !string.IsNullOrWhiteSpace(x)));
            return string.IsNullOrWhiteSpace(fullName) ? "Redaksia" : fullName;
        }

        private static PortalSectionDefinition ResolveSection(string? categoryName)
        {
            return PortalSectionRegistry.Resolve(categoryName);
        }

        private static string BuildSummary(string content)
        {
            var textOnly = System.Text.RegularExpressions.Regex.Replace(content, "<.*?>", string.Empty);
            var clean = string.Join(" ", textOnly
                .Replace("\r", " ", StringComparison.Ordinal)
                .Replace("\n", " ", StringComparison.Ordinal)
                .Split(' ', StringSplitOptions.RemoveEmptyEntries));

            if (clean.Length <= 170)
            {
                return clean;
            }

            return $"{clean[..167].TrimEnd()}...";
        }

        private static List<string> SplitParagraphs(string content)
        {
            if (string.IsNullOrWhiteSpace(content))
            {
                return new List<string>();
            }

            var paragraphs = content
                .Split(["\r\n\r\n", "\n\n"], StringSplitOptions.RemoveEmptyEntries)
                .Select(x => x.Trim())
                .Where(x => !string.IsNullOrWhiteSpace(x))
                .ToList();

            if (paragraphs.Count > 0)
            {
                return paragraphs;
            }

            var lines = content
                .Split(["\r\n", "\n"], StringSplitOptions.RemoveEmptyEntries)
                .Select(x => x.Trim())
                .Where(x => !string.IsNullOrWhiteSpace(x))
                .ToList();

            if (lines.Count > 0)
            {
                return lines;
            }

            return new List<string> { content.Trim() };
        }

        private static int EstimateReadTime(string content)
        {
            var wordCount = content.Split(' ', StringSplitOptions.RemoveEmptyEntries).Length;
            return Math.Max(1, (int)Math.Ceiling(wordCount / 220d));
        }

        private static string GetCoverImage(string? coverImage)
        {
            if (string.IsNullOrWhiteSpace(coverImage))
            {
                return FallbackImage;
            }

            var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", "articles", coverImage);
            if (!File.Exists(filePath))
            {
                return FallbackImage;
            }

            return $"/images/articles/{coverImage}";
        }
    }
}
