using System;

namespace Hydrogen;

public abstract class PrefixLoggerBase : LoggerDecorator {
	protected PrefixLoggerBase(ILogger decoratedLogger)
		: base(decoratedLogger) {
	}

	public override void Debug(string message) {
		base.Debug($"{GetPrefix()}" + message);
	}

	public override void Info(string message) {
		base.Info($"{GetPrefix()}" + message);
	}

	public override void Warning(string message) {
		base.Warning($"{GetPrefix()}" + message);
	}

	public override void Error(string message) {
		base.Error($"{GetPrefix()}" + message);
	}

	public override void Exception(Exception exception) {
		if (LoggerHelper.TryHydrateErrorMessage(exception, Options, out var message))
			Error(message);
	}

	protected abstract string GetPrefix();

}