namespace manaslearning.Models
{
    public class Enums
    {
        public enum LoginTypes { None = 0, OrganiztionAdmin, OrganiztionUser, Individual }
        
        public enum SelfTestType
        {
            Learning = 1,
            /// <summary>
            /// Hyperactive and Attention Test (Age below 6).
            /// </summary>
            HyperactiveAndAttention1, // age below 6
            /// <summary>
            /// Hyperactive and Attention Test (Age above 6).
            /// </summary>
            HyperactiveAndAttention2, // age above 6
            MotorSkills,
            SensitivityAndCommunication,
            Anxiety,
            AvoidingSchool,
            Sadness
        }

        public enum SitemapFrequency { Never, Yearly, Monthly, Weekly, Daily, Hourly, Always }
    }

    public enum AssessmentStatus : int
    {
        InProgress = 1,
        Completed
    }

    public enum AssessmentResultStatus : int
    {
        NotYetGenerated = 1,
        Generated
    }

    public enum SortDirection : short
    {
        /// <summary>
        /// Specifies Ascending Sort
        /// </summary>
        ASC = 1,
        /// <summary>
        /// Specifies Descendng Sort
        /// </summary>
        DESC
    }

    /// <summary>
    /// Enum represents all the organization types
    /// </summary>
    public enum OrganizationType
    {
        School = 1,
        Hospital,
        Individual
    }

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
        Dashboard_View,
        Organization_Read,
        Organization_Create,
        Organization_Update,
        Organization_Delete,
        Organization_ReadAll,
        OrganizationUser_Read,
        OrganizationUser_Create,
        OrganizationUser_Update,
        OrganizationUser_Delete,
        OrganizationUser_ReadAll,
        OrganizationUser_ResetPassword,
        OrganizationAddress_Read,
        OrganizationAddress_Create,
        OrganizationAddress_Update,
        OrganizationAddress_Delete,
        OrganizationAddress_ReadAll,
        OrganizationLicense_Read,
        OrganizationLicense_Add,
        OrganizationLicense_Reduce,
        OrganizationStudent_Read,
        OrganizationStudent_Create,
        OrganizationStudent_Update,
        OrganizationStudent_Delete,
        OrganizationStudent_ReadAll,
        Organization_SetDefaultAdmin,
        OrganizationStudentClass_ReadAll,
        OrganizationStudentClass_Read,
        OrganizationStudentClass_Create,
        OrganizationStudentClass_Update,
        OrganizationStudentClass_Delete,
        OrganizationTeacherClass_ReadAll,
        OrganizationTeacherClass_Read,
        OrganizationTeacherClass_Create,
        OrganizationTeacherClass_Update,
        OrganizationTeacherClass_Delete
    }
}
