namespace manaslearning.Admin.UI.Models;

public class OrganizationUserInfo
{
    public long UserId { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Email { get; set; }
    public string OrganizationName { get; set; }
    public LoginTypes LoginType { get; set; }
    public bool EmailVerified { get; set; }
    public bool IsSystemCreated { get; set; }
    public string LoginProvider { get; set; }
    public bool IsActive { get; set; }
}
