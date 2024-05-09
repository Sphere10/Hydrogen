// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

//using System;
//using System.Linq;
//using NUnit.Framework;

//namespace Hydrogen.Tests.Environment {
//	[TestFixture]
//	[Parallelizable(ParallelScope.Children)]
//	public class CommandLineArgsTests {
//		private string[] Header { get; } =
//		{
//			"Unit Testing",
//			"CommandLineArgsTest"
//		};

//		private string[] Footer { get; } =
//		{
//			"Footer"
//		};


//		//[Test]
//		//public void AmbiguousCommandOrValue


//		[Test]
//		public void MultipleCommandsAndParams() {
//			var args = new CommandLineParameters {
//				Parameters = new[] { new CommandLineParameter("p1", CommandLineParameterOptions.Mandatory) },
//				Commands = new[] {
//					new CommandLineCommand("c1", new[] {
//							new CommandLineParameter("p2", CommandLineParameterOptions.RequiresValue)
//						},
//						new[] {
//							new CommandLineCommand("c2", new[] {
//								new CommandLineParameter("p3", string.Empty,
//									CommandLineParameterOptions.RequiresValue | CommandLineParameterOptions.Multiple)
//							})
//						})
//				}
//			};

//			var result = args.TryParseArguments(new[] {
//				"--p1", "c1", "--p2", "p2value", "c2", "--p3", "p3value", "--p3", "p3value2"
//			});

//			ClassicAssert.IsTrue(result.Success);
//			var results = result.Value;

//			ClassicAssert.IsTrue(results.Arguments.Contains("p1"));
//			ClassicAssert.IsEmpty(results.Arguments["p1"]);
//			ClassicAssert.AreEqual("c1", results.SubCommand.CommandName);
//			ClassicAssert.AreEqual("p2value", results.SubCommand.Arguments["p2"].Single());
//			ClassicAssert.AreEqual("c2", results.SubCommand.SubCommand.CommandName);
//			ClassicAssert.AreEqual("p3value", results.SubCommand.SubCommand.Arguments["p3"].First());
//			ClassicAssert.AreEqual("p3value2", results.SubCommand.SubCommand.Arguments["p3"].Skip(1).First());
//		}

//		[Test]
//		public void ParametersWithoutValues() {
//			var args = new CommandLineParameters {
//				Parameters = new[]
//				{
//					new CommandLineParameter("p1", string.Empty, CommandLineParameterOptions.Mandatory),
//					new CommandLineParameter("p2", string.Empty,
//						CommandLineParameterOptions.Mandatory | CommandLineParameterOptions.RequiresValue),
//					new CommandLineParameter("p3", string.Empty, CommandLineParameterOptions.Mandatory)
//				},
//				Commands = new[]
//				{
//					new CommandLineCommand("c1", string.Empty)
//				}
//			};

//			var result = args.TryParseArguments(new[] { "--p1", "--p2", "p2value", "--p3", "c1" });
//			ClassicAssert.IsEmpty(result.Value.Arguments["p1"]);
//			ClassicAssert.AreEqual("p2value", result.Value.Arguments["p2"].Single());
//			ClassicAssert.IsEmpty(result.Value.Arguments["p3"]);
//			ClassicAssert.AreEqual("c1", result.Value.SubCommand.CommandName);
//		}

//		[Test]
//		public void DefaultConstructorParse() {
//			var args = new CommandLineParameters();
//			var result = args.TryParseArguments(Array.Empty<string>());
//			ClassicAssert.IsTrue(result.Success);
//		}

//		[Test]
//		public void InitArgParse() {
//			var args = new CommandLineParameters() {
//				Parameters = new[]
//				{
//					new CommandLineParameter("test", "unit testing",
//						CommandLineParameterOptions.Mandatory | CommandLineParameterOptions.RequiresValue)
//				},
//				Commands = new[] { new CommandLineCommand("unittest", "unit testing command") }
//			};

//			var result = args.TryParseArguments(new string[] { "--test", "valid", "unittest" });
//			ClassicAssert.IsTrue(result.Success);
//			ClassicAssert.AreEqual("unittest", result.Value.SubCommand.CommandName);
//			ClassicAssert.AreEqual("valid", result.Value.Arguments["test"].Single());
//		}

