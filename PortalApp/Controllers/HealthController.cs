using Microsoft.AspNetCore.Mvc;

namespace PortalApp.Controllers
{
    [Route("shendetesi")]
    public class HealthController : Controller
    {
        [HttpGet("")]
        public IActionResult Index()
        {
            return RedirectToActionPermanent("Show", "Sections", new { slug = "shendetesi" });
        }
    }
}
