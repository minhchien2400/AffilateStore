using AffiliateStoreBE.Common;
using AffiliateStoreBE.DbConnect;
using Microsoft.AspNetCore.Mvc;
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
using System.Security.Cryptography;

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
                var account = await _userManager.FindByNameAsync(signIn.UsernameOrEmail);
                if(account == null)
                {
                    account = await _userManager.FindByEmailAsync(signIn.UsernameOrEmail);
                }
                if (account != null && await _userManager.CheckPasswordAsync(account, signIn.Password))
                {
                    var userRoles = await _userManager.GetRolesAsync(account);
                    var jwtToken = GenerateJwt(account, userRoles);

                    var refreshToken = GenerateRefreshToken();

                    await _storeDbContext.AddAsync(new RefreshToken
                    {
                        Id = Guid.NewGuid(),
                        RefreshTokenStr = refreshToken,
                        ExpireDate = DateTime.UtcNow.AddDays(60),
                        AccountId = account.Id
                    });
                    await _storeDbContext.SaveChangesAsync();

                    return Ok(new
                    {
                        Token = new JwtSecurityTokenHandler().WriteToken(jwtToken),
                        Expiration = jwtToken.ValidTo,
                        RefreshToken = refreshToken
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
                        token = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(token));
                        var confirmationLink = Url.Action(nameof(ConfirmEmail), "Account", new { token, email = newAccount.Email }, protocol: Request.Scheme);
                        var message = new Message(new string[] { newAccount.Email! }, "Confirmation email link", $"Bạn vừa tạo một tài khoản. Bấm vào liên kết sau để xác thực tài khoản: {confirmationLink}");

                        _emailService.SendEmail(message);
                        if (!_userManager.Options.SignIn.RequireConfirmedAccount)
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

        [HttpPost("refresh")]
        [SwaggerResponse(200)]
        public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenModel tokens)
        {
            var principal = GetPrincipalFromExpiredToken(tokens.AccessToken);

            if (principal?.Identity?.Name is null)
                return Unauthorized();

            var user = await _userManager.FindByNameAsync(principal.Identity.Name);
            var userRoles = await _userManager.GetRolesAsync(user);
            var refreshTokenExist = await _storeDbContext.Set<RefreshToken>().AnyAsync(r => r.RefreshTokenStr.Equals(tokens.RefreshToken) && r.ExpireDate > DateTime.UtcNow);

            if (user is null || !refreshTokenExist)
                return Unauthorized();

            var token = GenerateJwt(user, userRoles);


            return Ok(new
            {
                JwtToken = new JwtSecurityTokenHandler().WriteToken(token),
                Expiration = token.ValidTo,
                RefreshToken = tokens.RefreshToken
            });
        }

        [Authorize]
        [HttpDelete("Revoke")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Revoke(string refreshToken)
        {
            var username = HttpContext.User.Identity?.Name;

            if (username is null)
                return Unauthorized();

            var user = await _userManager.FindByNameAsync(username);
            var refreshTokenRovoke = await _storeDbContext.Set<RefreshToken>().Where(a => a.RefreshTokenStr.Equals(refreshToken)).Select(a => a.RefreshTokenStr).FirstOrDefaultAsync();
            refreshTokenRovoke = null;
            await _storeDbContext.SaveChangesAsync();
            if (user is null)
                return Unauthorized();

            return Ok();
        }

        private ClaimsPrincipal? GetPrincipalFromExpiredToken(string token)
        {
            try
            {
                var secret = _configuration["JWT:Secret"] ?? throw new InvalidOperationException("Secret not configured");

                var validation = new TokenValidationParameters
                {
                    ValidIssuer = _configuration["JWT:ValidIssuer"],
                    ValidAudience = _configuration["JWT:ValidAudience"],
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret)),
                    ValidateLifetime = false
                };

                var principal = new JwtSecurityTokenHandler().ValidateToken(token, validation, out _);
                return principal;
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        private JwtSecurityToken GenerateJwt(Account account, IList<string> roles)
        {
            var authClaims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, account.UserName),
                new Claim(ClaimTypes.Email, account.Email),
                new Claim(ClaimTypes.Gender, GetEnumDescription(account.Gender)),
                new Claim(ClaimTypes.Country, account.Country),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            };
            foreach (var role in roles)
            {
                authClaims.Add(new Claim(ClaimTypes.Role, role));
            }

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(
                _configuration["JWT:Secret"] ?? throw new InvalidOperationException("Secret not configured")));

            var token = new JwtSecurityToken(
                issuer: _configuration["JWT:ValidIssuer"],
                audience: _configuration["JWT:ValidAudience"],
                expires: DateTime.UtcNow.AddSeconds(30),
                claims: authClaims,
                signingCredentials: new SigningCredentials(key, SecurityAlgorithms.HmacSha256)
                );

            return token;
        }

        private static string GenerateRefreshToken()
        {
            var randomNumber = new byte[64];

            using var generator = RandomNumberGenerator.Create();

            generator.GetBytes(randomNumber);

            return Convert.ToBase64String(randomNumber);
        }

        // Hàm giải mã mật khẩu
        private static string DecryptPassword(string hashedPassword)
        {
            byte[] hashedBytes = Convert.FromBase64String(hashedPassword);
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] decryptedBytes = sha256.ComputeHash(hashedBytes);
                return Encoding.UTF8.GetString(decryptedBytes);
            }
        }

        public class SignInModel
        {
            public string UsernameOrEmail { get; set; }
            public string Password { get; set; }
            // public bool RememberMe { get; set; }
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
        public class RefreshTokenModel
        {
            public string AccessToken { get; set; }
            public string RefreshToken { get; set; }
        }
    }
}