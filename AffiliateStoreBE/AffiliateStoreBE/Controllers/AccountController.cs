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

namespace AffiliateStoreBE.Controllers
{
    public class AccountController : ApiBaseController
    {
        private readonly StoreDbContext _storeDbContext;
        private readonly UserManager<Account> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IEmailService _emailService;
        private readonly IConfiguration _configuration;
        public AccountController(StoreDbContext storeDbContext, RoleManager<IdentityRole> roleManager, UserManager<Account> userManager, IEmailService emailService, IConfiguration configuration)
        {
            _storeDbContext = storeDbContext;
            _roleManager = roleManager;
            _userManager = userManager;
            _emailService = emailService;
            _configuration = configuration;
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
                        var message = new Message(new string[] { newAccount.Email! }, "Confirmation email link", confirmationLink!);
                        _emailService.SendEmail(message);
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

        //[HttpPost("forgotpassword")]
        //[SwaggerResponse(200)]
        //public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswprd acc)
        //{
        //    try
        //    {
        //        var account = await _userManager.FindByEmailAsync(acc.Email);
        //        if (account != null)
        //        {
        //            account.Password = acc.NewPassword;
        //            await _storeDbContext.SaveChangesAsync();
        //            return Ok(new
        //            {
        //                Result = true,
        //                Message = "Doi mat khau thanh cong",
        //            });
        //        }
        //        return Ok(new
        //        {
        //            Result = true,
        //            Message = "Email hoac mat khau khong chinh xac",
        //        });
        //    }
        //    catch (Exception ex)
        //    {
        //        throw;
        //    }
        //}

        [HttpPost("resetpassword")]
        [SwaggerResponse(200)]
        public async Task<IActionResult> SignUp([FromBody] string email)
        {
            try
            {
                var account = await _storeDbContext.Set<Account>().Where(a => a.Email.Equals(email)).FirstOrDefaultAsync();
                if (account != null)
                {
                    var newPassword = GeneratePassword();
                    return Ok(new
                    {
                        Result = true,
                    });
                }

                return Ok(new
                {
                    Result = false,
                    Message = "Email khong chinh xac",
                });
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
        public class ForgotPasswprd
        {
            public string Email { get; set; }
            public string OldPassword { get; set; }
            public string NewPassword { get; set; }
        }
    }
}