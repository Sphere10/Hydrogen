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
using Hydrogen.Application;
using Microsoft.Extensions.DependencyInjection;

namespace Hydrogen.Windows.Forms;

#warning Add icons
#warning Add menus
#warning Add plugin stuff to menus
#warning Add restore mainform to lastsize


public partial class BlockMainForm : MainForm, IBlockManager {

	#region Form activation/destruction

	public BlockMainForm() {
		InitializeComponent();

		// initialize local members
		PluginBindings = new Dictionary<IApplicationBlock, TaskPane>();
		MenuBindings = new Dictionary<IMenu, Expando>();
		MenuItemBindings = new Dictionary<Control, IMenuItem>();
		ToolStripBindings = new Dictionary<ToolStripItem, IMenuItem>();
		Plugins = new List<IApplicationBlock>();
		ActivePlugin = null;
		ActiveViewButtons = new List<ToolStripItem>();
		ActiveScreen = null;
		LongRunningScreens = new Dictionary<Type, ApplicationScreen>();
	}

	protected override void OnLoad(EventArgs e) {
		base.OnLoad(e);
		if (!Tools.Runtime.IsDesignMode) {
			RebuildToolBar();
		}
	}

	#endregion

	#region Properties

	public IDictionary<IApplicationBlock, TaskPane> PluginBindings { get; set; }

	public IApplicationBlock ActiveBlock { get; set; }

	public ApplicationScreen ActiveScreen { get; set; }

	public List<IApplicationBlock> Blocks { get; set; }

	private IDictionary<IMenu, Expando> MenuBindings { get; set; }

	private IDictionary<Control, IMenuItem> MenuItemBindings { get; set; }

	private IDictionary<ToolStripItem, IMenuItem> ToolStripBindings { get; set; }

	private IList<IApplicationBlock> Plugins { get; set; }

	private IApplicationBlock ActivePlugin { get; set; }

	private List<ToolStripItem> ActiveViewButtons { get; set; }

	private IDictionary<Type, ApplicationScreen> LongRunningScreens { get; set; }

	#endregion

	#region Block management

	public virtual void RegisterBlock(IApplicationBlock plugin) {

		#region Pre-conditions

		Debug.Assert(plugin != null);
		Debug.Assert(!PluginBindings.ContainsKey(plugin));

		#endregion

		this.Text = plugin.Name;

		TaskPane taskPane = CreateApplicationBlockPane(plugin);
		taskPane.AutoScroll = true;
		taskPane.Dock = DockStyle.Fill;
		taskPane.Size = new Size(
			_applicationBar.Width,
			_applicationBar.Height
		);
		_applicationBar.AddItem(
			new ApplicationBar.Item(
				taskPane,
				plugin.Image32x32,
				plugin.Name
			)
		);

		Plugins.Add(plugin);
		PluginBindings.Add(plugin, taskPane);

		if (ActiveBlock == null) {
			ActiveBlock = plugin;
		}
		if (plugin.ShowInMenuStrip) {
			RegisterBlockInMenu(plugin);
		}
		RebuildToolBar();

#warning Execute these on form load rather than now?
		foreach (IMenu menu in plugin.Menus) {
			foreach (IMenuItem menuItem in menu.Items) {
				if (menuItem.ExecuteOnLoad) {
					ExecuteMenuItem(menuItem);
				}
			}
		}

	}

	public virtual void UnregisterBlock(IApplicationBlock plugin) {

		#region Pre-conditions

		Debug.Assert(plugin != null);
		Debug.Assert(!PluginBindings.ContainsKey(plugin));

		#endregion

		try {
			_splitContainer.Panel1.Controls.Remove(PluginBindings[plugin]);
			DestroyPlugin(plugin, PluginBindings[plugin]);
			PluginBindings.Remove(plugin);
			plugin.Dispose();
			RebuildToolBar();
		} catch (Exception e) {
			ExceptionDialog.Show(e);
		}
	}

	public virtual bool IsBlockRegistered(IApplicationBlock plugin) {

		#region Pre-conditions

		Debug.Assert(plugin != null);

		#endregion

		return PluginBindings.ContainsKey(plugin);
	}

	public virtual IEnumerable<IApplicationBlock> RegisteredBlocks {
		get { return Plugins; }
	}

	public virtual void ExecuteMenuItem(IMenuItem menuItem) {
		try {
			if (menuItem is IControlMenuItem) {
				ExecuteControlMenuItem(menuItem as IControlMenuItem);
			} else if (menuItem is IScreenMenuItem) {
				ExecuteViewMenuItem(menuItem as IScreenMenuItem);
			} else if (menuItem is ILinkMenuItem) {
				ExecuteLinkMenuItem(menuItem as ILinkMenuItem);
			}
		} catch (Exception e) {
			ExceptionDialog.Show(e);
		}
	}

