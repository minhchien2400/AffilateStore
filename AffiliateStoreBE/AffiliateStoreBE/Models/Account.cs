using AffiliateStoreBE.Common.Models;
using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace AffiliateStoreBE.Models
{
    public class Account : IdentityUser
    {
        public int Age { get; set; }
        public Gender Gender { get; set; }
        public string Country { get; set; }
    }

    public enum Gender
    {
        Male = 0,
        Female = 1,
        Other = 2,
    }
}
