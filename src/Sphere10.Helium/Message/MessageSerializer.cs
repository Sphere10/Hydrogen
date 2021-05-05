using System;
using System.Collections.Generic;
using Sphere10.Framework;

namespace Sphere10.Helium.Message {
	internal class MessageSerializer : ItemSerializerBase<IMessage> {
		public override int CalculateSize(IMessage item) {
			throw new NotImplementedException();
		}

		public override IMessage Deserialize(int size, EndianBinaryReader reader) {
			throw new NotImplementedException();
		}

		public override int Serialize(IMessage @object, EndianBinaryWriter writer) {
			throw new NotImplementedException();
		}
	}
}