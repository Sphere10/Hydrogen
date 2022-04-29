using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace Hydrogen {

    public class BijectiveDictionary<U, V> : DictionaryDecorator<U, V>, IBijectiveDictionary<U, V> {
        private readonly BijectiveDictionary<V, U> _bijection;

        public BijectiveDictionary() : this(EqualityComparer<U>.Default, EqualityComparer<V>.Default) {
        }

        public BijectiveDictionary(IEqualityComparer<U> equalityComparerU, IEqualityComparer<V> equalityComparerV) 
            : this(new Dictionary<U, V>(equalityComparerU), new Dictionary<V, U>(equalityComparerV)) {
        }

        public BijectiveDictionary(IDictionary<U, V> internalDictionary, IDictionary<V, U> internalBijectiveDictionary) : base(internalDictionary) {
            _bijection = new BijectiveDictionary<V, U>(internalBijectiveDictionary, this);
        }

        public BijectiveDictionary(IDictionary<U, V> internalDictionary, BijectiveDictionary<V, U> bijection) : base(internalDictionary) {
            _bijection = bijection;
        }

        public override bool IsReadOnly => base.IsReadOnly && _bijection.InternalDictionary.IsReadOnly;

        public IBijectiveDictionary<V, U> Bijection => _bijection;

        public override void Add(U key, V value) {
            base.Add(key, value);
            _bijection.InternalDictionary.Add(value, key);
        }

        public override V this[U key] {
            get => base[key];
            set {
                base[key] = value;
                _bijection.InternalDictionary[value] = key;
            }
        }

        public override void Add(KeyValuePair<U, V> item) { 
            base.Add(item);
            _bijection.InternalDictionary.Add(item.ToInverse());
        }
        
        public override bool Remove(KeyValuePair<U, V> item) => base.Remove(item) && _bijection.InternalDictionary.Remove(item.ToInverse());
    
        public override bool Remove(U key) {
            if (TryGetValue(key, out var val))
                if (base.Remove(key))
	                if (!_bijection.InternalDictionary.Remove(val))
		                throw new InvalidOperationException("Failed to remove from bijection");
            return false;
        }

        public bool ContainsValue(V value) => _bijection.InternalDictionary.ContainsKey(value);

        public bool TryGetKey(V value, out U key) => _bijection.InternalDictionary.TryGetValue(value, out key);

        public override void Clear() { 
            base.Clear();
            _bijection.InternalDictionary.Clear();
        }

    }

}
