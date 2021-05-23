using System;
using System.Linq;
using NUnit.Framework;

namespace Sphere10.Framework.Tests.Environment {

	public class CommandLineArgsTests {

		private string[] Header { get; } = {
			"Unit Testing",
			"CommandLineArgsTest"
		};

		private string[] Footer { get; } = {
			"Footer"
		};


		[Test]
		public void NoCommandOrArgIsValid() {
			var args = new CommandLineArgs(Header,
				Footer,new [] { new CommandLineArgCommand("test", "test", Array.Empty<CommandLineArg>(), Array.Empty<CommandLineArgCommand>()) },
				new CommandLineArg[0]);

			var result = args.TryParse(Array.Empty<string>());
			
			Assert.IsTrue(result.Success);
			Assert.IsEmpty(result.Value.Commands);
			Assert.IsEmpty(result.Value.Arguments);
			
		}

		[Test]
		public void AtLeastOneCommandMustMatch() {
			var args = new CommandLineArgs(Header,
				Footer,new[] { new CommandLineArgCommand("test", "testing command", new CommandLineArg[0], new CommandLineArgCommand[0])},
				new CommandLineArg[0]);

			Result<CommandLineResults> result = args.TryParse(new[] { "test2", "--baz" });
			Assert.IsTrue(result.Failure);
		}

		[Test]
		public void FailMultipleCommandsMatched() {
			var args = new CommandLineArgs(Header,
				Footer,new[] { new CommandLineArgCommand("test", "testing command", new CommandLineArg[0], new CommandLineArgCommand[0])},
				new CommandLineArg[0]);

			Result<CommandLineResults> result = args.TryParse(new[] { "test", "test2", "--baz" });
			Assert.IsTrue(result.Failure);
		}

		[Test]
		public void MatchMultipleCommand() {
			var args = new CommandLineArgs(Header,
				Footer,new[] {
					new CommandLineArgCommand("test", "testing command", new CommandLineArg[0], new [] {
						new CommandLineArgCommand("test-2", "sub command", new [] {
							new CommandLineArg("foo", "baz description")
						}, new CommandLineArgCommand[0])
					}),
				},
				new CommandLineArg[0]);

			Result<CommandLineResults> result = args.TryParse(new[] { "test", "test-2", "--foo baz" });
			Assert.IsTrue(result.Success);
			
			
			Assert.AreEqual("test", result.Value.Commands.Single().Key);
			CommandLineResults testResult = result.Value.Commands["test"].Single();
			CommandLineResults test2Results = testResult.Commands["test-2"].Single();
			
			Assert.AreEqual("baz", test2Results.Arguments["foo"].Single());
		}

		[Test]
		public void CommandArgumentMandatory() {
			var args = new CommandLineArgs(Header,
				Footer,new[] {
					new CommandLineArgCommand("test", "testing command", new CommandLineArg[0], new [] {
						new CommandLineArgCommand("test-2", "sub command", new [] {
							new CommandLineArg("foo", "baz description", CommandLineArgTraits.Mandatory)
						}, new CommandLineArgCommand[0])
					}),
				},
				new CommandLineArg[0]);

			Result<CommandLineResults> result = args.TryParse(new[] { "test", "test-2", "--fo0 baz" });
			Assert.IsTrue(result.Failure);
		}

		[Test]
		public void CommandOnly() {
			var args = new CommandLineArgs(Header,
				Footer,new[] {
					new CommandLineArgCommand("test", "testing command", new CommandLineArg[0], new [] {
						new CommandLineArgCommand("test-2", "sub command", new [] {
							new CommandLineArg("foo", "baz description", CommandLineArgTraits.Optional)
						}, new CommandLineArgCommand[0])
					}),
				},
				new CommandLineArg[0]);

			Result<CommandLineResults> result = args.TryParse(new[] { "test" });
			Assert.IsTrue(result.Success);
		}
		

