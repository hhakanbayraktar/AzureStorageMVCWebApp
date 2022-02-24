using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using AzureStorageLibrary;
using AzureStorageLibrary.Model;
using AzureStorageLibrary.Service;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;

namespace WatermarkProcessFuction
{
  public class Function1
  {
    [FunctionName("Function1")]
    public async Task Run([QueueTrigger("watermarkqueue")] PictureWatermarkQueue myQueueItem, ILogger log)
    {
      ConnectionString.AzureStorageConnectionString = "AccountName=devstoreaccount1;AccountKey=Eby8vdM02xNOcqFlqUwJPLlmEtlCDXJ1OUzFT50uSRZ6IFsuFq2UVErCz4I6tq/K1SZFPTOtr/KBHBeksoGMGw==;DefaultEndpointsProtocol=http;BlobEndpoint=http://127.0.0.1:10000/devstoreaccount1;QueueEndpoint=http://127.0.0.1:10001/devstoreaccount1;TableEndpoint=http://127.0.0.1:10002/devstoreaccount1;";
      IBlobStorage blobStorage = new BlobStorage();

      INoSqlStorage<UserPicture> noSqlStorage = new TableStorage<UserPicture>();
      List<string> PicturesNewName = new List<string>();

      foreach (var item in myQueueItem.Pictures)
      {
        using var stream = await blobStorage.DownloadAsync(item, EContainerName.pictures);
        using var memoryStream = AddWatermark(myQueueItem.WatermarkText, stream);

        var newWatermarkFileName = Guid.NewGuid().ToString() + Path.GetExtension(item);
        await blobStorage.UploadAsync(memoryStream, newWatermarkFileName, EContainerName.watermarkpicture);
        PicturesNewName.Add(newWatermarkFileName);
        log.LogInformation($"Watermark added to {item}");
      }

      var userPicture = await noSqlStorage.Get(myQueueItem.UserId, myQueueItem.City);

      if (userPicture.WatermarkRawPath != null)
      {
        PicturesNewName.AddRange(userPicture.WatermarkPaths);
      }

      userPicture.WatermarkPaths = PicturesNewName;
      await noSqlStorage.Add(userPicture);

      HttpClient httpClient = new HttpClient();
      var response = await httpClient.GetAsync($"https://localhost:7168/Notification/CompleteWatermarkProcess/{myQueueItem.ConnectionId}");

      log.LogInformation($"Information sent Client({myQueueItem.ConnectionId})");
    }

    public static MemoryStream AddWatermark(string watermarkText, Stream pictureStream)
    {
      MemoryStream ms = new MemoryStream();

      using (Image image = Bitmap.FromStream(pictureStream))
      {
        using (Bitmap tempBitmap = new Bitmap(image.Width, image.Height))
        {
          using (Graphics graphics = Graphics.FromImage(tempBitmap))
          {
            graphics.DrawImage(image, 0, 0);

            var font = new Font(FontFamily.GenericSansSerif, 25, FontStyle.Bold);
            var color = Color.FromArgb(255, 0, 0);
            var brush = new SolidBrush(color);
            var point = new Point(20, image.Height - 50);
            graphics.DrawString(watermarkText, font, brush, point);
            tempBitmap.Save(ms, ImageFormat.Png);
          }
        }
      }

      ms.Position = 0;
      return ms;
    }
  }
}
