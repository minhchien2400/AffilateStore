namespace AffiliateStoreBE.Models
{
    public class EmailConfiguration
    {
        public string From { get; set; } = "chienfaker2k@gmail.com";
        public string SmtpServer { get; set; } = "smtp.gmail.com";
        public int Port { get; set; } = 465;
        public string UserName { get; set; } = "chienfaker2k@gmail.com";
        public string Password { get; set; } = "LeonTranEMS2023@";
    }
}

//"From": "chienfaker2k@gmail.com",
//    "SmtpServer": "smtp.gmail.com",
//    "Port": 465,
//    "UserName": "chienfaker2k@gmail.com",
//    "Password": "LeonTranEMS2023@"
