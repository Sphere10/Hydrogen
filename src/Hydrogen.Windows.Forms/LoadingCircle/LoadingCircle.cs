//
// Copyright ©2006, 2007, Martin R. Gagné (martingagne@gmail.com)
// All rights reserved.
//
// Redistribution and use in source and binary forms, with or without modification, 
// are permitted provided that the following conditions are met:
//
//   - Redistributions of source code must retain the above copyright notice, 
//     this list of conditions and the following disclaimer.
//
//   - Redistributions in binary form must reproduce the above copyright notice, 
//     this list of conditions and the following disclaimer in the documentation 
//     and/or other materials provided with the distribution.
// 
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND 
// ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED 
// WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE DISCLAIMED. 
// IN NO EVENT SHALL THE COPYRIGHT OWNER OR CONTRIBUTORS BE LIABLE FOR ANY DIRECT, 
// INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT 
// NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, 
// OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, 
// WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) 
// ARISING IN ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY 
// OF SUCH DAMAGE.
//

using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Windows.Forms;

namespace Hydrogen.Windows.Forms;

public partial class LoadingCircle : Control {
	// Constants =========================================================
	private const double NumberOfDegreesInCircle = 360;
	private const double NumberOfDegreesInHalfCircle = NumberOfDegreesInCircle / 2;
	private const int DefaultInnerCircleRadius = 8;
	private const int DefaultOuterCircleRadius = 10;
	private const int DefaultNumberOfSpoke = 10;
	private const int DefaultSpokeThickness = 4;

	private const int MacOSXInnerCircleRadius = 5;
	private const int MacOSXOuterCircleRadius = 11;
	private const int MacOSXNumberOfSpoke = 12;
	private const int MacOSXSpokeThickness = 2;

	private const int FireFoxInnerCircleRadius = 6;
	private const int FireFoxOuterCircleRadius = 7;
	private const int FireFoxNumberOfSpoke = 9;
	private const int FireFoxSpokeThickness = 4;

	private const int IE7InnerCircleRadius = 8;
	private const int IE7OuterCircleRadius = 9;
	private const int IE7NumberOfSpoke = 24;
	private const int IE7SpokeThickness = 4;


	// Enumeration =======================================================
	public enum StylePresets {
		MacOSX,
		Firefox,
		IE7,
		Custom
	}


	// Attributes ========================================================
	private readonly Color _defaultColor = Color.DarkGray;
	private readonly Timer _timer;
	private bool _isTimerActive;
	private int _numberOfSpoke;
	private int _spokeThickness;
	private int _progressValue;
	private int _outerCircleRadius;
	private int _innerCircleRadius;
	private PointF _centerPoint;
	private Color _color;
	private Color[] _colors;
	private double[] _angles;
	private StylePresets _stylePreset;
	private Control _parentContainerToDisable;
	private Control[] _exclusionControls;

	// Properties ========================================================

	/// <summary>
	/// Gets or sets the lightest color of the circle.
	/// </summary>
	/// <value>The lightest color of the circle.</value>
	[TypeConverter("System.Drawing.ColorConverter"),
	 Category("LoadingCircle"),
	 Description("Sets the color of spoke.")]
	public Color Color {
		get { return _color; }
		set {
			_color = value;

			GenerateColorsPallet();
			Invalidate();
		}
	}

	/// <summary>
	/// Gets or sets the outer circle radius.
	/// </summary>
	/// <value>The outer circle radius.</value>
	[System.ComponentModel.Description("Gets or sets the radius of outer circle."),
	 System.ComponentModel.Category("LoadingCircle")]
	public int OuterCircleRadius {
		get {
			if (_outerCircleRadius == 0)
				_outerCircleRadius = DefaultOuterCircleRadius;

			return _outerCircleRadius;
		}
		set {
			_outerCircleRadius = value;
			Invalidate();
		}
	}

	/// <summary>
	/// Gets or sets the inner circle radius.
	/// </summary>
	/// <value>The inner circle radius.</value>
	[System.ComponentModel.Description("Gets or sets the radius of inner circle."),
	 System.ComponentModel.Category("LoadingCircle")]
	public int InnerCircleRadius {
		get {
			if (_innerCircleRadius == 0)
				_innerCircleRadius = DefaultInnerCircleRadius;

			return _innerCircleRadius;
		}
		set {
			_innerCircleRadius = value;
			Invalidate();
		}
	}

