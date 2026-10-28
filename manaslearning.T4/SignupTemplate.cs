using manaslearning.T4.Templates.Models;

namespace manaslearning.T4.Templates
{
    public partial class SignupTemplate
    {
        private SignupModel _signupModel { get; set; }
        public SignupTemplate(SignupModel signupModel)
        {
            _signupModel = new SignupModel();
            _signupModel.Company = signupModel.Company;
            _signupModel.Name = signupModel.Name;
            _signupModel.ActivationUrl = signupModel.ActivationUrl;
        }
    }
}
