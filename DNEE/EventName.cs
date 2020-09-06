using DNEE.Internal;
using DNEE.Internal.Resources;
using System;
using System.Reflection;

namespace DNEE
{
    public struct EventName : IEquatable<EventName>
    {
        public DataOrigin Origin { get; }
        public string Name { get; }

        public EventName(DataOrigin origin, string name)
        {
            if (!origin.IsValid)
                throw new ArgumentException(SR.EventName_OriginNotValid, nameof(origin));
            if (name is null)
                throw new ArgumentNullException(nameof(name));
            Origin = origin;
            Name = name;
        }

        public bool IsValid => Origin != null && Name != null;

        public bool Equals(EventName other)
            => Origin == other.Origin && Name == other.Name;

        public override bool Equals(object obj)
            => obj is EventName name && Equals(name);

        public override int GetHashCode()
            => Origin?.GetHashCode() ?? 0 ^ Name?.GetHashCode() ?? 0;

        public override string ToString()
        {
            if (Origin == null) return ""; // for if this was default constructed
            return $"{Origin}::{Name}";
        }
    }
}