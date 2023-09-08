// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Transactions;
using System.Xml.Serialization;
using Hydrogen.Data;
using Hydrogen.Windows.Forms;

namespace Hydrogen.Utils.WinFormsTester;

public partial class MiscTestScreen : ApplicationScreen {
	public MiscTestScreen() {
		InitializeComponent();
	}

	private void button1_Click(object sender, EventArgs e) {
		var objs = new[] { new TestObject { ID = 1, Name = "One" }, new TestObject { ID = 2, Name = "Two" }, new TestObject { ID = 3, Name = "Three" } };
		listMerger1.DisplayMember = "Name";
		listMerger1.ValueMember = "ID";
		listMerger1.LeftItems = objs;
	}


	public class TestObject {
		public int ID { get; set; }
		public string Name { get; set; }
	}


	private void _clipTestButton_Click(object sender, EventArgs e) {
		var teststring = "01234567890123456789012345678901234567890123456789";
		var writer = new TextBoxWriter(_outputTextBox);

		writer.WriteLine(teststring);
		for (int i = 0; i < teststring.Length + 10; i++) {
			writer.WriteLine(teststring.Clip(i));
		}
	}

	private void _scopedContextTestButton_Click(object sender, EventArgs e) {
		var writer = new TextBoxWriter(_outputTextBox);

		TestScopePolicy(writer, ContextScopePolicy.None, ContextScopePolicy.None);
		TestScopePolicy(writer, ContextScopePolicy.None, ContextScopePolicy.MustBeNested);
		TestScopePolicy(writer, ContextScopePolicy.None, ContextScopePolicy.MustBeRoot);

		TestScopePolicy(writer, ContextScopePolicy.MustBeNested, ContextScopePolicy.None);
		TestScopePolicy(writer, ContextScopePolicy.MustBeNested, ContextScopePolicy.MustBeNested);
		TestScopePolicy(writer, ContextScopePolicy.MustBeNested, ContextScopePolicy.MustBeRoot);

		TestScopePolicy(writer, ContextScopePolicy.MustBeRoot, ContextScopePolicy.None);
		TestScopePolicy(writer, ContextScopePolicy.MustBeRoot, ContextScopePolicy.MustBeNested);
		TestScopePolicy(writer, ContextScopePolicy.MustBeRoot, ContextScopePolicy.MustBeRoot);

	}

	private static void TestScopePolicy(TextBoxWriter writer, ContextScopePolicy rootPolicy, ContextScopePolicy childPolicy) {
		try {
			ScopeContextTest.TextWriter = writer;
			writer.WriteLine("Testing Scoped Context: RootPolicy={0}, ChildPolicy={1}", rootPolicy, childPolicy);
			using (new ScopeContextTest(rootPolicy)) {
				using (new ScopeContextTest(childPolicy)) {
					using (new ScopeContextTest(childPolicy)) {
					}
				}
			}
		} catch (Exception error) {
			writer.WriteLine("Exception: {0}", error.ToDisplayString());
		} finally {
			writer.WriteLine();
			writer.WriteLine();
		}
	}


	public class ScopeContextTest : SyncContextScope {
		public static TextWriter TextWriter;

		public ScopeContextTest(ContextScopePolicy policy) : base(policy, "myTlsSlotName") {
			if (IsRootScope) {
				TextWriter.WriteLine("[Begin Root Scope]");

			} else {
				TextWriter.WriteLine("[Begin Child Scope]");
			}
		}

		protected override void OnScopeEndInternal() {
			TextWriter.WriteLine(RootScope != this ? "[End Child Scope]" : "[End Root Scope]");
		}

		protected override void OnContextEnd() {
			TextWriter.WriteLine("Context end");
		}

	}


