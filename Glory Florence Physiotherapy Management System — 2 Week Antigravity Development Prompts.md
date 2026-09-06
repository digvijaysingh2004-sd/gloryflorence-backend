# Glory Florence Physiotherapy Management System

## React + .NET 8 Web API — 10-Day Development Plan

### Technology

**Backend**

- .NET 8 Web API
- Entity Framework Core
- SQL Server
- JWT Authentication
- ASP.NET Core Identity/custom authentication as appropriate
- Clean/Onion Architecture
- REST APIs
- FluentValidation or equivalent validation approach
- Swagger/OpenAPI

**Frontend**

- React
- TypeScript
- Vite
- React Router
- Axios
- TanStack Query/React Query if appropriate
- Reusable components
- Responsive UI
- Role-based navigation and authorization

### Database

The project contains these 31 tables:

1. Users
2. Roles
3. UserRoles
4. Countries
5. States
6. Cities
7. Addresses
8. UserProfiles
9. Patients
10. Specializations
11. Categories
12. TreatmentTypes
13. Exercises
14. PatientMedicalHistory
15. PatientDocuments
16. AppointmentTypes
17. Appointments
18. TreatmentSessions
19. PatientAssessments
20. TreatmentPlans
21. TreatmentPlanDetails
22. ExercisePrescriptions
23. ExercisePrescriptionDetails
24. Invoices
25. InvoiceDetail
26. Payments
27. PaymentMethods
28. Genders
29. BloodGroups
30. Statuses
31. AuditLogs

## The document defines the main business flow as patient management, appointments, clinical assessment, treatment plans, exercise prescriptions, billing/payment and auditing.

# DAY 1 — Project Foundation + Architecture

## Backend Prompt

```text
You are working on the Glory Florence Physiotherapy Management System.

Create the backend using .NET 8 Web API with a clean Onion Architecture.

Before writing code:
1. Inspect the existing solution and all existing files.
2. Do not delete or overwrite existing working code unnecessarily.
3. If a project already exists, adapt the current architecture instead of creating duplicate projects.
4. Follow the database design provided for this project.

Create a production-ready structure similar to:

GloryFlorence.sln

src/
  GloryFlorence.API
  GloryFlorence.Application
  GloryFlorence.Domain
  GloryFlorence.Infrastructure

tests/
  GloryFlorence.UnitTests
  GloryFlorence.IntegrationTests

Architecture rules:
- Domain must not depend on Infrastructure.
- Application must contain business logic, DTOs, interfaces and validation.
- Infrastructure must contain EF Core, database configuration, repositories and external implementations.
- API must contain controllers, middleware, authentication configuration and API configuration.
- Use dependency injection.
- Use async/await.
- Use cancellation tokens where appropriate.
- Use DTOs instead of exposing EF entities directly.
- Use proper exception handling middleware.
- Use consistent API response/error format.
- Enable Swagger.
- Configure CORS for React frontend.
- Configure appsettings for SQL Server connection string.
- Configure development/production environments.

Create the initial DbContext and entity configuration structure.

Do NOT implement all business modules today.

Today only establish the complete foundation and make sure the solution builds successfully.

At the end:
- Run build.
- Fix compilation errors.
- Verify Swagger opens.
- Verify the API starts successfully.
- Provide a summary of created files and architecture.
```

## Frontend Prompt

```text
Create the frontend for the Glory Florence Physiotherapy Management System using React + TypeScript + Vite.

Before coding:
1. Inspect the existing frontend project.
2. Do not delete working code.
3. Reuse existing structure if present.
4. Create a scalable enterprise-level frontend architecture.

Create:

src/
  components/
  layouts/
  pages/
  features/
  services/
  hooks/
  types/
  utils/
  routes/
  constants/
  context/
  assets/

Configure:
- React Router
- Axios API client
- Environment configuration
- Global error handling
- Reusable Button/Input/Modal/Table/Card components
- Responsive layout
- Sidebar
- Header
- Main content area
- Loading states
- Empty states
- Error states
- Toast/notification system

Create the basic application shell only.

Do NOT implement every module yet.

Create:
- Login page placeholder
- Dashboard placeholder
- Main application layout
- Sidebar navigation structure
- Protected route structure
- 404 page

Use TypeScript strictly.

Make the UI professional and suitable for a physiotherapy management/medical administration system.

At the end run the frontend and fix all TypeScript/build errors.
```

