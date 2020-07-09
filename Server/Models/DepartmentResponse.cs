using RequiredNullableDemo.Data;

namespace RequiredNullableDemo.Models
{
    public class DepartmentResponse
    {
        public static implicit operator DepartmentResponse(Department department) =>
            new DepartmentResponse(department);

        private DepartmentResponse(Department department)
        {
            this.Id = department.Id;
            this.Name = department.Name;
        }

        public int Id { get; }

        public string Name { get; }
    }
}
