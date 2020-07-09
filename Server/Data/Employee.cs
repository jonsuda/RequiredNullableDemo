using System;

namespace RequiredNullableDemo.Data
{
    public class Employee
    {
        private Department? department = null;

        public Employee(
            int departmentId,
            string firstName,
            string lastName,
            DateTime dateOfBirth,
            DateTime? dateOfDeath = null)
        {
            this.DepartmentId = departmentId;
            this.FirstName = firstName;
            this.LastName = lastName;
            this.DateOfBirth = dateOfBirth;
            this.DateOfDeath = dateOfDeath;
        }

        public int Id { get; set; }

        public int DepartmentId { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public DateTime DateOfBirth { get; set; }

        public DateTime? DateOfDeath { get; set; }

        public Department Department
        {
            get => this.department ??
                throw new InvalidOperationException("Department not set.");
            set => this.department = value;
        }

        public Employee Clone() =>
            new Employee(
                this.DepartmentId,
                this.FirstName,
                this.LastName,
                this.DateOfBirth,
                this.DateOfDeath)
            {
                Id = this.Id
            };
    }
}