	/// <summary>
	/// Gets or sets the number of spoke.
	/// </summary>
	/// <value>The number of spoke.</value>
	[System.ComponentModel.Description("Gets or sets the number of spoke."),
	 System.ComponentModel.Category("LoadingCircle")]
	public int NumberSpoke {
		get {
			if (_numberOfSpoke == 0)
				_numberOfSpoke = DefaultNumberOfSpoke;

			return _numberOfSpoke;
		}
		set {
			if (_numberOfSpoke != value && _numberOfSpoke > 0) {
				_numberOfSpoke = value;
				GenerateColorsPallet();
				GetSpokesAngles();

				Invalidate();
			}
		}
	}

	/// <summary>
	/// Gets or sets a value indicating whether this <see cref="T:LoadingCircle"/> is active.
	/// </summary>
	/// <value><c>true</c> if active; otherwise, <c>false</c>.</value>
	[System.ComponentModel.Description("Gets or sets the number of spoke."),
	 System.ComponentModel.Category("LoadingCircle")]
	public bool Active {
		get { return _isTimerActive; }
		set {
			_isTimerActive = value;
			ActiveTimer();
		}
	}

	/// <summary>
	/// Gets or sets the spoke thickness.
	/// </summary>
	/// <value>The spoke thickness.</value>
	[System.ComponentModel.Description("Gets or sets the thickness of a spoke."),
	 System.ComponentModel.Category("LoadingCircle")]
	public int SpokeThickness {
		get {
			if (_spokeThickness <= 0)
				_spokeThickness = DefaultSpokeThickness;

			return _spokeThickness;
		}
		set {
			_spokeThickness = value;
			Invalidate();
		}
	}

	/// <summary>
	/// Gets or sets the rotation speed.
	/// </summary>
	/// <value>The rotation speed.</value>
	[System.ComponentModel.Description("Gets or sets the rotation speed. Higher the slower."),
	 System.ComponentModel.Category("LoadingCircle")]
	public int RotationSpeed {
		get { return _timer.Interval; }
		set {
			if (value > 0)
				_timer.Interval = value;
		}
	}


	/// <summary>
	/// Quickly sets the style to one of these presets, or a custom style if desired
	/// </summary>
	/// <value>The style preset.</value>
	[Category("LoadingCircle"),
	 Description("Quickly sets the style to one of these presets, or a custom style if desired"),
	 DefaultValue(typeof(StylePresets), "Custom")]
	public StylePresets StylePreset {
		get { return _stylePreset; }
		set {
			_stylePreset = value;

			switch (_stylePreset) {
				case StylePresets.MacOSX:
					SetCircleAppearance(MacOSXNumberOfSpoke,
						MacOSXSpokeThickness,
						MacOSXInnerCircleRadius,
						MacOSXOuterCircleRadius);
					break;
				case StylePresets.Firefox:
					SetCircleAppearance(FireFoxNumberOfSpoke,
						FireFoxSpokeThickness,
						FireFoxInnerCircleRadius,
						FireFoxOuterCircleRadius);
					break;
				case StylePresets.IE7:
					SetCircleAppearance(IE7NumberOfSpoke,
						IE7SpokeThickness,
						IE7InnerCircleRadius,
						IE7OuterCircleRadius);
					break;
				case StylePresets.Custom:
					SetCircleAppearance(DefaultNumberOfSpoke,
						DefaultSpokeThickness,
						DefaultInnerCircleRadius,
						DefaultOuterCircleRadius);
					break;
			}
		}
	}

	[Category("LoadingCircle")]
	[Description("Shows this control when animating and hides it when stopped")]
	public Control HideStopControl { get; set; }


	// Construtor ========================================================
	/// <summary>
	/// Initializes a new instance of the <see cref="T:LoadingCircle"/> class.
	/// </summary>
	public LoadingCircle() {
		//this.DoubleBuffered = true;
		//base.SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint | ControlStyles.ResizeRedraw, true);
		SetStyle(ControlStyles.UserPaint, true);
		SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
		SetStyle(ControlStyles.DoubleBuffer, true);
		SetStyle(ControlStyles.ResizeRedraw, true);
		SetStyle(ControlStyles.SupportsTransparentBackColor, true);

		this.StylePreset = StylePresets.Custom;
		this.BackColor = Color.Transparent;
		this.Size = new Size(64, 64);
		_color = _defaultColor;

		GenerateColorsPallet();
		GetSpokesAngles();
		GetControlCenterPoint();

		_timer = new Timer();
		_timer.Tick += new EventHandler(aTimer_Tick);
		ActiveTimer();

		this.Resize += new EventHandler(LoadingCircle_Resize);

	}

