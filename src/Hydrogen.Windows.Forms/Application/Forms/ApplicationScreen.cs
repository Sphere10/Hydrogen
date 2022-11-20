//-----------------------------------------------------------------------
// <copyright file="ApplicationScreen.cs" company="Sphere 10 Software">
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
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Diagnostics;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using Hydrogen.Application;
using Hydrogen.Windows.Forms;
using Hydrogen.Windows.Forms.Components.BlockFramework;


namespace Hydrogen.Windows.Forms {

    /// <summary>
    /// Application UIs can be presented as a screen. It is a proxy ApplicationServiceProvider
    /// routing to parent provider. 
    /// 
    /// NOTE: The ApplicationServiceProvider property, which defines the underlying provider all
    /// such  calls are routed to, is guaranteed to be set post-construction.
    /// </summary>
    public class ApplicationScreen : ApplicationControl, IHelpableObject {
		private int _showCount;
		private readonly List<ToolStripItem> _menuStripItems;

	    public event EventHandler ScreenLoaded;
		public event EventHandler ScreenDisplayed;
		public event EventHandler ScreenDisplayedFirstTime;
		public event EventHandler<HideScreenEventArgs> ScreenHidden;
	    public event EventHandler ScreenDestroyed;

		public ApplicationScreen()
            : this(null) {
		}

        public ApplicationScreen(IApplicationBlock applicationBlock) {
            ApplicationBlock = applicationBlock;
			Url = FileName = null;
			Type = HelpType.None;
            _menuStripItems = new List<ToolStripItem>();
	        _showCount = 0;
		}

		[Browsable(true), Category("Appearance")]
		public string ApplicationMenuStripText { get; set; }

		[Browsable(true), Category("Appearance")]
		public bool ShowInApplicationMenuStrip { get; set; }

        [Browsable(true), Category("Layout"), Description("How this screen will be displayed to the user")]
        public ScreenDisplayMode DisplayMode { get; set; }

        [Browsable(true), Category("Behavior"), Description("How this screen will be displayed to the user")]
        public ScreenActivationMode ActivationMode { get; set; }

		[Browsable(false)]
        public IApplicationBlock ApplicationBlock { get; set; }

	    //[Category("Behavior")]
	    //[DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
	    //[DefaultValue(true)]
	    //public bool AutoSave { get; set; }
        
        /// <summary>
        /// The menu items associated with this screen.
        /// </summary>
        [Browsable(false)]
        public ToolStripItem[] MenuItems {
            get {
                return _menuStripItems.ToArray();
            }
        }

        /// <summary>
        /// The toolbar associated with this screen.
        /// </summary>
        [Browsable(true), Category("Behavior"), Description("The toolbar associated with this screen.")]
        public ToolStrip ToolBar { get; set; }

	    public HelpType Type {
		    get;
		    private set;
	    }

	    public string FileName {
		    get;
		    private set;
	    }

	    public string Url {
		    get;
		    private set;
	    }

	    public int? PageNumber {
		    get;
		    private set;
	    }

	    public int? HelpTopicID {
		    get;
		    private set;
	    }

	    public int? HelpTopicAlias {
		    get;
		    private set;
	    }

		public override void SetLocalizedText(CultureInfo culture = null) {
			base.SetLocalizedText(culture);
            SetLocalizedTextInApplicationControls(this.Controls);
        }

		protected virtual void OnShowFirstTime() {
		}

		protected virtual void OnShow() {
			if (_showCount++ == 0)
				NotifyShowScreenFirstTime();
		}

	    protected virtual void OnHide(ref bool cancelHide) {
        }

	    protected virtual void OnDestroyScreen() {
	    }

        protected void RegisterMenuItem(ToolStripItem item) {
            _menuStripItems.Add(item);
        }

        private void SetLocalizedTextInApplicationControls(ControlCollection controls) {
            if (controls != null) {
                foreach (Control control in controls) {
                    if (control is ApplicationControl) {
                        ((ApplicationControl)control).SetLocalizedText();
                    }
                    SetLocalizedTextInApplicationControls(
                        control.Controls
                    );
                }
            }
        }

	    internal void NotifyShow() {
			OnShow();
			ScreenDisplayed?.Invoke(this, EventArgs.Empty);
		}

		internal void NotifyShowScreenFirstTime() {
			OnShowFirstTime();
			ScreenDisplayedFirstTime?.Invoke(this, EventArgs.Empty);
		}

		internal void NotifyHideScreen(ref bool cancel) {
			OnHide(ref cancel);
			if (!cancel) {
				var cancelArgs = new HideScreenEventArgs();
				ScreenHidden?.Invoke(this, cancelArgs);
			}
		}

		internal void NotifyScreenDestroyed() {
			OnDestroyScreen();
			ScreenDestroyed?.Invoke(this, EventArgs.Empty);
		}
	}
}

