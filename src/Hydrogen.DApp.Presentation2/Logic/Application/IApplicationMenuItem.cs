using Hydrogen;

namespace Hydrogen.DApp.Presentation2.Logic {
	public interface IApplicationMenuItem {
		event EventHandlerEx Hover;
		event EventHandlerEx Select;

		string Icon { get; }
		string Title { get; }
	}

}