namespace Hydrogen.ObjectSpaces;

public interface IObjectContainerAttachment {
	ObjectContainer Container { get; } 

	long ReservedStreamIndex { get; }

	void Attach();

	void Detach();
}
