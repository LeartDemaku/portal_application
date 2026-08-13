namespace PortalApp.Models.ViewModels
{
    public class PortalArticleCardViewModel
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Summary { get; set; } = string.Empty;
        public string CoverImageUrl { get; set; } = string.Empty;
        public string CategoryName { get; set; } = string.Empty;
        public string CategorySlug { get; set; } = string.Empty;
        public string AuthorName { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public int ReadTimeMinutes { get; set; }
        public List<string> Tags { get; set; } = new();
    }

    public class PortalSectionPreviewViewModel
    {
        public string Slug { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string AccentColor { get; set; } = string.Empty;
        public List<PortalArticleCardViewModel> Articles { get; set; } = new();
    }

    public class PortalNavigationItemViewModel
    {
        public string Slug { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
    }

    public class PortalHomeViewModel
    {
        public PortalArticleCardViewModel? FeaturedArticle { get; set; }
        public List<PortalArticleCardViewModel> HeadlineArticles { get; set; } = new();
        public List<PortalArticleCardViewModel> LatestArticles { get; set; } = new();
        public List<PortalSectionPreviewViewModel> Sections { get; set; } = new();
    }

    public class PortalListingPageViewModel
    {
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string AccentColor { get; set; } = string.Empty;
        public string? SearchQuery { get; set; }
        public string? TagName { get; set; }
        public string? CategorySlug { get; set; }
        public PortalArticleCardViewModel? FeaturedArticle { get; set; }
        public List<PortalArticleCardViewModel> HeadlineArticles { get; set; } = new();
        public List<PortalArticleCardViewModel> Articles { get; set; } = new();
    }

    public class PortalArticleDetailsViewModel
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public string CoverImageUrl { get; set; } = string.Empty;
        public string CategoryName { get; set; } = string.Empty;
        public string CategorySlug { get; set; } = string.Empty;
        public string AuthorName { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public int ReadTimeMinutes { get; set; }
        public List<string> Paragraphs { get; set; } = new();
        public List<string> Tags { get; set; } = new();
        public List<ArticleComment> Comments { get; set; } = new();
        public List<PortalArticleCardViewModel> RelatedArticles { get; set; } = new();
        public ArticleCommentStoreViewModel CommentForm { get; set; } = new();
    }
}
