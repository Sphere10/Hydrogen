namespace Sphere10.Framework {
	public interface ISecureItem {
		bool Protected { get; }
		IScope EnterUnprotectedScope();
	}
}
