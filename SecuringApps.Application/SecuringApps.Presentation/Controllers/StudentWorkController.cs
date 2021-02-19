using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SecuringApps.Application.Interfaces;
using SecuringApps.Presentation.Models;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;

namespace SecuringApps.Presentation.Controllers
{
    public class StudentWorkController : Controller
    {
        private IStudentWorkService _studentWorkService;
        private IStudentTaskService _studentTaskService;
        private IWebHostEnvironment _env;
        private readonly RoleManager<IdentityRole> _roleManager;
        private UserManager<ApplicationUser> _userManager;
        public StudentWorkController(IStudentWorkService studentWorkService,
                                            IStudentTaskService studentTaskService,
                                            IWebHostEnvironment env,
                                            UserManager<ApplicationUser> userManager,
                                            RoleManager<IdentityRole> roleManager)
        {
            _studentWorkService = studentWorkService;
            _studentTaskService = studentTaskService;
            _env = env;
            _userManager = userManager;
            _roleManager = roleManager;
        }
        [Authorize(Roles = "Student")]
        [HttpGet]
        public IActionResult Index(Guid Id)
        {
            var _studentWork = _studentWorkService.GetStudentWork();
            var getStudentWork = _studentWork.Where(x => x.workOwner == _userManager.GetUserId(User) || x.workOwner == _userManager.GetUserName(User));
            return View(getStudentWork.ToList());
        }
        [Authorize(Roles = "Teacher")]
        [HttpGet]
        public IActionResult AllStudentWork()
        {
            var allStudent = _studentWorkService.GetStudentWork();
            return View(allStudent.ToList());
        }



        [Authorize(Roles = "Student")]
        [HttpGet]
        public IActionResult Create(Guid Id)
        {
            var taskList = _studentTaskService.GetStudentTask();
            var value = taskList.Where(x => x.Id == Id);

            CreateStudentWorkModel model = new CreateStudentWorkModel();
            model.StudentTasks = value.ToList();

            return View(model);
        }