	private void _compressTestButton_Click(object sender, EventArgs e) {
		var writer = new TextBoxWriter(_outputTextBox);
		var seedString =
			"Hello world! This is a compressed string which contains some text that is used to compress and encrypt data. It is important that compression then encryption results in a smaller file than encryption alone. Hopefully this will show that indeed this is the case. Good luck!";
		for (int i = 0; i < 1000; i++) {
			var stringBuilder = new StringBuilder();
			stringBuilder.Append(seedString);
			for (int j = 0; j < i; j++)
				stringBuilder.Append(Tools.Maths.RNG.Next(0, 10000));
			var testString = stringBuilder.ToString();
			var compressedEncrypted = Tools.Text.CompressText(testString, "password");
			var compressed = Tools.Text.CompressText(testString);
			var encryptedOnly = Tools.Crypto.EncryptStringAES(testString, "password", "asdfasasdfasdfasdfasdfasdfasdf");
			writer.WriteLine("Orignal Size:{0}  Compressed:{1}  CompressedEncypted:{2} Encrypted:{3} Uncompressing worked: {4}",
				testString.Length,
				compressed.Length,
				compressedEncrypted.Length,
				encryptedOnly.Length,
				testString == Tools.Text.DecompressText(compressed) && testString == Tools.Text.DecompressText(compressedEncrypted, "password"));
		}
	}

	private void _pathTemplatesButton_Click(object sender, EventArgs e) {
		var writer = new TextBoxWriter(_outputTextBox);
		try {
			var templates = new[] { "{Personal}{/}Alpha{/}Beta{/}Gamma" };

			foreach (var template in templates) {
				writer.Write(template);
				writer.Write(" -> ");
				writer.WriteLine(Tools.FileSystem.ResolvePathTemplate(template));
			}
		} catch (Exception error) {
			writer.WriteLine(error.ToDiagnosticString());
		}
	}

	private void _sqliteTestButton_Click(object sender, EventArgs e) {
		var writer = new TextBoxWriter(_outputTextBox);
		try {

			for (var i = 100; i <= 10000; i = i * 10) {

				var db = CreateTempSqliteDB();
				var start = DateTime.Now;
				SqliteDACInsertTest(db, i);
				writer.WriteLine("{0} unbatched inserts no transaction = {1}", i, DateTime.Now.Subtract(start).TotalMilliseconds);
				File.Delete(db);

				db = CreateTempSqliteDB();
				start = DateTime.Now;
				SqliteDACExecuteBatch(db, i);
				writer.WriteLine("{0} batched inserts no transaction = {1}", i, DateTime.Now.Subtract(start).TotalMilliseconds);
				File.Delete(db);

				db = CreateTempSqliteDB();
				start = DateTime.Now;
				SqliteDACExecuteBatch2(db, i);
				writer.WriteLine("{0} batched inserts 2 no transaction = {1}", i, DateTime.Now.Subtract(start).TotalMilliseconds);
				File.Delete(db);

				db = CreateTempSqliteDB();
				using (var t = new TransactionScope()) {

					start = DateTime.Now;
					SqliteDACInsertTest(db, i);
					t.Complete();
					writer.WriteLine("{0} unbatched inserts with transaction = {1}", i, DateTime.Now.Subtract(start).TotalMilliseconds);
				}
				File.Delete(db);

				db = CreateTempSqliteDB();
				using (var t = new TransactionScope()) {
					start = DateTime.Now;
					SqliteDACExecuteBatch(db, i);
					t.Complete();
					writer.WriteLine("{0} optimized batched inserts with transaction = {1}", i, DateTime.Now.Subtract(start).TotalMilliseconds);
				}
				File.Delete(db);

				db = CreateTempSqliteDB();
				using (var t = new TransactionScope()) {
					start = DateTime.Now;
					SqliteDACExecuteBatch2(db, i);
					t.Complete();
					writer.WriteLine("{0} standard batched inserts with transaction = {1}", i, DateTime.Now.Subtract(start).TotalMilliseconds);
				}
				File.Delete(db);
			}


		} catch (Exception error) {
			writer.WriteLine(error.ToDiagnosticString());
		}
	}

