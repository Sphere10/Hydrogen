using System;
using System.IO;
using System.Linq;
using System.Text;
using NUnit.Framework;

namespace Sphere10.Framework.Tests {
	public class StreamMappedDynamicClusteredListTests {
	
		[Test]
		public void RequiresLoad() {
			using var stream = new MemoryStream();
			var list = new StreamMappedDynamicClusteredList<string>(128, new StringSerializer(Encoding.UTF8), stream);
			Assert.IsFalse(list.RequiresLoad);
			
			var secondList = new StreamMappedDynamicClusteredList<string>(128, new StringSerializer(Encoding.UTF8), stream);
			Assert.IsTrue(secondList.RequiresLoad);

			secondList.Load();
			Assert.IsFalse(secondList.RequiresLoad);

		}
	}
}
