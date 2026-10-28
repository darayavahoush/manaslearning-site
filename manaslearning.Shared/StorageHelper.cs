using Azure.Identity;
using Azure.Storage.Queues;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace manaslearning.Shared
{
    public class StorageHelper : IStorageHelper
    {
        public async Task<bool> InsertMessageIntoQueueAsync(string storageAccountName, string queueName, string message, ILogger log)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(storageAccountName) || string.IsNullOrWhiteSpace(queueName))
                    return false;

                string connectionString = $"https://{storageAccountName}.queue.core.windows.net/{queueName}";

                // Instantiate a QueueClient
                QueueClient queueClient = new QueueClient(
                    queueUri: new Uri(connectionString),
                    credential: new DefaultAzureCredential());

                var isQueueExists = await queueClient.ExistsAsync();
                if (!isQueueExists)
                {
                    log?.LogError($"'{queueName}' is not available in the Storage Queue.");
                    return false;
                }

                var response = await queueClient.SendMessageAsync(message);

                return true;
            }
            catch (Exception ex)
            {
                log?.LogError(ex, $"ErrorMessage={ex.Message}, Source=StorageHelper.InsertMessageIntoQueueAsync(), QueueName={queueName}");
                return false;
            }
        }
    }
}
