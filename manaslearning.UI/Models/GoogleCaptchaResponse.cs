using System;

namespace manaslearning.UI.Models
{
    /// <summary>
    /// <seealso cref="https://developers.google.com/recaptcha/docs/v3#site_verify_response"/>
    /// </summary>
    public class GoogleCaptchaResponse
    {
        public bool success { get; set; } // whether this request was a valid reCAPTCHA token for your site
        public double score { get; set; } // the score for this request (0.0 - 1.0)
        public string action { get; set; } // the action name for this request (important to verify)
        public DateTime challenge_ts { get; set; } // timestamp of the challenge load (ISO format yyyy-MM-dd'T'HH:mm:ssZZ)
        public string hostname { get; set; } // the hostname of the site where the reCAPTCHA was solved
    }
}
