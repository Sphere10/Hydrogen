namespace Sphere10.Framework {
	public interface ICacheReaper {

		void Register(ICache cache);

		void Deregister(ICache cache);

		long AvailableSpace();

		long MakeSpace(ICache requestingCache, long requestedBytes);
	}
}