	private void button2_Click(object sender, EventArgs e) {
		var writer = new TextBoxWriter(_outputTextBox);
		try {

			for (var i = 100; i <= 10000; i = i * 10) {

				DateTime start;
				var db = CreateTempSqliteDB();
				using (var t = new TransactionScope()) {

					start = DateTime.Now;
					SqliteDACInsertTest(db, i);
					t.Complete();
					writer.WriteLine("{0} unbatched inserts with transaction = {1}", i, DateTime.Now.Subtract(start).TotalMilliseconds);
				}
				File.Delete(db);

				db = CreateTempSqliteDB();
				using (var t = new TransactionScope()) {
					start = DateTime.Now;
					SqliteDACExecuteBatch(db, i);
					t.Complete();
					writer.WriteLine("{0} optimized batched inserts with transaction = {1}", i, DateTime.Now.Subtract(start).TotalMilliseconds);
				}
				File.Delete(db);

				db = CreateTempSqliteDB();
				start = DateTime.Now;
				SqliteDACExecuteBatchManualTransaction(db, i);
				writer.WriteLine("{0} optimized batched inserts with manual transaction = {1}", i, DateTime.Now.Subtract(start).TotalMilliseconds);
				File.Delete(db);

				db = CreateTempSqliteDB();
				using (var t = new TransactionScope()) {
					start = DateTime.Now;
					SqliteDACExecuteBatch2(db, i);
					t.Complete();
					writer.WriteLine("{0} standard batched inserts with transaction = {1}", i, DateTime.Now.Subtract(start).TotalMilliseconds);
				}
				File.Delete(db);
			}


		} catch (Exception error) {
			writer.WriteLine(error.ToDiagnosticString());
		}
	}


	private void _sqlServerTest_Click(object sender, EventArgs e) {
		var writer = new TextBoxWriter(_outputTextBox);
		try {

			for (var i = 100; i <= 10000; i = i * 10) {

				DateTime start;
				var db = CreateTempMSSQLDB();
				using (var t = new TransactionScope()) {

					start = DateTime.Now;
					SqlServerDACInsertTest(db, i);
					t.Complete();
					writer.WriteLine("{0} unbatched inserts with transaction = {1}", i, DateTime.Now.Subtract(start).TotalMilliseconds);
				}
				Tools.MSSQL.DropDatabase("localhost", db, "sa", null, false);


				db = CreateTempMSSQLDB();
				using (var t = new TransactionScope()) {
					start = DateTime.Now;
					SqlServerDACExecuteBatch(db, i);
					t.Complete();
					writer.WriteLine("{0} optimized batched inserts with transaction = {1}", i, DateTime.Now.Subtract(start).TotalMilliseconds);
				}
				Tools.MSSQL.DropDatabase("localhost", db, "sa", null, false);

				db = CreateTempMSSQLDB();
				start = DateTime.Now;
				SqlServerDACExecuteBatchManualTransaction(db, i);
				writer.WriteLine("{0} optimized batched inserts with manual transaction = {1}", i, DateTime.Now.Subtract(start).TotalMilliseconds);
				Tools.MSSQL.DropDatabase("localhost", db, "sa", null, false);

				db = CreateTempMSSQLDB();
				using (var t = new TransactionScope()) {
					start = DateTime.Now;
					SqlServerDACExecuteBatch2(db, i);
					t.Complete();
					writer.WriteLine("{0} standard batched inserts with transaction = {1}", i, DateTime.Now.Subtract(start).TotalMilliseconds);
				}
				Tools.MSSQL.DropDatabase("localhost", db, "sa", null, false);
			}


		} catch (Exception error) {
			writer.WriteLine(error.ToDiagnosticString());
		}
	}


	private async void _systemPathsButton_Click(object sender, EventArgs e) {
		var writer = new TextBoxWriter(_outputTextBox);
		try {
			foreach (var specialFolder in Tools.Enums.GetValues<Environment.SpecialFolder>())
				await writer.WriteLineAsync($"{specialFolder} -> {Environment.GetFolderPath(specialFolder)}");
		} catch (Exception error) {
			writer.WriteLine(error.ToDiagnosticString());
		}
	}


