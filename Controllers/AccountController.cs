using Microsoft.AspNetCore.Mvc;
using LibraryManagementSystemAimanSahharon.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;

namespace LibraryManagementSystemAimanSahharon.Controllers
{
    // This controller handles user authentication using Google OIDC and cookie-based sessions.
    public class AccountController : Controller
    {
        private readonly IMemberService _memberService;
        private readonly ILogger<AccountController> _logger;

        public AccountController(IMemberService memberService, ILogger<AccountController> logger)
        {
            _memberService = memberService;
            _logger = logger;
        }

        // GET /Account/Login
        // Initiates the Google OIDC redirect — sends the user to Google's consent screen
        [HttpGet]
        public IActionResult Login(string? returnUrl = "/")
        {
            // Challenge means "redirect to external provider"
            var properties = new AuthenticationProperties
            {
                RedirectUri = Url.Action("GoogleCallback", "Account",
                    new { returnUrl })
            };
            return Challenge(properties, GoogleDefaults.AuthenticationScheme);
        }

        // GET /Account/GoogleCallback
        // Google redirects here after the user authenticates successfully
        [HttpGet]
        public async Task<IActionResult> GoogleCallback(string? returnUrl = "/")
        {
            // Read the external authentication result (Google's response)
            var result = await HttpContext.AuthenticateAsync(
                CookieAuthenticationDefaults.AuthenticationScheme);

            if (!result.Succeeded)
            {
                _logger.LogWarning("Google authentication failed.");
                return RedirectToAction("Login");
            }

            // Provision new member OR retrieve existing member from our DB
            var member = await _memberService.ProvisionOrGetMemberAsync(result.Principal!);

            // Store the member's role and DB ID in the cookie claims
            // This lets us use [Authorize(Roles = "Librarian")] in controllers
            var claims = new List<System.Security.Claims.Claim>
            {
                new(System.Security.Claims.ClaimTypes.Role, member.Role),
                new("memberId", member.Id.ToString())
            };

            // Add our custom claims to the existing cookie identity
            var identity = result.Principal!.Identity as
                System.Security.Claims.ClaimsIdentity;
            identity?.AddClaims(claims);

            // Re-sign in with the updated claims
            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                result.Principal!);

            _logger.LogInformation("User {Email} signed in as {Role}",
                member.Email, member.Role);

            // Redirect back to where the user wanted to go
            return LocalRedirect(returnUrl ?? "/");
        }

        // POST /Account/Logout
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            // Clear the cookie — user is now signed out
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Index", "Books");
        }
    }
}
