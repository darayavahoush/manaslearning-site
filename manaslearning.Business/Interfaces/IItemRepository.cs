using manaslearning.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace manaslearning.Business.Interfaces
{
    public interface IItemRepository
    {
        Task<IEnumerable<Item>> GetItemsAsync();

        Task<Item> GetItemAsync(int itemId);

        Task<Item> GetItemByOidAsync(Guid oid);

        Task<IEnumerable<ItemInfo>> GetItemsInfoAsync();

        Task<ItemInfo> GetItemInfoByOidAsync(Guid oid);

        Task<IEnumerable<ItemInfo>> GetItemsInfoByAssessmentIdAsync(int assessmentId);
    }
}
