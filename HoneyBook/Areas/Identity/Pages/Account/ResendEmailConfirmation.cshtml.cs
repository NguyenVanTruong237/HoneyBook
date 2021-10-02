using System;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Security.Claims;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using HoneyBook.DataAccess.Repository.IRepository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;

namespace HoneyBook.Areas.Identity.Pages.Account
{
    [AllowAnonymous]
    public class ResendEmailConfirmationModel : PageModel
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IEmailSender _emailSender;
        private readonly IWebHostEnvironment _hostEnvironment;
        private readonly IUnitOfWork _unitOfWork;

        public ResendEmailConfirmationModel(UserManager<IdentityUser> userManager, 
            IEmailSender emailSender,
            IWebHostEnvironment hostEnvironment,
            IUnitOfWork unitOfWork)
        {
            _userManager = userManager;
            _emailSender = emailSender;
            _hostEnvironment = hostEnvironment;
            _unitOfWork = unitOfWork;
        }

        [BindProperty]
        public InputModel Input { get; set; }

        public class InputModel
        {
            [Required]
            [EmailAddress]
            public string Email { get; set; }
        }

        public void OnGet()
        {
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            var user = await _userManager.FindByEmailAsync(Input.Email);
            if (user == null)
            {
                ModelState.AddModelError(string.Empty, "Verification email sent. Please check your email.");
                return Page();
            }

            var userId = await _userManager.GetUserIdAsync(user);
            var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
            var callbackUrl = Url.Page(
                "/Account/ConfirmEmail",
                pageHandler: null,
                values: new { userId = userId, code = code },
                protocol: Request.Scheme);

            var PathToFile = _hostEnvironment.WebRootPath + Path.DirectorySeparatorChar.ToString()
                        + "Templates" + Path.DirectorySeparatorChar.ToString() + "EmailTemplates"
                        + Path.DirectorySeparatorChar.ToString() + "Confirm_Account_Registration.html";

            var subject = "Confirm Account Registration";
            string HtmlBody = "";
            using (StreamReader streamReader = System.IO.File.OpenText(PathToFile))
            {
                HtmlBody = streamReader.ReadToEnd();
            }
            string Message = $"Please confirm your account by <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicking here</a>.";

            var userInDb = _unitOfWork.ApplicationUser.GetFirstOrDefault(c => c.Email == user.Email);
            string messageBody = string.Format(HtmlBody,
                subject,
                String.Format("{0:dddd, d MMMM yyyy}", DateTime.Now),
                userInDb.Name,
                user.Email,
                Message,
                callbackUrl
                );

            await _emailSender.SendEmailAsync(
                Input.Email,
                "Confirm your email",
                messageBody);

            ModelState.AddModelError(string.Empty, "Verification email sent. Please check your email.");
            return Page();
        }
    }
}
