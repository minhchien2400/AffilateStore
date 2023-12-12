using AffiliateStoreBE.Common.Models;
using Microsoft.AspNetCore.Identity;
using System.ComponentModel;
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
        [Description("Male")]
        Male = 0,
        [Description("Felmale")]
        Female = 1,
        [Description("Other")]
        Other = 2,
    }
}
