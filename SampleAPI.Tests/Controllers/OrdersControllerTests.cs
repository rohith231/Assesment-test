using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using SampleAPI.Controllers;
using SampleAPI.Entities;
using SampleAPI.Repositories;

public class OrdersControllerTests
{
    private readonly OrdersController _controller;
    private readonly Mock<IOrderRepository> _mockRepository;

    public OrdersControllerTests()
    {
        _mockRepository = new Mock<IOrderRepository>();
        _controller = new OrdersController(_mockRepository.Object);
    }

    [Fact]
    public async Task GetRecentOrders_ReturnsOkResult_WithRecentOrders()
    {
        
        var orders = new List<Order>
    {
        new Order { EntryDate = DateTime.UtcNow.AddHours(-1), Name = "Recent Order", Description = "Recent Order Description" },
        new Order { EntryDate = DateTime.UtcNow.AddDays(-2), Name = "Old Order", Description = "Old Order Description", Deleted = true }
    };

        _mockRepository.Setup(repo => repo.GetRecentOrdersAsync()).ReturnsAsync(orders.Where(o => o.EntryDate >= DateTime.UtcNow.AddDays(-1) && !o.Deleted).ToList());

        
        var result = await _controller.GetRecentOrders();

        // Assert
        var actionResult = Assert.IsType<ActionResult<List<Order>>>(result);

        var okResult = Assert.IsType<OkObjectResult>(actionResult.Result);
        var returnOrders = Assert.IsType<List<Order>>(okResult.Value); 

        Assert.Single(returnOrders);
        Assert.Equal("Recent Order", returnOrders.First().Name); 
    }

    [Fact]
    public async Task GetRecentOrders_ReturnsEmptyList_WhenNoRecentOrders()
    {
        
        _mockRepository.Setup(repo => repo.GetRecentOrdersAsync()).ReturnsAsync(new List<Order>());

        
        var result = await _controller.GetRecentOrders();

       
        var actionResult = Assert.IsType<ActionResult<List<Order>>>(result);

        var okResult = Assert.IsType<OkObjectResult>(actionResult.Result);

        var returnOrders = Assert.IsType<List<Order>>(okResult.Value);
        Assert.Empty(returnOrders);
    }

    [Fact]
    public async Task GetRecentOrders_ReturnsStatusCode500_WhenExceptionThrown()
    {
        
        var exceptionMessage = "Test Exception";
        _mockRepository.Setup(repo => repo.GetRecentOrdersAsync()).Throws(new Exception(exceptionMessage));

        
        var result = await _controller.GetRecentOrders();

        var actionResult = Assert.IsType<ActionResult<List<Order>>>(result); 

        var objectResult = Assert.IsType<ObjectResult>(actionResult.Result);

        Assert.Equal(StatusCodes.Status500InternalServerError, objectResult.StatusCode);

        Assert.Equal($"Internal server error: {exceptionMessage}", objectResult.Value);
    }
   
    [Fact]
    public async Task SubmitOrder_ReturnsBadRequest_WhenOrderNameIsTooLong()
    {
        
        var invalidOrder = new Order { Name = new string('a', 101), Description = "Valid Description" };

        
        var result = await _controller.CreateOrderRequest(invalidOrder);

        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
        Assert.Equal("Order name is too long.", badRequestResult.Value);
    }
    [Fact]
    public async Task SubmitOrder_ReturnsBadRequest_WhenOrderDescriptionIsTooLong()
    {
        // Arrange
        var invalidOrder = new Order { Name = "Valid Name", Description = new string('a', 101) };

        // Act
        var result = await _controller.CreateOrderRequest(invalidOrder);

        // Assert
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
        Assert.Equal("Order description is too long.", badRequestResult.Value);
    }


    [Fact]
    public async Task SubmitOrder_ReturnsCreatedAtActionResult_WithValidOrder()
    {
        
        var order = new Order { Name = "New Order", Description = "New Order Description" };
        _mockRepository.Setup(repo => repo.AddOrderAsync(It.IsAny<Order>())).Returns(Task.CompletedTask);

        
        var result = await _controller.CreateOrderRequest(order);

        var createdAtActionResult = Assert.IsType<CreatedAtActionResult>(result);
        Assert.Equal("GetRecentOrders", createdAtActionResult.ActionName);
        var returnOrder = Assert.IsType<Order>(createdAtActionResult.Value);
        Assert.Equal("New Order", returnOrder.Name);
    }

    [Fact]
    public async Task SubmitOrder_ReturnsBadRequest_WhenOrderIsNull()
    {
        
        var result = await _controller.CreateOrderRequest(null);

        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
        Assert.Equal("Order is null.", badRequestResult.Value);
    }

    [Fact]
    public async Task SubmitOrder_ReturnsBadRequest_WhenOrderIsInvalid()
    {
        
        var invalidOrder = new Order { Name = new string('a', 101), Description = "Invalid Order Description" };
        var result = await _controller.CreateOrderRequest(invalidOrder);
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
        Assert.Equal("Order name is too long.", badRequestResult.Value); 
    }





}
