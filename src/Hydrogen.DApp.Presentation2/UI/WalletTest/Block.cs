using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hydrogen.DApp.Presentation2.UI.WalletTest {
	public class Block {

		public string Id { get; set; }
		public DateTime CreationDateTime { get; set; }
		public int Number { get; set; }
		public List<Transaction> Transactions { get; set; } = new List<Transaction>();

		static Random Random { get; set; } = new Random((int)DateTime.Now.Ticks);

		public Block() { 
		}

		public void FillWithTestData(string id, int number) {
			Id = id;
			Number = number;

			CreationDateTime = DateTime.Now;
			var transactionCount = Random.Next(1, 20);
			for (var i = 0 ; i < transactionCount; i++)
			{
				var transaction = new Transaction();
				transaction.Id = Guid.NewGuid().ToString();
				transaction.DateTime = DateTime.Now;
				transaction.Data = RandomString(20);
				Transactions.Add(transaction);

				System.Threading.Thread.Sleep(50);
			}
		}

		public static string RandomString(int length) {
			const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
			return new string(Enumerable.Repeat(chars, length)
				.Select(s => s[Random.Next(s.Length)]).ToArray());
		}
	}
}
