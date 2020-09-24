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
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="origin"/> is null 
        /// -OR- if <paramref name="name"/> is null.</exception>
        /// <exception cref="ArgumentException">Thrown if <paramref name="origin"/> is not a valid origin.</exception>
        public EventName(DataOrigin origin, string name)
        {
            if (origin is null)
                throw new ArgumentNullException(nameof(origin));
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

        /// <summary>
        /// Compares two <see cref="EventName"/>s for equality.
        /// </summary>
        /// <param name="left">The first name to compare.</param>
        /// <param name="right">The second name to compare.</param>
        /// <returns><see langword="true"/> if they are equal, <see langword="false"/> otherwise.</returns>
        public static bool operator ==(EventName left, EventName right) => left.Equals(right);

        /// <summary>
        /// Compares two <see cref="EventName"/>s for inequality.
        /// </summary>
        /// <param name="left">The first name to compare.</param>
        /// <param name="right">The second name to compare.</param>
        /// <returns><see langword="true"/> if they are not equal, <see langword="false"/> otherwise.</returns>
        public static bool operator !=(EventName left, EventName right) => !(left == right);
    }
}