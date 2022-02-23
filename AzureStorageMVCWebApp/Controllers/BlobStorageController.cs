using AzureStorageLibrary;
using AzureStorageMVCWebApp.Models;
using Microsoft.AspNetCore.Mvc;

namespace AzureStorageMVCWebApp.Controllers
{
  public class BlobStorageController : Controller
  {
    private readonly IBlobStorage _blobStorage;
    public BlobStorageController(IBlobStorage blobStorage)
    {
      _blobStorage = blobStorage;
    }
    public async Task<IActionResult> Index()
    {
      var names = _blobStorage.GetNames(EContainerName.pictures);
      string blobUrl = $"{_blobStorage.BlobUrl}/{EContainerName.pictures}";

      ViewBag.blobs = names.Select(x => new FileBlob { Name = x, Url = $"{blobUrl}/{x}" }).ToList();

      //ViewBag.logs = await _blobStorage.GetLogAsync("log.txt");
      return View();
    }

    [HttpPost]
    public async Task<IActionResult> Upload(IFormFile file)
    {
      //Append is not supported on local Storage Emulator.
      //await _blobStorage.SetLogAsync("Upload method entered", "log.txt");

      var newFileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
      await _blobStorage.UploadAsync(file.OpenReadStream(), newFileName, EContainerName.pictures);

      //await _blobStorage.SetLogAsync("Upload method exited", "log.txt");
      return RedirectToAction("Index");
    }
    [HttpGet]
    public async Task<IActionResult> Download(string fileName)
    {
      var stream = await _blobStorage.DownloadAsync(fileName, EContainerName.pictures);
      return File(stream, "application/octet-stream", fileName);
    }
    [HttpGet]
    public async Task<IActionResult> Delete(string fileName)
    {
      await _blobStorage.DeleteAsync(fileName, EContainerName.pictures);
      return RedirectToAction("Index");
    }
  }
}
