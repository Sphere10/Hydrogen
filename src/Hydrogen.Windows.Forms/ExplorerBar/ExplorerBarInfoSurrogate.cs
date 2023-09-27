// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using System.Reflection;

namespace Hydrogen.Windows.Forms;

/// <summary>
/// A class that is serialized instead of an ExplorerBarInfo (as 
/// ExplorerBarInfos contain objects that cause serialization problems)
/// </summary>
[Obfuscation(Exclude = true)]
[Serializable()]
public class ExplorerBarInfoSurrogate : ISerializable {

	#region Class Data

	/// <summary>
	/// This member is not intended to be used directly from your code.
	/// </summary>
	public TaskPaneInfoSurrogate TaskPaneInfoSurrogate;

	/// <summary>
	/// This member is not intended to be used directly from your code.
	/// </summary>
	public TaskItemInfoSurrogate TaskItemInfoSurrogate;

	/// <summary>
	/// This member is not intended to be used directly from your code.
	/// </summary>
	public ExpandoInfoSurrogate ExpandoInfoSurrogate;

	/// <summary>
	/// This member is not intended to be used directly from your code.
	/// </summary>
	public HeaderInfoSurrogate HeaderInfoSurrogate;

	/// <summary>
	/// Version number of the surrogate.  This member is not intended 
	/// to be used directly from your code.
	/// </summary>
	public int Version = 3300;

	#endregion


	#region Constructor

	/// <summary>
	/// Initializes a new instance of the ExplorerBarInfoSurrogate class with default settings
	/// </summary>
	public ExplorerBarInfoSurrogate() {
		this.TaskPaneInfoSurrogate = null;
		this.TaskItemInfoSurrogate = null;
		this.ExpandoInfoSurrogate = null;
		this.HeaderInfoSurrogate = null;
	}

	#endregion


	#region Methods

	/// <summary>
	/// Populates the ExplorerBarInfoSurrogate with data that is to be 
	/// serialized from the specified ExplorerBarInfo
	/// </summary>
	/// <param name="explorerBarInfo">The ExplorerBarInfo that contains the data 
	/// to be serialized</param>
	public void Load(ExplorerBarInfo explorerBarInfo) {
		this.TaskPaneInfoSurrogate = new TaskPaneInfoSurrogate();
		this.TaskPaneInfoSurrogate.Load(explorerBarInfo.TaskPane);

		this.TaskItemInfoSurrogate = new TaskItemInfoSurrogate();
		this.TaskItemInfoSurrogate.Load(explorerBarInfo.TaskItem);

		this.ExpandoInfoSurrogate = new ExpandoInfoSurrogate();
		this.ExpandoInfoSurrogate.Load(explorerBarInfo.Expando);

		this.HeaderInfoSurrogate = new HeaderInfoSurrogate();
		this.HeaderInfoSurrogate.Load(explorerBarInfo.Header);
	}


	/// <summary>
	/// Returns an ExplorerBarInfo that contains the deserialized ExplorerBarInfoSurrogate data
	/// </summary>
	/// <returns>An ExplorerBarInfo that contains the deserialized ExplorerBarInfoSurrogate data</returns>
	public ExplorerBarInfo Save() {
		ExplorerBarInfo explorerBarInfo = new ExplorerBarInfo();

		explorerBarInfo.TaskPane = this.TaskPaneInfoSurrogate.Save();
		explorerBarInfo.TaskItem = this.TaskItemInfoSurrogate.Save();
		explorerBarInfo.Expando = this.ExpandoInfoSurrogate.Save();
		explorerBarInfo.Header = this.HeaderInfoSurrogate.Save();

		return explorerBarInfo;
	}


	/// <summary>
	/// Populates a SerializationInfo with the data needed to serialize the ExplorerBarInfoSurrogate
	/// </summary>
	/// <param name="info">The SerializationInfo to populate with data</param>
	/// <param name="context">The destination for this serialization</param>
	[SecurityPermissionAttribute(SecurityAction.Demand, SerializationFormatter = true)]
	public void GetObjectData(SerializationInfo info, StreamingContext context) {
		info.AddValue("Version", this.Version);

		info.AddValue("TaskPaneInfoSurrogate", this.TaskPaneInfoSurrogate);
		info.AddValue("TaskItemInfoSurrogate", this.TaskItemInfoSurrogate);
		info.AddValue("ExpandoInfoSurrogate", this.ExpandoInfoSurrogate);
		info.AddValue("HeaderInfoSurrogate", this.HeaderInfoSurrogate);
	}


	/// <summary>
	/// Initializes a new instance of the ExplorerBarInfoSurrogate class using the information 
	/// in the SerializationInfo
	/// </summary>
	/// <param name="info">The information to populate the ExplorerBarInfoSurrogate</param>
	/// <param name="context">The source from which the ExplorerBarInfoSurrogate is deserialized</param>
	[SecurityPermissionAttribute(SecurityAction.Demand, SerializationFormatter = true)]
	protected ExplorerBarInfoSurrogate(SerializationInfo info, StreamingContext context)
		: base() {
		int version = info.GetInt32("Version");

		this.TaskPaneInfoSurrogate = (TaskPaneInfoSurrogate)info.GetValue("TaskPaneInfoSurrogate", typeof(TaskPaneInfoSurrogate));
		this.TaskItemInfoSurrogate = (TaskItemInfoSurrogate)info.GetValue("TaskItemInfoSurrogate", typeof(TaskItemInfoSurrogate));
		this.ExpandoInfoSurrogate = (ExpandoInfoSurrogate)info.GetValue("ExpandoInfoSurrogate", typeof(ExpandoInfoSurrogate));
		this.HeaderInfoSurrogate = (HeaderInfoSurrogate)info.GetValue("HeaderInfoSurrogate", typeof(HeaderInfoSurrogate));
	}

	#endregion

}