		[Test]
		[TestCase(CommandLineArgOptions.ForwardSlash)]
		[TestCase(CommandLineArgOptions.SingleDash)]
		[TestCase(CommandLineArgOptions.DoubleDash)]
		public void ArgNameMatchOptions(CommandLineArgOptions options) {
			var args = new CommandLineArgs(Header,
				Footer,new[] { new CommandLineArgCommand("test", "testing command", new CommandLineArg[0], new CommandLineArgCommand[0])},
				new CommandLineArg[] { new("test", "test", CommandLineArgTraits.Mandatory) },
				options);

			var parsedSpace = args.TryParse(new[] {
				"test",
					"/test test",
					"-test test",
					"--test test"
				});

			var parsedEquals = args.TryParse(new[] {
				"test",
					"/test=test",
					"-test=test",
					"--test=test"
				});

			Assert.IsTrue(parsedSpace.Success);
			Assert.AreEqual(1, parsedSpace.Value.Arguments["test"].Count());
			Assert.AreEqual("test", parsedSpace.Value.Arguments["test"].Single());

			Assert.IsTrue(parsedEquals.Success);
			Assert.AreEqual(1, parsedEquals.Value.Arguments["test"].Count());
			Assert.AreEqual("test", parsedEquals.Value.Arguments["test"].Single());
		}

		[Test]
		public void ArgTraitMulti() {
			var args = new CommandLineArgs(
				Header,
				Footer,new[] { new CommandLineArgCommand("test", "testing command", new CommandLineArg[0], new CommandLineArgCommand[0])},
				new CommandLineArg[] { new("multi", "multiple trait", CommandLineArgTraits.Multiple) },
				CommandLineArgOptions.DoubleDash | CommandLineArgOptions.SingleDash | CommandLineArgOptions.ForwardSlash);

			var parsed = args.TryParse(new[] {
				"test",
					"/multi a",
					"-multi b",
					"--multi c"
				});

			Assert.IsTrue(parsed.Success);
			Assert.AreEqual(new[] { "a", "b", "c" }, parsed.Value.Arguments["multi"]);
		}

		[Test]
		public void ArgTraitSingle() {
			var args = new CommandLineArgs(
				Header,
				Footer,new[] { new CommandLineArgCommand("test", "testing command", new CommandLineArg[0], new CommandLineArgCommand[0])},
				new CommandLineArg[] { new("single", "single trait") },
				CommandLineArgOptions.DoubleDash | CommandLineArgOptions.SingleDash | CommandLineArgOptions.ForwardSlash);

			var invalid = args.TryParse(new[] {
				"test",
					"/single a",
					"-single:b",
					"--single=c"
				});

			Assert.IsFalse(invalid.Success);
			Assert.AreEqual(1, invalid.ErrorMessages.Count());
			Assert.IsEmpty(invalid.Value.Arguments);

			var valid = args.TryParse(new[] {
				"test",
					"--single=c"
				});

			Assert.IsTrue(valid.Success);
			Assert.AreEqual("c", valid.Value.Arguments["single"].Single());
		}

		[Test]
		public void ArgTraitMandatory() {
			var args = new CommandLineArgs(
				Header,
				Footer,new[] { new CommandLineArgCommand("test", "testing command", new CommandLineArg[0], new CommandLineArgCommand[0])},
				new CommandLineArg[] {
					new("mandatory", "mandatory trait", CommandLineArgTraits.Mandatory)
				},
				CommandLineArgOptions.DoubleDash | CommandLineArgOptions.SingleDash | CommandLineArgOptions.ForwardSlash);

			var invalid = args.TryParse(new[] {
				"test",
					"/argumentA test",
				});

			Assert.IsFalse(invalid.Success);
			Assert.AreEqual(1, invalid.ErrorMessages.Count());
			Assert.IsEmpty(invalid.Value.Arguments);

			var valid = args.TryParse(new[] {
				"test",
					"/mandatory test",
				});

			Assert.IsTrue(valid.Success);
			Assert.AreEqual("test", valid.Value.Arguments["mandatory"].Single());
		}

		[Test]
		public void ArgCaseSensitive() {
			var args = new CommandLineArgs(
				Header,
				Footer,new[] { new CommandLineArgCommand("test", "testing command", new CommandLineArg[0], new CommandLineArgCommand[0])},
				new CommandLineArg[] {
					new("mAnDaToRy", "mandatory trait", CommandLineArgTraits.Mandatory)
				},
				CommandLineArgOptions.DoubleDash | CommandLineArgOptions.SingleDash | CommandLineArgOptions.ForwardSlash |
				CommandLineArgOptions.CaseSensitive);

			var invalid = args.TryParse(new[] {
				"test",
					"/mandatory test",
				});

			Assert.IsFalse(invalid.Success);
			Assert.AreEqual(1, invalid.ErrorMessages.Count());
			Assert.IsEmpty(invalid.Value.Arguments);

			var valid = args.TryParse(new[] {
				"test",
					"/mAnDaToRy test",
				});

			Assert.IsTrue(valid.Success);
			Assert.AreEqual("test", valid.Value.Arguments["mAnDaToRy"].Single());
		}

