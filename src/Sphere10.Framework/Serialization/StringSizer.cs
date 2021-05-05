using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sphere10.Framework {

	public class StringSizer : ItemSizer<string> {
		protected Encoding Encoding;

		public StringSizer(Encoding encoding) {
			Encoding = encoding;
		}

		public override int CalculateSize(string item) => Encoding.GetByteCount(item);
	}

}