---

# DAY 2 — Database + Authentication + User/Roles

## Backend Prompt

```text
Continue the Glory Florence Physiotherapy Management System backend.

Inspect everything created in Day 1 before making changes.

Implement the following database entities:

Users
Roles
UserRoles
UserProfiles

Implement:
- EF Core entities
- Fluent configurations
- Relationships
- Primary keys
- Foreign keys
- Unique constraints
- Indexes
- DTOs
- Repository/service interfaces where appropriate
- Application services
- Controllers
- Validation

Implement authentication:
- Login
- JWT access token
- Password hashing
- User validation
- Active/inactive user check
- Role claims
- Current user service

Implement authorization:
- Role-based authorization
- [Authorize]
- Role policies where appropriate

Create:
POST /api/auth/login
GET /api/auth/me

Create user/role APIs as appropriate.

Never return PasswordHash in API responses.

Create EF Core migration and update the SQL Server database.

Seed initial roles:
- Super Admin
- Admin
- Physiotherapist
- Doctor
- Receptionist
- Accountant
- Patient

Create a development admin account through a safe seed mechanism.

Run migration, build and test the APIs.

Do not implement patient/appointment/billing modules yet.
```

## Frontend Prompt

```text
Continue the React frontend.

Inspect the existing frontend before coding.

Implement complete authentication UI:

- Login page
- Login form validation
- API integration with POST /api/auth/login
- Store authentication state securely according to the project architecture
- Axios authentication interceptor
- Current user loading
- Logout
- Protected routes
- Role-based route protection

Create:
- AuthContext or equivalent auth state solution
- useAuth hook
- ProtectedRoute
- RoleProtectedRoute

Implement role-based sidebar navigation for:

Super Admin
Admin
Physiotherapist
Doctor
Receptionist
Accountant
Patient

Create a professional login page.

After login:
- Redirect to dashboard.
- Load current user.
- Display logged-in user name/role.
- Handle expired/invalid token.
- Redirect unauthorized users to an access-denied page.

Do not create fake API responses once backend APIs are available.
Use the actual backend API.
```

---

# DAY 3 — Master Data + Patient Management

## Backend Prompt

```text
Continue the project from Day 2.

Implement the following master/core tables:

Countries
States
Cities
Addresses
Genders
BloodGroups
Specializations
Categories

Implement complete CRUD APIs with:
- Pagination
- Search
- Sorting
- Filtering
- Active/inactive status
- Validation
- Proper HTTP status codes
- DTOs

Implement patient management:

Patients
UserProfiles
PatientMedicalHistory
PatientDocuments

Patient requirements:
- Create patient
- Edit patient
- View patient
- Search patient
- Pagination
- Activate/deactivate patient
- Patient registration number
- Personal details
- Contact details
- Address
- Emergency contact
- Blood group
- Gender
- Date of birth
- Medical history
- Documents

Patient account/login must remain optional as specified in the database design.

Create REST APIs for:
- Patient list
- Patient details
- Create
- Update
- Activate/deactivate
- Medical history CRUD
- Patient documents metadata CRUD

Implement proper relationship handling.

Do not expose EF entities directly.

Add validation and audit logging hooks where appropriate.

Create/update migrations.

Build and test all APIs.
```

## Frontend Prompt

```text
Continue the React frontend from Day 2.

Implement the Master Data and Patient Management modules.

Create pages:

/patients
/patients/create
/patients/:id
/patients/:id/edit

Patient list must contain:
- Search
- Pagination
- Status filter
- View
- Edit
- Activate/deactivate
- Add patient

Create a professional patient form with sections:

Personal Information
- Patient Number
- Name
- Gender
- Date of Birth
- Blood Group
- Phone
- Email

Address
- Country
- State
- City
- PIN Code
- Address lines

Emergency Contact
- Name
- Phone

Medical Information
- Medical history

Create patient details page with tabs/sections:
- Overview
- Medical History
- Documents
- Appointments
- Treatment Plans
- Billing

Use actual backend APIs.

Create reusable:
- DataTable
- SearchBar
- Pagination
- FormField
- SelectField
- Modal
- ConfirmationDialog

Handle loading, validation, empty and error states properly.
```

---

