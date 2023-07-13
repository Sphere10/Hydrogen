// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Dev Age
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;
using System.IO;

namespace DevAge.Runtime.Serialization;

/// <summary>
/// Static Class for serialization utilities
/// </summary>
public static class Utilities {

	#region Serialization Code

	/// <summary>
	/// Deserialize the stream. Using BinaryFormatter.
	/// </summary>
	/// <param name="p_Stream"></param>
	/// <returns></returns>
	public static object BinDeserialize(Stream p_Stream) {
		// BinaryFormattter Deprecated in .NET 7
		// TODO use GenericSerializer from Hydrogen, test for Stream's
		throw new NotImplementedException("BinaryFormattter Deprecated in .NET 7");
		//BinaryFormatter f = new BinaryFormatter();
		//object tmp;
		//tmp = f.Deserialize(p_Stream);
		//return tmp;
	}

	/// <summary>
	/// Serialize the stream. Using BinaryFormatter.
	/// </summary>
	/// <param name="p_Stream"></param>
	/// <param name="p_Object"></param>
	public static void BinSerialize(Stream p_Stream, object p_Object) {
		// BinaryFormattter Deprecated in .NET 7
		// TODO use GenericSerializer from Hydrogen, test for Stream's
		throw new NotImplementedException("BinaryFormattter Deprecated in .NET 7");
		//BinaryFormatter f = new BinaryFormatter();
		//f.Serialize(p_Stream,p_Object);
	}

	/// <summary>
	/// Deserialize the specified file. Using BinaryFormatter.
	/// </summary>
	/// <param name="p_strFileName"></param>
	/// <returns></returns>
	public static object BinDeserialize(string p_strFileName) {
		object tmp;
		using (FileStream l_Stream = new FileStream(p_strFileName, FileMode.Open, FileAccess.Read)) {
			tmp = BinDeserialize(l_Stream);
			l_Stream.Close();
		}
		return tmp;
	}

	/// <summary>
	/// Serialize the object to the specified file. Using BinaryFormatter.
	/// </summary>
	/// <param name="p_strFileName"></param>
	/// <param name="p_Object"></param>
	public static void BinSerialize(string p_strFileName, object p_Object) {
		using (FileStream l_Stream = new FileStream(p_strFileName, FileMode.Create, FileAccess.Write)) {
			BinSerialize(l_Stream, p_Object);
			l_Stream.Close();
		}
	}

	#endregion

}
