using DNEE.Internal;
using DNEE.Internal.Resources;
using System;
using System.Reflection;

namespace DNEE
{
    /// <summary>
    /// A struct that uniquely identifies an event that can be subscribed to.
    /// </summary>
    public struct EventName : IEquatable<EventName>
    {
        /// <summary>
        /// Gets the origin associated with the event.
        /// </summary>
        public DataOrigin Origin { get; }
        /// <summary>
        /// Gets the name of the event.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Constructs an <see cref="EventName"/> from a <see cref="DataOrigin"/> and name.
        /// </summary>
        /// <param name="origin">The origin associated with the event.</param>
        /// <param name="name">The name of the event.</param>
        public EventName(DataOrigin origin, string name)
        {
            if (!origin.IsValid)
                throw new ArgumentException(SR.EventName_OriginNotValid, nameof(origin));
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentNullException(nameof(name));
            Origin = origin;
            Name = name;
        }

        /// <summary>
        /// Gets whether or not this <see cref="EventName"/> is valid.
        /// </summary>
        public bool IsValid => Origin != null && Name != null;

        /// <inheritdoc/>
        public bool Equals(EventName other)
            => Origin == other.Origin && Name == other.Name;

        /// <inheritdoc/>
        public override bool Equals(object obj)
            => obj is EventName name && Equals(name);

        /// <inheritdoc/>
        public override int GetHashCode()
            => Origin?.GetHashCode() ?? 0 ^ Name?.GetHashCode() ?? 0;

        /// <inheritdoc/>
        public override string ToString()
        {
            if (Origin == null) return ""; // for if this was default constructed
            return $"{Origin}::{Name}";
        }
    }
}