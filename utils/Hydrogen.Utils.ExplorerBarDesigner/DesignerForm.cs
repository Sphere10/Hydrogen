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
using System.Windows.Forms;
using System.IO;
using System.Drawing.Drawing2D;
using Hydrogen.Windows.Forms;


namespace Hydrogen.Utils.ExplorerBarDesigner;

public partial class DesignerForm : Form {
	public DesignerForm() {
		InitializeComponent();

		_propertyGrid.SelectedObject = ToSettings(
			Tools.Xml.Read<ExplorerBarInfoSurrogate>(
				new StringReader(Resources.LunaExplorerBarTheme)
			)
		);
		_propertyGrid.PropertyValueChanged += new PropertyValueChangedEventHandler(propertyGrid1_PropertyValueChanged);
		ApplySettingsToExplorerBar();
	}

	public ExplorerBarSettings ToSettings(ExplorerBarInfoSurrogate surrogate) {
		ExplorerBarSettings retval = new ExplorerBarSettings();
		retval.ExpandoNormalBackColor = Tools.Drawing.ConvertStringToColor(surrogate.ExpandoInfoSurrogate.NormalBackColor);
		retval.ExpandoNormalBackImage = surrogate.ExpandoInfoSurrogate.NormalBackImage.ToImage();
		retval.ExpandoNormalBorder = surrogate.ExpandoInfoSurrogate.NormalBorder;
		retval.ExpandoNormalBorderColor = Tools.Drawing.ConvertStringToColor(surrogate.ExpandoInfoSurrogate.NormalBorderColor);
		retval.ExpandoNormalPadding = surrogate.ExpandoInfoSurrogate.NormalPadding;
		retval.ExpandoSpecialBackColor = Tools.Drawing.ConvertStringToColor(surrogate.ExpandoInfoSurrogate.SpecialBackColor);
		retval.ExpandoSpecialBackImage = surrogate.ExpandoInfoSurrogate.SpecialBackImage.ToImage();
		retval.ExpandoSpecialBorder = surrogate.ExpandoInfoSurrogate.SpecialBorder;
		retval.ExpandoSpecialBorderColor = Tools.Drawing.ConvertStringToColor(surrogate.ExpandoInfoSurrogate.SpecialBorderColor);
		retval.ExpandoSpecialPadding = surrogate.ExpandoInfoSurrogate.SpecialPadding;
		retval.ExpandoWatermarkAlignment = surrogate.ExpandoInfoSurrogate.WatermarkAlignment;

		retval.HeaderInfoFont = new Font(surrogate.HeaderInfoSurrogate.FontName, surrogate.HeaderInfoSurrogate.FontSize, surrogate.HeaderInfoSurrogate.FontStyle);
		retval.HeaderInfoGradientOffset = surrogate.HeaderInfoSurrogate.GradientOffset;
		retval.HeaderInfoMargin = surrogate.HeaderInfoSurrogate.Margin;
		retval.HeaderInfoNormalAlignment = surrogate.HeaderInfoSurrogate.NormalAlignment;
		retval.HeaderInfoNormalArrowDown = surrogate.HeaderInfoSurrogate.NormalArrowDown.ToImage();
		retval.HeaderInfoNormalArrowDownHot = surrogate.HeaderInfoSurrogate.NormalArrowDownHot.ToImage();
		retval.HeaderInfoNormalArrowUp = surrogate.HeaderInfoSurrogate.NormalArrowUp.ToImage();
		retval.HeaderInfoNormalArrowUpHot = surrogate.HeaderInfoSurrogate.NormalArrowUpHot.ToImage();
		retval.HeaderInfoNormalBackColor = Tools.Drawing.ConvertStringToColor(surrogate.HeaderInfoSurrogate.NormalBackColor);
		retval.HeaderInfoNormalBackImage = surrogate.HeaderInfoSurrogate.NormalBackImage.ToImage();
		retval.HeaderInfoNormalBorder = surrogate.HeaderInfoSurrogate.NormalBorder;
		retval.HeaderInfoNormalBorderColor = Tools.Drawing.ConvertStringToColor(surrogate.HeaderInfoSurrogate.NormalBorderColor);
		retval.HeaderInfoNormalGradientEndColor = Tools.Drawing.ConvertStringToColor(surrogate.HeaderInfoSurrogate.NormalGradientEndColor);
		retval.HeaderInfoNormalGradientStartColor = Tools.Drawing.ConvertStringToColor(surrogate.HeaderInfoSurrogate.NormalGradientStartColor);
		retval.HeaderInfoNormalPadding = surrogate.HeaderInfoSurrogate.NormalPadding;
		retval.HeaderInfoNormalTitleColor = Tools.Drawing.ConvertStringToColor(surrogate.HeaderInfoSurrogate.NormalTitle);
		retval.HeaderInfoNormalTitleHotColor = Tools.Drawing.ConvertStringToColor(surrogate.HeaderInfoSurrogate.NormalTitleHot);
		retval.HeaderInfoSpecialAlignment = surrogate.HeaderInfoSurrogate.SpecialAlignment;
		retval.HeaderInfoSpecialArrowDown = surrogate.HeaderInfoSurrogate.SpecialArrowDown.ToImage();
		retval.HeaderInfoSpecialArrowDownHot = surrogate.HeaderInfoSurrogate.SpecialArrowDownHot.ToImage();
		retval.HeaderInfoSpecialArrowUp = surrogate.HeaderInfoSurrogate.SpecialArrowUp.ToImage();
		retval.HeaderInfoSpecialArrowUpHot = surrogate.HeaderInfoSurrogate.SpecialArrowUpHot.ToImage();
		retval.HeaderInfoSpecialBackColor = Tools.Drawing.ConvertStringToColor(surrogate.HeaderInfoSurrogate.SpecialBackColor);
		retval.HeaderInfoSpecialBackImage = surrogate.HeaderInfoSurrogate.SpecialBackImage.ToImage();
		retval.HeaderInfoSpecialBorder = surrogate.HeaderInfoSurrogate.SpecialBorder;
		retval.HeaderInfoSpecialBorderColor = Tools.Drawing.ConvertStringToColor(surrogate.HeaderInfoSurrogate.SpecialBorderColor);
		retval.HeaderInfoSpecialGradientEndColor = Tools.Drawing.ConvertStringToColor(surrogate.HeaderInfoSurrogate.SpecialGradientEndColor);
		retval.HeaderInfoSpecialGradientStartColor = Tools.Drawing.ConvertStringToColor(surrogate.HeaderInfoSurrogate.SpecialGradientStartColor);
		retval.HeaderInfoSpecialPadding = surrogate.HeaderInfoSurrogate.SpecialPadding;
		retval.HeaderInfoSpecialTitleColor = Tools.Drawing.ConvertStringToColor(surrogate.HeaderInfoSurrogate.SpecialTitle);
		retval.HeaderInfoSpecialTitleHotColor = Tools.Drawing.ConvertStringToColor(surrogate.HeaderInfoSurrogate.SpecialTitleHot);
		retval.HeaderInfoTitleGradient = surrogate.HeaderInfoSurrogate.TitleGradient;
		retval.HeaderInfoTitleRadius = surrogate.HeaderInfoSurrogate.TitleRadius;

		retval.TaskItemFontDecoration = surrogate.TaskItemInfoSurrogate.FontDecoration;
		retval.TaskItemLinkHotColor = Tools.Drawing.ConvertStringToColor(surrogate.TaskItemInfoSurrogate.LinkHot);
		retval.TaskItemLinkNormalColor = Tools.Drawing.ConvertStringToColor(surrogate.TaskItemInfoSurrogate.LinkNormal);
		retval.TaskItemMargin = surrogate.TaskItemInfoSurrogate.Margin;
		retval.TaskItemPadding = surrogate.TaskItemInfoSurrogate.Padding;

		retval.TaskPaneGradientEnd = Tools.Drawing.ConvertStringToColor(surrogate.TaskPaneInfoSurrogate.GradientEndColor);
		retval.TaskPaneGradientStart = Tools.Drawing.ConvertStringToColor(surrogate.TaskPaneInfoSurrogate.GradientStartColor);
		retval.TaskPanelGradientDirection = surrogate.TaskPaneInfoSurrogate.GradientDirection;
		retval.TaskPanePadding = surrogate.TaskPaneInfoSurrogate.Padding;
		retval.TaskPaneStretchMode = surrogate.TaskPaneInfoSurrogate.StretchMode;
		retval.TaskPaneWaterMark = surrogate.TaskPaneInfoSurrogate.Watermark.ToImage();
		retval.TaskPaneWaterMarkContentAlignment = surrogate.TaskPaneInfoSurrogate.WatermarkAlignment;

		return retval;
	}

