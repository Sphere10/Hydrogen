
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows.Forms;

namespace Hydrogen;

public static class NotifyIconExtensions {

	public static void EnableContextMenuOnLeftClick(this NotifyIcon notifyIcon) {
		notifyIcon.MouseUp += (sender, args) => {
			if (args.Button == MouseButtons.Left) {
				MethodInfo mi = typeof(NotifyIcon).GetMethod("ShowContextMenu", BindingFlags.Instance | BindingFlags.NonPublic);
				mi.Invoke(notifyIcon, null);
			}
		};
	}
}
