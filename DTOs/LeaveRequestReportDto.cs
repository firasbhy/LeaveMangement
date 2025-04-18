public class LeaveRequestReportDto
{
    public int Year { get; set; }
    public int TotalRequests { get; set; }
    public int Approved { get; set; }
    public int Rejected { get; set; }
    public int Pending { get; set; }
    public Dictionary<string, int> ByType { get; set; }
}