	public ExplorerBarInfoSurrogate FromSettings(ExplorerBarSettings settings) {
		ExplorerBarInfoSurrogate retval = new ExplorerBarInfoSurrogate();
		retval.TaskPaneInfoSurrogate = new TaskPaneInfoSurrogate();
		retval.TaskItemInfoSurrogate = new TaskItemInfoSurrogate();
		retval.ExpandoInfoSurrogate = new ExpandoInfoSurrogate();
		retval.HeaderInfoSurrogate = new HeaderInfoSurrogate();

		retval.ExpandoInfoSurrogate.NormalBackColor = Tools.Drawing.ConvertColorToString(settings.ExpandoNormalBackColor);
		retval.ExpandoInfoSurrogate.NormalBackImage = settings.ExpandoNormalBackImage.ToByteArray();
		retval.ExpandoInfoSurrogate.NormalBorder = settings.ExpandoNormalBorder;
		retval.ExpandoInfoSurrogate.NormalBorderColor = Tools.Drawing.ConvertColorToString(settings.ExpandoNormalBorderColor);
		retval.ExpandoInfoSurrogate.NormalPadding = settings.ExpandoNormalPadding;
		retval.ExpandoInfoSurrogate.SpecialBackColor = Tools.Drawing.ConvertColorToString(settings.ExpandoSpecialBackColor);
		retval.ExpandoInfoSurrogate.SpecialBackImage = settings.ExpandoSpecialBackImage.ToByteArray();
		retval.ExpandoInfoSurrogate.SpecialBorder = settings.ExpandoSpecialBorder;
		retval.ExpandoInfoSurrogate.SpecialBorderColor = Tools.Drawing.ConvertColorToString(settings.ExpandoSpecialBorderColor);
		retval.ExpandoInfoSurrogate.SpecialPadding = settings.ExpandoSpecialPadding;
		retval.ExpandoInfoSurrogate.WatermarkAlignment = settings.ExpandoWatermarkAlignment;

		retval.HeaderInfoSurrogate.FontName = settings.HeaderInfoFont.Name;
		retval.HeaderInfoSurrogate.FontSize = settings.HeaderInfoFont.Size;
		retval.HeaderInfoSurrogate.FontStyle = settings.HeaderInfoFont.Style;
		retval.HeaderInfoSurrogate.GradientOffset = settings.HeaderInfoGradientOffset;
		retval.HeaderInfoSurrogate.Margin = settings.HeaderInfoMargin;
		retval.HeaderInfoSurrogate.NormalAlignment = settings.HeaderInfoNormalAlignment;
		retval.HeaderInfoSurrogate.NormalArrowDown = settings.HeaderInfoNormalArrowDown.ToByteArray();
		retval.HeaderInfoSurrogate.NormalArrowDownHot = settings.HeaderInfoNormalArrowDownHot.ToByteArray();
		retval.HeaderInfoSurrogate.NormalArrowUp = settings.HeaderInfoNormalArrowUp.ToByteArray();
		retval.HeaderInfoSurrogate.NormalArrowUpHot = settings.HeaderInfoNormalArrowUpHot.ToByteArray();
		retval.HeaderInfoSurrogate.NormalBackColor = Tools.Drawing.ConvertColorToString(settings.HeaderInfoNormalBackColor);
		retval.HeaderInfoSurrogate.NormalBackImage = settings.HeaderInfoNormalBackImage.ToByteArray();
		retval.HeaderInfoSurrogate.NormalBorder = settings.HeaderInfoNormalBorder;
		retval.HeaderInfoSurrogate.NormalBorderColor = Tools.Drawing.ConvertColorToString(settings.HeaderInfoNormalBorderColor);
		retval.HeaderInfoSurrogate.NormalGradientEndColor = Tools.Drawing.ConvertColorToString(settings.HeaderInfoNormalGradientEndColor);
		retval.HeaderInfoSurrogate.NormalGradientStartColor = Tools.Drawing.ConvertColorToString(settings.HeaderInfoNormalGradientStartColor);
		retval.HeaderInfoSurrogate.NormalPadding = settings.HeaderInfoNormalPadding;
		retval.HeaderInfoSurrogate.NormalTitle = Tools.Drawing.ConvertColorToString(settings.HeaderInfoNormalTitleColor);
		retval.HeaderInfoSurrogate.NormalTitleHot = Tools.Drawing.ConvertColorToString(settings.HeaderInfoNormalTitleHotColor);
		retval.HeaderInfoSurrogate.SpecialAlignment = settings.HeaderInfoSpecialAlignment;
		retval.HeaderInfoSurrogate.SpecialArrowDown = settings.HeaderInfoSpecialArrowDown.ToByteArray();
		retval.HeaderInfoSurrogate.SpecialArrowDownHot = settings.HeaderInfoSpecialArrowDownHot.ToByteArray();
		retval.HeaderInfoSurrogate.SpecialArrowUp = settings.HeaderInfoSpecialArrowUp.ToByteArray();
		retval.HeaderInfoSurrogate.SpecialArrowUpHot = settings.HeaderInfoSpecialArrowUpHot.ToByteArray();
		retval.HeaderInfoSurrogate.SpecialBackColor = Tools.Drawing.ConvertColorToString(settings.HeaderInfoSpecialBackColor);
		retval.HeaderInfoSurrogate.SpecialBackImage = settings.HeaderInfoSpecialBackImage.ToByteArray();
		retval.HeaderInfoSurrogate.SpecialBorder = settings.HeaderInfoSpecialBorder;
		retval.HeaderInfoSurrogate.SpecialBorderColor = Tools.Drawing.ConvertColorToString(settings.HeaderInfoSpecialBorderColor);
		retval.HeaderInfoSurrogate.SpecialGradientEndColor = Tools.Drawing.ConvertColorToString(settings.HeaderInfoSpecialGradientEndColor);
		retval.HeaderInfoSurrogate.SpecialGradientStartColor = Tools.Drawing.ConvertColorToString(settings.HeaderInfoSpecialGradientStartColor);
		retval.HeaderInfoSurrogate.SpecialPadding = settings.HeaderInfoSpecialPadding;
		retval.HeaderInfoSurrogate.SpecialTitle = Tools.Drawing.ConvertColorToString(settings.HeaderInfoSpecialTitleColor);
		retval.HeaderInfoSurrogate.SpecialTitleHot = Tools.Drawing.ConvertColorToString(settings.HeaderInfoSpecialTitleHotColor);
		retval.HeaderInfoSurrogate.TitleGradient = settings.HeaderInfoTitleGradient;
		retval.HeaderInfoSurrogate.TitleRadius = settings.HeaderInfoTitleRadius;

		retval.TaskItemInfoSurrogate.FontDecoration = settings.TaskItemFontDecoration;
		retval.TaskItemInfoSurrogate.LinkHot = Tools.Drawing.ConvertColorToString(settings.TaskItemLinkHotColor);
		retval.TaskItemInfoSurrogate.LinkNormal = Tools.Drawing.ConvertColorToString(settings.TaskItemLinkNormalColor);
		retval.TaskItemInfoSurrogate.Margin = settings.TaskItemMargin;
		retval.TaskItemInfoSurrogate.Padding = settings.TaskItemPadding;

		retval.TaskPaneInfoSurrogate.GradientEndColor = Tools.Drawing.ConvertColorToString(settings.TaskPaneGradientEnd);
		retval.TaskPaneInfoSurrogate.GradientStartColor = Tools.Drawing.ConvertColorToString(settings.TaskPaneGradientStart);
		retval.TaskPaneInfoSurrogate.GradientDirection = settings.TaskPanelGradientDirection;
		retval.TaskPaneInfoSurrogate.Padding = settings.TaskPanePadding;
		retval.TaskPaneInfoSurrogate.StretchMode = settings.TaskPaneStretchMode;
		retval.TaskPaneInfoSurrogate.Watermark = settings.TaskPaneWaterMark.ToByteArray();
		retval.TaskPaneInfoSurrogate.WatermarkAlignment = settings.TaskPaneWaterMarkContentAlignment;

		return retval;
	}


