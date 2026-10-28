using manaslearning.Business.Interfaces;
using manaslearning.Helpers;
using manaslearning.UI.Models;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace manaslearning.UI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ContactController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger _logger;
        private readonly IContactUsRepository _contactRepository;
        private readonly IEmailSender _emailSender;

        public ContactController(
            IConfiguration configuration,
            ILogger<ContactController> logger,
            IContactUsRepository contactRepository,
            IEmailSender emailSender)
        {
            _configuration = configuration;
            _logger = logger;
            _contactRepository = contactRepository;
            _emailSender = emailSender;
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Post(ContactRequestModel model)
        {
            try
            {
                // Input model null check
                if (model == null || model.Token.Trim().Length == 0)
                    return BadRequest(new { message = "Bad request." });

                // Name validation
                if (string.IsNullOrWhiteSpace(model.Name))
                    return BadRequest(new { message = "Please enter a valid name." });

                var name = model.Name?.Trim();
                if (name.Length > 255)
                    return BadRequest(new { message = "Please enter a valid name." });

                name = name.CleanInput();

                // Email validation
                if (string.IsNullOrWhiteSpace(model.Email))
                    return BadRequest(new { message = "Please enter a valid email." });

                var email = model.Email.Trim();
                if (email.Trim().Length == 0)
                    return BadRequest(new { message = "Please enter a valid email." });
                else if (email.Length > 255)
                    return BadRequest(new { message = "Please enter a valid email." });
                else if (!email.IsValidEmail())
                    return BadRequest(new { message = "Please enter a valid email." });

                // Subject validation
                if (string.IsNullOrWhiteSpace(model.Subject))
                    return BadRequest(new { message = "Please enter a valid subject." });

                var subject = model.Subject;
                if (subject.Trim().Length == 0)
                    return BadRequest(new { message = "Please enter a valid subject." });
                else if (subject.Length > 500)
                    return BadRequest(new { message = "Please enter a valid subject." });

                subject = subject.CleanInput();

                // Message validation
                if (string.IsNullOrWhiteSpace(model.Message))
                    return BadRequest(new { message = "Please enter a valid message." });

                var message = model.Message;
                if (message.Trim().Length == 0)
                    return BadRequest(new { message = "Please enter a valid message." });
                else if (message.Length > 1000)
                    return BadRequest(new { message = "Please enter a valid message." });

                message = message.CleanInput();

                // Validate google reCAPTCHA
                GoogleCaptchaResponse result = await ValidateCaptcha(model.Token);
                if (!result.success || result.score < 0.8)
                    return BadRequest(new { message = "Invalid request from a bot." }); // TODO: Log this case to database

                var inquiryId = await _contactRepository.SaveContactUs(model.Name, model.Email, model.Subject, model.Message);
                if (inquiryId == -1)
                    return Ok(new { IsSuccess = false });

                await SendEmailToUserAsync(inquiryId, model);

                return Ok(new { IsSuccess = true });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message, new { Source = "ContactController.Post" });
                return await Task.FromResult(StatusCode((int)HttpStatusCode.InternalServerError, new { IsSuccess = false }));
            }
        }

        private async Task SendEmailToUserAsync(int inquiryId, ContactRequestModel model)
        {
            var environment = _configuration.GetValue<string>("Environment");
            var supportEmail =  _configuration.GetValue<string>("Company:Info:SupportEmail");

            // email to user
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

            // email to support team
            var supportEmailSubject = $"Inquiry #{inquiryId} from {model.Name}";
            var supportEmailMessage = new StringBuilder();
            supportEmailMessage.AppendLine($"<p><strong>Inquiry #{inquiryId}</strong></p>");
            supportEmailMessage.AppendLine($"<p>Name: {model.Name}<br />");
            supportEmailMessage.AppendLine($"Email: {model.Email}</p>");
            supportEmailMessage.AppendLine($"<p>Subject: <br />{model.Subject}</p>");
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
