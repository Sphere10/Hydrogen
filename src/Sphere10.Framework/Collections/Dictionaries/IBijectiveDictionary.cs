using System.Collections.Generic;

namespace Sphere10.Framework {
    public interface IBijectiveDictionary<U, V> : IDictionary<U, V> {
        IBijectiveDictionary<V, U> Bijection { get; }

        bool ContainsValue(V value);

        bool TryGetKey(V value, out U key);

    }

}
