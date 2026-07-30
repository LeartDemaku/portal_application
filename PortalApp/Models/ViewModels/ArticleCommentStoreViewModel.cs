using System.ComponentModel.DataAnnotations;

namespace PortalApp.Models.ViewModels
{
    public class ArticleCommentStoreViewModel
    {

        [Required, EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required]
        public string Name { get; set; } = string.Empty;

        [Required]
        public string Comment { get; set; } = string.Empty;

    }
}
