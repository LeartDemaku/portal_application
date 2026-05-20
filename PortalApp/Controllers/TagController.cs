using Microsoft.AspNetCore.Mvc;
using PortalApp.Services;

namespace PortalApp.Controllers
{
    [Route("tag")]
    public class TagController : Controller
    {
        private readonly IPortalQueryService _portalQueryService;

        public TagController(IPortalQueryService portalQueryService)
        {
            _portalQueryService = portalQueryService;
        }

        [HttpGet("{name}")]
        public async Task<IActionResult> Show(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                return RedirectToAction("Index", "Home");
            }

            return View(await _portalQueryService.GetTagPageAsync(name));
        }
    }
}
