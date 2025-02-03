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

        // GET: api/Admin/Orders
        [HttpGet("Orders")]
        public async Task<ActionResult<IEnumerable<Order>>> GetAllOrders()
        {
            // Retrieve all orders with their related service details
            var orders = await _context.Orders
                .Include(o => o.Service)  // Include service details in the order
                .ToListAsync();

            if (orders == null || orders.Count == 0)
            {
                return NotFound("No orders found.");
            }

            return Ok(orders);
        }
        // PUT: api/Admin/Orders/{id}/Status
        [HttpPut("Orders/{id}/Status")]
        public async Task<ActionResult> UpdateOrderStatus(Guid id, [FromBody] OrderStatus status)
        {
            var order = await _context.Orders.FindAsync(id);
            if (order == null)
            {
                return NotFound($"Order with ID {id} not found.");
            }

            // Update the order status
            order.Status = status;
            _context.Entry(order).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return NoContent();  // Status 204 - Successfully updated, no content to return
        }
    }
}
