// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;
using System.Windows.Forms;
using System.Drawing;
using System.Drawing.Drawing2D;


namespace Hydrogen.Windows.Forms;

public class ExpandingCircle : Form {

	private readonly TimeSpan TimerTick = TimeSpan.FromMilliseconds(2);
	private const int DefaultCircleStartRadius = 2;
	private const int DefaultCircleMaxRadius = 100;
	private const int DefaultCircleThickness = 3;
	private const int DefaultExpansionSpeed = 50;
	private static readonly Color DefaultCircleColor = Color.LightGreen;


	public int StartRadius { get; set; }
	public int FinishRadius { get; set; }
	public int Thickness { get; set; }
	public int ExpansionSpeed { get; set; }
	public Color Color { get; set; }
	public int CentreX { get; set; }
	public int CentreY { get; set; }
	private int Radius { get; set; }
	private DateTime Time0 { get; set; }
	private Timer _timer;


	public ExpandingCircle()
		: this(0, 0, DefaultCircleStartRadius, DefaultCircleThickness, DefaultExpansionSpeed, DefaultCircleColor, DefaultCircleMaxRadius) {
		Enabled = false;
		Keys s;
	}


	private ExpandingCircle(int centreX, int centreY, int startRadius, int thickness, int expansionSpeed, Color color, int finishRadius) {
		// Set flicker-free rendering styles
		SetStyle(ControlStyles.UserPaint, true);
		SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
		SetStyle(ControlStyles.AllPaintingInWmPaint, true);
		SetStyle(ControlStyles.ResizeRedraw, true);
		SetStyle(ControlStyles.SupportsTransparentBackColor, true);

		// Set circle properties
		StartRadius = startRadius;
		FinishRadius = finishRadius;
		Radius = StartRadius;
		Thickness = thickness;
		ExpansionSpeed = expansionSpeed;
		Color = color;
		CentreX = centreX;
		CentreY = centreY;

		// Set Form Properties
		MinimumSize = new Size(1, 1);
		ControlBox = false;
		ShowIcon = false;
		ShowInTaskbar = false;
		Enabled = false;
		Visible = false;
		FormBorderStyle = FormBorderStyle.None;
		ShowInTaskbar = false;
		StartPosition = FormStartPosition.Manual;
		Width = (FinishRadius + Thickness) * 2;
		Height = (FinishRadius + Thickness) * 2;
		Location = new Point(centreX - Width / 2, centreY - Height / 2);
		BackColor = Color.Fuchsia;
		TransparencyKey = Color.Fuchsia;

		// Set timer properties
		_timer = new Timer();
		_timer.Interval = (int)TimerTick.TotalMilliseconds;
		_timer.Tick += new EventHandler(timer_Tick);
	}


	// Makes the window 'transparent' to events
	protected override CreateParams CreateParams {
		get {
			CreateParams cp = base.CreateParams;
			cp.ExStyle |= 0x00000020; // WS_EX_TRANSPARENT
			cp.ExStyle |= 0x00080000; // WS_EX_LAYERED 
			return cp;
		}
	}

	protected override bool ShowWithoutActivation {
		get { return true; }
	}

	public void StartExpanding() {
		Radius = StartRadius;
		Time0 = DateTime.Now;
		_timer.Start();
	}

	public void StopExpanding() {
		_timer.Stop();
		Close();
	}


	public void Expand() {

		DateTime now = DateTime.Now;
		Radius = (int)Math.Round(now.Subtract(Time0).TotalSeconds * ExpansionSpeed, 0);
		if (Radius >= Math.Min(Width / 2, Height / 2)) {
			StopExpanding();
		}
	}


	/// <summary>
	/// Handles the Tick event of the aTimer control.
	/// </summary>
	/// <param name="sender">The source of the event.</param>
	/// <param name="e">The <see cref="T:System.EventArgs"/> instance containing the event data.</param>
	private void timer_Tick(object sender, EventArgs e) {
		Expand();
		Invalidate();
	}

	/// <summary>
	/// Raises the <see cref="E:System.Windows.Forms.Control.Paint"></see> event.
	/// </summary>
	/// <param name="e">A <see cref="T:System.Windows.Forms.PaintEventArgs"></see> that contains the event data.</param>
	bool inPaint = false;

	protected override void OnPaint(PaintEventArgs e) {
		if (!inPaint) {
			inPaint = true;
			e.Graphics.SmoothingMode = SmoothingMode.None;
			float percentExpanded = (float)Radius / (float)FinishRadius;
			Opacity = 1 - percentExpanded; // (float)Math.Sqrt(percentExpanded);
			using (var pen = new Pen(Color, Thickness)) {
				e.Graphics.DrawCircle(pen, (int)Width / 2, (int)Height / 2, Radius);
			}

			base.OnPaint(e);
			inPaint = false;
		}
	}


	// Static Activators
	public static void ShowExpandingCircle(int centreX, int centreY, Color color, int thickness, int maxRadius, int expansionSpeedPixelsPerSecond) {
		var frm = new ExpandingCircle(centreX, centreY, DefaultCircleStartRadius, thickness, expansionSpeedPixelsPerSecond, color, maxRadius);
		frm.ShowInactiveTopmost();
		frm.StartExpanding();
	}


}
