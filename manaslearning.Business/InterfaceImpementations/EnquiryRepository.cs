using Dapper;
using manaslearning.Business.Interfaces;
using manaslearning.Helpers;
using manaslearning.Models;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace manaslearning.Business.InterfaceImpementations
{
    public class EnquiryRepository : IEnquiryRepository
    {
        private readonly IApplicationConnection _applicationConnection;
        private readonly ILogger _logger;

        public EnquiryRepository(
            IApplicationConnection applicationConnection,
            ILogger<EnquiryRepository> logger)
        {
            _applicationConnection = applicationConnection;
            _logger = logger;
        }

        public async Task<int> CreateEnquiryAsync(
            string name,
            string email,
            string phone,
            string city,
            string state,
            string country,
            int childAge,
            string serviceRequest,
            string message)
        {
            string procedureName = "[dbo].[CreateEnquiry]";
            try
            {                
                using (var connection = _applicationConnection.GetConnection())
                {
                    var param = new
                    {
                        Name = name,
                        Email = email,
                        Phone = phone,
                        City = city,
                        State = state,
                        Country = country,
                        ChildAge = childAge,
                        ServiceRequest = serviceRequest,
                        Message = message
                    };

                    return await connection.QueryFirstOrDefaultAsync<int>(procedureName, param: param, commandType: CommandType.StoredProcedure);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error={ex.Message}, Source=EnquiryRepository.CreateEnquiry, name={name}, email={email}");
                return -1;
            }
        }

        /// <summary>
        /// Get all Enquiries
        /// </summary>       
        /// <returns>IEnumerable</returns>
        public async Task<IEnumerable<Enquiry>> GetEnquiriesAsync()
        {
            string procedureName = "[dbo].[GetEnquiries]";
            try
            {
                using (var connection = _applicationConnection.GetConnection())
                {

                    return await connection.QueryAsync<Enquiry>(procedureName, commandType: CommandType.StoredProcedure);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"ErrorMessage={ex.Message}, Source=EnquiryRepository.GetEnquiriesAsync()");
                return null;
            }
        }

        /// <summary>
        /// Get Enquiry
        /// </summary>       
        /// <param name="EnquiryId"></param>
        /// <returns>Enquiry</returns>
        public async Task<Enquiry> GetEnquiryAsync(long enquiryId)
        {
            string procedureName = "[dbo].[GetEnquiry]";
            try
            {
                using (var connection = _applicationConnection.GetConnection())
                {
                    var param = new
                    {
                        EnquiryId = enquiryId
                    };
                    return await connection.QueryFirstOrDefaultAsync<Enquiry>(procedureName, param: param, commandType: CommandType.StoredProcedure);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"ErrorMessage={ex.Message}, Source=EnquiryRepository.GetEnquiryAsync(), EnquiryId={enquiryId}");
                return null;
            }
        }
    }
}
