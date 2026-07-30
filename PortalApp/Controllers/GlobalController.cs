using Microsoft.AspNetCore.Mvc;

namespace PortalApp.Controllers
{
    [Route("globale")]
    public class GlobalController : Controller
    {
        [HttpGet("")]
        public IActionResult Index()
        {
            return RedirectToActionPermanent("Show", "Sections", new { slug = "globale" });
        }
    }
}
