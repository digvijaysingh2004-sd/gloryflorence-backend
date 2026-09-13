using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using GloryFlorence.Application.Common;
using GloryFlorence.Application.DTOs;

namespace GloryFlorence.Application.Interfaces
{
    public interface IInvoiceService
    {
        Task<PagedResult<InvoiceDto>> GetInvoicesAsync(InvoiceFilterDto filter, CancellationToken cancellationToken);
        Task<InvoiceDto?> GetInvoiceByIdAsync(int id, CancellationToken cancellationToken);
        Task<InvoiceDto> CreateInvoiceAsync(CreateInvoiceDto dto, CancellationToken cancellationToken);
        Task<PaymentDto> AddPaymentAsync(int invoiceId, CreatePaymentDto dto, CancellationToken cancellationToken);
        Task DeleteInvoiceAsync(int id, CancellationToken cancellationToken);
    }
}
