using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using BookStore.Models;
using BookStore.Repository.IRepository;
using BookStore.Utility;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Logging;

namespace BookStore.Areas.Identity.Pages.Account
{
    [AllowAnonymous]
    public class RegisterModel : PageModel
    {
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly ILogger<RegisterModel> _logger;
        private readonly IEmailSender _emailSender;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IUnitOfWork iunitofwork;

        public RegisterModel(
            UserManager<IdentityUser> userManager,
            SignInManager<IdentityUser> signInManager,
            ILogger<RegisterModel> logger,
            IEmailSender emailSender , RoleManager<IdentityRole> _roleManager , IUnitOfWork iunitofwork) 
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _logger = logger;
            _emailSender = emailSender;
            this._roleManager = _roleManager;
            this.iunitofwork = iunitofwork;
        }

        [BindProperty]
        public InputModel Input { get; set; }

        public string ReturnUrl { get; set; }

        public IList<AuthenticationScheme> ExternalLogins { get; set; }

        public class InputModel
        {
            [Required]
            [EmailAddress]
            [Display(Name = "Email")]
            public string Email { get; set; }

            [Required]
            [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
            [DataType(DataType.Password)]
            [Display(Name = "Password")]
            public string Password { get; set; }

            [DataType(DataType.Password)]
            [Display(Name = "Confirm password")]
            [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
            public string ConfirmPassword { get; set; }
            public string Name { get; set; }
            public string StreetAdress { get; set; }
            public string City { get; set; }
            public string State { get; set; }
            public string PostalCode { get; set; }
            public string PhoneNumber { get; set; }

            public string Role { get; set; }
            public int? CompanyId { get; set; }
            public IEnumerable<SelectListItem> CompanyList { get; set; }
            public IEnumerable<SelectListItem> RolesList { get; set; }

        }

        public async Task OnGetAsync(string returnUrl = null)
        {
            Input = new InputModel()
            {
                CompanyList = iunitofwork.company.GetAll().Select(i => new SelectListItem { Text = i.Name, Value = i.Id.ToString() }),

                RolesList = _roleManager.Roles.Where(u => u.Name != Sd.Role_User_Indi).Select(x => x.Name).Select(x => new SelectListItem
                {
                    Text = x,
                    Value = x
                })
            };
            ReturnUrl = returnUrl;
            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
        }

        public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        {
            returnUrl ??= Url.Content("~/");
            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
            if (ModelState.IsValid)
            {
                var user = new ApplicationUser { UserName = Input.Email, Email = Input.Email , Name = Input.Name ,
                    PostalCode = Input.PostalCode , State = Input.State , StreetAdress = Input.StreetAdress ,
                    PhoneNumber = Input.PhoneNumber , City = Input.City , CompanyId = Input.CompanyId , Role = Input.Role};
                var result = await _userManager.CreateAsync(user, Input.Password);
                if (result.Succeeded)
                {
                    _logger.LogInformation("User created a new account with password.");
                    if (!await _roleManager.RoleExistsAsync(Sd.Role_Admin))
                    {
                        await _roleManager.CreateAsync(new IdentityRole(Sd.Role_Admin));
                    }
                    if (!await _roleManager.RoleExistsAsync(Sd.Role_Employee))
                    {
                        await _roleManager.CreateAsync(new IdentityRole(Sd.Role_Employee));
                    }
                    if (!await _roleManager.RoleExistsAsync(Sd.Role_User_Comp))
                    {
                        await _roleManager.CreateAsync(new IdentityRole(Sd.Role_User_Comp));
                    }
                    if (!await _roleManager.RoleExistsAsync(Sd.Role_User_Indi))
                    {
                        await _roleManager.CreateAsync(new IdentityRole(Sd.Role_User_Indi));
                    }

                    if (user.Role == null)
                    {
                        await _userManager.AddToRoleAsync(user, Sd.Role_User_Indi);
                    }
                    else
                    {
                        if (user.CompanyId > 0)
                        {
                            await _userManager.AddToRoleAsync(user, Sd.Role_User_Comp);
                        }
                        await _userManager.AddToRoleAsync(user, user.Role);
                    }

                      var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                      code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
                      var callbackUrl = Url.Page(
                          "/Account/ConfirmEmail",
                          pageHandler: null,
                          values: new { area = "Identity", userId = user.Id, code = code, returnUrl = returnUrl },
                          protocol: Request.Scheme);

                      await _emailSender.SendEmailAsync(Input.Email, "Confirm your email",
                          $"Please confirm your account by <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicking here</a>.");
                    
                    if (_userManager.Options.SignIn.RequireConfirmedAccount)
                    {
                        return RedirectToPage("RegisterConfirmation", new { email = Input.Email, returnUrl = returnUrl });
                    }
                    else
                    {
                        if(user.Role == null)
                        {
                            await _signInManager.SignInAsync(user, isPersistent: false);
                            return LocalRedirect(returnUrl);
                        }
                        else
                        {
                            return RedirectToAction("index", "User", new { Area = "admin" });
                        }

                      
                    }
                }
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }
            Input = new InputModel()
            {
                CompanyList = iunitofwork.company.GetAll().Select(i => new SelectListItem
                {
                    Text = i.Name,
                    Value = i.Id.ToString()
                }),
                RolesList = _roleManager.Roles.Where(u => u.Name != Sd.Role_User_Indi).Select(x => x.Name).Select(i => new SelectListItem
                {
                    Text = i,
                    Value = i
                })
            };

            // If we got this far, something failed, redisplay form
            return Page();
        }
    }
}
