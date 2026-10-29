using System.ComponentModel.DataAnnotations;

namespace PortalApp.Models.ViewModels
{
    public class ArticleCommentStoreViewModel
    {

        [Required(ErrorMessage = "Shkruani email-in.")]
        [EmailAddress(ErrorMessage = "Email-i nuk është i vlefshëm.")]
        [StringLength(200, ErrorMessage = "Email-i mund të ketë deri në 200 shkronja.")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Shkruani emrin.")]
        [StringLength(100, ErrorMessage = "Emri mund të ketë deri në 100 shkronja.")]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "Shkruani komentin.")]
        [StringLength(2000, MinimumLength = 2, ErrorMessage = "Komenti duhet të ketë 2 deri në 2000 shkronja.")]
        public string Comment { get; set; } = string.Empty;

    }
}
