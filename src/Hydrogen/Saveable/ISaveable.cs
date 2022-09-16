using System.Threading.Tasks;

namespace Hydrogen;

	public interface ISaveable {
		event EventHandlerEx<object> Saving;
		event EventHandlerEx<object> Saved;
		bool RequiresSave { get; }
		void Save();
		Task SaveAsync();
	}