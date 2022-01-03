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

        /// <summary>
        /// The <see cref="DataOrigin"/> owned by this owner.
        /// </summary>
        public DataOrigin Origin { get; }

        /// <summary>
        /// Creates a named <see cref="DataOrigin"/> for the caller.
        /// </summary>
        /// <param name="name">The name of the origin to create.</param>
        public DataOriginOwner(string name)
            : this(new DataOrigin(name, null, 1))
        {
        }

        /// <summary>
        /// Creates a named <see cref="DataOrigin"/> for the provided member.
        /// </summary>
        /// <param name="name">The name of the origin to create.</param>
        /// <param name="forMember">The member to create for.</param>
        public DataOriginOwner(string name, MemberInfo forMember)
            : this(new DataOrigin(name, forMember, 1))
        {
        }

        /// <summary>
        /// Takes ownership of the provided origin.
        /// </summary>
        /// <param name="origin">The origin to take ownership of.</param>
        /// <exception cref="ArgumentNullException">Thrown is <paramref name="origin"/> is null.</exception>
        /// <exception cref="ArgumentException">Thrown if <paramref name="origin"/> already has an owner.</exception>
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
