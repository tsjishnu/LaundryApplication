using LaundryApplication.Data;
using LaundryApplication.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;

namespace LaundryApplication.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AdminController : ControllerBase
    {
        private readonly LaundryDbContext _context;

        public AdminController(LaundryDbContext context)
        {
            _context = context;
        }

        // POST: api/Admin/AddService
        [HttpPost("AddService")]
        public async Task<ActionResult<Service>> AddService([FromBody] Service newService)
        {
            if (newService == null || string.IsNullOrEmpty(newService.ServiceName) || string.IsNullOrEmpty(newService.MaterialType) || newService.Price <= 0)
            {
                return BadRequest("Service name, material type, and price are required.");
            }

            // Check if the service already exists for this material type
            var existingService = await _context.Services
                .FirstOrDefaultAsync(s => s.ServiceName == newService.ServiceName && s.MaterialType == newService.MaterialType);

            if (existingService != null)
            {
                return Conflict("A service for this material type already exists.");
            }

            // Add the new service
            newService.Id = Guid.NewGuid();  // Assign a new unique ID
            _context.Services.Add(newService);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(AddService), new { id = newService.Id }, newService);
        }

        // GET: api/Admin/Services
        [HttpGet("Services")]
        public async Task<ActionResult> GetAllServices()
        {
            var services = await _context.Services.ToListAsync();
            return Ok(services);
        }

        // GET: api/Admin/Services/{id}
        [HttpGet("Services/{id}")]
        public async Task<ActionResult<Service>> GetServiceById(Guid id)
        {
            var service = await _context.Services.FindAsync(id);

            if (service == null)
            {
                return NotFound("Service not found.");
            }

            return Ok(service);
        }
    }
}
