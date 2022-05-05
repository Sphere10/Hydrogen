//-----------------------------------------------------------------------
// <copyright file="UIComboBox.cs" company="Sphere 10 Software">
//
// Copyright (c) Sphere 10 Software. All rights reserved. (http://www.sphere10.com)
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// <author>Herman Schoenfeld</author>
// <date>2018</date>
// </copyright>
//-----------------------------------------------------------------------

using System;
using UIKit;
using CoreGraphics;
using ObjCRuntime;
using Foundation;
using Hydrogen;

namespace Hydrogen.iOS
{


	public class UIComboBox : UITextField
	{
		PickerView pickerView;
		TapableView closeView;
		UIButton closeBtn;
		public UIView ViewForPicker; 
		public event EventHandler ValueChanged;
		public event EventHandler PickerClosed;
		public event EventHandler PickerShown;
		public event EventHandler PickerFadeInDidFinish;
		public UIComboBox(CGRect rect) : base (rect)
		{
			this.BorderStyle = UITextBorderStyle.RoundedRect;
			pickerView = new PickerView();
			this.TouchDown += delegate {	
				ShowPicker();
			};
			this.ShouldChangeCharacters += delegate {
				return false;	
			};
			pickerView.IndexChanged += delegate {
				var oldValue = this.Text;
				this.Text = pickerView.StringValue;	
				if(ValueChanged!= null && oldValue != Text)
					ValueChanged(this,null);
				
			};
			closeBtn = new UIButton(new CGRect(0,0,31,32));
            closeBtn.SetImage(Tools.iOSTool.EmbeddedImage("closebox.png"), UIControlState.Normal);
			closeBtn.TouchDown += delegate {
				HidePicker();
			};
			pickerView.AddSubview(closeBtn);
		}
		public override bool CanBecomeFirstResponder {
			get {
				return false;
			}
		}
		public override void LayoutSubviews ()
		{
			base.LayoutSubviews ();
			var parentView = ViewForPicker?? this.Superview;
			var parentH = parentView.Frame.Height;
			pickerView.Frame = new CGRect(0,parentH - pickerView.Frame.Height, parentView.Frame.Size.Width,pickerView.Frame.Height);
			closeBtn.Frame = closeBtn.Frame.SetLocation(new CGPoint(pickerView.Bounds.Width - 32,pickerView.Bounds.Y));
			pickerView.BringSubviewToFront(closeBtn);
		}
		
		/// <summary>
		/// Can be a collection of anyting. If you don't set the ValueMember or DisplayMember, it will use ToString() for the value and Title.
		/// </summary>
		public object [] Items {
			get{return pickerView.Items;}
			set{pickerView.Items = value;}
		}
		
		public string DisplayMember {
			get{return pickerView.DisplayMember;}
			set {pickerView.DisplayMember = value;}
		}
		public string ValueMember {
			get{return pickerView.ValueMember;}
			set {pickerView.ValueMember = value;}
		}
		public float Width {
			get{return pickerView.Width;}
			set {pickerView.Width = value;}
		}
		
		bool pickerVisible;
		