	// Events ============================================================
	/// <summary>
	/// Handles the Resize event of the LoadingCircle control.
	/// </summary>
	/// <param name="sender">The source of the event.</param>
	/// <param name="e">The <see cref="T:System.EventArgs"/> instance containing the event data.</param>
	private void LoadingCircle_Resize(object sender, EventArgs e) {
		GetControlCenterPoint();
	}

	/// <summary>
	/// Handles the Tick event of the aTimer control.
	/// </summary>
	/// <param name="sender">The source of the event.</param>
	/// <param name="e">The <see cref="T:System.EventArgs"/> instance containing the event data.</param>
	private void aTimer_Tick(object sender, EventArgs e) {
		_progressValue = ++_progressValue % _numberOfSpoke;
		Invalidate();
	}

	/// <summary>
	/// Raises the <see cref="E:System.Windows.Forms.Control.Paint"></see> event.
	/// </summary>
	/// <param name="e">A <see cref="T:System.Windows.Forms.PaintEventArgs"></see> that contains the event data.</param>
	protected override void OnPaint(PaintEventArgs e) {
		if ((this.BackColor == Color.Transparent) && (Parent != null)) {
			using (Bitmap behind = new Bitmap(Parent.Width, Parent.Height)) {
				foreach (Control c in Parent.Controls) {
					if (c != this && c.Bounds.IntersectsWith(this.Bounds)) {
						c.DrawToBitmap(behind, c.Bounds);
					}
				}
				e.Graphics.DrawImage(behind, -Left, -Top);
			}
		}

		if (_numberOfSpoke > 0) {
			e.Graphics.SmoothingMode = SmoothingMode.HighQuality;

			int intPosition = _progressValue;
			for (int intCounter = 0; intCounter < _numberOfSpoke; intCounter++) {
				intPosition = intPosition % _numberOfSpoke;
				DrawLine(e.Graphics,
					GetCoordinate(_centerPoint, _innerCircleRadius, _angles[intPosition]),
					GetCoordinate(_centerPoint, _outerCircleRadius, _angles[intPosition]),
					_colors[intCounter],
					_spokeThickness);
				intPosition++;
			}
		}
		base.OnPaint(e);

	}

	// Overridden Methods ================================================
	/// <summary>
	/// Retrieves the size of a rectangular area into which a control can be fitted.
	/// </summary>
	/// <param name="proposedSize">The custom-sized area for a control.</param>
	/// <returns>
	/// An ordered pair of type <see cref="T:System.Drawing.Size"></see> representing the width and height of a rectangle.
	/// </returns>
	public override Size GetPreferredSize(Size proposedSize) {
		proposedSize.Width =
			(_outerCircleRadius + _spokeThickness) * 2;

		return proposedSize;
	}

	// Methods ===========================================================

	public void StartAnimating(Control containerToDisable = null, params Control[] exclusionControls) {

		if (containerToDisable != null) {
			_parentContainerToDisable = containerToDisable;
			_exclusionControls = exclusionControls.Union(this).Union(Tools.Collection.AsEnumerableWhenNotNull(HideStopControl)).ToArray();
			_parentContainerToDisable.EnableChildren(false, _exclusionControls);
			//this.Parent.Enabled = false;
			this.Enabled = true;
		}
		this.Visible = true;
		this.Active = true;
		if (HideStopControl != null)
			HideStopControl.Visible = true;
	}

	public void StopAnimating() {
		this.Active = false;
		this.Visible = false;
		if (HideStopControl != null)
			HideStopControl.Visible = false;

		if (_parentContainerToDisable != null) {
			_parentContainerToDisable.EnableChildren(true, _exclusionControls);
		}
		_parentContainerToDisable = null;
		_exclusionControls = null;
	}

