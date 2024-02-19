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
using System.Windows.Forms;
using System.Diagnostics;


namespace Hydrogen.Windows.Forms;

public delegate void ButtonPressedHandler(ApplicationBar source, ApplicationBar.Item button);


public partial class ApplicationBar : ApplicationControl {
	private List<Item> _items;
	private int _buttonHeight = 32;
	private int _buttonsToShow = 0;
	public event ButtonPressedHandler ButtonPressed;

	public ApplicationBar() {
		using (this.EnterUpdateScope()) {
			InitializeComponent();
			_items = new List<Item>();
			ButtonHeight = 32;
		}
	}

	public Control ApplicationBarControl { get; set; }

	public int ButtonHeight {
		get { return _buttonHeight; }
		set {
			_buttonHeight = value;
			_splitContainer.SplitterIncrement = _buttonHeight;
		}
	}

	public void AddItem(Item item) {
		// set up the button
		if (item.Button == null) {
			item.Button = new SquareButton();
		}
		item.Button.Text = item.Text;
		item.Button.Image = item.Image;
		item.Button.Font = new Font(item.Button.Font, FontStyle.Bold);
		item.Button.ButtonStateChanged += new EventHandler(Button_ButtonStateChanged);
		item.Button.BottomBorderColor = Color.Transparent;

		// add to the button objectStream
		_items.Add(item);
		_splitContainer.Panel2.Controls.Add(item.Button);
		_buttonsToShow++;

		// if first button, then auto select and hide button panel
		if (_items.Count == 1) {
			_splitContainer.Panel2Collapsed = true;
			item.Button.Pressed = true;
			ShowMenuControl(item.MenuControl);
		} else {
			_splitContainer.Panel2Collapsed = false;
		}
		PerformCustomLayout();
	}

	public void RemoveItem(Item item) {
		Debug.Assert(_items.Contains(item));
		_items.Remove(item);
		_buttonsToShow--;
	}

	public bool ContainsItem(Item item) {
		return _items.Contains(item);
	}

	public Item[] Items {
		get { return _items.ToArray(); }
	}

	public Item SelectedItem {
		get {
			foreach (Item item in _items) {
				if (item.Button.Pressed) {
					return item;
				}
			}
			return null;
		}
		set {
			if (value != null) {
				value.Button.Pressed = true;
			}
		}
	}

	public virtual void PerformCustomLayout() {
		using (this.EnterUpdateScope()) {

			if (_items != null) {

				// place the buttons in the correct place
				if (!_splitContainer.Panel2Collapsed) {
					if (_items != null) {
						for (int i = 0; i < _items.Count && i < _buttonsToShow; i++) {
							Item item = _items[i];
							item.Button.Location = new Point(0, _buttonHeight * i);
							item.Button.Height = _buttonHeight;
							item.Button.Width = _splitContainer.Panel2.Width;
						}
						// show rest of buttons as 8x8 images in bottom toolbar
					}
					// make the height of panel1 correct
					_splitContainer.Panel1MinSize = 0;
					int buttonPanelHeight = _buttonHeight * _buttonsToShow;

					_splitContainer.Panel1MinSize =
						Tools.Values.ClipValue(
							_splitContainer.Height - _splitContainer.SplitterWidth
							                       - _items.Count * _buttonHeight,
							0,
							_splitContainer.Height
						);


					if (_splitContainer.Height > _buttonHeight) {
						_splitContainer.SplitterDistance =
							Tools.Values.ClipValue(
								_splitContainer.Height - buttonPanelHeight -
								_splitContainer.SplitterWidth,
								_splitContainer.Panel1MinSize + 1,
								_splitContainer.Height
							);
					}
				}

			}
		}
	}

	protected override void OnResize(EventArgs e) {
		base.OnResize(e);
		PerformCustomLayout();
	}

	protected virtual void OnButtonPressed(ApplicationBar source, ApplicationBar.Item button) {
	}

	protected void FireButtonPressedEvent(ApplicationBar source, ApplicationBar.Item button) {
		OnButtonPressed(source, button);
		ButtonPressed?.Invoke(source, button);
	}

	#region Handlers

	void Button_ButtonStateChanged(object sender, EventArgs e) {
		Item source = null;
		// find the source button
		foreach (Item item in _items) {
			if (item.Button == sender) {
				source = item;
				break;
			}
		}
		if (source != null) {
			if (source.Button.Pressed) {
				ShowMenuControl(source.MenuControl);
				FireButtonPressedEvent(this, source);
			}
			// unpress all other buttons
			foreach (Item item in _items) {
				if (item.Button != source.Button) {
					item.Button.Pressed = false;
				}
			}
		}

	}

	private void SplitterMoved(object sender, SplitterEventArgs e) {
		if (!Updating) {
			// calculate the buttons to show
			_buttonsToShow = (int)Math.Ceiling(
				(float)_splitContainer.Panel2.Height / (float)_buttonHeight
			);

			PerformCustomLayout();
		}
	}

	#endregion

	private void ShowMenuControl(Control control) {
		// get the current menu control
		Control currentControl = null;
		if (_splitContainer.Panel1.Controls.Count == 1) {
			currentControl = _splitContainer.Panel1.Controls[0];
		} else {
			// invalid state so just clear all menu controls in there
			_splitContainer.Panel1.Controls.Clear();
		}

		// add the new control
		control.Dock = DockStyle.Fill;
		_splitContainer.Panel1.Controls.Add(control);

		// remove the old control
		if (currentControl != null) {
			_splitContainer.Panel1.Controls.Remove(currentControl);
		}

		ApplicationBarControl = control;
	}


	public class Item {
		private SquareButton _button;
		private Control _menuControl;
		private Image _image;
		private string _text;


		public Item(Control menuControl, Image image, string text)
			: this(null, menuControl, image, text) {
		}

		public Item(SquareButton button, Control menuControl, Image image, string text) {
			_button = button;
			_menuControl = menuControl;
			_image = image;
			_text = text;
		}

		public SquareButton Button {
			get { return _button; }
			set { _button = value; }
		}

		public Control MenuControl {
			get { return _menuControl; }
			set { _menuControl = value; }
		}

		public Image Image {
			get { return _image; }
			set { _image = value; }
		}

		public string Text {
			get { return _text; }
			set { _text = value; }
		}


	}

}
