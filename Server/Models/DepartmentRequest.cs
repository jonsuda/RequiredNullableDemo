using System;
using System.Text.Json.Serialization;

namespace RequiredNullableDemo.Models
{
    public class DepartmentRequest
    {
        private string? name = null;

        public string Name
        {
            get => this.name ??
                throw new InvalidOperationException(
                    $"The value of {nameof(this.Name)} is not set.");
            set => this.name = value;
        }

        [JsonIgnore]
        public bool IsNameSet => this.name != null;
    }
}
