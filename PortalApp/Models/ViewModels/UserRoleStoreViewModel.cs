using System.ComponentModel.DataAnnotations;

namespace PortalApp.Models.ViewModels
{
    public class UserRoleStoreViewModel
    {
        [Required]
        [StringLength(255, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 2)]
        public string RoleName { get; set; } = string.Empty;
    }
}
