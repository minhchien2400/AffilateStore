using AffiliateStoreBE.Common.Models;

namespace AffiliateStoreBE.Common.Service
{
    public interface IBackgroundTaskService
    {
        Task Start<T>(object param, BackgroundTaskType taskType, string eventName, Type eventContextType, CommonConstants.Module eventModule, string message, string authorization = null) where T : BackgroundTaskAgent;
    }
}
