using AffiliateStoreBE.Common.Models;
using Microsoft.VisualBasic;
using System.Reflection;
using Volo.Abp;

namespace AffiliateStoreBE.Common.Service
{
    public abstract class BackgroundTaskAgent
    {
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private readonly IBackgroundTaskService _backgroundTaskService;
        private Timer _timer;
        private IHttpClientFactory _httpClientFactory;
        protected Guid createdById;
        protected Guid taskId;
        protected decimal? progressBar;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public BackgroundTaskAgent(IBackgroundTaskService backgroundTaskService, IServiceScopeFactory serviceScopeFactory, IHttpContextAccessor httpContextAccessor)
        {
            _serviceScopeFactory = serviceScopeFactory;
            _backgroundTaskService = backgroundTaskService;
            _httpContextAccessor = httpContextAccessor;
        }

        public BackgroundTaskAgent(IBackgroundTaskService backgroundTaskService, IServiceScopeFactory serviceScopeFactory, IHttpClientFactory httpClientFactory, IHttpContextAccessor httpContextAccessor)
        {
            _serviceScopeFactory = serviceScopeFactory;
            _httpClientFactory = httpClientFactory;
            _backgroundTaskService = backgroundTaskService;
            _httpContextAccessor = httpContextAccessor;
        }

        internal async Task Start(BackgroundTaskContext context)
        {
            BackgroundTaskStatus resultStatus = BackgroundTaskStatus.Succeed;
            try
            {
               
                    await Run(context).ConfigureAwait(false);
                    await UpdataProgressBar(1m);
            }
            catch (BusinessException ex)
            {
                resultStatus = BackgroundTaskStatus.FailedWithException;
            }
            catch (Exception ex)
            {
                resultStatus = BackgroundTaskStatus.Failed;
            }
            finally
            {
                context.TaskStatus = resultStatus;
                await OnCompleted(context);
                if (_timer != null)
                {
                    _timer.Dispose();
                }
            }
        }

        public abstract Task Run(BackgroundTaskContext context);

        public virtual async Task OnCompleted(BackgroundTaskContext context)
        {
            try
            {
                Guid? fileId = null;
                try
                {
                    if (context.ReportInfo != null)
                    {
                        using (context.ReportInfo.Stream)
                        {
                            var command = new UploadAzureBlobCommand
                            {
                                CreateById = context.CreatedById,
                                FileName = context.ReportInfo.FileName,
                                UploadStream = context.ReportInfo.Stream,
                                FolderId = Constants.BackgroundTaskFolderId,
                                KeepFileName = true,
                                TenantId = RuntimeContext.Current.TenantInfo.TenantId
                            };
                            var uploadFileInfo = await _mediator.Send(command);
                            if (uploadFileInfo != null)
                            {
                                fileId = uploadFileInfo.Id;
                            }
                        }
                        if (System.IO.File.Exists(context.ReportInfo.TempFilePath))
                        {
                            System.IO.File.Delete(context.ReportInfo.TempFilePath);
                        }
                    }
                }
                catch (Exception ex)
                {
                    _logger.Error($"failed to handle file, error:{ex}");
                    context.TaskStatus = BackgroundTaskStatus.Failed;
                }
                var endTime = new DateTimeOffset(DateTime.UtcNow);
                context.EndTime = endTime;
                string fileName = context?.ReportInfo?.FileName;
                await _dbRepository.UpdateAsync<BackgroundTask>(t => t.Id == context.Id, t => new BackgroundTask { TaskStatus = context.TaskStatus, EndTime = endTime, FileId = fileId, FileName = fileName });
                await _dbRepository.SaveChangesAsync();

                BackgroundTaskNotification notification = new BackgroundTaskNotification
                {
                    Id = context.Id,
                    CreatedById = context.CreatedById,
                    StartTime = context.StartTime,
                    EndTime = context.EndTime,
                    TaskStatus = context.TaskStatus,
                    TaskType = context.TaskType,
                    Extension = context.Message,
                    ProgressBar = progressBar,
                    FileId = fileId,
                };
                await _backgroundTaskService.SendNotifiction(JsonConvert.SerializeObject(notification), context.CreatedById);
            }
            catch (Exception ex)
            {
                _logger.Error($"failed to execute background task agent OnCompleted, error:{ex}");
            }
        }

        public virtual async Task OnStop(BackgroundTaskContext context)
        {
            try
            {
                await _dbRepository.UpdateAsync<BackgroundTask>(t => t.Id == context.Id, t => new BackgroundTask { TaskStatus = BackgroundTaskStatus.Failed });
                await _dbRepository.SaveChangesAsync();
            }
            catch (Exception ex) { _logger.Error($"failed to stop task, error:{ex}"); }
        }

        protected virtual async Task UpdataProgressBar(decimal value)
        {
            value = Math.Round(value, 2);
            if (value != progressBar)
            {
                progressBar = value;
                try
                {
                    await _dbRepository.UpdateAsync<BackgroundTask>(t => t.Id == taskId, t => new BackgroundTask { ProgressBar = value, });
                    await _dbRepository.SaveChangesAsync();
                    await _eventBus.GuiCommonClient.SendAsync(EventJobs.BackgroundTaskNotification, new GUICommonBackgroundTaskNotificationEventContext
                    {
                        UserId = createdById,
                        Method = "receiveBackgroundTaskNotificationProgressBar",
                        Message = JsonConvert.SerializeObject(new
                        {
                            Id = taskId,
                            CreatedById = createdById,
                            TaskStatus = BackgroundTaskStatus.InProgress,
                            ProgressBar = value,
                        }, jsonSettings),
                    }, new Dictionary<string, string> { { "Authorization", _httpContextAccessor.HttpContext.Request.Headers["Authorization"] } });
                }
                catch (Exception e)
                {
                    _logger.Error($"update background Task ProgressBar failed, taskId: {taskId}, progressbar: {value}. Error: {e}");
                }
            }
        }
    }
}