//		[Test]
//		public void NoCommandOrArgIsValid() {
//			var args = new CommandLineParameters(Header,
//				Footer, new CommandLineParameter[0],
//				new[]
//				{
//					new CommandLineCommand("test", "test", Array.Empty<CommandLineParameter>(),
//						Array.Empty<CommandLineCommand>())
//				});

//			var result = args.TryParseArguments(Array.Empty<string>());

//			ClassicAssert.IsTrue(result.Success);
//			ClassicAssert.IsNull(result.Value.SubCommand);
//			ClassicAssert.IsEmpty(result.Value.Arguments);
//		}

//		[Test]
//		public void AtLeastOneCommandMustMatch() {
//			var args = new CommandLineParameters(Header,
//				Footer, new CommandLineParameter[0],
//				new[]
//				{
//					new CommandLineCommand("test", "testing command", new CommandLineParameter[0],
//						new CommandLineCommand[0])
//				});

//			var result = args.TryParseArguments(new[] { "test2", "--baz" });
//			ClassicAssert.IsTrue(result.Failure);
//		}

//		[Test]
//		public void FailMultipleCommandsMatched() {
//			var args = new CommandLineParameters(Header,
//				Footer, new CommandLineParameter[0],
//				new[]
//				{
//					new CommandLineCommand("test", "testing command", new CommandLineParameter[0],
//						new CommandLineCommand[0])
//				});

//			var result = args.TryParseArguments(new[] { "test", "test2", "--baz" });
//			ClassicAssert.IsTrue(result.Failure);
//		}

//		[Test]
//		public void MatchMultipleCommand() {
//			var args = new CommandLineParameters(Header,
//				Footer, new CommandLineParameter[0],
//				new[]
//				{
//					new CommandLineCommand("test", "testing command", new CommandLineParameter[0], new[]
//					{
//						new CommandLineCommand("test-2", "sub command", new[]
//						{
//							new CommandLineParameter("foo", "baz description")
//						}, new CommandLineCommand[0])
//					}),
//				});

//			var result = args.TryParseArguments(new[] { "test", "test-2", "--foo baz" });
//			ClassicAssert.IsTrue(result.Success);
//			ClassicAssert.AreEqual("test", result.Value.SubCommand.CommandName);
//			var testResult = result.Value.SubCommand;
//			var test2Results = testResult.SubCommand;

//			ClassicAssert.AreEqual("baz", test2Results.Arguments["foo"].Single());
//		}

//		[Test]
//		public void CommandArgumentMandatory() {
//			var args = new CommandLineParameters(Header,
//				Footer, new CommandLineParameter[0],
//				new[]
//				{
//					new CommandLineCommand("test", "testing command", new CommandLineParameter[0], new[]
//					{
//						new CommandLineCommand("test-2", "sub command", new[]
//						{
//							new CommandLineParameter("foo", "baz description",
//								CommandLineParameterOptions.Mandatory | CommandLineParameterOptions.RequiresValue)
//						}, new CommandLineCommand[0])
//					}),
//				});

//			var invalid = args.TryParseArguments(new[] { "test", "test-2", "--fo0", "baz" });
//			ClassicAssert.IsFalse(invalid.Success);

//			var valid = args.TryParseArguments(new[] { "test", "test-2", "--foo", "baz" });
//			ClassicAssert.IsTrue(valid.Success);
//		}

//		[Test]
//		public void CommandOnly() {
//			var args = new CommandLineParameters(Header,
//				Footer, new CommandLineParameter[0],
//				new[]
//				{
//					new CommandLineCommand("test", "testing command", new CommandLineParameter[0], new[]
//					{
//						new CommandLineCommand("test-2", "sub command", new[]
//						{
//							new CommandLineParameter("foo", "baz")
//						})
//					}),
//				});

//			var result = args.TryParseArguments(new[] { "test", "test-2" });
//			ClassicAssert.IsTrue(result.Success);

//			ClassicAssert.AreEqual("test", result.Value.SubCommand.CommandName);
//			ClassicAssert.AreEqual("test-2", result.Value.SubCommand.SubCommand.CommandName);
//		}

