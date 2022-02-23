using Azure.Storage.Queues.Models;

namespace AzureStorageLibrary
{
  public interface IAzureQueue
  {
    Task SendMessageAsync(string message);
    Task<QueueMessage> RetrieveNextMessageAsync();
    Task DeleteMessageAsync(string messageId, string popReceipt);
  }
}
