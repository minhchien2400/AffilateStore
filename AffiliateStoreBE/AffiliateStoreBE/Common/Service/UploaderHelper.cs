using static AffiliateStoreBE.Common.Service.SystemSettingEntity;

namespace AffiliateStoreBE.Common.Service
{
    public class UploaderHelper
    {
        public class TempFileInfo
        {
            public string FilePath { get; set; }
            public string FolderPath { get; set; }
        }
        public enum UploadStatus
        {
            UploadSuccessful = 0,
            ValidateSuccessful = 1,
            SizeNotAllowd = 2,
            TypeNotAllowd = 3,
            Illegalchar = 4,
            IllegalFileNameLength = 5,
            TypeNotRealAllowd = 6,
            FileNameMAx = 7,
            LcmsSiteNotReady = 8
        }

        public static readonly List<string> WhiteFileTypes = new List<string>() { "doc", "docx", "xls", "xlsx" };
        public static readonly int MaxFileNameLength = 92;

        public static readonly Dictionary<int, string> FileTypeMappings = new Dictionary<int, string>()
        {
            { 3780, "PDF" },
            { 6677, "BMP" },
            { 7173, "GIF" },
            { 7373, "TIF;TIFF" },
            { 7790, "COM;EXE;DLL" },
            { 60104, "XLS" },
            { 6033, "DOC" },
            { 8075, "XLSX;ZIP;DOCX;PPTX;EGR" },
            { 8297, "RAR" },
           // { 8297, "EGR" },
            { 13780, "PNG" },
            { 55122, "7Z" },
            { 208207, "XLS;DOC;MSG" },
            { 255216, "JPG;JPEG;JPE" },
            { 9999999, "VALIDFILE" },

            //merge from rpg
            //{ 6063, "XML" },
            //{ 6787, "SWF" }},
            //{ 117115, "CS" }},
            //{ 239187, "ASPX" }},

            //search from internet
            //6063-XML
            //6033-HTML
            //239187-ASPX
            //117115-CS
            //119105-JS
            //210187-TXT
            //255254-SQL,RDP
            //64101-BAT
            //10056-BTSEED
            //5666-PSD
            //3780-PDF
            //7384-CHM
            //70105-LOG
            //8269-REG
            //6395-HLP

            //test by real file
            //239187 ASPX;CONFIG;CS;HTML;JS;LOG;XML
            //64101 BAT
            //255254 SQL
            //115115 TXT
        };

        #region Validation Before Upload File

        /// <summary>
        /// 上传文件之前需要验证文件。
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="fileStream"></param>
        /// <param name="fileControl"></param>
        /// <returns></returns>
        public static UploadStatus ValidationUploadFile(string fileName, Stream fileStream, DefaultFileControl fileControl, bool hasCustomWhiteList = false)
        {
            try
            {
                var result = CheckFileName(fileName);
                if (result == UploadStatus.ValidateSuccessful)
                {
                    var fileType = Path.GetExtension(fileName).ToLower();
                    if (fileType.Length < 2)
                    {
                        return UploadStatus.TypeNotAllowd;
                    }
                    fileType = fileType.Substring(1);
                    result = CheckFileTypeSize(fileType, fileStream.Length, fileControl, hasCustomWhiteList);
                    if (result == UploadStatus.ValidateSuccessful)
                    {
                        result = CheckFileNotRealType(fileStream, fileType);
                    }
                }
                return result;
            }
            catch (Exception e)
            {
                throw;
            }
        }

        /// <summary>
        /// 上传文件之前需要验证文件。
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="fileStream"></param>
        /// <param name="fileControl"></param>
        /// <returns></returns>
        public static UploadStatus GUICommonWebValidationUploadFile(string fileName, Stream fileStream, Stream headerOutStream, DefaultFileControl fileControl, bool hasCustomWhiteList = false)
        {
            try
            {
                var result = CheckFileName(fileName);
                if (result == UploadStatus.ValidateSuccessful)
                {
                    var fileType = Path.GetExtension(fileName).ToLower();
                    if (fileType.Length < 2)
                    {
                        return UploadStatus.TypeNotAllowd;
                    }
                    fileType = fileType.Substring(1);
                    result = CheckFileTypeSize(fileType, fileStream.Length, fileControl, hasCustomWhiteList);
                    if (result == UploadStatus.ValidateSuccessful)
                    {
                        result = GuiCommonWebCheckFileNotRealType(fileStream, headerOutStream, fileType);
                    }
                }
                return result;
            }
            catch (Exception e)
            {
                throw;
            }
        }

