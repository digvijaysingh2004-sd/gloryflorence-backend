using System;
using System.Linq;
using System.Threading.Tasks;
using GloryFlorence.Domain.Constants;
using GloryFlorence.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace GloryFlorence.Infrastructure.Data
{
    public static class ApplicationDbContextSeed
    {
        public static async Task SeedAsync(ApplicationDbContext context)
        {
            // Auto-apply migrations if pending
            // SQL Server check (commented out):
            // if (context.Database.IsSqlServer() && (await context.Database.GetPendingMigrationsAsync()).Any())
            if ((await context.Database.GetPendingMigrationsAsync()).Any())
            {
                await context.Database.MigrateAsync();
            }

            // Seed Roles and Default Users
            await SeedUsersAsync(context);

            // Seed Genders
            if (!await context.Genders.AnyAsync())
            {
                await context.Genders.AddRangeAsync(
                    new Gender { Name = "Male", Code = "M" },
                    new Gender { Name = "Female", Code = "F" },
                    new Gender { Name = "Other", Code = "O" }
                );
                await context.SaveChangesAsync();
            }

            // Seed BloodGroups
            if (!await context.BloodGroups.AnyAsync())
            {
                await context.BloodGroups.AddRangeAsync(
                    new BloodGroup { Name = "A+" },
                    new BloodGroup { Name = "A-" },
                    new BloodGroup { Name = "B+" },
                    new BloodGroup { Name = "B-" },
                    new BloodGroup { Name = "AB+" },
                    new BloodGroup { Name = "AB-" },
                    new BloodGroup { Name = "O+" },
                    new BloodGroup { Name = "O-" }
                );
                await context.SaveChangesAsync();
            }

            // Seed Countries, States, and Cities
            if (!await context.Countries.AnyAsync())
            {
                var india = new Country { Name = "India", Code = "IN" };
                var usa = new Country { Name = "United States", Code = "US" };
                await context.Countries.AddRangeAsync(india, usa);
                await context.SaveChangesAsync();

                var maharashtra = new State { Name = "Maharashtra", CountryId = india.Id };
                var nyState = new State { Name = "New York", CountryId = usa.Id };
                await context.States.AddRangeAsync(maharashtra, nyState);
                await context.SaveChangesAsync();

                var mumbai = new City { Name = "Mumbai", StateId = maharashtra.Id };
                var pune = new City { Name = "Pune", StateId = maharashtra.Id };
                var nyc = new City { Name = "New York City", StateId = nyState.Id };
                await context.Cities.AddRangeAsync(mumbai, pune, nyc);
                await context.SaveChangesAsync();
            }

            // Seed Specializations
            if (!await context.Specializations.AnyAsync())
            {
                await context.Specializations.AddRangeAsync(
                    new Specialization { Name = "Orthopedic", Description = "Treatment of musculoskeletal system disorders" },
                    new Specialization { Name = "Neurological", Description = "Rehabilitation of patients with neurological disorders" },
                    new Specialization { Name = "Pediatrics", Description = "Therapy for infants, toddlers, and children" },
                    new Specialization { Name = "Sports Medicine", Description = "Prevention and treatment of sports and exercise injuries" }
                );
                await context.SaveChangesAsync();
            }

            // Seed Categories
            if (!await context.Categories.AnyAsync())
            {
                await context.Categories.AddRangeAsync(
                    new Category { Name = "Consultation", Description = "Initial assessment and evaluation" },
                    new Category { Name = "Therapy Session", Description = "Standard physiotherapy treatment session" },
                    new Category { Name = "Follow-up", Description = "Progress review and adjustment of treatment" }
                );
                await context.SaveChangesAsync();
            }

            // Seed Patients
            if (!await context.Patients.AnyAsync())
            {
                await context.Patients.AddRangeAsync(
                    new Patient
                    {
                        FirstName = "John",
                        LastName = "Doe",
                        DateOfBirth = new DateTime(1990, 5, 15),
                        Gender = "Male",
                        Email = "john.doe@example.com",
                        PhoneNumber = "9876543210",
                        Address = "123 Main St, Mumbai, Maharashtra, India",
                        MedicalHistory = "Chronic lower back pain for 6 months. History of knee sprain.",
                        CreatedAt = DateTime.UtcNow
                    },
                    new Patient
                    {
                        FirstName = "Jane",
                        LastName = "Smith",
                        DateOfBirth = new DateTime(1985, 8, 22),
                        Gender = "Female",
                        Email = "jane.smith@example.com",
                        PhoneNumber = "8765432109",
                        Address = "456 Broadway, New York City, NY, USA",
                        MedicalHistory = "Post-operative ACL reconstruction rehabilitation. Left knee.",
                        CreatedAt = DateTime.UtcNow
                    }
                );
                await context.SaveChangesAsync();
            }

            // Seed Statuses (Generic Status Master)
            if (!await context.Statuses.AnyAsync())
            {
                var statuses = new[]
                {
                    // Appointment Statuses
                    new Status { Type = "Appointment", Code = "SCHEDULED", Name = "Scheduled", Description = "Appointment has been booked and scheduled" },
                    new Status { Type = "Appointment", Code = "CONFIRMED", Name = "Confirmed", Description = "Appointment confirmed by clinic or patient" },
                    new Status { Type = "Appointment", Code = "IN_PROGRESS", Name = "In Progress", Description = "Session is currently in progress" },
                    new Status { Type = "Appointment", Code = "COMPLETED", Name = "Completed", Description = "Appointment completed successfully" },
                    new Status { Type = "Appointment", Code = "CANCELLED", Name = "Cancelled", Description = "Appointment was cancelled" },
                    new Status { Type = "Appointment", Code = "RESCHEDULED", Name = "Rescheduled", Description = "Appointment was rescheduled to a new time" },
                    new Status { Type = "Appointment", Code = "NO_SHOW", Name = "No Show", Description = "Patient did not attend appointment" },

                    // TreatmentPlan Statuses
                    new Status { Type = "TreatmentPlan", Code = "DRAFT", Name = "Draft", Description = "Plan is in draft mode" },
                    new Status { Type = "TreatmentPlan", Code = "ACTIVE", Name = "Active", Description = "Plan is actively being executed" },
                    new Status { Type = "TreatmentPlan", Code = "COMPLETED", Name = "Completed", Description = "Plan goals achieved and completed" },
                    new Status { Type = "TreatmentPlan", Code = "DISCONTINUED", Name = "Discontinued", Description = "Plan discontinued before completion" },

                    // TreatmentSession Statuses
                    new Status { Type = "TreatmentSession", Code = "SCHEDULED", Name = "Scheduled", Description = "Session scheduled" },
                    new Status { Type = "TreatmentSession", Code = "IN_PROGRESS", Name = "In Progress", Description = "Session is currently being conducted" },
                    new Status { Type = "TreatmentSession", Code = "COMPLETED", Name = "Completed", Description = "Session completed successfully" },
                    new Status { Type = "TreatmentSession", Code = "CANCELLED", Name = "Cancelled", Description = "Session cancelled" },
                    new Status { Type = "TreatmentSession", Code = "NO_SHOW", Name = "No Show", Description = "Patient did not attend session" },

                    // Invoice Statuses
                    new Status { Type = "Invoice", Code = "DRAFT", Name = "Draft", Description = "Invoice generated in draft" },
                    new Status { Type = "Invoice", Code = "ISSUED", Name = "Issued", Description = "Invoice issued to patient" },
                    new Status { Type = "Invoice", Code = "PAID", Name = "Paid", Description = "Invoice has been paid in full" },
                    new Status { Type = "Invoice", Code = "PARTIALLY_PAID", Name = "Partially Paid", Description = "Partial payment received" },
                    new Status { Type = "Invoice", Code = "CANCELLED", Name = "Cancelled", Description = "Invoice was voided or cancelled" },

                    // Payment Statuses
                    new Status { Type = "Payment", Code = "PENDING", Name = "Pending", Description = "Payment processing is pending" },
                    new Status { Type = "Payment", Code = "COMPLETED", Name = "Completed", Description = "Payment completed successfully" },
                    new Status { Type = "Payment", Code = "FAILED", Name = "Failed", Description = "Payment attempt failed" },
                    new Status { Type = "Payment", Code = "REFUNDED", Name = "Refunded", Description = "Payment was refunded" }
                };

                await context.Statuses.AddRangeAsync(statuses);
                await context.SaveChangesAsync();
            }

            // Seed AppointmentTypes
            if (!await context.AppointmentTypes.AnyAsync())
            {
                await context.AppointmentTypes.AddRangeAsync(
                    new AppointmentType { Name = "Initial Assessment", DurationMinutes = 60, Description = "Comprehensive initial physical assessment and intake evaluation" },
                    new AppointmentType { Name = "Physiotherapy Session", DurationMinutes = 45, Description = "Standard individual physiotherapy treatment session" },
                    new AppointmentType { Name = "Manual Therapy", DurationMinutes = 45, Description = "Hands-on joint and soft tissue mobilization session" },
                    new AppointmentType { Name = "Sports Injury Rehab", DurationMinutes = 60, Description = "Specialized rehabilitation for athletic and exercise injuries" },
                    new AppointmentType { Name = "Post-Op Rehabilitation", DurationMinutes = 60, Description = "Targeted post-surgical physical rehabilitation" },
                    new AppointmentType { Name = "Initial Consultation", DurationMinutes = 45, Description = "Comprehensive evaluation and intake assessment" },
                    new AppointmentType { Name = "Standard Physiotherapy Session", DurationMinutes = 30, Description = "Standard one-on-one physiotherapy treatment" }
                );
                await context.SaveChangesAsync();
            }

            // Seed TreatmentTypes
            if (!await context.TreatmentTypes.AnyAsync())
            {
                var therapySessionCategory = await context.Categories.FirstOrDefaultAsync(c => c.Name == "Therapy Session")
                    ?? await context.Categories.FirstAsync();
                var consultationCategory = await context.Categories.FirstOrDefaultAsync(c => c.Name == "Consultation")
                    ?? therapySessionCategory;
                var followUpCategory = await context.Categories.FirstOrDefaultAsync(c => c.Name == "Follow-up")
                    ?? therapySessionCategory;

                await context.TreatmentTypes.AddRangeAsync(
                    new TreatmentType { CategoryId = therapySessionCategory.Id, Name = "Manual Therapy", Description = "Hands-on techniques to manipulate joints and soft tissue", DefaultDurationMinutes = 45, DefaultPrice = 50.00m },
                    new TreatmentType { CategoryId = therapySessionCategory.Id, Name = "Ultrasound Therapy", Description = "High frequency sound waves for deep tissue healing", DefaultDurationMinutes = 30, DefaultPrice = 40.00m },
                    new TreatmentType { CategoryId = therapySessionCategory.Id, Name = "Electrotherapy (TENS/IFT)", Description = "Electrical stimulation for pain relief and muscle stimulation", DefaultDurationMinutes = 30, DefaultPrice = 35.00m },
                    new TreatmentType { CategoryId = therapySessionCategory.Id, Name = "Therapeutic Exercise Session", Description = "Supervised targeted exercise protocol", DefaultDurationMinutes = 45, DefaultPrice = 45.00m },
                    new TreatmentType { CategoryId = consultationCategory.Id, Name = "Initial Physical Assessment", Description = "Thorough musculoskeletal and functional assessment", DefaultDurationMinutes = 60, DefaultPrice = 75.00m },
                    new TreatmentType { CategoryId = followUpCategory.Id, Name = "Follow-up Progress Review", Description = "Periodic check-in and program calibration", DefaultDurationMinutes = 30, DefaultPrice = 30.00m }
                );
                await context.SaveChangesAsync();
            }

            // Seed Exercises
            if (!await context.Exercises.AnyAsync())
            {
                var therapySessionCategory = await context.Categories.FirstOrDefaultAsync(c => c.Name == "Therapy Session")
                    ?? await context.Categories.FirstAsync();

                await context.Exercises.AddRangeAsync(
                    new Exercise
                    {
                        CategoryId = therapySessionCategory.Id,
                        Name = "Pelvic Tilt",
                        Description = "Gentle core exercise for lower back pain and lumbar stability",
                        Instructions = "Lie on back with knees bent. Contract abdominal muscles to flatten lower back firmly against the mat. Hold 5 seconds, relax, and repeat 10 times.",
                        VideoUrl = "https://example.com/videos/pelvic-tilt",
                        ImageUrl = "https://example.com/images/pelvic-tilt.jpg"
                    },
                    new Exercise
                    {
                        CategoryId = therapySessionCategory.Id,
                        Name = "Hamstring Stretch",
                        Description = "Flexibility stretch for posterior thigh muscles",
                        Instructions = "Lie flat. Loop a towel or strap around the ball of one foot. Keeping the knee straight, pull gently toward the chest until a stretch is felt behind the thigh. Hold 30 seconds.",
                        VideoUrl = "https://example.com/videos/hamstring-stretch",
                        ImageUrl = "https://example.com/images/hamstring-stretch.jpg"
                    },
                    new Exercise
                    {
                        CategoryId = therapySessionCategory.Id,
                        Name = "Quadriceps Sets",
                        Description = "Isometric exercise to strengthen anterior thigh and stabilize knee joint",
                        Instructions = "Sit or lie with leg extended straight. Tighten the thigh muscle to press the back of the knee down against the table. Hold for 5 seconds and repeat 15 times.",
                        VideoUrl = "https://example.com/videos/quad-sets",
                        ImageUrl = "https://example.com/images/quad-sets.jpg"
                    },
                    new Exercise
                    {
                        CategoryId = therapySessionCategory.Id,
                        Name = "Cervical Retraction (Chin Tucks)",
                        Description = "Postural exercise for neck alignment and cervical spine relief",
                        Instructions = "Sit upright with shoulders relaxed. Look straight ahead and gently pull the chin straight backward as if making a double chin. Hold 5 seconds and repeat 10 times.",
                        VideoUrl = "https://example.com/videos/chin-tucks",
                        ImageUrl = "https://example.com/images/chin-tucks.jpg"
                    },
                    new Exercise
                    {
                        CategoryId = therapySessionCategory.Id,
                        Name = "Shoulder Pendulum Exercise",
                        Description = "Gentle range of motion exercise for acute shoulder rehab",
                        Instructions = "Bend forward at the waist supporting torso with unaffected arm on a table. Let the affected arm hang relaxed. Gently swing it forward, backward, side-to-side and in small circles.",
                        VideoUrl = "https://example.com/videos/shoulder-pendulum",
                        ImageUrl = "https://example.com/images/shoulder-pendulum.jpg"
                    }
                );
                await context.SaveChangesAsync();
            }

            // Seed Appointments
            if (!await context.Appointments.AnyAsync())
            {
                var john = await context.Patients.FirstOrDefaultAsync(p => p.FirstName == "John");
                var jane = await context.Patients.FirstOrDefaultAsync(p => p.FirstName == "Jane");
                var therapistUser = await context.Users.FirstOrDefaultAsync(u => u.Role == Roles.Physiotherapist);
                var initConsultation = await context.AppointmentTypes.FirstOrDefaultAsync(at => at.Name == "Initial Consultation")
                    ?? await context.AppointmentTypes.FirstAsync();
                var stdSession = await context.AppointmentTypes.FirstOrDefaultAsync(at => at.Name == "Standard Physiotherapy Session")
                    ?? await context.AppointmentTypes.FirstAsync();

                if (john != null && jane != null && therapistUser != null)
                {
                    var today = DateTime.UtcNow.Date;

                    await context.Appointments.AddRangeAsync(
                        new Appointment
                        {
                            PatientId = john.Id,
                            PhysiotherapistId = therapistUser.Id,
                            AppointmentTypeId = initConsultation.Id,
                            AppointmentDate = today.AddDays(-2),
                            StartTime = new TimeSpan(9, 0, 0),
                            EndTime = new TimeSpan(9, 45, 0),
                            Status = "Completed",
                            Reason = "Initial intake and lower back pain evaluation",
                            Notes = "Patient attended on time. Complained of stiffness and pain.",
                            CreatedAt = DateTime.UtcNow.AddDays(-3)
                        },
                        new Appointment
                        {
                            PatientId = jane.Id,
                            PhysiotherapistId = therapistUser.Id,
                            AppointmentTypeId = stdSession.Id,
                            AppointmentDate = today.AddDays(-1),
                            StartTime = new TimeSpan(10, 30, 0),
                            EndTime = new TimeSpan(11, 0, 0),
                            Status = "Completed",
                            Reason = "Post-op ACL rehab session",
                            Notes = "Performed well with ultrasound and isometric quad sets.",
                            CreatedAt = DateTime.UtcNow.AddDays(-2)
                        },
                        new Appointment
                        {
                            PatientId = john.Id,
                            PhysiotherapistId = therapistUser.Id,
                            AppointmentTypeId = stdSession.Id,
                            AppointmentDate = today.AddDays(1),
                            StartTime = new TimeSpan(14, 0, 0),
                            EndTime = new TimeSpan(14, 30, 0),
                            Status = "Scheduled",
                            Reason = "Follow-up back rehab and manual therapy",
                            Notes = "Upcoming session for lumbar mobility.",
                            CreatedAt = DateTime.UtcNow.AddDays(-1)
                        }
                    );
                    await context.SaveChangesAsync();
                }
            }

            // Seed PatientAssessments
            if (!await context.PatientAssessments.AnyAsync())
            {
                var john = await context.Patients.FirstOrDefaultAsync(p => p.FirstName == "John");
                var jane = await context.Patients.FirstOrDefaultAsync(p => p.FirstName == "Jane");
                var therapistUser = await context.Users.FirstOrDefaultAsync(u => u.Role == Roles.Physiotherapist);

                if (john != null && jane != null && therapistUser != null)
                {
                    await context.PatientAssessments.AddRangeAsync(
                        new PatientAssessment
                        {
                            PatientId = john.Id,
                            PhysiotherapistId = therapistUser.Id,
                            AssessmentDate = DateTime.UtcNow.AddDays(-2),
                            ChiefComplaint = "Persistent aching pain in the lower lumbar region, aggravated by prolonged sitting.",
                            CurrentCondition = "Reduced lumbar flexion, hamstring tightness, localized tenderness at L4-L5 vertebrae.",
                            PainLevel = 7,
                            Diagnosis = "Chronic Mechanical Lower Back Strain with Lumbar Facet Joint Syndrome",
                            ClinicalNotes = "Patient works desk job 8+ hours daily. Neurological screening negative for radiculopathy. Normal reflex responses.",
                            Recommendations = "12-session rehabilitation plan focusing on core stabilization, pelvic tilts, and hamstring stretching.",
                            CreatedAt = DateTime.UtcNow.AddDays(-2)
                        },
                        new PatientAssessment
                        {
                            PatientId = jane.Id,
                            PhysiotherapistId = therapistUser.Id,
                            AssessmentDate = DateTime.UtcNow.AddDays(-1),
                            ChiefComplaint = "Post-operative stiffness and moderate pain in left knee following ACL reconstruction.",
                            CurrentCondition = "Active knee flexion limited to 90 degrees. Mild joint effusion present. Quadriceps atrophy noted.",
                            PainLevel = 5,
                            Diagnosis = "Post-ACL Reconstruction Arthrofibrosis & Quadriceps Weakness (Week 6 Post-Op)",
                            ClinicalNotes = "Surgical incisions well healed. Graft stability confirmed. Needs progressive closed-chain strength and ROM exercises.",
                            Recommendations = "Bi-weekly sessions including ultrasound therapy, isometric quadriceps sets, and gentle manual mobilization.",
                            CreatedAt = DateTime.UtcNow.AddDays(-1)
                        }
                    );
                    await context.SaveChangesAsync();
                }
            }

            // Seed TreatmentPlans & TreatmentPlanDetails
            if (!await context.TreatmentPlans.AnyAsync())
            {
                var john = await context.Patients.FirstOrDefaultAsync(p => p.FirstName == "John");
                var jane = await context.Patients.FirstOrDefaultAsync(p => p.FirstName == "Jane");
                var therapistUser = await context.Users.FirstOrDefaultAsync(u => u.Role == Roles.Physiotherapist);
                var johnAssessment = await context.PatientAssessments.FirstOrDefaultAsync(a => a.PatientId == (john != null ? john.Id : 0));
                var janeAssessment = await context.PatientAssessments.FirstOrDefaultAsync(a => a.PatientId == (jane != null ? jane.Id : 0));

                var manualTherapy = await context.TreatmentTypes.FirstOrDefaultAsync(t => t.Name == "Manual Therapy")
                    ?? await context.TreatmentTypes.FirstAsync();
                var ultrasoundTherapy = await context.TreatmentTypes.FirstOrDefaultAsync(t => t.Name == "Ultrasound Therapy")
                    ?? await context.TreatmentTypes.FirstAsync();
                var exerciseSession = await context.TreatmentTypes.FirstOrDefaultAsync(t => t.Name == "Therapeutic Exercise Session")
                    ?? await context.TreatmentTypes.FirstAsync();

                if (john != null && therapistUser != null && johnAssessment != null)
                {
                    var johnPlan = new TreatmentPlan
                    {
                        PatientId = john.Id,
                        PhysiotherapistId = therapistUser.Id,
                        AssessmentId = johnAssessment.Id,
                        StartDate = DateTime.UtcNow.AddDays(-2),
                        ExpectedEndDate = DateTime.UtcNow.AddDays(30),
                        NumberOfSessions = 12,
                        Goal = "Eliminate resting lumbar pain, restore full spinal mobility, and strengthen deep core musculature.",
                        Notes = "Monitor pain before and after sessions. Ergonomic workstation education provided.",
                        Status = "Active",
                        CreatedAt = DateTime.UtcNow.AddDays(-2)
                    };

                    johnPlan.TreatmentPlanDetails.Add(new TreatmentPlanDetail
                    {
                        TreatmentTypeId = manualTherapy.Id,
                        Frequency = "2x per week",
                        DurationMinutes = 45,
                        Instructions = "Lumbar mobilization (grade II-III) and soft tissue trigger point release.",
                        NumberOfSessions = 6,
                        CreatedAt = DateTime.UtcNow.AddDays(-2)
                    });

                    johnPlan.TreatmentPlanDetails.Add(new TreatmentPlanDetail
                    {
                        TreatmentTypeId = exerciseSession.Id,
                        Frequency = "3x per week",
                        DurationMinutes = 45,
                        Instructions = "Supervised pelvic tilts, abdominal bracing, and passive hamstring stretches.",
                        NumberOfSessions = 6,
                        CreatedAt = DateTime.UtcNow.AddDays(-2)
                    });

                    await context.TreatmentPlans.AddAsync(johnPlan);
                }

                if (jane != null && therapistUser != null && janeAssessment != null)
                {
                    var janePlan = new TreatmentPlan
                    {
                        PatientId = jane.Id,
                        PhysiotherapistId = therapistUser.Id,
                        AssessmentId = janeAssessment.Id,
                        StartDate = DateTime.UtcNow.AddDays(-1),
                        ExpectedEndDate = DateTime.UtcNow.AddDays(35),
                        NumberOfSessions = 10,
                        Goal = "Achieve 120-degree knee flexion, restore quad activation, and eliminate joint effusion.",
                        Notes = "Cryotherapy after sessions. Avoid high-impact loads.",
                        Status = "Active",
                        CreatedAt = DateTime.UtcNow.AddDays(-1)
                    };

                    janePlan.TreatmentPlanDetails.Add(new TreatmentPlanDetail
                    {
                        TreatmentTypeId = ultrasoundTherapy.Id,
                        Frequency = "2x per week",
                        DurationMinutes = 30,
                        Instructions = "Low-intensity continuous ultrasound to lateral patellar retinaculum.",
                        NumberOfSessions = 4,
                        CreatedAt = DateTime.UtcNow.AddDays(-1)
                    });

                    janePlan.TreatmentPlanDetails.Add(new TreatmentPlanDetail
                    {
                        TreatmentTypeId = exerciseSession.Id,
                        Frequency = "3x per week",
                        DurationMinutes = 45,
                        Instructions = "Isometric quad sets, terminal knee extensions, and supported heel slides.",
                        NumberOfSessions = 6,
                        CreatedAt = DateTime.UtcNow.AddDays(-1)
                    });

                    await context.TreatmentPlans.AddAsync(janePlan);
                }

                await context.SaveChangesAsync();
            }

            // Seed TreatmentSessions
            if (!await context.TreatmentSessions.AnyAsync())
            {
                var john = await context.Patients.FirstOrDefaultAsync(p => p.FirstName == "John");
                var jane = await context.Patients.FirstOrDefaultAsync(p => p.FirstName == "Jane");
                var therapistUser = await context.Users.FirstOrDefaultAsync(u => u.Role == Roles.Physiotherapist);
                var stdSessionType = await context.AppointmentTypes.FirstOrDefaultAsync(at => at.Name == "Standard Physiotherapy Session")
                    ?? await context.AppointmentTypes.FirstAsync();

                if (john != null && jane != null && therapistUser != null)
                {
                    // Ensure appointments exist for these sessions
                    var johnAppointment = await context.Appointments.FirstOrDefaultAsync(a => a.PatientId == john.Id && a.Status == "Completed");
                    if (johnAppointment == null)
                    {
                        johnAppointment = new Appointment
                        {
                            PatientId = john.Id,
                            PhysiotherapistId = therapistUser.Id,
                            AppointmentTypeId = stdSessionType.Id,
                            AppointmentDate = DateTime.UtcNow.Date.AddDays(-2),
                            StartTime = new TimeSpan(9, 0, 0),
                            EndTime = new TimeSpan(9, 45, 0),
                            Status = "Completed",
                            Reason = "Lower back manual therapy session",
                            Notes = "Initial rehab session completed.",
                            CreatedAt = DateTime.UtcNow.AddDays(-2)
                        };
                        await context.Appointments.AddAsync(johnAppointment);
                        await context.SaveChangesAsync();
                    }

                    var janeAppointment = await context.Appointments.FirstOrDefaultAsync(a => a.PatientId == jane.Id && a.Status == "Completed");
                    if (janeAppointment == null)
                    {
                        janeAppointment = new Appointment
                        {
                            PatientId = jane.Id,
                            PhysiotherapistId = therapistUser.Id,
                            AppointmentTypeId = stdSessionType.Id,
                            AppointmentDate = DateTime.UtcNow.Date.AddDays(-1),
                            StartTime = new TimeSpan(10, 30, 0),
                            EndTime = new TimeSpan(11, 0, 0),
                            Status = "Completed",
                            Reason = "Post-op knee session",
                            Notes = "Ultrasound and mobility exercises completed.",
                            CreatedAt = DateTime.UtcNow.AddDays(-1)
                        };
                        await context.Appointments.AddAsync(janeAppointment);
                        await context.SaveChangesAsync();
                    }

                    var johnPlan = await context.TreatmentPlans.FirstOrDefaultAsync(tp => tp.PatientId == john.Id);
                    var janePlan = await context.TreatmentPlans.FirstOrDefaultAsync(tp => tp.PatientId == jane.Id);

                    await context.TreatmentSessions.AddRangeAsync(
                        new TreatmentSession
                        {
                            AppointmentId = johnAppointment.Id,
                            PatientId = john.Id,
                            PhysiotherapistId = therapistUser.Id,
                            TreatmentPlanId = johnPlan?.Id,
                            SessionDate = johnAppointment.AppointmentDate,
                            StartTime = johnAppointment.StartTime,
                            EndTime = johnAppointment.EndTime,
                            PainLevelBefore = 7,
                            PainLevelAfter = 4,
                            Status = "Completed",
                            Assessment = "Patient reported tight lumbar spine with pain rating 7/10 at start of session.",
                            TreatmentPerformed = "Performed 20 mins lumbar joint mobilization and 25 mins core activation exercises.",
                            Recommendations = "Apply ice pack for 15 mins if soreness occurs; perform home pelvic tilts.",
                            Notes = "Patient responded favorably to mobilization. Pain reduced to 4/10 upon conclusion.",
                            CreatedAt = johnAppointment.CreatedAt
                        },
                        new TreatmentSession
                        {
                            AppointmentId = janeAppointment.Id,
                            PatientId = jane.Id,
                            PhysiotherapistId = therapistUser.Id,
                            TreatmentPlanId = janePlan?.Id,
                            SessionDate = janeAppointment.AppointmentDate,
                            StartTime = janeAppointment.StartTime,
                            EndTime = janeAppointment.EndTime,
                            PainLevelBefore = 5,
                            PainLevelAfter = 3,
                            Status = "Completed",
                            Assessment = "Left knee flexion was 92 degrees upon arrival with mild stiffness.",
                            TreatmentPerformed = "Applied continuous ultrasound therapy for 15 mins followed by assisted heel slides and quad sets.",
                            Recommendations = "Continue home quad sets 3x daily. Elevate knee post-exercise.",
                            Notes = "Flexion improved to 98 degrees by the end of session. Swelling was well controlled.",
                            CreatedAt = janeAppointment.CreatedAt
                        }
                    );

                    await context.SaveChangesAsync();
                }
            }

            // Seed ExercisePrescriptions & ExercisePrescriptionDetails
            if (!await context.ExercisePrescriptions.AnyAsync())
            {
                var john = await context.Patients.FirstOrDefaultAsync(p => p.FirstName == "John");
                var jane = await context.Patients.FirstOrDefaultAsync(p => p.FirstName == "Jane");
                var therapistUser = await context.Users.FirstOrDefaultAsync(u => u.Role == Roles.Physiotherapist);

                var johnPlan = await context.TreatmentPlans.FirstOrDefaultAsync(tp => tp.PatientId == (john != null ? john.Id : 0));
                var janePlan = await context.TreatmentPlans.FirstOrDefaultAsync(tp => tp.PatientId == (jane != null ? jane.Id : 0));

                var pelvicTilt = await context.Exercises.FirstOrDefaultAsync(e => e.Name.Contains("Pelvic Tilt"));
                var hamstringStretch = await context.Exercises.FirstOrDefaultAsync(e => e.Name.Contains("Hamstring"));
                var quadSets = await context.Exercises.FirstOrDefaultAsync(e => e.Name.Contains("Quadriceps"));

                if (john != null && therapistUser != null && johnPlan != null && pelvicTilt != null && hamstringStretch != null)
                {
                    var johnPrescription = new ExercisePrescription
                    {
                        PatientId = john.Id,
                        PhysiotherapistId = therapistUser.Id,
                        TreatmentPlanId = johnPlan.Id,
                        PrescriptionDate = DateTime.UtcNow.AddDays(-2),
                        Instructions = "Perform home lumbar stabilization and hamstring flexibility routine daily. Stop if sharp radiating pain occurs.",
                        Status = "Active",
                        CreatedAt = DateTime.UtcNow.AddDays(-2)
                    };

                    johnPrescription.PrescriptionDetails.Add(new ExercisePrescriptionDetail
                    {
                        ExerciseId = pelvicTilt.Id,
                        Sets = 3,
                        Repetitions = 10,
                        HoldSeconds = 5,
                        FrequencyPerDay = 2,
                        DurationWeeks = 4,
                        Instructions = "Focus on posterior pelvic tilt against firm surface; engage transverse abdominis.",
                        CreatedAt = DateTime.UtcNow.AddDays(-2)
                    });

                    johnPrescription.PrescriptionDetails.Add(new ExercisePrescriptionDetail
                    {
                        ExerciseId = hamstringStretch.Id,
                        Sets = 3,
                        Repetitions = 1,
                        HoldSeconds = 30,
                        FrequencyPerDay = 2,
                        DurationWeeks = 4,
                        Instructions = "Gentle stretch behind thigh, no bouncing. Keep lumbar spine neutral.",
                        CreatedAt = DateTime.UtcNow.AddDays(-2)
                    });

                    await context.ExercisePrescriptions.AddAsync(johnPrescription);
                }

                if (jane != null && therapistUser != null && janePlan != null && quadSets != null)
                {
                    var janePrescription = new ExercisePrescription
                    {
                        PatientId = jane.Id,
                        PhysiotherapistId = therapistUser.Id,
                        TreatmentPlanId = janePlan.Id,
                        PrescriptionDate = DateTime.UtcNow.AddDays(-1),
                        Instructions = "Follow progressive isometric knee strengthening. Apply cold pack for 15 minutes post-exercise.",
                        Status = "Active",
                        CreatedAt = DateTime.UtcNow.AddDays(-1)
                    };

                    janePrescription.PrescriptionDetails.Add(new ExercisePrescriptionDetail
                    {
                        ExerciseId = quadSets.Id,
                        Sets = 3,
                        Repetitions = 15,
                        HoldSeconds = 5,
                        FrequencyPerDay = 3,
                        DurationWeeks = 6,
                        Instructions = "Press popliteal fossa firmly downward against mat, maintain patellar glide.",
                        CreatedAt = DateTime.UtcNow.AddDays(-1)
                    });

                    await context.ExercisePrescriptions.AddAsync(janePrescription);
                }

                await context.SaveChangesAsync();
            }

            // Seed AuditLogs
            if (!await context.AuditLogs.AnyAsync())
            {
                var therapistUser = await context.Users.FirstOrDefaultAsync(u => u.Role == Roles.Physiotherapist);
                var adminUser = await context.Users.FirstOrDefaultAsync(u => u.Role == Roles.SuperAdmin);

                await context.AuditLogs.AddRangeAsync(
                    new AuditLog
                    {
                        UserId = adminUser?.Id,
                        Action = "SYSTEM_INITIALIZE",
                        EntityName = "System",
                        EntityId = "1",
                        NewValue = "Initial clinic master data, categories, and exercise masters seeded successfully.",
                        IPAddress = "127.0.0.1",
                        CreatedAt = DateTime.UtcNow.AddDays(-3)
                    },
                    new AuditLog
                    {
                        UserId = therapistUser?.Id,
                        Action = "CREATE_ASSESSMENT",
                        EntityName = nameof(PatientAssessment),
                        EntityId = "1",
                        NewValue = "Assessment created for Patient John Doe: Diagnosis: 'Chronic Mechanical Lower Back Strain', PainLevel: 7",
                        IPAddress = "127.0.0.1",
                        CreatedAt = DateTime.UtcNow.AddDays(-2)
                    },
                    new AuditLog
                    {
                        UserId = therapistUser?.Id,
                        Action = "CREATE_TREATMENT_PLAN",
                        EntityName = nameof(TreatmentPlan),
                        EntityId = "1",
                        NewValue = "Plan created for Patient John Doe, 12 sessions, Status: 'Active'",
                        IPAddress = "127.0.0.1",
                        CreatedAt = DateTime.UtcNow.AddDays(-2)
                    },
                    new AuditLog
                    {
                        UserId = therapistUser?.Id,
                        Action = "COMPLETE_TREATMENT_SESSION",
                        EntityName = nameof(TreatmentSession),
                        EntityId = "1",
                        NewValue = "Session 1 completed for John Doe. PainBefore: 7, PainAfter: 4",
                        IPAddress = "127.0.0.1",
                        CreatedAt = DateTime.UtcNow.AddDays(-2)
                    }
                );
                await context.SaveChangesAsync();
            }

            // Seed ClinicSettings
            if (!await context.ClinicSettings.AnyAsync())
            {
                await context.ClinicSettings.AddAsync(new ClinicSettings
                {
                    ClinicName = "Glory Florence Physiotherapy & Rehab Clinic",
                    Tagline = "Excellence in Physical Rehabilitation and Patient Care",
                    Email = "contact@gloryflorence.com",
                    Phone = "+1 (555) 987-6543",
                    Address = "789 Health & Wellness Avenue",
                    City = "Mumbai",
                    State = "Maharashtra",
                    Country = "India",
                    PostalCode = "400001",
                    TaxRegistrationNumber = "GSTIN27AABCU9603R1ZM",
                    CurrencySymbol = "₹",
                    WorkingHoursStart = new TimeSpan(8, 0, 0),
                    WorkingHoursEnd = new TimeSpan(19, 0, 0),
                    AppointmentSlotDurationMinutes = 30,
                    AutoConfirmAppointments = true,
                    EnableSmsNotifications = true,
                    EnableEmailNotifications = true,
                    UpdatedAt = DateTime.UtcNow
                });
                await context.SaveChangesAsync();
            }

            // Seed Invoices
            if (!await context.Invoices.AnyAsync())
            {
                var john = await context.Patients.FirstOrDefaultAsync(p => p.FirstName == "John");
                var jane = await context.Patients.FirstOrDefaultAsync(p => p.FirstName == "Jane");

                if (john != null && jane != null)
                {
                    var johnInvoice = new Invoice
                    {
                        InvoiceNumber = "INV-20260901-1001",
                        PatientId = john.Id,
                        Amount = 125.00m,
                        PaidAmount = 125.00m,
                        BalanceAmount = 0.00m,
                        Status = "Paid",
                        InvoiceDate = DateTime.UtcNow.AddDays(-5),
                        DueDate = DateTime.UtcNow.AddDays(10),
                        Notes = "Payment received in full via Credit Card",
                        CreatedAt = DateTime.UtcNow.AddDays(-5)
                    };
                    johnInvoice.InvoiceItems.Add(new InvoiceItem { Description = "Initial Physical Assessment", Quantity = 1, UnitPrice = 75.00m, TotalAmount = 75.00m });
                    johnInvoice.InvoiceItems.Add(new InvoiceItem { Description = "Manual Therapy Session", Quantity = 1, UnitPrice = 50.00m, TotalAmount = 50.00m });
                    johnInvoice.Payments.Add(new Payment { AmountPaid = 125.00m, PaymentMethod = "Credit Card", TransactionReference = "TXN-88493021", PaymentDate = DateTime.UtcNow.AddDays(-4), Notes = "Paid at reception" });

                    var janeInvoice = new Invoice
                    {
                        InvoiceNumber = "INV-20260902-1002",
                        PatientId = jane.Id,
                        Amount = 90.00m,
                        PaidAmount = 0.00m,
                        BalanceAmount = 90.00m,
                        Status = "Unpaid",
                        InvoiceDate = DateTime.UtcNow.AddDays(-2),
                        DueDate = DateTime.UtcNow.AddDays(12),
                        Notes = "Pending insurance claim processing",
                        CreatedAt = DateTime.UtcNow.AddDays(-2)
                    };
                    janeInvoice.InvoiceItems.Add(new InvoiceItem { Description = "Post-Op Knee Rehabilitation Session", Quantity = 2, UnitPrice = 45.00m, TotalAmount = 90.00m });

                    await context.Invoices.AddRangeAsync(johnInvoice, janeInvoice);
                    await context.SaveChangesAsync();
                }
            }
        }

        public static async Task SeedRolesAsync(ApplicationDbContext context)
        {
            if (!await context.Roles.AnyAsync())
            {
                foreach (var roleName in Roles.All)
                {
                    await context.Roles.AddAsync(new Role
                    {
                        Name = roleName,
                        Description = $"{roleName} role for the physiotherapy management system",
                        CreatedAt = DateTime.UtcNow
                    });
                }
                await context.SaveChangesAsync();
            }
        }

        public static async Task SeedUsersAsync(ApplicationDbContext context)
        {
            // Seed Roles first as Users depend on Roles
            await SeedRolesAsync(context);

            var roleUserSpecs = new[]
            {
                new { Username = "admin", Email = "admin@gloryflorence.com", Password = "Admin123!", Role = Roles.SuperAdmin, FirstName = "System", LastName = "Administrator" },
                new { Username = "clinicadmin", Email = "clinicadmin@gloryflorence.com", Password = "Admin123!", Role = Roles.Admin, FirstName = "Clinic", LastName = "Admin" },
                new { Username = "therapist", Email = "therapist@gloryflorence.com", Password = "Therapist123!", Role = Roles.Physiotherapist, FirstName = "John", LastName = "Therapist" },
                new { Username = "doctor", Email = "doctor@gloryflorence.com", Password = "Doctor123!", Role = Roles.Doctor, FirstName = "Sarah", LastName = "Doctor" },
                new { Username = "receptionist", Email = "receptionist@gloryflorence.com", Password = "Receptionist123!", Role = Roles.Receptionist, FirstName = "Mary", LastName = "Receptionist" },
                new { Username = "accountant", Email = "accountant@gloryflorence.com", Password = "Accountant123!", Role = Roles.Accountant, FirstName = "David", LastName = "Accountant" },
                new { Username = "patientuser", Email = "patientuser@gloryflorence.com", Password = "Patient123!", Role = Roles.Patient, FirstName = "Robert", LastName = "Patient" }
            };

            foreach (var spec in roleUserSpecs)
            {
                var existingUser = await context.Users.FirstOrDefaultAsync(u => u.Username == spec.Username || u.Email == spec.Email);
                if (existingUser != null)
                {
                    continue;
                }

                var role = await context.Roles.FirstOrDefaultAsync(r => r.Name == spec.Role);
                if (role != null)
                {
                    var user = new User
                    {
                        Username = spec.Username,
                        Email = spec.Email,
                        PasswordHash = BCrypt.Net.BCrypt.HashPassword(spec.Password),
                        Role = spec.Role,
                        IsActive = true,
                        FirstName = spec.FirstName,
                        LastName = spec.LastName,
                        CreatedAt = DateTime.UtcNow
                    };

                    await context.Users.AddAsync(user);
                    await context.SaveChangesAsync();

                    var userRole = new UserRole
                    {
                        UserId = user.Id,
                        RoleId = role.Id,
                        CreatedAt = DateTime.UtcNow
                    };
                    await context.UserRoles.AddAsync(userRole);

                    var userProfile = new UserProfile
                    {
                        UserId = user.Id,
                        PhoneNumber = "1234567890",
                        Bio = $"Default {spec.Role} Account",
                        CreatedAt = DateTime.UtcNow
                    };
                    await context.UserProfiles.AddAsync(userProfile);
                    await context.SaveChangesAsync();
                }
            }
        }

        public static async Task SeedUsersOnlyAsync(ApplicationDbContext context)
        {
            // Auto-apply migrations if pending
            if ((await context.Database.GetPendingMigrationsAsync()).Any())
            {
                await context.Database.MigrateAsync();
            }

            await SeedUsersAsync(context);
        }
    }
}