# DAY 4 — Treatment & Exercise Master + Appointments

## Backend Prompt

```text
Continue from the existing implementation.

Implement:

TreatmentTypes
Exercises
AppointmentTypes
Statuses

TreatmentTypes:
- CategoryId
- Name
- Description
- DefaultDurationMinutes
- DefaultPrice
- IsActive

Exercises:
- CategoryId
- Name
- Description
- Instructions
- VideoUrl
- ImageUrl
- IsActive

AppointmentTypes:
- Name
- DurationMinutes
- IsActive

Statuses:
Create the generic status master using Type, Code, Name and Description.

Seed appropriate statuses for:
- Appointment
- TreatmentPlan
- Invoice
- Payment

Implement full CRUD APIs for TreatmentTypes, Exercises and AppointmentTypes.

Then implement Appointments.

Appointment fields:
- Patient
- Physiotherapist
- Appointment date
- Start time
- End time
- Appointment type
- Status
- Reason
- Notes

Implement:
- Create appointment
- Update appointment
- Cancel appointment
- Reschedule appointment
- Appointment details
- Appointment list
- Date filtering
- Patient filtering
- Physiotherapist filtering
- Status filtering
- Pagination

Prevent obvious scheduling conflicts for the same physiotherapist.

Use proper validation.

Test all APIs.
```

## Frontend Prompt

```text
Continue the frontend.

Implement:

1. Treatment Types management
2. Exercise library
3. Appointment Types
4. Appointment Management

Create pages:

/treatment-types
/exercises
/appointment-types
/appointments
/appointments/create
/appointments/:id

Appointment UI should support:
- Calendar/date-based view
- List view
- Create appointment
- Edit/reschedule
- Cancel
- View details
- Patient selection
- Physiotherapist selection
- Appointment type
- Date/time
- Status
- Notes

Prevent invalid form submissions.

Create reusable appointment components.

Create exercise library UI with:
- Search
- Category filter
- Add/edit/delete
- Exercise details
- Video/image URL
- Active/inactive

Use actual APIs.
Do not use hardcoded mock data except where explicitly required for static UI.
```

---

# DAY 5 — Clinical Assessment + Treatment Plans

## Backend Prompt

```text
Continue the backend.

Implement:

PatientAssessments
TreatmentPlans
TreatmentPlanDetails
TreatmentSessions

PatientAssessment:
- PatientId
- SessionId nullable
- PhysiotherapistId
- AssessmentDate
- ChiefComplaint
- CurrentCondition
- PainLevel
- Diagnosis
- ClinicalNotes
- Recommendations

TreatmentPlan:
- PatientId
- PhysiotherapistId
- AssessmentId
- StartDate
- ExpectedEndDate
- NumberOfSessions
- Goal
- Notes
- Status

TreatmentPlanDetails:
- TreatmentPlanId
- TreatmentTypeId
- Frequency
- DurationMinutes
- Instructions
- NumberOfSessions

TreatmentSession:
- AppointmentId
- PatientId
- PhysiotherapistId
- TreatmentPlanId nullable
- SessionDate
- StartTime
- EndTime
- PainLevelBefore
- PainLevelAfter
- Notes
- Status

Implement complete APIs.

Business flow must be:

Patient
→ Assessment
→ Treatment Plan
→ Treatment Plan Details
→ Appointment
→ Treatment Session

Validate relationships.

A treatment plan must belong to the correct patient.

A session must not be created for a different patient than its appointment.

Implement proper status transitions.

Add audit logging for important clinical changes.

Build and test.
```

## Frontend Prompt

```text
Continue the frontend.

Implement the complete clinical workflow.

Patient Details must now include:

Assessment
Treatment Plans
Treatment Sessions

Create:

/patients/:patientId/assessments
/patients/:patientId/assessments/create
/patients/:patientId/treatment-plans
/patients/:patientId/treatment-plans/create
/treatment-sessions

Assessment form:
- Chief complaint
- Current condition
- Pain level
- Diagnosis
- Clinical notes
- Recommendations

Treatment Plan form:
- Assessment
- Start date
- Expected end date
- Number of sessions
- Goal
- Notes
- Status
- Multiple treatment plan details

Treatment plan details should allow selecting multiple TreatmentTypes and configuring:
- Frequency
- Duration
- Instructions
- Number of sessions

Treatment Session:
- Patient
- Appointment
- Treatment plan
- Date/time
- Pain before
- Pain after
- Notes
- Status

Make the clinical workflow intuitive for physiotherapists.

Do not expose unnecessary billing/admin controls to physiotherapists.

Use role-based UI permissions.
```

