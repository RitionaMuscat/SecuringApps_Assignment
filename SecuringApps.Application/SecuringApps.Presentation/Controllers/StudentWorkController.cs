using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SecuringApps.Application.Interfaces;
using SecuringApps.Presentation.Models;
using SecuringApps.Presentation.Utilities;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography;

namespace SecuringApps.Presentation.Controllers
{
    public class StudentWorkController : Controller
    {
        private IStudentWorkService _studentWorkService;
        private IStudentTaskService _studentTaskService;
        private IWebHostEnvironment _env;
        private readonly RoleManager<IdentityRole> _roleManager;
        private UserManager<ApplicationUser> _userManager;
        private ILogger<StudentWorkController> _logger;
        Encryption encryption = new Encryption();

        public StudentWorkController(IStudentWorkService studentWorkService,
                                            IStudentTaskService studentTaskService,
                                            IWebHostEnvironment env,
                                            UserManager<ApplicationUser> userManager,
                                            RoleManager<IdentityRole> roleManager,
                                            ILogger<StudentWorkController> logger)
        {
            _studentWorkService = studentWorkService;
            _studentTaskService = studentTaskService;
            _env = env;
            _userManager = userManager;
            _roleManager = roleManager;
            _logger = logger;
        }

        [Authorize(Roles = "Student")]
        [HttpGet]
        public IActionResult Index(Guid Id)
        {
            _logger.LogInformation("Student accessed its work");
            var _studentWork = _studentWorkService.GetStudentWork();
            var getStudentWork = _studentWork.Where(x => x.workOwner == _userManager.GetUserId(User) || x.workOwner == _userManager.GetUserName(User));
            return View(getStudentWork.ToList());
        }
        [Authorize(Roles = "Teacher")]
        [HttpGet]
        public IActionResult AllStudentWork()
        {
            _logger.LogInformation("Teacher accessed its work");

            var allStudent = _studentWorkService.GetStudentWork();
            return View(allStudent);
        }



        [Authorize(Roles = "Student")]
        [HttpGet]
        public IActionResult Create(string Id)
        {
            _logger.LogInformation("Student is creating work");
            Id = Id.Replace("|", "/").Replace("_", "+").Replace("$", "=");
            string output = Encryption.SymmetricDecrypt(Id);

            Guid taskId = new Guid(output);

            var taskList = _studentTaskService.GetStudentTask();
            var value = taskList.Where(x => x.Id == taskId);

            CreateStudentWorkModel model = new CreateStudentWorkModel();
            model.StudentTasks = value.ToList();

            return View(model);
        }

