
namespace Hydrogen {

	public interface IObservableCollection : ISuppressableEvents {
		event EventHandlerEx<object, EventTraits> Accessing;
		event EventHandlerEx<object, EventTraits> Accessed;
		event EventHandlerEx<object, EventTraits> Reading;
		event EventHandlerEx<object, EventTraits> Read;
		event EventHandlerEx<object, EventTraits> Mutating;
		event EventHandlerEx<object, EventTraits> Mutated;

		EventTraits EventFilter { get; set; }
	}

}