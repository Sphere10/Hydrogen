//using System.Linq;
//using NUnit.Framework;

//namespace Sphere10.Framework.Tests.Environment {

//	public class CommandLineArgsTests {

//		private string[] Header { get; } = {
//			"Unit Testing",
//			"CommandLineArgsTest"
//		};

//		private string[] Footer { get; } = {
//			"Footer"
//		};

//		[Test]
//		[TestCase(CommandLineArgOptions.ForwardSlash)]
//		[TestCase(CommandLineArgOptions.SingleDash)]
//		[TestCase(CommandLineArgOptions.DoubleDash)]
//		public void ArgNameMatchOptions(CommandLineArgOptions options) {
//			var args = new CommandLineArgs(Header,
//				Footer,
//				new CommandLineArg[] { new("test", "test", CommandLineArgTraits.Mandatory) },
//				options);

//			var parsedSpace = args.TryParse(new[] {
//					"/test test",
//					"-test test",
//					"--test test"
//				},
//				out var results,
//				out var messages);

//			var parsedEquals = args.TryParse(new[] {
//					"/test=test",
//					"-test=test",
//					"--test=test"
//				},
//				out var results2,
//				out var messages2);

//			Assert.IsTrue(parsedSpace);
//			Assert.AreEqual(1, results["test"].Count());
//			Assert.AreEqual("test", results["test"].Single());

//			Assert.IsTrue(parsedEquals);
//			Assert.AreEqual(1, results2["test"].Count());
//			Assert.AreEqual("test", results2["test"].Single());
//		}

//		[Test]
//		public void ArgTraitMulti() {
//			var args = new CommandLineArgs(
//				Header,
//				Footer,
//				new CommandLineArg[] { new("multi", "multiple trait", CommandLineArgTraits.Multiple) },
//				CommandLineArgOptions.DoubleDash | CommandLineArgOptions.SingleDash | CommandLineArgOptions.ForwardSlash);

//			var parsed = args.TryParse(new[] {
//					"/multi a",
//					"-multi b",
//					"--multi c"
//				},
//				out var results,
//				out var messages);

//			Assert.IsTrue(parsed);
//			Assert.AreEqual(new[] { "a", "b", "c" }, results["multi"]);
//		}

//		[Test]
//		public void ArgTraitSingle() {
//			var args = new CommandLineArgs(
//				Header,
//				Footer,
//				new CommandLineArg[] { new("single", "single trait") },
//				CommandLineArgOptions.DoubleDash | CommandLineArgOptions.SingleDash | CommandLineArgOptions.ForwardSlash);

//			var invalid = args.TryParse(new[] {
//					"/single a",
//					"-single:b",
//					"--single=c"
//				},
//				out var results,
//				out var messages);

//			Assert.IsFalse(invalid);
//			Assert.AreEqual(1, messages.Length);
//			Assert.IsEmpty(results);

//			var valid = args.TryParse(new[] {
//					"--single=c"
//				},
//				out var results2,
//				out var messages2);

//			Assert.IsTrue(valid);
//			Assert.IsEmpty(messages2);
//			Assert.AreEqual("c", results2["single"].Single());
//		}

//		[Test]
//		public void ArgTraitMandatory() {
//			var args = new CommandLineArgs(
//				Header,
//				Footer,
//				new CommandLineArg[] {
//					new("mandatory", "mandatory trait", CommandLineArgTraits.Mandatory)
//				},
//				CommandLineArgOptions.DoubleDash | CommandLineArgOptions.SingleDash | CommandLineArgOptions.ForwardSlash);

//			var invalid = args.TryParse(new[] {
//					"/argumentA test",
//				},
//				out var results,
//				out var messages);

//			Assert.IsFalse(invalid);
//			Assert.AreEqual(1, messages.Length);
//			Assert.IsEmpty(results);

//			var valid = args.TryParse(new[] {
//					"/mandatory test",
//				},
//				out var results2,
//				out var messages2);

//			Assert.IsTrue(valid);
//			Assert.IsEmpty(messages2);
//			Assert.AreEqual("test", results2["mandatory"].Single());
//		}

//		[Test]
//		public void ArgCaseSensitive() {
//			var args = new CommandLineArgs(
//				Header,
//				Footer,
//				new CommandLineArg[] {
//					new("mAnDaToRy", "mandatory trait", CommandLineArgTraits.Mandatory)
//				},
//				CommandLineArgOptions.DoubleDash | CommandLineArgOptions.SingleDash | CommandLineArgOptions.ForwardSlash |
//				CommandLineArgOptions.CaseSensitive);

