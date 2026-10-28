using Dapper;
using manaslearning.Business.Interfaces;
using Microsoft.Extensions.Logging;
using System;
using System.Data;
using System.Threading.Tasks;

namespace manaslearning.Business.InterfaceImpementations
{
    public class ContactUsRepository : IContactUsRepository
    {
        private readonly IApplicationConnection _applicationConnection;
        private readonly ILogger _logger;

        public ContactUsRepository(IApplicationConnection applicationConnection, ILogger<ContactUsRepository> logger)
        {
            _applicationConnection = applicationConnection;
            _logger = logger;
        }

        public async Task<int> SaveContactUs(string name, string email, string subject, string message)
        {
            string procedureName = "[dbo].[SaveContactUs]";
            try
            {
                using (var connection = _applicationConnection.GetConnection())
                {
                    var param = new
                    {
                        Name = name,
                        Email = email,
                        Subject = subject,
                        Message = message
                    };
                    return await connection.QueryFirstOrDefaultAsync<int>(procedureName, param: param, commandType: CommandType.StoredProcedure);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error={ex.Message}, Source=ContactUsRepository.SaveContactUs, name={name}, email={email}, subject={subject}");
                return -1;
            }
        }
    }
}
