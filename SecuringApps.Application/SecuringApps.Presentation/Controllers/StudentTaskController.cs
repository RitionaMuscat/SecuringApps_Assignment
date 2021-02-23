using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
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
        private ILogger<StudentTaskController> _logger;
        private IStudentTaskService _IStudentTaskService;
        private IWebHostEnvironment _env;
        public StudentTaskController
        (
            RoleManager<IdentityRole> roleManager,
            UserManager<ApplicationUser> userManager,
            ILogger<StudentTaskController> logger,
            IStudentTaskService studentTaskService,
            IWebHostEnvironment env
        )
        {
            _IStudentTaskService = studentTaskService;
            _userManager = userManager;
            _roleManager = roleManager;
            _logger = logger;
            _env = env;
        }
        [HttpGet]
        public IActionResult Index()
        {
            _logger.LogInformation("Accessing Tasks");
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
            _logger.LogInformation("Accessing the Create Task");

            CreateStudentTaskModel model = new CreateStudentTaskModel();
            return View();
        }

        [HttpPost]
        public IActionResult Create(CreateStudentTaskModel createStudentTask)
        {
            try
            {
                _logger.LogInformation("Create Task");

                if (createStudentTask.StudentTask.Deadline <= DateTime.Now)
                {
                    _logger.LogError("Deadline should be greater than today's date");

                    ModelState.AddModelError(string.Empty, "Deadline should be greater than today's date");
                }
                else
                {
                    createStudentTask.StudentTask.DocumentOwner = _userManager.GetUserId(User);
                    if (ModelState.IsValid)
                    {
                        _logger.LogInformation("Creating Task...");
                        _IStudentTaskService.AddStudentTask(createStudentTask.StudentTask);
                    }
                }

            }
            catch (Exception ex)
            {
                var exception = ex.Message;
                ModelState.AddModelError(string.Empty, exception);
                _logger.LogError("Error: " + exception);
                return RedirectToAction("Error", "Home");
            }

            CreateStudentTaskModel model = new CreateStudentTaskModel();
            return View();
        }


    }
}