	private void ExecuteViewMenuItem(IScreenMenuItem viewItem) {
		ShowScreen(
			ConstructScreen(viewItem.Parent.Parent, viewItem.Screen)
		);
		ExecuteLinkMenuItem(viewItem);

	}

	private void ExecuteLinkMenuItem(ILinkMenuItem linkItem) {
		linkItem.OnSelect();
	}

	private void ExecuteControlMenuItem(IControlMenuItem controlItem) {
		throw new NotImplementedException();
	}

	private Control CreateControlMenuItem(IControlMenuItem item) {
		return item.ControlToShow;
	}

	private Control CreateViewMenuItem(IScreenMenuItem viewItem) {
		TaskItem taskItem = new TaskItem();
		taskItem.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
		taskItem.BackColor = System.Drawing.Color.Transparent;
		taskItem.Image = viewItem.Image16x16;
		taskItem.Name = "N/A";
		taskItem.Text = viewItem.Text;
		taskItem.TextAlign = System.Drawing.ContentAlignment.TopLeft;
		taskItem.UseVisualStyleBackColor = false;
		taskItem.Click += new EventHandler(TaskItem_Clicked);
		return taskItem;
	}

	private Control CreateLinkMenuItem(ILinkMenuItem item) {
		TaskItem taskItem = new TaskItem();
		taskItem.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
		taskItem.BackColor = System.Drawing.Color.Transparent;
		taskItem.Image = item.Image16x16;
		taskItem.Name = "N/A";
		taskItem.Text = item.Text;
		taskItem.TextAlign = System.Drawing.ContentAlignment.TopLeft;
		taskItem.Font = new Font(taskItem.Font, FontStyle.Underline);
		taskItem.UseVisualStyleBackColor = false;
		taskItem.Click += new EventHandler(TaskItem_Clicked);
		return taskItem;
	}

	private Control CreateMenuItem(IMenuItem item) {

		#region Pre-conditions

		Debug.Assert(item != null);

		#endregion

		Control menuItem = null;
		if (item is IControlMenuItem) {
			menuItem = CreateControlMenuItem(item as IControlMenuItem);
		} else if (item is IScreenMenuItem) {
			menuItem = CreateViewMenuItem(item as IScreenMenuItem);
		} else if (item is ILinkMenuItem) {
			menuItem = CreateLinkMenuItem(item as ILinkMenuItem);
		}
		MenuItemBindings.Add(
			menuItem,
			item
		);

		#region Post-conditions

		Debug.Assert(menuItem != null);

		#endregion

		return menuItem;
	}

	private Expando CreateMenu(IMenu menu) {
		Expando expando = new Expando();
		expando.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
		expando.Animate = true;
		expando.AutoLayout = true;
		expando.Font = new System.Drawing.Font("Tahoma", 8.25F);
		expando.Location = new System.Drawing.Point(12, 12);
		expando.Name = "N/A";
		expando.Size = new System.Drawing.Size(179, 60);
		expando.TabIndex = 0;
		expando.Text = menu.Text;
		expando.TitleImage = menu.Image32x32;
		expando.SizeChanged += expando_SizeChanged;
		foreach (IMenuItem item in menu.Items) {
			if (item.ShowOnExplorerBar) {
				expando.Items.Add(
					CreateMenuItem(item)
				);
			}
		}

		MenuBindings.Add(menu, expando);
		return expando;
	}

	void expando_SizeChanged(object sender, EventArgs e) {
	}

	private TaskPane CreateApplicationBlockPane(IApplicationBlock plugin) {
		TaskPane taskPane = new TaskPane();
		taskPane.AutoScroll = true;
		taskPane.AutoScrollMargin = new System.Drawing.Size(12, 12);
		taskPane.Dock = System.Windows.Forms.DockStyle.Fill;
		taskPane.Location = new System.Drawing.Point(0, 0);
		taskPane.Name = plugin.Name + " TaskPane";
		taskPane.TabIndex = 0;
		taskPane.Text = "N/A";
		foreach (IMenu menu in plugin.Menus) {
			taskPane.Expandos.Add(
				CreateMenu(menu)
			);
		}
		return taskPane;
	}

	private void DestroyControlMenuItem(IControlMenuItem item) {
		throw new NotImplementedException();
	}

	private void DestroyViewMenuItem(IScreenMenuItem item) {
		throw new NotImplementedException();
	}

