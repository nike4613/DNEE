using DNEE.Internal;
using DNEE.Internal.Resources;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;

namespace DNEE
{
    /// <summary>
    /// An origin from which event data comes.
    /// </summary>
    [DebuggerDisplay("\\{{GetType().Name,nq} {Assembly.GetName().Name,nq}::{Name} AssumeTrusted = {ShouldAssumeTrusted}\\}")]
    public class DataOrigin
    {
        /// <summary>
        /// The name of the origin. Used only for human identification.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// The assembly that owns the origin. This will always be the assembly 
        /// that constructed this <see cref="DataOrigin"/>.
        /// </summary>
        public Assembly Assembly { get; }

        /// <summary>
        /// The member for which this origin was created. By default, it is the
        /// <see cref="Type"/> that constructed this origin.
        /// </summary>
        public MemberInfo Member { get; }

        /// <summary>
        /// Indicates whether or not this origin should be implicitly trusted.
        /// Subclasses may set this to <see langword="false"/> to indicate that 
        /// data that comes from that origin should be placed under far more scrutiny
        /// and handled much more carefully.
        /// </summary>
        public virtual bool ShouldAssumeTrusted => true;

        /// <summary>
        /// Constructs an origin with a name and optional owning member.
        /// </summary>
        /// <param name="name">The name of the origin.</param>
        /// <param name="member">The owning member to associate with, or the <see cref="Type"/> that called this constructor.</param>
        /// <exception cref="ArgumentException">Thrown if <paramref name="name"/> is null or whitespace.</exception>
        public DataOrigin(string name, MemberInfo? member = null)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException(SR.Origin_NameCannotBeNullOrWhitespace, nameof(name));

            Name = name;
            var constructing = GetConstructingMethod();
            Member = member ?? constructing.DeclaringType;
            Assembly = constructing.DeclaringType.Assembly;
        }

        internal DataOrigin(string name, MemberInfo? member, int skipToFindCreator)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException(SR.Origin_NameCannotBeNullOrWhitespace, nameof(name));

            Name = name;
            var constructing = GetConstructingMethod(skipToFindCreator);
            Member = member ?? constructing.DeclaringType;
            Assembly = constructing.DeclaringType.Assembly;
        }

        private static readonly ConditionalWeakTable<DataOrigin, object> sourceTable = new();
        private object? source;
        internal object Source => source ?? throw new InvalidOperationException();

        internal void SetSource(object source)
        {
            if (this.source != null)
                throw new InvalidOperationException(SR.Origin_SourceSetOnceOnly);
            sourceTable.Add(this, source);
            this.source = source;
        }

        /// <summary>
        /// Gets whether or not this origin is valid. (That is, whether or not it is attached to an <see cref="EventSource"/>.
        /// </summary>
        public bool IsValid => source != null && sourceTable.TryGetValue(this, out var val) && ReferenceEquals(source, val);

        /// <inheritdoc/>
        public override string ToString()
            => string.Format(SR.Origin_StringFormat, Name, Assembly.GetName().Name, Member.Name, GetType().Name);

        private static MethodBase GetConstructingMethod(int skipFrames = 0)
        {
            var trace = new StackTrace(2 + skipFrames, false);
            for (int i = 0; i < trace.FrameCount; i++)
            {
                var frame = trace.GetFrame(i);
                var method = frame.GetMethod();
                if (method.MemberType == MemberTypes.Constructor
                 && typeof(DataOrigin).IsAssignableFrom(method.DeclaringType))
                {
                    continue;
                }
                else
                { // we found a frame that isn't part of the constructor chain
                    return method;
                }
            }

            throw new InvalidOperationException(SR.Origin_CouldNotGetConstructingMember);
        }
    }
}
