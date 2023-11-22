// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Hydrogen.Windows.Forms;

public abstract class WizardBase<T> : SyncDisposable, IWizard<T> {
	public event EventHandlerEx Finished;
	private Form _owner;
	private WizardDialog<T> _dialog;
	private WizardScreen<T> _currentVisibleScreen;
	private readonly IFuture<IList<WizardScreen<T>>> _screens;
	private int _currentScreenIndex;
	private bool _started;
	private string _nextText;
	private string _finishText;
	private string _title;

	protected WizardBase(string title, T model, string finishText = null) {
		_screens = Tools.Values.Future.LazyLoad<IList<WizardScreen<T>>>(() => ConstructScreens().ToList());
		Model = model;
		_started = false;
		_nextText = "Next";
		_finishText = finishText ?? "Finish";
		Title = title;
		WizardResult = WizardResult.Cancelled;
	}

	public string Title {
		get { return _title; }
		set {
			_title = value;
			if (_dialog != null) {
				_dialog.Text = _title;
			}
		}
	}

	public T Model { get; private set; }

	public bool HasNext {
		get {
			CheckStarted();
			return _currentScreenIndex < _screens.Value.Count - 1;
		}
	}

	public bool HasPrevious {
		get {
			CheckStarted();
			return _currentScreenIndex > 0;
		}
	}

	public bool HideNext {
		get {
			CheckStarted();
			return _dialog._nextButton.Enabled;
		}
		set {
			CheckStarted();
			_dialog._nextButton.Enabled = value;
		}
	}

	public bool HidePrevious {
		get {
			CheckStarted();
			return _dialog._previousButton.Visible;
		}
		set {
			CheckStarted();
			_dialog._previousButton.Visible = !value;
		}
	}

	public string NextText {
		get {
			CheckStarted();
			return _dialog._nextButton.Text;
		}
		set {
			CheckStarted();
			_dialog._nextButton.Text = value;
		}
	}

	public WizardResult WizardResult { get; private set; }

	protected abstract IEnumerable<WizardScreen<T>> ConstructScreens();

	protected override void FreeManagedResources() {
		foreach (var screen in _screens.Value) {
			screen.Dispose();
		}
	}

	public async Task<WizardResult> Start(Form parent) {
		var tcs = new TaskCompletionSource();
		CheckNotStarted();
		_owner = parent;
		_dialog = new WizardDialog<T>();
		_dialog.Text = _title;
		_dialog.WizardManager = this;
		_dialog.FormClosing += (sender, args) => { };
		_dialog.FormClosed += (sender, args) => { tcs.SetResult(); };
		_currentScreenIndex = 0;
		_started = true;
		foreach (var screen in _screens.Value)
			await screen.Initialize();
		var maxWidth = _screens.Value.Max(x => x.Size.Width);
		var maxHeight = _screens.Value.Max(x => x.Size.Height);
		_dialog.Size = new Size(maxWidth, maxHeight) + _dialog.DialogSizeOverhead;
		await PresentScreen(_screens.Value[_currentScreenIndex]);
		_dialog.Show(_owner);
		await tcs.Task;
		return WizardResult;
	}

	public async Task Next() {
		CheckStarted();
		var validation = await _currentVisibleScreen.Validate();
		if (validation.IsFailure) {
			DialogEx.Show(_dialog, SystemIconType.Error, "Error", validation.ErrorMessages.ToParagraphCase(), "OK");
			return;
		}
		await _currentVisibleScreen.OnNext();
		if (HasNext) {
			await PresentScreen(_screens.Value[++_currentScreenIndex]);
		} else await Complete();
	}

	public async Task Previous() {
		CheckStarted();
		if (!HasPrevious)
			return;
		await _currentVisibleScreen.OnPrevious();
		await PresentScreen(_screens.Value[--_currentScreenIndex]);
	}

	public virtual Result CancelRequested() {
		return Result.Default;
	}

	public void RemoveSubsequentScreensOfType(Type type) {
		var formsToRemove = new List<WizardScreen<T>>();
		for (int i = _currentScreenIndex + 1; i < _screens.Value.Count; i++)
			if (_screens.Value[i].GetType() == type)
				formsToRemove.Add(_screens.Value[i]);
		formsToRemove.ForEach(f => _screens.Value.Remove(f));
	}

	public void RemoveSubsequentScreensOfType<U>() => RemoveSubsequentScreensOfType(typeof(U));

	public async Task InjectScreen(WizardScreen<T> screen) {
		CheckStarted();
		await screen.Initialize();
		_screens.Value.Insert(_currentScreenIndex + 1, screen);
		NextText = !HasNext ? _finishText : _nextText;
	}

	public void RemoveScreen(WizardScreen<T> screen) {
		_screens.Value.Remove(screen);
	}


	protected async Task Complete() {
		var validation = await Validate();
		if (validation.IsFailure) {
			DialogEx.Show(_dialog, SystemIconType.Error, "Error", validation.ErrorMessages.ToParagraphCase(), "OK");
			return;
		}
		;
		var finishResult = await Finish();
		if (finishResult.IsFailure) {
			DialogEx.Show(_dialog, SystemIconType.Error, "Error", finishResult.ErrorMessages.ToParagraphCase(true), "OK");
			WizardResult = WizardResult.Error;
			return;
		} else
			WizardResult = WizardResult.Success;
		FireFinishedEvent();
		_dialog.CloseDialog();
		await DisposeAsync();
	}

	protected virtual Task<Result> Validate() => Task.FromResult(Result.Default);

	protected abstract Task<Result> Finish();

	private void FireFinishedEvent() {
		if (Finished != null)
			Finished();
	}

	private async Task PresentScreen(WizardScreen<T> screen) {
		_currentVisibleScreen = screen;
		_currentVisibleScreen.Wizard = this;

		HidePrevious = !HasPrevious;
		NextText = !HasNext ? _finishText : _nextText;

		await _dialog.SetContent(screen);
		await screen.OnPresent();
	}

	private void CheckStarted() {
		if (!_started)
			throw new InvalidOperationException("Wizard has not been started");
	}
	private void CheckNotStarted() {
		if (_started)
			throw new InvalidOperationException("Wizard has already been started");
	}
}
