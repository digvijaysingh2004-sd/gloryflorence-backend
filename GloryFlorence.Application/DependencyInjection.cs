using System.Reflection;
using FluentValidation;
using GloryFlorence.Application.Interfaces;
using GloryFlorence.Application.Services;
using Microsoft.Extensions.DependencyInjection;

namespace GloryFlorence.Application
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplication(this IServiceCollection services)
        {
            services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());

            services.AddScoped<IPatientService, PatientService>();
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<ITreatmentTypeService, TreatmentTypeService>();
            services.AddScoped<IExerciseService, ExerciseService>();
            services.AddScoped<IAppointmentTypeService, AppointmentTypeService>();
            services.AddScoped<IAppointmentService, AppointmentService>();
            services.AddScoped<IPatientAssessmentService, PatientAssessmentService>();
            services.AddScoped<ITreatmentPlanService, TreatmentPlanService>();
            services.AddScoped<ITreatmentSessionService, TreatmentSessionService>();
            services.AddScoped<IExercisePrescriptionService, ExercisePrescriptionService>();
            services.AddScoped<IAuditLogService, AuditLogService>();
            services.AddScoped<IInvoiceService, InvoiceService>();
            services.AddScoped<IClinicSettingsService, ClinicSettingsService>();

            return services;
        }
    }
}
