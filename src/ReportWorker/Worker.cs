using Microsoft.EntityFrameworkCore;
using ReportWorker.Data;
using ReportWorker.Models;

namespace ReportWorker;

public class ReportWorkerService(ILogger<ReportWorkerService> logger, IServiceProvider serviceProvider) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                using var scope = serviceProvider.CreateScope();
                var db = scope.ServiceProvider.GetRequiredService<ReportDbContext>();

                db.Database.EnsureCreated();

                var pendingRequests = await db.DailyReportRequests
                    .Where(x => !x.IsProcessed)
                    .ToListAsync(stoppingToken);

                foreach (var request in pendingRequests)
                {
                    var entry = new DailyReportEntry
                    {
                        ReportDate = DateTime.UtcNow.Date,
                        TotalRequests = 1,
                        TotalBookings = 0,
                        TotalRevenue = 0m,
                        CreatedAt = DateTime.UtcNow
                    };

                    request.IsProcessed = true;
                    request.ProcessedAt = DateTime.UtcNow;
                    request.ResultSummary = $"Procesado en {request.ProcessedAt:O}";

                    db.DailyReportEntries.Add(entry);
                    logger.LogInformation("Solicitud {RequestId} procesada por el worker.", request.Id);
                }

                await db.SaveChangesAsync(stoppingToken);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error al procesar solicitudes del worker de reportes.");
            }

            await Task.Delay(TimeSpan.FromSeconds(30), stoppingToken);
        }
    }
}
