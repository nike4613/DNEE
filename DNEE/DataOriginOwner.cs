using DNEE.Internal.Resources;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace DNEE
{
    /// <summary>
    /// A private owner of a <see cref="DataOrigin"/>. Unline the <see cref="DataOrigin"/> itself, instances of this
    /// must be kept private.
    /// </summary>
    public sealed class DataOriginOwner
    {
        private readonly object originAssocObj = new();
        public DataOrigin Origin { get; }

        public DataOriginOwner(string name)
            : this(new DataOrigin(name, null, 1))
        {
        }

        public DataOriginOwner(string name, MemberInfo forMember)
            : this(new DataOrigin(name, forMember, 1))
        {
        }

        public DataOriginOwner(DataOrigin origin)
        {
            if (origin is null)
                throw new ArgumentNullException(nameof(origin));
            if (origin.IsValid)
                throw new ArgumentException(SR.OriginOwner_OriginAlreadyAttached);

            origin.SetSource(originAssocObj);
            Origin = origin;

            if (!origin.IsValid)
                throw new ArgumentException(SR.OriginOwner_OriginAlreadyAttached);
        }
    }
}
