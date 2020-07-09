using System;

namespace RequiredNullableDemo.Models
{
    public class EmployeeRequest
    {
        public int! DepartmentId { get; set; }

        public string! FirstName { get; set; }

        public string! LastName { get; set; }

        public DateTime! DateOfBirth { get; set; }

        public DateTime? DateOfDeath { get; set; }
    }
}
