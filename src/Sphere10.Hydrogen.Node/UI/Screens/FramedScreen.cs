using Terminal.Gui;
using Sphere10.Hydrogen.Node.Screens;

namespace Sphere10.Hydrogen.Node.UI {

	public abstract class FramedScreen<T> : Screen<T> {
		protected new FrameView Frame;

		protected override void LoadInternal() {
			Frame = new FrameView {
				X = 0,
				Y = 0,
				Width = Dim.Fill(),
				Height = Dim.Fill(),
				Title = this.Title
			};
			base.Add(Frame);
		}

		public override void Add(View view) {
			Frame.Add(view);
		}

		public override void Remove(View view) {
			Frame.Remove(view);
		}

		public override void RemoveAll() {
			Frame.RemoveAll();
		}
	}

}
