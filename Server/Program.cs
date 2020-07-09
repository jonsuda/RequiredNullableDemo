using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using RequiredNullableDemo.Controllers;
using RequiredNullableDemo.Infrastructure;
using RequiredNullableDemo.Models;

namespace RequiredNullableDemo
{
    public static class Program
    {
        static void Main()
        {
            using var listener = new HttpListener();
            listener.Prefixes.Add("http://localhost:38088/");
            listener.Start();
            Task.Factory.StartNew(ProcessRequests, listener);
            Console.ReadLine();
            listener.Stop();
        }

        private static void ProcessRequests(object? listener)
        {
            while (true)
            {
                var context = ((HttpListener?)listener)!.GetContext();
                try
                {
                    ProcessRequest(context);
                }
                catch (HttpStatusCodeException statusCodeException)
                {
                    SetResponse(
                        context.Response,
                        statusCodeException.Code,
                        ErrorResponse.FromException(statusCodeException));
                }
                catch (ValidationException validationException)
                {
                    SetResponse(
                        context.Response,
                        HttpStatusCode.BadRequest,
                        ValidationFailedResponse.FromException(validationException));
                }
                catch (Exception exception)
                {
                    SetResponse(
                        context.Response,
                        HttpStatusCode.InternalServerError,
                        new ErrorResponse(
                            (int)HttpStatusCode.InternalServerError,
                            exception.Message));
                }
            }
        }

        private static void ProcessRequest(HttpListenerContext context)
        {
            switch (context.Request.HttpMethod)
            {
                case "GET":
                    ProcessGet(context);
                    break;

                case "POST":
                    ProcessPost(context);
                    break;

                case "DELETE":
                    ProcessDelete(context);
                    break;

                default:
                    break;
            }
        }

        private static void ProcessGet(HttpListenerContext context)
        {
            var (collection, id, subcollection) = ParseRawUrl(context.Request.RawUrl);
            object? response = collection switch
            {
                "departments" when !id.HasValue && subcollection == null =>
                    DepartmentsController.Instance.GetDepartments(),

                "departments" when id.HasValue && subcollection == null =>
                    DepartmentsController.Instance.GetDepartment(id.Value),

                "departments" when id.HasValue && subcollection == "employees" =>
                    EmployeesController.Instance.GetEmployees(
                        id.Value, IncludeDepartment()),

                "employees" when !id.HasValue && subcollection == null =>
                    EmployeesController.Instance.GetEmployees(IncludeDepartment()),

                "employees" when id.HasValue && subcollection == null =>
                    EmployeesController.Instance.GetEmployee(
                        id.Value, IncludeDepartment()),

                _ => throw new InvalidRouteException()
            };
            SetResponse(context.Response, HttpStatusCode.OK, response);

            bool IncludeDepartment() =>
                context
                .Request
                .QueryString["includeDepartment"]
                ?.Trim()
                ?.ToLower() == "true";
        }

        private static void ProcessPost(HttpListenerContext context)
        {
            var (collection, id, subcollection) = ParseRawUrl(context.Request.RawUrl);
            object? response = collection switch
            {
                "departments" when !id.HasValue && subcollection == null =>
                    DepartmentsController.Instance.CreateDepartment(
                        Serializer.Instance.Deserialize<DepartmentRequest>(
                            context.Request.InputStream)),

                "departments" when id.HasValue && subcollection == null =>
                    DepartmentsController.Instance.UpdateDepartment(
                        id.Value,
                        Serializer.Instance.Deserialize<DepartmentRequest>(
                            context.Request.InputStream)),

                "employees" when !id.HasValue && subcollection == null =>
                    EmployeesController.Instance.CreateEmployee(
                        Serializer.Instance.Deserialize<EmployeeRequest>(
                            context.Request.InputStream)),

                "employees" when id.HasValue && subcollection == null =>
                    EmployeesController.Instance.UpdateEmployee(
                        id.Value,
                        Serializer.Instance.Deserialize<EmployeeRequest>(
                            context.Request.InputStream)),

                _ => throw new InvalidRouteException()
            };
            SetResponse(context.Response, HttpStatusCode.OK, response);
        }

        private static void ProcessDelete(HttpListenerContext context)
        {
            var (collection, id, subcollection) = ParseRawUrl(context.Request.RawUrl);
            switch (collection)
            {
                case "departments" when id.HasValue && subcollection == null:
                    DepartmentsController.Instance.DeleteDepartment(id.Value);
                    break;

                case "employees" when id.HasValue && subcollection == null:
                    EmployeesController.Instance.DeleteEmployee(id.Value);
                    break;

                default:
                    throw new InvalidRouteException();
            }
            SetResponse(context.Response, HttpStatusCode.OK);
        }

        private static (string collection, int? id, string? subcollection)
            ParseRawUrl(string rawUrl)
        {
            var components = SplitUrl();
            if (components.Length == 1)
            {
                return (components[0], null, null);
            }
            else if (components.Length == 2)
            {
                return (components[0], ParseInt(components[1]), null);
            }
            else if (components.Length == 3)
            {
                return (components[0], ParseInt(components[1]), components[2]);
            }
            else
            {
                throw new InvalidRouteException();
            }

            string[] SplitUrl()
            {
                int questionMarkIndex = rawUrl.IndexOf('?');
                if (questionMarkIndex >= 0)
                {
                    rawUrl = rawUrl.Substring(0, questionMarkIndex);
                }
                return rawUrl
                    .Substring(1)
                    .Split('/')
                    .Select(x => x.ToLower())
                    .ToArray();
            }

            static int ParseInt(string value)
            {
                if (!int.TryParse(value, out int result))
                {
                    throw new HttpStatusCodeException(
                        HttpStatusCode.BadRequest,
                        $"The value '{value}' doesn't represent a valid ID (number).");
                }
                return result;
            }
        }

        private static void SetResponse(
            HttpListenerResponse response, HttpStatusCode statusCode, object? body = null)
        {
            response.StatusCode = (int)statusCode;
            response.ContentType = "application/json";
            using var writer = new StreamWriter(response.OutputStream);
            if (body != null)
            {
                writer.Write(Serializer.Instance.Serialize(body));
            }
        }
    }
}
