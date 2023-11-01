namespace AffiliateStoreBE.Common.Models
{
    public class BackgroundTaskContext
    {
        public Guid Id { get; set; }

        public Guid CreatedById { get; set; }


        public Type Type { get; set; }

        public string Param { get; set; }

        public string Message { get; set; }

        public ReportInfo ReportInfo { get; set; }

        public BackgroundTaskType TaskType { get; set; }

        public BackgroundTaskStatus TaskStatus { get; set; }

        public DateTimeOffset StartTime { get; set; }

        public DateTimeOffset EndTime { get; set; }

        public string Authorization { get; set; }
    }

    public class ReportInfo
    {
        public Stream Stream { get; set; }
        public string FileName { get; set; }
        public string TempFilePath { get; set; }
    }
}
