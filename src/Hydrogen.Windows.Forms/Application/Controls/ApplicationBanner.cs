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
using Hydrogen.Application;
using Microsoft.Extensions.DependencyInjection;


namespace Hydrogen.Windows.Forms;

public partial class ApplicationBanner : ApplicationControl {
	private string _companyName;
	private string _version;
	private string _title;

	public ApplicationBanner()
		: base() {
		InitializeComponent();

		// Download child control defaults as defaults at construction
		CompanyName = _companyNameLabel.Text;
		Version = _versionLabel.Text;
		Title = _productNameLabel.Text;
		ApplicationIcon = Tools.Values.Future.LazyLoad(() => HydrogenFramework.Instance.ServiceProvider.GetService<IApplicationIconProvider>().ApplicationIcon);
	}

	protected IFuture<Icon> ApplicationIcon { get; }
	protected override void OnLoad(EventArgs e) {
		base.OnLoad(e);
	}

	protected override void InitializeUIPrimingData() {
		base.InitializeUIPrimingData();
		_iconPanel.BackgroundImage = ApplicationIcon.Value.ToBitmap().Resize(new Size(_iconPanel.Width, _iconPanel.Height), ResizeMethod.AspectFitPadded, paddingColor: Color.Transparent);
		// Upload to child controls the processed strings. Visual inheritance can be used to change these strings.
		_companyNameLabel.Text = Hydrogen.StringFormatter.FormatEx(CompanyName);
		_productNameLabel.Text = Hydrogen.StringFormatter.FormatEx(Title);
		_versionLabel.Text = Hydrogen.StringFormatter.FormatEx(Version);
	}


	[Category("Appearance")]
	[DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
	public new string CompanyName {
		get { return _companyName; }
		set {
			_companyName = value;
			if (Tools.Runtime.IsDesignMode) {
				_companyNameLabel.Text = _companyName;
			}
			if (Loaded) {
				_companyNameLabel.Text = Hydrogen.StringFormatter.FormatEx(_companyName);
			}
			Invalidate();
		}
	}

	[Category("Appearance")]
	[DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
	public string Version {
		get { return _version; }
		set {
			_version = value;
			if (Tools.Runtime.IsDesignMode) {
				_versionLabel.Text = _version;
			}
			if (Loaded) {
				_versionLabel.Text = Hydrogen.StringFormatter.FormatEx(_version);
			}
			Invalidate();
		}
	}

	[Category("Appearance")]
	[DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
	public string Title {
		get { return _title; }
		set {
			_title = value;
			if (Tools.Runtime.IsDesignMode) {
				_productNameLabel.Text = _title;
			}
			if (Loaded) {
				_productNameLabel.Text = Hydrogen.StringFormatter.FormatEx(_title);
			}
			Invalidate();
		}
	}

	[Category("Appearance")]
	[DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
	public Color FromColor {
		get { return _gradientPanel.FromColor; }
		set {
			_gradientPanel.FromColor = value;
			Invalidate();
		}
	}

	[Category("Appearance")]
	[DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
	public Color ToColor {
		get { return _gradientPanel.ToColor; }
		set {
			_gradientPanel.ToColor = value;
			Invalidate();
		}
	}

	private void _iconPanel_SizeChanged(object sender, EventArgs e) {
		if (Loaded) {
			_iconPanel.BackgroundImage = ApplicationIcon.Value.ToBitmap(_iconPanel.Width, _iconPanel.Height);
		}
	}

}
