using System;
using System.Linq;
using NUnit.Framework;

namespace Sphere10.Framework.Tests.Environment {
	[TestFixture]
	[Parallelizable(ParallelScope.Children)]
	public class CommandLineArgsTests {
		private string[] Header { get; } =
		{
			"Unit Testing",
			"CommandLineArgsTest"
		};

		private string[] Footer { get; } =
		{
			"Footer"
		};

		[Test]
		public void Basic() {
			var args = new CommandLineParameters() {
				Parameters = new[]
					{new CommandLineParameter("read", "unit testing", CommandLineParameterOptions.Mandatory)},
			};

			var result = args.TryParseArguments(new string[] { "--read", "12345" });
			Assert.IsTrue(result.Success);
			Assert.IsEmpty(result.Value.Commands);
			Assert.AreEqual("12345", result.Value.Arguments["read"].Single());
		}

		[Test]
		public void DefaultConstructorParse() {
			var args = new CommandLineParameters();
			var result = args.TryParseArguments(Array.Empty<string>());
			Assert.IsTrue(result.Success);
		}

		[Test]
		public void InitArgParse() {
			var args = new CommandLineParameters() {
				Parameters = new[]
					{new CommandLineParameter("test", "unit testing", CommandLineParameterOptions.Mandatory)},
				Commands = new[] { new CommandLineCommand("unittest", "unit testing command") }
			};

			var result = args.TryParseArguments(new string[] { "unittest", "--test", "valid" });
			Assert.IsTrue(result.Success);
			Assert.Contains("unittest", result.Value.Commands.Keys.ToArray());
			Assert.AreEqual("valid", result.Value.Arguments["test"].Single());
		}

		[Test]
		public void NoCommandOrArgIsValid() {
			var args = new CommandLineParameters(Header,
				Footer, new CommandLineParameter[0],
				new[]
				{
					new CommandLineCommand("test", "test", Array.Empty<CommandLineParameter>(),
						Array.Empty<CommandLineCommand>())
				});

			var result = args.TryParseArguments(Array.Empty<string>());

			Assert.IsTrue(result.Success);
			Assert.IsEmpty(result.Value.Commands);
			Assert.IsEmpty(result.Value.Arguments);
		}

		[Test]
		public void AtLeastOneCommandMustMatch() {
			var args = new CommandLineParameters(Header,
				Footer, new CommandLineParameter[0],
				new[]
				{
					new CommandLineCommand("test", "testing command", new CommandLineParameter[0],
						new CommandLineCommand[0])
				});

			Result<CommandLineResults> result = args.TryParseArguments(new[] { "test2", "--baz" });
			Assert.IsTrue(result.Failure);
		}

		[Test]
		public void FailMultipleCommandsMatched() {
			var args = new CommandLineParameters(Header,
				Footer, new CommandLineParameter[0],
				new[]
				{
					new CommandLineCommand("test", "testing command", new CommandLineParameter[0],
						new CommandLineCommand[0])
				});

			Result<CommandLineResults> result = args.TryParseArguments(new[] { "test", "test2", "--baz" });
			Assert.IsTrue(result.Failure);
		}

		[Test]
		public void MatchMultipleCommand() {
			var args = new CommandLineParameters(Header,
				Footer, new CommandLineParameter[0],
				new[]
				{
					new CommandLineCommand("test", "testing command", new CommandLineParameter[0], new[]
					{
						new CommandLineCommand("test-2", "sub command", new[]
						{
							new CommandLineParameter("foo", "baz description")
						}, new CommandLineCommand[0])
					}),
				});

			Result<CommandLineResults> result = args.TryParseArguments(new[] { "test", "test-2", "--foo baz" });
			Assert.IsTrue(result.Success);
			Assert.AreEqual("test", result.Value.Commands.Single().Key);
			CommandLineResults testResult = result.Value.Commands["test"];
			CommandLineResults test2Results = testResult.Commands["test-2"];

			Assert.AreEqual("baz", test2Results.Arguments["foo"].Single());
		}

		[Test]
		public void CommandArgumentMandatory() {
			var args = new CommandLineParameters(Header,
				Footer, new CommandLineParameter[0],
				new[]
				{
					new CommandLineCommand("test", "testing command", new CommandLineParameter[0], new[]
					{
						new CommandLineCommand("test-2", "sub command", new[]
						{
							new CommandLineParameter("foo", "baz description", CommandLineParameterOptions.Mandatory)
						}, new CommandLineCommand[0])
					}),
				});

			Result<CommandLineResults> result = args.TryParseArguments(new[] { "test", "test-2", "--fo0 baz" });
			Assert.IsTrue(result.Failure);
		}

