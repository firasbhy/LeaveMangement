using System.Xml.XPath;
using AutoMapper;
using LeaveManagement.Data;
using LeaveManagement.DTOs;
using LeaveManagement.Services;
using LinqKit;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LeaveManagement.Controllers
{
    [Microsoft.AspNetCore.Mvc.Route("api/[controller]")]
    [ApiController]
    public class LeaveRequestController : ControllerBase
    {
        private readonly AppDBContext _context;
        private readonly IMapper _mapper;
        private readonly ILeaveRequestService _leaveRequestService;
        public LeaveRequestController(AppDBContext context , IMapper mapper , ILeaveRequestService leaveRequestService)
        {
            _context = context;
            _mapper = mapper;
            _leaveRequestService = leaveRequestService;
        }
        [HttpGet]
        public async Task<ActionResult<IEnumerable<LeaveRequestDto>>> GetLeaveRequests()
        {
            var requests= await _context.LeaveRequests.ToListAsync();
            var dto = _mapper.Map<List<LeaveRequestDto>>(requests);
            return Ok(dto);
        }
        [HttpGet("{id}")]
        public async Task<ActionResult<LeaveRequest>> GetLeaveRequest(int id)
        {
            var LeaveRequest = await _context.LeaveRequests.FindAsync(id);
            if (LeaveRequest == null)
            {
                return NotFound();
            }
            return LeaveRequest;
        }
        [HttpPost]
        public async Task<ActionResult<LeaveRequest>> CreateLeaveRequest (CreateLeaveRequestDto dto)
        {
             // No overlapping leave dates per employee
        if (_leaveRequestService.HasOverlappingLeave(dto.EmployeeId, dto.StartDate, dto.EndDate))
        {
            return BadRequest("Overlapping leave detected.");
        }
        // Max 20 annual leave days per year
        var usedDays = _leaveRequestService.GetUsedAnnualDays(dto.EmployeeId, dto.StartDate.Year);
        var requestedDays = (dto.EndDate - dto.StartDate).Days + 1;
        if (dto.LeaveType == LeaveType.Annual && usedDays + requestedDays > 20)
        {
            return BadRequest("Annual leave limit exceeded (20 days max per year).");
        }
            // Sick leave requires a non-empty reason
        if (dto.LeaveType == LeaveType.Sick && string.IsNullOrWhiteSpace(dto.Reason))
        {
            return BadRequest("Sick leave requires a reason.");
        }

            var leaveRequest = _mapper.Map<LeaveRequest>(dto);
            
            leaveRequest.CreatedAt = DateTime.Now;
            _context.LeaveRequests.Add(leaveRequest);
            await _context.SaveChangesAsync();
            var result = _mapper.Map<LeaveRequestDto>(leaveRequest);
            return CreatedAtAction(nameof(GetLeaveRequest), new {id =leaveRequest.Id}, result);
        }
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateLeaveRequest( int id, LeaveRequest leaveRequest)
        {
            if(id != leaveRequest.Id)
            {
                return BadRequest();
            }
            _context.Entry(leaveRequest).State =EntityState.Modified;
            try{
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.LeaveRequests.Any(e => e.Id == id))
                {
                    return NotFound();
                }
                else {
                    throw;
                }
            }
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteLeaveRequest(int id)
        {
            var leaveRequest = await _context.LeaveRequests.FindAsync(id);
            if (leaveRequest == null)
            {
                return NotFound();
            }
            _context.LeaveRequests.Remove(leaveRequest);
            await _context.SaveChangesAsync();
            return NoContent();
        }
        [HttpGet("filter")]
        public async Task<ActionResult<IEnumerable<LeaveRequestDto>>> FilterLeavRequests( [FromQuery] int? employeeId, [FromQuery] LeaveType? leaveType, [FromQuery] LeaveStatus? status, [FromQuery] DateTime? startDate, [FromQuery] DateTime? endDate,[FromQuery] string? sortOrder, [FromQuery] string? keyword, [FromQuery] int page = 1,    [FromQuery] string? sortBy = "startDate"
    ){
            // Construction dynamique
    var predicate = PredicateBuilder.New<LeaveRequest>(true); // true = filtre initial (aucune condition)
          if (employeeId.HasValue)
        predicate = predicate.And(lr => lr.EmployeeId == employeeId);

    if (leaveType.HasValue)
        predicate = predicate.And(lr => lr.LeaveType == leaveType);

    if (status.HasValue)
        predicate = predicate.And(lr => lr.status == status);

    if (startDate.HasValue)
        predicate = predicate.And(lr => lr.StartDate >= startDate);

    if (endDate.HasValue)
        predicate = predicate.And(lr => lr.EndDate <= endDate);

    if (!string.IsNullOrEmpty(keyword))
        predicate = predicate.And(lr => lr.Reason.Contains(keyword));
               var query = _context.LeaveRequests
        .AsExpandable() // Obligatoire avec LinqKit pour que le filtre fonctionne
        .Where(predicate);
           query=sortBy?.ToLower() switch
           {
               "startdate" => sortOrder =="desc" ? query.OrderByDescending(r => r.StartDate) : query.OrderBy(r =>r.StartDate),
               "enddate" => sortOrder =="desc" ? query.OrderByDescending(r => r.EndDate) : query.OrderBy(r =>r.EndDate),
               "status" => sortOrder =="desc" ?  query.OrderByDescending(r =>r.status) : query.OrderBy(r => r.status ),
            "leavetype" => sortOrder =="desc" ?  query.OrderByDescending(r =>r.LeaveType) : query.OrderBy(r => r.LeaveType )


           };
           var totalCount= await query.CountAsync();
             
             
            var pagedResults= await query.Skip((page-1)*5).Take(5).ToListAsync();
            var dtos = _mapper.Map<List<LeaveRequestDto>>(pagedResults);
            return Ok(new{totalCount,sortBy,sortOrder,currentPage = page,results = dtos});
            
            

              
        }
    
    [HttpGet("report")]
public async Task<IActionResult> GetSummaryReport([FromQuery] int year)
{
    var requests = await _context.LeaveRequests
        .Where(r => r.StartDate.Year == year || r.EndDate.Year == year)
        .ToListAsync();

    var report = new
    {
        year = year,
        totalRequests = requests.Count,
        approved = requests.Count(r => r.status == LeaveStatus.Approved),
        rejected = requests.Count(r => r.status == LeaveStatus.Rejected),
        pending = requests.Count(r => r.status == LeaveStatus.Pending),
        byType = requests
            .GroupBy(r => r.LeaveType)
            .ToDictionary(
                g => g.Key.ToString(),
                g => g.Count()
            )
    };

    return Ok(report);
}
    
    }
    
}