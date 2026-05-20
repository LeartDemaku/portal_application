using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PortalApp.Models
{
    public class Article
    {
        public Article() 
        {
            ArticleTags = new List<ArticleTags>();
            ArticleComments = new List<ArticleComment>();
        }

        [Key]
        public int Id { get; set; }

        [Required]
        public string Title { get; set; } = string.Empty;
        public string? CoverImage { get; set; }

        [Required]
        public string Content { get; set; } = string.Empty;

        public int ArticleCategoryId { get; set; }
        [ForeignKey("ArticleCategoryId")]
        public ArticleCategory ArticleCategory { get; set; } = null!;

        [Required]
        public DateTime CreatedAt { get; set; }

        public DateTime? UpdatedAt { get; set; }

        public string? CreatedBy { get; set; }
        [ForeignKey("CreatedBy")]
        public ApplicationUser? CreatedByUser { get; set; }

        public string? ApprovedBy { get; set; }
        [ForeignKey("ApprovedBy")]
        public ApplicationUser? ApprovedByUser { get; set; }
        public List<ArticleTags> ArticleTags { get; set; }
        public List<ArticleComment> ArticleComments { get; set; }
    }
}
