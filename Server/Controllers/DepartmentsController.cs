using System;
using System.Collections.Generic;
using System.Linq;
using RequiredNullableDemo.Data;
using RequiredNullableDemo.Infrastructure;
using RequiredNullableDemo.Models;

namespace RequiredNullableDemo.Controllers
{
    public class DepartmentsController
    {
        private static readonly Lazy<DepartmentsController> instance =
            new Lazy<DepartmentsController>(() => new DepartmentsController());

        public static DepartmentsController Instance => instance.Value;

        private readonly Repository repository = Repository.Instance;

        private DepartmentsController()
        { }

        public IEnumerable<DepartmentResponse> GetDepartments() =>
            this
            .repository
            .GetDepartments()
            .Select(x => (DepartmentResponse)x)
            .ToArray();

        public DepartmentResponse GetDepartment(int id) =>
            this.repository.GetDepartment(id);

        public DepartmentResponse CreateDepartment(DepartmentRequest department)
        {
            this.ValidateDepartment(department);
            return this.repository.CreateDepartment(
                new Department(department.Name));
        }

        public DepartmentResponse UpdateDepartment(int id, DepartmentRequest department)
        {
            this.ValidateDepartment(department);
            return this.repository.UpdateDepartment(
                new Department(department.Name)
                {
                    Id = id
                });
        }

        public void DeleteDepartment(int id) =>
            this.repository.DeleteDepartment(id);

        private void ValidateDepartment(DepartmentRequest department)
        {
            if (!department.IsNameSet)
            {
                throw new ValidationException("Name is required.");
            }
        }
    }
}
