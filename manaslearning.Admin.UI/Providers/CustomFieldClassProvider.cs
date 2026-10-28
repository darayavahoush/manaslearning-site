using Microsoft.AspNetCore.Components.Forms;

namespace manaslearning.Admin.UI;

public class CustomFieldClassProvider : FieldCssClassProvider
{
    public override string GetFieldCssClass(EditContext editContext, in FieldIdentifier fieldIdentifier)
    {
        var isValid = !editContext.GetValidationMessages(fieldIdentifier).Any();

        if (editContext.IsModified(fieldIdentifier))
            return isValid ? "modified valid" : "modified invalid";
        else
            return isValid ? "valid" : "invalid";
    }
}
