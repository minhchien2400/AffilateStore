using AffiliateStoreBE.Common.Models;
using AffiliateStoreBE.Common;
using AffiliateStoreBE.DbConnect;
using Microsoft.AspNetCore.Mvc;
using static AffiliateStoreBE.Controllers.CategoryController;
using Swashbuckle.AspNetCore.Annotations;
using AffiliateStoreBE.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using AffiliateStoreBE.Service.IService;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.AspNetCore.Authorization;
using System.ComponentModel.DataAnnotations;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.WebUtilities;
using System.Security.Cryptography.Pkcs;

namespace AffiliateStoreBE.Controllers
{
    public class AccountController : ApiBaseController
    {
        private readonly StoreDbContext _storeDbContext;
        private readonly UserManager<Account> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly SignInManager<Account> _signInManager;
        private readonly IEmailService _emailService;
        private readonly IConfiguration _configuration;
        public AccountController(StoreDbContext storeDbContext, RoleManager<IdentityRole> roleManager, UserManager<Account> userManager, IEmailService emailService, IConfiguration configuration, SignInManager<Account> signInManager)
        {
            _storeDbContext = storeDbContext;
            _roleManager = roleManager;
            _userManager = userManager;
            _emailService = emailService;
            _configuration = configuration;
            _signInManager = signInManager;
        }

        [Authorize]
        [HttpPost("getaccount")]
        [SwaggerResponse(200)]
        public async Task<IActionResult> GetAccount()
        {
            try
            {
                var accounts = await _storeDbContext.Set<Account>().Select(a => new
                {
                    a.Id,
                    a.UserName
                }).ToListAsync();
                //var account = await _storeDbContext.Set<Account>().Where(a => a.Email.Equals(signIn.Email) && a.Password.Equals(signIn.Password)).FirstOrDefaultAsync();
                //if (account != null)
                //{
                //    return Ok(true);
                //}
                return Ok(false);
            }
            catch (Exception ex)
            {
                throw;
            }
        }


