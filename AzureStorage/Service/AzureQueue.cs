using Azure.Storage.Queues;
using Azure.Storage.Queues.Models;

namespace AzureStorageLibrary.Service
{
  public class AzureQueue : IAzureQueue
  {
    private readonly QueueClient _queueClient;
    public AzureQueue(string queueName)
    {
      _queueClient = new QueueClient(ConnectionString.AzureStorageConnectionString, queueName);
      _queueClient.CreateIfNotExists();
    }

    public async Task SendMessageAsync(string message)
    {
      await _queueClient.SendMessageAsync(message);
    }
    public async Task<QueueMessage> RetrieveNextMessageAsync()
    {
      QueueProperties properties = await _queueClient.GetPropertiesAsync();

      if (properties.ApproximateMessagesCount > 0)
      {
        QueueMessage[] queueMessages = await _queueClient.ReceiveMessagesAsync(1, TimeSpan.FromMinutes(1));

        if (queueMessages.Any())
        {
          return queueMessages[0];
        }
      }
      return null;
    }
    public async Task DeleteMessageAsync(string messageId, string popReceipt)
    {
      await _queueClient.DeleteMessageAsync(messageId, popReceipt);
    }
  }
}
