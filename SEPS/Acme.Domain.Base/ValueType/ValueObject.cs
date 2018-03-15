using System;
using System.Collections.Generic;
using System.Reflection;

namespace Acme.Domain.Base.ValueType
{
    /// <summary>
    /// <see href="https://lostechies.com/jimmybogard/2007/06/25/generic-value-object-equality/">Value base class credits</see>
    /// </summary>
    public abstract class ValueObject : IEquatable<ValueObject>
    {
        protected ValueObject()
        {
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;

            var other = obj as ValueObject;

            return Equals(other);
        }

        public override int GetHashCode()
        {
            var fields = GetFields();

            var startValue = 17;
            var multiplier = 59;

            var hashCode = startValue;

            foreach (var field in fields)
            {
                var value = field.GetValue(this);

                if (value != null)
                    hashCode = hashCode * multiplier + value.GetHashCode();
            }

            return hashCode;
        }

        public virtual bool Equals(ValueObject other)
        {
            var t = GetType();
            var otherType = other.GetType();

            if (t != otherType)
                return false;

            var fields = t.GetFields(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);

            foreach (FieldInfo field in fields)
            {
                var value1 = field.GetValue(other);
                var value2 = field.GetValue(this);

                if (value1 == null)
                {
                    if (value2 != null)
                        return false;
                }
                else if (!value1.Equals(value2))
                    return false;
            }

            return true;
        }

        private IEnumerable<FieldInfo> GetFields()
        {
            var t = GetType();

            var fields = new List<FieldInfo>();

            while (t != typeof(object))
            {
                fields.AddRange(t.GetFields(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public));

                t = t.BaseType;
            }

            return fields;
        }

        public static bool operator ==(ValueObject x, ValueObject y) =>
            x.Equals(y);

        public static bool operator !=(ValueObject x, ValueObject y) =>
            !(x == y);
    }
}