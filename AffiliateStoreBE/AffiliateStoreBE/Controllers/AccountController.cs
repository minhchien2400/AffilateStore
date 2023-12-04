using AffiliateStoreBE.Common.Models;
using AffiliateStoreBE.Common;
using AffiliateStoreBE.DbConnect;
using Microsoft.AspNetCore.Mvc;
using static AffiliateStoreBE.Controllers.CategoryController;
using Swashbuckle.AspNetCore.Annotations;
using AffiliateStoreBE.Models;
using Microsoft.EntityFrameworkCore;

namespace AffiliateStoreBE.Controllers
{
    public class AccountController : ApiBaseController
    {
        private readonly StoreDbContext _storeDbContext;
        public AccountController(StoreDbContext storeDbContext)
        {
            _storeDbContext = storeDbContext;
        }
        [HttpPost("signin")]
        [SwaggerResponse(200)]
        public async Task<IActionResult> SignIn([FromBody] SignInModel signIn)
        {
            try
            {
                var account = await _storeDbContext.Set<Account>().Where(a => a.Email.Equals(signIn.Email) && a.Password.Equals(signIn.Password)).FirstOrDefaultAsync();
                if (account != null)
                {
                    return Ok(true);
                }
                return Ok(false);
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
                var oldAccount = await _storeDbContext.Set<Account>().Where(a => a.Email.Equals(signUp.Email)).FirstOrDefaultAsync();
                if (oldAccount != null)
                {
                    return Ok(new
                    {
                        Result = false,
                        Message = "Email da ton tai",
                    });
                }

                return Ok(new
                {
                    Result = true,
                    Message = "Tao tai khoan thanh cong",
                });
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        [HttpPost("forgotpassword")]
        [SwaggerResponse(200)]
        public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswprd acc)
        {
            try
            {
                var account = await _storeDbContext.Set<Account>().Where(a => a.Email.Equals(acc.Email) && a.Password.Equals(acc.OldPassword)).FirstOrDefaultAsync();
                if (account != null)
                {
                    account.Password = acc.NewPassword;
                    await _storeDbContext.SaveChangesAsync();
                    return Ok(new
                    {
                        Result = true,
                        Message = "Doi mat khau thanh cong",
                    });
                }
                return Ok(new
                {
                    Result = true,
                    Message = "Email hoac mat khau khong chinh xac",
                });
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        [HttpPost("resetpassword")]
        [SwaggerResponse(200)]
        public async Task<IActionResult> SignUp([FromBody] string  email)
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
        public class SignInModel
        {
            public string Email { get; set; }
            public string Password { get; set; }
        }
        public class SignUpModel
        {
            public string Email { get; set; }
            public string Password { get; set; }
            public int Age { get; set; }
            public Gender Gender { get; set; }
            public string Country { get; set; }
        }
        public class ForgotPasswprd
        {
            public string Email { get; set; }
            public string OldPassword { get; set; }
            public string NewPassword { get; set; }
        }
    }
}