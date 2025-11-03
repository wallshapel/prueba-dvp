using BillingService.Application.DTOs;
using BillingService.Application.Interfaces;
using BillingService.Application.Services;
using BillingService.WebApi.Helpers;
using Microsoft.AspNetCore.Mvc;

namespace BillingService.WebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CustomersController : ControllerBase
    {
        private readonly ICustomerService _customerService;
        private readonly ILogger<CustomersController> _logger;

        public CustomersController(ICustomerService customerService, ILogger<CustomersController> logger)
        {
            _customerService = customerService;
            _logger = logger;
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateCustomerRequest request)
        {
            try
            {
                var customer = await _customerService.CreateCustomerAsync(request);
                return StatusCode(201, ApiResponse.Success(customer, "Customer created successfully", 201));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating customer");
                return BadRequest(ApiResponse.Error(ex.Message, 400));
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var customers = await _customerService.GetAllAsync();
            return Ok(ApiResponse.Success(customers));
        }

        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            try
            {
                var customer = await _customerService.GetByIdAsync(id);
                return Ok(ApiResponse.Success(customer));
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Customer not found: {Id}", id);
                return NotFound(ApiResponse.Error(ex.Message, 404));
            }
        }
    }
}
