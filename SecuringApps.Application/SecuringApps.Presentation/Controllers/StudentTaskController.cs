using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SecuringApps.Application.Interfaces;
using SecuringApps.Application.Services;
using SecuringApps.Application.ViewModels;
using SecuringApps.Data.Context;
using SecuringApps.Domain.Models;
using SecuringApps.Presentation.Data;
using SecuringApps.Presentation.Models;
using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using static Microsoft.AspNetCore.Identity.UI.V3.Pages.Account.Internal.ExternalLoginModel;

namespace SecuringApps.Presentation.Controllers
{
    public class InputModel
    {
        [Required]
        public string Name { get; set; }
        public string Description { get; set; }
        public string FilePath { get; set; }
        public DateTime Deadline { get; set; }
    }

    public class StudentTaskController : Controller
    {
        private readonly RoleManager<IdentityRole> _roleManager;
        private IStudentTaskService _IStudentTaskService;
        private IWebHostEnvironment _env;
        public StudentTaskController
        (
            RoleManager<IdentityRole> roleManager,
            IStudentTaskService studentTaskService,
             IWebHostEnvironment env

        )
        {
            _IStudentTaskService = studentTaskService;
            _roleManager = roleManager;
            _env = env;
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
            try
            {
                if (file != null)
                {
                    if (file.Length > 0)
                    {
                        string newFilename = Guid.NewGuid() + System.IO.Path.GetExtension(file.FileName);

                        string absolutePath = _env.WebRootPath + @"\Files\";

                        using (var stream = System.IO.File.Create(absolutePath + newFilename))
                        {
                            file.CopyTo(stream);
                        }
                        createStudentTask.StudentTask.FilePath = @"\Files\" + newFilename; //relative Path
                    }
                }
                if (ModelState.IsValid)
                {
                    _IStudentTaskService.AddStudentTask(createStudentTask.StudentTask);
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
