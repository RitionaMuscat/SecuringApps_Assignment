using IdentityModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Identity.UI.V3.Pages.Account.Internal;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using SecuringApps.Presentation.Models;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;

namespace SecuringApps.Presentation.Controllers
{
    public class StudentRegisterController : Controller
    {

        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILogger<RegisterModel> _logger;
        private readonly IEmailSender _emailSender;
        private readonly RoleManager<IdentityRole> _roleManager;

        public StudentRegisterController
        (
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            ILogger<RegisterModel> logger,
            IEmailSender emailSender,
            RoleManager<IdentityRole> roleManager
        )
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _logger = logger;
            _emailSender = emailSender;
            _roleManager = roleManager;
        }

        [Authorize(Policy = "writepolicy")]
        public IActionResult Create()
        {
            ViewData["roles"] = _roleManager.Roles.ToList();
            return View();
        }
        [BindProperty]
        public InputModel Input { get; set; }
        public string ReturnUrl { get; set; }
        public class InputModel
        {
            [Required]
            [EmailAddress]
            [Display(Name = "Email")]
            public string Email { get; set; }

            public string Name { get; set; }
        }

        public static string GenerateRandomPassword(PasswordOptions opts = null)
        {
            if (opts == null) opts = new PasswordOptions()
            {
                RequiredLength = 8,
                RequiredUniqueChars = 4,
                RequireDigit = true,
                RequireLowercase = true,
                RequireNonAlphanumeric = true,
                RequireUppercase = true
            };

            string[] randomChars = new[] {
                                            "ABCDEFGHJKLMNOPQRSTUVWXYZ",    // uppercase 
                                            "abcdefghijkmnopqrstuvwxyz",    // lowercase
                                            "0123456789",                   // digits
                                            "!@$?_-"                        // non-alphanumeric
            };
            CryptoRandom rand = new CryptoRandom();
            List<char> chars = new List<char>();

            if (opts.RequireUppercase)
                chars.Insert(rand.Next(0, chars.Count),
                    randomChars[0][rand.Next(0, randomChars[0].Length)]);

            if (opts.RequireLowercase)
                chars.Insert(rand.Next(0, chars.Count),
                    randomChars[1][rand.Next(0, randomChars[1].Length)]);

            if (opts.RequireDigit)
                chars.Insert(rand.Next(0, chars.Count),
                    randomChars[2][rand.Next(0, randomChars[2].Length)]);

            if (opts.RequireNonAlphanumeric)
                chars.Insert(rand.Next(0, chars.Count),
                    randomChars[3][rand.Next(0, randomChars[3].Length)]);

            for (int i = chars.Count; i < opts.RequiredLength
                || chars.Distinct().Count() < opts.RequiredUniqueChars; i++)
            {
                string rcs = randomChars[rand.Next(0, randomChars.Length)];
                chars.Insert(rand.Next(0, chars.Count),
                    rcs[rand.Next(0, rcs.Length)]);
            }

            return new string(chars.ToArray());
        }

        [HttpPost]
        public async Task<IActionResult> Create(ApplicationUser user)
        {
            ViewData["roles"] = _roleManager.Roles.ToList();
            if (ModelState.IsValid)
            {
                var role = _roleManager.FindByIdAsync(Input.Name).Result;
                if (role.Name == "Student")
                {
                    string randomPassword = GenerateRandomPassword();
                    var newUser = new ApplicationUser { UserName = Input.Email, Email = Input.Email, isStudent = true };

                    var result = await _userManager.CreateAsync(newUser, randomPassword);

                    await _userManager.AddToRoleAsync(newUser, role.Name);
                    if (result.Succeeded)
                    {
                        SendEmail(newUser.UserName, newUser.Email, "Your login credentials are: \n Username: " + newUser.Email + " \n Password: " + randomPassword);
                    }
                    else
                    {
                        foreach (var error in result.Errors)
                        {
                            ModelState.AddModelError(string.Empty, error.Description);
                        }
                    }
                }
            }

            return View();
        }
        public string SendEmail(string Name, string Email, string Message)
        {


 
            var toAddress = new MailAddress(Email);

            string subject = "Login Credential For Portal";
            string body = Message;
            try
            {
                var builder = new ConfigurationBuilder().AddJsonFile("appsettings.json");
                var config = builder.Build();
                var fromAddress = new MailAddress(config["Smtp:Username"]);
                var smtp = new SmtpClient(config["Smtp:Host"])
                {
                    Port = int.Parse(config["Smtp:Port"]),
                    Credentials = new NetworkCredential(config["Smtp:Username"], config["Smtp:Password"]),
                    EnableSsl = true,
                };

                using (var message = new MailMessage(fromAddress, toAddress)
                {
                    Subject = subject,
                    Body = body
                })

                    smtp.Send(message);
                return "Email Sent Successfully";
            }
            catch (System.Exception e)
            {
                return e.Message;
            }


        }
    }
}


