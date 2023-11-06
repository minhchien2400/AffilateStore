using AffiliateStoreBE.Common.Models;

namespace AffiliateStoreBE.Common
{
    public class SystemSettingEntity
    {
        [Serializable]
        public class DefaultFileControl : BaseEntity
        {
            public static readonly string Id = "Default Upload File Control";
            public string BlockedFileTypes { get; set; }

            public string FileTypes { get; set; }

            public int MaxUploadSizeInMb { get; set; }

            public List<string> GetFileTypes()
            {
                if (string.IsNullOrEmpty(FileTypes))
                {
                    return new List<string>();
                }
                return FileTypes.Trim().Split(';').Where(ft => !string.IsNullOrEmpty(ft)).ToList();
            }

            public List<string> GetBlockedFileTypes()
            {
                if (string.IsNullOrEmpty(BlockedFileTypes))
                {
                    return new List<string>();
                }
                return BlockedFileTypes.Trim().Split(';').Where(ft => !string.IsNullOrEmpty(ft)).ToList();
            }

            public static DefaultFileControl CreateDefault()
            {
                return new DefaultFileControl
                {
                    FileTypes = "DOC;DOCX;PPTX;PDF;XLS;XLSX;TXT;WPD;WPS;XML;HTML;MP3;MPA;WAV;WMA;AVI;FLV;M4V;MOV;MP4;MPG;WMV;SWF;3GP;GIF;JPG;PNG;BMP;ZIP;RAR;7Z;CSV;DAT;",
                    BlockedFileTypes = "ADE;ADP;BAS;BAT;CMD;COM;CPL;CRT;DLL;DO*;EXE;HTA;INF;INS;ISP;JS;JSE;LNK;MDB;MDE;MSC;MSI;MSP;MST;OCX;PCD;PIF;POT;PPT;REG;SCR;SCT;SHB;SHS;URL;VB;VBE;VBS;WSC;WSF;WSH;XL*;",
                    MaxUploadSizeInMb = 5120,
                };
            }
        }

    }
}