		[Test]
		public void CommandOnly() {
			var args = new CommandLineParameters(Header,
				Footer, new CommandLineParameter[0],
				new[]
				{
					new CommandLineCommand("test", "testing command", new CommandLineParameter[0], new[]
					{
						new CommandLineCommand("test-2", "sub command", new[]
						{
							new CommandLineParameter("foo", "baz description")
						}, new CommandLineCommand[0])
					}),
				});

			Result<CommandLineResults> result = args.TryParseArguments(new[] { "test" });
			Assert.IsTrue(result.Success);
		}


		[Test]
		[TestCase(CommandLineArgumentOptions.ForwardSlash)]
		[TestCase(CommandLineArgumentOptions.SingleDash)]
		[TestCase(CommandLineArgumentOptions.DoubleDash)]
		public void ArgNameMatchOptions(CommandLineArgumentOptions options) {
			var args = new CommandLineParameters(Header,
				Footer, new CommandLineParameter[] { new("test", "test", CommandLineParameterOptions.Mandatory) },
				new[]
				{
					new CommandLineCommand("test", "testing command", new CommandLineParameter[0],
						new CommandLineCommand[0])
				},
				options);

			var parsedSpace = args.TryParseArguments(new[]
			{
				"test",
				"/test", "test",
				"-test", "test",
				"--test", "test"
			});

			var parsedEquals = args.TryParseArguments(new[]
			{
				"test",
				"/test", "test",
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
			var args = new CommandLineParameters(
				Header,
				Footer,
				new CommandLineParameter[] { new("multi", "multiple trait", CommandLineParameterOptions.Multiple) },
				new[]
				{
					new CommandLineCommand("test", "testing command", new CommandLineParameter[0],
						new CommandLineCommand[0])
				},
				CommandLineArgumentOptions.DoubleDash | CommandLineArgumentOptions.SingleDash |
				CommandLineArgumentOptions.ForwardSlash);

			var parsed = args.TryParseArguments(new[]
			{
				"test",
				"/multi", "a",
				"-multi", "b",
				"--multi", "c"
			});

			Assert.IsTrue(parsed.Success);
			Assert.AreEqual(new[] { "a", "b", "c" }, parsed.Value.Arguments["multi"]);
		}

		[Test]
		public void ArgTraitSingle() {
			var args = new CommandLineParameters(
				Header,
				Footer, new CommandLineParameter[] { new("single", "single trait") },
				new[]
				{
					new CommandLineCommand("test", "testing command", new CommandLineParameter[0],
						new CommandLineCommand[0])
				},
				CommandLineArgumentOptions.DoubleDash | CommandLineArgumentOptions.SingleDash |
				CommandLineArgumentOptions.ForwardSlash);

			var invalid = args.TryParseArguments(new[]
			{
				"test",
				"/single", "a",
				"-single:b",
				"--single=c"
			});

			Assert.IsFalse(invalid.Success);
			Assert.AreEqual(1, invalid.ErrorMessages.Count());
			Assert.IsEmpty(invalid.Value.Arguments);

			var valid = args.TryParseArguments(new[]
			{
				"test",
				"--single=c"
			});

			Assert.IsTrue(valid.Success);
			Assert.AreEqual("c", valid.Value.Arguments["single"].Single());
		}

		[Test]
		public void ArgTraitMandatory() {
			var args = new CommandLineParameters(
				Header,
				Footer, new CommandLineParameter[]
				{
					new("mandatory", "mandatory trait", CommandLineParameterOptions.Mandatory)
				},
				new[]
				{
					new CommandLineCommand("test", "testing command", new CommandLineParameter[0],
						new CommandLineCommand[0])
				},
				CommandLineArgumentOptions.DoubleDash | CommandLineArgumentOptions.SingleDash |
				CommandLineArgumentOptions.ForwardSlash);

			var invalid = args.TryParseArguments(new[]
			{
				"test",
				"/argumentA", "test",
			});

			Assert.IsFalse(invalid.Success);
			Assert.AreEqual(1, invalid.ErrorMessages.Count());
			Assert.IsEmpty(invalid.Value.Arguments);

			var valid = args.TryParseArguments(new[]
			{
				"test",
				"/mandatory", "test",
			});

			Assert.IsTrue(valid.Success);
			Assert.AreEqual("test", valid.Value.Arguments["mandatory"].Single());
		}

		[Test]
		public void ArgCaseSensitive() {
			var args = new CommandLineParameters(
				Header,
				Footer, new CommandLineParameter[]
				{
					new("mAnDaToRy", "mandatory trait", CommandLineParameterOptions.Mandatory)
				},
				new[]
				{
					new CommandLineCommand("test", "testing command", new CommandLineParameter[0],
						new CommandLineCommand[0])
				},
				CommandLineArgumentOptions.DoubleDash | CommandLineArgumentOptions.SingleDash |
				CommandLineArgumentOptions.ForwardSlash |
				CommandLineArgumentOptions.CaseSensitive);

			var invalid = args.TryParseArguments(new[]
			{
				"test", "/mandatory", "test",
			});

			Assert.IsFalse(invalid.Success);
			Assert.AreEqual(1, invalid.ErrorMessages.Count());
			Assert.IsEmpty(invalid.Value.Arguments);

			var valid = args.TryParseArguments(new[]
			{
				"test",
				"/mAnDaToRy", "test",
			});

			Assert.IsTrue(valid.Success);
			Assert.AreEqual("test", valid.Value.Arguments["mAnDaToRy"].Single());
		}

		[Test]
		public void SuccessParseH() {
			var args = new CommandLineParameters(
				Header,
				Footer, new CommandLineParameter[]
				{
					new("parameter", "mandatory", CommandLineParameterOptions.Mandatory)
				},
				new[]
				{
					new CommandLineCommand("test", "testing command", new CommandLineParameter[0],
						new CommandLineCommand[0])
				});

			var help = args.TryParseArguments(new[]
			{
				"test",
				"/parameter", "test",
				"--h"
			});

			Assert.IsTrue(help.Success);
			Assert.IsTrue(help.Value.HelpRequested);
		}

		[Test]
		public void SuccessParseHelp() {
			var args = new CommandLineParameters(
				Header,
				Footer, new CommandLineParameter[]
				{
					new("parameter", "mandatory", CommandLineParameterOptions.Mandatory)
				},
				new[]
				{
					new CommandLineCommand("test", "testing command", new CommandLineParameter[0],
						new CommandLineCommand[0])
				});

			var help = args.TryParseArguments(new[]
			{
				"test",
				"/parameter", "test",
				"--help"
			});

			Assert.IsTrue(help.Success);
			Assert.IsTrue(help.Value.HelpRequested);
		}

		[Test]
		public void ArgDependencies() {
			var args = new CommandLineParameters(
				Header,
				Footer, new CommandLineParameter[]
				{
					new("mandatoryWithDependency", "mandatory", CommandLineParameterOptions.Mandatory, "test"),
					new("test", "optional")
				},
				new[]
				{
					new CommandLineCommand("test", "testing command", new CommandLineParameter[0],
						new CommandLineCommand[0])
				});

			var invalid = args.TryParseArguments(new[]
			{
				"test",
				"/mandatoryWithDependency", "test"
			});

			Assert.IsFalse(invalid.Success);
			Assert.AreEqual(1, invalid.ErrorMessages.Count());
			Assert.IsEmpty(invalid.Value.Arguments);

			var valid = args.TryParseArguments(new[]
			{
				"test",
				"/mandatoryWithDependency", "test",
				"/test", "test"
			});

			Assert.IsTrue(valid.Success);
			Assert.IsEmpty(valid.ErrorMessages);
			Assert.AreEqual("test", valid.Value.Arguments["mandatoryWithDependency"].Single());
			Assert.AreEqual("test", valid.Value.Arguments["test"].Single());
		}

		[Test]
		public void ParseComplexArg() {
			string[] input =
			{
				"test",
				"-param1", "value1",
				"--param2",
				"/param3:\"Test-:-work\"",
				"/param4=happy",
				"-param5", "'--=nice=--'"
			};

			var args = new CommandLineParameters(
				Header,
				Footer, new CommandLineParameter[]
				{
					new("param1", "1", CommandLineParameterOptions.Mandatory),
					new("param2", "2", CommandLineParameterOptions.Mandatory),
					new("param3", "3", CommandLineParameterOptions.Mandatory),
					new("param4", "4", CommandLineParameterOptions.Mandatory),
					new("param5", "5", CommandLineParameterOptions.Mandatory)
				},
				new[]
				{
					new CommandLineCommand("test", "testing command", new CommandLineParameter[0],
						new CommandLineCommand[0])
				});

			var parsed = args.TryParseArguments(input);

			Assert.IsTrue(parsed.Success);
			Assert.AreEqual(5, parsed.Value.Arguments.Count());
			Assert.IsEmpty(parsed.ErrorMessages);
		}
	}
}