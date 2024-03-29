﻿using Acme.Domain.Base.Entity;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Acme.Domain.Base.Repository;

public abstract class BaseSpecification<TAggregateRoot> where TAggregateRoot : BaseEntity, IAggregateRoot
{
    public List<Expression<Func<TAggregateRoot, BaseEntity>>> Includes { get; } =
        new List<Expression<Func<TAggregateRoot, BaseEntity>>>();

    public void AddInclude(Expression<Func<TAggregateRoot, BaseEntity>> includeExpression) =>
        Includes.Add(includeExpression);

    public bool IsSatisfiedBy(TAggregateRoot entity) =>
        ToExpression().Compile()(entity);

    public abstract Expression<Func<TAggregateRoot, bool>> ToExpression();
}