	private void propertyGrid1_PropertyValueChanged(object s, PropertyValueChangedEventArgs e) {
		ApplySettingsToExplorerBar();
	}

	private void ApplySettingsToExplorerBar() {
		taskPane1.UseCustomTheme(
			FromSettings((ExplorerBarSettings)_propertyGrid.SelectedObject)
		);
	}

	private void _exportButton_Click(object sender, EventArgs e) {
		ExportForm.ShowDialog(
			this,
			Tools.Xml.WriteToString(
				FromSettings((ExplorerBarSettings)_propertyGrid.SelectedObject)
			)
		);
	}
}


public class ExplorerBarSettings {

	public ExplorerBarSettings() {
		Image defaultImage = new Bitmap(1, 1);
		Color defaultColor = Color.Black;
		Font defaultFont = new Font(FontFamily.GenericSansSerif, 1);
		this.ExpandoNormalBackColor = defaultColor;
		this.ExpandoNormalBackImage = defaultImage;
		this.ExpandoNormalBorder = Border.Empty;
		this.ExpandoNormalBorderColor = defaultColor;
		this.ExpandoNormalPadding = Hydrogen.Windows.Forms.PaddingEx.Empty;
		this.ExpandoSpecialBackColor = Color.Black;
		this.ExpandoSpecialBackImage = defaultImage;
		this.ExpandoSpecialBorder = Border.Empty;
		this.ExpandoSpecialBorderColor = defaultColor;
		this.ExpandoSpecialPadding = PaddingEx.Empty;
		this.ExpandoWatermarkAlignment = ContentAlignment.BottomCenter;
		this.HeaderInfoFont = defaultFont;
		this.HeaderInfoGradientOffset = 1;
		this.HeaderInfoMargin = 1;
		this.HeaderInfoNormalAlignment = ContentAlignment.BottomCenter;
		this.HeaderInfoNormalArrowDown = defaultImage;
		this.HeaderInfoNormalArrowDownHot = defaultImage;
		this.ExpandoNormalBorder = Border.Empty;
		this.ExpandoNormalBorderColor = defaultColor;
		this.ExpandoNormalPadding = PaddingEx.Empty;
		this.ExpandoSpecialBackColor = defaultColor;
		this.ExpandoSpecialBackImage = defaultImage;
		this.ExpandoSpecialBorder = Border.Empty;
		this.ExpandoSpecialBorderColor = defaultColor;
		this.ExpandoSpecialPadding = PaddingEx.Empty;
		this.ExpandoWatermarkAlignment = ContentAlignment.BottomCenter;
		this.HeaderInfoFont = defaultFont;
		this.HeaderInfoGradientOffset = 1;
		this.HeaderInfoMargin = 0;
		this.HeaderInfoNormalAlignment = ContentAlignment.BottomCenter;
		this.HeaderInfoNormalArrowDown = defaultImage;
		this.HeaderInfoNormalArrowDownHot = defaultImage;
		this.HeaderInfoNormalArrowUp = defaultImage;
		this.HeaderInfoNormalArrowUpHot = defaultImage;
		this.HeaderInfoNormalBackColor = defaultColor;
		this.HeaderInfoNormalBackImage = defaultImage;
		this.HeaderInfoNormalBorder = Border.Empty;
		this.HeaderInfoNormalBorderColor = defaultColor;
		this.HeaderInfoNormalGradientEndColor = defaultColor;
		this.HeaderInfoNormalGradientStartColor = defaultColor;
		this.HeaderInfoNormalPadding = PaddingEx.Empty;
		this.HeaderInfoNormalTitleColor = defaultColor;
		this.HeaderInfoNormalTitleHotColor = defaultColor;
		this.HeaderInfoSpecialAlignment = ContentAlignment.BottomCenter;
		this.HeaderInfoSpecialArrowDown = defaultImage;
		this.HeaderInfoSpecialArrowDownHot = defaultImage;
		this.HeaderInfoSpecialArrowUp = defaultImage;
		this.HeaderInfoSpecialArrowUpHot = defaultImage;
		this.HeaderInfoSpecialBackColor = defaultColor;
		this.HeaderInfoSpecialBackImage = defaultImage;
		this.HeaderInfoSpecialBorder = Border.Empty;
		this.HeaderInfoSpecialBorderColor = defaultColor;
		this.HeaderInfoSpecialGradientEndColor = defaultColor;
		this.HeaderInfoSpecialGradientStartColor = defaultColor;
		this.HeaderInfoSpecialPadding = PaddingEx.Empty;
		this.HeaderInfoSpecialTitleColor = defaultColor;
		this.HeaderInfoSpecialTitleHotColor = defaultColor;
		this.HeaderInfoTitleGradient = false;
		this.HeaderInfoTitleRadius = 0;
		this.TaskItemFontDecoration = FontStyle.Regular;
		this.TaskItemLinkHotColor = defaultColor;
		this.TaskItemLinkNormalColor = defaultColor;
		this.TaskItemMargin = Margin.Empty;
		this.TaskItemPadding = PaddingEx.Empty;
		this.TaskPaneGradientEnd = defaultColor;
		this.TaskPaneGradientStart = defaultColor;
		this.TaskPanelGradientDirection = LinearGradientMode.BackwardDiagonal;
		this.TaskPanePadding = PaddingEx.Empty;
		this.TaskPaneStretchMode = ImageStretchMode.Normal;
		this.TaskPaneWaterMark = defaultImage;
		this.TaskPaneWaterMarkContentAlignment = ContentAlignment.BottomCenter;
	}

