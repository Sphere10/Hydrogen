// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;
using System.Drawing;
using System.Windows.Forms;

namespace Hydrogen.Windows.Forms;

public class PictureBoxEx : PictureBox {
	private SystemIconType _systemIcon;

	public PictureBoxEx() {
		SystemIcon = SystemIconType.None;
		SetStyle(ControlStyles.SupportsTransparentBackColor, true);

	}

	protected override void OnSizeChanged(EventArgs e) {
		base.OnSizeChanged(e);
		SetSystemImage();
	}

	protected override void OnDpiChangedAfterParent(EventArgs e) {
		SetSystemImage();
	}


	public SystemIconType SystemIcon {
		get => _systemIcon;
		set {
			_systemIcon = value;
			if (_systemIcon != SystemIconType.None)
				SetSystemImage();
		}
	}

	private void SetSystemImage() {
		Icon icon;
		var preferredIconSize = Size;
		switch (_systemIcon) {
			case SystemIconType.None:
				icon = null;
				break;
			case SystemIconType.Application:
				icon = new Icon(SystemIcons.Application, preferredIconSize);
				break;
			case SystemIconType.Asterisk:
				icon = new Icon(Resources.WarningShield_128x128, preferredIconSize);
				; //new Icon(SystemIcons.Asterisk, Size);
				break;
			case SystemIconType.Error:
				icon = new Icon(Resources.ErrorCircle_128x128, preferredIconSize);
				; //new Icon(SystemIcons.Error, Size);
				break;
			case SystemIconType.Exclamation:
				icon = new Icon(Resources.WarningShield_128x128, preferredIconSize);
				; //new Icon(SystemIcons.Exclamation, Size);
				break;
			case SystemIconType.Hand:
				icon = new Icon(SystemIcons.Hand, preferredIconSize);
				break;
			case SystemIconType.Information:
				icon = new Icon(Resources.InfoCircle_128x128, preferredIconSize);
				; //  new Icon(SystemIcons.Information, Size);
				break;
			case SystemIconType.Question:
				icon = new Icon(Resources.QuestionCircle_128x128, preferredIconSize);
				; //new Icon(SystemIcons.Question, Size);
				break;
			case SystemIconType.Warning:
				icon = new Icon(Resources.WarningTriangle_128x128, preferredIconSize);
				; //new Icon(SystemIcons.Warning, Size);
				break;
			case SystemIconType.WinLogo:
				icon = new Icon(Resources.VistaShield_128x128, preferredIconSize);
				; //new Icon(SystemIcons.WinLogo, Size);
				break;
			case SystemIconType.Shield:
				icon = new Icon(Resources.VistaShield_128x128, preferredIconSize);
				; //new Icon(SystemIcons.Shield, Size);
				break;
			default:
				throw new Exception($"Unsupported SystemIconType '{_systemIcon}'");

		}

		if (icon != null)
			this.Image = icon.ToBitmap().ResizeAndDispose(Size, ResizeMethod.AspectFit);

	}

}
