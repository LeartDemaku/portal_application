using Microsoft.AspNetCore.Mvc;

namespace PortalApp.Controllers
{
    [Route("politike")]
    public class PoliticsController : Controller
    {
        [HttpGet("")]
        public IActionResult Index()
        {
            return RedirectToActionPermanent("Show", "Sections", new { slug = "politike" });
        }
    }
}
