using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using OwaspDemo.Abstractions;

namespace OwaspDemo.Pages
{
    public class RegisterModel : PageModel
    {
        private readonly IUserManager _userManager;
        private readonly ISignInManager _signInManager;


        public RegisterModel(IUserManager userManager, ISignInManager signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        [BindProperty]
        public InputModel Input { get; set; }

        public class InputModel
        {
            [Required]
            [EmailAddress]
            [Display(Name = "User Name (email)")]
            public string Email { get; set; }

            [Required]
            [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 8)]
            [DataType(DataType.Password)]
            [Display(Name = "Password")]
            public string Password { get; set; }

            [DataType(DataType.Password)]
            [Display(Name = "Confirm password")]
            [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
            public string ConfirmPassword { get; set; }

            [Display(Name = "Enable Two-Factor Authentication")]
            public bool EnableTwoFactor { get; set; } = true;
        }

        public void OnGet()
        {
            Input = new InputModel();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (ModelState.IsValid)
            {
                var identityResult = await _userManager.CreateAsync(Input.Email, Input.Password);
                if (identityResult.Succeeded)
                {
                    await _signInManager.SignInAsync(Input.Email);

                    if (Input.EnableTwoFactor) return RedirectToPage("EnableAuthenticator");
                    return RedirectToPage("Index");

                    //var user = await _userManager.FindByNameAsync(Input.Email);
                    //var signinResult = await _signInManager.SignInOrTwoFactorAsync(user);

                    //if (signinResult.RequiresTwoFactor)
                    //{
                    //    return RedirectToPage("EnableAuthenticator");
                    //}

                    //if (signinResult.Succeeded)
                    //{
                    //    return RedirectToPage("Index");
                    //}
                    ////await _signInManager.SignInAsync(Input.Email);
                    ////return RedirectToPage("Index");
                }
                ModelState.AddModelError(string.Empty, "Registration failed");
            }

            // If we got this far, something failed, redisplay form
            return Page();
        }
    }
}