		public void ShowPicker()
		{
			if(PickerShown != null)
				PickerShown(this,null);
			LayoutSubviews ();
			pickerView.BringSubviewToFront(closeBtn);
			pickerVisible = true;
			var parentView = ViewForPicker ?? this.Superview;
			var parentFrame = (CGRect)parentView.Frame;
			//closeView = new TapableView(parentView.Bounds);
			//closeView.Tapped += delegate{
			//	HidePicker();	
			//};
			
			pickerView.Frame = (CGRect)pickerView.Frame.SetLocation(new CGPoint(0,parentFrame.Height));
			
			UIView.BeginAnimations("slidePickerIn");			
			UIView.SetAnimationDuration(0.3);
			UIView.SetAnimationDelegate(this);
			UIView.SetAnimationDidStopSelector (new Selector ("fadeInDidFinish"));
			//parentView.AddSubview(closeView);			
			parentView.AddSubview(pickerView);
			var tb = new UITextField(new CGRect(0,-100,15,25));
			//closeView.AddSubview(tb);
			tb.BecomeFirstResponder();
			tb.ResignFirstResponder();
			tb.RemoveFromSuperview();
			
			
			pickerView.Frame = (CGRect)pickerView.Frame.SetLocation(new CGPoint(0,parentFrame.Height - pickerView.Frame.Height));
			UIView.CommitAnimations();				
		}
		public bool IsHiding;
		public void HidePicker(bool Animated = true)
		{
			var parentView = ViewForPicker ?? Superview;
			if(PickerClosed!=null)
				PickerClosed(this,null);
			if(IsHiding || parentView == null)
				return;
			IsHiding = true;
			
			var parentH = parentView.Frame.Height;
			
			if (Animated) {
				UIView.BeginAnimations("slidePickerOut");			
				UIView.SetAnimationDuration(0.3);
				UIView.SetAnimationDelegate(this);
				UIView.SetAnimationDidStopSelector (new Selector ("fadeOutDidFinish"));
				pickerView.Frame = pickerView.Frame.SetLocation(new CGPoint(0,parentH));
				UIView.CommitAnimations();
			}
			else {
				parentView.SendSubviewToBack(pickerView);
				IsHiding = false;
			}
		}
		
		public void SetSelectedIndex(int index) {
			pickerView.Select(index, 0, true);
			pickerView.SelectedIndex = index;
		}
		
		public void SetSelectedValue(string Value) {
			var xx = 0;
			foreach(var item in pickerView.Items) {
				if (item.ToString() == Value) {
					pickerView.Select(xx, 0, true);
					pickerView.SelectedIndex = xx;
				}
				xx += 1;
			}
		}
		
		public object SelectedItem {
			get {return pickerView.SelectedItem;}
		}
		
		[Foundation.Export("fadeOutDidFinish")]
		public void FadeOutDidFinish ()
		{
			pickerView.RemoveFromSuperview();
			//closeView.RemoveFromSuperview();
			pickerVisible = false;
			IsHiding = false;
		}
		[Foundation.Export("fadeInDidFinish")]
		public void FadeInDidFinish ()
		{
			pickerView.BecomeFirstResponder();
			pickerView.BringSubviewToFront(closeBtn);
			
			if (PickerFadeInDidFinish != null) {
				PickerFadeInDidFinish(this, null);
			}
		}
		
	}
	
	public class PickerView : UIPickerView
	{
		public PickerView () : base ()
		{
			this.ShowSelectionIndicator = true;
			this.Width = 300f;
		}	
		
		object[] items;
		public object[] Items
		{
			get{return items;}
			set{
				items = value;
				this.Model = new PickerData(this);
				if(IndexChanged != null)
					IndexChanged(this,null);
			}
		}		
		
		public string DisplayMember {get;set;}
		public string ValueMember {get;set;}
		public float Width {get;set;}
		
		nint selectedIndex;
		public nint SelectedIndex{
			get {return selectedIndex;}
			set{
				if(selectedIndex == value)
					return;
				selectedIndex = value;
				if(IndexChanged != null)
					IndexChanged(this,null);
			}
		}
		
		public object SelectedItem {get{return items[SelectedIndex];}}
		
		public string StringValue {
			get{
				if(string.IsNullOrEmpty(ValueMember))
					return SelectedItem.ToString();
                return Tools.iOSTool.GetPropertyValue(SelectedItem, ValueMember);
			}
		}
		
		public event EventHandler IndexChanged;
	}
	
	public class PickerData : UIPickerViewModel
	{	
		PickerView Picker;
		public PickerData (PickerView picker)
		{		
			Picker = picker;			
		}
		
		public override nint GetComponentCount (UIPickerView uipv)
		{	
			return (1);	
		}
		
		public override nint GetRowsInComponent (UIPickerView uipv, nint comp)
		{
			//each component has its own count.
			int rows = Picker.Items.Length;
			return (rows);
		}
		
