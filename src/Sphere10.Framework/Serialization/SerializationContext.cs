using System;
using System.Collections.Generic;
using System.Linq;

namespace Sphere10.Framework {
	public class SerializationContext {
		
		

		private readonly Dictionary<Reference<object>, int> _referenceDictionary = new ();

		public SerializationContext(EndianBinaryReader reader) {
			Reader = reader ?? throw new ArgumentNullException(nameof(reader));
		}

		public SerializationContext(EndianBinaryWriter writer) {
			Writer = writer ?? throw new ArgumentNullException(nameof(writer));
		}

		public EndianBinaryWriter Writer { get; }

		public EndianBinaryReader Reader { get; }

		public EndianBitConverter BitConverter => Writer.BitConverter ?? Reader.BitConverter;
		
		public object GetObjectByIndex(Type type, int index) {
			return _referenceDictionary.FirstOrDefault(x => x.Key.Object.GetType() == type && x.Value == index).Key.Object
				?? throw new InvalidOperationException($"No reference stored for type {type.Name} with index {index}");
		}

		public int AddSerializedObject(object obj) {
			int index = _referenceDictionary.Count(x => x.Key.Object.GetType() == obj.GetType());
			_referenceDictionary.Add(Reference.For(obj), index);
			return index;
		}

		public bool TryGetObjectIndex(object obj, out int index) {
			return _referenceDictionary.TryGetValue(Reference.For(obj), out index);
		} 
	}
}
