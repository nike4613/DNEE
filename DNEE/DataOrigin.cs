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
    [DebuggerDisplay("\\{{GetType().Name,nq} {Assembly.GetName().Name,nq}::{Name} AssumeTrusted = {ShouldAssumeTrusted}\\}")]
    public class DataOrigin
    {
        public string Name { get; }

        public Assembly Assembly { get; }

        public MemberInfo Member { get; }

        public virtual bool ShouldAssumeTrusted => true;

        public DataOrigin(string name, MemberInfo? member = null)
        {
            Name = name;
            var constructing = GetConstructingMethod();
            Member = member ?? constructing.DeclaringType;
            Assembly = constructing.DeclaringType.Assembly;
        }

        internal DataOrigin(string name, MemberInfo? member, int skipToFindCreator)
        {
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

        public bool IsValid => source != null && sourceTable.TryGetValue(this, out var val) && ReferenceEquals(source, val);

        public override string ToString()
            => string.Format(SR.Origin_StringFormat, Name, Assembly.GetName().Name, Member.Name, GetType().Name);

        private static MethodBase GetConstructingMethod(int skipFrames = 0)
        {
            var trace = new StackTrace(2 + skipFrames, false);
            for (int i = trace.FrameCount - 1; i >= 0; i--)
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
