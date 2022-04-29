using System;

namespace Sphere10.Framework.Windows.Forms {
	[Flags]
	public enum FinishedUpdateBehaviour {
		/// <summary>
		/// Does not raise any event or change any UI/Model state
		/// </summary>
		DoNothing = 0,

		/// <summary>
		/// Raises StateChanged event
		/// </summary>
		NotifyStateChanged = 1 << 0,

		/// <summary>
		/// Copies UI to Model irrepesctive of UpdateModelOnStateChanged value"/>
		/// </summary>
		ForceCopyUIToModel = 1 << 1,
		
		/// <summary>
		///  Does not call StateChanged Event but copies UI to model
		/// </summary>
		CopyModelToUI = 1 << 2,

		Default = NotifyStateChanged,
	}
}
