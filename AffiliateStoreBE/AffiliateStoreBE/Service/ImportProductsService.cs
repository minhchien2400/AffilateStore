using AffiliateStoreBE.Common.Models;
using AffiliateStoreBE.Common.Service;
using AffiliateStoreBE.Models;
using Microsoft.AspNetCore.Cors.Infrastructure;
using Microsoft.OpenApi.Writers;
using Microsoft.VisualBasic;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Text;
using Volo.Abp;
using DocumentFormat.OpenXml.Spreadsheet;
using AffiliateStoreBE.Controllers;

namespace AffiliateStoreBE.Service
{
    public class ImportProductsService
    {
        private string _loggerSuffix = string.Empty;
        private decimal _miniProgressBar = 0.89m;
        private decimal _miniUploadStreamProgressBar = 0.95m;
        //private readonly ICustomDistributedCache _distributedCache;
        private Guid _currentUserId;
        private string sheetName_Candidates;
        private bool enableSemester = false;
        private int _totalDataRow = 0;
        private int _currentDataRow = 0;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private List<ProductSheetModel> _excelProduct;
        private List<ImageSheetModel> _excelImage;
        private string sheetName_Product = "";
        private string sheetName_Image = "";
        public ImportProductsService(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<bool> ImportProductExcel(ImportPathInfo request)
        {
            try
            {

               // validate here

                var originalSheets = new Dictionary<string, Worksheet>();


                using (var fs = new MemoryStream(request.ImportFileBytes))
                {
                    ReadExcel(fs, request.Language);

                    if (_excelProduct == null || _excelImage == null)
                    {
                        return false;
                    }

                    await InitData();

                    await CourseSheet();
                    await ClassSheet();
                    await CandidateSheet();

                    await GenerateReport(workbook, report, originalSheets, request.Language);
                }
                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private bool ValidateFileFormat(string filePath)
        {
            var extension = Path.GetExtension(filePath);
            if (!".xlsx".EqualsIgnoreCase(extension))
            {
                return false;
            }
            return true;
        }

        private async Task CourseSheet()
        {
            if (_excelCourse.Any())
            {
                await ValidateCoursesAsync();
                await CheckCourseChange();
                await UpdateCoursesAsync();
            }
        }
        private async Task ClassSheet()
        {
            if (_excelClasses.Any())
            {
                await ValidateClassesAsync();
                await CheckClassesChange();
                await UpdateClassesAsync();
                logger.Info($"importing the excel to import course Class Sheet end");
            }
        }
       

        private void ReadExcel(Stream stream, String language)
        {
            _excelProduct = ExcelHelper.ReadExcel<ProductSheetModel>(stream, sheetName_Product);
            _excelImage = ExcelHelper.ReadExcel<ImageSheetModel>(stream, sheetName_Image, dynamicColumnNames: structureNames);
        }

        private async Task InitData()
        {
            logger.Info($"importing the excel to import course InitData start");
            var users = _dbRepository.Context.Set<User>().Where(a => a.UserStatus == UserStatus.Active);
            var staffs = await users.Where(UserTypeExpression.StaffFilterIncludeGroup).Select(a => new { a.Username, a.Id }).ToListAsync();
            foreach (var staff in staffs)
            {
                _staffs[staff.Username.ToLower()] = staff.Id;
            }
            var staffEmails = await users.Where(UserTypeExpression.StaffGroupFilter).Where(a => !string.IsNullOrEmpty(a.Username)).Select(a => new { a.Username, a.Id }).ToListAsync();
            foreach (var staffEmail in staffEmails)
            {
                _staffsEmailGroup[staffEmail.Username.ToLower()] = staffEmail.Id;
            }
            var staffNames = await users.Where(UserTypeExpression.StaffGroupFilter).Select(a => new { a.DisplayName, a.Id }).ToListAsync();
            foreach (var staffName in staffNames)
            {
                _staffsNameGroup[staffName.DisplayName.ToLower()] = staffName.Id;
            }

            var candidates = await users.Where(UserTypeExpression.CandidateFilterIncludeGroup).Select(a => new { a.Username, a.Id, a.Type }).ToListAsync();
            foreach (var candidate in candidates)
            {
                _candidates[candidate.Username.ToLower()] = candidate.Id;
            }
            var candidatesEmails = await users.Where(UserTypeExpression.CandidateGroupFilter).Where(a => !string.IsNullOrEmpty(a.Username)).Select(a => new { a.Username, a.Id }).ToListAsync();
            foreach (var candidatesEmail in candidatesEmails)
            {
                _candidatesEmailGroup[candidatesEmail.Username.ToLower()] = candidatesEmail.Id;
            }
            var candidateNames = await users.Where(UserTypeExpression.CandidateGroupFilter).Select(a => new { a.DisplayName, a.Id }).ToListAsync();
            foreach (var candidateName in candidateNames)
            {
                _candidatesNameGroup[candidateName.DisplayName.ToLower()] = candidateName.Id;
            }

            var candidatesGroupMembers = await _dbRepository.Context.Set<AvePoint.Confucius.FeatureCommon.Domain.Common.Group>()
                .Where(a => _candidatesEmailGroup.Values.Contains(a.Id) || _candidatesNameGroup.Values.Contains(a.Id))
                .Select(b => new { b.Id, Members = b.Member.Where(a => a.User.Type.HasFlag(UserType.Candidate)).ToList<GroupMember>() }).ToListAsync();
            foreach (var candidatesGroupMember in candidatesGroupMembers)
            {
                _candidatesGroupMembers[candidatesGroupMember.Id] = candidatesGroupMember.Members;
            }

            var staffsGroupMembers = await _dbRepository.Context.Set<AvePoint.Confucius.FeatureCommon.Domain.Common.Group>()
                .Where(a => _staffsEmailGroup.Values.Contains(a.Id) || _staffsNameGroup.Values.Contains(a.Id))
                .Select(b => new { b.Id, Memebers = b.Member.Where(a => a.User.Type.HasFlag(UserType.Staff)).ToList<GroupMember>() }).ToListAsync();
            foreach (var staffsGroupMember in staffsGroupMembers)
            {
                _staffsGroupMembers[staffsGroupMember.Id] = staffsGroupMember.Memebers;
            }
            _organization = await _dbRepository.Context.Set<Organization>().Where(a => a.Status == BuildInStatus.NotBuildIn).Include(a => a.DataStructure).ToListAsync();
            _trunkOrganization = _organization.Where(a => a.ParentId.HasValue == false).ToList();
            foreach (var item in _trunkOrganization)
            {
                _organizationTree.Add(GetOrganizationTree(item, _organization));
            }

            var semesters = await _dbRepository.Context.Set<Semester>().Where(a => a.DeleteStatus == DeleteStatus.Ok && a.Status == BuildInStatus.NotBuildIn).Select(a => new { a.Name, a.Id }).ToListAsync();
            foreach (var semester in semesters)
            {
                _semesters[semester.Name.ToLower()] = semester.Id;
            }
        }

        private async Task ValidateCoursesAsync()
        {
            var courseCodes = _excelCourse.Select(a => a.Code.ToLower()).ToList();
            var organizations = await _dbRepository.Context.Set<Course>()
                    .Include(a => a.CourseOrganizations).ThenInclude(a => a.Organization)
                    .Where(a => courseCodes.Contains(a.Code.ToLower()) && a.DeleteStatus == DeleteStatus.Ok).ToListAsync();
            foreach (var course in _excelCourse)
            {
                var organization = organizations.Where(a => course.Code.ToLower() == a.Code.ToLower()).Select(a => a.CourseOrganizations.ToList()).FirstOrDefault();
                if (NeedValidateOrganization(organization, course.DynamicColumns))
                {
                    ValidateOrganization(course);
                }
                else
                {
                    course.NeedVerify = true;
                    List<string> valueList = new List<string>();
                    foreach (var item in course.DynamicColumns)
                    {
                        valueList.Add(item.Value);
                    }
                    if (valueList != null && valueList.Count > 0)
                    {
                        course.DynamicColumnValueStr = String.Join("/", valueList);
                    }
                }
                if (course.ReportStatus != ImportStatus.Failed)
                {
                    #region Code
                    if (string.IsNullOrEmpty(course.Code))
                    {
                        course.ReportStatus = ImportStatus.Failed;
                        course.InvalidMessage.Add(I18NEntity.GetString("Admin_ImportCourse_Column_Empty_Message", I18NEntity.GetString("AD_CourseManagement_Edit_CourseCode_Entry")));
                    }
                    else if (course.Code.Length > 32)
                    {
                        course.ReportStatus = ImportStatus.Failed;
                        course.InvalidMessage.Add(I18NEntity.GetString("Admin_ImportCourse_Column_MoreThan_Message", new[] { I18NEntity.GetString("AD_CourseManagement_Edit_CourseCode_Entry"), "32" }));
                    }

                    #endregion Code

                    #region Semester

                    if (enableSemester && string.IsNullOrEmpty(course.Semester))
                    {
                        course.ReportStatus = ImportStatus.Failed;
                        course.InvalidMessage.Add(I18NEntity.GetString("Admin_ImportCourse_Column_Empty_Message", I18NEntity.GetString("AD_CourseManagement_Edit_Semester_Entry")));
                    }
                    else if (enableSemester && !_semesters.ContainsKey(course.Semester.ToLower()))
                    {
                        course.ReportStatus = ImportStatus.Failed;
                        course.InvalidMessage.Add(I18NEntity.GetString("Admin_ImportCourseClass_Semester_MissMatch_Message", course.Semester));
                    }

                    #endregion

                    #region Name

                    if (string.IsNullOrEmpty(course.Name))
                    {
                        course.ReportStatus = ImportStatus.Failed;
                        course.InvalidMessage.Add(I18NEntity.GetString("Admin_ImportCourse_Column_Empty_Message", I18NEntity.GetString("AD_CourseManagement_Edit_CourseName_Entry")));
                    }
                    else if (course.Name.Length > 128)
                    {
                        course.ReportStatus = ImportStatus.Failed;
                        course.InvalidMessage.Add(I18NEntity.GetString("Admin_ImportCourse_Column_MoreThan_Message", new[] { I18NEntity.GetString("AD_CourseManagement_Edit_CourseName_Entry"), "128" }));
                    }

                    #endregion Name

                    #region Manager

                    if (string.IsNullOrEmpty(course.ManagerEmail))
                    {
                        course.ReportStatus = ImportStatus.Failed;
                        course.InvalidMessage.Add(I18NEntity.GetString("Admin_ImportCourse_Column_Empty_Message", I18NEntity.GetString("AD_CourseManagement_Edit_CourseManager_Entry")));
                    }
                    else
                    {
                        var listManagers = course.ListManagerEmails;
                        var listErrorCM = new List<string>();

                        foreach (var manager in listManagers)
                        {
                            if (!IsInGroup(manager, _staffsNameGroup) &&
                                !IsInGroup(manager.Trim(), _staffsEmailGroup) &&
                                !IsInGroup(manager.Trim(), _staffs))
                            {
                                if (!string.IsNullOrEmpty(manager))
                                {
                                    listErrorCM.Add(manager);
                                }
                            }
                        }
                        if (listErrorCM.Count > 1)
                        {
                            var inputEmail = string.Join("; ", listErrorCM);
                            course.ReportStatus = ImportStatus.Failed;
                            course.InvalidMessage.Add(I18NEntity.GetString("Admin_ImportCourse_EmailFormat_OrGroupEmail_Invalid_Message", new[] { inputEmail, I18NEntity.GetString("AD_CourseManagement_Edit_CourseManager_Entry"), I18NEntity.GetString("GC_ProfileInner_Staff_Entry").ToString().ToLower() }));
                        }
                        else if (listErrorCM.Count == 1)
                        {
                            course.ReportStatus = ImportStatus.Failed;
                            course.InvalidMessage.Add(I18NEntity.GetString("Admin_ImportCourse_Single_EmailFormat_OrGroupEmail_Invalid_Message", new[] { listErrorCM[0], I18NEntity.GetString("AD_CourseManagement_Edit_CourseManager_Entry"), I18NEntity.GetString("GC_ProfileInner_Staff_Entry").ToString().ToLower() }));
                        }
                    }

                    #endregion Manager

                    #region Co-Manager

                    if (!string.IsNullOrEmpty(course.CoManagers))
                    {
                        var allCoCMList = course.CoManagers.Split(new[] { ';', '；' }, StringSplitOptions.RemoveEmptyEntries).Select(i => i.Trim()).ToList();
                        var errorCoCMList = new List<string>();
                        var coManagers = course.CoManagerEmails;
                        var cmList = course.ListManagerEmails;
                        foreach (var coManager in coManagers)
                        {
                            if (!IsInGroup(coManager, _staffsNameGroup) &&
                                !IsInGroup(coManager.Trim(), _staffsEmailGroup) &&
                                !IsInGroup(coManager.Trim(), _staffs))
                            {
                                var errorEmail = allCoCMList.FirstOrDefault(i => i.Equals(coManager, StringComparison.OrdinalIgnoreCase));
                                if (!string.IsNullOrEmpty(errorEmail))
                                {
                                    errorCoCMList.Add(errorEmail);
                                }
                            }

                            //if (IsEmail(coManager) && cmList.Contains(coManager, StringComparer.OrdinalIgnoreCase))
                            //{
                            //    course.ReportStatus = ImportStatus.Failed;
                            //    course.InvalidMessage.Add(I18NEntity.GetString("Admin_ImportCourse_ManagerAndCoManager_NotSame_Message"));
                            //}
                        }
                        if (errorCoCMList.Count > 1)
                        {
                            var inputEmail = string.Join("; ", errorCoCMList);
                            course.ReportStatus = ImportStatus.Failed;
                            course.InvalidMessage.Add(I18NEntity.GetString("Admin_ImportCourse_EmailFormat_OrGroupEmail_Invalid_Message", new[] { inputEmail, I18NEntity.GetString("AD_CourseManagement_Edit_CoManager_Entry"), I18NEntity.GetString("GC_ProfileInner_Staff_Entry").ToString().ToLower() }));
                        }
                        else if (errorCoCMList.Count == 1)
                        {
                            course.ReportStatus = ImportStatus.Failed;
                            course.InvalidMessage.Add(I18NEntity.GetString("Admin_ImportCourse_Single_EmailFormat_OrGroupEmail_Invalid_Message", new[] { errorCoCMList[0], I18NEntity.GetString("AD_CourseManagement_Edit_CoManager_Entry"), I18NEntity.GetString("GC_ProfileInner_Staff_Entry").ToString().ToLower() }));
                        }
                    }

                    #endregion Co-Manager

                    #region PaperCrafter

                    if (!string.IsNullOrEmpty(course.PaperCrafers))
                    {
                        var allCrafterList = course.PaperCrafers.Split(new[] { ';', '；' }, StringSplitOptions.RemoveEmptyEntries).Select(i => i.Trim()).ToList();
                        var errorCrafterList = new List<string>();
                        var paperCrafters = course.PaperCrafterEmails;
                        foreach (var paperCrafter in paperCrafters)
                        {
                            if (!IsInGroup(paperCrafter, _staffsNameGroup) &&
                                !IsInGroup(paperCrafter.Trim(), _staffsEmailGroup) &&
                                !IsInGroup(paperCrafter.Trim(), _staffs))
                            {
                                var errorEmail = allCrafterList.FirstOrDefault(i => i.Equals(paperCrafter, StringComparison.OrdinalIgnoreCase));
                                if (!string.IsNullOrEmpty(errorEmail))
                                {
                                    errorCrafterList.Add(errorEmail);
                                }
                            }
                        }
                        if (errorCrafterList.Count > 1)
                        {
                            var inputEmail = string.Join("; ", errorCrafterList);
                            course.ReportStatus = ImportStatus.Failed;
                            course.InvalidMessage.Add(I18NEntity.GetString("Admin_ImportCourse_EmailFormat_OrGroupEmail_Invalid_Message", new[] { inputEmail, I18NEntity.GetString("AD_CourseManagement_Edit_PaperCrafter_Entry"), I18NEntity.GetString("GC_ProfileInner_Staff_Entry").ToString().ToLower() }));
                        }
                        else if (errorCrafterList.Count == 1)
                        {
                            course.ReportStatus = ImportStatus.Failed;
                            course.InvalidMessage.Add(I18NEntity.GetString("Admin_ImportCourse_Single_EmailFormat_OrGroupEmail_Invalid_Message", new[] { errorCrafterList[0], I18NEntity.GetString("AD_CourseManagement_Edit_PaperCrafter_Entry"), I18NEntity.GetString("GC_ProfileInner_Staff_Entry").ToString().ToLower() }));
                        }
                    }

                    #endregion PaperCrafter
                }
            }
        }

        private async Task ValidateClassesAsync()
        {
            var courseCodes = _excelClasses.Select(a => a.Code.ToLower()).ToList();
            var organizations = await _dbRepository.Context.Set<Course>()
                    .Include(a => a.CourseOrganizations).ThenInclude(a => a.Organization)
                    .Where(a => courseCodes.Contains(a.Code.ToLower()) && a.DeleteStatus == DeleteStatus.Ok).ToListAsync();
            foreach (var excelClass in _excelClasses)
            {
                var organization = organizations.Where(a => excelClass.Code.ToLower() == a.Code.ToLower()).Select(a => a.CourseOrganizations.ToList()).FirstOrDefault();
                if (NeedValidateOrganization(organization, excelClass.DynamicColumns))
                {
                    ValidateOrganization(excelClass);
                }
                else
                {
                    List<string> valueList = new List<string>();
                    foreach (var item in excelClass.DynamicColumns)
                    {
                        valueList.Add(item.Value);
                    }
                    if (valueList != null && valueList.Count > 0)
                    {
                        excelClass.DynamicColumnValueStr = String.Join("/", valueList);
                    }
                }
                if (excelClass.ReportStatus != ImportStatus.Failed)
                {

                    #region Code
                    if (string.IsNullOrEmpty(excelClass.Code))
                    {
                        excelClass.ReportStatus = ImportStatus.Failed;
                        excelClass.InvalidMessage.Add(I18NEntity.GetString("Admin_ImportCourse_Column_Empty_Message", I18NEntity.GetString("AD_CourseManagement_Edit_CourseCode_Entry")));
                    }
                    else if (excelClass.Code.Length > 32)
                    {
                        excelClass.ReportStatus = ImportStatus.Failed;
                        excelClass.InvalidMessage.Add(I18NEntity.GetString("Admin_ImportCourse_Column_MoreThan_Message", new[] { I18NEntity.GetString("AD_CourseManagement_Edit_CourseCode_Entry"), "32" }));
                    }

                    #endregion Code

                    #region Semester
                    if (enableSemester && string.IsNullOrEmpty(excelClass.Semester))
                    {
                        excelClass.ReportStatus = ImportStatus.Failed;
                        excelClass.InvalidMessage.Add(I18NEntity.GetString("Admin_ImportCourse_Column_Empty_Message", I18NEntity.GetString("AD_CourseManagement_Edit_Semester_Entry")));
                    }
                    else if (enableSemester && !_semesters.ContainsKey(excelClass.Semester.ToLower()))
                    {
                        excelClass.ReportStatus = ImportStatus.Failed;
                        excelClass.InvalidMessage.Add(I18NEntity.GetString("Admin_ImportCourseClass_Semester_MissMatch_Message", excelClass.Semester));
                    }
                    #endregion

                    #region Name

                    else if (string.IsNullOrEmpty(excelClass.ClassName))
                    {
                        excelClass.ReportStatus = ImportStatus.Failed;
                        excelClass.InvalidMessage.Add(I18NEntity.GetString("Admin_ImportCourse_Column_Empty_Message", I18NEntity.GetString("AD_CourseManagement_Export_ClassName_Entry")));
                    }

                    #endregion Name

                    #region Owner

                    else if (string.IsNullOrEmpty(excelClass.Owners))
                    {
                        excelClass.ReportStatus = ImportStatus.Failed;
                        excelClass.InvalidMessage.Add(I18NEntity.GetString("Admin_ImportCourse_Column_Empty_Message", I18NEntity.GetString("AD_CourseManagement_Export_CourseOwnersName_Entry")));
                    }
                    else
                    {
                        var allOwnerList = excelClass.Owners.Split(new[] { ';', '；' }, StringSplitOptions.RemoveEmptyEntries).Select(i => i.Trim()).ToList();
                        var errorOwnerList = new List<string>();
                        var owners = excelClass.OwnerEmails;
                        foreach (var owner in owners)
                        {
                            if (!IsInGroup(owner, _staffsNameGroup) &&
                                !IsInGroup(owner.Trim(), _staffsEmailGroup) &&
                                !IsInGroup(owner.Trim(), _staffs))
                            {
                                var errorEmail = allOwnerList.FirstOrDefault(i => i.Equals(owner, StringComparison.OrdinalIgnoreCase));
                                if (!string.IsNullOrEmpty(errorEmail))
                                {
                                    errorOwnerList.Add(errorEmail);
                                }
                            }
                        }
                        if (errorOwnerList.Count > 1)
                        {
                            var inputEmail = string.Join("; ", errorOwnerList);
                            excelClass.ReportStatus = ImportStatus.Failed;
                            excelClass.InvalidMessage.Add(I18NEntity.GetString("Admin_ImportCourse_EmailFormat_OrGroupEmail_Invalid_Message", new[] { inputEmail, I18NEntity.GetString("AD_CourseManagement_Create_Class_Owner_Entry"), I18NEntity.GetString("GC_ProfileInner_Staff_Entry").ToString().ToLower() }));
                        }
                        else if (errorOwnerList.Count == 1)
                        {
                            excelClass.ReportStatus = ImportStatus.Failed;
                            excelClass.InvalidMessage.Add(I18NEntity.GetString("Admin_ImportCourse_Single_EmailFormat_OrGroupEmail_Invalid_Message", new[] { errorOwnerList[0], I18NEntity.GetString("AD_CourseManagement_Create_Class_Owner_Entry"), I18NEntity.GetString("GC_ProfileInner_Staff_Entry").ToString().ToLower() }));
                        }
                    }
                    #endregion Owner
                }
            }
        }

        private bool IsInGroup(string groupInfo, Dictionary<string, Guid> userInfos)
        {
            bool result = false;
            if (userInfos.IsNullOrEmpty())
            {
                return result;
            }

            if (userInfos.Keys.Any(a => a.Equals(groupInfo, StringComparison.OrdinalIgnoreCase)))
            {
                result = true;
            }
            return result;
        }

        private void ProcessDuplicatedClass(List<ClassExcelModel> classList)
        {
            for (var i = 0; i < classList.Count - 1; i++)
            {
                if (classList[i].ReportStatus == ImportStatus.Skip)
                {
                    continue;
                }
                for (var j = i + 1; j < classList.Count; j++)
                {
                    if (classList[j].ReportStatus == ImportStatus.Skip)
                    {
                        continue;
                    }
                    if (classList[i].Equals(classList[j]))
                    {
                        classList[j].ReportStatus = ImportStatus.Skip;
                        classList[j].SkipMessage = _skipMessage;
                    }
                }
            }
        }
        private void ProcessDuplicatedCourse(List<CourseExcelModel> courseList)
        {
            for (var i = 0; i < courseList.Count - 1; i++)
            {
                if (courseList[i].ReportStatus == ImportStatus.Skip)
                {
                    continue;
                }
                for (var j = i + 1; j < courseList.Count; j++)
                {
                    if (courseList[j].ReportStatus == ImportStatus.Skip)
                    {
                        continue;
                    }
                    if (courseList[i].Equals(courseList[j]))
                    {
                        courseList[j].ReportStatus = ImportStatus.Skip;
                        courseList[j].SkipMessage = _skipMessage;
                    }
                }
            }
        }

        private async Task CheckCourseChange()
        {
            var validCourses = _excelCourse.Where(a => a.ReportStatus != ImportStatus.Failed);

            if (validCourses.Any())
            {
                ProcessDuplicatedCourse(validCourses.ToList());
                var courseCodes = validCourses.Select(a => a.Code.ToLower()).Distinct().ToList();
                var courses = await _dbRepository.Context.Set<Course>()
                    .Include(a => a.CourseOrganizations).ThenInclude(a => a.Organization).ThenInclude(a => a.DataStructure)
                    .Include(a => a.Semester)
                    .Where(a => courseCodes.Contains(a.Code.ToLower())
                        && a.DeleteStatus == DeleteStatus.Ok)
                    .Select(a => new
                    {
                        a.Id,
                        a.Code,
                        a.Name,
                        SemesterName = a.Semester.Name,
                        CourseOrganizationPath = a.CourseOrganizationPath,
                    })
                    .ToListAsync();
                var courseIdList = courses.Select(c => c.Id).ToList();
                var courseManagerLists = await _dbRepository.Context.Set<CourseManager>().Where(c => courseIdList.Contains(c.CourseId)).Select(s =>
                    new CourseManager
                    {
                        CourseId = s.CourseId,
                        ManagerId = s.ManagerId
                    }).ToListAsync();
                var courseCoManagerLists = await _dbRepository.Context.Set<CourseCoManager>().Where(c => courseIdList.Contains(c.CourseId)).Select(s =>
                    new CourseCoManager
                    {
                        CourseId = s.CourseId,
                        CoManagerId = s.CoManagerId
                    }).ToListAsync();
                var coursePaperCrafterLists = await _dbRepository.Context.Set<CoursePaperCrafter>().Where(c => courseIdList.Contains(c.CourseId)).Select(s =>
                    new CoursePaperCrafter
                    {
                        CourseId = s.CourseId,
                        PaperCrafterId = s.PaperCrafterId
                    }).ToListAsync();
                var courseUserList = new List<Guid>();
                courseUserList.AddRange(courseManagerLists.Select(c => c.ManagerId));
                courseUserList.AddRange(courseCoManagerLists.Select(c => c.CoManagerId));
                courseUserList.AddRange(coursePaperCrafterLists.Select(c => c.PaperCrafterId));
                courseUserList = courseUserList.Distinct().ToList();
                var users = await _dbRepository.Context.Set<User>().Where(c => courseUserList.Contains(c.Id)).Select(s =>
                    new User
                    {
                        Id = s.Id,
                        DisplayName = s.DisplayName,
                        Username = !string.IsNullOrEmpty(s.Username) ? s.Username : s.DisplayName
                    }).ToListAsync();

                foreach (var validCourse in validCourses)
                {
                    if (validCourse.ReportStatus == ImportStatus.Skip)
                    {
                        continue;
                    }
                    //var course = courses.AsParallel().FirstOrDefault(a => a.Code.EqualsIgnoreCase(validCourse.Code) && a.CourseOrganizationPath.EqualsIgnoreCase(validCourse.DynamicColumnValueStr) && a.SemesterName.EqualsIgnoreCase(validCourse.Semester));
                    var courseQuery = courses.AsParallel().Where(a => a.Code.EqualsIgnoreCase(validCourse.Code)
                    && a.CourseOrganizationPath.EqualsIgnoreCase(validCourse.DynamicColumnValueStr));
                    if (enableSemester)
                    {
                        courseQuery = courseQuery.Where(a => a.SemesterName.EqualsIgnoreCase(validCourse.Semester));
                    }
                    var course = courseQuery.FirstOrDefault();
                    if (course == null)
                    {
                        validCourse.HasChanged = true;
                    }
                    else
                    {
                        if (!validCourse.Name.EqualsIgnoreCase(course.Name))
                        {
                            validCourse.HasChanged = true;
                        }
                        var currentCourseManagers = courseManagerLists.Where(c => c.CourseId == course.Id).Select(c => c.ManagerId).ToList();
                        var currentManagerUsers = users.Where(u => currentCourseManagers.Contains(u.Id)).ToList();
                        var currentManagerUserDisplayNames = currentManagerUsers.Select(u => u.DisplayName).ToList();

                        List<string> managersBeforeName = new List<string>();
                        managersBeforeName.AddRange(currentManagerUserDisplayNames);
                        List<string> managersBeforeNameAndEmail = new List<string>();
                        managersBeforeNameAndEmail.AddRange(managersBeforeName);
                        managersBeforeNameAndEmail.AddRange(currentManagerUsers.Where(u => !String.IsNullOrEmpty(u.Username)).Select(u => u.Username.ToLower()).ToList());

                        if (validCourse.ListManagerEmails.Count != managersBeforeName.Count)
                        {
                            validCourse.HasChanged = true;
                        }
                        else if (validCourse.ListManagerEmails.Any(a => !managersBeforeNameAndEmail.Contains(a)))
                        {
                            validCourse.HasChanged = true;
                        }

                        var currentCoCourseManagers = courseCoManagerLists.Where(c => c.CourseId == course.Id).Select(c => c.CoManagerId).ToList();
                        var currentCoManagerUsers = users.Where(u => currentCoCourseManagers.Contains(u.Id)).ToList();
                        var currentCoManagerUserDisplayNames = currentCoManagerUsers.Select(u => u.DisplayName).ToList();

                        List<string> coManagersBeforeName = new List<string>();
                        coManagersBeforeName.AddRange(currentCoManagerUserDisplayNames);
                        List<string> coManagersBeforeNameAndEmail = new List<string>();
                        coManagersBeforeNameAndEmail.AddRange(coManagersBeforeName);
                        coManagersBeforeNameAndEmail.AddRange(currentCoManagerUsers.Where(u => !String.IsNullOrEmpty(u.Username)).Select(u => u.Username.ToLower()).ToList());

                        if (validCourse.CoManagerEmails.Count != coManagersBeforeName.Count)
                        {
                            validCourse.HasChanged = true;
                        }
                        else if (validCourse.CoManagerEmails.Any(a => !coManagersBeforeNameAndEmail.Contains(a)))
                        {
                            validCourse.HasChanged = true;
                        }

                        var currentPaperCrafters = coursePaperCrafterLists.Where(c => c.CourseId == course.Id).Select(c => c.PaperCrafterId).ToList();
                        var currentPaperCrafterUsers = users.Where(u => currentPaperCrafters.Contains(u.Id)).ToList();
                        var currentPaperCrafterUserDisplayNames = currentPaperCrafterUsers.Select(u => u.DisplayName).ToList();

                        List<string> paperCrafterBefore = new List<string>();
                        paperCrafterBefore.AddRange(currentPaperCrafterUserDisplayNames);
                        paperCrafterBefore.AddRange(currentPaperCrafterUsers.Where(u => !String.IsNullOrEmpty(u.Username)).Select(u => u.Username.ToLower()).ToList());

                        if (validCourse.PaperCrafterEmails.Count != currentPaperCrafterUsers.Count)
                        {
                            validCourse.HasChanged = true;
                        }
                        else if (validCourse.PaperCrafterEmails.Any(a => !paperCrafterBefore.Contains(a)))
                        {
                            validCourse.HasChanged = true;
                        }

                        if (!validCourse.HasChanged)
                        {
                            validCourse.ReportStatus = ImportStatus.Skip;
                            validCourse.SkipMessage = _skipMessage;
                        }
                    }
                }
            }
        }

        private async Task CheckClassesChange()
        {
            var validClasses = _excelClasses.Where(a => a.ReportStatus != ImportStatus.Failed);

            if (validClasses.Any())
            {
                ProcessDuplicatedClass(validClasses.ToList());
                var classNames = validClasses.Where(a => !string.IsNullOrEmpty(a.ClassName)).Select(a => a.ClassName.ToLower()).Distinct().ToList();
                //var baveNullClassName = validClasses.Any(a => string.IsNullOrEmpty(a.ClassName));
                var classes = await _dbRepository.Context.Set<CourseClass>()
                    .Include(a => a.Course).ThenInclude(a => a.Semester)
                    .Include(a => a.Course.CourseOrganizations).ThenInclude(a => a.Organization).ThenInclude(a => a.DataStructure)
                    .Include(a => a.CourseClassOwners).ThenInclude(a => a.Owner)
                    .Where(a => classNames.Contains(a.Name.ToLower())
                        /*&& a.ActiveStatus == FeatureCommon.Domain.ActiveStatus.Active*/
                        && a.Course.DeleteStatus == DeleteStatus.Ok)
                    .Select(a => new
                    {
                        a.Course.Code,
                        a.Name,
                        a.CourseClassOwners,
                        SemesterName = a.Course.Semester.Name,
                        CourseOrganizationPath = a.Course.CourseOrganizationPath,
                    }).ToListAsync();
                foreach (var validClasse in validClasses)
                {
                    if (validClasse.ReportStatus == ImportStatus.Skip)
                    {
                        continue;
                    }
                    //var courseClass = classes.AsParallel().FirstOrDefault(a => a.Name.EqualsIgnoreCase(validClasse.ClassName) && a.Code.EqualsIgnoreCase(validClasse.Code) && a.CourseOrganizationPath.EqualsIgnoreCase(validClasse.DynamicColumnValueStr) && a.SemesterName.EqualsIgnoreCase(validClasse.Semester));
                    var courseClass = enableSemester ? classes.AsParallel().FirstOrDefault(a => a.Name.EqualsIgnoreCase(validClasse.ClassName) && a.Code.EqualsIgnoreCase(validClasse.Code) && a.CourseOrganizationPath.EqualsIgnoreCase(validClasse.DynamicColumnValueStr) && a.SemesterName.EqualsIgnoreCase(validClasse.Semester))
                        : classes.AsParallel().FirstOrDefault(a => a.Name.EqualsIgnoreCase(validClasse.ClassName) && a.Code.EqualsIgnoreCase(validClasse.Code) && a.CourseOrganizationPath.EqualsIgnoreCase(validClasse.DynamicColumnValueStr));
                    if (courseClass == null)
                    {
                        validClasse.HasChanged = true;
                    }
                    else
                    {
                        //if (!validClasse.ClassName.EqualsIgnoreCase(courseClass.Name))
                        //{
                        //    validClasse.HasChanged = true;
                        //}

                        List<string> classOwnerBefore = new List<string>();
                        foreach (var item in courseClass.CourseClassOwners)
                        {
                            classOwnerBefore.Add(item.Owner.DisplayName);
                            if (!string.IsNullOrEmpty(item.Owner.Username) || !string.IsNullOrEmpty(item.Owner.DisplayName))
                            {
                                var realName = !string.IsNullOrEmpty(item.Owner.Username) ? item.Owner.Username.ToLower() : item.Owner.DisplayName.ToLower();
                                classOwnerBefore.Add(realName);
                            }
                        }

                        if (validClasse.OwnerEmails.Count != courseClass.CourseClassOwners.Count)
                        {
                            validClasse.HasChanged = true;
                        }
                        else if (validClasse.OwnerEmails.Any(a => !classOwnerBefore.Contains(a)))
                        {
                            validClasse.HasChanged = true;
                        }

                        if (!validClasse.HasChanged)
                        {
                            validClasse.ReportStatus = ImportStatus.Skip;
                            validClasse.SkipMessage = _skipMessage;
                        }
                    }
                }
            }
        }

        private Guid GetGoupIdInDb(string mailboxOrGroup, UserType userType)
        {
            Guid result = default(Guid);
            if (userType.IsStaff())
            {
                if (_staffs.Keys.Any(a => a.Equals(mailboxOrGroup, StringComparison.OrdinalIgnoreCase)))
                {
                    result = _staffs[mailboxOrGroup];
                }
                else if (_staffsEmailGroup.Keys.Any(a => a.Equals(mailboxOrGroup, StringComparison.OrdinalIgnoreCase)))
                {
                    result = _staffsEmailGroup[mailboxOrGroup];
                }
                else if (_staffsNameGroup.Keys.Any(a => a.Equals(mailboxOrGroup, StringComparison.OrdinalIgnoreCase)))
                {
                    result = _staffsNameGroup[mailboxOrGroup];
                }
            }
            else
            {
                if (_candidates.Keys.Any(a => a.Equals(mailboxOrGroup, StringComparison.OrdinalIgnoreCase)))
                {
                    result = _candidates[mailboxOrGroup];
                }
                else if (_candidatesEmailGroup.Keys.Any(a => a.Equals(mailboxOrGroup, StringComparison.OrdinalIgnoreCase)))
                {
                    result = _candidatesEmailGroup[mailboxOrGroup];
                }
                else if (_candidatesNameGroup.Keys.Any(a => a.Equals(mailboxOrGroup, StringComparison.OrdinalIgnoreCase)))
                {
                    result = _candidatesNameGroup[mailboxOrGroup];
                }
            }
            return result;
        }

        private List<Guid> GetGroupUserIds(List<string> mailboxOrGroups, UserType userType)
        {
            List<Guid> result = new List<Guid>();
            List<Guid> emailList;
            List<Guid> emailGroupList;
            List<Guid> emailGroupNames;
            if (userType == UserType.Staff)
            {
                emailList = mailboxOrGroups.Any() ?
                                   _staffs.Where(a => mailboxOrGroups.Contains(a.Key)).Select(a => a.Value).ToList() :
                                   new List<Guid>();

                emailGroupList = mailboxOrGroups.Any() ?
                                 _staffsEmailGroup.Where(a => mailboxOrGroups.Contains(a.Key)).Select(a => a.Value).ToList() :
                                 new List<Guid>();

                emailGroupNames = mailboxOrGroups.Any() ?
                                 _staffsNameGroup.Where(a => mailboxOrGroups.Contains(a.Key)).Select(a => a.Value).ToList() :
                                 new List<Guid>();
            }
            else
            {
                emailList = mailboxOrGroups.Any() ?
                                   _candidates.Where(a => mailboxOrGroups.Contains(a.Key)).Select(a => a.Value).ToList() :
                                   new List<Guid>();

                emailGroupList = mailboxOrGroups.Any() ?
                                 _candidatesEmailGroup.Where(a => mailboxOrGroups.Contains(a.Key)).Select(a => a.Value).ToList() :
                                 new List<Guid>();

                emailGroupNames = mailboxOrGroups.Any() ?
                                 _candidatesNameGroup.Where(a => mailboxOrGroups.Contains(a.Key)).Select(a => a.Value).ToList() :
                                 new List<Guid>();
            }
            result.AddRange(emailList);
            result.AddRange(emailGroupList);
            result.AddRange(emailGroupNames);
            result = result.Distinct().ToList();
            return result;
        }

        private async Task UpdateCoursesAsync()
        {
            Dictionary<Guid, List<Guid>> _courseRoleMapDicForAdd = new Dictionary<Guid, List<Guid>>();
            Dictionary<Guid, List<Guid>> _coCourseRoleMapDicForAdd = new Dictionary<Guid, List<Guid>>();
            Dictionary<Guid, List<Guid>> _paperRoleMapDicForAdd = new Dictionary<Guid, List<Guid>>();
            Dictionary<Guid, List<Guid>> _courseRoleMapDicForDelete = new Dictionary<Guid, List<Guid>>();
            Dictionary<Guid, List<Guid>> _coCourseRoleMapDicForDelete = new Dictionary<Guid, List<Guid>>();
            Dictionary<Guid, List<Guid>> _paperRoleMapDicForDelete = new Dictionary<Guid, List<Guid>>();
            var scopeIdList = new List<Guid>();
            var userIdList = new List<Guid>();
            var changedCourses = _excelCourse.Where(a => a.HasChanged);
            //没有匹配Organization并且HasChanged的excelCourse需要ValidateOrganization。
            foreach (var needValidateCourse in changedCourses)
            {
                if (needValidateCourse.NeedVerify)
                {
                    ValidateOrganization(needValidateCourse);
                }
            }
            changedCourses = changedCourses.Where(a => a.ReportStatus != ImportStatus.Failed && a.HasChanged);
            _currentDataRow += _excelCourse.Count() - changedCourses.Count();
            await UpdataProgressBar(0.01m + ((decimal)(_currentDataRow) / _totalDataRow * _miniProgressBar));
            if (changedCourses.Any())
            {
                var lowerCodes = changedCourses.Select(a => a.Code.ToLower()).ToList();
                var now = DateTimeOffset.UtcNow;
                var currentUserId = _currentUserId;
                var courses = await _dbRepository.Query(cxt => cxt.Set<Course>()
                .Include(a => a.CourseOrganizations).ThenInclude(a => a.Organization).ThenInclude(a => a.DataStructure)
                .Include(i => i.Managers)
                .Include(i => i.CoManagers)
                .Include(i => i.PaperCrafters)
                .Include(i => i.Semester)
               .Where(a => lowerCodes.Contains(a.Code.ToLower())
                   //&& a.ActiveStatus == ActiveStatus.Active
                   && a.DeleteStatus == DeleteStatus.Ok)).ToListAsync();
                var haveChangedCourses = changedCourses.GroupBy(a => a.Key).Select(a => a.LastOrDefault());
                var sameKeyChangedCourses = changedCourses.Except(haveChangedCourses).ToList();
                if (sameKeyChangedCourses != null && sameKeyChangedCourses.Any())
                {
                    sameKeyChangedCourses.ForEach(a => a.ReportStatus = ImportStatus.Success);
                }
                foreach (var changedCourse in haveChangedCourses.AsParallel())
                {
                    try
                    {
                        var courseRoleListForAdd = new List<Guid>();
                        var coCourseRoleListForAdd = new List<Guid>();
                        var paperRoleListForAdd = new List<Guid>();
                        var courseRoleListForDelete = new List<Guid>();
                        var coCourseRoleListForDelete = new List<Guid>();
                        var paperRoleListForDelete = new List<Guid>();
                        var name = changedCourse.Name;
                        var code = changedCourse.Code;

                        List<Guid> managerIds = new List<Guid>();
                        var managerUserIds = GetGroupUserIds(changedCourse.ListManagerEmails, UserType.Staff);
                        //foreach (var userId in managerUserIds)
                        //{
                        //    if (_staffsGroupMembers.ContainsKey(userId))
                        //    {
                        //        _staffsGroupMembers[userId].ForEach(a => managerIds.Add(a.UserId));
                        //    }
                        //    else
                        //    {
                        //        managerIds.Add(userId);
                        //    }
                        //}
                        managerIds = managerUserIds.Distinct().ToList();
                        List<Guid> coManagerIds = new List<Guid>();
                        var coManagerUserIds = GetGroupUserIds(changedCourse.CoManagerEmails, UserType.Staff);
                        //foreach (var userId in coManagerUserIds)
                        //{
                        //    if (_staffsGroupMembers.ContainsKey(userId))
                        //    {
                        //        _staffsGroupMembers[userId].ForEach(a => coManagerIds.Add(a.UserId));
                        //    }
                        //    else
                        //    {
                        //        coManagerIds.Add(userId);
                        //    }
                        //}
                        coManagerIds = coManagerUserIds.Distinct().ToList();
                        List<Guid> paperCrafterIds = GetGroupUserIds(changedCourse.PaperCrafterEmails, UserType.Staff);

                        //paperCrafterIds = changedCourse.PaperCrafterEmails.Any() ?
                        //            _staffs.Where(a => changedCourse.PaperCrafterEmails.Contains(a.Key)).Select(a => a.Value).ToList() :
                        //            new List<Guid>();
                        //var course = courses.FirstOrDefault(a => string.Equals(a.Code, changedCourse.Code, StringComparison.OrdinalIgnoreCase) && string.Equals(a.CourseOrganizationPath, changedCourse.DynamicColumnValueStr, StringComparison.OrdinalIgnoreCase) && string.Equals(a.Semester.Name, changedCourse.Semester, StringComparison.OrdinalIgnoreCase));
                        var course = enableSemester ? courses.FirstOrDefault(a => string.Equals(a.Code, changedCourse.Code, StringComparison.OrdinalIgnoreCase) && string.Equals(a.CourseOrganizationPath, changedCourse.DynamicColumnValueStr, StringComparison.OrdinalIgnoreCase) && string.Equals(a.Semester.Name, changedCourse.Semester, StringComparison.OrdinalIgnoreCase))
                            : courses.FirstOrDefault(a => string.Equals(a.Code, changedCourse.Code, StringComparison.OrdinalIgnoreCase) && string.Equals(a.CourseOrganizationPath, changedCourse.DynamicColumnValueStr, StringComparison.OrdinalIgnoreCase));
                        if (course == null)
                        {
                            course = Convert(name, code, managerIds, coManagerUserIds, paperCrafterIds);
                            course.CreatedById = currentUserId;
                            course.ModifiedById = currentUserId;
                            course.CreatedTime = now;
                            course.ModifiedTime = now;
                            course.SemesterId = enableSemester ? _semesters[changedCourse.Semester.ToLower()] : null;
                            course = await _dbRepository.AddAsync(course);
                            var addList = new List<CourseOrganization>();
                            foreach (var organizationId in changedCourse.OrganizationIds)
                            {
                                var courseOrganization = new CourseOrganization();
                                courseOrganization.CourseId = course.Id;
                                courseOrganization.OrganizationId = organizationId;
                                addList.Add(courseOrganization);
                            }
                            await _dbRepository.AddRangeAsync(addList);
                        }
                        else
                        {
                            if (course.Managers.Any())
                            {
                                _dbRepository.DeleteRange(course.Managers);
                                courseRoleListForDelete.AddRange(course.Managers.Select(a => a.ManagerId));
                            }
                            if (course.CoManagers.Any())
                            {
                                _dbRepository.DeleteRange(course.CoManagers);
                                coCourseRoleListForDelete.AddRange(course.CoManagers.Select(a => a.CoManagerId));
                                //updateRoleList.AddRange(course.CoManagers.Select(a => new UpdateRole() { Id = a.CoManagerId, UpdateRoleEnum = UpdateRoleEnum.Delete, RoleId = RoleConstants.CoCourseManagerRoleId }));
                            }
                            if (course.PaperCrafters.Any())
                            {
                                _dbRepository.DeleteRange(course.PaperCrafters);
                                paperRoleListForDelete.AddRange(course.PaperCrafters.Select(a => a.PaperCrafterId));
                                //updateRoleList.AddRange(course.PaperCrafters.Select(a => new UpdateRole() { Id = a.PaperCrafterId, UpdateRoleEnum = UpdateRoleEnum.Delete, RoleId = RoleConstants.PaperCrafterRoleId }));
                            }
                            course.Code = code;
                            course.Name = name;

                            course.Managers = managerIds.Select(a => new CourseManager()
                            {
                                CourseId = course.Id,
                                ManagerId = a
                            }).ToList();
                            course.CoManagers = coManagerIds.Select(a => new CourseCoManager()
                            {
                                CourseId = course.Id,
                                CoManagerId = a
                            }).ToList();
                            course.PaperCrafters = paperCrafterIds.Select(a => new CoursePaperCrafter()
                            {
                                CourseId = course.Id,
                                PaperCrafterId = a
                            }).ToList();
                            course.ModifiedById = currentUserId;
                            course.ModifiedTime = now;
                            _dbRepository.Update(course);
                        }

                        if (managerIds.Any())
                        {
                            courseRoleListForAdd.AddRange(managerIds);
                        }
                        if (coManagerIds.Any())
                        {
                            coCourseRoleListForAdd.AddRange(coManagerIds);
                        }
                        if (paperCrafterIds.Any())
                        {
                            paperRoleListForAdd.AddRange(paperCrafterIds);
                        }
                        if (!_courseRoleMapDicForAdd.ContainsKey(course.Id))
                        {
                            _courseRoleMapDicForAdd.Add(course.Id, courseRoleListForAdd);
                        }
                        if (!_coCourseRoleMapDicForAdd.ContainsKey(course.Id))
                        {
                            _coCourseRoleMapDicForAdd.Add(course.Id, coCourseRoleListForAdd);
                        }
                        if (!_paperRoleMapDicForAdd.ContainsKey(course.Id))
                        {
                            _paperRoleMapDicForAdd.Add(course.Id, paperRoleListForAdd);
                        }
                        if (!_courseRoleMapDicForDelete.ContainsKey(course.Id))
                        {
                            _courseRoleMapDicForDelete.Add(course.Id, courseRoleListForDelete);
                        }
                        if (!_coCourseRoleMapDicForDelete.ContainsKey(course.Id))
                        {
                            _coCourseRoleMapDicForDelete.Add(course.Id, coCourseRoleListForDelete);
                        }
                        if (!_paperRoleMapDicForDelete.ContainsKey(course.Id))
                        {
                            _paperRoleMapDicForDelete.Add(course.Id, paperRoleListForDelete);
                        }
                        scopeIdList.Add(course.Id);
                        userIdList.AddRange(courseRoleListForAdd);
                        userIdList.AddRange(coCourseRoleListForAdd);
                        userIdList.AddRange(paperRoleListForAdd);
                        userIdList.AddRange(courseRoleListForDelete);
                        userIdList.AddRange(coCourseRoleListForDelete);
                        userIdList.AddRange(paperRoleListForDelete);
                        changedCourse.ReportStatus = ImportStatus.Success;
                    }
                    catch (Exception ex)
                    {
                        changedCourse.ReportStatus = ImportStatus.Failed;
                        changedCourse.InvalidMessage.Add(I18NEntity.GetString("Admin_ImportCourse_InternalError_Message"));
                        logger.Error($"update Course Code: {changedCourse.Code} error :{ex.Message}", ex);
                    }
                    finally
                    {
                        _currentDataRow++;
                        await UpdataProgressBar(0.01m + ((decimal)(_currentDataRow) / _totalDataRow * _miniProgressBar));
                    }
                }
                await _dbRepository.SaveChangesAsync();
            }
            scopeIdList = scopeIdList.Distinct().ToList();
            userIdList = userIdList.Distinct().ToList();
            List<UserRoleMapping> userRoleMappings = new List<UserRoleMapping>();
            if (scopeIdList != null && scopeIdList.Count > 0 && userIdList != null && userIdList.Count > 0)
            {
                userRoleMappings = await _commonQueryService.GetUserRoleMappingQueryByUserIdList(userIdList, scopeIdList).AsNoTracking().ToListAsync();
            }
            await _userService.BatchDeleteRolesAsync(_courseRoleMapDicForDelete, RoleConstants.CourseManagerRoleId, userRoleMappings);
            await _userService.BatchAssignRolesAsync(_courseRoleMapDicForAdd, RoleConstants.CourseManagerRoleId, userRoleMappings, ScopeType.CourseManager);

            await _userService.BatchDeleteRolesAsync(_coCourseRoleMapDicForDelete, RoleConstants.CoCourseManagerRoleId, userRoleMappings);
            await _userService.BatchAssignRolesAsync(_coCourseRoleMapDicForAdd, RoleConstants.CoCourseManagerRoleId, userRoleMappings, ScopeType.CocourseManager);

            await _userService.BatchDeleteRolesAsync(_paperRoleMapDicForDelete, RoleConstants.PaperCrafterRoleId, userRoleMappings);
            await _userService.BatchAssignRolesAsync(_paperRoleMapDicForAdd, RoleConstants.PaperCrafterRoleId, userRoleMappings, ScopeType.QuestionCrafter);
        }

        private async Task UpdateClassesAsync()
        {
            Dictionary<Guid, List<Guid>> _ownerRoleMapDicForAdd = new Dictionary<Guid, List<Guid>>();
            Dictionary<Guid, List<Guid>> _ownerRoleMapDicForDelete = new Dictionary<Guid, List<Guid>>();
            var scopeIdList = new List<Guid>();
            var userIdList = new List<Guid>();
            var changedClasses = _excelClasses.Where(a => a.HasChanged);
            _currentDataRow += _excelClasses.Count() - changedClasses.Count();
            await UpdataProgressBar(0.01m + ((decimal)(_currentDataRow) / _totalDataRow * _miniProgressBar));
            if (changedClasses.Any())
            {
                var lowerName = changedClasses.Select(a => a.ClassName.ToLower()).ToList();
                var lowerCode = changedClasses.Select(a => a.Code.ToLower()).ToList();
                var now = DateTimeOffset.UtcNow;
                var currentUserId = _currentUserId;
                var classes = await _dbRepository.Query(cxt => cxt.Set<CourseClass>()
                .Include(i => i.Course)
                .ThenInclude(a => a.CourseOrganizations).ThenInclude(a => a.Organization).ThenInclude(a => a.DataStructure)
                .Include(i => i.CourseClassOwners).ThenInclude(a => a.Owner)
                .Where(a => lowerName.Contains(a.Name.ToLower())
                   && a.Course.DeleteStatus == DeleteStatus.Ok)).ToListAsync();
                var courses = await _dbRepository.Query(cxt => cxt.Set<Course>()
                .Include(a => a.CourseOrganizations).ThenInclude(a => a.Organization).ThenInclude(a => a.DataStructure)
                .Include(a => a.Semester)
                .Where(a => lowerCode.Contains(a.Code.ToLower())
                   && a.DeleteStatus == DeleteStatus.Ok)).ToListAsync();
                var coursesIds = courses.Select(a => a.Id).ToList();
                var courseRosters = await _dbRepository.Query(cxt => cxt.Set<CourseRoster>().AsNoTracking()
                .Where(a => coursesIds.Contains(a.CourseId)
                    && a.Candidate.UserStatus == UserStatus.Active)).ToListAsync();
                var haveChangeCourseIds = new List<Guid>();
                foreach (var changedClass in changedClasses.AsParallel())
                {
                    try
                    {
                        var ownerRoleListForAdd = new List<Guid>();
                        var ownerRoleListForDelete = new List<Guid>();
                        var name = changedClass.ClassName;
                        var code = changedClass.Code;

                        List<Guid> ownerIds = new List<Guid>();
                        List<Guid> userIds = GetGroupUserIds(changedClass.OwnerEmails, UserType.Staff);
                        //foreach (var userId in userIds)
                        //{
                        //    if (_staffsGroupMembers.ContainsKey(userId))
                        //    {
                        //        _staffsGroupMembers[userId].ForEach(a => ownerIds.Add(a.UserId));
                        //    }
                        //    else
                        //    {
                        //        ownerIds.Add(userId);
                        //    }
                        //}
                        ownerIds = userIds.Distinct().ToList();

                        //List<Guid> ownerIds = GetGroupUserIds(changedClass.OwnerEmails, UserType.Staff);
                        //var course = courses.FirstOrDefault(a => string.Equals(a.Code, changedClass.Code, StringComparison.OrdinalIgnoreCase) && string.Equals(a.CourseOrganizationPath, changedClass.DynamicColumnValueStr, StringComparison.OrdinalIgnoreCase) && string.Equals(a.Semester.Name, changedClass.Semester));
                        var course = enableSemester ? courses.FirstOrDefault(a => string.Equals(a.Code, changedClass.Code, StringComparison.OrdinalIgnoreCase) && string.Equals(a.CourseOrganizationPath, changedClass.DynamicColumnValueStr, StringComparison.OrdinalIgnoreCase) && string.Equals(a.Semester.Name, changedClass.Semester, StringComparison.OrdinalIgnoreCase))
                            : courses.FirstOrDefault(a => string.Equals(a.Code, changedClass.Code, StringComparison.OrdinalIgnoreCase) && string.Equals(a.CourseOrganizationPath, changedClass.DynamicColumnValueStr, StringComparison.OrdinalIgnoreCase));
                        if (course == null)
                        {
                            //Must do
                            changedClass.HasChanged = false;
                            changedClass.ReportStatus = ImportStatus.Failed;
                            changedClass.InvalidMessage.Add(I18NEntity.GetString("Admin_ImportCourseClass_Course_MissMatch_Message", changedClass.Code));

                        }
                        else
                        {
                            var coueseClass = classes.FirstOrDefault(a => a.CourseId == course.Id && string.Equals(a.Name, changedClass.ClassName, StringComparison.OrdinalIgnoreCase));
                            if (coueseClass == null)
                            {
                                coueseClass = ConvertClass(name, course.Id, ownerIds);
                                coueseClass.CreatedById = currentUserId;
                                coueseClass.ModifiedById = currentUserId;
                                coueseClass.CreatedTime = now;
                                coueseClass.ModifiedTime = now;
                                coueseClass = await _dbRepository.AddAsync(coueseClass);

                                var currentCourseRoster = courseRosters.Where(a => a.CourseId == course.Id).ToList();
                                if (currentCourseRoster.All(a => a.ClassId == null))
                                {
                                    currentCourseRoster.ForEach(a => a.ClassId = coueseClass.Id);
                                    _dbRepository.Context.UpdateRange(currentCourseRoster);
                                };
                                classes.Add(coueseClass);
                            }
                            else
                            {
                                if (coueseClass.CourseClassOwners.Any())
                                {
                                    _dbRepository.DeleteRange(coueseClass.CourseClassOwners);
                                    ownerRoleListForDelete.AddRange(coueseClass.CourseClassOwners.Select(a => a.OwnerId));
                                }
                                coueseClass.Name = name;
                                coueseClass.CourseClassOwners = ownerIds.Select(a => new CourseClassOwner()
                                {
                                    CourseId = course.Id,
                                    ClassId = coueseClass.Id,
                                    OwnerId = a
                                }).ToList();
                                course.ModifiedById = currentUserId;
                                course.ModifiedTime = now;
                                _dbRepository.Update(course);
                            }
                            if (ownerIds.Any())
                            {
                                ownerRoleListForAdd.AddRange(ownerIds);
                            }
                            if (!_ownerRoleMapDicForAdd.ContainsKey(coueseClass.Id))
                            {
                                _ownerRoleMapDicForAdd.Add(coueseClass.Id, ownerRoleListForAdd);
                            }
                            if (!_ownerRoleMapDicForDelete.ContainsKey(coueseClass.Id))
                            {
                                _ownerRoleMapDicForDelete.Add(coueseClass.Id, ownerRoleListForDelete);
                            }
                            scopeIdList.Add(coueseClass.Id);
                            userIdList.AddRange(ownerRoleListForAdd);
                            userIdList.AddRange(ownerRoleListForDelete);
                            changedClass.ReportStatus = ImportStatus.Success;
                        }

                    }
                    catch (Exception ex)
                    {
                        changedClass.ReportStatus = ImportStatus.Failed;
                        changedClass.InvalidMessage.Add(I18NEntity.GetString("Admin_ImportCourse_InternalError_Message"));
                        logger.Error($"update Course class Name: {changedClass.ClassName} error :{ex.Message}.", ex);
                    }
                    finally
                    {
                        _currentDataRow++;
                        await UpdataProgressBar(0.01m + ((decimal)(_currentDataRow) / _totalDataRow * _miniProgressBar));
                    }
                }
            }
            scopeIdList = scopeIdList.Distinct().ToList();
            userIdList = userIdList.Distinct().ToList();
            List<UserRoleMapping> userRoleMappings = new List<UserRoleMapping>();
            if (scopeIdList != null && scopeIdList.Count > 0 && userIdList != null && userIdList.Count > 0)
            {
                userRoleMappings = await _commonQueryService.GetUserRoleMappingQueryByUserIdList(userIdList, scopeIdList).AsNoTracking().ToListAsync();
            }
            await _userService.BatchDeleteRolesAsync(_ownerRoleMapDicForDelete, RoleConstants.ClassOwnerRoleId, userRoleMappings);
            await _userService.BatchAssignRolesAsync(_ownerRoleMapDicForAdd, RoleConstants.ClassOwnerRoleId, userRoleMappings, ScopeType.ClassOwner);
        }

        private Course Convert(string name, string code, List<Guid> managerIds, List<Guid> coManagerIds, List<Guid> paperCrafterIds)
        {
            var entity = new Course()
            {
                Id = Guid.NewGuid(),
                Name = name,
                Code = code,
                ActiveStatus = ActiveStatus.Active,
                DeleteStatus = DeleteStatus.Ok
            };
            if (managerIds.Any())
            {
                entity.Managers = managerIds.Select(a => new CourseManager()
                {
                    CourseId = entity.Id,
                    ManagerId = a
                }).ToList();
            }
            if (coManagerIds.Any())
            {
                entity.CoManagers = coManagerIds.Select(a => new CourseCoManager()
                {
                    CourseId = entity.Id,
                    CoManagerId = a
                }).ToList();
            }
            if (paperCrafterIds.Any())
            {
                entity.PaperCrafters = paperCrafterIds.Select(a => new CoursePaperCrafter()
                {
                    CourseId = entity.Id,
                    PaperCrafterId = a
                }).ToList();
            }
            return entity;
        }
        private CourseClass ConvertClass(string name, Guid courseId, List<Guid> ownerIds)
        {
            var entity = new CourseClass()
            {
                Id = Guid.NewGuid(),
                CourseId = courseId,
                Name = name,
            };

            if (ownerIds.Any())
            {
                entity.CourseClassOwners = ownerIds.Select(a => new CourseClassOwner()
                {
                    CourseId = courseId,
                    ClassId = entity.Id,
                    OwnerId = a
                }).ToList();
            }
            return entity;
        }

        #region ValidateOrganization

        private void ValidateOrganization<T>(T excelModel) where T : OrganizationDynamicExcelModel
        {
            if (excelModel != null)
            {
                if (excelModel != null && excelModel.DynamicColumns == null)
                {
                    excelModel.DynamicColumns = new List<DynamicExcelModel>();
                }
                if (_dataStructure == null)
                {
                    _dataStructure = new List<DataStructure>();
                }
                if (_dataStructure.Count != excelModel.DynamicColumns.Count)
                {
                    //dataStructure层级与Excel列不匹配
                    excelModel.ReportStatus = ImportStatus.Failed;
                    excelModel.InvalidMessage.Add(I18NEntity.GetString("Admin_ImportCourse_DataStructureColumn_MissMatch_Message", _dataStructure.Count, excelModel.DynamicColumns.Count));
                }
                else
                {
                    if (_organization == null)
                    {
                        _organization = new List<Organization>();
                    }
                    if (_organization.Count == 0 && excelModel.DynamicColumns.Count > 0)
                    {
                        //数据库没有配置organization
                        excelModel.ReportStatus = ImportStatus.Failed;
                        excelModel.InvalidMessage.Add(I18NEntity.GetString("Admin_ImportCourse_OrganizationColumn_MissMatch_Message"));
                    }
                }
                if (excelModel.DynamicColumns.Count > 0)
                {
                    OrganizationTree currentTree = null;
                    foreach (var dc in excelModel.DynamicColumns)
                    {
                        currentTree = _organizationTree.Where(a => string.Equals(a.Name, dc.Value, StringComparison.OrdinalIgnoreCase) && string.Equals(a.StructureName, dc.Name, StringComparison.OrdinalIgnoreCase)).FirstOrDefault();
                        if (currentTree != null)
                        {
                            break;
                        }

                    }
                    if (currentTree == null)
                    {
                        //Trunk OrganizationTree的第一层 与Excel 不匹配
                        excelModel.ReportStatus = ImportStatus.Failed;
                        excelModel.InvalidMessage.Add(I18NEntity.GetString("Admin_ImportCourse_OrganizationColumn_MissMatch_Message"));

                    }
                    else
                    {
                        List<string> titleList = new List<string>();
                        List<string> valueList = new List<string>();
                        foreach (var ds in _dataStructure)
                        {
                            var currentColumn = excelModel.DynamicColumns.Where(a => string.Equals(a.Name, ds.Name, StringComparison.OrdinalIgnoreCase)).FirstOrDefault();
                            if (currentColumn == null || string.IsNullOrEmpty(currentColumn.Value))
                            {
                                //dataStructure Name与Excel内的Column不匹配
                                excelModel.ReportStatus = ImportStatus.Failed;
                                excelModel.InvalidMessage.Add(I18NEntity.GetString("Admin_ImportCourse_Column_Empty_Message", ds.Name));
                                break;
                            }
                            else
                            {
                                titleList.Add(currentColumn.Name.ToLower().Trim());
                                valueList.Add(currentColumn.Value.Trim());
                            }
                        }
                        if (titleList != null && titleList.Count > 0)
                        {
                            excelModel.DynamicColumnTitleStr = String.Join("/", titleList);
                        }
                        if (valueList != null && valueList.Count > 0)
                        {
                            excelModel.DynamicColumnValueStr = String.Join("/", valueList);
                        }
                        Dictionary<string, string> templst = new Dictionary<string, string>();
                        var ret = FilterOrganizationTreeNames("", "", currentTree, templst);
                        if (ret.Keys.Contains((string)(excelModel.DynamicColumnValueStr).ToLower()))
                        {
                            var value = ret[excelModel.DynamicColumnValueStr.ToLower()];
                            var idStrList = value.Split("/").ToList();
                            foreach (var idStr in idStrList)
                            {
                                excelModel.OrganizationIds.Add(Guid.Parse(idStr));
                            }
                        }
                        else
                        {
                            //OrganizationName Path 与 Excel的 OrganizationName Path不匹配
                            excelModel.ReportStatus = ImportStatus.Failed;
                            excelModel.InvalidMessage.Add(I18NEntity.GetString("Admin_ImportCourse_OrganizationPathColumn_MissMatch_Message", excelModel.DynamicColumnValueStr));
                        }
                        excelModel.DynamicColumnValueStr = excelModel.DynamicColumnValueStr.ToLower();
                    }
                }
            }
        }
        #endregion
        private async Task ValidateCandidatesAsync()
        {
            var courseCodes = _excelCandidates.Select(a => a.CourseCode.ToLower()).Distinct().ToList();
            var organizations = await _dbRepository.Context.Set<Course>()
                   .Include(a => a.CourseOrganizations).ThenInclude(a => a.Organization).Include(t => t.CourseClasses)
                   .Where(a => courseCodes.Contains(a.Code.ToLower()) && a.DeleteStatus == DeleteStatus.Ok)
                   .Select(t => new
                   {
                       t.CourseOrganizations,
                       t.Code,
                       KeyClass = t.CourseOrganizationPath + t.Code + (enableSemester ? t.Semester.Name : string.Empty)
                   }).ToListAsync();

            foreach (var candidate in _excelCandidates)
            {
                var organization = organizations.Where(a => candidate.CourseCode.ToLower() == a.Code.ToLower()).Select(a => a.CourseOrganizations.ToList()).FirstOrDefault();
                if (NeedValidateOrganization(organization, candidate.DynamicColumns))
                {
                    ValidateOrganization(candidate);
                }
                else
                {
                    List<string> valueList = new List<string>();
                    foreach (var item in candidate.DynamicColumns)
                    {
                        valueList.Add(item.Value);
                    }
                    if (valueList != null && valueList.Count > 0)
                    {
                        candidate.DynamicColumnValueStr = String.Join("/", valueList);
                    }
                }
                if (candidate.ReportStatus != ImportStatus.Failed)
                {
                    #region CourseCode

                    if (string.IsNullOrEmpty(candidate.CourseCode))
                    {
                        candidate.ReportStatus = ImportStatus.Failed;
                        candidate.InvalidMessage.Add(I18NEntity.GetString("Admin_ImportCourse_Column_Empty_Message", I18NEntity.GetString("AD_CourseManagement_Edit_CourseCode_Entry")));
                    }
                    else if (candidate.CourseCode.Length > 32)
                    {
                        candidate.ReportStatus = ImportStatus.Failed;
                        candidate.InvalidMessage.Add(I18NEntity.GetString("Admin_ImportCourse_Column_MoreThan_Message", new[] { I18NEntity.GetString("AD_CourseManagement_Edit_CourseCode_Entry"), "32" }));
                    }
                    //else if (!courses.Any(a => candidate.CourseCode.EqualsIgnoreCase(a)))
                    //{
                    //    candidate.ReportStatus = ImportStatus.Failed;
                    //    candidate.InvalidMessage.Add(I18NEntity.GetString("Admin_ImportCourse_CourseCode_NotExist_Message"));
                    //}

                    #endregion CourseCode

                    #region Semester
                    if (enableSemester && string.IsNullOrEmpty(candidate.Semester))
                    {
                        candidate.ReportStatus = ImportStatus.Failed;
                        candidate.InvalidMessage.Add(I18NEntity.GetString("Admin_ImportCourse_Column_Empty_Message", I18NEntity.GetString("AD_CourseManagement_Edit_Semester_Entry")));
                    }
                    else if (enableSemester && !_semesters.ContainsKey(candidate.Semester.ToLower()))
                    {
                        candidate.ReportStatus = ImportStatus.Failed;
                        candidate.InvalidMessage.Add(I18NEntity.GetString("Admin_ImportCourseClass_Semester_MissMatch_Message", candidate.Semester));
                    }
                    #endregion

                    #region Name

                    if (organizations.Any(t => t.KeyClass.ToLower().Equals(candidate.KeyClass)) && string.IsNullOrEmpty(candidate.ClassName))
                    {
                        candidate.ReportStatus = ImportStatus.Failed;
                        candidate.InvalidMessage.Add(I18NEntity.GetString("Admin_ImportCourse_Column_Empty_Message", I18NEntity.GetString("AD_CourseManagement_Export_ClassName_Entry")));
                    }

                    #endregion Name

                    #region Username

                    if (string.IsNullOrEmpty(candidate.CandidateEmail))
                    {
                        candidate.ReportStatus = ImportStatus.Failed;
                        candidate.InvalidMessage.Add(I18NEntity.GetString("Admin_ImportCourse_Column_Empty_Message", I18NEntity.GetString("AD_CourseManagement_View_CandidateName_Entry")));
                    }
                    else
                    {
                        var mails = candidate.CandidateEmail.Replace("；", ";").Split(new[] { ";" }, StringSplitOptions.RemoveEmptyEntries);
                        if (mails.Count() != 1)
                        {
                            candidate.ReportStatus = ImportStatus.Failed;
                            candidate.InvalidMessage.Add(I18NEntity.GetString("Admin_ImportCourse_OnlyAssignedOneCandidatesUser_GroupFormat_Message", I18NEntity.GetString("AD_CourseManagement_View_CandidateName_Entry")));
                        }
                        else if (!IsInGroup(mails[0], _candidates) && !IsInGroup(mails[0], _candidatesEmailGroup) && !IsInGroup(mails[0], _candidatesNameGroup))
                        {
                            candidate.ReportStatus = ImportStatus.Failed;
                            candidate.InvalidMessage.Add(I18NEntity.GetString("Admin_ImportCourse_Single_EmailFormat_OrGroupEmail_Invalid_Message", new[] { mails[0], I18NEntity.GetString("AD_CourseManagement_View_CandidateName_Entry"), I18NEntity.GetString("GC_ProfileInner_Candidate_Entry").ToString().ToLower() }));
                        }
                    }

                    #endregion Username
                }
            }
        }

        private async Task CheckCandidateChange()
        {
            _excelCandidates.Where(t => t.ReportStatus == ImportStatus.NA).GroupBy(a => (a.DynamicColumnValueStr.ToLower() + a.CourseCode.ToLower() + a.CandidateEmail.ToLower().Replace("；", ";").Replace(";", "") + a.Semester?.ToLower()))
                .Where(a => a.Count() > 1)
                .SelectMany(a => a.Skip(1)).ToList().ForEach((a) =>
                {
                    a.HasChanged = false;
                    a.ReportStatus = ImportStatus.Skip;
                    a.SkipMessage = _skipMessage;
                });
            var validCandidates = _excelCandidates.Where(a => a.ReportStatus == ImportStatus.NA).ToList();

            if (validCandidates.Any())
            {
                var courseCodes = validCandidates.Select(a => a.CourseCode.ToLower()).Distinct();
                var courseRosters = await _dbRepository.Query(cxt => cxt.Set<CourseRoster>()).AsNoTracking()
                    .Include(a => a.Course).ThenInclude(a => a.CourseOrganizations).ThenInclude(a => a.Organization).ThenInclude(a => a.DataStructure)
                    .Include(a => a.Course.Semester)
                    .Include(a => a.Class)
                    .Include(a => a.Candidate)
                    .Where(a => courseCodes.Contains(a.Course.Code.ToLower())
                    && a.Course.DeleteStatus == FeatureCommon.Domain.DeleteStatus.Ok
                    && a.Candidate.UserStatus == UserStatus.Active).ToListAsync();
                var rosterKeys = enableSemester ? courseRosters.Select(a => a.Course.CourseOrganizationPath.ToLower() + a.Course.Code.ToLower() + (a.Class == null ? string.Empty : a.Class.Name.ToLower()) + (!string.IsNullOrEmpty(a.Candidate.Username) ? a.Candidate.Username.ToLower() : a.Candidate.DisplayName.ToLower()) + a.Course.Semester.Name.ToLower()).ToList()
                    : courseRosters.Select(a => a.Course.CourseOrganizationPath.ToLower() + a.Course.Code.ToLower() + (a.Class == null ? string.Empty : a.Class.Name.ToLower()) + (!string.IsNullOrEmpty(a.Candidate.Username) ? a.Candidate.Username.ToLower() : a.Candidate.DisplayName.ToLower())).ToList();
                var validKeys = validCandidates.Select(a => a.Key).Distinct().ToList();
                var addKeys = validKeys.Except(rosterKeys);

                validCandidates.ForEach(a =>
                {
                    if (addKeys.Contains(a.Key))
                    {
                        a.HasChanged = true;
                    }
                    else
                    {
                        a.ReportStatus = ImportStatus.Skip;
                        a.SkipMessage = _skipMessage;
                    }
                });
                var addCandidates = _excelCandidates.Where(a => a.HasChanged).ToList();

                addCandidates.GroupBy(a => a.Key)
                    .Where(a => a.Count() > 1)
                    .SelectMany(a => a.Skip(1)).ToList().ForEach((a) =>
                    {
                        a.HasChanged = false;
                        a.ReportStatus = ImportStatus.Skip;
                        a.SkipMessage = _skipMessage;
                    });

                //排除candidate以组名或者邮件方式添加，但实际两者相同的情况
                List<string> needAddCodeGroups = new List<string>();
                addCandidates = _excelCandidates.Where(a => a.HasChanged).ToList();
                foreach (var item in addCandidates)
                {
                    Guid groupIdIndb = GetGoupIdInDb(item.CandidateEmail.ToLower(), UserType.Candidate);
                    if (groupIdIndb != default(Guid))
                    {
                        string tempString = item.CourseCode + item.ClassName + groupIdIndb.ToString();
                        if (needAddCodeGroups.Contains(tempString))
                        {
                            item.HasChanged = false;
                            item.ReportStatus = ImportStatus.Skip;
                            item.SkipMessage = _skipMessage;
                        }
                        else
                        {
                            needAddCodeGroups.Add(tempString);
                        }
                    }
                }
            }
        }

        private async Task<List<Guid>> UpdateCandidateAsync()
        {
            var vResult = new List<Guid>();
            var changedCandidates = _excelCandidates.Where(a => a.HasChanged).ToList();
            _currentDataRow = _excelCandidates.Count - changedCandidates.Count;
            await UpdataProgressBar(0.01m + ((decimal)(_currentDataRow) / _totalDataRow * _miniProgressBar));
            if (changedCandidates.Any())
            {
                var courseCodes = changedCandidates.AsParallel().Select(a => a.CourseCode.ToLower()).Distinct().ToList();
                var courses = await _dbRepository.Query(cxt => cxt.Set<Course>()
                 .Include(a => a.CourseOrganizations).ThenInclude(a => a.Organization).ThenInclude(a => a.DataStructure)
                 .Include(a => a.Semester)
                 .Where(a => courseCodes.Contains(a.Code.ToLower()) && a.DeleteStatus == DeleteStatus.Ok)).ToListAsync();
                var courseids = courses.Select(a => a.Id);
                vResult = courseids.ToList();
                var classes = await _dbRepository.Query(cxt => cxt.Set<CourseClass>()
                 .Include(i => i.Course).ThenInclude(a => a.CourseOrganizations).ThenInclude(a => a.Organization).ThenInclude(a => a.DataStructure)
                 .Include(i => i.CourseClassOwners).ThenInclude(a => a.Owner)
                 .Where(a => courseids.Contains(a.CourseId) && a.Course.DeleteStatus == DeleteStatus.Ok)).ToListAsync();

                var courseRosterIds = changedCandidates.Where(a => !string.IsNullOrEmpty(a.CourseRosterId)).Select(a => a.CourseRosterId.ToLower()).Distinct().ToList();
                //var candidateUPNs = changedCandidates.Where(a => a.CandidateUPN!=null || a.CandidateUPN != Guid.Empty).Select(a => a.CandidateUPN).Distinct().ToList();
                var courseRosters = await _dbRepository.Context.Set<CourseRoster>().AsNoTracking().Where(i => courseRosterIds.Contains(i.Id.ToString().ToLower())).ToListAsync();
                var allTaregtCourseRoster = await _dbRepository.Context.Set<CourseRoster>().AsNoTracking().Where(i => courseids.Contains(i.CourseId)).ToListAsync();

                Dictionary<string, CandidateModel> needAdd = new Dictionary<string, CandidateModel>();
                List<CourseRoster> needUpdate = new List<CourseRoster>();
                List<Guid> haveChangedCourseId = new List<Guid>();
                Dictionary<Guid, Guid> updateClassId = new Dictionary<Guid, Guid>();
                foreach (var item in changedCandidates.AsParallel().ToList())
                {
                    var addCandidates = true;
                    var tempCandicateId = item.CandidateEmail.ToLower().Replace("；", ";").Replace(";", "");
                    var userId = GetGoupIdInDb(tempCandicateId, UserType.Candidate);
                    //var course = courses.FirstOrDefault(a => string.Equals(a.Code, item.CourseCode, StringComparison.OrdinalIgnoreCase) && string.Equals(a.CourseOrganizationPath, item.DynamicColumnValueStr, StringComparison.OrdinalIgnoreCase) && string.Equals(a.Semester.Name, item.Semester, StringComparison.OrdinalIgnoreCase));
                    var course = enableSemester ? courses.FirstOrDefault(a => string.Equals(a.Code, item.CourseCode, StringComparison.OrdinalIgnoreCase) && string.Equals(a.CourseOrganizationPath, item.DynamicColumnValueStr, StringComparison.OrdinalIgnoreCase) && string.Equals(a.Semester.Name, item.Semester, StringComparison.OrdinalIgnoreCase))
                        : courses.FirstOrDefault(a => string.Equals(a.Code, item.CourseCode, StringComparison.OrdinalIgnoreCase) && string.Equals(a.CourseOrganizationPath, item.DynamicColumnValueStr, StringComparison.OrdinalIgnoreCase));
                    if (course == null)
                    {
                        item.HasChanged = false;
                        item.ReportStatus = ImportStatus.Failed;
                        item.InvalidMessage.Add(I18NEntity.GetString("Admin_ImportCourseClass_Course_MissMatch_Message", item.CourseCode));
                    }
                    else
                    {
                        if (!haveChangedCourseId.Exists(a => a == course.Id))
                        {
                            haveChangedCourseId.Add(course.Id);
                        }
                        Guid? classId = null;
                        var courseClasses = classes.Where(a => a.CourseId == course.Id).ToList();
                        if ((string.IsNullOrEmpty(item.ClassName) || item.ClassName == "N/A") && courseClasses != null && courseClasses.Any())
                        {
                            addCandidates = false;
                            item.HasChanged = false;
                            item.ReportStatus = ImportStatus.Failed;
                            item.InvalidMessage.Add(I18NEntity.GetString("Admin_ImportCourseClass_Class_MissMatch_Message", item.ClassName));
                        }
                        if (!string.IsNullOrEmpty(item.ClassName) && item.ClassName != "N/A")
                        {
                            var coueseClass = courseClasses.FirstOrDefault(a => string.Equals(a.Name, item.ClassName, StringComparison.OrdinalIgnoreCase));
                            if (coueseClass != null)
                            {
                                classId = coueseClass.Id;
                                if (!updateClassId.ContainsKey(course.Id))
                                {
                                    updateClassId.Add(course.Id, coueseClass.Id);
                                }
                            }
                            else
                            {
                                addCandidates = false;
                                item.HasChanged = false;
                                item.ReportStatus = ImportStatus.Failed;
                                item.InvalidMessage.Add(I18NEntity.GetString("Admin_ImportCourseClass_Class_MissMatch_Message", item.ClassName));
                            }
                        }
                        //change class 
                        var courseRoster = courseRosters.Where(i => item.CourseRosterId == i.Id.ToString()).FirstOrDefault();
                        var taregtCourseRoster = allTaregtCourseRoster.Where(i => course.Id == i.CourseId).Select(i => i.CandidateId).ToList();
                        if (courseRoster != null && courseRoster.CourseId != course.Id)
                        {
                            if (taregtCourseRoster.Exists(a => a == courseRoster.CandidateId))
                            {
                                addCandidates = false;
                                item.HasChanged = false;
                                item.ReportStatus = ImportStatus.Failed;
                                item.InvalidMessage.Add(I18NEntity.GetString("Admin_ImportCourseClass_Candidate_exist_Message"));
                            }
                            if (addCandidates)
                            {
                                courseRoster.CourseId = course.Id;
                                courseRoster.ClassId = classId;
                                courseRoster.ModifiedTime = DateTimeOffset.UtcNow;
                                needUpdate.Add(courseRoster);
                            }
                        }
                        else
                        {
                            string classIdStr = classId.HasValue ? classId.Value.ToString() : string.Empty;
                            if (addCandidates)
                            {
                                if (_candidatesGroupMembers.ContainsKey(userId))
                                {
                                    var candiateModel = new CandidateModel()
                                    {
                                        CourseId = course.Id,
                                        ClassId = classId,
                                        CandidateId = userId,
                                        CandidateType = UserType.CandidateGroup,
                                        ModifiedTime = DateTimeOffset.UtcNow,
                                    };
                                    needAdd[candiateModel.CourseId.ToString() + classIdStr + candiateModel.CandidateId.ToString()] = candiateModel;
                                }
                                else
                                {
                                    var candiateModel = new CandidateModel()
                                    {
                                        CourseId = course.Id,
                                        ClassId = classId,
                                        CandidateId = _candidates[item.CandidateEmail.ToLower().Replace("；", ";").Replace(";", "")],
                                        CandidateType = UserType.Candidate,
                                        ModifiedTime = DateTimeOffset.UtcNow,
                                    };
                                    needAdd[candiateModel.CourseId.ToString() + classIdStr + candiateModel.CandidateId.ToString()] = candiateModel;
                                }
                            }
                        }
                    }
                    _currentDataRow++;
                    await UpdataProgressBar(0.01m + ((decimal)(_currentDataRow) / _totalDataRow * _miniProgressBar));
                }
                if (needUpdate.Count > 0)
                {
                    _dbRepository.Context.UpdateRange(needUpdate);
                    await _dbRepository.SaveChangesAsync();
                }
                List<CandidateModel> rosters = needAdd.Values.ToList<CandidateModel>();
                var command = new BulkAddCandidatesCommand()
                {
                    Candidates = rosters
                };

                var count = await _mediator.Send(command);

                //var AllCourseRosters = await _dbRepository.Context.Set<CourseRoster>().Where(a => haveChangedCourseId.Contains(a.CourseId)).ToListAsync();
                //foreach (var courseId in haveChangedCourseId)
                //{
                //    var CourseRosters = AllCourseRosters.Where(a => a.CourseId == courseId);
                //    //classid 全为空说明没有class 不需要重置 class 为NA的学生  classid 全不为空 说明都有class 不需要更新
                //    if (!(CourseRosters.All(a => a.ClassId != null) || CourseRosters.All(a => a.ClassId == null)) && updateClassId.TryGetValue(courseId, out var classId))
                //    {
                //        var needUpdateCourseRosters = CourseRosters.Where(a => a.ClassId == null).ToList();
                //        needUpdateCourseRosters.ForEach(a => a.ClassId = classId);
                //        _dbRepository.Context.UpdateRange(needUpdateCourseRosters);
                //        await _dbRepository.SaveChangesAsync();
                //    }
                //}
                //if (count == rosters.Count)
                //{
                changedCandidates.ForEach(a =>
                {
                    if (a.ReportStatus == ImportStatus.NA)
                    {
                        a.ReportStatus = ImportStatus.Success;
                    }
                });
                //}
            }
            return vResult;
        }

        private async Task GenerateReport(Workbook workbook, ImportReportInfo report, Dictionary<string, Worksheet> originalSheets, String language)
        {
            ExcelLanguageType excellanguage = ExcelLanguageType.English;
            var languageTemplateName = String.Empty;
            //Switch Japanese Template if Japanese
            if (!String.IsNullOrEmpty(language) && language.IndexOf("ja-jp", StringComparison.OrdinalIgnoreCase) > -1)
            {
                languageTemplateName = "_jp";
                excellanguage = ExcelLanguageType.Japanese;
            }
            //Switch German Template if German
            else if (!String.IsNullOrEmpty(language) && language.IndexOf("de-de", StringComparison.OrdinalIgnoreCase) > -1)
            {
                languageTemplateName = "_de";
                excellanguage = ExcelLanguageType.German;
            }
            //Switch Chinese Template if Chinese
            else if (!String.IsNullOrEmpty(language) && language.IndexOf("zh-cn", StringComparison.OrdinalIgnoreCase) > -1)
            {
                languageTemplateName = "_cn";
                excellanguage = ExcelLanguageType.Chinese;
            }
            //Switch Dutch Template if Dutch
            else if (!String.IsNullOrEmpty(language) && language.IndexOf("nl-nl", StringComparison.OrdinalIgnoreCase) > -1)
            {
                languageTemplateName = "_nl";
                excellanguage = ExcelLanguageType.Dutch;
            }

            var courseTemplate = String.Format(Constants.CourseTemplate, languageTemplateName);

            foreach (var sheet in originalSheets)
            {
                if (sheet.Key.EqualsIgnoreCase(sheetName_Course))
                {
                    ExcelHelper.UpdateWorksheetWithReport(sheet.Value, _excelCourse, excellanguage);
                }
                if (sheet.Key.EqualsIgnoreCase(sheetName_Classes))
                {
                    ExcelHelper.UpdateWorksheetWithReport(sheet.Value, _excelClasses, excellanguage);
                }
                if (sheet.Key.EqualsIgnoreCase(sheetName_Candidates))
                {
                    ExcelHelper.UpdateWorksheetWithReport(sheet.Value, _excelCandidates, excellanguage);
                }
            }

            var allRow = new List<ExcelDynamic>();
            allRow.AddRange(_excelCourse);
            allRow.AddRange(_excelClasses);
            allRow.AddRange(_excelCandidates);

            if (allRow.All(a => a.ReportStatus == ImportStatus.Skip))
            {
                report.Status = ImportReportStatus.Skip;
            }
            else if (allRow.All(a => a.ReportStatus == ImportStatus.Failed))
            {
                report.Status = ImportReportStatus.Failed;
            }
            else if (!allRow.Any(a => a.ReportStatus == ImportStatus.Failed))
            {
                report.Status = ImportReportStatus.Success;
            }
            else if (allRow.Any(a => a.ReportStatus == ImportStatus.Failed))
            {
                report.Status = ImportReportStatus.PartialFailed;
            }

            var newStream = new MemoryStream();
            workbook.Save(newStream, SaveFormat.Xlsx);
            //report.Report = await ExcelHelper.CreateReport(newStream, _distributedCache);
            report.ReportBytes = newStream.ToArray();

            //using (Workbook workbook = new Workbook(courseTemplate))
            //{
            //    var sampleDataSheet = workbook.Worksheets[sheetName_SampleData];
            //    using (var stream = ExcelHelper.CreateExcelBySheetList(new List<Worksheet> { sampleDataSheet, originalSheets[sheetName_Course], originalSheets[sheetName_Candidates] }, 1))
            //    {
            //        report.Report = await ExcelHelper.CreateReport(stream, _distributedCache);
            //    }
            //}
        }

        private void ValidateEmail(ExcelDynamic row, string inputEmail, UserType userType, string ColumnName)
        {
            var users = userType == UserType.Staff ? _staffs : _candidates;

            var cellValue = inputEmail.ToLower();
            if (!IsEmail(cellValue))
            {
                row.ReportStatus = ImportStatus.Failed;
                row.InvalidMessage.Add(I18NEntity.GetString("Admin_ImportCourse_EmailFormat_Invalid_Message", new[] { inputEmail, ColumnName }));
            }
            else if (cellValue.Length > 256)
            {
                row.ReportStatus = ImportStatus.Failed;
                row.InvalidMessage.Add(I18NEntity.GetString("Admin_ImportCourse_Column_MoreThan_Message", new[] { ColumnName, "256" }));
            }
            else if (!users.ContainsKey(cellValue))
            {
                row.ReportStatus = ImportStatus.Failed;
                row.InvalidMessage.Add(I18NEntity.GetString("Admin_ImportCourse_Email_NotExist_Message", new[] { inputEmail, userType.ToDescription().ToLower() }));
            }
        }

        private bool IsValidEmail(string inputEmail, UserType userType)
        {
            var result = false;
            var cellValue = inputEmail.ToLower();
            if (userType == UserType.Staff)
            {
                result = IsEmail(cellValue) && (IsInGroup(cellValue, _staffs));
            }
            else
            {
                result = IsEmail(cellValue) && (IsInGroup(cellValue, _candidates) || IsInGroup(cellValue, _candidatesEmailGroup) || IsInGroup(cellValue, _candidatesNameGroup));
            }
            return result;
        }

        private bool IsEmail(string input)
        {
            string emailStr = @"^[A-Za-z0-9\u4e00-\u9fa5.#_-]+@[a-zA-Z0-9_-]+(\.[a-zA-Z0-9_-]+)+$";
            var emailReg = new Regex(emailStr);
            return emailReg.IsMatch(input);
        }

        /// <summary>
        /// 获取tree结构
        /// </summary>
        /// <param name="currOrg"></param>
        /// <param name="organizationList"></param>
        /// <returns></returns>
        private OrganizationTree GetOrganizationTree(Organization currOrg, List<Organization> organizationList)
        {
            OrganizationTree organization = new OrganizationTree();
            organization.Id = currOrg.Id;
            organization.Name = currOrg.Name;
            organization.ParentId = currOrg.ParentId;
            organization.StructureId = currOrg.StructureId;
            organization.Description = currOrg.Description;
            organization.StructureLevel = currOrg.DataStructure?.Level;
            organization.StructureName = currOrg.DataStructure?.Name;
            var organizations = organizationList.FindAll(x => x.ParentId == currOrg.Id).OrderBy(x => x.CreatedTime);
            List<OrganizationTree> list = new List<OrganizationTree>();
            foreach (var item in organizations)
            {
                list.Add(GetOrganizationTree(item, organizationList));
            }
            organization.Children = list;
            return organization;
        }

        private Dictionary<string, string> FilterOrganizationTreeNames(string names, string values, OrganizationTree preNode, Dictionary<string, string> list)
        {
            names += preNode.Name.ToLower() + "/";
            values += preNode.Id.ToString().ToLower() + "/";
            if (preNode.Children == null || preNode.Children.Count < 1)
                list.Add(names.TrimEnd('/'), values.TrimEnd('/'));
            if (preNode.Children != null)
            {
                foreach (var item in preNode.Children)
                {
                    FilterOrganizationTreeNames(names, values, item, list);
                }
            }
            return list;
        }
    }

    public class ProductSheetModel
    {
        public string ProductName { get; set; }
        public string Description { get; set; }
        public int Price { get; set; }
        public string CategoryName { get; set; }
    }

    public class ImageSheetModel
    {
        public string ProductName { get; set; }
        public string Image { get; set; }
    }
}