	private void DestroyLinkMenuItem(ILinkMenuItem item) {
		throw new NotImplementedException();
	}

	private void DestroyMenuItem(IMenuItem item) {

		#region Pre-conditions

		Debug.Assert(item != null);

		#endregion

		if (item is IControlMenuItem) {
			DestroyControlMenuItem(item as IControlMenuItem);
		} else if (item is IScreenMenuItem) {
			DestroyViewMenuItem(item as IScreenMenuItem);
		} else if (item is ILinkMenuItem) {
			DestroyLinkMenuItem(item as ILinkMenuItem);
		}
	}

	private void DestroyMenu(IMenu menu, Expando expando) {
		foreach (IMenuItem item in menu.Items) {
			if (item.ShowOnExplorerBar) {
				DestroyMenuItem(item);
			}
		}

		expando.Dispose();
	}

	private void DestroyPlugin(IApplicationBlock plugin, TaskPane taskPane) {
		foreach (IMenu menu in plugin.Menus) {
			DestroyMenu(menu, MenuBindings[menu]);
			MenuBindings.Remove(menu);
		}
		taskPane.Dispose();
		// remove link from explorer bar
	}

	private void TaskItem_Clicked(object sender, EventArgs e) {
		if (sender is TaskItem) {
			TaskItem taskItem = sender as TaskItem;
			if (MenuItemBindings.ContainsKey(taskItem)) {
				IMenuItem menuItem = MenuItemBindings[taskItem];
				ExecuteMenuItem(menuItem);
			} else {
#warning TaskItem did not bind to a IMenuItem
			}
		}
	}

	private void LinkItem_Clicked(object sender, EventArgs e) {
		if (sender is TaskItem) {
			TaskItem taskItem = sender as TaskItem;
			if (MenuItemBindings.ContainsKey(taskItem)) {
				ILinkMenuItem menuItem = MenuItemBindings[taskItem] as ILinkMenuItem;
				ExecuteMenuItem(menuItem);
			} else {
#warning TaskItem did not bind to a IMenuItem
			}
		}
	}

	#endregion

	#region Menu & Toolbar management

	private void RegisterBlockInMenu(IApplicationBlock block) {
		ToolStripMenuItem blockHeader = new ToolStripMenuItem(
			block.Name
		);

		// register each menu in block
		foreach (IMenu menu in block.Menus) {

			if (menu.ShowInMenuStrip) {

				ToolStripMenuItem menuHeader = new ToolStripMenuItem(menu.Text);
				menuHeader.Tag = menu;
				foreach (IMenuItem item in menu.Items) {
					ILinkMenuItem linkItem = item as ILinkMenuItem;
					if (linkItem.ShowOnExplorerBar) {
						ToolStripMenuItem newItem = new ToolStripMenuItem(
							linkItem.Text,
							linkItem.Image16x16,
							ToolStripItemActivate
						);
						int index = menuHeader.DropDownItems.Add(newItem);
						menuHeader.DropDownItems[index].Tag = item;
						ToolStripBindings.Add(
							newItem,
							item
						);
					}
				}
				blockHeader.DropDownItems.Add(menuHeader);
			}
		}

		// insert the block menu before the help menu
		InsertMenuItemBeforeHelpMenu(blockHeader);
	}

	private void RegisterScreenInMenu(ApplicationScreen screen) {
		ToolStripMenuItem headerMenu = new ToolStripMenuItem(
			screen.ApplicationMenuStripText
		);
		headerMenu.Tag = screen;
		foreach (ToolStripItem menuItem in screen.MenuItems) {
			headerMenu.DropDownItems.Add(
				menuItem
			);
		}

		InsertMenuItemBeforeHelpMenu(headerMenu);
	}

	private void UnregisterScreenFromMenu(ApplicationScreen screen) {
		for (int i = 0; i < MenuStrip.Items.Count; i++) {
			if (MenuStrip.Items[i].Tag == screen) {
				MenuStrip.Items.RemoveAt(i);
				break;
			}
		}
	}

	private void ToolStripItemActivate(object sender, EventArgs e) {
		if (sender is ToolStripItem) {
			ToolStripItem stripItem = sender as ToolStripItem;
			if (ToolStripBindings.ContainsKey(stripItem)) {
				IMenuItem menuItem = ToolStripBindings[stripItem];
				ExecuteMenuItem(menuItem);
			} else {
#warning TaskItem did not bind to a IMenuItem
			}
		}
	}