		[Test]
		public void SuccessParseH() {
			var args = new CommandLineArgs(
				Header,
				Footer,new[] { new CommandLineArgCommand("test", "testing command", new CommandLineArg[0], new CommandLineArgCommand[0])},
				new CommandLineArg[] {
					new("parameter", "mandatory", CommandLineArgTraits.Mandatory)
				},
				CommandLineArgOptions.DoubleDash | CommandLineArgOptions.SingleDash | CommandLineArgOptions.ForwardSlash |
				CommandLineArgOptions.PrintHelpOnH | CommandLineArgOptions.PrintHelpOnHelp);

			var help = args.TryParse(new[] {
				"test",
					"/parameter test",
					"--h"
				});

			Assert.IsTrue(help.Success);
			Assert.IsTrue(help.Value.HelpRequested);
		}

		[Test]
		public void SuccessParseHelp() {
			var args = new CommandLineArgs(
				Header,
				Footer,new[] { new CommandLineArgCommand("test", "testing command", new CommandLineArg[0], new CommandLineArgCommand[0])},
				new CommandLineArg[] {
					new("parameter", "mandatory", CommandLineArgTraits.Mandatory)
				},
				CommandLineArgOptions.DoubleDash | CommandLineArgOptions.SingleDash | CommandLineArgOptions.ForwardSlash |
				CommandLineArgOptions.PrintHelpOnH | CommandLineArgOptions.PrintHelpOnHelp);

			var help = args.TryParse(new[] {
				"test",
					"/parameter test",
					"--help"
				});

			Assert.IsTrue(help.Success);
			Assert.IsTrue(help.Value.HelpRequested);
		}

		[Test]
		public void ArgDependencies() {
			var args = new CommandLineArgs(
				Header,
				Footer,new[] { new CommandLineArgCommand("test", "testing command", new CommandLineArg[0], new CommandLineArgCommand[0])},
				new CommandLineArg[] {
					new("mandatoryWithDependency", "mandatory", CommandLineArgTraits.Mandatory, "test"),
					new("test", "optional", CommandLineArgTraits.Optional)
				},
				CommandLineArgOptions.DoubleDash | CommandLineArgOptions.SingleDash | CommandLineArgOptions.ForwardSlash |
				CommandLineArgOptions.PrintHelpOnH | CommandLineArgOptions.PrintHelpOnHelp);

			var invalid = args.TryParse(new[] {
				"test",
					"/mandatoryWithDependency test"
				});

			Assert.IsFalse(invalid.Success);
			Assert.AreEqual(1, invalid.ErrorMessages.Count());
			Assert.IsEmpty(invalid.Value.Arguments);

			var valid = args.TryParse(new[] {
				"test",
					"/mandatoryWithDependency test",
					"/test test"
				});

			Assert.IsTrue(valid.Success);
			Assert.IsEmpty(valid.ErrorMessages);
			Assert.AreEqual("test", valid.Value.Arguments["mandatoryWithDependency"].Single());
			Assert.AreEqual("test", valid.Value.Arguments["test"].Single());
		}

		[Test]
		public void ParseComplexArg() {
			string[] input = {
				"test",
				"-param1 value1",
				"--param2",
				"/param3:\"Test-:-work\"",
				"/param4=happy",
				"-param5 '--=nice=--'"
			};

			var args = new CommandLineArgs(
				Header,
				Footer,new[] { new CommandLineArgCommand("test", "testing command", new CommandLineArg[0], new CommandLineArgCommand[0])},
				new CommandLineArg[] {
					new("param1", "1", CommandLineArgTraits.Mandatory),
					new("param2", "2", CommandLineArgTraits.Mandatory),
					new("param3", "3", CommandLineArgTraits.Mandatory),
					new("param4", "4", CommandLineArgTraits.Mandatory),
					new("param5", "5", CommandLineArgTraits.Mandatory)
				});

			var parsed = args.TryParse(input);

			Assert.IsTrue(parsed.Success);
			Assert.AreEqual(5,parsed.Value.Arguments.Count());
			Assert.IsEmpty(parsed.ErrorMessages);
		}
	}
}
