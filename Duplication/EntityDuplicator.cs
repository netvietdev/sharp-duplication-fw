﻿using Duplication.Caches;
using Duplication.Exceptions;
using Duplication.Extensions;
using Duplication.Models;
using Duplication.SetValueStrategies;
using Duplication.SetValueStrategies.Builders;
using Duplication.SetValueStrategies.Builders.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Duplication
{
    internal class EntityDuplicator
    {
        public T Duplicate<T>(T source) where T : IEntityCloneable<T>
        {
            return Duplicate(source, source.SetValueStrategyBuilder.Build());
        }

        public T Duplicate<T>(T source, ISetValueStrategyBuilder<T> overloadSetValueStrategyBuilder) where T : IEntityCloneable<T>
        {
            var setValueStrategies = overloadSetValueStrategyBuilder.MergeWith(source.SetValueStrategyBuilder);
            return Duplicate(source, setValueStrategies);
        }

        private T Duplicate<T>(T source, IDictionary<Expression<Func<T, object>>, ISetValueStrategy> registeredStrategies) where T : IEntityCloneable<T>
        {
            var registeredProperties = registeredStrategies.Keys.Select(x => x.GetPropertyInfo()).ToList();
            var entityAdapter = EntityTypeAdapterCache.Current.GetEntityTypeAdapter(typeof(T));
            ValidateRegisteredStrategies(entityAdapter.PublicProperties, registeredProperties);

            var duplicated = source.Clone();

            foreach (var kvp in registeredStrategies)
            {
                kvp.Value.SetValue(kvp.Key.GetPropertyInfo(), source, duplicated);
            }

            return duplicated;
        }

        private void ValidateRegisteredStrategies(IEnumerable<PropertyInfo> publicProperties, IEnumerable<PropertyInfo> registeredProperties)
        {
            var notRegisteredProperty =
                publicProperties.Where(p => p.CanWrite)
                    .FirstOrDefault(p => registeredProperties.All(rp => rp.Name != p.Name));

            if (notRegisteredProperty != null)
            {
                throw new NotRegisteredSetValueStrategyException(notRegisteredProperty);
            }
        }
    }
}
