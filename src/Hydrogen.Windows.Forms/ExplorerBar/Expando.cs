// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;
using System.Collections;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Design;
using System.Drawing.Imaging;
using System.Windows.Forms;

namespace Hydrogen.Windows.Forms;

/// <summary>
/// Represents the method that will handle the StateChanged, ExpandoAdded, 
/// and ExpandoRemoved events of an Expando or TaskPane
/// </summary>
/// <param name="sender">The source of the event</param>
/// <param name="e">A ExpandoEventArgs that contains the event data</param>
public delegate void ExpandoEventHandler(object sender, ExpandoEventArgs e);


/// <summary>
/// A Control that replicates the collapsable panels found in 
/// Windows XP's Explorer Bar
/// </summary>
[ToolboxItem(true),
 DefaultEvent("StateChanged"),
 DesignerAttribute(typeof(ExpandoDesigner))]
public class Expando : Control, ISupportInitialize {

	#region EventHandlers

	/// <summary>
	/// Occurs when the value of the Collapsed property changes
	/// </summary>
	public event ExpandoEventHandler StateChanged;

	/// <summary>
	/// Occurs when the value of the TitleImage property changes
	/// </summary>
	public event ExpandoEventHandler TitleImageChanged;

	/// <summary>
	/// Occurs when the value of the SpecialGroup property changes
	/// </summary>
	public event ExpandoEventHandler SpecialGroupChanged;

	/// <summary>
	/// Occurs when the value of the Watermark property changes
	/// </summary>
	public event ExpandoEventHandler WatermarkChanged;

	/// <summary>
	/// Occurs when an item (Control) is added to the Expando
	/// </summary>
	public event ControlEventHandler ItemAdded;

	/// <summary>
	/// Occurs when an item (Control) is removed from the Expando
	/// </summary>
	public event ControlEventHandler ItemRemoved;

	/// <summary>
	/// Occurs when a value in the CustomSettings or CustomHeaderSettings 
	/// proterties changes
	/// </summary>
	public event EventHandler CustomSettingsChanged;

	#endregion


	#region Class Data

	/// <summary>
	/// Required designer variable
	/// </summary>
	private Container _components = null;

	///// <summary>
	///// System settings for the Expando
	///// </summary>
	private ExplorerBarInfo _systemSettings;

	/// <summary>
	/// Is the Expando a special group
	/// </summary>
	private bool _specialGroup;

	/// <summary>
	/// The height of the Expando in its expanded state
	/// </summary>
	private int _expandedHeight;

	/// <summary>
	/// The image displayed on the left side of the titlebar
	/// </summary>
	private Image _titleImage;

	/// <summary>
	/// The height of the header section 
	/// (includes titlebar and title image)
	/// </summary>
	private int _headerHeight;

	/// <summary>
	/// Is the Expando collapsed
	/// </summary>
	private bool _collapsed;

	/// <summary>
	/// The state of the titlebar
	/// </summary>
	private FocusStates _focusState;

	/// <summary>
	/// The height of the titlebar
	/// </summary>
	private int _titleBarHeight;

	/// <summary>
	/// Specifies whether the Expando is allowed to animate
	/// </summary>
	private bool _animate;

	/// <summary>
	/// Spcifies whether the Expando is currently animating a fade
	/// </summary>
	private bool _animatingFade;

	/// <summary>
	/// Spcifies whether the Expando is we currently animating a slide
	/// </summary>
	private bool _animatingSlide;

	/// <summary>
	/// An image of the "client area" which is used 
	/// during a fade animation
	/// </summary>
	private Image _animationImage;

	/// <summary>
	/// An AnimationHelper that help the Expando to animate
	/// </summary>
	private AnimationHelper _animationHelper;

	/// <summary>
	/// The TaskPane the Expando belongs to
	/// </summary>
	private TaskPane _taskpane;

	/// <summary>
	/// Should the Expando layout its items itself
	/// </summary>
	private bool _autoLayout;

	/// <summary>
	/// The last known width of the Expando 
	/// (used while animating)
	/// </summary>
	private int _oldWidth;

	/// <summary>
	/// Specifies whether the Expando is currently initialising
	/// </summary>
	private bool _initialising;

	/// <summary>
	/// Internal list of items contained in the Expando
	/// </summary>
	private ItemCollection _itemList;

	/// <summary>
	/// Internal list of controls that have been hidden
	/// </summary>
	private ArrayList _hiddenControls;

	/// <summary>
	/// A panel the Expando can move its controls onto when it is 
	/// animating from collapsed to expanded.
	/// </summary>
	private AnimationPanel _dummyPanel;

	/// <summary>
	/// Specifies whether the Expando is allowed to collapse
	/// </summary>
	private bool _canCollapse;

	/// <summary>
	/// The height of the Expando at the end of its slide animation
	/// </summary>
	private int _slideEndHeight;

	/// <summary>
	/// The index of the Image that is used as a watermark
	/// </summary>
	private Image _watermark;

	/// <summary>
	/// Specifies whether the Expando should draw a focus rectangle 
	/// when it has focus
	/// </summary>
	private bool _showFocusCues;

	/// <summary>
	/// Specifies whether the Expando is currently performing a 
	/// layout operation
	/// </summary>
	private bool _layout = false;

	/// <summary>
	/// Specifies the custom settings for the Expando
	/// </summary>
	private ExpandoInfo _customSettings;

	/// <summary>
	/// Specifies the custom header settings for the Expando
	/// </summary>
	private HeaderInfo _customHeaderSettings;

	/// <summary>
	/// An array of pre-determined heights for use during a 
	/// fade animation
	/// </summary>
	private int[] _fadeHeights;

	/// <summary>
	/// Specifies whether the Expando should use Windows 
	/// defsult Tab handling mechanism
	/// </summary>
	private bool _useDefaultTabHandling;

	/// <summary>
	/// Specifies the number of times BeginUpdate() has been called
	/// </summary>
	private int _beginUpdateCount;

	/// <summary>
	/// Specifies whether slide animations should be batched
	/// </summary>
	private bool _slideAnimationBatched;

	/// <summary>
	/// Specifies whether the Expando is currently being dragged
	/// </summary>
	private bool _dragging;

	/// <summary>
	/// Specifies the Point that a drag operation started at
	/// </summary>
	private Point _dragStart;

	#endregion


	#region Constructor

	/// <summary>
	/// Initializes a new instance of the Expando class with default settings
	/// </summary>
	public Expando()
		: base() {
		// This call is required by the Windows.Forms Form Designer.
		this._components = new System.ComponentModel.Container();

		// set control styles
		this.SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
		this.SetStyle(ControlStyles.AllPaintingInWmPaint, true);
		this.SetStyle(ControlStyles.UserPaint, true);
		this.SetStyle(ControlStyles.ResizeRedraw, true);
		this.SetStyle(ControlStyles.SupportsTransparentBackColor, true);
		this.SetStyle(ControlStyles.Selectable, true);
		this.TabStop = true;

		// get the system theme settings
		this._systemSettings = ExplorerBarInfo.Default;

		this._customSettings = new ExpandoInfo();
		this._customSettings.Expando = this;
		this._customSettings.SetDefaultEmptyValues();

		this._customHeaderSettings = new HeaderInfo();
		this._customHeaderSettings.Expando = this;
		this._customHeaderSettings.SetDefaultEmptyValues();

		this.BackColor = this._systemSettings.Expando.NormalBackColor;

		// the height of the Expando in the expanded state
		this._expandedHeight = 100;

		// animation
		this._animate = false;
		this._animatingFade = false;
		this._animatingSlide = false;
		this._animationImage = null;
		this._slideEndHeight = -1;
		this._animationHelper = null;
		this._fadeHeights = new int[AnimationHelper.NumAnimationFrames];

		// size
		this.Size = new Size(this._systemSettings.Header.BackImageWidth, this._expandedHeight);
		this._titleBarHeight = this._systemSettings.Header.BackImageHeight;
		this._headerHeight = this._titleBarHeight;
		this._oldWidth = this.Width;

		// start expanded
		this._collapsed = false;

		// not a special group
		this._specialGroup = false;

		// unfocused titlebar
		this._focusState = FocusStates.None;

		// no title image
		this._titleImage = null;
		this._watermark = null;

		this.Font = new Font(this.TitleFont.Name, 8.25f, FontStyle.Regular);

		// don't get the Expando to layout its items itself
		this._autoLayout = false;

		// don't know which TaskPane we belong to
		this._taskpane = null;

		// internal list of items
		this._itemList = new ItemCollection(this);
		this._hiddenControls = new ArrayList();

		// initialise the dummyPanel
		this._dummyPanel = new AnimationPanel();
		this._dummyPanel.Size = this.Size;
		this._dummyPanel.Location = new Point(-1000, 0);

		this._canCollapse = true;

		this._showFocusCues = false;
		this._useDefaultTabHandling = true;

		this.CalcAnimationHeights();

		this._slideAnimationBatched = false;

		this._dragging = false;
		this._dragStart = Point.Empty;

		this._beginUpdateCount = 0;

		this._initialising = false;
		this._layout = false;
	}

	#endregion


	#region Methods

	#region Animation

	#region Fade Collapse/Expand

	/// <summary>
	/// Collapses the group without any animation.  
	/// </summary>
	public void Collapse() {
		this._collapsed = true;

		if (!this.Animating && this.Height != this.HeaderHeight) {
			this.Height = this._headerHeight;

			// fix: Raise StateChanged event
			//      Jewlin (jewlin88@hotmail.com)
			//      22/10/2004
			//      v3.0
			this.OnStateChanged(new ExpandoEventArgs(this));
		}
	}


	/// <summary>
	/// Expands the group without any animation.  
	/// </summary>
	public void Expand() {
		this._collapsed = false;

		if (!this.Animating && this.Height != this.ExpandedHeight) {
			this.Height = this.ExpandedHeight;

			// fix: Raise StateChanged event
			//      Jewlin (jewlin88@hotmail.com)
			//      22/10/2004
			//      v3.0
			this.OnStateChanged(new ExpandoEventArgs(this));
		}
	}


	/// <summary>
	/// Gets the Expando ready to start its collapse/expand animation
	/// </summary>
	protected void StartFadeAnimation() {
		//
		this._animatingFade = true;

		//
		this.SuspendLayout();

		// get an image of the client area that we can
		// use for alpha-blending in our animation
		this._animationImage = this.GetFadeAnimationImage();

		// set each control invisible (otherwise they
		// appear to slide off the bottom of the group)
		foreach (Control control in this.Controls) {
			control.Visible = false;
		}

		// restart the layout engine
		this.ResumeLayout(false);
	}


	/// <summary>
	/// Updates the next "frame" of the animation
	/// </summary>
	/// <param name="animationStepNum">The current step in the animation</param>
	/// <param name="numAnimationSteps">The total number of steps in the animation</param>
	protected void UpdateFadeAnimation(int animationStepNum, int numAnimationSteps) {
		// fix: use the precalculated heights to determine 
		//      the correct height
		//      David Nissimoff (dudi_001@yahoo.com.br)
		//      22/10/2004
		//      v3.0

		// set the height of the group
		if (this._collapsed) {
			this.Height = this._fadeHeights[animationStepNum - 1] + this._headerHeight;
		} else {
			this.Height = (this.ExpandedHeight - this.HeaderHeight) - this._fadeHeights[animationStepNum - 1] + this.HeaderHeight - 1;
		}

		if (this.TaskPane != null) {
			this.TaskPane.DoLayout();
		} else {
			// draw the next frame
			this.Invalidate();
		}
	}


	/// <summary>
	/// Gets the Expando to stop its animation
	/// </summary>
	protected void StopFadeAnimation() {
		//
		this._animatingFade = false;

		//
		this.SuspendLayout();

		// get rid of the image used for the animation
		this._animationImage.Dispose();
		this._animationImage = null;

		// set the final height of the group, depending on
		// whether we are collapsed or expanded
		if (this._collapsed) {
			this.Height = this.HeaderHeight;
		} else {
			this.Height = this.ExpandedHeight;
		}

		// set each control visible again
		foreach (Control control in this.Controls) {
			control.Visible = !this._hiddenControls.Contains(control);
		}

		//
		this.ResumeLayout(true);

		if (this.TaskPane != null) {
			this.TaskPane.DoLayout();
		}
	}


	/// <summary>
	/// Returns an image of the group's display area to be used
	/// in the fade animation
	/// </summary>
	/// <returns>The Image to use during the fade animation</returns>
	protected Image GetFadeAnimationImage() {
		if (this.Height == this.ExpandedHeight) {
			return this.GetExpandedImage();
		} else {
			return this.GetCollapsedImage();
		}
	}


	/// <summary>
	/// Gets the image to be used in the animation while the 
	/// Expando is in its expanded state
	/// </summary>
	/// <returns>The Image to use during the fade animation</returns>
	protected Image GetExpandedImage() {
		// create a new image to draw into
		Bitmap image = new Bitmap(this.Width, this.Height);

		this.DrawToBitmap(image, this.ClientRectangle);

		// return the completed animation image
		return image;
	}


