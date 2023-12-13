using IdentityModel;

namespace AvePoint.Confucius.FeatureCommon.Service
{
    public static class HttpContextExtension
    {

        public static Guid CurrentUserId(this HttpContext context)
        {
            var userId = Guid.Empty;
            var userIdClaim = GetClaimValue(context, JwtClaimTypes.Id);
            Guid.TryParse(userIdClaim, out userId);
            return userId;
        }

        public static string CurrentUserEmail(this HttpContext context)
        {
            return GetClaimValue(context, JwtClaimTypes.Email);
        }

        public static string CurrentUserName(this HttpContext context)
        {
            return GetClaimValue(context, JwtClaimTypes.Name);
        }


        public static string GetHostUrl(this HttpContext context)
        {
            return $"{context.Request.Scheme}://{context.GetHost()}";
        }

        public static string GetHost(this HttpContext context)
        {
            var strs = context.Request.Host.Value.Split('/');
            return strs.First();
        }


        private static String GetClaimValue(HttpContext context, String claimType)
        {
            var claimValue = context.User?.Claims?.FirstOrDefault(s => s.Type.Equals(claimType))?.Value;
            if (claimValue == null)
            {
                claimValue = string.Empty;
            }
            return claimValue;
        }
        /// <summary>
        /// 获取客户端IP地址
        /// </summary>
        /// <returns></returns>
        public static string GetIP(this HttpContext context)
        {
            //string result = context.Request.ServerVariables["HTTP_X_FORWARDED_FOR"];
            //if (string.IsNullOrEmpty(result))
            //{
            //    result = context.Request.ServerVariables["REMOTE_ADDR"];
            //}
            //if (string.IsNullOrEmpty(result))
            //{
            //    result = HttpContext.Current.Request.UserHostAddress;
            //}
            //if (string.IsNullOrEmpty(result))
            //{
            //    return "0.0.0.0";
            //}
            return context.Connection.LocalIpAddress.ToString();
        }
    }
}