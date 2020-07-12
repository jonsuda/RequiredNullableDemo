using System;

namespace RequiredNullableDemo
{
    public struct RequiredValue<T>
        where T : struct
    {
        public static implicit operator T(RequiredValue<T> requiredValue) => requiredValue.Value;

        public static implicit operator RequiredValue<T>(T? value) => new RequiredValue<T>(value);

        private readonly T? value;

        public RequiredValue(T? value) => this.value = value;

        public bool HasValue => this.value.HasValue;

        public T Value =>
            this.value ??
            throw new InvalidOperationException("Required object must have a value");

        public T? GetValueOrNull() => this.value;

        public T GetValueOrDefault(T defaultValue) => this.value ?? defaultValue;

        public override int GetHashCode() => this.value.GetHashCode();

        public override bool Equals(object? other) => this.value.Equals(other);

        public override string ToString() =>
            this.value?.ToString() ?? string.Empty;
    }
}