	[Category("Task Pane")] public Color TaskPaneGradientStart { get; set; }
	[Category("Task Pane")] public Color TaskPaneGradientEnd { get; set; }
	[Category("Task Pane")] public LinearGradientMode TaskPanelGradientDirection { get; set; }
	[Category("Task Pane")] public PaddingEx TaskPanePadding { get; set; }
	[Category("Task Pane")] public ImageStretchMode TaskPaneStretchMode { get; set; }
	[Category("Task Pane")] public Image TaskPaneWaterMark { get; set; }
	[Category("Task Pane")] public ContentAlignment TaskPaneWaterMarkContentAlignment { get; set; }

	[Category("Task Item")] public PaddingEx TaskItemPadding { get; set; }
	[Category("Task Item")] public Margin TaskItemMargin { get; set; }
	[Category("Task Item")] public Color TaskItemLinkNormalColor { get; set; }
	[Category("Task Item")] public Color TaskItemLinkHotColor { get; set; }
	[Category("Task Item")] public FontStyle TaskItemFontDecoration { get; set; }


	[Category("Task Expando")] public Color ExpandoNormalBackColor { get; set; }
	[Category("Task Expando")] public Border ExpandoNormalBorder { get; set; }
	[Category("Task Expando")] public Color ExpandoNormalBorderColor { get; set; }
	[Category("Task Expando")] public PaddingEx ExpandoNormalPadding { get; set; }
	[Category("Task Expando")] public Image ExpandoNormalBackImage { get; set; }
	[Category("Task Expando")] public Color ExpandoSpecialBackColor { get; set; }
	[Category("Task Expando")] public Border ExpandoSpecialBorder { get; set; }
	[Category("Task Expando")] public Color ExpandoSpecialBorderColor { get; set; }
	[Category("Task Expando")] public PaddingEx ExpandoSpecialPadding { get; set; }
	[Category("Task Expando")] public Image ExpandoSpecialBackImage { get; set; }
	[Category("Task Expando")] public ContentAlignment ExpandoWatermarkAlignment { get; set; }