	#region Sqlite

	public string CreateTempSqliteDB() {
		var table = "CREATE TABLE IF NOT EXISTS TTC (id INTEGER PRIMARY KEY, Route_ID TEXT, Branch_Code TEXT, Version INTEGER, Stop INTEGER, Vehicle_Index INTEGER, Day Integer, Time TEXT)";
		var tempFile = string.Format("{0}{1}{2}", Path.GetTempPath(), Path.DirectorySeparatorChar, Guid.NewGuid());
		var dac = Tools.Sqlite.Create(tempFile);
		using (dac.BeginScope()) {
			dac.ExecuteNonQuery(table);
		}
		return tempFile;
	}

	public void SqliteDACInsertTest(string path, int numInserts) {
		var dac = Tools.Sqlite.Open(path);
		using (dac.BeginScope(true)) {
			for (var i = 0; i < numInserts; i++)
				dac.Insert("TTC", new[] { new ColumnValue("Route_ID", Guid.NewGuid().ToString()), });
		}
	}

	public void SqliteDACExecuteBatch(string path, int numInserts) {
		var dac = Tools.Sqlite.Open(path);
		using (dac.BeginScope()) {
			var builder = dac.CreateSQLBuilder();
			for (var i = 0; i < numInserts; i++)
				builder.Insert("TTC", new[] { new ColumnValue("Route_ID", Guid.NewGuid().ToString()), });
			dac.ExecuteBatch(builder);
		}
	}

	public void SqliteDACExecuteBatch2(string path, int numInserts) {
		var dac = Tools.Sqlite.Open(path);
		using (dac.BeginScope()) {
			var builder = dac.CreateSQLBuilder();
			for (int i = 0; i < numInserts; i++)
				builder.Insert("TTC", new[] { new ColumnValue("Route_ID", Guid.NewGuid().ToString()), });

			dac.ExecuteNonQuery(builder.ToString());
		}
	}


	public void SqliteDACExecuteBatchManualTransaction(string path, int numInserts) {
		var dac = Tools.Sqlite.Open(path);
		using (dac.BeginScope()) {
			var builder = dac.CreateSQLBuilder();
			builder.BeginTransaction();
			for (int i = 0; i < numInserts; i++)
				builder.Insert("TTC", new[] { new ColumnValue("Route_ID", Guid.NewGuid().ToString()), });
			builder.CommitTransaction();
			dac.ExecuteBatch(builder);
		}
	}

	#endregion


	#region SQL Server

	public string CreateTempMSSQLDB() {
		var table = "CREATE TABLE TTC (id INTEGER PRIMARY KEY IDENTITY, Route_ID VARCHAR(255), Branch_Code VARCHAR(255), Version INTEGER, Stop INTEGER, Vehicle_Index INTEGER, Day Integer, Time VARCHAR(255))";
		var dbName = Guid.NewGuid().ToStrictAlphaString();
		Tools.MSSQL.CreateDatabase("localhost", dbName, "sa", null, false);
		var dac = Tools.MSSQL.Open("localhost", dbName, "sa", null);
		using (dac.BeginScope()) {
			dac.ExecuteNonQuery(table);
		}
		return dbName;
	}

	public void SqlServerDACInsertTest(string dbName, int numInserts) {
		var dac = Tools.MSSQL.Open("localhost", dbName, "sa", null);
		;
		using (dac.BeginScope(true)) {
			for (var i = 0; i < numInserts; i++)
				dac.Insert("TTC", new[] { new ColumnValue("Route_ID", Guid.NewGuid().ToString()), });
		}
	}