	/// <summary>
	/// Gets the image to be used in the animation while the 
	/// Expando is in its collapsed state
	/// </summary>
	/// <returns>The Image to use during the fade animation</returns>
	protected Image GetCollapsedImage() {
		// this is pretty nasty.  after much experimentation, 
		// this is the least preferred way to get the image as
		// it is a pain in the backside, but it stops any 
		// flickering and it gets xp themed controls to draw 
		// their borders properly.
		// we have to do this in two stages:
		//    1) pretend we're expanded and draw our background,
		//       borders and "client area" into a bitmap
		//    2) set the bitmap as our dummyPanel's background, 
		//       move all our controls onto the dummyPanel and 
		//       get the dummyPanel to print itself

		int width = this.Width;
		int height = this.ExpandedHeight;


		// create a new image to draw that is the same
		// size we would be if we were expanded
		Image backImage = new Bitmap(width, height);

		// get a graphics object we can draw into
		Graphics g = Graphics.FromImage(backImage);

		// draw our parents background
		this.PaintTransparentBackground(g, new Rectangle(0, 0, width, height));

		// don't need to draw the titlebar as it is ignored 
		// when we paint with the animation image, but we do 
		// need to draw the borders and "client area"

		this.OnPaintTitleBarBackground(g);
		this.OnPaintTitleBar(g);

		// borders
		using (SolidBrush brush = new SolidBrush(this.BorderColor)) {
			// top border
			g.FillRectangle(brush,
				this.Border.Left,
				this.HeaderHeight,
				width - this.Border.Left - this.Border.Right,
				this.Border.Top);

			// left border
			g.FillRectangle(brush,
				0,
				this.HeaderHeight,
				this.Border.Left,
				height - this.HeaderHeight);

			// right border
			g.FillRectangle(brush,
				width - this.Border.Right,
				this.HeaderHeight,
				this.Border.Right,
				height - this.HeaderHeight);

			// bottom border
			g.FillRectangle(brush,
				this.Border.Left,
				height - this.Border.Bottom,
				width - this.Border.Left - this.Border.Right,
				this.Border.Bottom);
		}

		// "client area"
		using (SolidBrush brush = new SolidBrush(this.BackColor)) {
			g.FillRectangle(brush,
				this.Border.Left,
				this.HeaderHeight,
				width - this.Border.Left - this.Border.Right,
				height - this.HeaderHeight - this.Border.Bottom - this.Border.Top);
		}

		// check if we have a background image
		if (this.BackImage != null) {
			// tile the backImage
			using (TextureBrush brush = new TextureBrush(this.BackImage, WrapMode.Tile)) {
				g.FillRectangle(brush,
					this.Border.Left,
					this.HeaderHeight,
					width - this.Border.Left - this.Border.Right,
					height - this.HeaderHeight - this.Border.Bottom - this.Border.Top);
			}
		}

		// watermark
		if (this.Watermark != null) {
			// work out a rough location of where the watermark should go
			Rectangle rect = new Rectangle(0, 0, this.Watermark.Width, this.Watermark.Height);
			rect.X = width - this.Border.Right - this.Watermark.Width;
			rect.Y = height - this.Border.Bottom - this.Watermark.Height;

			// shrink the destination rect if necesary so that we
			// can see all of the image

			if (rect.X < 0) {
				rect.X = 0;
			}

			if (rect.Width > this.ClientRectangle.Width) {
				rect.Width = this.ClientRectangle.Width;
			}

			if (rect.Y < this.DisplayRectangle.Top) {
				rect.Y = this.DisplayRectangle.Top;
			}

			if (rect.Height > this.DisplayRectangle.Height) {
				rect.Height = this.DisplayRectangle.Height;
			}

			// draw the watermark
			g.DrawImage(this.Watermark, rect);
		}

		// cleanup resources;
		g.Dispose();


		// make sure the dummyPanel is the same size as our image
		// (we don't want any tiling of the image)
		this._dummyPanel.Size = new Size(width, height);
		this._dummyPanel.HeaderHeight = this.HeaderHeight;
		this._dummyPanel.Border = this.Border;

		// set the image as the dummyPanels background
		this._dummyPanel.BackImage = backImage;

		// move all our controls to the dummyPanel, and then add
		// the dummyPanel to us
		while (this.Controls.Count > 0) {
			Control control = this.Controls[0];

			this.Controls.RemoveAt(0);
			this._dummyPanel.Controls.Add(control);

			control.Visible = !this._hiddenControls.Contains(control);
		}
		this.Controls.Add(this._dummyPanel);


		// create a new image for the dummyPanel to draw itself into
		Bitmap image = new Bitmap(width, height);
		this._dummyPanel.DrawToBitmap(image, _dummyPanel.ClientRectangle);

		this.Controls.Remove(this._dummyPanel);

		// get our controls back
		while (this._dummyPanel.Controls.Count > 0) {
			Control control = this._dummyPanel.Controls[0];

			control.Visible = false;

			this._dummyPanel.Controls.RemoveAt(0);
			this.Controls.Add(control);
		}

		// dispose of the background image
		this._dummyPanel.BackImage = null;
		backImage.Dispose();

		return image;
	}


	// Added: CalcAnimationHeights()
	//        David Nissimoff (dudi_001@yahoo.com.br)
	//        22/10/2004
	//        v3.0

	/// <summary>
	/// Caches the heights that the Expando should be for each frame 
	/// of a fade animation
	/// </summary>
	internal void CalcAnimationHeights() {
		// Windows XP uses a Bezier curve to calculate the height of 
		// an Expando during a fade animation, so here we precalculate 
		// the height of the "client area" for each frame.
		// 
		// I can't describe what's happening better than David Nissimoff, 
		// so here's David's description of what goes on:
		//
		//   "The only thing that I've noticed is that the animation routine 
		// doesn't completely simulate the one used in Windows. After 2 days 
		// of endless tests I have finally discovered what should've been written 
		// to accurately simulate Windows XP behaviour.
		//   I first created a simple application in VB that would copy an 
		// area of the screen (set to one of the Windows' expandos) every time 
		// it changed. Having that information, analyzing every frame of the 
		// animation I could see that it would always be formed of 23 steps.
		//    Once having all of the animation, frame by frame, I could see 
		// that the expando's height obeyed to a bézier curve. For testing 
		// purposes, I have created an application that draws the bézier curve 
		// on top of the frames put side by side, and it matches 100%.
		//    The height of the expando in each step would be the vertical 
		// position of the bézier in the horizontal position(i.e. the step).
		//    A bézier should be drawn into a Graphics object, with x1 set to 
		// 0 (initial step = 0) and y1 to the initial height of the expando to 
		// be animated. The first control point (x2,y2) is defined by:
		//    x2 = (numAnimationSteps / 4) * 3
		//    y2 = (HeightVariation / 4) * 3
		// The second control point (x3,y3) is defined as follows:
		//    x3 = numAnimationSteps / 4
		//    y3 = HeightVariation / 4
		// The end point (x3,y3) would be:
		//    x4 = 22 --> 23 steps = 0 to 22
		//    y4 = FinalAnimationHeight
		// Then, to get the height of the expando on any desired step, you 
		// should call the Bitmap used to create the Graphics and look pixel by 
		// pixel in the column of the step number until you find the curve."
		//
		// I hope that helps ;)

		using (Bitmap bitmap = new Bitmap(this._fadeHeights.Length, this.ExpandedHeight - this.HeaderHeight)) {
			// draw the bezier curve
			using (Graphics g = Graphics.FromImage(bitmap)) {
				g.Clear(Color.White);
				g.DrawBezier(new Pen(Color.Black),
					0,
					bitmap.Height - 1,
					bitmap.Width / 4 * 3,
					bitmap.Height / 4 * 3,
					bitmap.Width / 4,
					bitmap.Height / 4,
					bitmap.Width - 1,
					0);
			}

			// extract heights
			for (int i = 0; i < bitmap.Width; i++) {
				int j = bitmap.Height - 1;

				for (; j > 0; j--) {
					if (bitmap.GetPixel(i, j).R == 0) {
						break;
					}
				}

				this._fadeHeights[i] = j;
			}
		}
	}

	#endregion

	#region Slide Show/Hide

	/// <summary>
	/// Gets the Expando ready to start its show/hide animation
	/// </summary>
	protected internal void StartSlideAnimation() {
		this._animatingSlide = true;

		this._slideEndHeight = this.CalcHeightAndLayout();
	}


	/// <summary>
	/// Updates the next "frame" of a slide animation
	/// </summary>
	/// <param name="animationStepNum">The current step in the animation</param>
	/// <param name="numAnimationSteps">The total number of steps in the animation</param>
	protected internal void UpdateSlideAnimation(int animationStepNum, int numAnimationSteps) {
		// the percentage we need to adjust our height by
		// double step = (1 / (double) numAnimationSteps) * animationStepNum;
		// replacement by: Joel Holdsworth (joel@airwebreathe.org.uk)
		//                 Paolo Messina (ppescher@hotmail.com)
		//                 05/06/2004
		//                 v1.1
		double step = (1.0 - Math.Cos(Math.PI * (double)animationStepNum / (double)numAnimationSteps)) / 2.0;

		// set the height of the group
		this.Height = this._expandedHeight + (int)((this._slideEndHeight - this._expandedHeight) * step);

		if (this.TaskPane != null) {
			this.TaskPane.DoLayout();
		} else {
			// draw the next frame
			this.Invalidate();
		}
	}


	/// <summary>
	/// Gets the Expando to stop its animation
	/// </summary>
	protected internal void StopSlideAnimation() {
		this._animatingSlide = false;

		// make sure we're the right height
		this.Height = this._slideEndHeight;
		this._slideEndHeight = -1;

		this.DoLayout();
	}

	#endregion

	#endregion

	#region Controls

	/// <summary>
	/// Hides the specified Control
	/// </summary>
	/// <param name="control">The Control to hide</param>
	public void HideControl(Control control) {
		this.HideControl(new Control[] { control });
	}


	/// <summary>
	/// Hides the Controls contained in the specified array
	/// </summary>
	/// <param name="controls">The array Controls to hide</param>
	public void HideControl(Control[] controls) {
		// don't bother if we are animating
		if (this.Animating || this.Collapsed) {
			return;
		}

		this.SuspendLayout();

		// flag to check if we actually hid any controls
		bool anyHidden = false;

		foreach (Control control in controls) {
			// hide the control if we own it and it is not already hidden
			if (this.Controls.Contains(control) && !this._hiddenControls.Contains(control)) {
				anyHidden = true;

				control.Visible = false;
				this._hiddenControls.Add(control);
			}
		}

		this.ResumeLayout(false);

		// if we didn't hide any, get out of here
		if (!anyHidden) {
			return;
		}

		//
		if (this._beginUpdateCount > 0) {
			this._slideAnimationBatched = true;

			return;
		}

		// are we able to animate?
		if (!this.AutoLayout || !this.Animate) {
			// guess not
			this.DoLayout();
		} else {
			if (this._animationHelper != null) {
				this._animationHelper.Dispose();
				this._animationHelper = null;
			}

			this._animationHelper = new AnimationHelper(this, AnimationHelper.SlideAnimation);

			this._animationHelper.StartAnimation();
		}
	}


	/// <summary>
	/// Shows the specified Control
	/// </summary>
	/// <param name="control">The Control to show</param>
	public void ShowControl(Control control) {
		this.ShowControl(new Control[] { control });
	}


	/// <summary>
	/// Shows the Controls contained in the specified array
	/// </summary>
	/// <param name="controls">The array Controls to show</param>
	public void ShowControl(Control[] controls) {
		// don't bother if we are animating
		if (this.Animating || this.Collapsed) {
			return;
		}

		this.SuspendLayout();

		// flag to check if any controls were shown
		bool anyHidden = false;

		foreach (Control control in controls) {
			// show the control if we own it and it is not already shown
			if (this.Controls.Contains(control) && this._hiddenControls.Contains(control)) {
				anyHidden = true;

				control.Visible = true;
				this._hiddenControls.Remove(control);
			}
		}

		this.ResumeLayout(false);

		// if we didn't show any, get out of here
		if (!anyHidden) {
			return;
		}

		//
		if (this._beginUpdateCount > 0) {
			this._slideAnimationBatched = true;

			return;
		}

		// are we able to animate?
		if (!this.AutoLayout || !this.Animate) {
			// guess not
			this.DoLayout();
		} else {
			if (this._animationHelper != null) {
				this._animationHelper.Dispose();
				this._animationHelper = null;
			}

			this._animationHelper = new AnimationHelper(this, AnimationHelper.SlideAnimation);

			this._animationHelper.StartAnimation();
		}
	}

	#endregion

	#region Dispose

	/// <summary> 
	/// Releases the unmanaged resources used by the Expando and 
	/// optionally releases the managed resources
	/// </summary>
	/// <param name="disposing">True to release both managed and unmanaged 
	/// resources; false to release only unmanaged resources</param>
	protected override void Dispose(bool disposing) {
		if (disposing) {
			if (_components != null) {
				_components.Dispose();
			}

			if (this._systemSettings != null) {
				this._systemSettings.Dispose();
				this._systemSettings = null;
			}

			if (this._animationHelper != null) {
				this._animationHelper.Dispose();
				this._animationHelper = null;
			}
		}

		base.Dispose(disposing);
	}

	#endregion

	#region Invalidation

	/// <summary>
	/// Invalidates the titlebar area
	/// </summary>
	protected void InvalidateTitleBar() {
		this.Invalidate(new Rectangle(0, 0, this.Width, this._headerHeight), false);
	}

	#endregion

	#region ISupportInitialize Members

	/// <summary>
	/// Signals the object that initialization is starting
	/// </summary>
	public void BeginInit() {
		this._initialising = true;
	}


	/// <summary>
	/// Signals the object that initialization is complete
	/// </summary>
	public void EndInit() {
		this._initialising = false;

		this.DoLayout();

		this.CalcAnimationHeights();
	}


	/// <summary>
	/// Gets whether the Expando is currently initializing
	/// </summary>
	[Browsable(false)]
	public bool Initialising {
		get { return this._initialising; }
	}

	#endregion

	#region Keys

	/// <summary>
	/// Processes a dialog key
	/// </summary>
	/// <param name="keyData">One of the Keys values that represents 
	/// the key to process</param>
	/// <returns>true if the key was processed by the control; 
	/// otherwise, false</returns>
	protected override bool ProcessDialogKey(Keys keyData) {
		if (this.UseDefaultTabHandling || this.Parent == null || !(this.Parent is TaskPane)) {
			return base.ProcessDialogKey(keyData);
		}

		Keys key = keyData & Keys.KeyCode;

		if (key != Keys.Tab) {
			switch (key) {
				case Keys.Left:
				case Keys.Up:
				case Keys.Right:
				case Keys.Down: {
					if (this.ProcessArrowKey(((key == Keys.Right) ? true : (key == Keys.Down)))) {
						return true;
					}

					break;
				}
			}

			return base.ProcessDialogKey(keyData);
		}

		if (key == Keys.Tab) {
			if (this.ProcessTabKey(((keyData & Keys.Shift) == Keys.None))) {
				return true;
			}
		}

		return base.ProcessDialogKey(keyData);
	}


	/// <summary>
	/// Selects the next available control and makes it the active control
	/// </summary>
	/// <param name="forward">true to cycle forward through the controls in 
	/// the Expando; otherwise, false</param>
	/// <returns>true if a control is selected; otherwise, false</returns>
	protected virtual bool ProcessTabKey(bool forward) {
		if (forward) {
			if ((this.Focused && !this.Collapsed) || this.Items.Count == 0) {
				return base.SelectNextControl(this, forward, true, true, false);
			} else {
				return this.Parent.SelectNextControl(this.Items[this.Items.Count - 1], forward, true, true, false);
			}
		} else {
			if (this.Focused || this.Items.Count == 0 || this.Collapsed) {
				return this.Parent.SelectNextControl(this, forward, true, true, false);
			} else {
				this.Select();

				return this.Focused;
			}
		}
	}


