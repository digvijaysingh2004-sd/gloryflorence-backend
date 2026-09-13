using System.Threading;
using System.Threading.Tasks;
using FluentValidation;
using GloryFlorence.API.Common;
using GloryFlorence.Application.Common;
using GloryFlorence.Application.DTOs;
using GloryFlorence.Application.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace GloryFlorence.API.Controllers
{
    [ApiController]
    [Route("api/invoices")]
    public class InvoicesController : BaseApiController
    {
        private readonly IInvoiceService _invoiceService;
        private readonly IValidator<CreateInvoiceDto> _createInvoiceValidator;
        private readonly IValidator<CreatePaymentDto> _createPaymentValidator;

        public InvoicesController(
            IInvoiceService invoiceService,
            IValidator<CreateInvoiceDto> createInvoiceValidator,
            IValidator<CreatePaymentDto> createPaymentValidator)
        {
            _invoiceService = invoiceService;
            _createInvoiceValidator = createInvoiceValidator;
            _createPaymentValidator = createPaymentValidator;
        }

        [HttpGet]
        [ProducesResponseType(typeof(ApiResponse<PagedResult<InvoiceDto>>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAll([FromQuery] InvoiceFilterDto filter, CancellationToken cancellationToken)
        {
            var result = await _invoiceService.GetInvoicesAsync(filter, cancellationToken);
            return Ok(ApiResponse<PagedResult<InvoiceDto>>.SuccessResponse(result, "Invoices retrieved successfully."));
        }

        [HttpGet("{id:int}")]
        [ProducesResponseType(typeof(ApiResponse<InvoiceDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetById(int id, CancellationToken cancellationToken)
        {
            var invoice = await _invoiceService.GetInvoiceByIdAsync(id, cancellationToken);
            if (invoice == null)
            {
                return NotFound(ApiResponse<InvoiceDto>.FailureResponse($"Invoice with ID {id} was not found."));
            }
            return Ok(ApiResponse<InvoiceDto>.SuccessResponse(invoice, "Invoice retrieved successfully."));
        }

        [HttpPost]
        [ProducesResponseType(typeof(ApiResponse<InvoiceDto>), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Create([FromBody] CreateInvoiceDto dto, CancellationToken cancellationToken)
        {
            var validationResult = await _createInvoiceValidator.ValidateAsync(dto, cancellationToken);
            if (!validationResult.IsValid)
            {
                throw new ValidationException(validationResult.Errors);
            }

            var created = await _invoiceService.CreateInvoiceAsync(dto, cancellationToken);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, ApiResponse<InvoiceDto>.SuccessResponse(created, "Invoice generated successfully."));
        }

        [HttpPost("{id:int}/payments")]
        [ProducesResponseType(typeof(ApiResponse<PaymentDto>), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> AddPayment(int id, [FromBody] CreatePaymentDto dto, CancellationToken cancellationToken)
        {
            var validationResult = await _createPaymentValidator.ValidateAsync(dto, cancellationToken);
            if (!validationResult.IsValid)
            {
                throw new ValidationException(validationResult.Errors);
            }

            var payment = await _invoiceService.AddPaymentAsync(id, dto, cancellationToken);
            return CreatedAtAction(nameof(GetById), new { id = id }, ApiResponse<PaymentDto>.SuccessResponse(payment, "Payment recorded successfully."));
        }

        [HttpDelete("{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
        {
            await _invoiceService.DeleteInvoiceAsync(id, cancellationToken);
            return NoContent();
        }

        [HttpGet("~/api/patients/{patientId:int}/invoices")]
        [ProducesResponseType(typeof(ApiResponse<PagedResult<InvoiceDto>>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetByPatient(int patientId, [FromQuery] InvoiceFilterDto filter, CancellationToken cancellationToken)
        {
            filter.PatientId = patientId;
            var result = await _invoiceService.GetInvoicesAsync(filter, cancellationToken);
            return Ok(ApiResponse<PagedResult<InvoiceDto>>.SuccessResponse(result, "Patient invoices retrieved successfully."));
        }
    }
}