        [HttpPost("signin")]
        [SwaggerResponse(200)]
        public async Task<IActionResult> SignIn([FromBody] SignInModel signIn)
        {
            try
            {
                //var account = await _userManager.FindByEmailAsync(signIn.Email);
                var account = await _userManager.FindByNameAsync(signIn.Username);
                if (account != null && await _userManager.CheckPasswordAsync(account, signIn.Password))
                {
                    var authClaims = new List<Claim>
                    {
                        new Claim(ClaimTypes.Name, account.UserName),
                        new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                    };
                    var userRoles = await _userManager.GetRolesAsync(account);
                    foreach (var role in userRoles)
                    {
                        authClaims.Add(new Claim(ClaimTypes.Role, role));
                    }

                    var jwtToken = GetToken(authClaims);

                    return Ok(new
                    {
                        token = new JwtSecurityTokenHandler().WriteToken(jwtToken),
                        expiration = jwtToken.ValidTo
                    });
                }

                return Unauthorized();
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        [HttpPost("signup")]
        [SwaggerResponse(200)]
        public async Task<IActionResult> SignUp([FromBody] SignUpModel signUp)
        {
            try
            {
                var accountExist = await _userManager.FindByEmailAsync(signUp.Email);
                if (accountExist != null)
                {
                    return Ok(new
                    {
                        Result = false,
                        Message = "Email da ton tai",
                    });
                }

                var newAccount = new Account
                {
                    Email = signUp.Email,
                    SecurityStamp = Guid.NewGuid().ToString(),
                    UserName = signUp.UserName,
                    Age = signUp.Age,
                    Gender = signUp.Gender,
                    Country = signUp.Country
                };
                if (await _roleManager.RoleExistsAsync(signUp.Role))
                {
                    var result = await _userManager.CreateAsync(newAccount, signUp.Password);
                    if (result.Succeeded)
                    {
                        await _userManager.AddToRoleAsync(newAccount, signUp.Role);

                        // token verify email...
                        var token = await _userManager.GenerateEmailConfirmationTokenAsync(newAccount);
                        var confirmationLink = Url.Action(nameof(ConfirmEmail), "Account", new { token, email = newAccount.Email }, protocol: Request.Scheme);
                        var message = new Message(new string[] { newAccount.Email! }, "Confirmation email link", $"Bạn vừa tạo một tài khoản. Bấm vào liên kết sau để xác thực tài khoản: {confirmationLink}");

                        _emailService.SendEmail(message);
                        if(!_userManager.Options.SignIn.RequireConfirmedAccount)
                        {
                            await _signInManager.SignInAsync(newAccount, isPersistent: true); //isPersistent giup luu lai cookie de duy tri dang nhap
                            //LocalRedirect()
                        }
                        return Ok(new
                        {
                            Result = true,
                            Message = "Tao tai khoan thanh cong",
                        });
                    }


                }
                else
                {
                    return Ok(new
                    {
                        Result = false,
                        Message = "Role exist",
                    });
                }
                return Ok(new
                {
                    Result = false,
                    Message = "Tao tai khoan khong thanh cong",
                });
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        [HttpPost("forgotpassword")]
        [SwaggerResponse(200)]
        public async Task<IActionResult> ForgotPassword([Required] string email)
        {
            try
            {
                var account = await _userManager.FindByEmailAsync(email);
                if (account != null)
                {
                    var token = await _userManager.GeneratePasswordResetTokenAsync(account);
                    token = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(token));
                    var forgotPasswordLink = Url.Action("ResetPassword", "Account", new { token, email }, Request.Scheme);
                    var message = new Message(new string[] { account.Email! }, "Confirm email link", forgotPasswordLink!);
                    _emailService.SendEmail(message);
                    return Ok(new
                    {
                        Result = true,
                    });
                }
                return Ok(new
                {
                    Result = false,
                    Message = "Forgot password failed!",
                });
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        [HttpGet("signout")]
        [SwaggerResponse(200)]
        public async Task<IActionResult> SignOut(string token, string email)
        {
            try
            {
                await _signInManager.SignOutAsync();
                return Ok(true);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        [HttpGet("resetpassword")]
        [SwaggerResponse(200)]
        public async Task<IActionResult> ResetPassword(string token, string email)
        {
            try
            {

                return Ok(new ResetPasswordModel { Token = token, Email = email });
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        [HttpGet("authentication")]
        [SwaggerResponse(200)]
        public async Task<IActionResult> ConfirmEmail(string token, string email)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user != null)
            {
                token = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(token));
                var result = await _userManager.ConfirmEmailAsync(user, token);
                if (result.Succeeded)
                {
                    return Ok("Confirm email successfull!");
                }
            }
            return Ok(false);
        }

        private JwtSecurityToken GetToken(List<Claim> authClaims)
        {
            var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:Secret"]));

            var token = new JwtSecurityToken(
                issuer: _configuration["JWT:ValidIssuer"],
                audience: _configuration["JWT:ValidAudience"],
                expires: DateTime.UtcNow.AddHours(3),
                claims: authClaims,
                signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256)
                );

            return token;
        }

        public class SignInModel
        {
            public string Username { get; set; }
            public string Password { get; set; }
            public bool RememberMe { get; set; } 
        }
        public class SignUpModel
        {
            public string UserName { get; set; }
            public string Email { get; set; }
            public string Password { get; set; }
            public int Age { get; set; }
            public Gender Gender { get; set; }
            public string Country { get; set; }
            public string Role { get; set; }
        }
        public class ResetPasswordModel
        {
            public string Password { get; set; } = null!;
            [Compare("Password", ErrorMessage = "The password and confirmpassword not same")]
            public string ConfirmPassword { get; set; } = null!;
            public string Email { get; set; } = null!;
            public string Token { get; set; } = null!;
        }
    }
}