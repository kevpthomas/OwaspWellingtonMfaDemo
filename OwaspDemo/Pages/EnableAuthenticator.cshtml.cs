using System.ComponentModel.DataAnnotations;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using OwaspDemo.Abstractions;
using OwaspDemo.Models;

namespace OwaspDemo.Pages
{
    public class EnableAuthenticatorModel : PageModel
    {
        // https://github.com/google/google-authenticator/wiki/Key-Uri-Format
        private const string AuthenticatorUriFormat = "otpauth://totp/{0}:{1}?secret={2}&issuer={0}&digits=6";

        private readonly IUserManager _userManager;
        private readonly UrlEncoder _urlEncoder;

        public EnableAuthenticatorModel(IUserManager userManager, UrlEncoder urlEncoder)
        {
            _userManager = userManager;
            _urlEncoder = urlEncoder;
        }

        public string SharedKey { get; set; }

        public string AuthenticatorUri { get; set; }

        public string QrCodeUrl { get; set; }
        
        [TempData]
        public string StatusMessage { get; set; }

        [BindProperty]
        public InputModel Input { get; set; }

        public class InputModel
        {
            [Required]
            [StringLength(6, ErrorMessage = "The {0} must be exactly {1} characters.", MinimumLength = 6)]
            [DataType(DataType.Text)]
            [Display(Name = "Verification Code")]
            public string Code { get; set; }
        }

        public async Task<IActionResult> OnGetAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            await LoadSharedKeyAndQrCodeUriAsync(user);

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }
            
            if (!ModelState.IsValid)
            {
                await LoadSharedKeyAndQrCodeUriAsync(user);
                return Page();
            }
            
            // Strip spaces and hyphens
            var verificationCode = Input.Code.Replace(" ", string.Empty).Replace("-", string.Empty);

            // ReSharper disable once InconsistentNaming
            var is2faTokenValid = await _userManager.VerifyTwoFactorTokenAsync(
                user, verificationCode);
            
            if (!is2faTokenValid)
            {
                ModelState.AddModelError("Input.Code", "Verification code is invalid. Your device clock might not be synced.");
                await LoadSharedKeyAndQrCodeUriAsync(user);
                return Page();
            }

            await _userManager.SetTwoFactorEnabledAsync(user, true);

            StatusMessage = "Your authenticator app has been verified.";

            return RedirectToPage("Index");
        }

        private async Task LoadSharedKeyAndQrCodeUriAsync(UserData user)
        {
            // Load the authenticator key & QR code URI to display on the form
            var unformattedKey = await _userManager.GetAuthenticatorKeyAsync(user);
            if (string.IsNullOrEmpty(unformattedKey))
            {
                await _userManager.ResetAuthenticatorKeyAsync(user);
                unformattedKey = await _userManager.GetAuthenticatorKeyAsync(user);
            }

            SharedKey = FormatKey(unformattedKey);

            const string issuer = "OWASP MFA Demo";
            QrCodeUrl = string.Format(AuthenticatorUriFormat,
                issuer,
                user.UserName,
                unformattedKey);

            AuthenticatorUri = string.Format(AuthenticatorUriFormat,
                _urlEncoder.Encode(issuer),
                _urlEncoder.Encode(user.UserName),
                unformattedKey);
        }

        private static string FormatKey(string unformattedKey)
        {
            var result = new StringBuilder();
            var currentPosition = 0;
            while (currentPosition + 4 < unformattedKey.Length)
            {
                result.Append(unformattedKey.Substring(currentPosition, 4)).Append(" ");
                currentPosition += 4;
            }
            if (currentPosition < unformattedKey.Length)
            {
                result.Append(unformattedKey.Substring(currentPosition));
            }

            return result.ToString().ToUpperInvariant();
        }
    }
}