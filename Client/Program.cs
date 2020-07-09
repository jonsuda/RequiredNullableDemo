using System;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using RequiredNullableDemo.Models;

namespace RequiredNullableDemo
{
    class Program
    {
        private static readonly Operation[] operations = new Operation[]
        {
            new ListDepartmentsOperation(),
            new GetDepartmentOperation(),
            new CreateDepartmentOperation(),
            new UpdateDepartmentOperation(),
            new DeleteDepartmentOperation(),
            new ListEmployeesOperation(),
            new ListDepartmentEmployeesOperation(),
            new GetEmployeeOperation(),
            new CreateEmployeeOperation(),
            new UpdateEmployeeOperation(),
            new DeleteEmployeeOperation(),
            new ExitOperation()
        };

        static async Task Main()
        {
            using var client = new HttpClient();
            Operation operation;
            while (!(operation = PickOperation()).Exit)
            {
                try
                {
                    var response = await operation.ExecuteAsync(client);
                    Console.WriteLine(
                        $"{response.StatusCode} ({(int)response.StatusCode})");
                    var content = await response.Content.ReadAsStringAsync();
                    if (!string.IsNullOrWhiteSpace(content))
                    {
                        Console.WriteLine();
                        Console.WriteLine(content);
                    }
                    Console.WriteLine();
                }
                catch (Exception exception)
                {
                    Console.WriteLine(exception);
                    Console.WriteLine();
                }
            }
            Console.WriteLine();
        }

        private static Operation PickOperation()
        {
            for (int i = 0; i < operations.Length; i++)
            {
                Console.WriteLine($"{i + 1} - {operations[i].Label}");
            }
            return operations[ReadNumber(1, operations.Length) - 1];
        }

        private static int ReadNumber(int from, int to)
        {
            int value;
            string valueAsString;
            do
            {
                Console.Write("> ");
                valueAsString = Console.ReadLine();
            } while (!int.TryParse(
                valueAsString, out value) || value < from || value > to);
            Console.WriteLine();
            return value;
        }

        private static int ReadId(string label)
        {
            Console.WriteLine(label);
            return ReadNumber(0, int.MaxValue);
        }

        private static int? ReadNullableId(string label)
        {
            int? id = null;
            string idAsString;
            Console.WriteLine(label);
            do
            {
                Console.Write("> ");
                idAsString = Console.ReadLine();
                if (int.TryParse(idAsString, out int value) && value > 0)
                {
                    id = value;
                }
            } while (!(id.HasValue || string.IsNullOrWhiteSpace(idAsString)));
            Console.WriteLine();
            return id;
        }

        private static string ReadBoolean(string question)
        {
            bool? value;
            Console.WriteLine($"{question} [Y/N]");
            do
            {
                Console.Write("> ");
                value = Console.ReadLine().ToUpper() switch
                {
                    "Y" => true,
                    "YES" => true,
                    "N" => false,
                    "NO" => false,
                    _ => null
                };
            } while (value == null);
            Console.WriteLine();
            return value.Value.ToString().ToLower();
        }

        private static string? ReadString(string label)
        {
            Console.WriteLine(label);
            Console.Write("> ");
            string? value = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(value))
            {
                value = null;
            }
            Console.WriteLine();
            return value;
        }

        private static DateTime? ReadDateTime(string label)
        {
            DateTime? value = null;
            string? valueAsString;
            do
            {
                valueAsString = ReadString(label);
                if (valueAsString != null &&
                    DateTime.TryParse(valueAsString, out var parsedValue))
                {
                    value = parsedValue;
                }
            } while (!(value.HasValue || valueAsString == null));
            return value;
        }

        private static DepartmentRequest ReadDepartment() =>
            new DepartmentRequest()
            {
                Name = ReadString("Name")
            };

        private static EmployeeRequest ReadEmployee() =>
            new EmployeeRequest()
            {
                DepartmentId = ReadNullableId("Department ID"),
                FirstName = ReadString("First Name"),
                LastName = ReadString("Last Name"),
                DateOfBirth = ReadDateTime("Date of Birth"),
                DateOfDeath = ReadDateTime("Date of Death")
            };

        private abstract class Operation
        {
            protected Operation(string label, bool exit = false)
            {
                this.Label = label;
                this.Exit = exit;
            }

            public string Label { get; }

            public bool Exit { get; }

            public virtual Task<HttpResponseMessage> ExecuteAsync(HttpClient client) =>
                throw new NotImplementedException();

            protected Task<HttpResponseMessage> GetAsync(
                HttpClient client, string relativeUrl)
            {
                var url = this.GenerateUrl(relativeUrl);
                Console.WriteLine($"GET: {url}");
                Console.WriteLine();
                return client.GetAsync(url);
            }

