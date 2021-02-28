using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SecuringApps.Application.Interfaces;
using SecuringApps.Presentation.Models;
using SecuringApps.Presentation.Utilities;
using System;
using System.Linq;

namespace SecuringApps.Presentation.Controllers
{
    public class CommentsController : Controller
    {
        private ICommentsService _commentService;
        private IStudentWorkService _studentWorkService;
        private IWebHostEnvironment _env;
        private readonly RoleManager<IdentityRole> _roleManager;
        private UserManager<ApplicationUser> _userManager;
        private ILogger<CommentsController> _logger;
        public CommentsController(IStudentWorkService studentWorkService,
                                            ICommentsService commentService,
                                            IWebHostEnvironment env,
                                            UserManager<ApplicationUser> userManager,
                                            RoleManager<IdentityRole> roleManager,
                                            ILogger<CommentsController> logger
                                            )
        {
            _studentWorkService = studentWorkService;
            _commentService = commentService;
            _env = env;
            _userManager = userManager;
            _roleManager = roleManager;
            _logger = logger;
        }
        [HttpGet]
        public IActionResult Index()
        {
            _logger.LogInformation("User just accessed the access comment page");
            var _comments = _commentService.GetComments();
            var _studentWork = _studentWorkService.GetStudentWork();
            var _work = from a in _comments
                        select a;

            return View(_work.ToList());

        }

        [HttpGet]
        public IActionResult Create(String Id)
        {
            _logger.LogInformation("User just accessed the access comment page");
            Id = Id.Replace("|", "/").Replace("_", "+").Replace("$", "=");
            string output = Encryption.SymmetricDecrypt(Id);

            Guid workId = new Guid(output);

            var workList = _studentWorkService.GetStudentWork();
            var value = from i in workList
                        where i.Id.Equals(workId)
                        select i;

            CreateCommentsModel model = new CreateCommentsModel();
            model.StudentWork = value.ToList();

            return View(model);
        }

        [HttpPost]
        public IActionResult Create(CreateCommentsModel data, IFormFile file)
        {

            CreateCommentsModel model = new CreateCommentsModel();
            model.StudentWork = _studentWorkService.GetStudentWork().ToList();
            var value = from a in model.StudentWork
                        where a.Id.Equals(data.Comments.StudentWork.Id)
                        select a;
            try
            {
                data.Comments.writtenBy = _userManager.GetUserId(User);
                data.Comments.writtenOn = DateTime.Now;
                _commentService.AddComments(data.Comments);
            }
            catch (Exception ex)
            {
                var error = ex.Message;
                error = ex.InnerException.ToString();
                //log errors
                ViewData["warning"] = "Your work was not added. Check your details";

                TempData["error"] = "this is a test error";
                return RedirectToAction("Error", "Home");
            }
            return View("~/Views/Home/Index.cshtml");
        }

        public IActionResult DetailsPartialView()
        {
            var Comments = _commentService.GetComments();
            var _comments = _commentService.GetComments();
            var getCommentsWork = from a in _comments
                                  select a;
            return View(getCommentsWork);
        }
    }
}