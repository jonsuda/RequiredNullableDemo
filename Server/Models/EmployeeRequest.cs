using System;

namespace RequiredNullableDemo.Models
{
    public class EmployeeRequest
    {
        public int DepartmentId { get; set; }

        public string FirstName { get; set; } = null!;

        public string LastName { get; set; } = null!;

        public DateTime DateOfBirth { get; set; }

        public DateTime? DateOfDeath { get; set; }
    }
}