	/// <summary>
	/// Selects the next available control and makes it the active control
	/// </summary>
	/// <param name="forward">true to cycle forward through the controls in 
	/// the Expando; otherwise, false</param>
	/// <returns>true if a control is selected; otherwise, false</returns>
	protected virtual bool ProcessArrowKey(bool forward) {
		if (forward) {
			if (this.Focused && !this.Collapsed) {
				return base.SelectNextControl(this, forward, true, true, false);
			} else if ((this.Items.Count > 0 && this.Items[this.Items.Count - 1].Focused) || this.Collapsed) {
				int index = this.TaskPane.Expandos.IndexOf(this);

				if (index < this.TaskPane.Expandos.Count - 1) {
					this.TaskPane.Expandos[index + 1].Select();

					return this.TaskPane.Expandos[index + 1].Focused;
				} else {
					return true;
				}
			}
		} else {
			if (this.Focused) {
				int index = this.TaskPane.Expandos.IndexOf(this);

				if (index > 0) {
					return this.Parent.SelectNextControl(this, forward, true, true, false);
				} else {
					return true;
				}
			} else if (this.Items.Count > 0) {
				if (this.Items[0].Focused) {
					this.Select();

					return this.Focused;
				} else {
					return this.Parent.SelectNextControl(this.FindFocusedChild(), forward, true, true, false);
				}
			}
		}

		return false;
	}


	/// <summary>
	/// Gets the control contained in the Expando that currently has focus
	/// </summary>
	/// <returns>The control contained in the Expando that currently has focus, 
	/// or null if no child controls have focus</returns>
	protected Control FindFocusedChild() {
		if (this.Controls.Count == 0) {
			return null;
		}

		foreach (Control control in this.Controls) {
			if (control.ContainsFocus) {
				return control;
			}
		}

		return null;
	}

	#endregion

	#region Layout

	/// <summary>
	/// Prevents the Expando from drawing until the EndUpdate method is called
	/// </summary>
	public void BeginUpdate() {
		this._beginUpdateCount++;
	}


	/// <summary>
	/// Resumes drawing of the Expando after drawing is suspended by the 
	/// BeginUpdate method
	/// </summary>
	public void EndUpdate() {
		this._beginUpdateCount = Math.Max(--this._beginUpdateCount, 0);

		if (_beginUpdateCount == 0) {
			if (this._slideAnimationBatched) {
				this._slideAnimationBatched = false;

				if (this.Animate && this.AutoLayout) {
					if (this._animationHelper != null) {
						this._animationHelper.Dispose();
						this._animationHelper = null;
					}

					this._animationHelper = new AnimationHelper(this, AnimationHelper.SlideAnimation);

					this._animationHelper.StartAnimation();
				} else {
					this.DoLayout(true);
				}
			} else {
				this.DoLayout(true);
			}
		}
	}


	/// <summary>
	/// Forces the control to apply layout logic to child controls, 
	/// and adjusts the height of the Expando if necessary
	/// </summary>
	public void DoLayout() {
		this.DoLayout(true);
	}


	/// <summary>
	/// Forces the control to apply layout logic to child controls, 
	/// and adjusts the height of the Expando if necessary
	/// </summary>
	public virtual void DoLayout(bool performRealLayout) {
		if (this._layout) {
			return;
		}

		this._layout = true;

		// stop the layout engine
		this.SuspendLayout();

		// work out the height of the header section

		// is there an image to display on the titlebar
		if (this._titleImage != null) {
			// is the image bigger than the height of the titlebar
			if (this._titleImage.Height > this._titleBarHeight) {
				this._headerHeight = this._titleImage.Height;
			}
			// is the image smaller than the height of the titlebar
			else if (this._titleImage.Height < this._titleBarHeight) {
				this._headerHeight = this._titleBarHeight;
			}
			// is the image smaller than the current header height
			else if (this._titleImage.Height < this._headerHeight) {
				this._headerHeight = this._titleImage.Height;
			}
		} else {
			this._headerHeight = this._titleBarHeight;
		}

		// do we need to layout our items
		if (this.AutoLayout) {
			Control c;
			TaskItem ti;
			Point p;

			// work out how wide to make the controls, and where
			// the top of the first control should be
			int y = this.DisplayRectangle.Y + this.Padding.Top;
			int width = this.PseudoClientRect.Width - this.Padding.Left - this.Padding.Right;

			// for each control in our list...
			for (int i = 0; i < this._itemList.Count; i++) {
				c = (Control)this._itemList[i];

				if (this._hiddenControls.Contains(c)) {
					continue;
				}

				// set the starting point
				p = new Point(this.Padding.Left, y);

				// is the control a TaskItem?  if so, we may
				// need to take into account the margins
				if (c is TaskItem) {
					ti = (TaskItem)c;

					// only adjust the y co-ord if this isn't the first item 
					if (i > 0) {
						y += ti.Margin.Top;

						p.Y = y;
					}

					// adjust and set the width and height
					ti.Width = width;
					ti.Height = ti.PreferredHeight;
				} else {
					y += this._systemSettings.TaskItem.Margin.Top;

					p.Y = y;
				}

				// set the location of the control
				c.Location = p;

				// update the next starting point.
				y += c.Height;

				// is the control a TaskItem?  if so, we may
				// need to take into account the bottom margin
				if (i < this._itemList.Count - 1) {
					if (c is TaskItem) {
						ti = (TaskItem)c;

						y += ti.Margin.Bottom;
					} else {
						y += this._systemSettings.TaskItem.Margin.Bottom;
					}
				}
			}

			// workout where the bottom of the Expando should be
			y += this.Padding.Bottom + this.Border.Bottom;

			// adjust the ExpandedHeight if they're not the same
			if (y != this.ExpandedHeight) {
				this.ExpandedHeight = y;

				// if we're not collapsed then we had better change
				// our height as well
				if (!this.Collapsed) {
					this.Height = this.ExpandedHeight;

					// if we belong to a TaskPane then it needs to
					// re-layout its Expandos
					if (this.TaskPane != null) {
						this.TaskPane.DoLayout(true);
					}
				}
			}
		}

		if (this.Collapsed) {
			this.Height = this.HeaderHeight;
		}

		// restart the layout engine
		this.ResumeLayout(performRealLayout);

		this._layout = false;
	}


	/// <summary>
	/// Calculates the height that the Expando would be if a 
	/// call to DoLayout() were made
	/// </summary>
	/// <returns>The height that the Expando would be if a 
	/// call to DoLayout() were made</returns>
	internal int CalcHeightAndLayout() {
		// stop the layout engine
		this.SuspendLayout();

		// work out the height of the header section

		// is there an image to display on the titlebar
		if (this._titleImage != null) {
			// is the image bigger than the height of the titlebar
			if (this._titleImage.Height > this._titleBarHeight) {
				this._headerHeight = this._titleImage.Height;
			}
			// is the image smaller than the height of the titlebar
			else if (this._titleImage.Height < this._titleBarHeight) {
				this._headerHeight = this._titleBarHeight;
			}
			// is the image smaller than the current header height
			else if (this._titleImage.Height < this._headerHeight) {
				this._headerHeight = this._titleImage.Height;
			}
		} else {
			this._headerHeight = this._titleBarHeight;
		}

		int y = -1;

		// do we need to layout our items
		if (this.AutoLayout) {
			Control c;
			TaskItem ti;
			Point p;

			// work out how wide to make the controls, and where
			// the top of the first control should be
			y = this.DisplayRectangle.Y + this.Padding.Top;
			int width = this.PseudoClientRect.Width - this.Padding.Left - this.Padding.Right;

			// for each control in our list...
			for (int i = 0; i < this._itemList.Count; i++) {
				c = (Control)this._itemList[i];

				if (this._hiddenControls.Contains(c)) {
					continue;
				}

				// set the starting point
				p = new Point(this.Padding.Left, y);

				// is the control a TaskItem?  if so, we may
				// need to take into account the margins
				if (c is TaskItem) {
					ti = (TaskItem)c;

					// only adjust the y co-ord if this isn't the first item 
					if (i > 0) {
						y += ti.Margin.Top;

						p.Y = y;
					}

					// adjust and set the width and height
					ti.Width = width;
					ti.Height = ti.PreferredHeight;
				} else {
					y += this._systemSettings.TaskItem.Margin.Top;

					p.Y = y;
				}

				// set the location of the control
				c.Location = p;

				// update the next starting point.
				y += c.Height;

				// is the control a TaskItem?  if so, we may
				// need to take into account the bottom margin
				if (i < this._itemList.Count - 1) {
					if (c is TaskItem) {
						ti = (TaskItem)c;

						y += ti.Margin.Bottom;
					} else {
						y += this._systemSettings.TaskItem.Margin.Bottom;
					}
				}
			}

			// workout where the bottom of the Expando should be
			y += this.Padding.Bottom + this.Border.Bottom;
		}

		// restart the layout engine
		this.ResumeLayout(true);

		return y;
	}


	/// <summary>
	/// Updates the layout of the Expandos items while in design mode, and 
	/// adds/removes itemss from the ControlCollection as necessary
	/// </summary>
	internal void UpdateItems() {
		if (this.Items.Count == this.Controls.Count) {
			// make sure the the items index in the ControlCollection 
			// are the same as in the ItemCollection (indexes in the 
			// ItemCollection may have changed due to the user moving 
			// them around in the editor)
			this.MatchControlCollToItemColl();

			return;
		}

		// were any items added
		if (this.Items.Count > this.Controls.Count) {
			// add any extra items in the ItemCollection to the 
			// ControlCollection
			for (int i = 0; i < this.Items.Count; i++) {
				if (!this.Controls.Contains(this.Items[i])) {
					this.OnItemAdded(new ControlEventArgs(this.Items[i]));
				}
			}
		} else {
			// items were removed
			int i = 0;
			Control control;

			// remove any extra items from the ControlCollection
			while (i < this.Controls.Count) {
				control = (Control)this.Controls[i];

				if (!this.Items.Contains(control)) {
					this.OnItemRemoved(new ControlEventArgs(control));
				} else {
					i++;
				}
			}
		}

		this.Invalidate(true);
	}


	/// <summary>
	/// Make sure the controls index in the ControlCollection 
	/// are the same as in the ItemCollection (indexes in the 
	/// ItemCollection may have changed due to the user moving 
	/// them around in the editor or calling ItemCollection.Move())
	/// </summary>
	internal void MatchControlCollToItemColl() {
		this.SuspendLayout();

		for (int i = 0; i < this.Items.Count; i++) {
			this.Controls.SetChildIndex(this.Items[i], i);
		}

		this.ResumeLayout(false);

		this.DoLayout();

		this.Invalidate(true);
	}


	/// <summary>
	/// Performs the work of scaling the entire control and any child controls
	/// </summary>
	/// <param name="dx">The ratio by which to scale the control horizontally</param>
	/// <param name="dy">The ratio by which to scale the control vertically</param>
	[Obsolete]
	protected override void ScaleCore(float dx, float dy) {
		// fix: need to adjust expanded height when scaling
		//      AndrewEames (andrew@cognex.com)
		//      14/09/2005
		//      v3.3

		base.ScaleCore(dx, dy);

		this._expandedHeight = (int)(_expandedHeight * dy);
	}

	#endregion

	#endregion


	#region Properties

	#region Alignment

	/// <summary>
	/// Gets the alignment of the text in the title bar.
	/// </summary>
	[Browsable(false)]
	public ContentAlignment TitleAlignment {
		get {
			if (this.SpecialGroup) {
				if (this.CustomHeaderSettings.SpecialAlignment != ContentAlignment.MiddleLeft) {
					return this.CustomHeaderSettings.SpecialAlignment;
				}

				return this.SystemSettings.Header.SpecialAlignment;
			}

			if (this.CustomHeaderSettings.NormalAlignment != ContentAlignment.MiddleLeft) {
				return this.CustomHeaderSettings.NormalAlignment;
			}

			return this.SystemSettings.Header.NormalAlignment;
		}
	}

	#endregion

	#region Animation

	/// <summary>
	/// Gets or sets whether the Expando is allowed to animate
	/// </summary>
	[Category("Appearance"),
	 DefaultValue(false),
	 Description("Specifies whether the Expando is allowed to animate")]
	public bool Animate {
		get { return this._animate; }

		set {
			if (this._animate != value) {
				this._animate = value;
			}
		}
	}


	/// <summary>
	/// Gets whether the Expando is currently animating
	/// </summary>
	[Browsable(false)]
	public bool Animating {
		get { return (this._animatingFade || this._animatingSlide); }
	}


	/// <summary>
	/// Gets the Image used by the Expando while it is animating
	/// </summary>
	protected Image AnimationImage {
		get { return this._animationImage; }
	}


	/// <summary>
	/// Gets the height that the Expando should be at the end of its 
	/// slide animation
	/// </summary>
	protected int SlideEndHeight {
		get { return this._slideEndHeight; }
	}

	#endregion

	#region Border

	/// <summary>
	/// Gets the width of the border along each side of the Expando's pane.
	/// </summary>
	[Browsable(false)]
	public Border Border {
		get {
			if (this.SpecialGroup) {
				if (this.CustomSettings.SpecialBorder != Border.Empty) {
					return this.CustomSettings.SpecialBorder;
				}

				return this.SystemSettings.Expando.SpecialBorder;
			}

			if (this.CustomSettings.NormalBorder != Border.Empty) {
				return this.CustomSettings.NormalBorder;
			}

			return this.SystemSettings.Expando.NormalBorder;
		}
	}


	/// <summary>
	/// Gets the color of the border along each side of the Expando's pane.
	/// </summary>
	[Browsable(false)]
	public Color BorderColor {
		get {
			if (this.SpecialGroup) {
				if (this.CustomSettings.SpecialBorderColor != Color.Empty) {
					return this.CustomSettings.SpecialBorderColor;
				}

				return this.SystemSettings.Expando.SpecialBorderColor;
			}

			if (this.CustomSettings.NormalBorderColor != Color.Empty) {
				return this.CustomSettings.NormalBorderColor;
			}

			return this.SystemSettings.Expando.NormalBorderColor;
		}
	}