//		[Test]
//		[TestCase(new[] { "/test", "test" }, CommandLineArgumentOptions.ForwardSlash)]
//		[TestCase(new[] { "-test", "test" }, CommandLineArgumentOptions.SingleDash)]
//		[TestCase(new[] { "--test", "test" }, CommandLineArgumentOptions.DoubleDash)]
//		[TestCase(new[] { "/test=test" }, CommandLineArgumentOptions.ForwardSlash)]
//		[TestCase(new[] { "-test=test" }, CommandLineArgumentOptions.SingleDash)]
//		[TestCase(new[] { "--test=test" }, CommandLineArgumentOptions.DoubleDash)]
//		public void ArgNameMatchOptions(string[] args, CommandLineArgumentOptions options) {
//			var p = new CommandLineParameters(Header,
//				Footer,
//				new CommandLineParameter[]
//				{
//					new("test", "test", CommandLineParameterOptions.Mandatory |
//										CommandLineParameterOptions.RequiresValue)
//				},
//				options);

//			var parsed = p.TryParseArguments(args);

//			ClassicAssert.IsTrue(parsed.Success);
//			ClassicAssert.AreEqual(1, parsed.Value.Arguments["test"].Count());
//			ClassicAssert.AreEqual("test", parsed.Value.Arguments["test"].Single());
//		}

//		[Test]
//		public void ArgTraitMulti() {
//			var args = new CommandLineParameters(
//				Header,
//				Footer,
//				new CommandLineParameter[]
//				{
//					new("multi", "multiple trait", CommandLineParameterOptions.Multiple |
//												   CommandLineParameterOptions.RequiresValue)
//				},
//				CommandLineArgumentOptions.DoubleDash | CommandLineArgumentOptions.SingleDash |
//				CommandLineArgumentOptions.ForwardSlash);

//			var parsed = args.TryParseArguments(new[]
//			{
//				"/multi", "a",
//				"-multi", "b",
//				"--multi", "c"
//			});

//			ClassicAssert.IsTrue(parsed.Success);
//			ClassicAssert.AreEqual(new[] { "a", "b", "c" }, parsed.Value.Arguments["multi"]);
//		}

//		[Test]
//		public void ArgTraitSingle() {
//			var args = new CommandLineParameters(
//				Header,
//				Footer,
//				new CommandLineParameter[] { new("single", "single trait", CommandLineParameterOptions.RequiresValue) },
//				CommandLineArgumentOptions.DoubleDash | CommandLineArgumentOptions.SingleDash |
//				CommandLineArgumentOptions.ForwardSlash);

//			var invalid = args.TryParseArguments(new[]
//			{
//				"/single", "a",
//				"-single:b",
//				"--single=c"
//			});

//			ClassicAssert.IsFalse(invalid.Success);
//			ClassicAssert.AreEqual(1, invalid.ErrorMessages.Count());

//			var valid = args.TryParseArguments(new[]
//			{
//				"--single=c"
//			});

//			ClassicAssert.IsTrue(valid.Success);
//			ClassicAssert.AreEqual("c", valid.Value.Arguments["single"].Single());
//		}

//		[Test]
//		public void ArgTraitMandatory() {
//			var args = new CommandLineParameters(
//				Header,
//				Footer, new CommandLineParameter[]
//				{
//					new("mandatory", "mandatory trait", CommandLineParameterOptions.Mandatory |
//														CommandLineParameterOptions.RequiresValue)
//				},
//				CommandLineArgumentOptions.DoubleDash | CommandLineArgumentOptions.SingleDash |
//				CommandLineArgumentOptions.ForwardSlash);

//			var invalid = args.TryParseArguments(new[]
//			{
//				"test"
//			});

//			ClassicAssert.IsFalse(invalid.Success);
//			ClassicAssert.AreEqual(1, invalid.ErrorMessages.Count());
//			ClassicAssert.IsEmpty(invalid.Value.Arguments);

//			var valid = args.TryParseArguments(new[]
//			{
//				"/mandatory", "test",
//			});

//			ClassicAssert.IsTrue(valid.Success);
//			ClassicAssert.AreEqual("test", valid.Value.Arguments["mandatory"].Single());
//		}

//		[Test]
//		public void ArgCaseSensitive() {
//			var args = new CommandLineParameters(
//				Header,
//				Footer, new CommandLineParameter[]
//				{
//					new("mAnDaToRy", "mandatory trait", CommandLineParameterOptions.Mandatory |
//														CommandLineParameterOptions.RequiresValue)
//				}
//				,
//				CommandLineArgumentOptions.DoubleDash | CommandLineArgumentOptions.SingleDash |
//				CommandLineArgumentOptions.ForwardSlash |
//				CommandLineArgumentOptions.CaseSensitive);

