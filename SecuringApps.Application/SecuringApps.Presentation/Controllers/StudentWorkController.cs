using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SecuringApps.Application.Interfaces;
using SecuringApps.Presentation.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SecuringApps.Presentation.Controllers
{
    public class StudentWorkController : Controller
    {
        private IStudentWorkService _studentWorkService;
        private IStudentTaskService _studentTaskService;
        private IWebHostEnvironment _env;
        private UserManager<ApplicationUser> _userManager;
        public StudentWorkController(IStudentWorkService studentWorkService,
                                            IStudentTaskService studentTaskService,
                                            IWebHostEnvironment env,
                                            UserManager<ApplicationUser> userManager)
        {
            _studentWorkService = studentWorkService;
            _studentTaskService = studentTaskService;
            _env = env;
            _userManager = userManager;
        }
        //public IActionResult Index()
        //{
        //    return View();
        //}
        [Authorize(Policy = "readpolicy")]
        [HttpGet]
        public IActionResult Create(Guid Id)
        {
            var taskList = _studentTaskService.GetStudentTask();
            var value = from i in taskList
                        where i.Id.Equals(Id)
                        select i;

            CreateStudentWorkModel model = new CreateStudentWorkModel();
            model.StudentTasks = value.ToList();

            return View(model);
        }

        [HttpPost]
        public IActionResult Create(CreateStudentWorkModel data, IFormFile file)
        {
            CreateStudentWorkModel model = new CreateStudentWorkModel();
            model.StudentTasks = _studentTaskService.GetStudentTask().ToList();
            var value = from a in model.StudentTasks
                        where a.Id.Equals(data.StudentWork.StudentTask.Id)
                        select a;

            try
            {
                if (file != null)
                {
                    if (System.IO.Path.GetExtension(file.FileName) == ".pdf")
                    {
                        data.StudentWork.submittedOn = DateTime.Now;
                        foreach (var item in value)
                        {
                            var StudentWork = _studentWorkService.GetStudentWork().ToList();
                            var vals = from v in StudentWork
                                       where v.StudentTask.Id.Equals(data.StudentWork.StudentTask.Id) && v.workOwner.Equals(_userManager.GetUserId(User))
                                       select v;
                            if (DateTime.Now > item.Deadline)
                            {
                                ModelState.AddModelError(string.Empty, "Submission date expired.");
                                break;
                            }
                            if (vals.Any())
                            {
                             
                                    ModelState.AddModelError(string.Empty, "Your work is already submitted");
                                    break;
                                
                            }

                            else
                            {
                                if (file.Length > 0)
                                {
                                    string newFilename = Guid.NewGuid() + System.IO.Path.GetExtension(file.FileName);

                                    string absolutePath = _env.WebRootPath + @"\Files\";

                                    using (var stream = System.IO.File.Create(absolutePath + newFilename))
                                    {
                                        file.CopyTo(stream);
                                    }
                                    data.StudentWork.filePath = @"\Files\" + newFilename; //relative Path
                                    data.StudentWork.workOwner = _userManager.GetUserId(User);

                                    _studentWorkService.AddStudentWork(data.StudentWork);
                                    break;
                                }
                            }
                        }
                    }
                    else
                    {
                        ModelState.AddModelError(string.Empty, "File Extension allowed is PDF");
                    }
                 
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Please attach document");
                }
            }
            catch (Exception ex)
            {
                var error = ex.Message;
                error = ex.InnerException.ToString();
                //log errors
                ViewData["warning"] = "Your work was not added. Check your details";

                //i want to redirect the user to a common page (when there is an error)
                TempData["error"] = "this is a test error";
                return RedirectToAction("Error", "Home");
            }
            return View(model);
        }
    }
}

