using System.Linq;
using Sphere10.Framework;
using Terminal.Gui;

namespace VelocityNET.Presentation.Node {

	public abstract class Screen : View {

		protected Screen() {
			X = 0;
			Y = 0; // Leave one row for the toplevel menu
				   // By using Dim.Fill(), it will automatically resize without manual intervention
			Width = Dim.Fill();
			Height = Dim.Fill();
			BuildUI();
		}

		protected abstract void BuildUI();

		public virtual string Title
			=> GetType().GetCustomAttributesOfType<TitleAttribute>().SingleOrDefault()?.Title ?? "(untitled)";

		public virtual void OnCreate() {
		}

		public virtual void OnAppearing() {
		}

		public virtual void OnAppeared() {
		}

		public virtual void OnDisappearing(out bool cancel) {
			cancel = false;
		}

		public virtual void OnDisappeared() {
		}

		public virtual void OnDestroy() {
		}

	}

	public abstract class Screen<T> : Screen {
		public virtual T Model { get; set; }
	}


}