		public override string GetTitle (UIPickerView uipv, nint row, nint comp)
		{
			//each component would get its own title.
			var theObject = Picker.Items[row];
			if(string.IsNullOrEmpty(Picker.DisplayMember))
				return theObject.ToString();
            return Tools.iOSTool.GetPropertyValue(theObject, Picker.DisplayMember);
		}
		
		
		
		public override void Selected (UIPickerView uipv, nint row, nint comp)
		{
			Picker.SelectedIndex = row;
			//Picker.Select(row,comp,false);
		}
		
		public override nfloat GetComponentWidth (UIPickerView uipv, nint comp)
		{
			return Picker.Width;
		}
		
		public override nfloat GetRowHeight (UIPickerView uipv, nint comp)
		{
			return (40f);	
		}
		
		//CDEUTSCH: there are issues with this losing the contents of rows when there are multiple pickers on a page.
		//public override UIView GetView (UIPickerView pickerView, int row, int component, UIView view) {
		//	((UIView)Picker.Items[row]).ReloadInputViews();
		//	return (UIView)Picker.Items[row];
		//}
		
	}

	/*public class UIComboBox : UITextField
	{
		PickerView pickerView;
		TapableView closeView;
		UIButton closeBtn;
		public UIView ViewForPicker; 
		public event EventHandler ValueChanged;
		public event EventHandler PickerClosed;
		public event EventHandler PickerShown;
		public UIComboBox(CGRect rect) : base (rect)
		{
			this.BorderStyle = UITextBorderStyle.RoundedRect;
			pickerView = new PickerView();
			this.TouchDown += delegate {	
				ShowPicker();
			};
			this.ShouldChangeCharacters += delegate {
				return false;	
			};
			pickerView.IndexChanged += delegate {
				var oldValue = this.Text;
				this.Text = pickerView.StringValue;	
				if(ValueChanged!= null && oldValue != Text)
					ValueChanged(this,null);
				
			};
			closeBtn = new UIButton(new CGRect(0,0,31,32));
			closeBtn.SetImage(UIImage.FromFile("Images/closebox.png"),UIControlState.Normal);
			closeBtn.TouchDown += delegate {
				HidePicker();
			};
			pickerView.AddSubview(closeBtn);
		}
		public override bool CanBecomeFirstResponder {
			get {
				return false;
			}
		}
		
		public override void LayoutSubviews ()
		{
			base.LayoutSubviews ();
			var parentView = ViewForPicker?? this.Superview;
			var parentH = parentView.Frame.Height;
			pickerView.Frame = new CGRect(0,parentH - pickerView.Frame.Height,parentView.Frame.Size.Width,pickerView.Frame.Height);
			closeBtn.Frame = closeBtn.Frame.SetLocation(new CGPoint(pickerView.Bounds.Width - 32,pickerView.Bounds.Y));
			pickerView.BringSubviewToFront(closeBtn);
		}
		
		public object [] Items {
			get{return pickerView.Items;}
			set{pickerView.Items = value;}
		}
		
		public string DisplayMember {
			get{return pickerView.DisplayMember;}
			set {pickerView.DisplayMember = value;}
		}
		bool pickerVisible;
		
		public void ShowPicker()
		{
			if(PickerShown != null)
				PickerShown(this,null);
			LayoutSubviews ();
			pickerView.BringSubviewToFront(closeBtn);
			pickerVisible = true;
			var parentView = ViewForPicker ?? this.Superview;
			var parentFrame = parentView.Frame;
			//closeView = new TapableView(parentView.Bounds);
			//closeView.Tapped += delegate{
			//	HidePicker();	
			//};
			
			pickerView.Frame = pickerView.Frame.SetLocation(new CGPoint(0,parentFrame.Height));
			
			UIView.BeginAnimations("slidePickerIn");			
			UIView.SetAnimationDuration(0.3);
			UIView.SetAnimationDelegate(this);
			UIView.SetAnimationDidStopSelector (new Selector ("fadeInDidFinish"));
			//parentView.AddSubview(closeView);			
			parentView.AddSubview(pickerView);
			var tb = new UITextField(new CGRect(0,-100,15,25));
			pickerView.AddSubview(tb);
			tb.BecomeFirstResponder();
			tb.ResignFirstResponder();
			tb.RemoveFromSuperview();
			
			
			pickerView.Frame = pickerView.Frame.SetLocation(new CGPoint(0,parentFrame.Height - pickerView.Frame.Height));
			UIView.CommitAnimations();	
			
		}
		bool isHiding;
		public void HidePicker()
		{
			if(PickerClosed!=null)
				PickerClosed(this,null);
			if(isHiding)
				return;
			isHiding = true;
			var parentView = ViewForPicker ?? Superview;
			var parentH = parentView.Frame.Height;
			
			UIView.BeginAnimations("slidePickerOut");			
			UIView.SetAnimationDuration(0.3);
			UIView.SetAnimationDelegate(this);
			UIView.SetAnimationDidStopSelector (new Selector ("fadeOutDidFinish"));
			pickerView.Frame = pickerView.Frame.SetLocation(new CGPoint(0,parentH));
			UIView.CommitAnimations();
		}
		
		public object SelectedItem {
			get {return pickerView.SelectedItem;}
		}
		
		[Export("fadeOutDidFinish")]
		public void FadeOutDidFinish ()
		{
			pickerView.RemoveFromSuperview();
			//closeView.RemoveFromSuperview();
			pickerVisible = false;
			isHiding = false;
		}
		[Export("fadeInDidFinish")]
		public void FadeInDidFinish ()
		{
			pickerView.BecomeFirstResponder();
			pickerView.BringSubviewToFront(closeBtn);
		}
		
	}
	
	public class PickerView : UIPickerView
	{
		public PickerView () : base ()
		{
			this.ShowSelectionIndicator = true;
		}	
		
		object[] items;
		public object[] Items
		{
			get{return items;}
			set{
				items = value;
				this.Model = new PickerData(this);
				if(IndexChanged != null)
					IndexChanged(this,null);
			}
		}		
		public string DisplayMember{get;set;}
		
		int selectedIndex;
		public int SelectedIndex{
			get {return selectedIndex;}
			set{
				if(selectedIndex == value)
					return;
				selectedIndex = value;
				if(IndexChanged != null)
					IndexChanged(this,null);
			}
		}
		
		public object SelectedItem {get{return items[SelectedIndex];}}
		
		public string StringValue {
			get{
				if(string.IsNullOrEmpty(DisplayMember))
					return SelectedItem.ToString();
				return GraphicsTool.GetPropertyValue(SelectedItem,DisplayMember);
			}
		}
		
		public event EventHandler IndexChanged;
	}
	
	public class PickerData : UIPickerViewModel
	{	
		PickerView Picker;
		public PickerData (PickerView picker)
		{		
			Picker = picker;			
		}
		
		public override int GetComponentCount (UIPickerView uipv)
		{	
			return (1);	
		}
		
		public override int GetRowsInComponent (UIPickerView uipv, int comp)
		{
			//each component has its own count.
			int rows = Picker.Items.Length;
			return (rows);
		}
		
		public override string GetTitle (UIPickerView uipv, int row, int comp)
		{
			
			//each component would get its own title.
			
			var theObject = Picker.Items[row];
			if(string.IsNullOrEmpty(Picker.DisplayMember))
				return theObject.ToString();
			return GraphicsTool.GetPropertyValue(theObject,Picker.DisplayMember);
		}
		
		
		public override void Selected (UIPickerView uipv, int row, int comp)
		{
			Picker.SelectedIndex = row;
			//Picker.Select(row,comp,false);
		}
		
		public override float GetComponentWidth (UIPickerView uipv, int comp)
		{
			return (300f);	
		}
		
		public override float GetRowHeight (UIPickerView uipv, int comp)
		{
			return (40f);	
		}
	}*/
}

