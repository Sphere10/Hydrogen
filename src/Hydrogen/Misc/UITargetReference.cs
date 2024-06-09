// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT NON-AI software license, see the accompanying file
// LICENSE or visit https://sphere10.com/legal/NON-AI-MIT.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace Hydrogen;

/// <summary>
///  A cross-platform reference to a target, typically a UI object. On some platforms the target and it's owner
/// are needed, in others not. This object should be used in cross-platform code to referencing such targets.
/// </summary>
[XmlRoot]
[DataContract]
public class UITargetReference {

	public object Target { get; set; }

	public object TargetOwner { get; set; }

}
