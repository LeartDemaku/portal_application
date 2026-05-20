using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PortalApp.Models;
using System.ComponentModel.DataAnnotations;

namespace PortalApp.Models.ViewModels
{
    public class ArticleEditViewModel
    {
        public ArticleEditViewModel()
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

        public List<string> ArticleTags { get; set; }
        public List<ArticleCategory> ArticleCategories { get; set; }

        public string CreatedBy { get; set; } = string.Empty;

        public bool IsApproved { get; set; }
        public string? CurrentCoverImage { get; set; }
    }
}
