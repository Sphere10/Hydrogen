using System;
using System.Collections.Generic;
using System.Linq;
using Sphere10.Framework;
using Terminal.Gui;

namespace VelocityNET.Presentation.Node {

	public abstract class MultiPartScreen : Screen {
		private readonly Screen[] _screenParts;
		private FrameView _partFrameView;
		private ListView _partListView;
		private Screen _activePart;


		protected MultiPartScreen(IEnumerable<Screen> screenParts) {
			Guard.ArgumentNotNull(screenParts, nameof(screenParts));
			_screenParts = screenParts.ToArray();
			_partListView.SetSource(_screenParts.Select(x => x.Title).ToList());
			_partListView.SelectedItemChanged += args => ShowScreen(_screenParts[args.Item]);
			if (_screenParts.Any())
				ShowScreen(_screenParts[0]);
		}

		protected sealed override void BuildUI() {
			_partFrameView = new FrameView(this.Title) {
				X = 0,
				Y = 0,
				Width = 50,
				Height = Dim.Fill()
			};
			this.Add(_partFrameView);

			_partListView = new ListView() {
				X = 0,
				Y = 0,
				Width = Dim.Fill(),
				Height = Dim.Fill()
			};
			_partFrameView.Add(_partListView);
		}


		public void ShowScreen(Screen part) {
			if (part == _activePart)
				return;

			if (_activePart != null) {
				_activePart.OnDisappearing(out var cancel);
				if (cancel)
					return;
				this.Remove(_activePart);
				_activePart.OnDisappeared();
			}
			part.X = Pos.Right(_partFrameView);
			part.Y = 0;
			part.Width = Dim.Fill();
			part.Height = Dim.Fill();
			part.OnAppearing();
			this.Add(part);
			part.OnAppeared();
			_activePart = part;
		}

	}

	public abstract class MultiPartScreen<T> : MultiPartScreen {

		protected MultiPartScreen(T model, IEnumerable<Screen<T>> screenParts ) : base(screenParts) {
			Guard.ArgumentNotNull(model, nameof(model));
			Model = model;
			foreach (var screenPart in screenParts) {
				screenPart.Model = model;
			}
		}

		public virtual T Model { get; set; }

	}

}
