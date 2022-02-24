using AzureStorageMVCWebApp.Hubs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;

namespace AzureStorageMVCWebApp.Controllers
{
  [Route("[controller]/[action]")]
  [ApiController]
  public class NotificationController : ControllerBase
  {
    private readonly IHubContext<NotificationHub> _hubContext;
    public NotificationController(IHubContext<NotificationHub> hubContext)
    {
      _hubContext = hubContext;
    }

    [HttpGet("{connectionId}")]
    public IActionResult CompleteWatermarkProcess(string connectionId)
    {
      _hubContext.Clients.Client(connectionId).SendAsync("NotifyCompleteWatermarkProcess");
      return Ok();
    }
  }
}
