using Acme.Seps.Domain.Base.Factory;
using System;

namespace Acme.Seps.Domain.Base.Entity
{
    /// <summary>
    /// <see href="https://ayende.com/blog/2500/generic-entity-equality">Entity base class credits</see>
    /// </summary>
    public abstract class Entity<TKey> : BaseEntity, IEquatable<Entity<TKey>>
    {
        public TKey Id { get; private set; }

        protected Entity() { }

        protected Entity(IIdentityFactory<TKey> identityFactory)
        {
            if (identityFactory == null)
                throw new ArgumentNullException(nameof(identityFactory));

            Id = identityFactory.CreateIdentity();
        }

        public override bool Equals(object obj) => Equals(obj as Entity<TKey>);

        public bool Equals(Entity<TKey> otherEntity) => ReferenceEquals(otherEntity, this) && otherEntity.Id.Equals(Id);

        public override int GetHashCode() => base.GetHashCode();

        public static bool operator ==(Entity<TKey> x, Entity<TKey> y) => Equals(x, y);

        public static bool operator !=(Entity<TKey> x, Entity<TKey> y) => !(x == y);
    }
}