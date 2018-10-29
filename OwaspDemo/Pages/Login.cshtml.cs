using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using OwaspDemo.Abstractions;
using OwaspDemo.Models;

namespace OwaspDemo.Pages
{
    public class LoginModel : PageModel
    {
        private readonly ISignInManager _signInManager;

        public LoginModel(ISignInManager signInManager)
        {
            _signInManager = signInManager;
        }

        [BindProperty] // Bind on Post
        public LoginData LoginData { get; set; }
        
        public async Task OnGetAsync()
        {
            await HttpContext.SignOutAsync();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (ModelState.IsValid)
            {
                var result = await _signInManager.PasswordSignInAsync(LoginData.Username, LoginData.Password);
                if (result.RequiresTwoFactor) return RedirectToPage("LoginWith2fa");
                if (result.Succeeded) return RedirectToPage("Index");
            }

            ModelState.AddModelError(string.Empty, "Invalid login attempt.");
            return Page();
        }
    }
}