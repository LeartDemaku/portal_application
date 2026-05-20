using System.ComponentModel.DataAnnotations;

namespace PortalApp.Models
{

    public class ArticleCategory
    {
        public ArticleCategory()
        {
            Articles = new List<Article>();
        }
        [Key]
        public int Id { get; set; }

        [Required]
        public string Name { get; set; } = string.Empty;

        [Required]
        public DateTime CreatedAt { get; set; }

        public DateTime? UpdatedAt { get; set; }

        public List<Article> Articles { get; set; }
    }
}
