using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Inventory_Managment_System.DTOs;
using Inventory_Managment_System.Models;
using System.Threading.Tasks;
using System.Linq;
using Inventory_Managment_System.Services;
using Microsoft.Extensions.Logging;

namespace Inventory_Managment_System.Controllers
{
    public class AuthController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly EmailService _emailService;
        private readonly ILogger<AuthController> _logger;

        public AuthController(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            EmailService emailService,
            ILogger<AuthController> logger)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _emailService = emailService;
            _logger = logger;
        }

        [HttpGet]
        public IActionResult Login(string returnUrl = null)
        {
            _logger.LogInformation("Accessing Login GET action with returnUrl: {ReturnUrl}", returnUrl);
            ViewData["ReturnUrl"] = returnUrl;
            return View(new LoginModel { ReturnUrl = returnUrl });
        }

        [HttpPost]
        public async Task<IActionResult> ValidateLogin(LoginModel model, string returnUrl = null)
        {
            _logger.LogInformation("Login attempt for user: {UserName}", model.UserName);
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByNameAsync(model.UserName?.Trim());
                if (user != null && user.is_active)
                {
                    var result = await _signInManager.PasswordSignInAsync(model.UserName, model.Password, model.RememberMe, lockoutOnFailure: true);
                    if (result.Succeeded)
                    {
                        HttpContext.Session.SetString("username", user.Name ?? user.UserName);
                        var roles = await _userManager.GetRolesAsync(user);
                        var role = roles.FirstOrDefault() ?? "User";
                        HttpContext.Session.SetString("role", role);
                        HttpContext.Session.SetInt32("EmployeeId", int.TryParse(user.Id, out int id) ? id : 1);
                        HttpContext.Session.SetInt32("EmployeeLocationId", user.Location_id ?? 0);

                        returnUrl = returnUrl ?? Url.Content("~/");
                        string redirectController = role == "Admin" ? "AdminDashboard" : "ManagerDashboard";
                        _logger.LogInformation("Login successful for user: {UserName}, redirecting to {Controller}", model.UserName, redirectController);
                        return RedirectToAction("Index", redirectController);
                    }
                    if (result.RequiresTwoFactor)
                    {
                        try
                        {
                            var code = await _userManager.GenerateTwoFactorTokenAsync(user, "Email");
                            await _emailService.SendEmailAsync(user.Email, "Your 2FA Code", $"Your two-factor authentication code is: {code}");
                            _logger.LogInformation("2FA code sent to user: {UserName}", model.UserName);
                        }
                        catch (Exception ex)
                        {
                            _logger.LogError(ex, "Failed to send 2FA code for user: {UserName}", model.UserName);
                            ModelState.AddModelError("", "Failed to send 2FA code. Please try again later.");
                            return View("Login", model);
                        }
                        return RedirectToAction("LoginWith2fa", new { rememberMe = model.RememberMe, returnUrl });
                    }
                    if (result.IsLockedOut)
                    {
                        _logger.LogWarning("User account locked out for user: {UserName}", model.UserName);
                        ModelState.AddModelError("", "User account is locked out.");
                    }
                    else
                    {
                        _logger.LogWarning("Invalid login attempt for user: {UserName}", model.UserName);
                        ModelState.AddModelError("", "Invalid login attempt.");
                    }
                }
                else
                {
                    _logger.LogWarning("Invalid login attempt or user is inactive: {UserName}", model.UserName);
                    ModelState.AddModelError("", "Invalid login attempt or user is inactive.");
                }
            }
            else
            {
                _logger.LogWarning("Form validation failed for login attempt: {UserName}", model.UserName);
                ModelState.AddModelError("", "Form validation failed. Please check your input.");
            }
            return View("Login", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            _logger.LogInformation("User logging out");
            await _signInManager.SignOutAsync();
            HttpContext.Session.Clear();
            return RedirectToAction("Login", "Auth");
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterModel model)
        {
            if (ModelState.IsValid)
            {
                var user = new ApplicationUser
                {
                    UserName = model.UserName,
                    Email = model.Email,
                    Name = model.UserName,
                    Phone = "",
                    Location_id = null,
                    is_active = true
                };
                var result = await _userManager.CreateAsync(user, model.Password);

                if (result.Succeeded)
                {
                    _logger.LogInformation("User registered successfully: {UserName}", model.UserName);
                    await _signInManager.SignInAsync(user, isPersistent: false);
                    return RedirectToAction("Index", "Home");
                }
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                    _logger.LogWarning("Registration error for user {UserName}: {Error}", model.UserName, error.Description);
                }
            }
            return View(model);
        }

        [HttpGet]
        public IActionResult ForgotPassword()
        {
            _logger.LogInformation("Accessing Forgot Password GET action");
            return View(new ForgotPasswordModel());
        }

        [HttpPost]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordModel model)
        {
            _logger.LogInformation("Forgot password request for email: {Email}", model.Email);
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByEmailAsync(model.Email);
                if (user == null || !user.is_active)
                {
                    ModelState.AddModelError("", "No active user found with that email address.");
                    _logger.LogWarning("Forgot password failed - no active user found for email: {Email}", model.Email);
                    return View(model);
                }
                return RedirectToAction("SecurityQuestion", new { email = model.Email });
            }
            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> SecurityQuestion(string email)
        {
            _logger.LogInformation("Accessing Security Question GET action for email: {Email}", email);
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null || string.IsNullOrEmpty(user.SecurityQuestion) || string.IsNullOrEmpty(user.SecurityAnswer))
            {
                _logger.LogWarning("Security question not set or user not found for email: {Email}", email);
                ModelState.AddModelError("", "Security question not set for this account. Contact support.");
                return View("ForgotPassword", new ForgotPasswordModel { Email = email });
            }
            ViewBag.Email = email;
            ViewBag.SecurityQuestion = user.SecurityQuestion;
            return View(new SecurityQuestionModel { Email = email });
        }

        [HttpPost]
        public async Task<IActionResult> SecurityQuestion(SecurityQuestionModel model)
        {
            _logger.LogInformation("Security question submission for email: {Email}", model.Email);
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByEmailAsync(model.Email);
                if (user == null || string.IsNullOrEmpty(user.SecurityAnswer))
                {
                    ModelState.AddModelError("", "Invalid email or security answer not set.");
                    _logger.LogWarning("Security question failed - invalid email or answer not set for: {Email}", model.Email);
                    return View(model);
                }
                if (user.SecurityAnswer.ToLower() != model.Answer.ToLower())
                {
                    ModelState.AddModelError("", "Incorrect security answer.");
                    _logger.LogWarning("Incorrect security answer for email: {Email}", model.Email);
                    return View(model);
                }
                return RedirectToAction("ResetPassword", new { email = model.Email });
            }
            return View(model);
        }

        [HttpGet]
        public IActionResult ResetPassword(string email)
        {
            _logger.LogInformation("Accessing Reset Password GET action for email: {Email}", email);
            return View(new ResetPasswordModel { Email = email });
        }

        [HttpPost]
        public async Task<IActionResult> ResetPassword(ResetPasswordModel model)
        {
            _logger.LogInformation("Reset password request for email: {Email}", model.Email);
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByEmailAsync(model.Email);
                if (user == null)
                {
                    ModelState.AddModelError("", "No user found with that email address.");
                    _logger.LogWarning("Reset password failed - user not found for email: {Email}", model.Email);
                    return View(model);
                }
                var token = await _userManager.GeneratePasswordResetTokenAsync(user); // Generate token for reset
                var result = await _userManager.ResetPasswordAsync(user, token, model.NewPassword);
                if (result.Succeeded)
                {
                    _logger.LogInformation("Password reset successful for email: {Email}", model.Email);
                    return RedirectToAction("ResetPasswordConfirmation");
                }
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                    _logger.LogWarning("Reset password error for email {Email}: {Error}", model.Email, error.Description);
                }
            }
            return View(model);
        }

        [HttpGet]
        public IActionResult ResetPasswordConfirmation()
        {
            _logger.LogInformation("Accessing Reset Password Confirmation view");
            return View();
        }

        [HttpGet]
        public IActionResult ForgotPasswordConfirmation()
        {
            _logger.LogInformation("Accessing Forgot Password Confirmation view");
            // This view is no longer needed for the new flow but kept for fallback
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> LoginWith2fa(bool rememberMe, string returnUrl = null)
        {
            var user = await _signInManager.GetTwoFactorAuthenticationUserAsync();
            if (user == null)
            {
                _logger.LogWarning("No user found for 2FA login");
                return RedirectToAction(nameof(Login));
            }

            var model = new LoginWith2faModel { RememberMe = rememberMe };
            ViewData["ReturnUrl"] = returnUrl;
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> LoginWith2fa(LoginWith2faModel model, string returnUrl = null)
        {
            if (ModelState.IsValid)
            {
                var result = await _signInManager.TwoFactorAuthenticatorSignInAsync(model.TwoFactorCode, model.RememberMe, rememberClient: model.RememberMachine);
                if (result.Succeeded)
                {
                    var user = await _signInManager.GetTwoFactorAuthenticationUserAsync();
                    if (user != null && user.is_active)
                    {
                        HttpContext.Session.SetString("username", user.Name ?? user.UserName);
                        var roles = await _userManager.GetRolesAsync(user);
                        HttpContext.Session.SetString("role", roles.FirstOrDefault() ?? "User");
                        HttpContext.Session.SetInt32("EmployeeId", int.TryParse(user.Id, out int id) ? id : 1);
                        HttpContext.Session.SetInt32("EmployeeLocationId", user.Location_id ?? 0);

                        returnUrl = returnUrl ?? Url.Content("~/");
                        _logger.LogInformation("2FA login successful for user: {UserName}, redirecting to {ReturnUrl}", user.UserName, returnUrl);
                        return LocalRedirect(returnUrl);
                    }
                    await _signInManager.SignOutAsync();
                    _logger.LogWarning("2FA login failed - user is inactive: {UserName}", user?.UserName);
                    ModelState.AddModelError("", "User is inactive.");
                }
                if (result.IsLockedOut)
                {
                    _logger.LogWarning("User account locked out during 2FA login");
                    ModelState.AddModelError("", "User is locked out.");
                }
                else if (result.IsNotAllowed)
                {
                    _logger.LogWarning("2FA not allowed for user");
                    ModelState.AddModelError("", "2FA is not enabled for this user.");
                }
                else
                {
                    _logger.LogWarning("Invalid 2FA code");
                    ModelState.AddModelError("", "Invalid 2FA code.");
                }
            }
            ViewData["ReturnUrl"] = returnUrl;
            return View(model);
        }
    }
}