	private void RebuildToolBar() {
		ToolStrip.SuspendLayout();
		ToolStrip.Items.Clear();

		#region Add standard buttons

		#endregion

		#region Add screen list buttons

		foreach (IApplicationBlock block in Plugins) {
			if (block.ShowInToolStrip) {
				if (ToolStrip.Items.Count > 0) {
					ToolStrip.Items.Add(new ToolStripSeparator());
				}
				foreach (IMenu menu in block.Menus) {
					foreach (IMenuItem item in menu.Items) {
						if (item is ILinkMenuItem) {
							ILinkMenuItem linkItem = item as ILinkMenuItem;
							if (linkItem.ShowOnToolStrip) {
								ToolStripButton button = new ToolStripButton(
									string.Empty,
									item.Image16x16 != null ? item.Image16x16 : Hydrogen.Windows.Forms.Resources.DefaultToolStripImage,
									ToolStripItemActivate
								);
								button.ToolTipText = linkItem.Text;
								ToolStrip.Items.Add(button);
								ToolStripBindings.Add(
									button,
									item
								);
							}
						}
					}
				}
			}
		}

		#endregion

		#region Add active screen buttons

		if (ActiveScreen != null) {
			if (ActiveScreen.ToolBar != null) {
				if (ToolStrip.Items.Count > 0) {
					ToolStrip.Items.Add(new ToolStripSeparator());

				}

				int buttonCount = ActiveScreen.ToolBar.Items.Count;
				ActiveViewButtons.Clear();
				for (int i = 0; i < buttonCount; i++) {
					ActiveViewButtons.Add(
						ActiveScreen.ToolBar.Items[i]
					);
				}
				if (ActiveScreen.ToolBar.Tag == null) {
					foreach (Control ctrl in ActiveScreen.Controls) {
						ctrl.Location = new Point(
							ctrl.Location.X,
							ctrl.Location.Y - ActiveScreen.ToolBar.Height
						);
					}
					ActiveScreen.ToolBar.Visible = false;
					ActiveScreen.ToolBar.Tag = "Removed";
				}
				ToolStrip.Items.AddRange(ActiveViewButtons.ToArray());
			}
		}

		#endregion

		#region Add help buttons

		ToolStrip.Items.Add(new ToolStripSeparator());

		ToolStripItem contextHelpButton = ToolStrip.Items.Add(
			string.Empty,
			Hydrogen.Windows.Forms.Resources.Help_16x16x32,
			ContextHelp_Click
		);
		contextHelpButton.ToolTipText = "Get help for currently opened screen";

		#endregion

		ToolStrip.ResumeLayout();
	}

	private void InsertMenuItemBeforeHelpMenu(ToolStripMenuItem menuItem) {
		bool foundLocation = false;
		int location = 0;
		for (int i = 0; i < MenuStrip.Items.Count; i++) {
			if (MenuStrip.Items[i] == HelpToolStripMenuItem) {
				location = i;
				foundLocation = true;
				break;
			}
		}
		if (!foundLocation) {
			location = 0;
		}
		MenuStrip.Items.Insert(location, menuItem);
	}

	#endregion

	#region Screen management

	private ApplicationScreen ConstructScreen(IApplicationBlock owner, Type screenType) {

		#region Pre-conditions

		Debug.Assert(owner != null);
		Debug.Assert(screenType != null);
		Debug.Assert(screenType.IsSubclassOf(typeof(ApplicationScreen)));

		#endregion

		ApplicationScreen screen = null;
		if (LongRunningScreens.ContainsKey(screenType)) {
			screen = LongRunningScreens[screenType];
		} else {
			try {
				// try to create with parameters
				screen = Activator.CreateInstance(screenType, owner, this) as ApplicationScreen;
			} catch (MissingMethodException) {
				try {
					// try to create with just block parameter
					screen = Activator.CreateInstance(screenType, owner) as ApplicationScreen;
				} catch (MissingMethodException) {
					// try to create without parameters
					screen = Activator.CreateInstance(screenType) as ApplicationScreen;
				}
			}

			#region Validate

			if (screen == null) {
				throw new ApplicationException(
					string.Format(
						"Could not instantiate the screen of type '{0}' as it did not inherit from '{1}'.",
						screenType.Name,
						typeof(ApplicationScreen).Name
					)
				);
			}

			#endregion

			// set screen context if activation did not do so
			//if (screen.WinFormsWinFormsApplicationProvider == null) {
			//    screen.WinFormsWinFormsApplicationProvider = base.WinFormsWinFormsApplicationProvider;
			//}
			if (screen.ApplicationBlock == null) {
				screen.ApplicationBlock = owner;
			}
			// add this screen to long running screens if it is not to be destroyed.
			if (screen.ActivationMode == ScreenActivationMode.KeepAlive) {
				LongRunningScreens.Add(screenType, screen);
			}
		}


		#region Post-conditions

		Debug.Assert(screen != null);

		#endregion

		return screen;

	}

