using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace manaslearning.Shared
{
    public interface IStorageHelper
    {
        Task<bool> InsertMessageIntoQueueAsync(string storageAccountName, string queueName, string message, ILogger logger);
    }
}
