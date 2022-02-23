using AzureStorageLibrary;
using AzureStorageLibrary.Model;
using Microsoft.AspNetCore.Mvc;

namespace AzureStorageMVCWebApp.Controllers
{
  public class TableStorageController : Controller
  {
    private readonly INoSqlStorage<Product> _noSqlStorage;
    public TableStorageController(INoSqlStorage<Product> noSqlStorage)
    {
      _noSqlStorage = noSqlStorage;
    }

    public IActionResult Index()
    {
      ViewBag.products = _noSqlStorage.All().ToList();
      ViewBag.IsUpdate = false;
      return View();
    }

    [HttpPost]
    public async Task<IActionResult> Create(Product product)
    {
      product.RowKey = Guid.NewGuid().ToString();
      product.PartitionKey = "Pens";

      await _noSqlStorage.Add(product);
      return RedirectToAction("Index");
    }

    public async Task<IActionResult> Update(string rowKey, string partitionKey)
    {
      var product = await _noSqlStorage.Get(rowKey, partitionKey);
      ViewBag.products = _noSqlStorage.All().ToList();
      ViewBag.IsUpdate = true;
      return View("Index", product);
    }

    [HttpPost]
    public async Task<IActionResult> Update(Product product)
    {
      product.ETag = "*";
      await _noSqlStorage.Update(product);
      return RedirectToAction("Index");
    }

    [HttpGet]
    public async Task<IActionResult> Delete(string rowKey, string partitionKey)
    {
      await _noSqlStorage.Delete(rowKey, partitionKey);
      return RedirectToAction("Index");
    }
    [HttpGet]
    public IActionResult Query(int price)
    {
      ViewBag.IsUpdate = false;
      ViewBag.products = _noSqlStorage.Query(x=> x.Price > price).ToList();
      return View("Index");
    }
  }
}