            protected Task<HttpResponseMessage> PostAsync(
                HttpClient client, string relativeUrl, object body)
            {
                var url = this.GenerateUrl(relativeUrl);
                var content = JsonSerializer.Serialize(
                    body,
                    new JsonSerializerOptions()
                    {
                        IgnoreNullValues = true,
                        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                        WriteIndented = true
                    });
                Console.WriteLine($"POST: {url}");
                Console.WriteLine();
                Console.WriteLine(content);
                Console.WriteLine();
                return client.PostAsync(url, new StringContent(content));
            }

            protected Task<HttpResponseMessage> DeleteAsync(
                HttpClient client, string relativeUrl)
            {
                var url = this.GenerateUrl(relativeUrl);
                Console.WriteLine($"DELETE: {url}");
                Console.WriteLine();
                return client.DeleteAsync(url);
            }

            private string GenerateUrl(string relativeUrl) =>
                $"http://localhost:38088/{relativeUrl}";
        }

        private class ListDepartmentsOperation : Operation
        {
            public ListDepartmentsOperation()
                : base("List Departments")
            { }

            public override Task<HttpResponseMessage> ExecuteAsync(HttpClient client) =>
                this.GetAsync(client, "Departments");
        }

        private class GetDepartmentOperation : Operation
        {
            public GetDepartmentOperation()
                : base("Get Department")
            { }

            public override Task<HttpResponseMessage> ExecuteAsync(HttpClient client) =>
                this.GetAsync(client, $"Departments/{ReadId("Department ID")}");
        }

        private class CreateDepartmentOperation : Operation
        {
            public CreateDepartmentOperation()
                : base("Create Department")
            { }

            public override Task<HttpResponseMessage> ExecuteAsync(HttpClient client) =>
                this.PostAsync(client, "Departments", ReadDepartment());
        }

        private class UpdateDepartmentOperation : Operation
        {
            public UpdateDepartmentOperation()
                : base("Update Department")
            { }

            public override Task<HttpResponseMessage> ExecuteAsync(HttpClient client) =>
                this.PostAsync(client, $"Departments/{ReadId("Id")}", ReadDepartment());
        }

        private class DeleteDepartmentOperation : Operation
        {
            public DeleteDepartmentOperation()
                : base("Delete Department")
            { }

            public override Task<HttpResponseMessage> ExecuteAsync(HttpClient client) =>
                this.DeleteAsync(client, $"Departments/{ReadId("Id")}");
        }

        private class ListEmployeesOperation : Operation
        {
            public ListEmployeesOperation()
                : base("List Employees")
            { }

            public override Task<HttpResponseMessage> ExecuteAsync(HttpClient client) =>
                this.GetAsync(
                    client,
                    $"Employees?includeDepartment={ReadBoolean("Include Department")}");
        }

        private class ListDepartmentEmployeesOperation : Operation
        {
            public ListDepartmentEmployeesOperation()
                : base("List Department Employees")
            { }

            public override Task<HttpResponseMessage> ExecuteAsync(HttpClient client) =>
                this.GetAsync(client, $"Departments/{ReadId("Department ID")}/Employees");
        }

        private class GetEmployeeOperation : Operation
        {
            public GetEmployeeOperation()
                : base("Get Employee")
            { }

            public override Task<HttpResponseMessage> ExecuteAsync(HttpClient client) =>
                this.GetAsync(
                    client,
                    $"Employees/{ReadId("Employee ID")}?includeDepartment={ReadBoolean("Include Department")}");
        }

        private class CreateEmployeeOperation : Operation
        {
            public CreateEmployeeOperation()
                : base("Create Employee")
            { }

            public override Task<HttpResponseMessage> ExecuteAsync(HttpClient client) =>
                this.PostAsync(client, "Employees", ReadEmployee());
        }

        private class UpdateEmployeeOperation : Operation
        {
            public UpdateEmployeeOperation()
                : base("Update Employee")
            { }

            public override Task<HttpResponseMessage> ExecuteAsync(HttpClient client) =>
                this.PostAsync(
                    client,
                    $"Employees/{ReadId("Employee ID")}", ReadEmployee());
        }

        private class DeleteEmployeeOperation : Operation
        {
            public DeleteEmployeeOperation()
                : base("Delete Employee")
            { }

            public override Task<HttpResponseMessage> ExecuteAsync(HttpClient client) =>
                this.DeleteAsync(client, $"Employees/{ReadId("Employee ID")}");
        }

        private class ExitOperation : Operation
        {
            public ExitOperation()
                : base("Exit", exit: true)
            { }
        }
    }
}