---

# DAY 6 — Exercise Prescription

## Backend Prompt

```text
Continue the backend.

Implement:

ExercisePrescriptions
ExercisePrescriptionDetails

Business relationship:

Patient
→ TreatmentPlan
→ ExercisePrescription
→ ExercisePrescriptionDetails
→ Exercise

ExercisePrescription:
- PatientId
- PhysiotherapistId
- TreatmentPlanId
- PrescriptionDate
- Instructions
- CreatedAt
- UpdatedAt

ExercisePrescriptionDetails:
- PrescriptionId
- ExerciseId
- Sets
- Repetitions
- HoldSeconds
- FrequencyPerDay
- DurationWeeks
- Instructions

Implement:
- Create prescription
- Add multiple exercises
- Edit prescription
- Remove exercise
- View prescription
- Patient prescription history
- Filter by patient/treatment plan

Validate that:
- Patient exists.
- Treatment plan belongs to patient.
- Physiotherapist is valid.
- Exercise exists.
- Duplicate exercises are handled correctly.

Create APIs for complete CRUD operations.

Return nested DTOs where appropriate so the frontend can display a complete prescription.

Add audit logging for prescription changes.
```

## Frontend Prompt

```text
Continue the frontend.

Implement Exercise Prescription management.

Create prescription UI inside Patient/Treatment Plan details.

The physiotherapist should be able to:

1. Select treatment plan.
2. Create exercise prescription.
3. Search exercise library.
4. Add multiple exercises.
5. Configure:
   - Sets
   - Repetitions
   - Hold seconds
   - Frequency per day
   - Duration in weeks
   - Instructions
6. Remove exercises.
7. Edit prescription.
8. View prescription history.

Create a professional exercise prescription screen.

Show each exercise as a clean card/row.

Add:
- Exercise search
- Category filter
- Exercise information
- Prescription preview

Use real APIs.

Make the UI responsive and easy for a physiotherapist to use.
```

---

# DAY 7 — Billing + Invoice + Payments

## Backend Prompt

```text
Continue the backend.

Implement:

Invoices
InvoiceDetails
Payments
PaymentMethods

Invoice:
- InvoiceNumber
- PatientId
- AppointmentId nullable
- InvoiceDate
- SubTotal
- DiscountAmount
- TaxAmount
- TotalAmount
- PaidAmount
- BalanceAmount
- Status

InvoiceDetails:
- InvoiceId
- TreatmentTypeId nullable
- Description
- Quantity
- UnitPrice
- Discount
- Tax
- TotalAmount

Payment:
- InvoiceId
- PatientId
- PaymentDate
- Amount
- PaymentMethodId
- TransactionReference
- Status
- Notes

PaymentMethods:
- Cash
- UPI
- Credit Card
- Debit Card
- Bank Transfer
- Insurance

Implement:

- Create invoice
- Add invoice details
- Calculate subtotal
- Calculate discount
- Calculate tax
- Calculate total
- Record payment
- Support partial payment
- Calculate paid amount
- Calculate balance
- Automatically determine invoice status
- Payment history
- Invoice details
- Invoice list
- Search/filter/pagination

Use database transactions for invoice/payment operations.

Do not trust calculated totals sent by frontend. Recalculate important monetary values on the server.

Implement validation and audit logging.

Test partial payment and full payment scenarios.
```

## Frontend Prompt

```text
Continue the frontend.

Implement Billing module.

Create:

/invoices
/invoices/create
/invoices/:id
/invoices/:id/payment
/payments

Invoice list:
- Invoice number
- Patient
- Date
- Total
- Paid
- Balance
- Status
- View
- Add payment

Invoice creation:
- Patient
- Appointment
- Invoice date
- Multiple invoice items
- Treatment type
- Description
- Quantity
- Unit price
- Discount
- Tax
- Total

Create payment modal/page:
- Amount
- Payment method
- Transaction reference
- Notes

Support:
- Full payment
- Partial payment
- Remaining balance

Display clear badges for:
- Paid
- Unpaid
- Partially Paid

Use backend-calculated totals.

Do not calculate authoritative billing values only on the frontend.

Create printable professional invoice view.
```

