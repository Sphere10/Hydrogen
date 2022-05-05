namespace Hydrogen {
	public interface ISecureItem {
		bool Protected { get; }
		IScope EnterUnprotectedScope();
	}
}
