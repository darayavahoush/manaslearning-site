using manaslearning.Business.Interfaces;
using manaslearning.Helpers;
using manaslearning.UI.Attributes;
using manaslearning.UI.Models;
using manaslearning.UI.ViewModels;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace manaslearning.UI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OnlineEnquiryController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger _logger;
        private readonly IEnquiryRepository _enquiryRepository;
        private readonly IEmailSender _emailSender;

        public OnlineEnquiryController(
            IConfiguration configuration,
            ILogger<OnlineEnquiryController> logger,
            IEnquiryRepository enquiryRepository,
            IEmailSender emailSender)
        {
            _configuration = configuration;
            _logger = logger;
            _enquiryRepository = enquiryRepository;
            _emailSender = emailSender;
        }

        [HttpPost]
        public async Task<IActionResult> PostAsync([FromBody] OnlineEnquiryRequestModel model)
        {
            try
            {
                var validationResult = model.IsValid();
                if (!validationResult.Item1)
                {
                    return BadRequest(new { message = validationResult.Item2 });
                }
                
                model.Name = model.Name.CleanInput();               

                model.Message = model.Message.CleanInput();

                // Validate google reCAPTCHA
                GoogleCaptchaResponse result = await ValidateCaptcha(model.Token);
                if (!result.success || result.score < 0.8)
                    return BadRequest(new { message = "Invalid request from a bot." }); // TODO: Log this case to database

                var inquiryId = await _enquiryRepository.CreateEnquiryAsync(
                    model.Name,
                    model.Email,
                    model.Phone,
                    model.City,
                    model.State,
                    model.Country,
                    model.ChildAge,
                    JsonConvert.SerializeObject(model.ServiceRequest),
                    model.Message);

                if (inquiryId == -1)
                    return Ok(new { IsSuccess = false });

                await SendEmailToUserAsync(inquiryId, model);

                return Ok(new { IsSuccess = true });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message, new { Source = "OnlineEnquiryController.PostAsync" });
                return await Task.FromResult(StatusCode((int)HttpStatusCode.InternalServerError, new { IsSuccess = false }));
            }
        }

        // GET api/OnlineEnquiry?EnquiryId={enquiryId}
        [HttpGet("{enquiryId:int}")]
        public async Task<IActionResult> GetAsync(int enquiryId)
        {
            try
            {
                var enquiry = await _enquiryRepository.GetEnquiryAsync(enquiryId);
                if (enquiry == null)
                {
                    return NotFound();
                }

                return Ok(new OnlineEnquiryDto
                {
                    EnquiryId = enquiry.EnquiryId,
                    ChildAge = enquiry.ChildAge,
                    City = enquiry.City,
                    Country = enquiry.Country,
                    Email = enquiry.Email,
                    Message = enquiry.Message,
                    Name = enquiry.Name,
                    Phone = enquiry.Phone,
                    ServiceRequest = enquiry.ServiceRequest,
                    State = enquiry.State
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"{ex.Message}. Source=OnlineEnquiryController.GetAsync();EnquiryId={enquiryId}");
                return this.StatusCode((int)HttpStatusCode.InternalServerError);
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetAsync()
        {
            try
            {
                var enquiries = await _enquiryRepository.GetEnquiriesAsync();
                if (enquiries == null || !enquiries.Any())
                    return NotFound();

                return Ok(enquiries
                    .OrderByDescending(x => x.EnquiryId)
                    .Select(e => new OnlineEnquiryDto
                    {
                        EnquiryId = e.EnquiryId,
                        ChildAge = e.ChildAge,
                        City = e.City,
                        Country = e.Country,
                        Email = e.Email,
                        Message = e.Message,
                        Name = e.Name,
                        Phone = e.Phone,
                        ServiceRequest = e.ServiceRequest,
                        State = e.State
                    }));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"{ex.Message}. Source=EnquiryController.GetAsync()");
                return this.StatusCode((int)HttpStatusCode.InternalServerError);
            }
        }

        private async Task SendEmailToUserAsync(int inquiryId, OnlineEnquiryRequestModel model)
        {
            var environment = _configuration.GetValue<string>("Environment");
            var supportEmail = _configuration.GetValue<string>("Company:Info:SupportEmail"); // TODO: Read from separate DL

            // email to user
            if (!string.IsNullOrWhiteSpace(model.Email))
            {
                var userReplySubject = $"Thank you for contacting us!";
                var userReplyMessage = new StringBuilder();
                userReplyMessage.AppendLine("<p><strong>Thank you for contacting us.</strong></p>");
                userReplyMessage.AppendLine("<p>We have received your inquiry and will respond to you very soon. For urgent inquiries, please call us at <a href='tel: +919342157217'>(+91) 934-215-7217</a>.</p>");
                userReplyMessage.AppendLine($"<p>You can also email us at <a href='mailto: {supportEmail}'>{supportEmail}</a>.</p>");
                userReplyMessage.AppendLine("<br />");
                userReplyMessage.AppendLine("<p><strong>Thank You,</strong><br />The MANAS Learning Team</p>");

                await _emailSender.SendEmailAsync(
                                    model.Email,
                                    (string.IsNullOrWhiteSpace(environment) || environment == "PROD") ? $"{userReplySubject}" : $"{environment}: {userReplySubject}",
                                    userReplyMessage.ToString());
            }

            // email to support team
            var supportEmailSubject = $"Inquiry #{inquiryId} from {model.Name}";
            var supportEmailMessage = new StringBuilder();
            supportEmailMessage.AppendLine($"<p><strong>Inquiry #{inquiryId}</strong></p>");
            supportEmailMessage.AppendLine($"<p>Name: {model.Name}</p>");
            supportEmailMessage.AppendLine($"<p>Email: {model.Email}</p>");
            supportEmailMessage.AppendLine($"<p>Phone: {model.Phone}</p>");
            supportEmailMessage.AppendLine($"<p>City: {model.City}</p>");
            supportEmailMessage.AppendLine($"<p>ChildAge: {model.ChildAge}</p>");
            supportEmailMessage.AppendLine($"<p>Service Request: {string.Join(",", model.ServiceRequest?.Types)} </p>");            
            supportEmailMessage.AppendLine($"<p>Message: <br />{model.Message}</p>");
            supportEmailMessage.AppendLine($"<p><strong>Thank You.</strong>");

            await _emailSender.SendEmailAsync(
                                supportEmail,
                                (string.IsNullOrWhiteSpace(environment) || environment == "PROD") ? $"{supportEmailSubject}" : $"{environment}: {supportEmailSubject}",
                                supportEmailMessage.ToString());
        }

        private async Task<GoogleCaptchaResponse> ValidateCaptcha(string token)
        {
            HttpClient httpClient = new HttpClient();
            var response = await httpClient.GetStringAsync($"https://www.google.com/recaptcha/api/siteverify?secret={_configuration.GetValue<string>("reCaptcha:SecretKey")}&response={token}");
            var result = JsonConvert.DeserializeObject<GoogleCaptchaResponse>(response);
            return result;
        }
    }
}
