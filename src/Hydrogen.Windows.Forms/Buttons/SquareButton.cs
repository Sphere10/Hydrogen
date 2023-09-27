// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using System.Drawing.Drawing2D;

namespace Hydrogen.Windows.Forms;

public partial class SquareButton : ButtonBase {
	private Color _topBorderColor = Color.FromArgb(0, 45, 150);
	private Color _bottomBorderColor = Color.FromArgb(0, 45, 150);
	private Color _leftBorderColor = Color.FromArgb(0, 45, 150);
	private Color _rightBorderColor = Color.FromArgb(0, 45, 150);

	protected Color _gradientButtonDark = Color.FromArgb(125, 165, 224);
	protected Color _gradientButtonLight = Color.FromArgb(203, 225, 252);
	protected Color _gradientButtonHoverDark = Color.FromArgb(247, 192, 91);
	protected Color _gradientButtonHoverLight = Color.FromArgb(255, 255, 220);
	protected Color _gradientButtonSelectedDark = Color.FromArgb(239, 150, 21);
	protected Color _gradientButtonSelectedLight = Color.FromArgb(251, 230, 148);
	protected Image image = null;
	protected bool _selected = false;
	private bool _isHovering = false;
	private bool _pressed = false;

	public event EventHandler ButtonStateChanged;

	public SquareButton() {
		InitializeComponent();
	}

	[Description("Dark gradient color of the button"), Category("Appearance")]
	public Color GradientButtonNormalDark {
		get { return _gradientButtonDark; }
		set { _gradientButtonDark = value; }
	}

	[Description("Light gradient color of the button"), Category("Appearance")]
	public Color GradientButtonNormalLight {
		get { return _gradientButtonLight; }
		set { _gradientButtonLight = value; }
	}

	[Description("Dark gradient color of the button when the mouse is moving over it"), Category("Appearance")]
	public Color GradientButtonHoverDark {
		get { return _gradientButtonHoverDark; }
		set { _gradientButtonHoverDark = value; }
	}

	[Description("Light gradient color of the button when the mouse is moving over it"), Category("Appearance")]
	public Color GradientButtonHoverLight {
		get { return _gradientButtonHoverLight; }
		set { _gradientButtonHoverLight = value; }
	}

	[Description("Dark gradient color of the seleced button"), Category("Appearance")]
	public Color GradientButtonSelectedDark {
		get { return _gradientButtonSelectedDark; }
		set { _gradientButtonSelectedDark = value; }
	}

	[Description("Light gradient color of the seleced button"), Category("Appearance")]
	public Color GradientButtonSelectedLight {
		get { return _gradientButtonSelectedLight; }
		set { _gradientButtonSelectedLight = value; }
	}

	[Description("Color of border"), Category("Appearance")]
	public Color TopBorderColor {
		get { return _topBorderColor; }
		set { _topBorderColor = value; }
	}

	public Color LeftBorderColor {
		get { return _leftBorderColor; }
		set { _leftBorderColor = value; }
	}

	public Color RightBorderColor {
		get { return _rightBorderColor; }
		set { _rightBorderColor = value; }
	}

	public Color BottomBorderColor {
		get { return _bottomBorderColor; }
		set { _bottomBorderColor = value; }
	}

	public bool Pressed {
		get { return _pressed; }
		set {
			_pressed = value;
			Invalidate();
		}
	}

	protected override void OnPaint(PaintEventArgs pevent) {
		Brush br;
		Rectangle rect = new Rectangle(0, 0, this.Width, this.Height);
		if (Enabled) {
			if (Pressed)
				if (IsHovering)
					br = new LinearGradientBrush(rect, _gradientButtonSelectedLight, _gradientButtonSelectedDark, 90f);
				else
					br = new LinearGradientBrush(rect, _gradientButtonSelectedLight, _gradientButtonSelectedDark, 90f);
			else if (IsHovering)
				br = new LinearGradientBrush(rect, _gradientButtonHoverLight, _gradientButtonHoverDark, 90f);
			else
				br = new LinearGradientBrush(rect, _gradientButtonLight, _gradientButtonDark, 90f);
		} else
			br = new LinearGradientBrush(rect, _gradientButtonLight, _gradientButtonDark, 90f);

		// draw body
		pevent.Graphics.FillRectangle(br, 0, 0, this.Width, this.Height);
		br.Dispose();


		// draw border
		using (Pen pen = new Pen(_topBorderColor)) {
			pevent.Graphics.DrawLine(
				pen,
				new Point(0, 0),
				new Point(pevent.ClipRectangle.Width - 1, 0)
			);

			pen.Color = _leftBorderColor;
			pevent.Graphics.DrawLine(
				pen,
				new Point(0, 0),
				new Point(0, pevent.ClipRectangle.Height - 1)
			);

			pen.Color = _rightBorderColor;
			pevent.Graphics.DrawLine(
				pen,
				new Point(pevent.ClipRectangle.Width - 1, 0),
				new Point(pevent.ClipRectangle.Width - 1, pevent.ClipRectangle.Height - 1)
			);

			pen.Color = _bottomBorderColor;
			pevent.Graphics.DrawLine(
				pen,
				new Point(0, pevent.ClipRectangle.Height - 1),
				new Point(pevent.ClipRectangle.Width - 1, pevent.ClipRectangle.Height - 1)
			);
		}




		if (Text.Length > 0)
			pevent.Graphics.DrawString(this.Text, Font, Brushes.Black, 36, this.Height / 2 - Font.Height / 2);

		if (Image != null) {
			pevent.Graphics.DrawImage(Image, 36 / 2 - Image.Width / 2, this.Height / 2 - Image.Height / 2, Image.Width, Image.Height);
		}

	}

	protected bool IsHovering {
		get { return _isHovering; }
		set { _isHovering = value; }
	}

	protected override void OnClick(EventArgs e) {
		if (!Pressed) {
			Pressed = !Pressed;
			if (ButtonStateChanged != null) {
				ButtonStateChanged(this, new EventArgs());
			}
			base.OnClick(e);
		}
	}

	protected override void OnMouseEnter(EventArgs e) {
		IsHovering = true;
		base.OnMouseEnter(e);
	}

	protected override void OnMouseLeave(EventArgs e) {
		IsHovering = false;
		base.OnMouseLeave(e);
	}

}
