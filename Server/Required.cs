using System;

namespace RequiredNullableDemo
{
    public struct Required<T>
        where T : class
    {
        public static implicit operator T(Required<T> requiredValue) => requiredValue.Value;

        public static implicit operator Required<T>(T? value) => new Required<T>(value);

        public static bool operator ==(
            Required<T> requiredValue1, Required<T> requiredValue2) =>
            requiredValue1.Equals(requiredValue2);

        public static bool operator !=(
            Required<T> requiredValue1, Required<T> requiredValue2) =>
            !(requiredValue1 == requiredValue2);

        private readonly T? value;

        public Required(T? value) => this.value = value;

        public bool HasValue => this.value != null;

        public T Value =>
            this.value ??
            throw new InvalidOperationException("Required object must have a value.");

        public T? GetValueOrNull() => this.value;

        public T GetValueOrDefault(T defaultValue) => this.value ?? defaultValue;

        public override int GetHashCode() =>
            this.value?.GetHashCode() ?? 0;

        public override bool Equals(object? other)
        {
            bool equals;
            if (this.HasValue)
            {
                equals = this.Value.Equals(other);
            }
            else
            {
                equals = other == null;
            }
            return equals;
        }

        public override string ToString() =>
            this.value?.ToString() ?? string.Empty;
    }
}
