using System.Text;

namespace Hydrogen {

	public class StringSizer : ItemSizer<string> {
		protected Encoding Encoding;

		public StringSizer(Encoding encoding) {
			Encoding = encoding;
		}

		public override int CalculateSize(string item) => Encoding.GetByteCount(item);
	}

}