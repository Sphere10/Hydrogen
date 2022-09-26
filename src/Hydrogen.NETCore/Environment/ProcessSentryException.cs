namespace Hydrogen;

public class ProcessSentryException : SoftwareException {
	public ProcessSentryException(string fileName, int errorCode, string message)
		: base($"Execution of `{fileName} returned errorcode {errorCode}. {message.AsAmendmentIf(!string.IsNullOrWhiteSpace(message))}") {
	}
}