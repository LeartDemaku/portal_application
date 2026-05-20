using Microsoft.AspNetCore.Mvc;

namespace PortalApp.Controllers
{
    [Route("ekonomi")]
    public class EconomyController : Controller
    {
        [HttpGet("")]
        public IActionResult Index()
        {
            return RedirectToActionPermanent("Show", "Sections", new { slug = "ekonomi" });
        }
    }
}
