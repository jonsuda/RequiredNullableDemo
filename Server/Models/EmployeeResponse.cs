﻿using System;
using RequiredNullableDemo.Data;

namespace RequiredNullableDemo.Models
{
    public class EmployeeResponse
    {
        public static EmployeeResponse FromEmployee(
            Employee employee, bool includeDepartment) =>
            new EmployeeResponse(employee, includeDepartment);

        private EmployeeResponse(Employee employee, bool includeDepartment)
        {
            this.Id = employee.Id;
            this.DepartmentId = employee.DepartmentId;
            this.FirstName = employee.FirstName;
            this.LastName = employee.LastName;
            this.DateOfBirth = employee.DateOfBirth;
            this.DateOfDeath = employee.DateOfDeath;
            if (includeDepartment)
            {
                this.Department = employee.Department;
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
