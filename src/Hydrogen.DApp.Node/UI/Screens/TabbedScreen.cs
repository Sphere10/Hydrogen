// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System.Collections.Generic;
using System.Linq;
using Terminal.Gui;

namespace Hydrogen.DApp.Node.UI;

/// <summary>
/// A screen that is composed of many component screens, driven by a left menu.
/// </summary>
public abstract class TabbedScreen : Screen {
	private readonly Screen[] _componentScreens;
	private FrameView _componentScreenFrame;
	private ListView _componentScreenList;
	private Screen _activeComponentScreen;


	protected TabbedScreen(IEnumerable<Screen> componentScreens) {
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

		foreach (var screen in _componentScreens)
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


public abstract class TabbedScreen<T> : TabbedScreen {

	protected TabbedScreen(T model, params Screen<T>[] componentScreens)
		: this(model, componentScreens as IEnumerable<Screen<T>>) {
	}

	protected TabbedScreen(T model, IEnumerable<Screen<T>> componentScreens)
		: base(componentScreens) {
		Guard.ArgumentNotNull(model, nameof(model));
		Model = model;
		foreach (var screenPart in componentScreens) {
			screenPart.Model = model;
		}
	}

	public virtual T Model { get; set; }

}
