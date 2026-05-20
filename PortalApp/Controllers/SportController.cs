using Microsoft.AspNetCore.Mvc;

namespace PortalApp.Controllers
{
    [Route("sport")]
    public class SportController : Controller
    {
        [HttpGet("")]
        public IActionResult Index()
        {
            return RedirectToActionPermanent("Show", "Sections", new { slug = "sport" });
        }
    }
}
