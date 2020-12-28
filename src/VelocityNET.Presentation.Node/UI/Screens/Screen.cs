using System;
using System.Collections.Generic;
using System.Linq;
using Sphere10.Framework;
using Terminal.Gui;
using VelocityNET.Presentation.Node.Screens;

namespace VelocityNET.Presentation.Node.UI {

	public abstract class Screen : View {
		private bool _initialized;
		private bool _enabled;
		private Dictionary<View, bool> _viewCouldFocus;
		public event EventHandlerEx<Screen> Created;
		public event EventHandlerEx<Screen> Loading;
		public event EventHandlerEx<Screen> Loaded;
		public event EventHandlerEx<Screen> Appearing;
		public event EventHandlerEx<Screen> Appeared;
		public event EventHandlerEx<Screen> Disappearing;
		public event EventHandlerEx<Screen> Disappeared;
		public event EventHandlerEx<Screen> Destroying;

		protected Screen() {
			X = 0;
			Y = 0; // Leave one row for the toplevel menu
				   // By using Dim.Fill(), it will automatically resize without manual intervention
			Width = Dim.Fill();
			Height = Dim.Fill();
			_initialized = false;
			_enabled = true;
			_viewCouldFocus = new Dictionary<View, bool>();
		}

		public void Load() {
			Guard.Ensure(!_initialized, "Already loaded");
			NotifyLoading();
			LoadInternal();
			NotifyLoaded();
		}

		public bool Enabled {
			get => _enabled;
			set {
				if (value == _enabled)
					return;
				if (value) 
					EnableControls();
				 else 
					DisableControls();
				_enabled = value;
			}
		}

		public virtual string Title
			=> GetType().GetCustomAttributesOfType<TitleAttribute>().SingleOrDefault()?.Title ?? "(untitled)";

		public virtual IEnumerable<StatusItem> BuildStatusItems()
			=> Enumerable.Empty<StatusItem>();

		protected abstract void LoadInternal();

		protected virtual void OnCreate() {
		}

		protected virtual void OnLoading() {

		}

		protected virtual void OnLoaded() {
			_initialized = true;
			if (!_enabled)
				DisableControls();
		}
	
		protected virtual void OnAppearing() {
		}

		protected virtual void OnAppeared() {
		}

		protected virtual void OnDisappearing(out bool cancel) {
			cancel = false;
		}

		protected virtual void OnDisappeared() {
		}

		protected virtual void OnDestroy() {
		}

		internal void NotifyCreated() {
			OnCreate();
			Created?.Invoke(this);
		}

		internal void NotifyLoading() {
			OnLoading();
			Loading?.Invoke(this);
		}

		internal void NotifyLoaded() {
			OnLoaded();
			Loaded?.Invoke(this);
		}

		internal void NotifyAppearing() {
			OnAppearing();
			Appearing?.Invoke(this);
		}

		internal void NotifyAppeared() {
			OnAppeared();
			Appeared?.Invoke(this);
		}

		internal void NotifyDisappearing(out bool cancel) {
			OnDisappearing(out cancel);
			Disappearing?.Invoke(this);
		}

		internal void NotifyDisappeared() {
			OnDisappeared();
			Disappeared?.Invoke(this);
		}

		internal void NotifyDestroying() {
			OnDestroy();
			Destroying?.Invoke(this);
		}

		private void EnableControls() {
			foreach (var view in _viewCouldFocus.Keys)
				view.CanFocus = _viewCouldFocus[view];
			_viewCouldFocus.Clear();
			this.ColorScheme = Application.Current.ColorScheme;
		}

		private void DisableControls() {
			void LearnViewCanFocus(View view) {
				_viewCouldFocus[view] = view.CanFocus;
				foreach (var child in view.Subviews)
					LearnViewCanFocus(child);
			}
			LearnViewCanFocus(this);
			foreach (var view in _viewCouldFocus.Keys)
				view.CanFocus = false;
			this.ColorScheme = Colors.Error;
		}

	}


	public abstract class Screen<T> : Screen {
		private T _model;

		public virtual T Model {
			get => _model;
			set {
				if (value.Equals(_model))
					return;
				_model = value;
				OnModelChanged();
			}
		}

		public virtual void OnModelChanged() {
		}
	}


}