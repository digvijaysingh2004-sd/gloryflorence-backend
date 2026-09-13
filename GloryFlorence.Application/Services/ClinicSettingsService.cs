using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using GloryFlorence.Application.DTOs;
using GloryFlorence.Application.Interfaces;
using GloryFlorence.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace GloryFlorence.Application.Services
{
    public class ClinicSettingsService : IClinicSettingsService
    {
        private readonly IUnitOfWork _unitOfWork;

        public ClinicSettingsService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<ClinicSettingsDto> GetClinicSettingsAsync(CancellationToken cancellationToken)
        {
            var settings = await _unitOfWork.ClinicSettings.Query().FirstOrDefaultAsync(cancellationToken);

            if (settings == null)
            {
                settings = new ClinicSettings();
                await _unitOfWork.ClinicSettings.AddAsync(settings, cancellationToken);
                await _unitOfWork.SaveChangesAsync(cancellationToken);
            }

            return MapToDto(settings);
        }

        public async Task<ClinicSettingsDto> UpdateClinicSettingsAsync(UpdateClinicSettingsDto dto, CancellationToken cancellationToken)
        {
            var settings = await _unitOfWork.ClinicSettings.Query().FirstOrDefaultAsync(cancellationToken);

            if (settings == null)
            {
                settings = new ClinicSettings();
                await _unitOfWork.ClinicSettings.AddAsync(settings, cancellationToken);
            }

            if (!string.IsNullOrWhiteSpace(dto.ClinicName)) settings.ClinicName = dto.ClinicName;
            if (dto.Tagline != null) settings.Tagline = dto.Tagline;
            if (dto.Email != null) settings.Email = dto.Email;
            if (dto.Phone != null) settings.Phone = dto.Phone;
            if (dto.Address != null) settings.Address = dto.Address;
            if (dto.City != null) settings.City = dto.City;
            if (dto.State != null) settings.State = dto.State;
            if (dto.Country != null) settings.Country = dto.Country;
            if (dto.PostalCode != null) settings.PostalCode = dto.PostalCode;
            if (dto.TaxRegistrationNumber != null) settings.TaxRegistrationNumber = dto.TaxRegistrationNumber;
            if (dto.CurrencySymbol != null) settings.CurrencySymbol = dto.CurrencySymbol;
            if (dto.WorkingHoursStart.HasValue) settings.WorkingHoursStart = dto.WorkingHoursStart.Value;
            if (dto.WorkingHoursEnd.HasValue) settings.WorkingHoursEnd = dto.WorkingHoursEnd.Value;
            if (dto.AppointmentSlotDurationMinutes.HasValue) settings.AppointmentSlotDurationMinutes = dto.AppointmentSlotDurationMinutes.Value;
            if (dto.AutoConfirmAppointments.HasValue) settings.AutoConfirmAppointments = dto.AutoConfirmAppointments.Value;
            if (dto.EnableSmsNotifications.HasValue) settings.EnableSmsNotifications = dto.EnableSmsNotifications.Value;
            if (dto.EnableEmailNotifications.HasValue) settings.EnableEmailNotifications = dto.EnableEmailNotifications.Value;

            settings.UpdatedAt = DateTime.UtcNow;

            _unitOfWork.ClinicSettings.Update(settings);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return MapToDto(settings);
        }

        private static ClinicSettingsDto MapToDto(ClinicSettings s)
        {
            return new ClinicSettingsDto
            {
                Id = s.Id,
                ClinicName = s.ClinicName,
                Tagline = s.Tagline,
                Email = s.Email,
                Phone = s.Phone,
                Address = s.Address,
                City = s.City,
                State = s.State,
                Country = s.Country,
                PostalCode = s.PostalCode,
                TaxRegistrationNumber = s.TaxRegistrationNumber,
                CurrencySymbol = s.CurrencySymbol,
                WorkingHoursStart = s.WorkingHoursStart,
                WorkingHoursEnd = s.WorkingHoursEnd,
                AppointmentSlotDurationMinutes = s.AppointmentSlotDurationMinutes,
                AutoConfirmAppointments = s.AutoConfirmAppointments,
                EnableSmsNotifications = s.EnableSmsNotifications,
                EnableEmailNotifications = s.EnableEmailNotifications,
                UpdatedAt = s.UpdatedAt
            };
        }
    }
}
