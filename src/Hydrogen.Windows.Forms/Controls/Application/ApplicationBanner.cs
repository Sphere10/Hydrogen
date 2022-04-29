//-----------------------------------------------------------------------
// <copyright file="ApplicationBanner.cs" company="Sphere 10 Software">
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
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Imaging;
using System.Text;
using System.Windows.Forms;
using Hydrogen;



namespace Hydrogen.Windows.Forms {
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
		}

		protected override void OnLoad(EventArgs e) {
			base.OnLoad(e);
		}

		protected override void InitializeUIPrimingData() {
			base.InitializeUIPrimingData();
			_iconPanel.BackgroundImage = ApplicationServices.ApplicationIcon.ToBitmap().Resize(new Size(_iconPanel.Width, _iconPanel.Height), ResizeMethod.AspectFit);
			// Upload to child controls the processed strings. Visual inheritance can be used to change these strings.
			_companyNameLabel.Text = ApplicationServices.ProductInformation.ProcessTokensInString(CompanyName);
			_productNameLabel.Text = ApplicationServices.ProductInformation.ProcessTokensInString(Title);
			_versionLabel.Text = ApplicationServices.ProductInformation.ProcessTokensInString(Version);
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
					_companyNameLabel.Text = ApplicationServices.ProductInformation.ProcessTokensInString(_companyName);
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
					_versionLabel.Text = ApplicationServices.ProductInformation.ProcessTokensInString(_version);
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
					_productNameLabel.Text = ApplicationServices.ProductInformation.ProcessTokensInString(_title);
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
				_iconPanel.BackgroundImage = ApplicationServices.ApplicationIcon.ToBitmap(_iconPanel.Width, _iconPanel.Height);
			}
		}

	}
}
