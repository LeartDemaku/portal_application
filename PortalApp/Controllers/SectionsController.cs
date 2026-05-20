using Microsoft.AspNetCore.Mvc;
using PortalApp.Services;

namespace PortalApp.Controllers
{
    [Route("kategori")]
    public class SectionsController : Controller
    {
        private readonly IPortalQueryService _portalQueryService;

        public SectionsController(IPortalQueryService portalQueryService)
        {
            _portalQueryService = portalQueryService;
        }

        [HttpGet("{slug}")]
        public async Task<IActionResult> Show(string slug)
        {
            var page = await _portalQueryService.GetSectionPageAsync(slug);

            if (page == null)
            {
                return RedirectToAction("Index", "Home");
            }

            return View(page);
        }
    }
}