	public void SqlServerDACExecuteBatch(string dbName, int numInserts) {
		var dac = Tools.MSSQL.Open("localhost", dbName, "sa", null);
		using (dac.BeginScope()) {
			var builder = dac.CreateSQLBuilder();
			for (var i = 0; i < numInserts; i++)
				builder.Insert("TTC", new[] { new ColumnValue("Route_ID", Guid.NewGuid().ToString()), });
			dac.ExecuteBatch(builder);
		}
	}

	public void SqlServerDACExecuteBatch2(string dbName, int numInserts) {
		var dac = Tools.MSSQL.Open("localhost", dbName, "sa", null);
		using (dac.BeginScope()) {
			var builder = dac.CreateSQLBuilder();
			for (int i = 0; i < numInserts; i++)
				builder.Insert("TTC", new[] { new ColumnValue("Route_ID", Guid.NewGuid().ToString()), });

			dac.ExecuteNonQuery(builder.ToString());
		}
	}


	public void SqlServerDACExecuteBatchManualTransaction(string dbName, int numInserts) {
		var dac = Tools.MSSQL.Open("localhost", dbName, "sa", null);
		using (dac.BeginScope()) {
			var builder = dac.CreateSQLBuilder();
			builder.BeginTransaction();
			for (int i = 0; i < numInserts; i++)
				builder.Insert("TTC", new[] { new ColumnValue("Route_ID", Guid.NewGuid().ToString()), });
			builder.CommitTransaction();
			dac.ExecuteBatch(builder);
		}
	}

	#endregion