	public static IDisposable EnterAnimationScope(Control containerControl, float relativeSize = 0.5f, StylePresets style = StylePresets.MacOSX, bool disableControls = false, params Control[] exclusionControls) {
		if (containerControl == null)
			throw new ArgumentNullException("containerControl");

		if (relativeSize < 0.01f || relativeSize > 1f)
			throw new ArgumentOutOfRangeException("relativeSize", relativeSize, "Must be within (0.01, 1.0)");

		var loadingCircle = new LoadingCircle();
		loadingCircle.StylePreset = style;
		Action startAction = () => {
			containerControl.Controls.Add(loadingCircle);
			loadingCircle.BringToFront();
			var sideLength = (int)(Math.Min(containerControl.Width, containerControl.Height) * relativeSize) - 1;
			loadingCircle.Size = new Size(sideLength, sideLength);
			loadingCircle.Location = new Point((containerControl.Width - loadingCircle.Width) / 2, (containerControl.Height - loadingCircle.Height) / 2);
			loadingCircle.InnerCircleRadius = sideLength / 5;
			loadingCircle.OuterCircleRadius = sideLength / 3;
			loadingCircle.SpokeThickness = loadingCircle.InnerCircleRadius / 3;
		};

		startAction();
		loadingCircle.StartAnimating(disableControls ? containerControl : null, exclusionControls);
		return new ActionScope(() => {
			loadingCircle.StopAnimating();
			containerControl.Controls.Remove(loadingCircle);
		});
	}

	public IDisposable BeginAnimationScope() => BeginAnimationScope(null);

	public IDisposable BeginAnimationScope(Control containerToDisable, params Control[] exclusionControls) {
		StartAnimating(containerToDisable, exclusionControls);
		return new ActionScope(StopAnimating);
	}

	//[Obsolete("Use BeginAnimationScope")]
	//public void PerformActionAsync(Control control, Action loadingAction, float relativeSize = 0.5f) {
	//    this.Visible = true;
	//    bool manageContainer = !control.Controls.Contains(this);
	//    if (manageContainer) {
	//        control.Controls.Add(this);
	//        var sideLength = (int) (Math.Min(control.Width, control.Height)*relativeSize) - 1;
	//        this.Size = new Size(sideLength, sideLength);
	//        this.Location = new Point((control.Width - this.Width)/2, (control.Height - this.Height)/2);
	//        this.InnerCircleRadius = sideLength/5;
	//        this.OuterCircleRadius = sideLength/3;
	//        this.SpokeThickness = this.InnerCircleRadius/3;
	//    }
	//    this.Active = true;
	//    control.Enabled = false;
	//    this.BringToFront();
	//    Tools.Threads.QueueAction(() => {
	//        try {
	//            loadingAction();
	//        } finally {
	//            this.BeginInvokeEx(
	//                () => {
	//                    this.Active = false;
	//                    this.Visible = false;
	//                    control.Enabled = true;
	//                    if (manageContainer) {
	//                        control.Controls.Remove(this);
	//                    }
	//                }
	//                );
	//        }
	//    });

	//}

	/// <summary>
	/// Darkens a specified color.
	/// </summary>
	/// <param name="objColor">Color to darken.</param>
	/// <param name="intPercent">The percent of darken.</param>
	/// <returns>The new color generated.</returns>
	private Color Darken(Color objColor, int intPercent) {
		int intRed = objColor.R;
		int intGreen = objColor.G;
		int intBlue = objColor.B;
		return Color.FromArgb(intPercent, Math.Min(intRed, byte.MaxValue), Math.Min(intGreen, byte.MaxValue), Math.Min(intBlue, byte.MaxValue));
	}

	/// <summary>
	/// Generates the colors pallet.
	/// </summary>
	private void GenerateColorsPallet() {
		_colors = GenerateColorsPallet(_color, Active, _numberOfSpoke);
	}

	/// <summary>
	/// Generates the colors pallet.
	/// </summary>
	/// <param name="objColor">Color of the lightest spoke.</param>
	/// <param name="blnShadeColor">if set to <c>true</c> the color will be shaded on X spoke.</param>
	/// <param name="intNbSpoke"></param>
	/// <returns>An array of color used to draw the circle.</returns>
	private Color[] GenerateColorsPallet(Color objColor, bool blnShadeColor, int intNbSpoke) {
		Color[] objColors = new Color[NumberSpoke];

		// Value is used to simulate a gradient feel... For each spoke, the 
		// color will be darken by value in intIncrement.
		byte bytIncrement = (byte)(byte.MaxValue / NumberSpoke);

		//Reset variable in case of multiple passes
		byte percentageOfDarken = 0;

		for (int intCursor = 0; intCursor < NumberSpoke; intCursor++) {
			if (blnShadeColor) {
				if (intCursor == 0 || intCursor < NumberSpoke - intNbSpoke)
					objColors[intCursor] = objColor;
				else {
					// Increment alpha channel color
					percentageOfDarken += bytIncrement;

					// Ensure that we don't exceed the maximum alpha
					// channel value (255)
					if (percentageOfDarken > byte.MaxValue)
						percentageOfDarken = byte.MaxValue;

					// Determine the spoke forecolor
					objColors[intCursor] = Darken(objColor, percentageOfDarken);
				}
			} else
				objColors[intCursor] = objColor;
		}

		return objColors;
	}