        [HttpPost]
        public IActionResult Create(CreateStudentWorkModel data, IFormFile file)
        {
            CreateStudentWorkModel model = new CreateStudentWorkModel();
            model.StudentTasks = _studentTaskService.GetStudentTask().ToList();
            var value = model.StudentTasks.Where(x => x.Id == data.StudentWork.StudentTask.Id);

            try
            {
                if (file != null)
                {
                    if (System.IO.Path.GetExtension(file.FileName) == ".pdf")
                    {
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
                                    string newFilename = Guid.NewGuid() + Path.GetExtension(file.FileName);

                                    string absolutePath = _env.WebRootPath + @"\Files\";

                                    string getFullFilePath = Path.GetFullPath(file.FileName);
                                    using (var stream = System.IO.File.Create(absolutePath + newFilename))
                                    {
                                        file.CopyTo(stream);
                                        CreateEncryptor(stream, "HR$2pIjHR$2pIj12");
                                    }

                                    CompareFileHashes(newFilename);
                                    string encryptedfilename = Guid.NewGuid() + "ENCRYPT" + Path.GetExtension(file.FileName);
                                    //CreateEncryptor(newFilename, encryptedfilename, "HR$2pIjHR$2pIj12");
                                    data.StudentWork.filePath = @"\Files\" + newFilename; //relative Path
                                    data.StudentWork.workOwner = _userManager.GetUserName(User);

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

        public IActionResult DownloadFile(Guid id)
        {
            var _files = _studentWorkService.GetStudentWork();

            var file = _files.Where(x => x.Id == id).FirstOrDefault();
            var filename = file.filePath.Substring(7);

            WebClient webClient = new WebClient();

            webClient.DownloadFileCompleted += (sender, e) =>
            {
                if (e.Error == null & !e.Cancelled)
                {
                    Debug.WriteLine(@"Download completed!");
                }
            };

            var url = new Uri(_env.WebRootPath + file.filePath);

            try
            {
                webClient.OpenRead(url);

                Debug.WriteLine(filename);

                var path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), filename);
                webClient.DownloadFileAsync(url, path);
            }
            catch (Exception ex)
            {
                var err = ex.Message;
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
            fileName1 = _env.WebRootPath + @"\Files\" + fileName1;
            var fileName2 = "";
            if (getAllFiles.Any())
            {

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
        public static void CreateEncryptor(Stream source, string password)
        {
            ICryptoTransform transform;
      
            try
            {
                byte[] SaltBytes = new byte[16];
                RandomNumberGenerator.Fill(SaltBytes); //RandomNumberGenerator is used for .Net Core 3

                AesManaged aes = new AesManaged();
                aes.BlockSize = aes.LegalBlockSizes[0].MaxSize;
                aes.KeySize = aes.LegalKeySizes[0].MaxSize;

                Rfc2898DeriveBytes key = new Rfc2898DeriveBytes(password, SaltBytes, iterations);
                aes.Key = key.GetBytes(aes.KeySize / 8);

                byte[] IVBytes = new byte[aes.BlockSize / 8];
                RandomNumberGenerator.Fill(IVBytes); //RandomNumberGenerator is used for .Net Core 3
                aes.IV = IVBytes;

                aes.Mode = CipherMode.CBC;
                 transform = aes.CreateEncryptor(aes.Key, aes.IV);

                //Store/Send the Salt and IV - this can be shared. It's more important that it's very random, than being private.
                source.WriteByte((byte)SaltBytes.Length);
                source.Write(SaltBytes, 0, SaltBytes.Length);
                source.WriteByte((byte)IVBytes.Length);
                source.Write(IVBytes, 0, IVBytes.Length);
                var cryptoStream = new CryptoStream(source, transform, CryptoStreamMode.Write);

            }
            catch (Exception ex)
            {
                var e = ex.Message;
            }
            source.Flush();

            //return cryptoStream;
        }

        public static Stream CreateDecryptor(Stream source, string password)
        {
            var ArrayLength = source.ReadByte();
            if (ArrayLength == -1) throw new Exception("Salt length not found");
            byte[] SaltBytes = new byte[ArrayLength];
            var readBytes = source.Read(SaltBytes, 0, ArrayLength);
            if (readBytes != ArrayLength) throw new Exception("No support for multiple reads");

            ArrayLength = source.ReadByte();
            if (ArrayLength == -1) throw new Exception("Salt length not found");
            byte[] IVBytes = new byte[ArrayLength];
            readBytes = source.Read(IVBytes, 0, ArrayLength);
            if (readBytes != ArrayLength) throw new Exception("No support for multiple reads");

            AesManaged aes = new AesManaged();
            aes.BlockSize = aes.LegalBlockSizes[0].MaxSize;
            aes.KeySize = aes.LegalKeySizes[0].MaxSize;
            aes.IV = IVBytes;

            Rfc2898DeriveBytes key = new Rfc2898DeriveBytes(password, SaltBytes, iterations);
            aes.Key = key.GetBytes(aes.KeySize / 8);

            aes.Mode = CipherMode.CBC;
            ICryptoTransform transform = aes.CreateDecryptor(aes.Key, aes.IV);

            var cryptoStream = new CryptoStream(source, transform, CryptoStreamMode.Read);
            return cryptoStream;
        }

        public const int iterations = 1042; // Recommendation is >= 1000.
    }
    /* private  void EncryptFile(string inputFile, string outputFile, string skey)
     {
         try
         {
             using (RijndaelManaged aes = new RijndaelManaged())
             {

                 byte[] key = ASCIIEncoding.UTF8.GetBytes(skey);
                 int keySize = aes.KeySize;

                 aes.BlockSize = 128;
                  byte[] IV = ASCIIEncoding.UTF8.GetBytes(skey);
                 var IVL = IV.Length;

                 inputFile = _env.WebRootPath + @"\Files\" + inputFile;
                 outputFile = _env.WebRootPath + @"\Files\" + outputFile;
                 using (FileStream fsCrypt = new FileStream(outputFile, FileMode.Create))
                 {
                     using (ICryptoTransform encryptor = aes.CreateEncryptor(key, IV))
                     {
                         using (CryptoStream cs = new CryptoStream(fsCrypt, encryptor, CryptoStreamMode.Write))
                         {
                             using (FileStream fsIn = new FileStream(inputFile, FileMode.Open))
                             {
                                 int data;
                                 while ((data = fsIn.ReadByte()) != -1)
                                 {
                                     cs.WriteByte((byte)data);
                                 }
                             }
                         }
                     }
                 }
             }
         }
         catch (Exception ex)
         {
             var e = ex.Message;
         }
     }*/

}
