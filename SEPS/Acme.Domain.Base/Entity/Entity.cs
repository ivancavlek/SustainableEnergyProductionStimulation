using Acme.Domain.Base.Factory;
using System;

namespace Acme.Domain.Base.Entity;

/// <summary>
/// <see href="https://ayende.com/blog/2500/generic-entity-equality">Entity base class credits</see>
/// </summary>
public abstract class Entity<TKey> : BaseEntity, IEquatable<Entity<TKey>>
{
    public Guid Id { get; private set; }

    protected Entity() { }

    protected Entity(IIdentityFactory<Guid> identityFactory) =>
        Id = identityFactory.MustNotBeNull(nameof(identityFactory)).CreateIdentity();

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
}