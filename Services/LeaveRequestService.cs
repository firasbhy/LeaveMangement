using LeaveManagement.Data;
using Microsoft.EntityFrameworkCore;

namespace LeaveManagement.Services
{
    public class LeaveRequestService : ILeaveRequestService
    {
        private readonly AppDBContext _context;

        public LeaveRequestService(AppDBContext context)
        {
            _context = context;
        }

        public bool HasOverlappingLeave(int employeeId, DateTime startDate, DateTime endDate, int? leaveRequestId = null)
        {
            return _context.LeaveRequests.Any(lr =>
                lr.EmployeeId == employeeId &&
                (leaveRequestId == null || lr.Id != leaveRequestId) &&
                lr.StartDate <= endDate &&
                lr.EndDate >= startDate
            );
        }

        public int GetUsedAnnualDays(int employeeId, int year)
        {
            return _context.LeaveRequests
                .Where(lr => lr.EmployeeId == employeeId &&
                             lr.LeaveType == LeaveType.Annual &&
                             lr.StartDate.Year == year)
                .Sum(lr => (lr.EndDate - lr.StartDate).Days + 1);

        }
    }
}