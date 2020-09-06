using DNEE.Internal;
using DNEE.Internal.Resources;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Text;

namespace DNEE
{
    [DebuggerDisplay("\\{{GetType().Name,nq} {Assembly.GetName().Name,nq}::{Name} TrustByDefault = {ShouldTrustByDefault}\\}")]
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

        private EventSource? source;
        internal EventSource Source
        {
            get => source ?? throw new InvalidOperationException();
            set
            {
                if (source != null)
                    throw new InvalidOperationException(SR.Origin_SourceSetOnceOnly);
                source = value;
            }
        }

        public bool IsValid => source != null;

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
