using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using RequiredNullableDemo.Infrastructure;

namespace RequiredNullableDemo.Data
{
    public class Repository
    {
        private static readonly Lazy<Repository> instance =
            new Lazy<Repository>(() => new Repository());

        public static Repository Instance => instance.Value;

        private readonly List<Department> departments;

        private readonly List<Employee> employees;

        private Repository()
        {
            this.departments = new List<Department>();
            this.employees = new List<Employee>();

            // prepopulate with sample data
            var musicDepartment = this.CreateDepartment(new Department("Music"));
            var actingDepartment = this.CreateDepartment(new Department("Acting"));
            this.CreateEmployee(new Employee(
                musicDepartment.Id,
                "Johnny",
                "Cash",
                new DateTime(1932, 2, 26),
                new DateTime(2003, 9, 12)));
            this.CreateEmployee(new Employee(
                musicDepartment.Id,
                "Paul",
                "McCartney",
                new DateTime(1942, 6, 18)));
            this.CreateEmployee(new Employee(
                actingDepartment.Id,
                "Tom",
                "Hanks",
                new DateTime(1956, 7, 9)));
            this.CreateEmployee(new Employee(
                musicDepartment.Id,
                "John",
                "Lennon",
                new DateTime(1940, 10, 9),
                new DateTime(1980, 12, 8)));
            this.CreateEmployee(new Employee(
                actingDepartment.Id,
                "Leslie",
                "Nielsen",
                new DateTime(1926, 2, 11),
                new DateTime(2010, 11, 28)));
        }

        public IEnumerable<Department> GetDepartments() =>
            this.departments.Select(x => x.Clone());

        public Department GetDepartment(int id) =>
            this.GetInternalDepartment(id, HttpStatusCode.NotFound).Clone();

        public Department CreateDepartment(Department department)
        {
            department = new Department(department.Name.Trim())
            {
                Id = this.GenerateDepartmentId()
            };
            this.AssertDepartmentNameIsUnique(department.Name);
            this.departments.Add(department);
            return department.Clone();
        }

        public Department UpdateDepartment(Department department)
        {
            var existingDepartment =
                this.GetInternalDepartment(department.Id, HttpStatusCode.NotFound);
            var newName = department.Name.Trim();
            this.AssertDepartmentNameIsUnique(newName, existingDepartment.Id);
            existingDepartment.Name = newName;
            return existingDepartment.Clone();
        }

        public void DeleteDepartment(int id)
        {
            if (this.GetEmployees(id, false).Count() > 0)
            {
                throw new HttpStatusCodeException(
                    HttpStatusCode.BadRequest,
                    "The specified department has employees; please delete all employees before deleting the department.");
            }
            this.departments.Remove(
                this.GetInternalDepartment(id, HttpStatusCode.NotFound));
        }

        public IEnumerable<Employee> GetEmployees(bool includeDepartment) =>
            this
            .employees
            .Select(x => this.CloneEmployee(x, includeDepartment))
            .ToArray();

        public IEnumerable<Employee> GetEmployees(int departmentId, bool includeDepartment)
        {
            this.VerifyDepartmentExists(departmentId, HttpStatusCode.NotFound);
            return this
                .GetEmployees(includeDepartment)
                .Where(x => x.DepartmentId == departmentId);
        }

        public Employee GetEmployee(int id, bool includeDepartment) =>
            this.CloneEmployee(this.GetEmployee(id), includeDepartment);

        public Employee CreateEmployee(Employee employee)
        {
            this.VerifyDepartmentExists(
                employee.DepartmentId, HttpStatusCode.BadRequest);
            employee = employee.Clone();
            employee.Id = this.GenerateEmployeeId();
            this.employees.Add(employee);
            return this.CloneEmployee(employee, true);
        }

        public Employee UpdateEmployee(Employee employee)
        {
            var existingEmployee = this.GetEmployee(employee.Id);
            this.VerifyDepartmentExists(
                employee.DepartmentId, HttpStatusCode.BadRequest);
            existingEmployee.DepartmentId = employee.DepartmentId;
            existingEmployee.FirstName = employee.FirstName.Trim();
            existingEmployee.LastName = employee.LastName.Trim();
            existingEmployee.DateOfBirth = employee.DateOfBirth;
            existingEmployee.DateOfDeath = employee.DateOfDeath;
            return this.CloneEmployee(existingEmployee, true);
        }

        public void DeleteEmployee(int id) =>
            this.employees.Remove(this.GetEmployee(id));

        private void AssertDepartmentNameIsUnique(string name, int? id = null)
        {
            if (this
                .departments
                .Where(x =>
                    x.Id != id &&
                    string.Equals(
                        name, x.Name, StringComparison.InvariantCultureIgnoreCase))
                .Count() > 0)
            {
                throw new HttpStatusCodeException(
                    HttpStatusCode.BadRequest,
                    $"A department with the specified name ('{name}') already exists.");
            }
        }

        private Department GetInternalDepartment(
            int id, HttpStatusCode notFoundStatusCode) =>
            this.GetInternalDepartment(
                id,
                () => new HttpStatusCodeException(
                    notFoundStatusCode,
                    $"No department with the specified ID ({id}) could be found."));

        private void VerifyDepartmentExists(
            int id, HttpStatusCode notFoundStatusCode) =>
            this.GetInternalDepartment(id, notFoundStatusCode);

        private Department GetInternalDepartment(
            int id,
            Func<Exception> createNotFoundException)
        {
            var department = this.departments.SingleOrDefault(x => x.Id == id);
            if (department == null)
            {
                throw createNotFoundException();
            }
            return department;
        }

        private Employee GetEmployee(int id)
        {
            var employee = this.employees.SingleOrDefault(x => x.Id == id);
            if (employee == null)
            {
                throw new HttpStatusCodeException(
                    HttpStatusCode.NotFound,
                    $"No employee with the specified ID ({id}) could be found.");
            }
            return employee;
        }

        private Employee CloneEmployee(Employee employee, bool includeDepartment)
        {
            var clonedEmployee = employee.Clone();
            if (includeDepartment)
            {
                clonedEmployee.Department =
                    GetEmployeeDepartment(employee.DepartmentId);
            }
            return clonedEmployee;

            Department GetEmployeeDepartment(int id) =>
            this.GetInternalDepartment(
                id,
                () => new HttpStatusCodeException(
                    HttpStatusCode.InternalServerError,
                    $"Referential integrity violation: No department with the ID that the employee record specifies ({id}) could be found.")
                ).Clone();
        }

        private int GenerateDepartmentId() => this.GenerateId(this.departments, x => x.Id);

        private int GenerateEmployeeId() => this.GenerateId(this.employees, x => x.Id);

        private int GenerateId<T>(IEnumerable<T> entities, Func<T, int> getId) =>
            entities.Select(getId).DefaultIfEmpty().Max() + 1;
    }
}
