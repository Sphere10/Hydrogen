using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sphere10.Framework {

	public class StringSizer: DynamicObjectSizer<string> {
		protected Encoding Encoding;

		public StringSizer(Encoding encoding) {
			Encoding = encoding;
		}

		public override int CalculateTotalSize(IEnumerable<string> items, bool calculateIndividualItems, out int[] itemSizes) {
			var sizes = items.Select(CalculateSize).ToArray();
			itemSizes = calculateIndividualItems ? sizes.ToArray() : null;
			return sizes.Sum();
		}

		public override int CalculateSize(string item) => Encoding.GetByteCount(item);
	}

}