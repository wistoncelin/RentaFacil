namespace ReportWorker.Models;

public class DailyReportEntry
{
    public int Id { get; set; }
    public DateTime ReportDate { get; set; }
    public int TotalRequests { get; set; }
    public int TotalBookings { get; set; }
    public decimal TotalRevenue { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
