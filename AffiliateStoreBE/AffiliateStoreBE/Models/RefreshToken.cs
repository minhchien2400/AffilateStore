namespace AffiliateStoreBE.Models
{
    public class RefreshToken
    {
        public Guid Id { get; set; }    
        public string RefreshTokenStr { get; set; }
        public DateTime ExpireDate { get; set; }
        public string AccountId { get; set; }
        public Account Account { get; set; }
    }
}