---

# DAY 8 — Dashboard + Documents + Audit

## Backend Prompt

```text
Continue the backend.

Implement system-level features.

PatientDocuments:
- Upload document metadata
- File name
- Storage key
- File size
- Content type
- Uploaded by
- Uploaded at

Create document upload/download/delete APIs.

For local development use a configurable local storage provider.
Keep the implementation abstract enough to support Azure Blob/S3 later.

Implement AuditLogs:
- UserId
- Action
- EntityName
- EntityId
- OldValue
- NewValue
- IPAddress
- CreatedAt

Create reusable audit logging service.

Track important actions:
- Login
- Patient creation/update
- Appointment changes
- Assessment changes
- Treatment plan changes
- Prescription changes
- Invoice changes
- Payment changes
- User/role changes

Create dashboard APIs.

Dashboard should provide relevant statistics such as:
- Total patients
- Today's appointments
- Upcoming appointments
- Completed sessions
- Active treatment plans
- Pending/unpaid invoices
- Revenue/payment summary

Apply role-based dashboard data where necessary.

Test everything.
```

## Frontend Prompt

```text
Continue the frontend.

Implement professional Dashboard.

Dashboard should contain cards/widgets for:
- Total Patients
- Today's Appointments
- Upcoming Appointments
- Active Treatment Plans
- Today's Sessions
- Pending Payments
- Revenue

Create quick actions:
- Add Patient
- New Appointment
- New Assessment
- New Treatment Plan
- Create Invoice

Implement patient documents:
- Upload
- View metadata
- Download
- Delete
- File type/size display

Implement Audit Logs page for authorized administrators.

Audit log UI:
- User
- Action
- Entity
- Date/time
- IP
- Details
- Old/new values where appropriate

Dashboard must use actual backend APIs.

Make the dashboard responsive and professional.
```

---

# DAY 9 — Integration + Authorization + UX Polish

## Backend Prompt

```text
Perform a complete backend integration and quality pass.

Do not create new unnecessary modules.

Inspect the entire solution.

Verify all 31 database entities and their relationships.

Verify:

Authentication
Authorization
Users
Roles
UserRoles
Countries
States
Cities
Addresses
UserProfiles
Patients
Specializations
Categories
TreatmentTypes
Exercises
PatientMedicalHistory
PatientDocuments
AppointmentTypes
Appointments
TreatmentSessions
PatientAssessments
TreatmentPlans
TreatmentPlanDetails
ExercisePrescriptions
ExercisePrescriptionDetails
Invoices
InvoiceDetails
Payments
PaymentMethods
Genders
BloodGroups
Statuses
AuditLogs

Check every controller/service/repository/DTO/validator.

Implement/fix:
- Pagination
- Search
- Filtering
- Sorting
- Validation
- Authorization
- Error handling
- Logging
- Audit logging
- Database transactions
- Concurrency where needed
- Null handling
- Foreign key validation

Verify role permissions.

For example:
- Admin/Super Admin: full management
- Physiotherapist: patients, appointments, assessments, treatments, prescriptions
- Receptionist: patients, appointments
- Accountant: billing/payments
- Patient: own information and permitted records

Do not expose unauthorized data.

Run:
- Build
- Unit tests
- Integration tests where available
- EF migration verification
- Swagger endpoint verification

Fix all errors before finishing.
```

## Frontend Prompt

```text
Perform a complete frontend integration and quality pass.

Inspect the complete application.

Verify all routes and modules.

Remove:
- Broken links
- Duplicate components
- Unused imports
- Temporary mock data
- Console errors
- Hardcoded API URLs
- Placeholder functionality

Verify:
- Authentication
- Role-based navigation
- Protected routes
- API error handling
- Loading states
- Empty states
- Form validation
- Pagination
- Search
- Filtering
- Modal behavior
- Confirmation dialogs
- Toast notifications
- Responsive layout

Check all major workflows:

Login
→ Dashboard
→ Patient
→ Appointment
→ Assessment
→ Treatment Plan
→ Treatment Session
→ Exercise Prescription
→ Invoice
→ Payment

Ensure IDs and relationships are correctly passed between pages.

Improve UI consistency across the entire application.

Fix all TypeScript/build/runtime errors.

Do not rewrite working functionality unnecessarily.
```

