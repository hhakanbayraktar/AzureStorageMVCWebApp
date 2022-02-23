using AzureStorageLibrary;
using AzureStorageLibrary.Model;
using AzureStorageMVCWebApp.Models;
using Microsoft.AspNetCore.Mvc;

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
      List<FileBlob> fileBlob = new List<FileBlob>();
      var user = await _noSqlStorage.Get(UserId, City);

      ViewBag.blobUrl = $"{_blobStorage.BlobUrl}/{EContainerName.pictures}";

      if (user == null)
      {
        user.Paths.ForEach(x =>
        {
          fileBlob.Add(new FileBlob { Name=x, Url= $"{_blobStorage.BlobUrl}/{EContainerName.pictures}/{x}" });
        });
      }

      return View();
    }
  }
}
