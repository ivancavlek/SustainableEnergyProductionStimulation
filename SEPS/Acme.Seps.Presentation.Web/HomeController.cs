using Microsoft.AspNetCore.Mvc;

namespace Acme.Seps.Presentation.Web
{
    public class HomeController : Controller
    {
        // GET: /<controller>/
        public IActionResult Index()
        {
            return View();
        }
    }
}