	/// <summary>
	/// Gets the control center point.
	/// </summary>
	private void GetControlCenterPoint() {
		_centerPoint = GetControlCenterPoint(this);
	}

	/// <summary>
	/// Gets the control center point.
	/// </summary>
	/// <returns>PointF object</returns>
	private PointF GetControlCenterPoint(Control objControl) {
		return new PointF(objControl.Width / 2, objControl.Height / 2 - 1);
	}

	/// <summary>
	/// Draws the line with GDI+.
	/// </summary>
	/// <param name="objGraphics">The Graphics object.</param>
	/// <param name="objPointOne">The point one.</param>
	/// <param name="objPointTwo">The point two.</param>
	/// <param name="objColor">Color of the spoke.</param>
	/// <param name="intLineThickness">The thickness of spoke.</param>
	private void DrawLine(Graphics objGraphics, PointF objPointOne, PointF objPointTwo,
	                      Color objColor, int intLineThickness) {
		using (Pen objPen = new Pen(new SolidBrush(objColor), intLineThickness)) {
			objPen.StartCap = LineCap.Round;
			objPen.EndCap = LineCap.Round;
			objGraphics.DrawLine(objPen, objPointOne, objPointTwo);
		}
	}

	/// <summary>
	/// Gets the coordinate.
	/// </summary>
	/// <param name="_objCircleCenter">The Circle center.</param>
	/// <param name="_intRadius">The radius.</param>
	/// <param name="_dblAngle">The angle.</param>
	/// <returns></returns>
	private PointF GetCoordinate(PointF _objCircleCenter, int _intRadius, double _dblAngle) {
		double dblAngle = Math.PI * _dblAngle / NumberOfDegreesInHalfCircle;

		return new PointF(_objCircleCenter.X + _intRadius * (float)Math.Cos(dblAngle),
			_objCircleCenter.Y + _intRadius * (float)Math.Sin(dblAngle));
	}

	/// <summary>
	/// Gets the spokes angles.
	/// </summary>
	private void GetSpokesAngles() {
		_angles = GetSpokesAngles(NumberSpoke);
	}

	/// <summary>
	/// Gets the spoke angles.
	/// </summary>
	/// <param name="_shtNumberSpoke">The number spoke.</param>
	/// <returns>An array of angle.</returns>
	private double[] GetSpokesAngles(int _intNumberSpoke) {
		double[] Angles = new double[_intNumberSpoke];
		double dblAngle = (double)NumberOfDegreesInCircle / _intNumberSpoke;

		for (int shtCounter = 0; shtCounter < _intNumberSpoke; shtCounter++)
			Angles[shtCounter] = (shtCounter == 0 ? dblAngle : Angles[shtCounter - 1] + dblAngle);

		return Angles;
	}

	/// <summary>
	/// Actives the timer.
	/// </summary>
	private void ActiveTimer() {
		if (_isTimerActive)
			_timer.Start();
		else {
			_timer.Stop();
			_progressValue = 0;
		}

		GenerateColorsPallet();
		Invalidate();
	}

	/// <summary>
	/// Sets the circle appearance.
	/// </summary>
	/// <param name="numberSpoke">The number spoke.</param>
	/// <param name="spokeThickness">The spoke thickness.</param>
	/// <param name="innerCircleRadius">The inner circle radius.</param>
	/// <param name="outerCircleRadius">The outer circle radius.</param>
	public void SetCircleAppearance(int numberSpoke, int spokeThickness,
	                                int innerCircleRadius, int outerCircleRadius) {
		NumberSpoke = numberSpoke;
		SpokeThickness = spokeThickness;
		InnerCircleRadius = innerCircleRadius;
		OuterCircleRadius = outerCircleRadius;

		Invalidate();
	}


}
