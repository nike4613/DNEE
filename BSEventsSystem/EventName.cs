using System;
using System.Reflection;

namespace BSEventsSystem
{
    public struct EventName : IEquatable<EventName>
    {
        public Assembly NameContext { get; }
        public string Name { get; }

        public EventName(Assembly assembly, string name)
        {
            NameContext = assembly;
            Name = name;
        }

        public bool IsValid => NameContext != null && Name != null;

        public static EventName InSelf(string name)
            => new EventName(Assembly.GetCallingAssembly(), name);
        public static EventName In<T>(string name)
            => new EventName(typeof(T).Assembly, name);

        public bool Equals(EventName other)
            => NameContext == other.NameContext && Name == other.Name;

        public override bool Equals(object obj)
            => obj is EventName name && Equals(name);

        public override int GetHashCode()
            => NameContext?.GetHashCode() ?? 0 ^ Name?.GetHashCode() ?? 0;

        public override string ToString()
        {
            if (NameContext == null) return "";
            return $"{NameContext.GetName().Name}::{Name}";
        }
    }
}