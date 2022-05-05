//-----------------------------------------------------------------------
// <copyright file="UITargetReference.cs" company="Sphere 10 Software">
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

using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace Hydrogen {

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
}
