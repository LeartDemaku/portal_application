using Microsoft.AspNetCore.Identity;

namespace PortalApp.Models
{
    public class ApplicationUser : IdentityUser
    {
        public ApplicationUser()
        {
            Articles = new List<Article>();
        }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }

        public List<Article> Articles { get; set; }

    }
}
