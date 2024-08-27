using log4net;
using Microsoft.AspNetCore.Mvc;
using SampleAPI.Entities;
using SampleAPI.Repositories;
using SampleAPI.Requests;

namespace SampleAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class OrdersController : ControllerBase
    {
        private readonly IOrderRepository _orderRepository;
        private static readonly ILog log = LogManager.GetLogger(typeof(OrdersController));
        public OrdersController(IOrderRepository orderRepository)
        {
            _orderRepository = orderRepository;
        }

        [HttpGet("recentorders")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<List<Order>>> GetRecentOrders()
        {
            try
            {
                var orders = await _orderRepository.GetRecentOrdersAsync();
                return Ok(orders);
            }
            catch (Exception ex)
            {
                log.Info(ex);
                return StatusCode(StatusCodes.Status500InternalServerError, $"Internal server error: {ex.Message}");
            }
        }


        /// TODO: Add an endpoint to allow users to create an order using <see cref="CreateOrderRequest"/>.
        /// 
        [HttpPost("submitorder")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> CreateOrderRequest([FromBody] Order order)
        {
            try
            {
                if (order == null)
                {
                    return BadRequest("Order is null.");
                }

                if (string.IsNullOrEmpty(order.Name) || order.Name.Length > 100)
                {
                    return BadRequest("Order name is too long.");
                }

                if (string.IsNullOrEmpty(order.Description) || order.Description.Length > 100)
                {
                    return BadRequest("Order description is too long.");
                }
                order.Id = Guid.NewGuid();
                order.EntryDate = order.EntryDate;
                await _orderRepository.AddOrderAsync(order);


                return CreatedAtAction(nameof(GetRecentOrders), new { id = order.Id }, order);
            }
            catch (Exception ex)
            {
                log.Info(ex);
                return StatusCode(StatusCodes.Status500InternalServerError, $"Internal server error: {ex.Message}");
            }
        }


        [HttpGet("{name}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<Order>> GetOrderByname(string name)
        {
            try
            {
                var order = await _orderRepository.GetOrderByNameAsync(name);
                if (order == null)
                {
                    return NotFound();
                }
                return Ok(order);
            }
            catch (Exception ex)
            {
                log.Info(ex);
                return StatusCode(StatusCodes.Status500InternalServerError, $"Internal server error: {ex.Message}");
            }
        }
    }
}
