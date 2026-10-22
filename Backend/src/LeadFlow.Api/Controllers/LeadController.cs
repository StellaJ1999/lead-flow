using Microsoft.AspNetCore.Mvc;

namespace LeadFlow.Api.Controllers
{
    public class LeadController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
