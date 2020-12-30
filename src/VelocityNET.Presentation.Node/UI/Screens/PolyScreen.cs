using System.Collections.Generic;
using System.Linq;
using Sphere10.Framework;
using Terminal.Gui;
using VelocityNET.Presentation.Node.Screens;

namespace VelocityNET.Presentation.Node.UI {

	/// <summary>
	/// A host screen for many component screens that are selected from a left menu.
	/// </summary>
	public abstract class PolyScreen : Screen {
		private readonly Screen[] _componentScreens;
		private FrameView _componentScreenFrame;
		private ListView _componentScreenList;
		private Screen _activeComponentScreen;


		protected PolyScreen(IEnumerable<Screen> componentScreens) {
			Guard.ArgumentNotNull(componentScreens, nameof(componentScreens));
			_componentScreens = componentScreens.ToArray();
		}

		public sealed override IEnumerable<StatusItem> BuildStatusItems() 
			=> base.BuildStatusItems().Concat(BuildStatusItemsInternal()).Concat(_activeComponentScreen.BuildStatusItems());

		protected virtual IEnumerable<StatusItem> BuildStatusItemsInternal()
			=> Enumerable.Empty<StatusItem>();

		protected sealed override void LoadInternal() {
			_componentScreenFrame = new FrameView(this.Title) {
				X = 0,
				Y = 0,
				Width = 50,
				Height = Dim.Fill()
			};
			this.Add(_componentScreenFrame);

			_componentScreenList = new ListView() {
				X = 0,
				Y = 0,
				Width = Dim.Fill(),
				Height = Dim.Fill()
			};
			_componentScreenList.SetSource(_componentScreens.Select(x => x.Title).ToList());
			_componentScreenList.SelectedItemChanged += args => ShowScreen(_componentScreens[args.Item]);
			_componentScreenFrame.Add(_componentScreenList);

			foreach(var screen in _componentScreens)
				screen.Load();

		}

		protected override void OnAppearing() {
			base.OnAppearing();
			if (AppearCount == 1 && _componentScreens.Any()) {
				_componentScreenList.FocusFirst();
			}
		}

		protected override void OnAppeared() {
			base.OnAppeared();
			this.SetFocus();
		}


		public void ShowScreen(Screen componentScreen) {
			if (componentScreen == _activeComponentScreen)
				return;

			if (_activeComponentScreen != null) {
				_activeComponentScreen.NotifyDisappearing(out var cancel);
				if (cancel)
					return;
				this.Remove(_activeComponentScreen);
				_activeComponentScreen.NotifyDisappeared();
			}
			componentScreen.X = Pos.Right(_componentScreenFrame);
			componentScreen.Y = 0;
			componentScreen.Width = Dim.Fill();
			componentScreen.Height = Dim.Fill();
			componentScreen.NotifyAppearing();
			this.Add(componentScreen);
			componentScreen.NotifyAppeared();
			_activeComponentScreen = componentScreen;
			Navigator.NotifyStatusBarChanged();
		}

	}

	public abstract class PolyScreen<T> : PolyScreen {
		
		protected PolyScreen(T model, params Screen<T>[] componentScreens ) 
			: this(model, componentScreens as IEnumerable<Screen<T>>) {
		}

		protected PolyScreen(T model, IEnumerable<Screen<T>> componentScreens)
			: base(componentScreens) {
			Guard.ArgumentNotNull(model, nameof(model));
			Model = model;
			foreach (var screenPart in componentScreens) {
				screenPart.Model = model;
			}
		}

		public virtual T Model { get; set; }

	}

}
