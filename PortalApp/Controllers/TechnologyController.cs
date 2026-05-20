using Microsoft.AspNetCore.Mvc;

namespace PortalApp.Controllers
{
    [Route("teknologji")]
    public class TechnologyController : Controller
    {
        [HttpGet("")]
        public IActionResult Index()
        {
            return RedirectToActionPermanent("Show", "Sections", new { slug = "teknologji" });
        }
    }
}
