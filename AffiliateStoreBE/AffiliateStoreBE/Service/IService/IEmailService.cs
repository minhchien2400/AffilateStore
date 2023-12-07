
using AffiliateStoreBE.Models;

namespace AffiliateStoreBE.Service.IService
{
    public interface IEmailService
    {
        void SendEmail(Message message);
    }
}
