using Sphere10.Framework;

namespace Sphere10.Hydrogen.Presentation2.Logic {
	public interface IApplicationMenuItem {
		event EventHandlerEx Hover;
		event EventHandlerEx Select;

		string Icon { get; }
		string Title { get; }
	}

}