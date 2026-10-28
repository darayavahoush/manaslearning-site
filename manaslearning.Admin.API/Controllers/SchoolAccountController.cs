using manaslearning.Admin.API.Attributes;
using manaslearning.Admin.API.Helpers;
using manaslearning.Admin.API.Models;
using manaslearning.Business.Interfaces;
using manaslearning.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SpreadsheetGear;
using System.Net;

namespace manaslearning.Admin.API.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class SchoolAccountController : ControllerBase
    {
        private readonly ILogger _logger;
        private readonly IOrganizationRepository _organizationRepository;
        private readonly IOrganizationAddressRepository _organizationAddressRepository;
        private readonly IProfileRepository _profileRepository;
        private readonly IAssessmentRepository _assessmentRepository;
        private readonly IItemRepository _itemRepository;
        private readonly ILicenseHistoryRepository _userLicenseHistoryRepository;

        private static Dictionary<int, char> lettersDictionary;
        private static char[] lettersArray = "ABCDEFGHIJKLMNOPQRSTUVWXYZ".ToCharArray();

        public SchoolAccountController(ILogger<SchoolAccountController> logger,
            IOrganizationRepository organizationRepository,
            IOrganizationAddressRepository organizationAddressRepository,
            IProfileRepository profileRepository,
            IAssessmentRepository assessmentRepository,
            IItemRepository itemRepository,
            ILicenseHistoryRepository userLicenseHistoryRepository)
        {
            _logger = logger;
            _organizationRepository = organizationRepository;
            _organizationAddressRepository = organizationAddressRepository;
            _profileRepository = profileRepository;
            _assessmentRepository = assessmentRepository;
            _itemRepository = itemRepository;
            _userLicenseHistoryRepository = userLicenseHistoryRepository;

            this.InitializeLettersDictionary();
        }

        [HttpPost]
        [SuppressModelStateInvalidFilter]
        public async Task<ActionResult> PostAsync(IEnumerable<SchoolUserAccountPostModel.SchoolUser> model)
        {
            try
            {
                IList<SchoolUserAccountResponseModel> response = new List<SchoolUserAccountResponseModel>();
                if (model == null || !model.Any() || model.Any(m => string.IsNullOrWhiteSpace(m.Email)))
                {
                    return new StatusCodeResult((int)HttpStatusCode.BadRequest);
                }

                var assessments = await _assessmentRepository.GetActiveAssessmentsAsync();
                var distinctAssessmentCodes = model.SelectMany(s => s.Licenses.Select(l => l.AssessmentCode)).Distinct();

                if (assessments == null || !assessments.Any() || !distinctAssessmentCodes.Any())
                { throw new Exception($"Error occured while fetching Assessments"); }

                var items = await _itemRepository.GetItemsInfoAsync();
                if (items == null || !items.Any())
                { throw new Exception($"Error occured while fetching Items"); }

                Dictionary<string, (int, int)> dicAssessmentItems = new Dictionary<string, (int, int)>();

                foreach (var assessmentCode in distinctAssessmentCodes)
                {
                    var assessment = assessments.Where(d => d.Code.ToLower() == assessmentCode.ToLower()).FirstOrDefault();
                    if (assessment == null)
                    { throw new Exception($"Error occured while fetching Assessment. AssessmentCode : {assessmentCode}"); }

                    var itemInfo = items.Where(i => i.AssessmentCode.ToLower() == assessmentCode.ToLower()).FirstOrDefault();
                    if (itemInfo == null)
                    { throw new Exception($"Error occured while fetching Item. AssessmentCode : {assessmentCode}"); }

                    dicAssessmentItems.Add(assessmentCode, (assessment.AssessmentId, itemInfo.ItemId));
                }

                int userCount = model.Count();
                _logger.LogInformation($"START: Create bulk users - Count: {userCount}");
                for (int index=0; index < userCount; index++)
                {
                    var schoolUser = model.ElementAt(index);
                    _logger.LogInformation($"Email: {schoolUser.Email} - {index}/{userCount} started");
                    var schoolUserAccountDto = new SchoolUserAccountResponseModel()
                    {
                        Email = schoolUser.Email,
                        SchoolName = schoolUser.SchoolName,
                        TeacherFirstName = schoolUser.TeacherFirstName,
                        TeacherLastName = schoolUser.TeacherLastName
                    };
                    try
                    {
                        var validationResult = schoolUser.IsValid();
                        if (validationResult.Item1) // Item1: bool
                        {
                            var organization = await _organizationRepository.GetOrganizationByNameAsync(schoolUser.SchoolName?.Trim());
                            int organizationResult = 0;
                            if (organization == null)
                            {
                                // Create Organization
                                organizationResult = await _organizationRepository.CreateOrganizationAsync(null, schoolUser.SchoolName?.Trim(), schoolUser.SchoolDisplayName?.Trim(), null, 1); // OrganizationType: 1 - School
                                _logger.LogInformation($"Organization: { schoolUser.SchoolName} created.");
                            }
                            else
                            {
                                // Update Organization
                                organizationResult = await _organizationRepository.UpdateOrganizationAsync(null, schoolUser.SchoolName?.Trim(), schoolUser.SchoolDisplayName?.Trim(), (int)OrganizationType.School, organization.ParentOrganizationId, null, organization.Oid);
                                _logger.LogInformation($"Organization: { schoolUser.SchoolName} already exists.");
                            }

                            if (organizationResult <= 0 && organizationResult == -2)
                            { throw new Exception($"Error occured while creating/updating organization, OrganizationResult : {organizationResult}"); }

                            // Get existing organization addresses                       
                            var organizationAddresses = await _organizationAddressRepository.GetOrganizationAddressesAsync(organizationResult);
                            if (!organizationAddresses.Any(oa => oa.AddressLine1?.Trim() == schoolUser.SchoolAddressLine1?.Trim()))
                            {
                                // Create Organization Address
                                int organizationAddressId = await _organizationAddressRepository.CreateOrganizationAddressAsync
                                                                      (
                                                                          organizationResult,
                                                                          1, // 1 - AddressTypeId
                                                                          schoolUser.SchoolAddressLine1?.Trim(),
                                                                          schoolUser.SchoolAddressLine2?.Trim(),
                                                                          schoolUser.SchoolAddressLine3?.Trim(),
                                                                          schoolUser.SchoolCity?.Trim(),
                                                                          schoolUser.SchoolState?.Trim(),
                                                                          schoolUser.SchoolCountry?.Trim(),
                                                                          schoolUser.SchoolZipCode?.Trim(),
                                                                          schoolUser.SchoolPhone1?.Trim(),
                                                                          schoolUser.SchoolPhone2?.Trim(),
                                                                          true // IsDefault 
                                                                      );
                                _logger.LogInformation($"OrganizationAddress: {schoolUser.SchoolAddressLine1} created.");
                                if (organizationAddressId < 0 && organizationAddressId != -2)
                                { throw new Exception($"Error occured while creating/updating organization address, OrganizationAddressResult : {organizationAddressId}"); }

                            }
                            else
                            {
                                _logger.LogInformation($"OrganizationAddress: {schoolUser.SchoolAddressLine1} already exists.");
                            }

                            // Create User
                            long userId = 0;
                            PasswordHashHelper passwordHashHelper = new PasswordHashHelper();
                            string password = passwordHashHelper.GenerateRandomPassword();
                            schoolUserAccountDto.Password = password;
                            var userProfile = await _profileRepository.GetUserProfileByEmailIdAsync(schoolUser.Email);
                            if (userProfile == null)
                            {
                                userId = await _profileRepository.CreateUserAsync
                                        (
                                           schoolUser.TeacherFirstName?.Trim(),
                                           schoolUser.TeacherLastName?.Trim(),
                                           $"{schoolUser.TeacherFirstName} {schoolUser.TeacherLastName}".Trim(),
                                           schoolUser.Email?.Trim(),
                                           true, //Email confirmed
                                           null, // PhoneNumber
                                           2, // OrganizationUser
                                           passwordHashHelper.HashPassword(password),
                                           PasswordHashHelper.NewSecurityStamp(),
                                           PasswordHashHelper.GenerateConcurrencyStampAsync(),
                                           organizationResult,
                                           true // System created
                                        );
                                if (userId < 0)
                                { throw new Exception($"Error occured while creating system user, EmailId: {schoolUser.Email}, Result: {userId}"); }

                                _logger.LogInformation($"User: {schoolUser.Email} created.");
                            }
                            else
                            {
                                userId = userProfile.UserId;
                                schoolUserAccountDto.Password = String.Empty;
                                schoolUserAccountDto.Remarks = "User Already exists.";
                                _logger.LogInformation($"User: {schoolUser.Email} already exists.");
                            }

                            foreach (var license in schoolUser.Licenses)
                            {
                                // Create User License history
                                if (license.LicenseCount > 0)
                                {
                                    int result = await _userLicenseHistoryRepository.CreateLicenseHistoryAsync(userId, dicAssessmentItems[license.AssessmentCode].Item1, dicAssessmentItems[license.AssessmentCode].Item2, license.LicenseCount, 0); // 0 - CreatedBy
                                    if (result < 0)
                                    { throw new Exception($"Error occured while creating licenses. EmailId: {schoolUser.Email}, AssessmentCode: {license.AssessmentCode} Result: {result}"); }
                                }
                                _logger.LogInformation($"User: {schoolUser.Email}, Assessment code: {license.AssessmentCode} - User licenses({license.LicenseCount}) added.");
                            }
                            schoolUserAccountDto.Status = "Success";
                        }
                        else
                        {
                            schoolUserAccountDto.Status = "Failed";
                            schoolUserAccountDto.Remarks = validationResult.Item2; // Item2: Errormessage
                        }                        
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex.Message, $"ErrorMessage={ex.Message}, Source=SchoolAccountController.PostAsync(), Email : {schoolUser.Email}, SchoolName: {schoolUser.SchoolName}");
                        schoolUserAccountDto.Status = "Failed";
                        schoolUserAccountDto.Remarks = ex.Message;
                    }
                    finally
                    {
                        _logger.LogInformation($"Email: {schoolUser.Email} ended");
                    }
                    response.Add(schoolUserAccountDto);                    
                }

                var report = GetUsersExcelAsStream(response);
                return Ok(new { Content = report, ContentType = AppConstants.ExcelFileContentType, FileName = $"BulkUsers_{DateTime.Now.Ticks}.xlsx" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, $"ErrorMessage={ex.Message}, Source=SchoolAccountController.PostAsync()");
                return new StatusCodeResult((int)HttpStatusCode.InternalServerError);
            }
        }

        private byte[] GetUsersExcelAsStream(IList<SchoolUserAccountResponseModel> schoolUserAccountResponseModels)
        {
            var workBook = Factory.GetWorkbook();

            #region Sheet 1 - Users

            var sheet1 = workBook.Worksheets[0];
            sheet1.Name = "Users";
            sheet1.Cells.Font.Name = "Arial";
            sheet1.Cells.Font.Size = 10;

            // Sheet 1 - Header
            var rowNumber = 1;

            var interiorColor = Color.FromArgb(112, 48, 160);
            var fontColor = Color.FromArgb(255, 255, 255);

            var snoCell = sheet1.Cells[$"{GetExcelColumnName(0)}{rowNumber}"];
            SetCell(snoCell, interiorColor, fontColor, 5.00, "Sno");

            var schoolNameCell = sheet1.Cells[$"{GetExcelColumnName(1)}{rowNumber}"];
            SetCell(schoolNameCell, interiorColor, fontColor, 50.00, "School Name");

            var userNameCell = sheet1.Cells[$"{GetExcelColumnName(2)}{rowNumber}"];
            SetCell(userNameCell, interiorColor, fontColor, 40.00, "User Name");

            var passwordCell = sheet1.Cells[$"{GetExcelColumnName(3)}{rowNumber}"];
            SetCell(passwordCell, interiorColor, fontColor, 15.00, "Password");

            var statusCell = sheet1.Cells[$"{GetExcelColumnName(4)}{rowNumber}"];
            SetCell(statusCell, interiorColor, fontColor, 15.00, "Status");

            var remarksCell = sheet1.Cells[$"{GetExcelColumnName(5)}{rowNumber}"];
            SetCell(remarksCell, interiorColor, fontColor, 50.00, "Remarks");

            // Sheet 1 - Details            
            rowNumber++;
            foreach (var item in schoolUserAccountResponseModels)
            {
                // Sno #
                var columnIndex = 0;
                var cell = sheet1.Cells[$"{GetExcelColumnName(columnIndex)}{rowNumber}"];
                cell.Value = rowNumber - 1;                

                // School Name
                columnIndex += 1;
                cell = sheet1.Cells[$"{GetExcelColumnName(columnIndex)}{rowNumber}"];
                cell.Value = item.SchoolName;                

                // User Name
                columnIndex += 1;
                cell = sheet1.Cells[$"{GetExcelColumnName(columnIndex)}{rowNumber}"];
                cell.Value = item.Email;                

                // Password
                columnIndex += 1;
                cell = sheet1.Cells[$"{GetExcelColumnName(columnIndex)}{rowNumber}"];
                cell.Value = item.Password;                

                // Status
                columnIndex += 1;
                cell = sheet1.Cells[$"{GetExcelColumnName(columnIndex)}{rowNumber}"];
                cell.Value = item.Status;                

                // Remarks
                columnIndex += 1;
                cell = sheet1.Cells[$"{GetExcelColumnName(columnIndex)}{rowNumber}"];
                cell.Value = item.Remarks;               

                rowNumber++;
            }

            #endregion Sheet 1 - Users

            return workBook.SaveToMemory(FileFormat.OpenXMLWorkbook);
        }

        private static void SetCell(IRange cell, Color interiorColor, Color fontColor, double cellWidth, string cellValue)
        {
            cell.Interior.Color = interiorColor;
            cell.Font.Color = fontColor;
            cell.Borders.LineStyle = LineStyle.Continous;
            cell.Value = cellValue;
            cell.ColumnWidth = cellWidth;
        }

        /// <summary>
        /// Initialize a dictionary with the english alphabet 0-25 = A-Z
        /// </summary>
        private void InitializeLettersDictionary()
        {
            lettersDictionary = new Dictionary<int, char>();

            for (int i = 0; i < lettersArray.Length; i++)
            {
                lettersDictionary.Add(i, lettersArray[i]);
            }
        }

        /// <summary>
        /// Gets the corresponding excel column name from an index.
        ///  Ex: 0 = A, 26 = AA
        /// </summary>
        /// <param name="columnIndex">Index starts from 0.</param>
        /// <returns>string</returns>
        private string GetExcelColumnName(int columnIndex)
        {
            int remainder = columnIndex % 26;
            int quotient = columnIndex / 26;

            if (quotient == 0) return lettersDictionary[remainder].ToString();
            else return GetExcelColumnName(quotient - 1) + lettersDictionary[remainder].ToString();
        }
    }      
}
