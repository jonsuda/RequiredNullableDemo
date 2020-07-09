using System;

namespace RequiredNullableDemo.Models
{
    public class EmployeeRequest
    {
        public RequiredValue<int> DepartmentId { get; set; }

        public Required<string> FirstName { get; set; }

        public Required<string> LastName { get; set; }

        public RequiredValue<DateTime> DateOfBirth { get; set; }

        public DateTime? DateOfDeath { get; set; }
    }
}
