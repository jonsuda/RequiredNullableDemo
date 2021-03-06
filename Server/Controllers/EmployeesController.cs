﻿using System;
using System.Collections.Generic;
using System.Linq;
using RequiredNullableDemo.Data;
using RequiredNullableDemo.Infrastructure;
using RequiredNullableDemo.Models;

namespace RequiredNullableDemo.Controllers
{
    public class EmployeesController
    {
        private static readonly Lazy<EmployeesController> instance =
            new Lazy<EmployeesController>(() => new EmployeesController());

        public static EmployeesController Instance => instance.Value;

        private readonly Repository repository = Repository.Instance;

        private EmployeesController()
        { }

        public IEnumerable<EmployeeResponse> GetEmployees(bool includeDepartment) =>
            this
            .repository
            .GetEmployees(includeDepartment)
            .Select(x => EmployeeResponse.FromEmployee(x, includeDepartment))
            .ToArray();

        public IEnumerable<EmployeeResponse> GetEmployees(
            int departmentId, bool includeDepartment) =>
            this
            .repository
            .GetEmployees(departmentId, includeDepartment)
            .Select(x => EmployeeResponse.FromEmployee(x, includeDepartment))
            .ToArray();

        public EmployeeResponse? GetEmployee(int id, bool includeDepartment) =>
            EmployeeResponse.FromEmployee(
                this.repository.GetEmployee(id, includeDepartment), includeDepartment);

        public EmployeeResponse CreateEmployee(EmployeeRequest employee)
        {
            this.ValidateEmployee(employee);
            return EmployeeResponse.FromEmployee(
                this.repository.CreateEmployee(
                    new Employee(
                        employee.DepartmentId,
                        employee.FirstName,
                        employee.LastName,
                        employee.DateOfBirth,
                        employee.DateOfDeath)),
                true);
        }

        public EmployeeResponse UpdateEmployee(int id, EmployeeRequest employee)
        {
            this.ValidateEmployee(employee);
            return EmployeeResponse.FromEmployee(
                this.repository.UpdateEmployee(
                    new Employee(
                        employee.DepartmentId,
                        employee.FirstName,
                        employee.LastName,
                        employee.DateOfBirth,
                        employee.DateOfDeath)
                    {
                        Id = id
                    }),
                true);
        }

        public void DeleteEmployee(int id) =>
            this.repository.DeleteEmployee(id);

        private void ValidateEmployee(EmployeeRequest employee)
        {
            // generate the list of all validation (error) messages
            var validationMessages = new List<string>();
            if (employee.FirstName is null)
            {
                validationMessages.Add("First Name is required.");
            }
            if (employee.LastName is null)
            {
                // An intentional bug: The validation of the last name doesn't work

                //validationMessages.Add("Last Name is required.");
            }

            // throw an exception if there is at least one validation message
            if (validationMessages.Count > 0)
            {
                throw new ValidationException(validationMessages.ToArray());
            }
        }
    }
}
