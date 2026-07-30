using System.ComponentModel.DataAnnotations;

namespace PortalApp.Models.ViewModels
{
    public class ArticleStoreViewModel
    {
        public ArticleStoreViewModel()
        {
            ArticleTags = new List<string>();
            ArticleCategories = new List<ArticleCategory>();
        }

        [Required]
        public string Title { get; set; } = string.Empty;

        public IFormFile? CoverImage { get; set; }

        [Required]
        public string Content { get; set; } = string.Empty;

        [Range(1, int.MaxValue)]
        public int ArticleCategoryId { get; set; }

        public bool IsApproved { get; set; }

        public List<string> ArticleTags { get; set; }
        public List<ArticleCategory> ArticleCategories { get; set; }
    }
}
