using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hydrogen.DApp.Presentation2.UI.WalletTest {
	public class Transaction {
		public string Id { get; set; }
		public string Data { get; set; }
		public DateTime DateTime { get; set; }

		public Transaction() {
		}

		override public string ToString() {
			return $"Id: {Id.Substring(0, 8)} Time: {DateTime.ToString()}";
		}
	}
}
