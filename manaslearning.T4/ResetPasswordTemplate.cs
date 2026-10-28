using manaslearning.T4.Templates.Models;

namespace manaslearning.T4.Templates
{
    public partial class ResetPasswordTemplate
    {
        private ResetPasswordModel _resetPasswordModel { get; set; }
        public ResetPasswordTemplate(ResetPasswordModel resetPasswordModel)
        {
            _resetPasswordModel = new ResetPasswordModel();
            _resetPasswordModel.Company = resetPasswordModel.Company;
            _resetPasswordModel.Name = resetPasswordModel.Name;
            _resetPasswordModel.ResetPasswordUrl = resetPasswordModel.ResetPasswordUrl;
        }
    }
}
