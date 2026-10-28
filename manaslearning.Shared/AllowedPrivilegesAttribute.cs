using manaslearning.Models;
using Microsoft.AspNetCore.Mvc;
using System;

namespace manaslearning.Shared
{
    /// <summary>
    /// An attribute, which allows the user based on the user pivileges.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method)]
    public class AllowedPrivilegesAttribute : TypeFilterAttribute
    {
        /// <summary>
        /// Example: [AllowedPrivileges(PrivilegeTypes.Users_Read, PrivilegeTypes.Users_Create)]
        /// </summary>
        public AllowedPrivilegesAttribute(params PrivilegeTypes[] privileges): base(typeof(UserAuthorizationActionFilter))
        {
            Arguments = new object[] { privileges };
        }
    }
}
