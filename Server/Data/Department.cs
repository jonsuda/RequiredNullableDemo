namespace RequiredNullableDemo.Data
{
    public class Department
    {
        public Department(string name)
        {
            this.Name = name;
        }

        public int Id { get; set; }

        public string Name { get; set; }

        public Department Clone() =>
            new Department(this.Name)
            {
                Id = this.Id
            };
    }
}
