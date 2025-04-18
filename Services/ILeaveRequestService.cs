
namespace LeaveManagement.Services
{
    public interface ILeaveRequestService
    {
        bool HasOverlappingLeave(int employeeId, DateTime startDate, DateTime endDate, int? leaveRequestId = null);
        int GetUsedAnnualDays(int employeeId, int year);
    }
}