---

# DAY 10 — Final Testing + Production Readiness

## Backend Prompt

```text
This is the final production-readiness pass.

Treat the Glory Florence Physiotherapy Management System as a real production application.

Inspect the complete backend.

Perform:

1. Build verification
2. Database migration verification
3. API endpoint verification
4. Authentication testing
5. Authorization testing
6. Validation testing
7. Error handling testing
8. Audit logging verification
9. Transaction verification
10. Security review
11. Performance review
12. Swagger review

Check for:
- SQL injection risks
- Missing authorization
- Sensitive data exposure
- Password exposure
- Missing validation
- Incorrect foreign key handling
- N+1 queries where obvious
- Missing indexes on important foreign keys/search fields
- Incorrect async code
- Unhandled exceptions
- Incorrect HTTP status codes

Make sure PasswordHash is never returned.

Verify database relationships.

Create/update:
- README
- Database setup instructions
- Migration instructions
- Environment configuration documentation
- API documentation
- Default development account instructions

Do not make destructive database changes.

At the end:
- Build successfully.
- Run all tests.
- Fix all errors.
- Report remaining warnings separately.
```

## Frontend Prompt

```text
This is the final production-readiness pass for the React frontend.

Inspect the complete application.

Perform a complete QA pass.

Test:

Authentication
Dashboard
Patients
Medical History
Documents
Appointments
Assessments
Treatment Plans
Treatment Sessions
Exercise Prescriptions
Invoices
Payments
Master Data
Audit Logs

Verify:
- Every page loads correctly.
- Every API request handles loading/error/success states.
- Every form validates correctly.
- Every protected page checks authorization.
- Unauthorized users cannot access restricted pages.
- Search works.
- Pagination works.
- Filters work.
- Create/edit/delete flows work.
- Confirmation dialogs work.
- Toast notifications work.
- Responsive design works.

Check desktop and mobile layouts.

Remove:
- Mock data
- Debug console logs
- Dead code
- Unused dependencies
- Broken routes
- Temporary placeholders

Optimize:
- API calls
- Component rendering
- Query caching where appropriate
- Bundle/import usage

Make sure the final application looks like a professional enterprise physiotherapy management system.

Run production build.

Fix every build and TypeScript error.

Finally provide:
1. Completed modules
2. Remaining issues
3. Environment variables required
4. Backend URL configuration
5. Database configuration
6. Commands to run backend
7. Commands to run frontend
```

---

# Final Business Flow

After completing all 10 days, the application should support this complete workflow:

```text
LOGIN
  ↓
DASHBOARD
  ↓
PATIENT MANAGEMENT
  ↓
CREATE PATIENT
  ↓
APPOINTMENT
  ↓
PATIENT ASSESSMENT
  ↓
TREATMENT PLAN
  ↓
TREATMENT PLAN DETAILS
  ↓
TREATMENT SESSION
  ↓
EXERCISE PRESCRIPTION
  ↓
INVOICE
  ↓
PAYMENT
  ↓
AUDIT LOG
```

The clinical portion follows the database design's intended relationship of patient → treatment plan → exercise prescription → prescription details → exercise.

## How you should use these prompts

**Day 1:** Backend prompt → wait for completion → Frontend prompt\
**Day 2:** Backend prompt → wait → Frontend prompt\
**Day 3:** Backend prompt → wait → Frontend prompt\
...\
**Day 10:** Final backend → final frontend.

### Very important Antigravity rule

Har prompt ke beginning mein agent ko **existing code inspect karne** ko bolo. Aur har day ke end mein:

```text
Do not proceed to the next module.
Build and verify the current implementation first.
Do not delete existing working functionality.
Do not create duplicate entities, services, controllers or components.
Use the existing architecture and naming conventions.
```

Isse Antigravity har baar project ko dobara se rewrite karne ke bajaye **incrementally build** karega.

**Result:** 2 weeks ke end mein tumhare paas 31-table database ke around structured **.NET 8 Web API + React frontend**, authentication, RBAC, patient management, appointments, clinical workflow, exercises, billing/payments, documents, dashboard aur audit logging ka complete MVP/production-oriented foundation hoga.
