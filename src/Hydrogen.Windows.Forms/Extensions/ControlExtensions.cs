//-----------------------------------------------------------------------
// <copyright file="ControlExtensions.cs" company="Sphere 10 Software">
//
// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// <author>Herman Schoenfeld</author>
// <date>2018</date>
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using System.Drawing.Drawing2D;
using System.Drawing;
using Hydrogen.Windows.Forms;

namespace Hydrogen;

public static class ControlExtensions {
	// TKey is control to drag, TValue is a flag used while dragging
	private static Dictionary<Control, bool> draggables = new Dictionary<Control, bool>();
	private static System.Drawing.Size mouseOffset;
	private static readonly Type[] NeverDisableTypes = new[] { typeof(GroupBox), typeof(Form), typeof(ValidationIndicator) };

	public static bool TryGetParentForm(this Control control, out Form parent) {
		while (control != null) {
			if (control is Form form) {
				parent = form;
				return true;
			}
			control = control.Parent;
		}
		parent = null;
		return false;
	}

	public static IEnumerable<string> GetControlPath(this Control c) {
		yield return c.Name;

		if (c.Parent != null) {
			Control parent = c.Parent;

			while (parent != null) {
				yield return parent.Name;
				parent = parent.Parent;
			}
		}
	}

	/// <summary>
	/// Searches this control to find a control with the name specified.
	/// </summary>
	/// <param name="control">The Control in which to begin searching.</param>
	/// <param name="id">The ID of the control to be found.</param>
	/// <param name="recursive">Whether to search child control's recursively.</param>
	/// <returns>The control if it is found or null if it is not.</returns>
	public static Control FindControl(this Control control, string id, bool recursive) {
		Control controlFound;
		if (control != null) {
			if (recursive) {
				controlFound = control.FindControl(id, recursive);
				if (controlFound != null) {
					return controlFound;
				}
			}

			foreach (Control c in control.Controls) {
				controlFound = c.FindControl(id, recursive);

				if (controlFound != null) {
					return controlFound;
				}
			}
		}

		return null;
	}

	public static void RemoveAllControls(this Control control) {
		try {
			control.SuspendLayout();
			while (control.Controls.Count > 0) {
				control.Controls.Remove(control.Controls[0]);
			}
		} finally {
			control.ResumeLayout();
		}
	}

	/// <summary>
	/// Simulates a transparent background by getting the parent control
	/// to paint its background and foreground into the specified Graphics 
	/// at the specified location
	/// </summary>
	/// <param name="g">The Graphics used to paint the background</param>
	/// <param name="clipRect">The Rectangle that represents the rectangle 
	/// in which to paint</param>
	public static void PaintTransparentBackground(this Control control, Graphics g, Rectangle clipRect) {
		// check if we have a parent
		if (control.Parent != null) {
			// convert the clipRects coordinates from ours to our parents
			clipRect.Offset(control.Location);

			PaintEventArgs e = new PaintEventArgs(g, clipRect);

			// save the graphics state so that if anything goes wrong 
			// we're not fubar
			GraphicsState state = g.Save();

			try {
				// move the graphics object so that we are drawing in 
				// the correct place
				g.TranslateTransform((float)-control.Location.X, (float)-control.Location.Y);

				// draw the parents background and foreground

				Tools.Reflection.InvokeMethod(control, "InvokePaintBackground", control.Parent, e);
				Tools.Reflection.InvokeMethod(control, "InvokePaint", control.Parent, e);

				return;
			} finally {
				// reset everything back to where they were before
				g.Restore(state);
				clipRect.Offset(-control.Location.X, -control.Location.Y);
			}
		}

		// we don't have a parent, so fill the rect with
		// the default control color
		g.FillRectangle(SystemBrushes.Control, clipRect);
	}

	public static PointF GetCenterPoint(this Control control) {
		return new PointF(control.Width / 2, control.Height / 2 - 1);
	}

	public static void InvokeEx(this Control control, Action action) {
		if (!control.IsHandleCreated) {
			System.Windows.Forms.Application.OpenForms.Cast<Form>().Reverse().First().InvokeEx(action);
		} else {
			if (control.InvokeRequired)
				control.Invoke(action);
			else
				action();
		}
	}

	public static void BeginInvokeEx(this Control control, Action action) {
		if (control.IsHandleCreated)
			control.BeginInvoke(action);
		else {
			System.Windows.Forms.Application.OpenForms.Cast<Form>().Reverse().First().BeginInvoke(action);
		}
	}

	public static void EnableChildren(this Control control, bool enabled, params Control[] preserveControls) {
		EnableChildrenInternal(control, 0, enabled, new HashSet<Control>(preserveControls));
	}

	private static void EnableChildrenInternal(this Control control, int nestingLevel, bool enabled, HashSet<Control> preserveControls) {
		IDictionary<Control, bool> controlStates = null;
		if (nestingLevel == 0) {
			controlStates = preserveControls.ToDictionary(c => c, c => c.Enabled);
		}

		foreach (Control child in control.Controls) {
			if (!preserveControls.Contains(child)) {
				if (!child.GetType().IsIn(NeverDisableTypes)) {
					child.Enabled = enabled;
				}
				child.EnableChildrenInternal(nestingLevel + 1, enabled, preserveControls);
			}
		}
		if (nestingLevel == 0) {
			preserveControls.Update(c => c.Enabled = controlStates[c]);
		}
	}

	/// <summary>
	/// Enabling/disabling dragging for control
	/// </summary>
	public static void Draggable(this Control control, bool Enable) {
		if (Enable) {
			// enable drag feature
			if (draggables.ContainsKey(control)) {
				// return if control is already draggable
				return;
			}
			// 'false' - initial state is 'not dragging'
			draggables.Add(control, false);

			// assign required event handlersnnn
			control.MouseDown += new MouseEventHandler(control_MouseDown);
			control.MouseUp += new MouseEventHandler(control_MouseUp);
			control.MouseMove += new MouseEventHandler(control_MouseMove);
		} else {
			// disable drag feature
			if (!draggables.ContainsKey(control)) {
				// return if control is not draggable
				return;
			}
			// remove event handlers
			control.MouseDown -= control_MouseDown;
			control.MouseUp -= control_MouseUp;
			control.MouseMove -= control_MouseMove;
			draggables.Remove(control);
		}
	}

	static void control_MouseDown(object sender, MouseEventArgs e) {
		mouseOffset = new System.Drawing.Size(e.Location);
		// turning on dragging
		draggables[(Control)sender] = true;
	}
	static void control_MouseUp(object sender, MouseEventArgs e) {
		// turning off dragging
		draggables[(Control)sender] = false;
	}
	static void control_MouseMove(object sender, MouseEventArgs e) {
		// only if dragging is turned on
		if (draggables[(Control)sender] == true) {
			// calculations of control's new position
			System.Drawing.Point newLocationOffset = e.Location - mouseOffset;
			((Control)sender).Left += newLocationOffset.X;
			((Control)sender).Top += newLocationOffset.Y;
		}
	}
}
