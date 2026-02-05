using Microsoft.AspNetCore.SignalR;

namespace DAO.Manager.Hubs;

public class ScanProgressHub : Hub
{
    public async Task JoinScanGroup(string scanId)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, $"scan-{scanId}");
    }

    public async Task LeaveScanGroup(string scanId)
    {
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"scan-{scanId}");
    }
}
