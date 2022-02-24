using AzureStorageLibrary;
using AzureStorageLibrary.Model;
using AzureStorageLibrary.Service;
using AzureStorageMVCWebApp.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Text;

namespace AzureStorageMVCWebApp.Controllers
{
  public class PicturesController : Controller
  {
    public string UserId { get; set; } = "12345";
    public string City { get; set; } = "London";

    private readonly INoSqlStorage<UserPicture> _noSqlStorage;
    private readonly IBlobStorage _blobStorage;
    public PicturesController(INoSqlStorage<UserPicture> noSqlStorage, IBlobStorage blobStorage)
    {
      _noSqlStorage = noSqlStorage;
      _blobStorage = blobStorage;
    }

    public async Task<IActionResult> Index()
    {
      ViewBag.UserId = UserId;
      ViewBag.City = City;
      List<FileBlob> fileBlob = new List<FileBlob>();
      var user = await _noSqlStorage.Get(UserId, City);

      if (user != null)
      {
        user.Paths.ForEach(x =>
        {
          fileBlob.Add(new FileBlob { Name = x, Url = $"{_blobStorage.BlobUrl}/{EContainerName.pictures}/{x}" });
        });
      }
      ViewBag.fileBlobs = fileBlob;
      return View();
    }

    [HttpPost]
    public async Task<IActionResult> Index(IEnumerable<IFormFile> pictures)
    {
      List<string> pictureList = new List<string>();

      foreach (var picture in pictures)
      {
        var newPictureName = $"{Guid.NewGuid().ToString()}{Path.GetExtension(picture.FileName)}";
        await _blobStorage.UploadAsync(picture.OpenReadStream(), newPictureName, EContainerName.pictures);

        pictureList.Add(newPictureName);
      }

      var isUser = await _noSqlStorage.Get(UserId, City);
      if (isUser != null)
      {
        pictureList.AddRange(isUser.Paths);
        isUser.Paths = pictureList;
      }
      else
      {
        isUser = new UserPicture()
        {
          RowKey = UserId,
          PartitionKey = City,
          Paths = pictureList
        };
      }

      await _noSqlStorage.Add(isUser);

      return RedirectToAction("Index");
    }

    public async Task<IActionResult> ShowWatermark()
    {
      UserPicture userPicture = await _noSqlStorage.Get(UserId, City);

      List<FileBlob> fileBlob = new List<FileBlob>();
      userPicture.WatermarkPaths.ForEach(x =>
      {
        fileBlob.Add(new FileBlob { Name = x, Url = $"{_blobStorage.BlobUrl}/{EContainerName.watermarkpicture}/{x}" });
      });

      ViewBag.fileBlobs = fileBlob;
      return View();
    }

    [HttpPost]
    public async Task<IActionResult> AddWatermark(PictureWatermarkQueue pictureWatermarkQueue)
    {
      var jsonString = JsonConvert.SerializeObject(pictureWatermarkQueue);
      string jsonStringBase64 = Convert.ToBase64String(Encoding.UTF8.GetBytes(jsonString));
      AzureQueue azureQueue = new AzureQueue("watermarkqueue");
      await azureQueue.SendMessageAsync(jsonStringBase64);
      return Ok();
    }
  }
}