        [HttpPost]
        [Authorize(Roles = "Student")]
        public IActionResult Create(CreateStudentWorkModel data, IFormFile file)
        {
            CreateStudentWorkModel model = new CreateStudentWorkModel();
            model.StudentTasks = _studentTaskService.GetStudentTask().ToList();
            var value = model.StudentTasks.Where(x => x.Id == data.StudentWork.StudentTask.Id);

            try
            {
                _logger.LogInformation("Creating work..");
                if (file != null)
                {
                    if (System.IO.Path.GetExtension(file.FileName) == ".pdf")
                    {
                        _logger.LogInformation("Checking file extension");
                        data.StudentWork.submittedOn = DateTime.Now;

                        var StudentTask = _studentTaskService.GetStudentTask().ToList();
                        var val = from a in StudentTask
                                  where a.Id.Equals(data.StudentWork.StudentTask.Id)
                                  select a;

                        var StudentWork = _studentWorkService.GetStudentWork().ToList();
                        var vals = from v in StudentWork
                                   where v.StudentTask.Id.Equals(data.StudentWork.StudentTask.Id) && v.workOwner.Equals(_userManager.GetUserName(User))
                                   select v;
                        foreach (var item in val)
                        {
                            _logger.LogInformation("Task exists, starting validation");
                            if (DateTime.Now > item.Deadline)
                            {
                                _logger.LogInformation("Submission date expired");
                                ModelState.AddModelError(string.Empty, "Submission date expired.");
                                break;
                            }
                            if (vals.Any())
                            {
                                _logger.LogInformation("Work Submitted");
                                ModelState.AddModelError(string.Empty, "Your work is already submitted");
                                break;
                            }
                            else
                            {
                                if (file.Length > 0)
                                {
                                    _logger.LogInformation("Work Submitted");
                                    string newFilename = Guid.NewGuid() + Path.GetExtension(file.FileName);

                                    string absolutePath = _env.ContentRootPath + @"\Files\";

                                    string getFullFilePath = Path.GetFullPath(file.FileName);
                                    var Key = EncyrptFiles.GenerateNewKeyPair();
                                    var privateKey = Key.PrivateKey;
                                    using (var stream = System.IO.File.Create(absolutePath + newFilename))
                                    {
                                        string signature = EncyrptFiles.DigitallySign(stream, Key.PrivateKey);
                                        data.StudentWork.signature = signature;

                                        file.CopyTo(stream);
                                        _logger.LogInformation("File copied");

                                        bool result = EncyrptFiles.DigitallyVerify(stream, signature, Key.PublicKey);
                                        data.StudentWork.isDigitallySigned = result;
                                    }

                                  //  data.StudentWork.filePath = @"\Files\" + newFilename; //relative Path
                                 
                                    string fileNameNew = EncyrptFiles.FileEncrypt(absolutePath + newFilename, "PWd123!");
                                     data.StudentWork.filePath = fileNameNew;
                                    data.StudentWork.workOwner = _userManager.GetUserName(User);

                                    _studentWorkService.AddStudentWork(data.StudentWork);
                                    break;
                                }
                            }
                        }
                    }
                    else
                    {
                        _logger.LogError("File Extension allowed is PDF");
                        ModelState.AddModelError(string.Empty, "File Extension allowed is PDF");
                    }

                }
                else
                {
                    _logger.LogError("Document not attacghed.");
                    ModelState.AddModelError(string.Empty, "Please attach document");
                }
            }
            catch (Exception ex)
            {

                var error = ex.Message;

                _logger.LogError("Error: " + error);
                error = ex.InnerException.ToString();

                //log errors
                ViewData["warning"] = "Your work was not added. Check your details";

                //i want to redirect the user to a common page (when there is an error)
                TempData["error"] = "this is a test error";
                return RedirectToAction("Error", "Home");
            }
            return View(model);
        }

        public IActionResult DownloadFile(String id)
        {
            id = id.Replace("|", "/").Replace("_", "+").Replace("$", "=");
            string output = Encryption.SymmetricDecrypt(id);

            Guid fileId = new Guid(output);

            var _files = _studentWorkService.GetStudentWork();

            var file = _files.Where(x => x.Id == fileId).FirstOrDefault();
            var filename = file.filePath.Substring(7, 15);

            var url = new Uri(_env.ContentRootPath + file.filePath);

            var Key = EncyrptFiles.GenerateNewKeyPair();
            var privateKey = Key.PrivateKey;

            EncyrptFiles.FileDecrypt(url.LocalPath, filename,"PWd123!");

            WebClient webClient = new WebClient();

            webClient.DownloadFileCompleted += (sender, e) =>
            {
                if (e.Error == null & !e.Cancelled)
                {
                    Debug.WriteLine(@"Download completed!");
                    _logger.LogInformation("Download completed!");
                }
            };

            try
            {

                webClient.OpenRead(url);

                Debug.WriteLine(filename);

                //var path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), filename);
                //webClient.DownloadFileAsync(url, path);
                _logger.LogInformation("Download completed!");
            }
            catch (Exception ex)
            {
                var err = ex.Message;
                _logger.LogError("Error: " + err);
            }

            return View();

        }
        private void CompareFileHashes(string fileName1)
        {
            var getAllFiles = _studentWorkService.GetStudentWork();
            var getFileName = from a in getAllFiles
                              select a;

            SHA256 sha = SHA256.Create("SHA256");
            HashAlgorithm hash = HashAlgorithm.Create(sha.ToString());

            byte[] fileHash1;
            byte[] fileHash2;
            fileName1 = @"\Files\" + fileName1;
            var fileName2 = "";
            if (getAllFiles.Any())
            {
                _logger.LogInformation("Found Files, Comparing hashes ");
                foreach (var item in getFileName)
                {
                    fileName2 = _env.WebRootPath + item.filePath;
                    using (FileStream fileStream1 = new FileStream(fileName1, FileMode.Open),
                      fileStream2 = new FileStream(fileName2, FileMode.Open))
                    {
                        // Compute file hashes
                        fileHash1 = hash.ComputeHash(fileStream1);
                        fileHash2 = hash.ComputeHash(fileStream2);
                    }
                    if (BitConverter.ToString(fileHash1) == BitConverter.ToString(fileHash2))
                        ModelState.AddModelError(string.Empty, "File is the same");
                }

            }

        }
    }
}