	/// <summary>
	/// Gets the width of the border along each side of the Expando's Title Bar.
	/// </summary>
	[Browsable(false)]
	public Border TitleBorder {
		get {
			if (this.SpecialGroup) {
				if (this.CustomHeaderSettings.SpecialBorder != Border.Empty) {
					return this.CustomHeaderSettings.SpecialBorder;
				}

				return this.SystemSettings.Header.SpecialBorder;
			}

			if (this.CustomHeaderSettings.NormalBorder != Border.Empty) {
				return this.CustomHeaderSettings.NormalBorder;
			}

			return this.SystemSettings.Header.NormalBorder;
		}
	}

	#endregion

	#region Color

	/// <summary>
	/// Gets the background color of the titlebar
	/// </summary>
	[Browsable(false)]
	public Color TitleBackColor {
		get {
			if (this.SpecialGroup) {
				if (this.CustomHeaderSettings.SpecialBackColor != Color.Empty &&
				    this.CustomHeaderSettings.SpecialBackColor != Color.Transparent) {
					return this.CustomHeaderSettings.SpecialBackColor;
				} else if (this.CustomHeaderSettings.SpecialBorderColor != Color.Empty) {
					return this.CustomHeaderSettings.SpecialBorderColor;
				}

				if (this.SystemSettings.Header.SpecialBackColor != Color.Transparent) {
					return this._systemSettings.Header.SpecialBackColor;
				}

				return this.SystemSettings.Header.SpecialBorderColor;
			}

			if (this.CustomHeaderSettings.NormalBackColor != Color.Empty &&
			    this.CustomHeaderSettings.NormalBackColor != Color.Transparent) {
				return this.CustomHeaderSettings.NormalBackColor;
			} else if (this.CustomHeaderSettings.NormalBorderColor != Color.Empty) {
				return this.CustomHeaderSettings.NormalBorderColor;
			}

			if (this.SystemSettings.Header.NormalBackColor != Color.Transparent) {
				return this._systemSettings.Header.NormalBackColor;
			}

			return this.SystemSettings.Header.NormalBorderColor;
		}
	}


	/// <summary>
	/// Gets whether any of the title bar's gradient colors are empty colors
	/// </summary>
	protected bool AnyCustomTitleGradientsEmpty {
		get {
			if (this.SpecialGroup) {
				if (this.CustomHeaderSettings.SpecialGradientStartColor == Color.Empty) {
					return true;
				} else if (this.CustomHeaderSettings.SpecialGradientEndColor == Color.Empty) {
					return true;
				}
			} else {
				if (this.CustomHeaderSettings.NormalGradientStartColor == Color.Empty) {
					return true;
				} else if (this.CustomHeaderSettings.NormalGradientEndColor == Color.Empty) {
					return true;
				}
			}

			return false;
		}
	}

	#endregion

	#region Client Rectangle

	/// <summary>
	/// Returns a fake Client Rectangle.  
	/// The rectangle takes into account the size of the titlebar 
	/// and borders (these are actually parts of the real 
	/// ClientRectangle)
	/// </summary>
	protected Rectangle PseudoClientRect {
		get {
			return new Rectangle(this.Border.Left,
				this.HeaderHeight + this.Border.Top,
				this.Width - this.Border.Left - this.Border.Right,
				this.Height - this.HeaderHeight - this.Border.Top - this.Border.Bottom);
		}
	}


	/// <summary>
	/// Returns the height of the fake client rectangle
	/// </summary>
	protected int PseudoClientHeight {
		get { return this.Height - this.HeaderHeight - this.Border.Top - this.Border.Bottom; }
	}

	#endregion

	#region Display Rectangle

	/// <summary>
	/// Overrides DisplayRectangle so that docked controls
	/// don't cover the titlebar or borders
	/// </summary>
	[Browsable(false)]
	public override Rectangle DisplayRectangle {
		get {
			return new Rectangle(this.Border.Left,
				this.HeaderHeight + this.Border.Top,
				this.Width - this.Border.Left - this.Border.Right,
				this.ExpandedHeight - this.HeaderHeight - this.Border.Top - this.Border.Bottom);
		}
	}


	/// <summary>
	/// Gets a rectangle that contains the titlebar area
	/// </summary>
	protected Rectangle TitleBarRectangle {
		get {
			return new Rectangle(0,
				this.HeaderHeight - this.TitleBarHeight,
				this.Width,
				this.TitleBarHeight);
		}
	}

	#endregion

	#region Focus

	/// <summary>
	/// Gets or sets a value indicating whether the Expando should display 
	/// focus rectangles
	/// </summary>
	[Category("Appearance"),
	 DefaultValue(false),
	 Description("Determines whether the Expando should display a focus rectangle.")]
	public new bool ShowFocusCues {
		get { return this._showFocusCues; }

		set {
			if (this._showFocusCues != value) {
				this._showFocusCues = value;

				if (this.Focused) {
					this.InvalidateTitleBar();
				}
			}
		}
	}


	/// <summary>
	/// Gets or sets whether the Expando should use Windows 
	/// default Tab handling mechanism
	/// </summary>
	[Category("Appearance"),
	 DefaultValue(true),
	 Description("Specifies whether the Expando should use Windows default Tab handling mechanism")]
	public bool UseDefaultTabHandling {
		get { return this._useDefaultTabHandling; }

		set { this._useDefaultTabHandling = value; }
	}

	#endregion

	#region Fonts

	/// <summary>
	/// Gets the color of the Title Bar's text.
	/// </summary>
	[Browsable(false)]
	public Color TitleForeColor {
		get {
			if (this.SpecialGroup) {
				if (this.CustomHeaderSettings.SpecialTitleColor != Color.Empty) {
					return this.CustomHeaderSettings.SpecialTitleColor;
				}

				return this.SystemSettings.Header.SpecialTitleColor;
			}

			if (this.CustomHeaderSettings.NormalTitleColor != Color.Empty) {
				return this.CustomHeaderSettings.NormalTitleColor;
			}

			return this.SystemSettings.Header.NormalTitleColor;
		}
	}


	/// <summary>
	/// Gets the color of the Title Bar's text when highlighted.
	/// </summary>
	[Browsable(false)]
	public Color TitleHotForeColor {
		get {
			if (this.SpecialGroup) {
				if (this.CustomHeaderSettings.SpecialTitleHotColor != Color.Empty) {
					return this.CustomHeaderSettings.SpecialTitleHotColor;
				}

				return this.SystemSettings.Header.SpecialTitleHotColor;
			}

			if (this.CustomHeaderSettings.NormalTitleHotColor != Color.Empty) {
				return this.CustomHeaderSettings.NormalTitleHotColor;
			}

			return this.SystemSettings.Header.NormalTitleHotColor;
		}
	}


	/// <summary>
	/// Gets the current color of the Title Bar's text, depending 
	/// on the current state of the Expando
	/// </summary>
	[Browsable(false)]
	public Color TitleColor {
		get {
			if (this.FocusState == FocusStates.Mouse) {
				return this.TitleHotForeColor;
			}

			return this.TitleForeColor;
		}
	}


	/// <summary>
	/// Gets the font used to render the Title Bar's text.
	/// </summary>
	[Browsable(false)]
	public Font TitleFont {
		get {
			if (this.CustomHeaderSettings.TitleFont != null) {
				return this.CustomHeaderSettings.TitleFont;
			}

			return this.SystemSettings.Header.TitleFont;
		}
	}

	#endregion

	#region Images

	/// <summary>
	/// Gets the expand/collapse arrow image currently displayed 
	/// in the title bar, depending on the current state of the Expando
	/// </summary>
	[Browsable(false)]
	public Image ArrowImage {
		get {
			// fix: return null if the Expando isn't allowed to 
			//      collapse (this will stop an expand/collapse 
			//      arrow appearing on the titlebar
			//      dani kenan (dani_k@netvision.net.il)
			//      11/10/2004
			//      v2.1
			if (!this.CanCollapse) {
				return null;
			}

			if (this.SpecialGroup) {
				if (this._collapsed) {
					if (this.FocusState == FocusStates.None) {
						if (this.CustomHeaderSettings.SpecialArrowDown != null) {
							return this.CustomHeaderSettings.SpecialArrowDown;
						}

						return this.SystemSettings.Header.SpecialArrowDown;
					} else {
						if (this.CustomHeaderSettings.SpecialArrowDownHot != null) {
							return this.CustomHeaderSettings.SpecialArrowDownHot;
						}

						return this.SystemSettings.Header.SpecialArrowDownHot;
					}
				} else {
					if (this.FocusState == FocusStates.None) {
						if (this.CustomHeaderSettings.SpecialArrowUp != null) {
							return this.CustomHeaderSettings.SpecialArrowUp;
						}

						return this.SystemSettings.Header.SpecialArrowUp;
					} else {
						if (this.CustomHeaderSettings.SpecialArrowUpHot != null) {
							return this.CustomHeaderSettings.SpecialArrowUpHot;
						}

						return this.SystemSettings.Header.SpecialArrowUpHot;
					}
				}
			} else {
				if (this._collapsed) {
					if (this.FocusState == FocusStates.None) {
						if (this.CustomHeaderSettings.NormalArrowDown != null) {
							return this.CustomHeaderSettings.NormalArrowDown;
						}

						return this.SystemSettings.Header.NormalArrowDown;
					} else {
						if (this.CustomHeaderSettings.NormalArrowDownHot != null) {
							return this.CustomHeaderSettings.NormalArrowDownHot;
						}

						return this.SystemSettings.Header.NormalArrowDownHot;
					}
				} else {
					if (this.FocusState == FocusStates.None) {
						if (this.CustomHeaderSettings.NormalArrowUp != null) {
							return this.CustomHeaderSettings.NormalArrowUp;
						}

						return this.SystemSettings.Header.NormalArrowUp;
					} else {
						if (this.CustomHeaderSettings.NormalArrowUpHot != null) {
							return this.CustomHeaderSettings.NormalArrowUpHot;
						}

						return this.SystemSettings.Header.NormalArrowUpHot;
					}
				}
			}
		}
	}


	/// <summary>
	/// Gets the width of the expand/collapse arrow image 
	/// currently displayed in the title bar
	/// </summary>
	protected int ArrowImageWidth {
		get {
			if (this.ArrowImage == null) {
				return 0;
			}

			return this.ArrowImage.Width;
		}
	}


	/// <summary>
	/// Gets the height of the expand/collapse arrow image 
	/// currently displayed in the title bar
	/// </summary>
	protected int ArrowImageHeight {
		get {
			if (this.ArrowImage == null) {
				return 0;
			}

			return this.ArrowImage.Height;
		}
	}


	/// <summary>
	/// The background image used for the Title Bar.
	/// </summary>
	[Browsable(false)]
	public Image TitleBackImage {
		get {
			if (this.SpecialGroup) {
				if (this.CustomHeaderSettings.SpecialBackImage != null) {
					return this.CustomHeaderSettings.SpecialBackImage;
				}

				return this.SystemSettings.Header.SpecialBackImage;
			}

			if (this.CustomHeaderSettings.NormalBackImage != null) {
				return this.CustomHeaderSettings.NormalBackImage;
			}

			return this.SystemSettings.Header.NormalBackImage;
		}
	}


	/// <summary>
	/// Gets the height of the background image used for the Title Bar.
	/// </summary>
	protected int TitleBackImageHeight {
		get { return this.SystemSettings.Header.BackImageHeight; }
	}


	/// <summary>
	/// The image used on the left side of the Title Bar.
	/// </summary>
	[Category("Appearance"),
	 DefaultValue(null),
	 Description("The image used on the left side of the Title Bar.")]
	public Image TitleImage {
		get { return this._titleImage; }

		set {
			this._titleImage = value;

			this.DoLayout();

			this.InvalidateTitleBar();

			OnTitleImageChanged(new ExpandoEventArgs(this));
		}
	}


	/// <summary>
	/// The width of the image used on the left side of the Title Bar.
	/// </summary>
	protected int TitleImageWidth {
		get {
			if (this.TitleImage == null) {
				return 0;
			}

			return this.TitleImage.Width;
		}
	}


	/// <summary>
	/// The height of the image used on the left side of the Title Bar.
	/// </summary>
	protected int TitleImageHeight {
		get {
			if (this.TitleImage == null) {
				return 0;
			}

			return this.TitleImage.Height;
		}
	}


	/// <summary>
	/// Gets the Image that is used as a watermark in the Expando's 
	/// client area
	/// </summary>
	[Category("Appearance"),
	 DefaultValue(null),
	 Description("The Image used as a watermark in the client area of the Expando.")]
	public Image Watermark {
		get { return this._watermark; }

		set {
			if (this._watermark != value) {
				this._watermark = value;

				this.Invalidate();

				OnWatermarkChanged(new ExpandoEventArgs(this));
			}
		}
	}


	/// <summary>
	/// The background image used for the Expandos content area.
	/// </summary>
	[Browsable(false)]
	public Image BackImage {
		get {
			if (this.SpecialGroup) {
				if (this.CustomSettings.SpecialBackImage != null) {
					return this.CustomSettings.SpecialBackImage;
				}

				return this.SystemSettings.Expando.SpecialBackImage;
			}

			if (this.CustomSettings.NormalBackImage != null) {
				return this.CustomSettings.NormalBackImage;
			}

			return this.SystemSettings.Expando.NormalBackImage;
		}
	}

	#endregion

	#region Items

	/// <summary>
	/// An Expando.ItemCollection representing the collection of 
	/// Controls contained within the Expando
	/// </summary>
	[Category("Behavior"),
	 DefaultValue(null),
	 Description("The Controls contained in the Expando"),
	 DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
	 Editor(typeof(ItemCollectionEditor), typeof(UITypeEditor))]
	public Expando.ItemCollection Items {
		get { return this._itemList; }
	}


