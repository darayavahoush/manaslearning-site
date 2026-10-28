using Dapper;
using manaslearning.Business.InterfaceImpementations;
using manaslearning.Business.Interfaces;
using manaslearning.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Data;
using System.Linq;
using System.Net;

namespace manaslearning.Shared
{
    public class UserAuthorizationActionFilter : IActionFilter
    {
        private readonly ILogger<ApplicationConnection> _logger;
        private readonly PrivilegeTypes[] _privileges;

        public UserAuthorizationActionFilter(PrivilegeTypes[] privileges)
        {
            this._privileges = privileges;
        }

        public void OnActionExecuting(ActionExecutingContext context)
        {
            if (!IsAuthorized(context))
            {
                context.Result = new StatusCodeResult((int)HttpStatusCode.Forbidden); // user unauthorized
                return;
            }
        }

        public void OnActionExecuted(ActionExecutedContext context)
        {
            // do nothing
        }

        private bool IsAuthorized(ActionExecutingContext context)
        {
            var email = context.HttpContext.User.GetEmail();

            if (string.IsNullOrWhiteSpace(email))
                return false;

            string procedureName = "[Admin].[GetUserPrivilegesByEmail]";

            try
            {
                var configuration = context.HttpContext.RequestServices.GetService(typeof(IConfiguration)) as IConfiguration;
                using (var connection = new ApplicationConnection(configuration, _logger).GetConnection())
                {
                    var privileges = connection.Query<Privilege>(
                        sql: procedureName,
                        param: new { Email = email },
                        commandType: CommandType.StoredProcedure);

                    if (privileges == null || !privileges.Any())
                        return false;

                    foreach (var privilege in _privileges)
                        if (privileges.FirstOrDefault(p => p.PrivilegeName == privilege.ToString()) != null)
                            return true;

                    return false;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"ErrorMessage={ex.Message}, Source=UserAuthorizationActionFilter.IsAuthorized, email={email}.");
                return false;
            }
        }
    }
}
