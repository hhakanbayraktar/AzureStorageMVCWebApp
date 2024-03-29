﻿namespace AzureStorageLibrary.Model
{
  public class PictureWatermarkQueue
  {
    public string UserId { get; set; }
    public string City { get; set; }
    public List<string> Pictures { get; set; }
    public string ConnectionId { get; set; }
    public string WatermarkText { get; set; }
  }
}
