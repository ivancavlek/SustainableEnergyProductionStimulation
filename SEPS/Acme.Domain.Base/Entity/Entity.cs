using Acme.Domain.Base.Factory;
using Light.GuardClauses;
using System;

namespace Acme.Domain.Base.Entity
{
    /// <summary>
    /// <see href="https://ayende.com/blog/2500/generic-entity-equality">Entity base class credits</see>
    /// </summary>
    public abstract class Entity<TKey> : BaseEntity, IEquatable<Entity<TKey>>
    {
        public event EventHandler<EntityExecutionLoggingEventArgs> EntityExecutionLogging = delegate { };

        public TKey Id { get; private set; }

        protected Entity()
        {
        }

        protected Entity(IIdentityFactory<TKey> identityFactory)
        {
            identityFactory.MustNotBeNull(nameof(identityFactory));

            Id = identityFactory.CreateIdentity();
        }

        public override bool Equals(object obj) =>
            Equals(obj as Entity<TKey>);

        public bool Equals(Entity<TKey> otherEntity) =>
            ReferenceEquals(otherEntity, this) && otherEntity.Id.Equals(Id);

        public override int GetHashCode() =>
            base.GetHashCode();

        public static bool operator ==(Entity<TKey> x, Entity<TKey> y) =>
            Equals(x, y);

        public static bool operator !=(Entity<TKey> x, Entity<TKey> y) =>
            !(x == y);

        protected void Log(EntityExecutionLoggingEventArgs entityExecutionLoggingEventArgs) =>
            EntityExecutionLogging(this, entityExecutionLoggingEventArgs);
    }
}