//			var invalid = args.TryParseArguments(new[]
//			{
//				"/mandatory", "test",
//			});

//			ClassicAssert.IsFalse(invalid.Success);
//			ClassicAssert.AreEqual(1, invalid.ErrorMessages.Count());
//			ClassicAssert.IsEmpty(invalid.Value.Arguments);

//			var valid = args.TryParseArguments(new[]
//			{
//				"/mAnDaToRy", "test",
//			});

//			ClassicAssert.IsTrue(valid.Success);
//			ClassicAssert.AreEqual("test", valid.Value.Arguments["mAnDaToRy"].Single());
//		}

//		[Test]
//		public void SuccessParseH() {
//			var args = new CommandLineParameters(
//				Header,
//				Footer, new CommandLineParameter[]
//				{
//					new("parameter", "mandatory", CommandLineParameterOptions.Mandatory |
//												  CommandLineParameterOptions.RequiresValue)
//				});

//			var help = args.TryParseArguments(new[]
//			{
//				"/parameter", "test",
//				"--h"
//			});

//			ClassicAssert.IsTrue(help.Success);
//			ClassicAssert.IsTrue(help.Value.HelpRequested);
//		}

//		[Test]
//		public void SuccessParseHelp() {
//			var args = new CommandLineParameters(
//				Header,
//				Footer, new CommandLineParameter[]
//				{
//					new("parameter", "mandatory", CommandLineParameterOptions.Mandatory)
//				});

//			var help = args.TryParseArguments(new[]
//			{
//				"/parameter",
//				"--help"
//			});

//			ClassicAssert.IsTrue(help.Success);
//			ClassicAssert.IsTrue(help.Value.HelpRequested);
//		}

//		[Test]
//		public void ArgDependencies() {
//			var args = new CommandLineParameters(
//				Header,
//				Footer, new CommandLineParameter[]
//				{
//					new("mandatoryWithDependency", "mandatory", CommandLineParameterOptions.Mandatory |
//																CommandLineParameterOptions.RequiresValue, "test"),
//					new("test", "optional", CommandLineParameterOptions.RequiresValue)
//				});

//			var invalid = args.TryParseArguments(new[]
//			{
//				"/mandatoryWithDependency", "test"
//			});

//			ClassicAssert.IsFalse(invalid.Success);
//			ClassicAssert.AreEqual(1, invalid.ErrorMessages.Count());

//			var valid = args.TryParseArguments(new[]
//			{
//				"/mandatoryWithDependency", "test",
//				"/test", "test"
//			});

//			ClassicAssert.IsTrue(valid.Success);
//			ClassicAssert.IsEmpty(valid.ErrorMessages);
//			ClassicAssert.AreEqual("test", valid.Value.Arguments["mandatoryWithDependency"].Single());
//			ClassicAssert.AreEqual("test", valid.Value.Arguments["test"].Single());
//		}

//		[Test]
//		public void ParseComplexArg() {
//			string[] input =
//			{
//				"-param1", "value1",
//				"--param2",
//				"/param3:\"Test-:-work\"",
//				"/param4=happy",
//				"-param5", "'--=nice=--'"
//			};

//			var args = new CommandLineParameters(
//				Header,
//				Footer, new CommandLineParameter[]
//				{
//					new("param1", "1", CommandLineParameterOptions.Mandatory |
//									   CommandLineParameterOptions.RequiresValue),
//					new("param2", "2", CommandLineParameterOptions.Mandatory),
//					new("param3", "3", CommandLineParameterOptions.Mandatory |
//									   CommandLineParameterOptions.RequiresValue),
//					new("param4", "4", CommandLineParameterOptions.Mandatory |
//									   CommandLineParameterOptions.RequiresValue),
//					new("param5", "5", CommandLineParameterOptions.Mandatory |
//									   CommandLineParameterOptions.RequiresValue)
//				});
//			var parsed = args.TryParseArguments(input);

//			ClassicAssert.IsTrue(parsed.Success);
//			ClassicAssert.AreEqual(5, parsed.Value.Arguments.Count());
//			ClassicAssert.IsEmpty(parsed.ErrorMessages);
//		}
//	}
//}