	private void ShowScreen(ApplicationScreen screen) {

		#region Pre-conditions

		Debug.Assert(screen != null);

		#endregion

		var cancelRemove = false;

		// notify current view for permission to remove
		ActiveScreen?.NotifyHideScreen(ref cancelRemove);

		// remove if current view granted permission
		if (!cancelRemove) {
			_splitContainer.Panel2.SuspendLayout();

			// remove the view toolbar items if any
			if (ActiveScreen != null) {
				UnregisterScreenFromMenu(ActiveScreen);
				// put the tool buttons back into the screen toolbar
				ActiveScreen.ToolBar?.Items.AddRange(ActiveViewButtons.ToArray());
				ActiveViewButtons.Clear();

				// remove the view control
				if (_splitContainer.Panel2.Controls.Contains(ActiveScreen)) {
					_splitContainer.Panel2.Controls.Remove(ActiveScreen);
				}
				// dispose view and release its resources
				if (ActiveScreen.ActivationMode == ScreenActivationMode.AlwaysCreate) {
					ActiveScreen.Dispose();
				}
			}

			// collapse menu if new screen demands it
			bool collapseMenu =
				screen.DisplayMode == ScreenDisplayMode.Filled ||
				screen.DisplayMode == ScreenDisplayMode.FilledAndMaximized;
			_splitContainer.Panel1Collapsed = collapseMenu;

			// maximize if new screen demands it
			bool maximize =
				screen.DisplayMode == ScreenDisplayMode.FilledAndMaximized ||
				screen.DisplayMode == ScreenDisplayMode.Maximized;
			if (maximize) {
				base.WindowState = FormWindowState.Maximized;
			}

			// place the new view in the content panel
			screen.Dock = DockStyle.Fill;
			screen.Location = new Point(0, 0);
			screen.Name = "VIEW";
			screen.TabIndex = 0;
			screen.Location = new Point(
				_splitContainer.Panel2.Padding.Left,
				_splitContainer.Panel2.Padding.Top
			);
			Size newSize = new Size(
				_splitContainer.Width -
				(_splitContainer.SplitterDistance + _splitContainer.SplitterWidth) -
				(_splitContainer.Panel2.Padding.Left + _splitContainer.Panel2.Padding.Right),
				_splitContainer.Height -
				(_splitContainer.Panel2.Padding.Top + _splitContainer.Panel2.Padding.Bottom)
			);
			if (collapseMenu) {
				newSize.Width = newSize.Width + _splitContainer.SplitterDistance;
			}
			;
			screen.Size = newSize;
			screen.PerformLayout();
			_splitContainer.Panel2.Controls.Add(screen);
			_splitContainer.Panel2.ResumeLayout(false);

			// set current view sa the active view
			ActiveScreen = screen;

			// place new tool strip items
			if (ActiveScreen.ShowInApplicationMenuStrip) {
				RegisterScreenInMenu(ActiveScreen);
			}
			RebuildToolBar();

			// let view know that it has begun
			ActiveScreen.NotifyShow();
		}
	}

	#endregion

	#region Misc

	public virtual void ShowActiveScreenContextHelp() {
		var helpServices = HydrogenFramework.Instance.ServiceProvider.GetService<IHelpServices>();
		if (helpServices == null)
			return;

		if (ActiveBlock != null && ActiveScreen != null) {
			helpServices.ShowContextHelp(ActiveScreen);
		}
	}

	#endregion

	#region Handlers

	protected virtual void ContextHelp_Click(object sender, EventArgs e) {
		ShowActiveScreenContextHelp();
	}

	protected virtual void MainForm_HelpRequested(object sender, HelpEventArgs hlpevent) {
		ShowActiveScreenContextHelp();
	}

	#endregion

	#region Toolbar Handlers

	#endregion

	#region ApplicationBar Handlres

	private void _applicationBar_ButtonPressed(ApplicationBar source, ApplicationBar.Item button) {
		// purpose here is to set ActivePlugin to current Plugin visible in application bar
		// current taskpane on application bar is current active view
		if (_applicationBar.ApplicationBarControl != null) {
			TaskPane pane = (TaskPane)_applicationBar.ApplicationBarControl;
			foreach (Control control in PluginBindings.Values) {
				if (pane == control) {
					foreach (IApplicationBlock block in PluginBindings.Keys) {
						if (PluginBindings[block] == control) {
							ActiveBlock = block;
							break;
						}
					}
					break;
				}
			}
		}
	}

	#endregion

}
