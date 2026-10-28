using manaslearning.Admin.UI.Models;
using manaslearning.Admin.UI.ResponseModels;
using manaslearning.Admin.UI.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.JSInterop;
using Microsoft.VisualBasic.FileIO;

namespace manaslearning.Admin.UI.Pages.Users;

public partial class CreateBulkUsers
{
    [Inject] ILogger<CreateBulkUsers> _logger { get; set; }
    [Inject] IJSRuntime _jsRuntime { get; set; }
    [Inject] IUserService _userService { get; set; }

    [Parameter] public EventCallback Completed { get; set; }

    private UploadState FileUploadState = UploadState.None;
    private string errorMessage;

    private async Task OnInputFileChange(InputFileChangeEventArgs e)
    {
        List<SchoolUserAccount> schoolUserAccounts = new List<SchoolUserAccount>();
        FileUploadState = UploadState.InProgress;
        Console.WriteLine($"UploadState: {FileUploadState}");
        try
        {
            var file = e.File;
            _logger.LogInformation($"File: {file.Name}");

            using (Stream stream = file.OpenReadStream())
            using (MemoryStream ms = new MemoryStream())
            {
                await stream.CopyToAsync(ms);
                stream.Close();

                var memoryStream = new MemoryStream(ms.ToArray());

                var textFieldParser = new TextFieldParser(memoryStream)
                {
                    Delimiters = new string[] { "\t" }, // Tab delimited text file
                    TrimWhiteSpace = true,
                    HasFieldsEnclosedInQuotes = true
                };

                var row = textFieldParser.ReadLine(); // Skip first row with column names

                while (!textFieldParser.EndOfData)
                {
                    try
                    {
                        var currentRowFields = textFieldParser.ReadFields();
                        if (currentRowFields == null)
                        {
                            continue;
                        }
                        else if (currentRowFields.Count() != 25)
                        {
                            throw new InvalidDataException("Make sure there are 25 fields in each row.");
                        }

                        var schoolUserAccount = new SchoolUserAccount();
                        schoolUserAccount.SchoolName = currentRowFields[0].Trim();
                        schoolUserAccount.SchoolDisplayName = currentRowFields[1].Trim();
                        schoolUserAccount.Email = currentRowFields[2].Trim();
                        schoolUserAccount.TeacherFirstName = currentRowFields[3].Trim();
                        schoolUserAccount.TeacherLastName = currentRowFields[4].Trim();
                        schoolUserAccount.SchoolAddressLine1 = currentRowFields[5].Trim();
                        schoolUserAccount.SchoolAddressLine2 = currentRowFields[6].Trim();
                        schoolUserAccount.SchoolAddressLine3 = currentRowFields[7].Trim();
                        schoolUserAccount.SchoolCity = currentRowFields[8].Trim();
                        schoolUserAccount.SchoolState = currentRowFields[9].Trim();
                        schoolUserAccount.SchoolCountry = currentRowFields[10].Trim();
                        schoolUserAccount.SchoolZipCode = currentRowFields[11].Trim();
                        schoolUserAccount.SchoolPhone1 = currentRowFields[12].Trim();
                        schoolUserAccount.SchoolPhone2 = currentRowFields[13].Trim();

                        schoolUserAccount.Licenses = new List<SchoolUserLicense>();
                        schoolUserAccount.Licenses.Add(new SchoolUserLicense
                        {
                            AssessmentCode = "ADHD",
                            LicenseCount = int.TryParse(currentRowFields[14], out int adhd) ? adhd : adhd
                        });

                        schoolUserAccount.Licenses.Add(new SchoolUserLicense
                        {
                            AssessmentCode = "Autism",
                            LicenseCount = int.TryParse(currentRowFields[15], out int autism) ? autism : autism
                        });

                        schoolUserAccount.Licenses.Add(new SchoolUserLicense
                        {
                            AssessmentCode = "LD",
                            LicenseCount = int.TryParse(currentRowFields[16], out int ld) ? ld : ld
                        });

                        schoolUserAccount.Licenses.Add(new SchoolUserLicense
                        {
                            AssessmentCode = "Dyslexia",
                            LicenseCount = int.TryParse(currentRowFields[17], out int dyslexia) ? dyslexia : dyslexia
                        });

                        schoolUserAccount.Licenses.Add(new SchoolUserLicense
                        {
                            AssessmentCode = "Dyscalculia",
                            LicenseCount = int.TryParse(currentRowFields[18], out int dyscalculia) ? dyscalculia : dyscalculia
                        });

                        schoolUserAccount.Licenses.Add(new SchoolUserLicense
                        {
                            AssessmentCode = "Dysgraphia",
                            LicenseCount = int.TryParse(currentRowFields[19], out int dysgraphia) ? dysgraphia : dysgraphia
                        });

                        schoolUserAccount.Licenses.Add(new SchoolUserLicense
                        {
                            AssessmentCode = "Dyspraxia",
                            LicenseCount = int.TryParse(currentRowFields[20], out int dyspraxia) ? dyspraxia : dyspraxia
                        });

                        schoolUserAccount.Licenses.Add(new SchoolUserLicense
                        {
                            AssessmentCode = "Attention",
                            LicenseCount = int.TryParse(currentRowFields[21], out int attention) ? attention : attention
                        });

                        schoolUserAccount.Licenses.Add(new SchoolUserLicense
                        {
                            AssessmentCode = "Memory",
                            LicenseCount = int.TryParse(currentRowFields[22], out int memory) ? memory : memory
                        });

                        schoolUserAccount.Licenses.Add(new SchoolUserLicense
                        {
                            AssessmentCode = "Emotion",
                            LicenseCount = int.TryParse(currentRowFields[23], out int emotion) ? emotion : emotion
                        });

                        schoolUserAccount.Licenses.Add(new SchoolUserLicense
                        {
                            AssessmentCode = "Dysorthographia",
                            LicenseCount = int.TryParse(currentRowFields[24], out int dysorthographia) ? dysorthographia : dysorthographia
                        });

                        schoolUserAccounts.Add(schoolUserAccount);
                    }
                    catch (MalformedLineException ex)
                    {
                        _logger.LogError($"Line {ex.Message} is invalid. Skipping");
                    }
                }

                var response = await _userService.CreateBulkUsersAsync(schoolUserAccounts);

                var result = response.Data as BulkUsersResponseModel;
                if (result == null)
                {
                    FileUploadState = UploadState.Error;
                    errorMessage = $"File: {file.Name} ErrorMessage: response can not be null.";
                    _logger.LogError($"File: {file.Name} ErrorMessage: response can not be null.");
                    return;
                }

                await _jsRuntime.InvokeVoidAsync("saveAsFile", result.FileName, Convert.ToBase64String(result.Content));
                FileUploadState = UploadState.Complete;
                await Completed.InvokeAsync();
            }
        }
        catch (Exception ex)
        {
            FileUploadState = UploadState.Error;
            errorMessage = ex.Message;
            _logger.LogError($"ErrorMessage: {ex.Message}");
        }
    }
}

public enum UploadState
{
    None,
    InProgress,
    Complete,
    Error
}
