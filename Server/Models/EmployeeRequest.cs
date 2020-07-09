using System;
using System.Text.Json.Serialization;

namespace RequiredNullableDemo.Models
{
    public class EmployeeRequest
    {
        private int? departmentId = null;

        private string? firstName = null;

        private string? lastName = null;

        private DateTime? dateOfBirth = null;

        public int DepartmentId
        {
            get => this.departmentId ??
                throw new InvalidOperationException(
                    $"The value of {nameof(this.DepartmentId)} is not set.");
            set => this.departmentId = value;
        }

        [JsonIgnore]
        public bool IsDepartmentIdSet => this.departmentId.HasValue;

        public string FirstName
        {
            get => this.firstName ??
                throw new InvalidOperationException(
                    $"The value of {nameof(this.FirstName)} is not set.");
            set => this.firstName = value;
        }

        [JsonIgnore]
        public bool IsFirstNameSet => this.firstName != null;

        public string LastName
        {
            get => this.lastName ??
                throw new InvalidOperationException(
                    $"The value of {nameof(this.LastName)} is not set.");
            set => this.lastName = value;
        }

        [JsonIgnore]
        public bool IsLastNameSet => this.lastName != null;

        public DateTime DateOfBirth
        {
            get => this.dateOfBirth ??
                throw new InvalidOperationException(
                    $"The value of {nameof(this.DateOfBirth)} is not set.");
            set => this.dateOfBirth = value;
        }

        [JsonIgnore]
        public bool IsDateOfBirthSet => this.dateOfBirth.HasValue;

        public DateTime? DateOfDeath { get; set; }
    }
}
