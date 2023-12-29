namespace Hydrogen.ObjectSpaces;

public interface IObjectContainerAttachment {
	ObjectContainer Container { get; } 

	long ReservedStreamIndex { get; }

	bool IsAttached { get; }

	void Attach();

	void Detach();
}
