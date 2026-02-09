using Microsoft.AspNetCore.Mvc;

namespace NetCore.web.Controllers
{
    public class MembershipController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
