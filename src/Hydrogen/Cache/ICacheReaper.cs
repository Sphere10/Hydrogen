namespace Hydrogen {
	public interface ICacheReaper {

		void Register(ICache cache);

		void Deregister(ICache cache);

		long AvailableSpace();

		long MakeSpace(ICache requestingCache, long requestedBytes);
	}
}
