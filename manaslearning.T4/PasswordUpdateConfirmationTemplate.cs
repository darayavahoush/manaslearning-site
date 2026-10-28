using manaslearning.T4.Templates.Models;

namespace manaslearning.T4.Templates
{
    public partial class PasswordUpdateConfirmationTemplate
    {
        private PasswordUpdateConfirmationModel _passwordUpdateConfirmationModel { get; set; }
        public PasswordUpdateConfirmationTemplate(PasswordUpdateConfirmationModel passwordUpdateConfirmationModel)
        {
            _passwordUpdateConfirmationModel = new PasswordUpdateConfirmationModel();
            _passwordUpdateConfirmationModel.Company = passwordUpdateConfirmationModel.Company;
            _passwordUpdateConfirmationModel.Name = passwordUpdateConfirmationModel.Name;
            _passwordUpdateConfirmationModel.UserName = passwordUpdateConfirmationModel.UserName;
        }
    }
}
