using Microsoft.AspNetCore.Mvc;

namespace Acme.Seps.Presentation.Web.Controllers
{
    public sealed class HomeController : Controller
    {
        // GET: /<controller>/
        public IActionResult Index()
        {
            return View();
        }
    }
}
