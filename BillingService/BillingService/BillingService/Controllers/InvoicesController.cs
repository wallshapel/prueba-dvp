using BillingService.Application.DTOs;
using BillingService.Application.Interfaces;
using BillingService.Application.Services;
using BillingService.WebApi.Helpers;
using Microsoft.AspNetCore.Mvc;

namespace BillingService.WebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class InvoicesController : ControllerBase
    {
        private readonly IInvoiceService _invoiceService;
        private readonly ILogger<InvoicesController> _logger;

        public InvoicesController(IInvoiceService invoiceService, ILogger<InvoicesController> logger)
        {
            _invoiceService = invoiceService;
            _logger = logger;
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateInvoiceRequest request)
        {
            try
            {
                var invoice = await _invoiceService.CreateInvoiceAsync(request);
                return StatusCode(201, ApiResponse.Success(invoice, "Invoice created successfully", 201));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating invoice");
                return BadRequest(ApiResponse.Error(ex.Message, 400));
            }
        }

        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            try
            {
                var invoice = await _invoiceService.GetByIdAsync(id);
                return Ok(ApiResponse.Success(invoice));
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Invoice not found: {Id}", id);
                return NotFound(ApiResponse.Error(ex.Message, 404));
            }
        }

        [HttpGet("range")]
        public async Task<IActionResult> GetByDateRange([FromQuery] DateTime from, [FromQuery] DateTime to)
        {
            var invoices = await _invoiceService.GetByDateRangeAsync(from, to);
            return Ok(ApiResponse.Success(invoices));
        }

    }
}
