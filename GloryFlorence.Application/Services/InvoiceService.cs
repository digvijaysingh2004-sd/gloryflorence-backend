using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using GloryFlorence.Application.Common;
using GloryFlorence.Application.Common.Exceptions;
using GloryFlorence.Application.DTOs;
using GloryFlorence.Application.Interfaces;
using GloryFlorence.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace GloryFlorence.Application.Services
{
    public class InvoiceService : IInvoiceService
    {
        private readonly IUnitOfWork _unitOfWork;

        public InvoiceService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<PagedResult<InvoiceDto>> GetInvoicesAsync(InvoiceFilterDto filter, CancellationToken cancellationToken)
        {
            var query = _unitOfWork.Invoices.Query()
                .Include(i => i.Patient)
                .Include(i => i.InvoiceItems)
                .Include(i => i.Payments)
                .AsQueryable();

            if (filter.PatientId.HasValue)
            {
                query = query.Where(i => i.PatientId == filter.PatientId.Value);
            }

            if (!string.IsNullOrWhiteSpace(filter.Status))
            {
                var statusStr = filter.Status.Trim().ToLower();
                query = query.Where(i => i.Status.ToLower() == statusStr);
            }

            if (!string.IsNullOrWhiteSpace(filter.Search))
            {
                var search = filter.Search.Trim().ToLower();
                query = query.Where(i =>
                    i.InvoiceNumber.ToLower().Contains(search) ||
                    (i.Patient != null && (i.Patient.FirstName.ToLower().Contains(search) || i.Patient.LastName.ToLower().Contains(search))));
            }

            var totalCount = await query.CountAsync(cancellationToken);

            var page = filter.PageNumber < 1 ? 1 : filter.PageNumber;
            var pageSize = filter.PageSize < 1 ? 10 : filter.PageSize;

            var items = await query
                .OrderByDescending(i => i.InvoiceDate)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync(cancellationToken);

            var dtos = items.Select(MapToDto).ToList();

            return new PagedResult<InvoiceDto>(dtos, totalCount, page, pageSize);
        }

        public async Task<InvoiceDto?> GetInvoiceByIdAsync(int id, CancellationToken cancellationToken)
        {
            var invoice = await _unitOfWork.Invoices.Query()
                .Include(i => i.Patient)
                .Include(i => i.InvoiceItems)
                .Include(i => i.Payments)
                .FirstOrDefaultAsync(i => i.Id == id, cancellationToken);

            if (invoice == null)
            {
                return null;
            }

            return MapToDto(invoice);
        }

        public async Task<InvoiceDto> CreateInvoiceAsync(CreateInvoiceDto dto, CancellationToken cancellationToken)
        {
            var patient = await _unitOfWork.Patients.GetByIdAsync(dto.PatientId, cancellationToken);
            if (patient == null)
            {
                throw new NotFoundException(nameof(Patient), dto.PatientId);
            }

            var itemsSubTotal = dto.Items.Sum(x => x.Quantity * x.UnitPrice);
            var subTotal = dto.SubTotal > 0 ? dto.SubTotal : itemsSubTotal;
            var taxAmount = dto.TaxAmount ?? 0m;
            var discountAmount = dto.DiscountAmount ?? 0m;
            var totalAmount = dto.TotalAmount > 0 ? dto.TotalAmount : (subTotal + taxAmount - discountAmount);

            var now = DateTime.UtcNow;
            var invoiceNumber = $"INV-{now:yyyyMMdd}-{Random.Shared.Next(1000, 9999)}";

            var invoice = new Invoice
            {
                InvoiceNumber = invoiceNumber,
                PatientId = dto.PatientId,
                SubTotal = subTotal,
                TaxAmount = taxAmount,
                DiscountAmount = discountAmount,
                TotalAmount = totalAmount,
                Amount = totalAmount,
                PaidAmount = 0m,
                BalanceAmount = totalAmount,
                Status = "Unpaid",
                InvoiceDate = dto.InvoiceDate ?? now,
                DueDate = dto.DueDate ?? now.AddDays(30),
                Notes = dto.Notes,
                CreatedAt = now
            };

            foreach (var item in dto.Items)
            {
                var itemTotal = item.TotalAmount > 0 ? item.TotalAmount : (item.Quantity * item.UnitPrice);
                invoice.InvoiceItems.Add(new InvoiceItem
                {
                    TreatmentTypeId = item.TreatmentTypeId,
                    Description = item.Description,
                    Quantity = item.Quantity,
                    UnitPrice = item.UnitPrice,
                    TotalAmount = itemTotal
                });
            }

            await _unitOfWork.Invoices.AddAsync(invoice, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return (await GetInvoiceByIdAsync(invoice.Id, cancellationToken))!;
        }

        public async Task<PaymentDto> AddPaymentAsync(int invoiceId, CreatePaymentDto dto, CancellationToken cancellationToken)
        {
            var invoice = await _unitOfWork.Invoices.Query()
                .Include(i => i.Payments)
                .FirstOrDefaultAsync(i => i.Id == invoiceId, cancellationToken);

            if (invoice == null)
            {
                throw new NotFoundException(nameof(Invoice), invoiceId);
            }

            var payment = new Payment
            {
                InvoiceId = invoiceId,
                AmountPaid = dto.AmountPaid,
                PaymentMethod = dto.PaymentMethod,
                TransactionReference = dto.TransactionReference,
                PaymentDate = dto.PaymentDate ?? DateTime.UtcNow,
                Notes = dto.Notes
            };

            await _unitOfWork.Payments.AddAsync(payment, cancellationToken);

            invoice.PaidAmount += dto.AmountPaid;
            var fullTotal = invoice.TotalAmount > 0 ? invoice.TotalAmount : invoice.Amount;
            invoice.BalanceAmount = fullTotal - invoice.PaidAmount;

            if (invoice.BalanceAmount <= 0)
            {
                invoice.Status = "Paid";
                invoice.BalanceAmount = 0;
            }
            else if (invoice.PaidAmount > 0)
            {
                invoice.Status = "Partially Paid";
            }

            invoice.UpdatedAt = DateTime.UtcNow;
            _unitOfWork.Invoices.Update(invoice);

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return new PaymentDto
            {
                Id = payment.Id,
                InvoiceId = payment.InvoiceId,
                AmountPaid = payment.AmountPaid,
                PaymentMethod = payment.PaymentMethod,
                TransactionReference = payment.TransactionReference,
                PaymentDate = payment.PaymentDate,
                Notes = payment.Notes
            };
        }

        public async Task DeleteInvoiceAsync(int id, CancellationToken cancellationToken)
        {
            var invoice = await _unitOfWork.Invoices.GetByIdAsync(id, cancellationToken);
            if (invoice == null)
            {
                throw new NotFoundException(nameof(Invoice), id);
            }

            _unitOfWork.Invoices.Delete(invoice);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }

        private static InvoiceDto MapToDto(Invoice invoice)
        {
            var fullTotal = invoice.TotalAmount > 0 ? invoice.TotalAmount : invoice.Amount;
            return new InvoiceDto
            {
                Id = invoice.Id,
                InvoiceNumber = invoice.InvoiceNumber,
                PatientId = invoice.PatientId,
                PatientName = invoice.Patient != null ? $"{invoice.Patient.FirstName} {invoice.Patient.LastName}".Trim() : string.Empty,
                PatientEmail = invoice.Patient?.Email,
                PatientPhone = invoice.Patient?.PhoneNumber,
                SubTotal = invoice.SubTotal > 0 ? invoice.SubTotal : fullTotal,
                TaxAmount = invoice.TaxAmount,
                DiscountAmount = invoice.DiscountAmount,
                TotalAmount = fullTotal,
                Amount = fullTotal,
                PaidAmount = invoice.PaidAmount,
                BalanceAmount = invoice.BalanceAmount,
                Status = invoice.Status,
                InvoiceDate = invoice.InvoiceDate,
                DueDate = invoice.DueDate,
                Notes = invoice.Notes,
                CreatedAt = invoice.CreatedAt,
                Items = invoice.InvoiceItems.Select(item => new InvoiceItemDto
                {
                    Id = item.Id,
                    InvoiceId = item.InvoiceId,
                    TreatmentTypeId = item.TreatmentTypeId,
                    Description = item.Description,
                    Quantity = item.Quantity,
                    UnitPrice = item.UnitPrice,
                    TotalAmount = item.TotalAmount
                }).ToList(),
                Payments = invoice.Payments.Select(p => new PaymentDto
                {
                    Id = p.Id,
                    InvoiceId = p.InvoiceId,
                    AmountPaid = p.AmountPaid,
                    PaymentMethod = p.PaymentMethod,
                    TransactionReference = p.TransactionReference,
                    PaymentDate = p.PaymentDate,
                    Notes = p.Notes
                }).ToList()
            };
        }
    }
}
