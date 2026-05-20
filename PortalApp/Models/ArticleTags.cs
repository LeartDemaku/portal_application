using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PortalApp.Models
{
    public class ArticleTags
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int ArticleId { get; set; }
        [ForeignKey("ArticleId")]
        public Article Article { get; set; } = null!;

        [Required]
        public int ArticleTagId { get; set; }
        [ForeignKey("ArticleTagId")]
        public ArticleTag ArticleTag { get; set; } = null!;

        [Required]
        public DateTime CreatedAt { get; set; }
    }
}