	/// <summary>
	/// A Control.ControlCollection representing the collection of 
	/// controls contained within the control
	/// </summary>
	[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
	public new Control.ControlCollection Controls {
		get { return base.Controls; }
	}

	#endregion

	#region Layout

	/// <summary>
	/// Gets or sets whether the Expando will automagically layout its items
	/// </summary>
	[Bindable(true),
	 Category("Layout"),
	 DefaultValue(false),
	 Description("The AutoLayout property determines whether the Expando will automagically layout its items.")]
	public bool AutoLayout {
		get { return this._autoLayout; }

		set {
			this._autoLayout = value;

			if (this._autoLayout) {
				this.DoLayout();
			}
		}
	}

	#endregion

	#region Padding

	/// <summary>
	/// Gets the amount of space between the border and items along 
	/// each side of the Expando.
	/// </summary>
	[Browsable(false)]
	public new PaddingEx Padding {
		get {
			if (this.SpecialGroup) {
				if (this.CustomSettings.SpecialPadding != PaddingEx.Empty) {
					return this.CustomSettings.SpecialPadding;
				}

				return this.SystemSettings.Expando.SpecialPadding;
			}

			if (this.CustomSettings.NormalPadding != PaddingEx.Empty) {
				return this.CustomSettings.NormalPadding;
			}

			return this.SystemSettings.Expando.NormalPadding;
		}
	}


	/// <summary>
	/// Gets the amount of space between the border and items along 
	/// each side of the Title Bar.
	/// </summary>
	[Browsable(false)]
	public PaddingEx TitlePadding {
		get {
			if (this.SpecialGroup) {
				if (this.CustomHeaderSettings.SpecialPadding != PaddingEx.Empty) {
					return this.CustomHeaderSettings.SpecialPadding;
				}

				return this.SystemSettings.Header.SpecialPadding;
			}

			if (this.CustomHeaderSettings.NormalPadding != PaddingEx.Empty) {
				return this.CustomHeaderSettings.NormalPadding;
			}

			return this.SystemSettings.Header.NormalPadding;
		}
	}

	#endregion

	#region Size

	/// <summary>
	/// Gets or sets the height and width of the control
	/// </summary>
	public new Size Size {
		get { return base.Size; }

		set {
			if (!this.Size.Equals(value)) {
				if (!this.Animating) {
					this.Width = value.Width;

					if (!this.Initialising) {
						this.ExpandedHeight = value.Height; //HS THIS LINE THROWS -- VALUES NOT SET PROPERLY
					}
				}
			}
		}
	}


	/// <summary>
	/// Specifies whether the Size property should be 
	/// serialized at design time
	/// </summary>
	/// <returns>true if the Size property should be 
	/// serialized, false otherwise</returns>
	private bool ShouldSerializeSize() {
		return this.TaskPane != null;
	}


	/// <summary>
	/// Gets the height of the Expando in its expanded state
	/// </summary>
	[Bindable(true),
	 Category("Layout"),
	 DefaultValue(100),
	 Description("The height of the Expando in its expanded state.")]
	public int ExpandedHeight {
		get { return this._expandedHeight; }

		set {
			this._expandedHeight = value;

			this.CalcAnimationHeights();

			if (!this.Collapsed && !this.Animating) {
				this.Height = this._expandedHeight;

				if (this.TaskPane != null) {
					this.TaskPane.DoLayout();
				}
			}
		}
	}


	/// <summary>
	/// Gets the height of the header section of the Expando
	/// </summary>
	protected int HeaderHeight {
		get { return this._headerHeight; }
	}


	/// <summary>
	/// Gets the height of the titlebar
	/// </summary>
	protected int TitleBarHeight {
		get { return this._titleBarHeight; }
	}

	#endregion

	#region Special Groups

	/// <summary>
	/// Gets or sets whether the Expando should be rendered as a Special Group.
	/// </summary>
	[Bindable(true),
	 Category("Appearance"),
	 DefaultValue(false),
	 Description("The SpecialGroup property determines whether the Expando will be rendered as a SpecialGroup.")]
	public bool SpecialGroup {
		get { return this._specialGroup; }

		set {
			this._specialGroup = value;

			this.DoLayout();

			if (this._specialGroup) {
				if (this.CustomSettings.SpecialBackColor != Color.Empty) {
					this.BackColor = this.CustomSettings.SpecialBackColor;
				} else {
					this.BackColor = this.SystemSettings.Expando.SpecialBackColor;
				}
			} else {
				if (this.CustomSettings.NormalBackColor != Color.Empty) {
					this.BackColor = this.CustomSettings.NormalBackColor;
				} else {
					this.BackColor = this.SystemSettings.Expando.NormalBackColor;
				}
			}

			this.Invalidate();

			OnSpecialGroupChanged(new ExpandoEventArgs(this));
		}
	}

	#endregion

	#region State

	/// <summary>
	/// Gets or sets whether the Expando is collapsed.
	/// </summary>
	[Bindable(true),
	 Category("Appearance"),
	 DefaultValue(false),
	 Description("The Collapsed property determines whether the Expando is collapsed.")]
	public bool Collapsed {
		get { return this._collapsed; }

		set {
			if (this._collapsed != value) {
				// if we're supposed to collapse, check if we can
				if (value && !this.CanCollapse) {
					// looks like we can't so time to bail
					return;
				}

				this._collapsed = value;

				// only animate if we're allowed to, we're not in 
				// design mode and we're not initialising
				if (this.Animate && !this.DesignMode && !this.Initialising) {
					if (this._animationHelper != null) {
						this._animationHelper.Dispose();
						this._animationHelper = null;
					}

					this._animationHelper = new AnimationHelper(this, AnimationHelper.FadeAnimation);

					this.OnStateChanged(new ExpandoEventArgs(this));

					this._animationHelper.StartAnimation();
				} else {
					if (this._collapsed) {
						this.Collapse();
					} else {
						this.Expand();
					}

					// don't need to raise OnStateChanged as 
					// Collapse() or Expand() will do it for us
				}
			}
		}
	}


	/// <summary>
	/// Gets or sets whether the title bar is in a highlighted state.
	/// </summary>
	[Browsable(false)]
	protected internal FocusStates FocusState {
		get { return this._focusState; }

		set {
			// fix: if the Expando isn't allowed to collapse, 
			//      don't update the titlebar highlight
			//      dani kenan (dani_k@netvision.net.il)
			//      11/10/2004
			//      v2.1
			if (!this.CanCollapse) {
				value = FocusStates.None;
			}

			if (this._focusState != value) {
				this._focusState = value;

				this.InvalidateTitleBar();

				if (this._focusState == FocusStates.Mouse) {
					this.Cursor = Cursors.Hand;
				} else {
					this.Cursor = Cursors.Default;
				}
			}
		}
	}


	/// <summary>
	/// Gets or sets whether the Expando is able to collapse
	/// </summary>
	[Bindable(true),
	 Category("Behavior"),
	 DefaultValue(true),
	 Description("The CanCollapse property determines whether the Expando is able to collapse.")]
	public bool CanCollapse {
		get { return this._canCollapse; }

		set {
			if (this._canCollapse != value) {
				this._canCollapse = value;

				// if the Expando is collapsed and it's not allowed 
				// to collapse, then we had better expand it
				if (!this._canCollapse && this.Collapsed) {
					this.Collapsed = false;
				}

				this.InvalidateTitleBar();
			}
		}
	}

	#endregion

	#region System Settings

	///// <summary>
	///// Gets or sets the system settings for the Expando
	///// </summary>
	[Browsable(false)]
	protected internal ExplorerBarInfo SystemSettings {
		get { return this._systemSettings; }

		set {
			// make sure we have a new value
			if (this._systemSettings != value) {
				this.SuspendLayout();

				// get rid of the old settings
				if (this._systemSettings != null) {
					this._systemSettings.Dispose();
					this._systemSettings = null;
				}

				// set the new settings
				this._systemSettings = value;

				this._titleBarHeight = this._systemSettings.Header.BackImageHeight;

				// is there an image to display on the titlebar
				if (this._titleImage != null) {
					// is the image bigger than the height of the titlebar
					if (this._titleImage.Height > this._titleBarHeight) {
						this._headerHeight = this._titleImage.Height;
					}
					// is the image smaller than the height of the titlebar
					else if (this._titleImage.Height < this._titleBarHeight) {
						this._headerHeight = this._titleBarHeight;
					}
					// is the image smaller than the current header height
					else if (this._titleImage.Height < this._headerHeight) {
						this._headerHeight = this._titleImage.Height;
					}
				} else {
					this._headerHeight = this._titleBarHeight;
				}

				if (this.SpecialGroup) {
					if (this.CustomSettings.SpecialBackColor != Color.Empty) {
						this.BackColor = this.CustomSettings.SpecialBackColor;
					} else {
						this.BackColor = this.SystemSettings.Expando.SpecialBackColor;
					}
				} else {
					if (this.CustomSettings.NormalBackColor != Color.Empty) {
						this.BackColor = this.CustomSettings.NormalBackColor;
					} else {
						this.BackColor = this.SystemSettings.Expando.NormalBackColor;
					}
				}

				// update the system settings for each TaskItem
				for (int i = 0; i < this._itemList.Count; i++) {
					Control control = (Control)this._itemList[i];

					if (control is TaskItem) {
						((TaskItem)control).SystemSettings = this._systemSettings;
					}
				}

				this.ResumeLayout(false);

				// if our parent is not an TaskPane then re-layout the 
				// Expando (don't need to do this if our parent is a 
				// TaskPane as it will tell us when to do it)
				if (this.TaskPane == null) {
					this.DoLayout();
				}
			}
		}
	}


	/// <summary>
	/// Gets the custom settings for the Expando
	/// </summary>
	[Category("Appearance"),
	 Description(""),
	 DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
	 TypeConverter(typeof(ExpandoInfoConverter))]
	public ExpandoInfo CustomSettings {
		get { return this._customSettings; }
		internal set { this._customSettings = value; }
	}


	/// <summary>
	/// Gets the custom header settings for the Expando
	/// </summary>
	[Category("Appearance"),
	 Description(""),
	 DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
	 TypeConverter(typeof(HeaderInfoConverter))]
	public HeaderInfo CustomHeaderSettings {
		get { return this._customHeaderSettings; }
		internal set { this._customHeaderSettings = value; }
	}


	/// <summary>
	/// Resets the custom settings to their default values
	/// </summary>
	public void ResetCustomSettings() {
		this.CustomSettings.SetDefaultEmptyValues();
		this.CustomHeaderSettings.SetDefaultEmptyValues();

		this.FireCustomSettingsChanged(EventArgs.Empty);
	}

	#endregion

	#region TaskPane

	/// <summary>
	/// Gets or sets the TaskPane the Expando belongs to
	/// </summary>
	protected internal TaskPane TaskPane {
		get { return this._taskpane; }

		set {
			this._taskpane = value;

			if (value != null) {
				this.SystemSettings = this.TaskPane.SystemSettings;
			}
		}
	}

	#endregion

	#region Text

	/// <summary>
	/// Gets or sets the text displayed on the titlebar
	/// </summary>
	public override string Text {
		get { return base.Text; }

		set {
			base.Text = value;

			this.InvalidateTitleBar();
		}
	}

	#endregion

	#region Visible

	/// <summary>
	/// Gets or sets a value indicating whether the Expando is displayed
	/// </summary>
	public new bool Visible {
		get { return base.Visible; }

		set {
			// fix: TaskPane will now perform a layout if the 
			//      Expando is to become invisible and the TaskPane 
			//      is currently invisible
			//      Brian Nottingham (nottinbe@slu.edu)
			//      22/12/2004
			//      v3.0
			//if (base.Visible != value)
			if (base.Visible != value || (!value && this.Parent != null && !this.Parent.Visible)) {
				base.Visible = value;

				if (this.TaskPane != null) {
					this.TaskPane.DoLayout();
				}
			}
		}
	}

	#endregion

	#endregion


	#region Events

	#region Controls

	/// <summary>
	/// Raises the ControlAdded event
	/// </summary>
	/// <param name="e">A ControlEventArgs that contains the event data</param>
	protected override void OnControlAdded(ControlEventArgs e) {
		// don't do anything if we are animating
		// (as we're probably the ones who added the control)
		if (this.Animating) {
			return;
		}

		base.OnControlAdded(e);

		// add the control to the ItemCollection if necessary
		if (!this.Items.Contains(e.Control)) {
			this.Items.Add(e.Control);
		}
	}


	/// <summary>
	/// Raises the ControlRemoved event
	/// </summary>
	/// <param name="e">A ControlEventArgs that contains the event data</param>
	protected override void OnControlRemoved(ControlEventArgs e) {
		// don't do anything if we are animating 
		// (as we're probably the ones who removed the control)
		if (this.Animating) {
			return;
		}

		base.OnControlRemoved(e);

		// remove the control from the itemList
		if (this.Items.Contains(e.Control)) {
			this.Items.Remove(e.Control);
		}

		// update the layout of the controls
		this.DoLayout();
	}

	#endregion

	#region Custom Settings

	/// <summary>
	/// Raises the CustomSettingsChanged event
	/// </summary>
	/// <param name="e">An EventArgs that contains the event data</param>
	internal void FireCustomSettingsChanged(EventArgs e) {
		this._titleBarHeight = this.TitleBackImageHeight;

		// is there an image to display on the titlebar
		if (this._titleImage != null) {
			// is the image bigger than the height of the titlebar
			if (this._titleImage.Height > this._titleBarHeight) {
				this._headerHeight = this._titleImage.Height;
			}
			// is the image smaller than the height of the titlebar
			else if (this._titleImage.Height < this._titleBarHeight) {
				this._headerHeight = this._titleBarHeight;
			}
			// is the image smaller than the current header height
			else if (this._titleImage.Height < this._headerHeight) {
				this._headerHeight = this._titleImage.Height;
			}
		} else {
			this._headerHeight = this._titleBarHeight;
		}

		if (this.SpecialGroup) {
			if (this.CustomSettings.SpecialBackColor != Color.Empty) {
				this.BackColor = this.CustomSettings.SpecialBackColor;
			} else {
				this.BackColor = this.SystemSettings.Expando.SpecialBackColor;
			}
		} else {
			if (this.CustomSettings.NormalBackColor != Color.Empty) {
				this.BackColor = this.CustomSettings.NormalBackColor;
			} else {
				this.BackColor = this.SystemSettings.Expando.NormalBackColor;
			}
		}

		this.DoLayout();

		this.Invalidate(true);

		this.OnCustomSettingsChanged(e);
	}


	/// <summary>
	/// Raises the CustomSettingsChanged event
	/// </summary>
	/// <param name="e">An EventArgs that contains the event data</param>
	protected virtual void OnCustomSettingsChanged(EventArgs e) {
		if (CustomSettingsChanged != null) {
			CustomSettingsChanged(this, e);
		}
	}

	#endregion

	#region Expando

	/// <summary>
	/// Raises the StateChanged event
	/// </summary>
	/// <param name="e">An ExpandoStateChangedEventArgs that contains the event data</param>
	protected virtual void OnStateChanged(ExpandoEventArgs e) {
		if (StateChanged != null) {
			StateChanged(this, e);
		}
	}


	/// <summary>
	/// Raises the TitleImageChanged event
	/// </summary>
	/// <param name="e">An ExpandoEventArgs that contains the event data</param>
	protected virtual void OnTitleImageChanged(ExpandoEventArgs e) {
		if (TitleImageChanged != null) {
			TitleImageChanged(this, e);
		}
	}


	/// <summary>
	/// Raises the SpecialGroupChanged event
	/// </summary>
	/// <param name="e">An ExpandoEventArgs that contains the event data</param>
	protected virtual void OnSpecialGroupChanged(ExpandoEventArgs e) {
		if (SpecialGroupChanged != null) {
			SpecialGroupChanged(this, e);
		}
	}


	/// <summary>
	/// Raises the WatermarkChanged event
	/// </summary>
	/// <param name="e">An ExpandoEventArgs that contains the event data</param>
	protected virtual void OnWatermarkChanged(ExpandoEventArgs e) {
		if (WatermarkChanged != null) {
			WatermarkChanged(this, e);
		}
	}

	#endregion

	#region Focus

	/// <summary>
	/// Raises the GotFocus event
	/// </summary>
	/// <param name="e">An EventArgs that contains the event data</param>
	protected override void OnGotFocus(EventArgs e) {
		base.OnGotFocus(e);

		this.InvalidateTitleBar();
	}


	/// <summary>
	/// Raises the LostFocus event
	/// </summary>
	/// <param name="e">An EventArgs that contains the event data</param>
	protected override void OnLostFocus(EventArgs e) {
		base.OnLostFocus(e);

		this.InvalidateTitleBar();
	}

	#endregion

	#region Items

	/// <summary>
	/// Raises the ItemAdded event
	/// </summary>
	/// <param name="e">A ControlEventArgs that contains the event data</param>
	protected virtual void OnItemAdded(ControlEventArgs e) {
		// add the expando to the ControlCollection if it hasn't already
		if (!this.Controls.Contains(e.Control)) {
			this.Controls.Add(e.Control);
		}

		// check if the control is a TaskItem
		if (e.Control is TaskItem) {
			TaskItem item = (TaskItem)e.Control;

			// set anchor styles
			item.Anchor = (AnchorStyles.Left | AnchorStyles.Top | AnchorStyles.Right);

			// tell the TaskItem who's its daddy...
			item.Expando = this;
			item.SystemSettings = this._systemSettings;
		}

		// update the layout of the controls
		this.DoLayout();

		//
		if (ItemAdded != null) {
			ItemAdded(this, e);
		}
	}


	/// <summary>
	/// Raises the ItemRemoved event
	/// </summary>
	/// <param name="e">A ControlEventArgs that contains the event data</param>
	protected virtual void OnItemRemoved(ControlEventArgs e) {
		// remove the control from the ControlCollection if it hasn't already
		if (this.Controls.Contains(e.Control)) {
			this.Controls.Remove(e.Control);
		}

		// update the layout of the controls
		this.DoLayout();

		//
		if (ItemRemoved != null) {
			ItemRemoved(this, e);
		}
	}

	#endregion

	#region Keys

	/// <summary>
	/// Raises the KeyUp event
	/// </summary>
	/// <param name="e">A KeyEventArgs that contains the event data</param>
	protected override void OnKeyUp(KeyEventArgs e) {
		// fix: should call OnKeyUp instead of OnKeyDown
		//      Simon Cropp (simonc@avanade.com)
		//      14/09/2005
		//      v3.3
		base.OnKeyUp(e);

		if (e.KeyCode == Keys.Space || e.KeyCode == Keys.Enter) {
			this.Collapsed = !this.Collapsed;
		}
	}

	#endregion

	#region Location

	/// <summary>
	/// Raises the LocationChanged event
	/// </summary>
	/// <param name="e">An EventArgs that contains the event data</param>
	protected override void OnLocationChanged(EventArgs e) {
		base.OnLocationChanged(e);

		// sometimes the title image gets cropped (why???) if the 
		// expando is scrolled from off-screen to on-screen so we'll 
		// repaint the titlebar if the expando has a titlebar image 
		// and it is taller then the titlebar
		if (this.TitleImage != null && this.TitleImageHeight > this.TitleBarHeight) {
			this.InvalidateTitleBar();
		}
	}

	#endregion

	#region Mouse

	/// <summary>
	/// Raises the MouseUp event
	/// </summary>
	/// <param name="e">A MouseEventArgs that contains the event data</param>
	protected override void OnMouseUp(MouseEventArgs e) {
		base.OnMouseUp(e);

		// was it the left mouse button
		if (e.Button == MouseButtons.Left) {
			if (this._dragging) {
				this.Cursor = Cursors.Default;

				this._dragging = false;

				this.TaskPane.DropExpando(this);
			} else {
				// was it in the titlebar area
				if (e.Y < this.HeaderHeight && e.Y > (this.HeaderHeight - this.TitleBarHeight)) {
					// make sure that our taskPane (if we have one) is not animating
					if (!this.Animating) {
						// collapse/expand the group
						this.Collapsed = !this.Collapsed;
					}

					if (this.CanCollapse) {
						this.Select();
					}
				}
			}

			this._dragStart = Point.Empty;
		}
	}


	/// <summary>
	/// Raises the MouseDown event
	/// </summary>
	/// <param name="e">A MouseEventArgs that contains the event data</param>
	protected override void OnMouseDown(MouseEventArgs e) {
		base.OnMouseDown(e);

		// we're not doing anything here yet...
		// but we might later :)

		if (e.Button == MouseButtons.Left) {
			if (this.TaskPane != null && this.TaskPane.AllowExpandoDragging && !this.Animating) {
				this._dragStart = this.PointToScreen(new Point(e.X, e.Y));
			}
		}
	}


	/// <summary>
	/// Raises the MouseMove event
	/// </summary>
	/// <param name="e">A MouseEventArgs that contains the event data</param>
	protected override void OnMouseMove(MouseEventArgs e) {
		base.OnMouseMove(e);

		if (e.Button == MouseButtons.Left && this._dragStart != Point.Empty) {
			Point p = this.PointToScreen(new Point(e.X, e.Y));

			if (!this._dragging) {
				if (Math.Abs(this._dragStart.X - p.X) > 8 || Math.Abs(this._dragStart.Y - p.Y) > 8) {
					this._dragging = true;

					this.FocusState = FocusStates.None;
				}
			}

			if (this._dragging) {
				if (this.TaskPane.ClientRectangle.Contains(this.TaskPane.PointToClient(p))) {
					this.Cursor = Cursors.Default;
				} else {
					this.Cursor = Cursors.No;
				}

				this.TaskPane.UpdateDropPoint(p);

				return;
			}
		}

		// check if the mouse is moving in the titlebar area
		if (e.Y < this.HeaderHeight && e.Y > (this.HeaderHeight - this.TitleBarHeight)) {
			// change the cursor to a hand and highlight the titlebar
			this.FocusState = FocusStates.Mouse;
		} else {
			// reset the titlebar highlight and cursor if they haven't already
			this.FocusState = FocusStates.None;
		}
	}


	/// <summary>
	/// Raises the MouseLeave event
	/// </summary>
	/// <param name="e">An EventArgs that contains the event data</param>
	protected override void OnMouseLeave(EventArgs e) {
		base.OnMouseLeave(e);

		// reset the titlebar highlight if it hasn't already
		this.FocusState = FocusStates.None;
	}

	#endregion

	#region Paint

	/// <summary>
	/// Raises the PaintBackground event
	/// </summary>
	/// <param name="e">A PaintEventArgs that contains the event data</param>
	protected override void OnPaintBackground(PaintEventArgs e) {
		// we may have a solid background color, but the titlebar back image
		// might have treansparent bits, so instead we draw our own 
		// transparent background (rather than getting windows to draw 
		// a solid background)
		this.PaintTransparentBackground(e.Graphics, e.ClipRectangle);

		// paint the titlebar background
		if (this.TitleBarRectangle.IntersectsWith(e.ClipRectangle)) {
			this.OnPaintTitleBarBackground(e.Graphics);
		}

		// only paint the border and "display rect" if we are not collapsed
		if (this.Height != this._headerHeight) {
			if (this.PseudoClientRect.IntersectsWith(e.ClipRectangle)) {
				this.OnPaintBorder(e.Graphics);

				this.OnPaintDisplayRect(e.Graphics);
			}
		}
	}


	/// <summary>
	/// Raises the Paint event
	/// </summary>
	/// <param name="e">A PaintEventArgs that contains the event data</param>
	protected override void OnPaint(PaintEventArgs e) {
		// paint the titlebar
		if (this.TitleBarRectangle.IntersectsWith(e.ClipRectangle)) {
			this.OnPaintTitleBar(e.Graphics);
		}
	}


	#region TitleBar

	/// <summary>
	/// Paints the title bar background
	/// </summary>
	/// <param name="g">The Graphics used to paint the titlebar</param>
	protected void OnPaintTitleBarBackground(Graphics g) {
		// fix: draw grayscale titlebar when disabled
		//      Brad Jones (brad@bradjones.com)
		//      20/08/2004
		//      v1.21

		int y = 0;

		// work out where the top of the titleBar actually is
		if (this.HeaderHeight > this.TitleBarHeight) {
			y = this.HeaderHeight - this.TitleBarHeight;
		}

		if (this.CustomHeaderSettings.TitleGradient && !this.AnyCustomTitleGradientsEmpty) {
			// gradient titlebar
			Color start = this.CustomHeaderSettings.NormalGradientStartColor;
			if (this.SpecialGroup) {
				start = this.CustomHeaderSettings.SpecialGradientStartColor;
			}

			Color end = this.CustomHeaderSettings.NormalGradientEndColor;
			if (this.SpecialGroup) {
				end = this.CustomHeaderSettings.SpecialGradientEndColor;
			}

			if (!this.Enabled) {
				// simulate saturation of 0

				start = Color.FromArgb((int)(start.GetBrightness() * 255),
					(int)(start.GetBrightness() * 255),
					(int)(start.GetBrightness() * 255));
				end = Color.FromArgb((int)(end.GetBrightness() * 255),
					(int)(end.GetBrightness() * 255),
					(int)(end.GetBrightness() * 255));
			}

			using (LinearGradientBrush brush = new LinearGradientBrush(this.TitleBarRectangle, start, end, LinearGradientMode.Horizontal)) {
				// work out where the gradient starts
				if (this.CustomHeaderSettings.GradientOffset > 0f && this.CustomHeaderSettings.GradientOffset < 1f) {
					ColorBlend colorBlend = new ColorBlend();
					colorBlend.Colors = new Color[] { brush.LinearColors[0], brush.LinearColors[0], brush.LinearColors[1] };
					colorBlend.Positions = new float[] { 0f, this.CustomHeaderSettings.GradientOffset, 1f };
					brush.InterpolationColors = colorBlend;
				}

				// check if we need round corners
				if (this.CustomHeaderSettings.TitleRadius > 0) {
					GraphicsPath path = new GraphicsPath();

					// top
					path.AddLine(this.TitleBarRectangle.Left + this.CustomHeaderSettings.TitleRadius,
						this.TitleBarRectangle.Top,
						this.TitleBarRectangle.Right - (this.CustomHeaderSettings.TitleRadius * 2) - 1,
						this.TitleBarRectangle.Top);

					// right corner
					path.AddArc(this.TitleBarRectangle.Right - (this.CustomHeaderSettings.TitleRadius * 2) - 1,
						this.TitleBarRectangle.Top,
						this.CustomHeaderSettings.TitleRadius * 2,
						this.CustomHeaderSettings.TitleRadius * 2,
						270,
						90);

					// right
					path.AddLine(this.TitleBarRectangle.Right,
						this.TitleBarRectangle.Top + this.CustomHeaderSettings.TitleRadius,
						this.TitleBarRectangle.Right,
						this.TitleBarRectangle.Bottom);

					// bottom
					path.AddLine(this.TitleBarRectangle.Right,
						this.TitleBarRectangle.Bottom,
						this.TitleBarRectangle.Left - 1,
						this.TitleBarRectangle.Bottom);

					// left corner
					path.AddArc(this.TitleBarRectangle.Left,
						this.TitleBarRectangle.Top,
						this.CustomHeaderSettings.TitleRadius * 2,
						this.CustomHeaderSettings.TitleRadius * 2,
						180,
						90);

					g.SmoothingMode = SmoothingMode.AntiAlias;

					g.FillPath(brush, path);

					g.SmoothingMode = SmoothingMode.Default;
				} else {
					g.FillRectangle(brush, this.TitleBarRectangle);
				}
			}
		} else if (this.TitleBackImage != null) {
			// check if the system header background images have different 
			// RightToLeft values compared to what we do.  if they are different, 
			// then we had better mirror them
			if ((this.RightToLeft == RightToLeft.Yes && !this.SystemSettings.Header.RightToLeft) ||
			    (this.RightToLeft == RightToLeft.No && this.SystemSettings.Header.RightToLeft)) {
				if (this.SystemSettings.Header.NormalBackImage != null) {
					this.SystemSettings.Header.NormalBackImage.RotateFlip(RotateFlipType.RotateNoneFlipX);
				}

				if (this.SystemSettings.Header.SpecialBackImage != null) {
					this.SystemSettings.Header.SpecialBackImage.RotateFlip(RotateFlipType.RotateNoneFlipX);
				}

				this.SystemSettings.Header.RightToLeft = (this.RightToLeft == RightToLeft.Yes);
			}

			if (this.Enabled) {
				//if (this.SystemSettings.OfficialTheme) {
				// left edge
				g.DrawImage(this.TitleBackImage,
					new Rectangle(0, y, 5, this.TitleBarHeight),
					new Rectangle(0, 0, 5, this.TitleBackImage.Height),
					GraphicsUnit.Pixel);

				// right edge
				g.DrawImage(this.TitleBackImage,
					new Rectangle(this.Width - 5, y, 5, this.TitleBarHeight),
					new Rectangle(this.TitleBackImage.Width - 5, 0, 5, this.TitleBackImage.Height),
					GraphicsUnit.Pixel);

				// middle
				g.DrawImage(this.TitleBackImage,
					new Rectangle(5, y, this.Width - 10, this.TitleBarHeight),
					new Rectangle(5, 0, this.TitleBackImage.Width - 10, this.TitleBackImage.Height),
					GraphicsUnit.Pixel);
				//	} else {
				//		g.DrawImage(this.TitleBackImage, 0, y, this.Width, this.TitleBarHeight);
				//	}
			} else {
				//	if (this.SystemSettings.OfficialTheme) {
				using (Image image = new Bitmap(this.Width, this.TitleBarHeight)) {
					using (Graphics g2 = Graphics.FromImage(image)) {
						// left edge
						g2.DrawImage(this.TitleBackImage,
							new Rectangle(0, y, 5, this.TitleBarHeight),
							new Rectangle(0, 0, 5, this.TitleBackImage.Height),
							GraphicsUnit.Pixel);


						// right edge
						g2.DrawImage(this.TitleBackImage,
							new Rectangle(this.Width - 5, y, 5, this.TitleBarHeight),
							new Rectangle(this.TitleBackImage.Width - 5, 0, 5, this.TitleBackImage.Height),
							GraphicsUnit.Pixel);

						// middle
						g2.DrawImage(this.TitleBackImage,
							new Rectangle(5, y, this.Width - 10, this.TitleBarHeight),
							new Rectangle(5, 0, this.TitleBackImage.Width - 10, this.TitleBackImage.Height),
							GraphicsUnit.Pixel);
					}

					ControlPaint.DrawImageDisabled(g, image, 0, y, this.TitleBackColor);
				}
				//} else {
				//    // first stretch the background image for ControlPaint.
				//    using (Image image = new Bitmap(this.TitleBackImage, this.Width, this.TitleBarHeight)) {
				//        ControlPaint.DrawImageDisabled(g, image, 0, y, this.TitleBackColor);
				//    }
				//}
			}
		} else {
			// single color titlebar
			using (SolidBrush brush = new SolidBrush(this.TitleBackColor)) {
				g.FillRectangle(brush, 0, y, this.Width, this.TitleBarHeight);
			}
		}
	}


	/// <summary>
	/// Paints the title bar
	/// </summary>
	/// <param name="g">The Graphics used to paint the titlebar</param>
	protected void OnPaintTitleBar(Graphics g) {
		int y = 0;

		// work out where the top of the titleBar actually is
		if (this.HeaderHeight > this.TitleBarHeight) {
			y = this.HeaderHeight - this.TitleBarHeight;
		}

		// draw the titlebar image if we have one
		if (this.TitleImage != null) {
			int x = 0;
			//int y = 0;

			if (this.RightToLeft == RightToLeft.Yes) {
				x = this.Width - this.TitleImage.Width;
			}

			if (this.Enabled) {
				g.DrawImage(this.TitleImage, x, 0);
			} else {
				ControlPaint.DrawImageDisabled(g, TitleImage, x, 0, this.TitleBackColor);
			}
		}

		// get which collapse/expand arrow we should draw
		Image arrowImage = this.ArrowImage;

		// get the titlebar's border and padding
		Border border = this.TitleBorder;
		PaddingEx padding = this.TitlePadding;

		// draw the arrow if we have one
		if (arrowImage != null) {
			// work out where to position the arrow
			int x = this.Width - arrowImage.Width - border.Right - padding.Right;
			y += border.Top + padding.Top;

			if (this.RightToLeft == RightToLeft.Yes) {
				x = border.Right + padding.Right;
			}

			// draw it...
			if (this.Enabled) {
				g.DrawImage(arrowImage, x, y);
			} else {
				ControlPaint.DrawImageDisabled(g, arrowImage, x, y, this.TitleBackColor);
			}
		}

		// check if we have any text to draw in the titlebar
		if (this.Text.Length > 0) {
			// a rectangle that will contain our text
			Rectangle rect = new Rectangle();

			// work out the x coordinate
			if (this.TitleImage == null) {
				rect.X = border.Left + padding.Left;
			} else {
				rect.X = this.TitleImage.Width + border.Left;
			}

			// work out the y coordinate
			ContentAlignment alignment = this.TitleAlignment;

			switch (alignment) {
				case ContentAlignment.MiddleLeft:
				case ContentAlignment.MiddleCenter:
				case ContentAlignment.MiddleRight:
					rect.Y = ((this.HeaderHeight - this.TitleFont.Height) / 2) + ((this.HeaderHeight - this.TitleBarHeight) / 2) + border.Top + padding.Top;
					break;

				case ContentAlignment.TopLeft:
				case ContentAlignment.TopCenter:
				case ContentAlignment.TopRight:
					rect.Y = (this.HeaderHeight - this.TitleBarHeight) + border.Top + padding.Top;
					break;

				case ContentAlignment.BottomLeft:
				case ContentAlignment.BottomCenter:
				case ContentAlignment.BottomRight:
					rect.Y = this.HeaderHeight - this.TitleFont.Height;
					break;
			}

			// the height of the rectangle
			rect.Height = this.TitleFont.Height;

			// make sure the text stays inside the header
			if (rect.Bottom > this.HeaderHeight) {
				rect.Y -= rect.Bottom - this.HeaderHeight;
			}

			// work out how wide the rectangle should be
			if (arrowImage != null) {
				rect.Width = this.Width - arrowImage.Width - border.Right - padding.Right - rect.X;
			} else {
				rect.Width = this.Width - border.Right - padding.Right - rect.X;
			}

			// don't wrap the string, and use an ellipsis if
			// the string is too big to fit the rectangle
			StringFormat sf = new StringFormat();
			sf.FormatFlags = StringFormatFlags.NoWrap;
			sf.Trimming = StringTrimming.EllipsisCharacter;

			// should the string be aligned to the left/center/right
			switch (alignment) {
				case ContentAlignment.MiddleLeft:
				case ContentAlignment.TopLeft:
				case ContentAlignment.BottomLeft:
					sf.Alignment = StringAlignment.Near;
					break;

				case ContentAlignment.MiddleCenter:
				case ContentAlignment.TopCenter:
				case ContentAlignment.BottomCenter:
					sf.Alignment = StringAlignment.Center;
					break;

				case ContentAlignment.MiddleRight:
				case ContentAlignment.TopRight:
				case ContentAlignment.BottomRight:
					sf.Alignment = StringAlignment.Far;
					break;
			}

			if (this.RightToLeft == RightToLeft.Yes) {
				sf.FormatFlags |= StringFormatFlags.DirectionRightToLeft;

				if (this.TitleImage == null) {
					rect.X = this.Width - rect.Width - border.Left - padding.Left;
				} else {
					rect.X = this.Width - rect.Width - this.TitleImage.Width - border.Left;
				}
			}

			// draw the text
			using (SolidBrush brush = new SolidBrush(this.TitleColor)) {
				//g.DrawString(this.Text, this.TitleFont, brush, rect, sf);
				if (this.Enabled) {
					g.DrawString(this.Text, this.TitleFont, brush, rect, sf);
				} else {
					ControlPaint.DrawStringDisabled(g, this.Text, this.TitleFont, SystemColors.ControlLightLight, rect, sf);
				}
			}
		}

		// check if windows will let us show a focus rectangle 
		// if we have focus
		if (this.Focused && base.ShowFocusCues) {
			if (this.ShowFocusCues) {
				// for some reason, if CanCollapse is false the focus rectangle 
				// will be drawn 2 pixels higher than it should be, so move it down
				if (!this.CanCollapse) {
					y += 2;
				}

				ControlPaint.DrawFocusRectangle(g, new Rectangle(2, y, this.Width - 4, this.TitleBarHeight - 3));
			}
		}
	}

	#endregion

	#region DisplayRect

	/// <summary>
	/// Paints the "Display Rectangle".  This is the dockable
	/// area of the control (ie non-titlebar/border area).  This is
	/// also the same as the PseudoClientRect.
	/// </summary>
	/// <param name="g">The Graphics used to paint the DisplayRectangle</param>
	protected void OnPaintDisplayRect(Graphics g) {
		// are we animating a fade
		if (this._animatingFade && this.AnimationImage != null) {
			// calculate the transparency value for the animation image
			float alpha = (((float)(this.Height - this.HeaderHeight)) / ((float)(this.ExpandedHeight - this.HeaderHeight)));

			float[][] ptsArray = {
				new float[] { 1, 0, 0, 0, 0 },
				new float[] { 0, 1, 0, 0, 0 },
				new float[] { 0, 0, 1, 0, 0 },
				new float[] { 0, 0, 0, alpha, 0 },
				new float[] { 0, 0, 0, 0, 1 }
			};

			ColorMatrix colorMatrix = new ColorMatrix(ptsArray);
			ImageAttributes imageAttributes = new ImageAttributes();
			imageAttributes.SetColorMatrix(colorMatrix, ColorMatrixFlag.Default, ColorAdjustType.Bitmap);

			// work out how far up the animation image we need to start
			int y = this.AnimationImage.Height - this.PseudoClientHeight - this.Border.Bottom;

			// draw the image
			g.DrawImage(this.AnimationImage,
				new Rectangle(0, this.HeaderHeight, this.Width, this.Height - this.HeaderHeight),
				0,
				y,
				this.AnimationImage.Width,
				this.AnimationImage.Height - y,
				GraphicsUnit.Pixel,
				imageAttributes);
		}
		// are we animating a slide
		else if (this._animatingSlide) {
			// check if we have a background image
			if (this.BackImage != null) {
				// tile the backImage
				using (TextureBrush brush = new TextureBrush(this.BackImage, WrapMode.Tile)) {
					g.FillRectangle(brush, this.DisplayRectangle);
				}
			} else {
				// just paint the area with a solid brush
				using (SolidBrush brush = new SolidBrush(this.BackColor)) {
					g.FillRectangle(brush,
						this.Border.Left,
						this.HeaderHeight + this.Border.Top,
						this.Width - this.Border.Left - this.Border.Right,
						this.Height - this.HeaderHeight - this.Border.Top - this.Border.Bottom);
				}
			}
		} else {
			// check if we have a background image
			if (this.BackImage != null) {
				// tile the backImage
				using (TextureBrush brush = new TextureBrush(this.BackImage, WrapMode.Tile)) {
					g.FillRectangle(brush, this.DisplayRectangle);
				}
			} else {
				// just paint the area with a solid brush
				using (SolidBrush brush = new SolidBrush(this.BackColor)) {
					g.FillRectangle(brush, this.DisplayRectangle);
				}
			}

			if (this.Watermark != null) {
				// work out a rough location of where the watermark should go
				Rectangle rect = new Rectangle(0, 0, this.Watermark.Width, this.Watermark.Height);
				rect.X = this.PseudoClientRect.Right - this.Watermark.Width;
				rect.Y = this.DisplayRectangle.Bottom - this.Watermark.Height;

				// shrink the destination rect if necesary so that we
				// can see all of the image

				if (rect.X < 0) {
					rect.X = 0;
				}

				if (rect.Width > this.ClientRectangle.Width) {
					rect.Width = this.ClientRectangle.Width;
				}

				if (rect.Y < this.DisplayRectangle.Top) {
					rect.Y = this.DisplayRectangle.Top;
				}

				if (rect.Height > this.DisplayRectangle.Height) {
					rect.Height = this.DisplayRectangle.Height;
				}

				// draw the watermark
				g.DrawImage(this.Watermark, rect);
			}
		}
	}

	#endregion

	#region Borders

	/// <summary>
	/// Paints the borders
	/// </summary>
	/// <param name="g">The Graphics used to paint the border</param>
	protected void OnPaintBorder(Graphics g) {
		// get the current border and border colors
		Border border = this.Border;
		Color c = this.BorderColor;

		// check if we are currently animating a fade
		if (this._animatingFade) {
			// calculate the alpha value for the color
			int alpha = (int)(255 * (((float)(this.Height - this.HeaderHeight)) / ((float)(this.ExpandedHeight - this.HeaderHeight))));

			// make sure it doesn't go past 0 or 255
			if (alpha < 0) {
				alpha = 0;
			} else if (alpha > 255) {
				alpha = 255;
			}

			// update the color with the alpha value
			c = Color.FromArgb(alpha, c.R, c.G, c.B);
		}

		// draw the borders
		using (SolidBrush brush = new SolidBrush(c)) {
			g.FillRectangle(brush, border.Left, this.HeaderHeight, this.Width - border.Left - border.Right, border.Top); // top border
			g.FillRectangle(brush, 0, this.HeaderHeight, border.Left, this.Height - this.HeaderHeight); // left border
			g.FillRectangle(brush, this.Width - border.Right, this.HeaderHeight, border.Right, this.Height - this.HeaderHeight); // right border
			g.FillRectangle(brush, border.Left, this.Height - border.Bottom, this.Width - border.Left - border.Right, border.Bottom); // bottom border
		}
	}

	#endregion

	#endregion

	#region Parent

	/// <summary>
	/// Raises the ParentChanged event
	/// </summary>
	/// <param name="e">An EventArgs that contains the event data</param>
	protected override void OnParentChanged(EventArgs e) {
		if (this.Parent == null) {
			this.TaskPane = null;
		} else if (this.Parent is TaskPane) {
			this.TaskPane = (TaskPane)this.Parent;

			this.Location = this.TaskPane.CalcExpandoLocation(this);
		}

		base.OnParentChanged(e);
	}

	#endregion

	#region Size

	/// <summary>
	/// Raises the SizeChanged event
	/// </summary>
	/// <param name="e">An EventArgs that contains the event data</param>
	protected override void OnSizeChanged(EventArgs e) {
		base.OnSizeChanged(e);

		// if we are currently animating and the width of the
		// group has changed (eg. due to a scrollbar on the 
		// TaskPane appearing/disappearing), get a new image 
		// to use for the animation. (if we were to continue to 
		// use the old image it would be shrunk or stretched making 
		// the animation look wierd)
		if (this.Animating && this.Width != this._oldWidth) {
			// if the width or height of the group is zero it probably 
			// means that our parent form has been minimized so we should 
			// immediately stop animating
			if (this.Width == 0) {
				this._animationHelper.StopAnimation();
			} else {
				this._oldWidth = this.Width;

				if (this.AnimationImage != null) {
					// get the new animationImage
					this._animationImage = this.GetFadeAnimationImage();
				}
			}
		}
		// check if the width has changed.  if it has re-layout
		// the group so that the TaskItems can resize themselves
		// if neccessary
		else if (this.Width != this._oldWidth) {
			this._oldWidth = this.Width;

			this.DoLayout();
		}
	}

	#endregion

	#endregion


	#region ItemCollection

	/// <summary>
	/// Represents a collection of Control objects
	/// </summary>
	public class ItemCollection : CollectionBase {

		#region Class Data

		/// <summary>
		/// The Expando that owns this ControlCollection
		/// </summary>
		private Expando owner;

		#endregion


		#region Constructor

		/// <summary>
		/// Initializes a new instance of the Expando.ItemCollection class
		/// </summary>
		/// <param name="owner">An Expando representing the expando that owns 
		/// the Control collection</param>
		public ItemCollection(Expando owner)
			: base() {
			if (owner == null) {
				throw new ArgumentNullException("owner");
			}

			this.owner = owner;
		}

		#endregion


		#region Methods

		/// <summary>
		/// Adds the specified control to the control collection
		/// </summary>
		/// <param name="value">The Control to add to the control collection</param>
		public void Add(Control value) {
			if (value == null) {
				throw new ArgumentNullException("value");
			}

			this.List.Add(value);
			this.owner.Controls.Add(value);

			this.owner.OnItemAdded(new ControlEventArgs(value));
		}


		/// <summary>
		/// Adds an array of control objects to the collection
		/// </summary>
		/// <param name="controls">An array of Control objects to add 
		/// to the collection</param>
		public void AddRange(Control[] controls) {
			if (controls == null) {
				throw new ArgumentNullException("controls");
			}

			for (int i = 0; i < controls.Length; i++) {
				this.Add(controls[i]);
			}
		}


		/// <summary>
		/// Removes all controls from the collection
		/// </summary>
		public new void Clear() {
			while (this.Count > 0) {
				this.RemoveAt(0);
			}
		}


		/// <summary>
		/// Determines whether the specified control is a member of the 
		/// collection
		/// </summary>
		/// <param name="control">The Control to locate in the collection</param>
		/// <returns>true if the Control is a member of the collection; 
		/// otherwise, false</returns>
		public bool Contains(Control control) {
			if (control == null) {
				throw new ArgumentNullException("control");
			}

			return (this.IndexOf(control) != -1);
		}


		/// <summary>
		/// Retrieves the index of the specified control in the control 
		/// collection
		/// </summary>
		/// <param name="control">The Control to locate in the collection</param>
		/// <returns>A zero-based index value that represents the position 
		/// of the specified Control in the Expando.ItemCollection</returns>
		public int IndexOf(Control control) {
			if (control == null) {
				throw new ArgumentNullException("control");
			}

			for (int i = 0; i < this.Count; i++) {
				if (this[i] == control) {
					return i;
				}
			}

			return -1;
		}


		/// <summary>
		/// Removes the specified control from the control collection
		/// </summary>
		/// <param name="value">The Control to remove from the 
		/// Expando.ItemCollection</param>
		public void Remove(Control value) {
			if (value == null) {
				throw new ArgumentNullException("value");
			}

			this.List.Remove(value);
			this.owner.Controls.Remove(value);

			this.owner.OnItemRemoved(new ControlEventArgs(value));
		}


		/// <summary>
		/// Removes a control from the control collection at the 
		/// specified indexed location
		/// </summary>
		/// <param name="index">The index value of the Control to 
		/// remove</param>
		public new void RemoveAt(int index) {
			this.Remove(this[index]);
		}


		/// <summary>
		/// Moves the specified control to the specified indexed location 
		/// in the control collection
		/// </summary>
		/// <param name="value">The control to be moved</param>
		/// <param name="index">The indexed location in the control collection 
		/// that the specified control will be moved to</param>
		public void Move(Control value, int index) {
			if (value == null) {
				throw new ArgumentNullException("value");
			}

			// make sure the index is within range
			if (index < 0) {
				index = 0;
			} else if (index > this.Count) {
				index = this.Count;
			}

			// don't go any further if the expando is already 
			// in the desired position or we don't contain it
			if (!this.Contains(value) || this.IndexOf(value) == index) {
				return;
			}

			this.List.Remove(value);

			// if the index we're supposed to move the expando to
			// is now greater to the number of expandos contained, 
			// add it to the end of the list, otherwise insert it at 
			// the specified index
			if (index > this.Count) {
				this.List.Add(value);
			} else {
				this.List.Insert(index, value);
			}

			// re-layout the controls
			this.owner.MatchControlCollToItemColl();
		}


		/// <summary>
		/// Moves the specified control to the top of the control collection
		/// </summary>
		/// <param name="value">The control to be moved</param>
		public void MoveToTop(Control value) {
			this.Move(value, 0);
		}


		/// <summary>
		/// Moves the specified control to the bottom of the control collection
		/// </summary>
		/// <param name="value">The control to be moved</param>
		public void MoveToBottom(Control value) {
			this.Move(value, this.Count);
		}

		#endregion


		#region Properties

		/// <summary>
		/// The Control located at the specified index location within 
		/// the control collection
		/// </summary>
		/// <param name="index">The index of the control to retrieve 
		/// from the control collection</param>
		public virtual Control this[int index] {
			get { return this.List[index] as Control; }
		}

		#endregion

	}

	#endregion


	#region ItemCollectionEditor

	/// <summary>
	/// A custom CollectionEditor for editing ItemCollections
	/// </summary>
	internal class ItemCollectionEditor : CollectionEditor {
		/// <summary>
		/// Initializes a new instance of the CollectionEditor class 
		/// using the specified collection type
		/// </summary>
		/// <param name="type"></param>
		public ItemCollectionEditor(Type type)
			: base(type) {

		}


		/// <summary>
		/// Edits the value of the specified object using the specified 
		/// service provider and context
		/// </summary>
		/// <param name="context">An ITypeDescriptorContext that can be 
		/// used to gain additional context information</param>
		/// <param name="isp">A service provider object through which 
		/// editing services can be obtained</param>
		/// <param name="value">The object to edit the value of</param>
		/// <returns>The new value of the object. If the value of the 
		/// object has not changed, this should return the same object 
		/// it was passed</returns>
		public override object EditValue(ITypeDescriptorContext context, IServiceProvider isp, object value) {
			Expando originalControl = (Expando)context.Instance;

			object returnObject = base.EditValue(context, isp, value);

			originalControl.UpdateItems();

			return returnObject;
		}


		/// <summary>
		/// Gets the data types that this collection editor can contain
		/// </summary>
		/// <returns>An array of data types that this collection can contain</returns>
		protected override Type[] CreateNewItemTypes() {
			return new Type[] {
				typeof(TaskItem),
				typeof(Button),
				typeof(CheckBox),
				typeof(CheckedListBox),
				typeof(ComboBox),
				typeof(DateTimePicker),
				typeof(Label),
				typeof(LinkLabel),
				typeof(ListBox),
				typeof(ListView),
				typeof(Panel),
				typeof(ProgressBar),
				typeof(RadioButton),
				typeof(TabControl),
				typeof(TextBox),
				typeof(TreeView)
			};
		}

	}

	#endregion


	#region AnimationPanel

	/// <summary>
	/// An extremely stripped down version of an Expando that an 
	/// Expando can use instead of itself to get an image of its 
	/// "client area" and child controls
	/// </summary>
	internal class AnimationPanel : Panel {

		#region Class Data

		/// <summary>
		/// The height of the header section 
		/// (includes titlebar and title image)
		/// </summary>
		protected int headerHeight;

		/// <summary>
		/// The border around the "client area"
		/// </summary>
		protected Border border;

		/// <summary>
		/// The background image displayed in the control
		/// </summary>
		protected Image backImage;

		#endregion


		#region Constructor

		/// <summary>
		/// Initializes a new instance of the AnimationPanel class with default settings
		/// </summary>
		public AnimationPanel()
			: base() {
			this.headerHeight = 0;
			this.border = new Border();
			this.backImage = null;
		}

		#endregion


		#region Properties

		/// <summary>
		/// Overrides AutoScroll to disable scrolling
		/// </summary>
		public new bool AutoScroll {
			get { return false; }

			set { }
		}


		/// <summary>
		/// Gets or sets the height of the header section of the Expando
		/// </summary>
		public int HeaderHeight {
			get { return this.headerHeight; }

			set { this.headerHeight = value; }
		}


		/// <summary>
		/// Gets or sets the border around the "client area"
		/// </summary>
		public Border Border {
			get { return this.border; }

			set { this.border = value; }
		}


		/// <summary>
		/// Gets or sets the background image displayed in the control
		/// </summary>
		public Image BackImage {
			get { return this.backImage; }

			set { this.backImage = value; }
		}


		/// <summary>
		/// Overrides DisplayRectangle so that docked controls
		/// don't cover the titlebar or borders
		/// </summary>
		public override Rectangle DisplayRectangle {
			get {
				return new Rectangle(this.Border.Left,
					this.HeaderHeight + this.Border.Top,
					this.Width - this.Border.Left - this.Border.Right,
					this.Height - this.HeaderHeight - this.Border.Top - this.Border.Bottom);
			}
		}

		#endregion


		#region Events

		/// <summary>
		/// Raises the Paint event
		/// </summary>
		/// <param name="e">A PaintEventArgs that contains the event data</param>
		protected override void OnPaint(PaintEventArgs e) {
			base.OnPaint(e);

			if (this.BackImage != null) {
				e.Graphics.DrawImageUnscaled(this.BackImage, 0, 0);
			}
		}

		#endregion

	}

	#endregion


	#region AnimationHelper

	/// <summary>
	/// A class that helps Expandos animate
	/// </summary>
	public class AnimationHelper : IDisposable {

		#region Class Data

		/// <summary>
		/// The number of frames in an animation
		/// </summary>
		public static readonly int NumAnimationFrames = 23;

		/// <summary>
		/// Specifes that a fade animation is to be performed
		/// </summary>
		public static int FadeAnimation = 1;

		/// <summary>
		/// Specifes that a slide animation is to be performed
		/// </summary>
		public static int SlideAnimation = 2;

		/// <summary>
		/// The type of animation to perform
		/// </summary>
		private int animationType;

		/// <summary>
		/// The Expando to animate
		/// </summary>
		private Expando expando;

		/// <summary>
		/// The current frame in animation
		/// </summary>
		private int animationStepNum;

		/// <summary>
		/// The number of frames in the animation
		/// </summary>
		private int numAnimationSteps;

		/// <summary>
		/// The amount of time each frame is shown (in milliseconds)
		/// </summary>
		private int animationFrameInterval;

		/// <summary>
		/// Specifies whether an animation is being performed
		/// </summary>
		private bool animating;

		/// <summary>
		/// A timer that notifies the helper when the next frame is due
		/// </summary>
		private System.Windows.Forms.Timer animationTimer;

		#endregion


		#region Constructor

		/// <summary>
		/// Initializes a new instance of the AnimationHelper class with the specified settings
		/// </summary>
		/// <param name="expando">The Expando to be animated</param>
		/// <param name="animationType">The type of animation to perform</param>
		public AnimationHelper(Expando expando, int animationType) {
			this.expando = expando;
			this.animationType = animationType;

			this.animating = false;

			this.numAnimationSteps = NumAnimationFrames;
			this.animationFrameInterval = 10;

			// I know that this isn't the best way to do this, but I 
			// haven't quite worked out how to do it with threads so 
			// this will have to do for the moment
			this.animationTimer = new System.Windows.Forms.Timer();
			this.animationTimer.Tick += new EventHandler(this.animationTimer_Tick);
			this.animationTimer.Interval = this.animationFrameInterval;
		}

		#endregion


		#region Methods

		/// <summary>
		/// Releases all resources used by the AnimationHelper
		/// </summary>
		public void Dispose() {
			if (this.animationTimer != null) {
				this.animationTimer.Stop();
				this.animationTimer.Dispose();
				this.animationTimer = null;
			}

			this.expando = null;
		}


		/// <summary>
		/// Starts the Expando collapse/expand animation
		/// </summary>
		public void StartAnimation() {
			// don't bother going any further if we are already animating
			if (this.Animating) {
				return;
			}

			this.animationStepNum = 0;

			// tell the expando to get ready to animate
			if (this.AnimationType == FadeAnimation) {
				this.expando.StartFadeAnimation();
			} else {
				this.expando.StartSlideAnimation();
			}

			// start the animation timer
			this.animationTimer.Start();
		}


		/// <summary>
		/// Updates the animation for the Expando
		/// </summary>
		protected void PerformAnimation() {
			// if we have more animation steps to perform
			if (this.animationStepNum < this.numAnimationSteps) {
				// update the animation step number
				this.animationStepNum++;

				// tell the animating expando to update the animation
				if (this.AnimationType == FadeAnimation) {
					this.expando.UpdateFadeAnimation(this.animationStepNum, this.numAnimationSteps);
				} else {
					this.expando.UpdateSlideAnimation(this.animationStepNum, this.numAnimationSteps);
				}
			} else {
				this.StopAnimation();
			}
		}


		/// <summary>
		/// Stops the Expando collapse/expand animation
		/// </summary>
		public void StopAnimation() {
			// stop the animation
			this.animationTimer.Stop();
			this.animationTimer.Dispose();

			if (this.AnimationType == FadeAnimation) {
				this.expando.StopFadeAnimation();
			} else {
				this.expando.StopSlideAnimation();
			}
		}

		#endregion


		#region Properties

		/// <summary>
		/// Gets the Expando that is te be animated
		/// </summary>
		public Expando Expando {
			get { return this.expando; }
		}


		/// <summary>
		/// Gets or sets the number of steps that are needed for the Expando 
		/// to get from fully expanded to fully collapsed, or visa versa
		/// </summary>
		public int NumAnimationSteps {
			get { return this.numAnimationSteps; }

			set {
				if (value < 0) {
					throw new ArgumentOutOfRangeException("value", "NumAnimationSteps must be at least 0");
				}

				// only change this if we are not currently animating
				// (if we are animating, changing this could cause things
				// to screw up big time)
				if (!this.animating) {
					this.numAnimationSteps = value;
				}
			}
		}


		/// <summary>
		/// Gets or sets the number of milliseconds that each "frame" 
		/// of the animation stays on the screen
		/// </summary>
		public int AnimationFrameInterval {
			get { return this.animationFrameInterval; }

			set { this.animationFrameInterval = value; }
		}


		/// <summary>
		/// Gets whether the Expando is currently animating
		/// </summary>
		public bool Animating {
			get { return this.animating; }
		}


		/// <summary>
		/// Gets the type of animation to perform
		/// </summary>
		public int AnimationType {
			get { return this.animationType; }
		}

		#endregion


		#region Events

		/// <summary>
		/// Event handler for the animation timer tick event
		/// </summary>
		/// <param name="sender">The object that fired the event</param>
		/// <param name="e">An EventArgs that contains the event data</param>
		private void animationTimer_Tick(object sender, EventArgs e) {
			// do the next bit of the aniation
			this.PerformAnimation();
		}

		#endregion

	}

	#endregion

}
