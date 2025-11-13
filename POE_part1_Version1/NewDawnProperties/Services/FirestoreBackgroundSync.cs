using Microsoft.Extensions.Hosting;
using NewDawnProperties.Services;

public class FirestoreBackgroundSync : BackgroundService
{
    private readonly IServiceProvider _services;

    public FirestoreBackgroundSync(IServiceProvider services)
    {
        _services = services;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        using var scope = _services.CreateScope();
        var firestoreService = scope.ServiceProvider.GetRequiredService<FirestoreSyncService>();

        // Run full sync once at startup
        await firestoreService.FullSyncAsync();

        while (!stoppingToken.IsCancellationRequested)
        {
            await firestoreService.IncrementalSyncAsync();
            await Task.Delay(TimeSpan.FromMinutes(5), stoppingToken);
        }
    }
}
