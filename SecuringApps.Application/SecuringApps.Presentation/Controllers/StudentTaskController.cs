using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace SecuringApps.Presentation.Controllers
{
    public class StudentTaskController : Controller
    {
        [Authorize(Policy = "writepolicy")]
        public IActionResult Create()
        {
            return View();
        }
    }
}
