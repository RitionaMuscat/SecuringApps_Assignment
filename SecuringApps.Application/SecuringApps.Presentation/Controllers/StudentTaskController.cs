using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SecuringApps.Application.Interfaces;
using SecuringApps.Presentation.Models;
using System;

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
            return View(list);
        }

        [Authorize(Policy = "writepolicy")]
        public IActionResult Create()
        {
            CreateStudentTaskModel model = new CreateStudentTaskModel();
            return View();
        }
        [HttpPost]
        public IActionResult Create(CreateStudentTaskModel createStudentTask, IFormFile file)
        {
            var GetExtension = System.IO.Path.GetExtension(file.FileName);
            try
            {
                if (file != null)
                {
                    if (GetExtension != ".pdf")
                    {
                        ModelState.AddModelError(string.Empty, "The file path that can be uploaded should be pdf");
                    }
                    else if (createStudentTask.StudentTask.Deadline <= DateTime.Now)
                    {
                        ModelState.AddModelError(string.Empty, "Deadline should be greater than today's date");
                    }
                    else if (file.Length > 0)
                    {
                        string newFilename = Guid.NewGuid() + System.IO.Path.GetExtension(file.FileName);
                        string absolutePath = _env.WebRootPath + @"\Files\";

                        using (var stream = System.IO.File.Create(absolutePath + newFilename))
                        {
                            file.CopyTo(stream);
                        }
                        createStudentTask.StudentTask.DocumentOwner = _userManager.GetUserName(User);
                        createStudentTask.StudentTask.FilePath = @"\Files\" + newFilename; //relative Path
                        if (ModelState.IsValid)
                        {
                            _IStudentTaskService.AddStudentTask(createStudentTask.StudentTask);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                var exception = ex.Message;
                return RedirectToAction("Error", "Home");
            }

            CreateStudentTaskModel model = new CreateStudentTaskModel();
            return View();
        }
    }
}
