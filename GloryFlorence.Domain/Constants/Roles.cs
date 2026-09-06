namespace GloryFlorence.Domain.Constants
{
    public static class Roles
    {
        public const string SuperAdmin = "Super Admin";
        public const string Admin = "Admin";
        public const string Physiotherapist = "Physiotherapist";
        public const string Doctor = "Doctor";
        public const string Receptionist = "Receptionist";
        public const string Accountant = "Accountant";
        public const string Patient = "Patient";

        public static readonly string[] All = new[]
        {
            SuperAdmin,
            Admin,
            Physiotherapist,
            Doctor,
            Receptionist,
            Accountant,
            Patient
        };
    }
}
