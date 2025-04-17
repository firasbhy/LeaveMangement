
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

public class LeaveRequest
{
    public int Id {get; set;}
    [Required]
    public int EmployeeId {get; set;}
    [ForeignKey("EmployeeId")]
    public Employee Employee {get; set;}
    [Required]
    public LeaveType LeaveType{get; set;}
    [Required]
    public DateTime  StartDate{get; set;}
    [Required]
    public DateTime EndDate {get; set;}
    [Required]
    public LeaveStatus status {get; set;}
    [Required]
    public string Reason {get; set;}
    public DateTime CreatedAt{get; set;} = DateTime.UtcNow;


}