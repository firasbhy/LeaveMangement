using LeaveManagement.Data;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LeaveManagement.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LeaveRequestController : ControllerBase
    {
        private readonly AppDBContext _context;
        public LeaveRequestController(AppDBContext context)
        {
            _context = context;
        }
        [HttpGet]
        public async Task<ActionResult<IEnumerable<LeaveRequest>>> GetLeaveRequests()
        {
            return await _context.LeaveRequests.ToListAsync();
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
        public async Task<ActionResult<LeaveRequest>> CreateLeaveRequest (LeaveRequest leaveRequest)
        {
            leaveRequest.CreatedAt = DateTime.Now;
            _context.LeaveRequests.Add(leaveRequest);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetLeaveRequest), new {id =leaveRequest.Id}, leaveRequest);
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
    }
}