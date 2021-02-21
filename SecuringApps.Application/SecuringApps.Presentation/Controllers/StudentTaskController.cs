using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SecuringApps.Application.Interfaces;
using SecuringApps.Presentation.Models;
using System;
using System.Linq;
namespace SecuringApps.Presentation.Controllers
{
    public class StudentTaskController : Controller
    {
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly UserManager<ApplicationUser> _userManager;

        private IStudentTaskService _IStudentTaskService;
        private IWebHostEnvironment _env;
        public StudentTaskController
        (
            RoleManager<IdentityRole> roleManager,
                UserManager<ApplicationUser> userManager,
            IStudentTaskService studentTaskService,
            IWebHostEnvironment env
        )
        {
            _IStudentTaskService = studentTaskService;
            _userManager = userManager;
            _roleManager = roleManager;
            _env = env;
        }
        [HttpGet]
        public IActionResult Index()
        {
            var list = _IStudentTaskService.GetStudentTask();
   
            var getUser = _userManager.Users.Where(x => x.Id == _userManager.GetUserId(User)).ToList();

            var id = getUser.Select(x => x.createdBy);

            var getTasks = from l in list
                           where id.Contains(l.DocumentOwner)
                           select l;
         
            return View(getTasks);

 }

        [Authorize(Roles = "Teacher")]
        public IActionResult Create()
        {
            CreateStudentTaskModel model = new CreateStudentTaskModel();
            return View();
        }

        [HttpPost]
        public IActionResult Create(CreateStudentTaskModel createStudentTask)
        {
            try
            {
                if (createStudentTask.StudentTask.Deadline <= DateTime.Now)
                {
                    ModelState.AddModelError(string.Empty, "Deadline should be greater than today's date");
                }
                else
                {
                    createStudentTask.StudentTask.DocumentOwner = _userManager.GetUserId(User);
                    if (ModelState.IsValid)
                    {
                        _IStudentTaskService.AddStudentTask(createStudentTask.StudentTask);
                    }
                }

            }
            catch (Exception ex)
            {
                var exception = ex.Message;
                ModelState.AddModelError(string.Empty, exception);
                return RedirectToAction("Error", "Home");
            }

            CreateStudentTaskModel model = new CreateStudentTaskModel();
            return View();
        }


    }
}
