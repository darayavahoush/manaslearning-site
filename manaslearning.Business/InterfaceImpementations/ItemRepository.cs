using Dapper;
using manaslearning.Business.Interfaces;
using manaslearning.Models;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace manaslearning.Business.InterfaceImpementations
{
    public class ItemRepository : IItemRepository
    {
        private readonly IApplicationConnection _applicationConnection;
        private readonly ILogger _logger;

        public ItemRepository(IApplicationConnection applicationConnection, ILogger<ItemRepository> logger)
        {
            _applicationConnection = applicationConnection;
            _logger = logger;
        }
        
        /// <summary>
        /// Get items
        /// </summary>       
        /// <returns>IEnumerable</returns>
        public async Task<IEnumerable<Item>> GetItemsAsync()
        {
            string procedureName = "[dbo].[GetItems]";
            try
            {
                using (var connection = _applicationConnection.GetConnection())
                {
                    
                    return await connection.QueryAsync<Item>(procedureName, commandType: CommandType.StoredProcedure);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"ErrorMessage={ex.Message}, Source=ItemRepository.GetItemsAsync()");
                return null;
            }
        }

        /// <summary>
        /// Get all items
        /// </summary>       
        /// <returns>IEnumerable</returns>
        public async Task<IEnumerable<Item>> GetAllItemsAsync()
        {
            string procedureName = "[dbo].[GetAllItems]";
            try
            {
                using (var connection = _applicationConnection.GetConnection())
                {

                    return await connection.QueryAsync<Item>(procedureName, commandType: CommandType.StoredProcedure);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"ErrorMessage={ex.Message}, Source=ItemRepository.GetAllItemsAsync()");
                return null;
            }
        }

        /// <summary>
        /// Get item
        /// </summary>       
        /// <param name="itemId"></param>
        /// <returns>Item</returns>
        public async Task<Item> GetItemAsync(int itemId)
        {
            string procedureName = "[dbo].[GetItem]";
            try
            {
                using (var connection = _applicationConnection.GetConnection())
                {
                    var param = new
                    {                        
                        ItemId = itemId
                    };
                    return await connection.QueryFirstOrDefaultAsync<Item>(procedureName, param: param, commandType: CommandType.StoredProcedure);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"ErrorMessage={ex.Message}, Source=ItemRepository.GetItemAsync(), ItemId={itemId}");
                return null;
            }
        }

        /// <summary>
        /// Get item
        /// </summary>       
        /// <param name="oid"></param>
        /// <returns>Item</returns>
        public async Task<Item> GetItemByOidAsync(Guid oid)
        {
            string procedureName = "[dbo].[GetItemByOid]";
            try
            {
                using (var connection = _applicationConnection.GetConnection())
                {
                    var param = new
                    {                        
                        Oid = oid
                    };
                    return await connection.QueryFirstOrDefaultAsync<Item>(procedureName, param: param, commandType: CommandType.StoredProcedure);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"ErrorMessage={ex.Message}, Source=ItemRepository.GetItemByOidAsync(), Oid={oid}");
                return null;
            }
        }

        /// <summary>
        /// Get items info
        /// </summary>       
        /// <returns>IEnumerable</returns>
        public async Task<IEnumerable<ItemInfo>> GetItemsInfoAsync()
        {
            string procedureName = "[dbo].[GetItemsInfo]";
            try
            {
                using (var connection = _applicationConnection.GetConnection())
                {

                    return await connection.QueryAsync<ItemInfo>(procedureName, commandType: CommandType.StoredProcedure);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"ErrorMessage={ex.Message}, Source=ItemRepository.GetItemsInfoAsync()");
                return null;
            }
        }

        /// <summary>
        /// Get iteminfo
        /// </summary>       
        /// <param name="oid"></param>
        /// <returns>Item</returns>
        public async Task<ItemInfo> GetItemInfoByOidAsync(Guid oid)
        {
            string procedureName = "[dbo].[GetItemInfoByOid]";
            try
            {
                using (var connection = _applicationConnection.GetConnection())
                {
                    var param = new
                    {
                        Oid = oid
                    };
                    return await connection.QueryFirstOrDefaultAsync<ItemInfo>(procedureName, param: param, commandType: CommandType.StoredProcedure);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"ErrorMessage={ex.Message}, Source=ItemRepository.GetItemInfoByOidAsync(), Oid={oid}");
                return null;
            }
        }

        /// <summary>
        /// Get items info by AssessmentId
        /// </summary>       
        /// <returns>IEnumerable</returns>
        public async Task<IEnumerable<ItemInfo>> GetItemsInfoByAssessmentIdAsync(int assessmentId)
        {
            string procedureName = "[dbo].[GetItemsInfoByAssessmentId]";
            try
            {
                using (var connection = _applicationConnection.GetConnection())
                {
                    var param = new
                    {
                        AssessmentId = assessmentId
                    };
                    return await connection.QueryAsync<ItemInfo>(procedureName, param: param, commandType: CommandType.StoredProcedure);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"ErrorMessage={ex.Message}, Source=ItemRepository.GetItemsInfoByAssessmentIdAsync()");
                return null;
            }
        }
    }
}
