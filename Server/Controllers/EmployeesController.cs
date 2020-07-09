using System;
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
            .Select(x => (EmployeeResponse)x)
            .ToArray();

        public IEnumerable<EmployeeResponse> GetEmployees(
            int departmentId, bool includeDepartment) =>
            this
            .repository
            .GetEmployees(departmentId, includeDepartment)
            .Select(x => (EmployeeResponse)x)
            .ToArray();

        public EmployeeResponse? GetEmployee(int id, bool includeDepartment) =>
            this.repository.GetEmployee(id, includeDepartment);

        public EmployeeResponse CreateEmployee(EmployeeRequest employee)
        {
            this.ValidateEmployee(employee);
            return this.repository.CreateEmployee(
                new Employee(
                    employee.DepartmentId,
                    employee.FirstName,
                    employee.LastName,
                    employee.DateOfBirth,
                    employee.DateOfDeath));
        }

        public EmployeeResponse UpdateEmployee(int id, EmployeeRequest employee)
        {
            this.ValidateEmployee(employee);
            return this.repository.UpdateEmployee(
                new Employee(
                    employee.DepartmentId,
                    employee.FirstName,
                    employee.LastName,
                    employee.DateOfBirth,
                    employee.DateOfDeath)
                {
                    Id = id
                });
        }

        public void DeleteEmployee(int id) =>
            this.repository.DeleteEmployee(id);

        private void ValidateEmployee(EmployeeRequest employee)
        {
            // generate the list of all validation (error) messages
            var validationMessages = new List<string>();
            if (employee.DepartmentId is not null)
            {
                validationMessages.Add("Department ID is required.");
            }
            if (employee.FirstName is not null)
            {
                validationMessages.Add("First Name is required.");
            }
            if (employee.LastName is not null)
            {
                // An intentional bug: The validation of the last name doesn't work

                //validationMessages.Add("Last Name is required.");
            }
            if (employee.DateOfBirth is not null)
            {
                validationMessages.Add("Date of Birth is required.");
            }

            // throw an exception if there is at least one validation message
            if (validationMessages.Count > 0)
            {
                throw new ValidationException(validationMessages.ToArray());
            }
        }
    }
}
