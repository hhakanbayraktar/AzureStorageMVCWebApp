using AzureStorageLibrary.Service;
using System.Text;

Console.WriteLine("Hello, World!");
AzureStorageLibrary.ConnectionString.AzureStorageConnectionString = "AccountName=devstoreaccount1;AccountKey=Eby8vdM02xNOcqFlqUwJPLlmEtlCDXJ1OUzFT50uSRZ6IFsuFq2UVErCz4I6tq/K1SZFPTOtr/KBHBeksoGMGw==;DefaultEndpointsProtocol=http;BlobEndpoint=http://127.0.0.1:10000/devstoreaccount1;QueueEndpoint=http://127.0.0.1:10001/devstoreaccount1;TableEndpoint=http://127.0.0.1:10002/devstoreaccount1;";
AzureQueue queue = new AzureQueue("examplequeue");

////Message Insert
//string base64 = Convert.ToBase64String(Encoding.UTF8.GetBytes("Azure Storage Queue Example")); //For Turkish Char
//await queue.SendMessageAsync(base64);
//Console.WriteLine("Message Sent.");

//Message Read
var message = await queue.RetrieveNextMessageAsync();
string messageTxt = Encoding.UTF8.GetString(Convert.FromBase64String(message.MessageText));
Console.WriteLine("Message:" + messageTxt);

////Message Delete
//var message = queue.RetrieveNextMessageAsync().Result;
//await queue.DeleteMessageAsync(message.MessageId, message.PopReceipt);
//Console.WriteLine("Message Deleted");