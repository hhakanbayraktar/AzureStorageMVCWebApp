using Microsoft.Azure.Cosmos.Table;
using Newtonsoft.Json;

namespace AzureStorageLibrary.Model
{
  public class UserPicture : TableEntity
  {
    public string RawPaths { get; set; }
    public string WatermarkRawPath { get; set; }

    [IgnoreProperty]
    public List<string> Paths
    {
      get => RawPaths == null ? null : JsonConvert.DeserializeObject<List<string>>(RawPaths);
      set => RawPaths = JsonConvert.SerializeObject(value);
    }

    [IgnoreProperty]
    public List<string> WatermarkPaths
    {
      get => WatermarkRawPath == null ? null : JsonConvert.DeserializeObject<List<string>>(WatermarkRawPath);
      set => WatermarkRawPath = JsonConvert.SerializeObject(value);
    }
  }
}
