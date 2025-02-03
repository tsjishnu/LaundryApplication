using LaundryApplication.Data;
using LaundryApplication.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LaundryApplication.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CustomerController : ControllerBase
    {
        private readonly LaundryDbContext _context;

        public CustomerController(LaundryDbContext context)
        {
            _context = context;
        }

        [HttpGet("Services")]
        public async Task<ActionResult<IEnumerable<Service>>> GetAllServices()
        {
            var services = await _context.Services.ToListAsync();

            if (services == null || services.Count == 0)
            {
                return NotFound("No services available.");
            }

            return Ok(services);
        }

        [HttpGet("Services/{id}")]
        public async Task<ActionResult<Service>> GetServiceById(Guid id)
        {
            var service = await _context.Services.FindAsync(id);

            if (service == null)
            {
                return NotFound($"Service with ID {id} not found.");
            }

            return Ok(service);
        }
        // POST: api/Customer/PlaceOrder
        [HttpPost("PlaceOrder")]
        public async Task<ActionResult<Order>> PlaceOrder([FromBody] Order orderRequest)
        {
            if (orderRequest == null || orderRequest.ServiceId == Guid.Empty || orderRequest.Quantity <= 0 || orderRequest.ExpectedDeliveryDate == DateTime.MinValue)
            {
                return BadRequest("Service ID, quantity, expected delivery date, and additional description are required.");
            }

            // Ensure that the service ID is valid
            var service = await _context.Services.FindAsync(orderRequest.ServiceId);
            if (service == null)
            {
                return NotFound($"Service with ID {orderRequest.ServiceId} not found.");
            }

            // Create and save the order
            orderRequest.Id = Guid.NewGuid();  // Assign a new unique ID
            orderRequest.DateCreated = DateTime.UtcNow;  // Set the creation timestamp

            _context.Orders.Add(orderRequest);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(PlaceOrder), new { id = orderRequest.Id }, orderRequest);
        }
        // GET: api/Customer/Orders/{id}/Status
        [HttpGet("Orders/{id}/Status")]
        public async Task<ActionResult> GetOrderStatus(Guid id)
        {
            // Retrieve the order with the given ID
            var order = await _context.Orders
                .Include(o => o.Service)  // Optional: Include the service details if needed
                .FirstOrDefaultAsync(o => o.Id == id);

            if (order == null)
            {
                return NotFound($"Order with ID {id} not found.");
            }
            return Ok(new { OrderId = order.Id, Status = order.Status });
        }
    }
}
