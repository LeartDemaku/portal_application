using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PortalApp.Models
{
    public class ArticleComment
    {
        [Key]
        public int Id { get; set; }

        [Required, EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required]
        public string Name { get; set; } = string.Empty;

        [Required]
        public string Comment { get; set; } = string.Empty;

        [Required]
        public int ArticleId { get; set; }

        [ForeignKey("ArticleId")]
        public Article Article { get; set; } = null!;

        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}
