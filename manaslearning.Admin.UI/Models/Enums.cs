namespace manaslearning.Admin.UI.Models
{
    public enum HttpContentType
    {
        Json = 1, // Default
        String,
        Empty
    }

    public enum LoginTypes { None = 0, Teacher, Parent }

    public enum ResetPasswordState { None = 0, Generated, Failed }

    /// <summary>
    /// Enum represents all the privileges
    /// </summary>
    public enum PrivilegeTypes
    {
        Users_Read = 1, // Note: Make sure this sequence is aligned with [Privilege] table
        Users_Create,
        Users_Update,
        Users_Delete,
        Users_Create_Bulk_Users,
        Users_Reset_Password,
        Blog_Read,
        Blog_Create,
        Blog_Update,
        Blog_Delete,
        Assessments_Read,
        Assessments_Generate_Report,
        Assessments_Download_Report,
        Dashboard_View
    }

    /// <summary>
    /// Enum represents organization types
    /// </summary>
    public enum OrganizationType
    {
        School = 1,
        Hospital,
        Individual
    }
}
