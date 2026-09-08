namespace ReportWorker.Models;

public class DailyReportRequest
{
    public int Id { get; set; }
    public DateTime RequestedAt { get; set; } = DateTime.UtcNow;
    public string RequestType { get; set; } = "ReservationSummary";
    public bool IsProcessed { get; set; }
    public DateTime? ProcessedAt { get; set; }
    public string? ResultSummary { get; set; }
}
