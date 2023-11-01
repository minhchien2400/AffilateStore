namespace AffiliateStoreBE.Common.Models
{
    public class BackgroundTask : BaseEntity
    {
        public Guid Id { get; set; }
        public DateTimeOffset StartTime { get; set; }
        public DateTimeOffset EndTime { get; set; }
        public BackgroundTaskType TaskType { get; set; }
        public BackgroundTaskStatus TaskStatus { get; set; }
        public string Parameters { get; set; }
        public string Message { get; set; }
        public DateTimeOffset Heartbeat { get; set; }
        public Guid? FileId { get; set; }
        public string FileName { get; set; }
        public Boolean IsDeleted { get; set; }
        public decimal? ProgressBar { get; set; }
    }

    public enum BackgroundTaskStatus
    {
        None,
        InProgress,
        Succeed,
        Failed,
        FailedWithException
    }

    public enum BackgroundTaskType
    {
        Grading_DownloadAllPDF,
        Grading_DownloadSelectedPDF,
        Grading_PublishToLMS,
        Grading_DownloadSinglePDF,
        Exam_DownloadAllPDF,
        Exam_DownloadSelectedPDF,
        Exam_DownloadSinglePDF,
        ImportExamExcel,
        ImportCourseExcel,
        UploadPhotosBatch,
        ImportOrganization,
        SyncCandidateToUnfinished,
        ExportAllCourse
    }
}
