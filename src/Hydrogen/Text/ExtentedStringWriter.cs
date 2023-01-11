using System.IO;
using System.Text;

namespace Hydrogen;

public sealed class StringWriterEx : StringWriter {

	public StringWriterEx() : this(Encoding.Default) {
	}

	public StringWriterEx(Encoding encoding) : this(new StringBuilder(), encoding) {
	}

	public StringWriterEx(StringBuilder builder, Encoding encoding) : base(builder) {
		this.Encoding = encoding;
	}
	public override Encoding Encoding { get; }
}