        /// <summary>
        /// 验证文件后缀名是否被更改，目前只支持部分后缀名，参考FileTypeMappings属性。
        /// </summary>
        /// <param name="fs"></param>
        /// <param name="fileType"></param>
        /// <returns></returns>
        private static UploadStatus GuiCommonWebCheckFileNotRealType(Stream fs, Stream headerOutStream, string fileType)
        {
            var fileTypeCodeList = new List<int>();
            foreach (var item in FileTypeMappings)
            {
                var ls = item.Value.Split(';');
                if (ls.Contains(fileType, StringComparer.OrdinalIgnoreCase))
                {
                    fileTypeCodeList.Add(item.Key);
                }
            }
            if (fileTypeCodeList.Count == 0)
            {
                return UploadStatus.ValidateSuccessful;
            }

            try
            {
                var buffer = new byte[2];
                var result = fs.Read(buffer, 0, buffer.Length);
                if (result == 0)
                {
                    return UploadStatus.ValidateSuccessful;
                }
                headerOutStream.Write(buffer, 0, result);
                BinaryReader br = new BinaryReader(headerOutStream);
                string typeStr = string.Empty;
                headerOutStream.Position = 0;

                byte data = br.ReadByte();
                typeStr += data.ToString();

                data = br.ReadByte();
                typeStr += data.ToString();

                int typeCode = 0;
                if (int.TryParse(typeStr, out typeCode) && !fileTypeCodeList.Contains(typeCode))
                {
                    return UploadStatus.TypeNotRealAllowd;
                }
                return UploadStatus.ValidateSuccessful;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                headerOutStream.Position = 0;
                //if (fs != null)
                //{
                //    br.Close();
                //}
            }
        }

        /// <summary>
        /// 验证文件名字非空，非法字符，最长92字符。
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        private static UploadStatus CheckFileName(string fileName)
        {
            if (string.IsNullOrEmpty(fileName))
            {
                return UploadStatus.FileNameMAx;
            }
            if (fileName.Length > MaxFileNameLength)
            {
                return UploadStatus.FileNameMAx;
            }
            var r = new System.Text.RegularExpressions.Regex("[/\\\"?<>#%]"); //[\\/:*?\"<>|#{}%~&+]");
            if (r.IsMatch(fileName))
            {
                return UploadStatus.Illegalchar;
            }
            return UploadStatus.ValidateSuccessful;
        }

        /// <summary>
        /// 验证文件大小，文件后缀名黑名单，白名单。
        /// </summary>
        /// <param name="fileType"></param>
        /// <param name="fileSize"></param>
        /// <param name="fileControl"></param>
        /// <returns></returns>
        private static UploadStatus CheckFileTypeSize(string fileType, long fileSize, DefaultFileControl fileControl, bool hasCustomWhiteList)
        {
            if (fileControl != null && fileControl.MaxUploadSizeInMb > 0)
            {
                decimal maxSize = Math.Round((Convert.ToDecimal(fileControl.MaxUploadSizeInMb) * 1024 * 1024), MidpointRounding.AwayFromZero);
                if (fileSize > maxSize)
                {
                    return UploadStatus.SizeNotAllowd;
                }
            }

            if (fileControl != null)
            {
                var flag = CheckFileType(fileType, WhiteFileTypes);
                if (!flag)
                {
                    flag = CheckFileType(fileType, fileControl.GetBlockedFileTypes());
                    if (flag) { return UploadStatus.TypeNotAllowd; }
                }
                flag = hasCustomWhiteList || CheckFileType(fileType, fileControl.GetFileTypes());
                if (!flag) { return UploadStatus.TypeNotAllowd; }
            }
            return UploadStatus.ValidateSuccessful;
        }

        private static bool CheckFileType(string fileType, List<string> fileTypeList)
        {
            foreach (var item in fileTypeList)
            {
                var t = item.ToLower();
                if (t.IndexOf('*') >= 0)
                {
                    t = t.Replace("*", ".*");

                    var r = new System.Text.RegularExpressions.Regex("/^" + t + "$/");
                    if (r.IsMatch(fileType))
                    {
                        return true;
                    }
                }
                else if (t.EqualsIgnoreCase(fileType))
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// 验证文件后缀名是否被更改，目前只支持部分后缀名，参考FileTypeMappings属性。
        /// </summary>
        /// <param name="fs"></param>
        /// <param name="fileType"></param>
        /// <returns></returns>
        private static UploadStatus CheckFileNotRealType(Stream fs, string fileType)
        {
            if (fs.Length == 0)
            {
                return UploadStatus.ValidateSuccessful;
            }
            fs.Position = 0;
            var fileTypeCodeList = new List<int>();
            foreach (var item in FileTypeMappings)
            {
                var ls = item.Value.Split(';');
                if (ls.Contains(fileType, StringComparer.OrdinalIgnoreCase))
                {
                    fileTypeCodeList.Add(item.Key);
                }
            }
            if (fileTypeCodeList.Count == 0)
            {
                return UploadStatus.ValidateSuccessful;
            }

            BinaryReader br = new BinaryReader(fs);
            string typeStr = string.Empty;
            try
            {
                fs.Position = 0;

                byte data = br.ReadByte();
                typeStr += data.ToString();

                data = br.ReadByte();
                typeStr += data.ToString();

                int typeCode = 0;
                if (int.TryParse(typeStr, out typeCode) && !fileTypeCodeList.Contains(typeCode))
                {
                    return UploadStatus.TypeNotRealAllowd;
                }
                return UploadStatus.ValidateSuccessful;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                fs.Position = 0;
                //if (fs != null)
                //{
                //    br.Close();
                //}
            }
        }

        #endregion Validation Before Upload File

        #region Copy file
        public static byte[] GetBytes(IFormFile file)
        {
            if (file != null)
            {
                using (var ms = new MemoryStream())
                {
                    file.CopyTo(ms);
                    return ms.ToArray();
                }
            }
            return null;
        }
        #endregion
    }
}
