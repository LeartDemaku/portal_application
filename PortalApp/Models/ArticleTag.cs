using System.ComponentModel.DataAnnotations;

namespace PortalApp.Models
{
    public class ArticleTag
    {
        public ArticleTag()
        {
            ArticleArticleTags = new List<ArticleTags>();
        }

        [Key]
        public int Id { get; set; }

        [Required]
        public string Title { get; set; } = string.Empty;

        [Required]
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public DateTime? UpdateAt { get; set; }

        public List<ArticleTags> ArticleArticleTags { get; set; } = new List<ArticleTags>();
    }
}