	private async void button3_Click(object sender, EventArgs e) {
		try {
			//var writer = new TextBoxWriter(_clipTestTextBox);
			//var shortnedUrl = await Hydrogen.UrlShortner.GoogleAsync("https://sphere10.com", "AIzaSyCt4dKG_UgO8qeusgM0BtrxB1Pb_-7KKDw");
			//writer.WriteLine("TinyURL: " + shortnedUrl);
			var xxx =
				@"<?xml version=""1.0"" encoding=""utf-8""?>
<object name="""" type=""TK0"" assembly="""">
  <!-- Data section : Don't edit any attributes ! -->
  <properties>
    <property name=""Keys"" type=""TK1"" assembly="""">
      <items />
    </property>
    <property name=""Values"" type=""TK1"" assembly="""">
      <items />
    </property>
  </properties>
  <!-- TypeDictionary : Don't edit anything in this section at all ! -->
  <typedictionary name="""" type=""System.Collections.Hashtable"" assembly=""mscorlib, Version=2.0.5.0, Culture=neutral, PublicKeyToken=7cec85d7bea7798e"">
    <items>
      <item>
        <properties />
      </item>
      <item>
        <properties />
      </item>
    </items>
  </typedictionary>
</object>";
			var x = new XmlDeepDeserializer();
			x.Deserialize(new StringReader(xxx));
		} catch (Exception error) {
			ExceptionDialog.Show(this, error);
		}
	}


	private async void _macAddressesButton_Click(object sender, EventArgs e) {
		try {
			var writer = new TextBoxWriter(_outputTextBox);
			foreach (var macAddr in Tools.Network.GetMacAddresses())
				await writer.WriteLineAsync(macAddr);
		} catch (Exception error) {
			ExceptionDialog.Show(this, error);
		}
	}


	[XmlRoot("Menu")]
	public class Menu {

		public Menu() {
			SubMenus = new Menu[0];
			Url = "#";
		}

		public Menu(string text, string url, string glyph = null) : this() {
			Text = text;
			Url = url;
			Glyph = glyph;
		}


		[XmlAttribute("text")] public string Text { get; set; }

		[XmlAttribute("url")] public string Url { get; set; }

		[XmlAttribute("open")] public bool Open { get; set; }

		[XmlAttribute("icon")] public string FontAwesomeIcon { get; set; }

		[XmlAttribute("glyph")] public string Glyph { get; set; }

		[XmlElement("Menu")] public Menu[] SubMenus { get; set; }

	}


	public class SideBarModelBuilder {

		public Menu CraeteDefaultSideBarModel() {
			return new Menu {
				SubMenus =
					new[] { CreateDashboardMenu() }
						.Concat(CreateAssetsMenu())
						.Concat(CreateUsersMenu())
						.Concat(CreateAccountsMenu())
						.Concat(CreateTransactionsMenu())
						.Concat(CreateCardMenu())
						.Concat(CreateTransformersMenu())
						.Concat(CreateAuditMenu())
						.Concat(CreateReportsMenu())
						.Concat(CreateConfigurationMenu())
						.ToArray()
			};
		}

		protected Menu CreateDashboardMenu() {
			return new Menu {
				Text = "Dashboard",
				FontAwesomeIcon = "fa-home",
				Url = "/Dashboard"
			};
		}

		protected Menu CreateAssetsMenu() {
			return new Menu {
				Text = "Assets",
				FontAwesomeIcon = "fa-btc",
				SubMenus = new[] {
					new Menu("Asset Types", "/Assets"),
					new Menu("Issue Asset", "/Assets/Issue"),
				}
			};
		}

		protected Menu CreateUsersMenu() {
			return new Menu {
				Text = "Users",
				FontAwesomeIcon = "fa-user",
				SubMenus = new[] {
					new Menu("Persons", "/Persons"),
					new Menu("{Agents}", "/Agents"),
					new Menu("Auditors", "/Auditors"),
				}
			};
		}

		protected Menu CreateAccountsMenu() {
			return new Menu {
				Text = "Accounts",
				FontAwesomeIcon = "fa-money",
				SubMenus = new[] {
					new Menu("Accounts", "/Accounts"),
					new Menu("Endpoints", "/Endpoints"),
					new Menu("Transactions", "/Transactions"),
				}
			};
		}


		protected Menu CreateTransactionsMenu() {
			return new Menu {
				Text = "Transactions",
				FontAwesomeIcon = "fa-money",
				SubMenus = new[] {
					new Menu("Transactions", "/Transactions"),
					new Menu("Redeem Codes", "/RedeemCodes"),
				}
			};
		}


		protected Menu CreateCardMenu() {
			return new Menu {
				Text = "Cards",
				FontAwesomeIcon = "fa-credit-card",
				SubMenus = new[] {
					new Menu("View Cards", "/Cards"),
					new Menu("New Card", "/Cards/New"),
				}
			};
		}

		protected Menu CreateTransformersMenu() {
			return new Menu {
				Text = "Transformers",
				FontAwesomeIcon = "fa-balance-scale",
				SubMenus = new[] {
					new Menu("View Transformers", "/Transformers"),
					new Menu("New Transformer", "/Transformers/New"),
				}
			};
		}

		protected Menu CreateAuditMenu() {
			return new Menu {
				Text = "Audit",
				FontAwesomeIcon = "fa-shield",
				SubMenus = new[] {
					new Menu("Audits", "/Audits"),
				}
			};
		}

		protected Menu CreateReportsMenu() {
			return new Menu {
				Text = "Reports",
				FontAwesomeIcon = "fa-area-chart",
				SubMenus = new[] {
					new Menu("AML Compliance", "/Reports/AML"),
					new Menu("Agent Reconcilliation", "/Reports/AgentReconcilliation"),
				}
			};
		}

		protected Menu CreateConfigurationMenu() {
			return new Menu {
				Text = "Configuration",
				FontAwesomeIcon = "fa-gear",
				SubMenus = new[] {
					new Menu("Settings", "/Settings"),
					new Menu("Blockchain Engine", "/Engine"),
					new Menu("Communications", "/Communications")
				}
			};
		}
	}

	private void _mappingTests_Click(object sender, EventArgs e) {
		try {

			var _writer = new TextBoxWriter(_outputTextBox);


			var member = Tools.Mapping.GetMember<TestClass>(x => x.Name);

			_writer.WriteLine(member);
		} catch (Exception error) {
			ExceptionDialog.Show(this, error);
		}
	}
}
