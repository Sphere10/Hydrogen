namespace Hydrogen {
	public abstract class CacheReaperBase : ICacheReaper {

		public abstract void Register(ICache cache);

		public abstract void Deregister(ICache cache);

		public abstract long AvailableSpace();

		public abstract long MakeSpace(ICache requestingCache, long requestedBytes);
	}
}
