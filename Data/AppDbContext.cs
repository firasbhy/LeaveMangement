
using Microsoft.EntityFrameworkCore;

namespace LeaveManagement.Data
{
    public class AppDBContext : DbContext
           {
            public AppDBContext(DbContextOptions<AppDBContext> options) : base(options) {}
            public DbSet<Employee> Employees {get; set;}
            public DbSet<LeaveRequest> LeaveRequests {get; set;}


           }
           
}