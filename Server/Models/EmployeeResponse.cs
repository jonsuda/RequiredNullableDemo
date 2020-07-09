using System;
using RequiredNullableDemo.Data;

namespace RequiredNullableDemo.Models
{
    public class EmployeeResponse
    {
        public static implicit operator EmployeeResponse(Employee employee) =>
            new EmployeeResponse(employee);

        private EmployeeResponse(Employee employee)
        {
            this.Id = employee.Id;
            this.DepartmentId = employee.DepartmentId;
            this.FirstName = employee.FirstName;
            this.LastName = employee.LastName;
            this.DateOfBirth = employee.DateOfBirth;
            this.DateOfDeath = employee.DateOfDeath;
            if (employee.Department.HasValue)
            {
                this.Department = employee.Department.Value;
            }
        }

        public int Id { get; }

        public int DepartmentId { get; }

        public string FirstName { get; }

        public string LastName { get; }

        public DateTime DateOfBirth { get; }

        public DateTime? DateOfDeath { get; }

        public DepartmentResponse? Department { get; }
    }
}
