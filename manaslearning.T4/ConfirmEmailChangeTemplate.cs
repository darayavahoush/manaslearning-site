using manaslearning.T4.Templates.Models;

namespace manaslearning.T4.Templates
{
    public partial class ConfirmEmailChangeTemplate
    {

        private ConfirmEmailChangeModel _confirmEmailChangeModel { get; set; }

        public ConfirmEmailChangeTemplate(ConfirmEmailChangeModel confirmEmailChangeModel)
        {
            _confirmEmailChangeModel = new ConfirmEmailChangeModel();
            _confirmEmailChangeModel.Company = confirmEmailChangeModel.Company;
            _confirmEmailChangeModel.Name = confirmEmailChangeModel.Name;
            _confirmEmailChangeModel.OldEmail = confirmEmailChangeModel.OldEmail;
            _confirmEmailChangeModel.NewEmail = confirmEmailChangeModel.NewEmail;
            _confirmEmailChangeModel.ConfirmUrl = confirmEmailChangeModel.ConfirmUrl;
        }
    }
}
