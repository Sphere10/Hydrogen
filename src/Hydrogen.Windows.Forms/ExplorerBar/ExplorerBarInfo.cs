/*
 * Copyright © 2004-2005, Mathew Hall
 * All rights reserved.
 *
 * Redistribution and use in source and binary forms, with or without modification, 
 * are permitted provided that the following conditions are met:
 *
 *    - Redistributions of source code must retain the above copyright notice, 
 *      this list of conditions and the following disclaimer.
 * 
 *    - Redistributions in binary form must reproduce the above copyright notice, 
 *      this list of conditions and the following disclaimer in the documentation 
 *      and/or other materials provided with the distribution.
 *
 * THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND 
 * ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED 
 * WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE DISCLAIMED. 
 * IN NO EVENT SHALL THE COPYRIGHT OWNER OR CONTRIBUTORS BE LIABLE FOR ANY DIRECT, 
 * INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT 
 * NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, 
 * OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, 
 * WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) 
 * ARISING IN ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY 
 * OF SUCH DAMAGE.
 */


using System;

namespace Hydrogen.Windows.Forms;

/// <summary>
/// A class that contains system defined settings for an XPExplorerBar
/// </summary>
public class ExplorerBarInfo : IDisposable {
	private static ExplorerBarInfo _default = null;

	static ExplorerBarInfo() {
		//if (Tools.DesignModeTool.IsDesignMode) {
		//    XmlSerializer xmlSerializer = new XmlSerializer(typeof(ExplorerBarInfoSurrogate));
		//    var surrogate = (ExplorerBarInfoSurrogate)xmlSerializer.Deserialize(new System.IO.StringReader(Resources.LunaExplorerBarTheme));
		//    _default = surrogate.Save();
		//} else {
		//    _default =
		//        XmlProvider
		//        .ReadFromString<ExplorerBarInfoSurrogate>(Resources.LunaExplorerBarTheme)
		//        .Save();
		//}
		_default = Tools.Xml.ReadFromString<ExplorerBarInfoSurrogate>(Resources.LunaExplorerBarTheme).Save();
	}

	public static ExplorerBarInfo Default {
		get {
			var cloner = new WinFormsCompatibleDeepObjectCloner();
			return (ExplorerBarInfo)cloner.Clone(_default);
		}
	}

	/// <summary>
	/// System defined settings for a TaskPane
	/// </summary>
	private TaskPaneInfo taskPane;

	/// <summary>
	/// System defined settings for a TaskItem
	/// </summary>
	private TaskItemInfo taskItem;

	/// <summary>
	/// System defined settings for an Expando
	/// </summary>
	private ExpandoInfo expando;

	/// <summary>
	/// System defined settings for an Expando's header
	/// </summary>
	private HeaderInfo header;


	/// <summary>
	/// Initializes a new instance of the ExplorerBarInfo class with 
	/// default settings
	/// </summary>
	public ExplorerBarInfo() {
		this.taskPane = new TaskPaneInfo();
		this.taskItem = new TaskItemInfo();
		this.expando = new ExpandoInfo();
		this.header = new HeaderInfo();
	}

	/// <summary>
	/// Sets the arrow images for use when theming is not supported
	/// </summary>
	public void SetUnthemedArrowImages() {
		this.Header.SetUnthemedArrowImages();
	}


	/// <summary>
	/// Force use of default values
	/// </summary>
	public void UseClassicTheme() {

		this.TaskPane.SetDefaultValues();
		this.Expando.SetDefaultValues();
		this.Header.SetDefaultValues();
		this.TaskItem.SetDefaultValues();

		this.SetUnthemedArrowImages();
	}


	/// <summary>
	/// Releases all resources used by the ExplorerBarInfo
	/// </summary>
	public void Dispose() {
		this.taskPane.Dispose();
		this.header.Dispose();
		this.expando.Dispose();
	}

	/// <summary>
	/// Gets the ExplorerPane settings
	/// </summary>
	public TaskPaneInfo TaskPane {
		get { return this.taskPane; }

		set { this.taskPane = value; }
	}

	/// <summary>
	/// Gets the TaskLink settings
	/// </summary>
	public TaskItemInfo TaskItem {
		get { return this.taskItem; }

		set { this.taskItem = value; }
	}

	/// <summary>
	/// Gets the Group settings
	/// </summary>
	public ExpandoInfo Expando {
		get { return this.expando; }

		set { this.expando = value; }
	}

	/// <summary>
	/// Gets the Header settings
	/// </summary>
	public HeaderInfo Header {
		get { return this.header; }

		set { this.header = value; }
	}


}