	[Category("Header Info")] public Font HeaderInfoFont { get; set; }
	[Category("Header Info")] public int HeaderInfoMargin { get; set; }
	[Category("Header Info")] public Image HeaderInfoSpecialBackImage { get; set; }
	[Category("Header Info")] public Image HeaderInfoNormalBackImage { get; set; }
	[Category("Header Info")] public Color HeaderInfoSpecialTitleColor { get; set; }
	[Category("Header Info")] public Color HeaderInfoNormalTitleColor { get; set; }
	[Category("Header Info")] public Color HeaderInfoSpecialTitleHotColor { get; set; }
	[Category("Header Info")] public Color HeaderInfoNormalTitleHotColor { get; set; }
	[Category("Header Info")] public ContentAlignment HeaderInfoSpecialAlignment { get; set; }
	[Category("Header Info")] public ContentAlignment HeaderInfoNormalAlignment { get; set; }
	[Category("Header Info")] public PaddingEx HeaderInfoSpecialPadding { get; set; }
	[Category("Header Info")] public PaddingEx HeaderInfoNormalPadding { get; set; }
	[Category("Header Info")] public Border HeaderInfoSpecialBorder { get; set; }
	[Category("Header Info")] public Border HeaderInfoNormalBorder { get; set; }
	[Category("Header Info")] public Color HeaderInfoSpecialBorderColor { get; set; }
	[Category("Header Info")] public Color HeaderInfoNormalBorderColor { get; set; }
	[Category("Header Info")] public Color HeaderInfoSpecialBackColor { get; set; }
	[Category("Header Info")] public Color HeaderInfoNormalBackColor { get; set; }
	[Category("Header Info")] public Image HeaderInfoSpecialArrowUp { get; set; }
	[Category("Header Info")] public Image HeaderInfoSpecialArrowUpHot { get; set; }
	[Category("Header Info")] public Image HeaderInfoSpecialArrowDown { get; set; }
	[Category("Header Info")] public Image HeaderInfoSpecialArrowDownHot { get; set; }
	[Category("Header Info")] public Image HeaderInfoNormalArrowUp { get; set; }
	[Category("Header Info")] public Image HeaderInfoNormalArrowUpHot { get; set; }
	[Category("Header Info")] public Image HeaderInfoNormalArrowDown { get; set; }
	[Category("Header Info")] public Image HeaderInfoNormalArrowDownHot { get; set; }
	[Category("Header Info")] public bool HeaderInfoTitleGradient { get; set; }
	[Category("Header Info")] public Color HeaderInfoSpecialGradientStartColor { get; set; }
	[Category("Header Info")] public Color HeaderInfoSpecialGradientEndColor { get; set; }
	[Category("Header Info")] public Color HeaderInfoNormalGradientStartColor { get; set; }
	[Category("Header Info")] public Color HeaderInfoNormalGradientEndColor { get; set; }
	[Category("Header Info")] public float HeaderInfoGradientOffset { get; set; }
	[Category("Header Info")] public int HeaderInfoTitleRadius { get; set; }
}
