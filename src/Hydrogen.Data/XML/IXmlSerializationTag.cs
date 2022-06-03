//-----------------------------------------------------------------------
// <copyright file="IXmlSerializationTag.cs" company="Sphere 10 Software">
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

// -----------------------------------------------------------------------------------
// Use it as you please, but keep this header.
// Author : Marcus Deecke, 2006
// Web    : www.yaowi.com
// Email  : code@yaowi.com
// http://www.codeproject.com/KB/XML/deepserializer.aspx
// -----------------------------------------------------------------------------------

namespace Hydrogen.Data {
	public interface IXmlSerializationTag
  {
    string ASSEMBLY_TAG { get; }
    string INDEX_TAG { get; }
    string ITEM_TAG { get; }
    string ITEMS_TAG { get; }
    string NAME_ATT_KEY_TAG { get; }
    string NAME_ATT_VALUE_TAG { get; }
    string NAME_TAG { get; }
    string OBJECT_TAG { get; }
    string PROPERTIES_TAG { get; }
    string PROPERTY_TAG { get; }
    string TYPE_DICTIONARY_TAG { get; }
    string TYPE_TAG { get; }
    string GENERIC_TYPE_ARGUMENTS_TAG { get; }
    string CONSTRUCTOR_TAG { get;}
    string BINARY_DATA_TAG { get;}
  }
}
