using Microsoft.Extensions.Configuration;

namespace manaslearning.UI.Models
{
    /// <summary>
    /// Non-secret consultation booking settings (cal.com links and the paid consultation fee).
    /// Read from the "Consultation" configuration section.
    /// </summary>
    public class ConsultationSettings
    {
        public string CalOrigin { get; set; } = "https://app.cal.com";
        public string FreeCalLink { get; set; }
        public string IepCalLink { get; set; }
        public string PaidCalLink { get; set; }
        public string PaidAmount { get; set; } = "999.00";

        public static ConsultationSettings From(IConfiguration configuration)
        {
            var settings = new ConsultationSettings();
            configuration.GetSection("Consultation").Bind(settings);
            return settings;
        }
    }
}