//			var invalid = args.TryParse(new[] {
//					"/mandatory test",
//				},
//				out var results,
//				out var messages);

//			Assert.IsFalse(invalid);
//			Assert.AreEqual(1, messages.Length);
//			Assert.IsEmpty(results);

//			var valid = args.TryParse(new[] {
//					"/mAnDaToRy test",
//				},
//				out var results2,
//				out var messages2);

//			Assert.IsTrue(valid);
//			Assert.IsEmpty(messages2);
//			Assert.AreEqual("test", results2["mAnDaToRy"].Single());
//		}

//		[Test]
//		public void NoParseOnH() {
//			var args = new CommandLineArgs(
//				Header,
//				Footer,
//				new CommandLineArg[] {
//					new("parameter", "mandatory", CommandLineArgTraits.Mandatory)
//				},
//				CommandLineArgOptions.DoubleDash | CommandLineArgOptions.SingleDash | CommandLineArgOptions.ForwardSlash |
//				CommandLineArgOptions.PrintHelpOnH | CommandLineArgOptions.PrintHelpOnHelp);

//			var help = args.TryParse(new[] {
//					"/parameter test",
//					"--h"
//				},
//				out var results,
//				out var messages);

//			Assert.IsFalse(help);
//			Assert.IsEmpty(messages);
//			Assert.IsEmpty(results);
//		}

//		[Test]
//		public void NoParseOnHelp() {
//			var args = new CommandLineArgs(
//				Header,
//				Footer,
//				new CommandLineArg[] {
//					new("parameter", "mandatory", CommandLineArgTraits.Mandatory)
//				},
//				CommandLineArgOptions.DoubleDash | CommandLineArgOptions.SingleDash | CommandLineArgOptions.ForwardSlash |
//				CommandLineArgOptions.PrintHelpOnH | CommandLineArgOptions.PrintHelpOnHelp);

//			var help = args.TryParse(new[] {
//					"/parameter test",
//					"--help"
//				},
//				out var results,
//				out var messages);

//			Assert.IsFalse(help);
//			Assert.IsEmpty(messages);
//			Assert.IsEmpty(results);
//		}

//		[Test]
//		public void ArgDependencies() {
//			var args = new CommandLineArgs(
//				Header,
//				Footer,
//				new CommandLineArg[] {
//					new("mandatoryWithDependency", "mandatory", CommandLineArgTraits.Mandatory, "test"),
//					new("test", "optional", CommandLineArgTraits.Optional)
//				},
//				CommandLineArgOptions.DoubleDash | CommandLineArgOptions.SingleDash | CommandLineArgOptions.ForwardSlash |
//				CommandLineArgOptions.PrintHelpOnH | CommandLineArgOptions.PrintHelpOnHelp);

//			var invalid = args.TryParse(new[] {
//					"/mandatoryWithDependency test"
//				},
//				out var results,
//				out var messages);

//			Assert.IsFalse(invalid);
//			Assert.AreEqual(1, messages.Length);
//			Assert.IsEmpty(results);

//			var valid = args.TryParse(new[] {
//					"/mandatoryWithDependency test",
//					"/test test"
//				},
//				out var results2,
//				out var messages2);

//			Assert.IsTrue(valid);
//			Assert.IsEmpty(messages2);
//			Assert.AreEqual("test", results2["mandatoryWithDependency"].Single());
//			Assert.AreEqual("test", results2["test"].Single());
//		}

//		[Test]
//		public void ParseComplexArg() {
//			string[] input = {
//				"-param1 value1",
//				"--param2",
//				"/param3:\"Test-:-work\"",
//				"/param4=happy",
//				"-param5 '--=nice=--'"
//			};

//			var args = new CommandLineArgs(
//				Header,
//				Footer,
//				new CommandLineArg[] {
//					new("param1", "1", CommandLineArgTraits.Mandatory),
//					new("param2", "2", CommandLineArgTraits.Mandatory),
//					new("param3", "3", CommandLineArgTraits.Mandatory),
//					new("param4", "4", CommandLineArgTraits.Mandatory),
//					new("param5", "5", CommandLineArgTraits.Mandatory)
//				});

//			var parsed = args.TryParse(input, out var results, out var messages);

//			Assert.IsTrue(parsed);
//			Assert.AreEqual(5, results.Count());
//			Assert.IsEmpty(messages);
//		}
//	}
//}
