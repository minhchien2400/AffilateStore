using System.ComponentModel;

namespace AffiliateStoreBE.Common.Models
{
    public class CommonConstants
    {
        public static readonly Int32 PeoplePickerBrowserPageSize = 10;
        public const string ReportStatusColumnName = "Import Status";
        public const string ReportMessageColumnName = "Comments";
        public const string TurnitinSigningSecret = "ab6e88e2-976b-41a4-96ce-be59b53e6b85";


        public class CDNSettings
        {
            public static readonly String CDNPrefix = "CDN_Prefix";
            public static readonly String ThirdParty = "Third_Party";
        }

        public class Turnitin
        {
            public static readonly String SignalrKey = "TurnitinPlagiarismSimilarity";
            public static readonly String TurnitinEventType = "X-Turnitin-EventType";
            public static readonly String TurnitinSignature = "X-Turnitin-Signature";
            public static readonly String UploadFileNameHtml = "{0} Question{1} anwser.html";
            public static readonly String UploadFileNameDocx = "{0} Question{1} anwser.docx";

            public static readonly String UploadFileURL = "{0}/api/v1/submissions/{1}/original";
            public static readonly String SubmissionInfoURL = "{0}/api/v1/submissions/{1}";
            public static readonly String GenerateSimilarityURL = "{0}/api/v1/submissions/{1}/similarity";
            public static readonly String EULAURL = "{0}/api/v1/eula/{1}/accept/{2}";
            public static readonly String AccepEULAURL = "{0}/api/v1/eula/{1}/accept";
            public static readonly String DeleteSubmissionURL = "{0}/api/v1/submissions/{1}/?hard=true";
            public static readonly String CreateSubmissionURL = "{0}/api/v1/submissions";
            public static readonly String VersionInfoURL = "{0}/api/v1/eula/latest?lang=en-US";
            public static readonly String ListWebHookURL = "{0}/api/v1/webhooks";
            public static readonly String WebHookURL = "{0}/api/turnitinwebhook";
        }



        public class AppSettingServiceName
        {
            public static readonly String ConfigDeployHostName = "DeployHost";
            public static readonly String ConfigRegionHostName = "RegionHost";
            public static readonly String VideoServerUrl = "VideoServerUrl";
            public static readonly String VideoAPPUrl = "VideoAPPUrl";
            public static readonly String VideoServerHost = "VideoServerHost";
            public static readonly String OnlyOnePageForLiveVideo = "OnlyOnePageForLiveVideo";
            public static readonly String CheckLiveVideoTime = "CheckLiveVideoTime";
            public static readonly String authHost = "authHost";
        }

        public class CommonWorkflowInfo
        {
            public static readonly String Name = "CommonWorkflow";
            public static readonly String StartEventName = "StartWorkflowStages";
            public static readonly String NextEventName = "ExecuteNextStep";

            public static readonly String AdminWorkflowId = "AdminWorkflowId";
            public static readonly String ScheduleWorkflowId = "ScheduleWorkflowId";
            public static readonly String AuthoringWorkflowId = "AuthoringWorkflowId";
            public static readonly String PrintingWorkflowId = "PrintingWorkflowId";
            public static readonly String TakingWorkflowId = "TakingWorkflowId";
            public static readonly String MarkingWorkflowId = "MarkingWorkflowId";
            public static readonly String ReportingWorkflowId = "ReportingWorkflowId";

            public static string GetWorkflowId(Module module)
            {
                string id = Name;
                switch (module)
                {
                    case Module.Admin:
                        id = AdminWorkflowId;
                        break;

                    case Module.Authoring:
                        id = AuthoringWorkflowId;
                        break;

                    case Module.Common:
                        id = Name;
                        break;

                    case Module.Marking:
                        id = MarkingWorkflowId;
                        break;

                    case Module.Reporting:
                        id = ReportingWorkflowId;
                        break;

                    case Module.Schedule:
                        id = ScheduleWorkflowId;
                        break;

                    case Module.Taking:
                        id = TakingWorkflowId;
                        break;
                }
                return id;
            }
        }

        public enum Module
        {
            [Description("/admin")]
            Admin,

            [Description("/schedule")]
            Schedule,

            [Description("/authoring")]
            Authoring,

            [Description("/taking")]
            Taking,

            [Description("/ExamApp")]
            ExamApp,

            [Description("/marking")]
            Marking,

            [Description("/reporting")]
            Reporting,

            [Description("/common")]
            Common,

            [Description("/")]
            //[Description("/account")]
            Account,

            [Description("/ecms")]
            Ecms,

            [Description("/timer")]
            Timer,

            [Description("/facerecognizer")]
            FaceRecognizer,

            [Description("/preview")]
            Aspose,

            [Description("/database")]
            Database,

            [Description("/face")]
            Face,

            [Description("/dbsetup")]
            DBSetup,

            [Description("/publicAPI")]
            PublicAPI,
        }
    }
}
