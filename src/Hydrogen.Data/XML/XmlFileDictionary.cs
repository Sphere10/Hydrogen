// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;
using System.Collections.Generic;
using System.IO;
using System.Diagnostics;

namespace Hydrogen.Data;

public class XmlFileDictionary<T1, T2> : DictionaryDecorator<T1, T2>, IPersistedDictionary<T1, T2> {
	private readonly bool _useSimpleXmlSerialization;

	public XmlFileDictionary(bool simpleSerialization = false)
		: this(new Dictionary<T1, T2>(), simpleSerialization) {
	}

	public XmlFileDictionary(IDictionary<T1, T2> internalDictionary, bool simpleSerialization = false)
		: base(internalDictionary) {
		Filename = string.Empty;
		_useSimpleXmlSerialization = simpleSerialization;
	}

	public XmlFileDictionary(string filename, bool simpleSerialization = false)
		: this(filename, new Dictionary<T1, T2>(), simpleSerialization) {
	}

	public XmlFileDictionary(string filename, IDictionary<T1, T2> internalDictionary, bool simpleSerialization = false)
		: base(internalDictionary) {
		Debug.Assert(filename != null);
		_useSimpleXmlSerialization = simpleSerialization;
		Filename = filename;
		Load();
	}

	public string Directory => Path.GetDirectoryName(Filename);

	public string Filename { get; protected set; }

	public virtual void CreateFileForFirstTime() {
		try {
			Tools.FileSystem.CreateBlankFile(Filename, true);
		} catch (Exception error) {
			throw new SoftwareException(
				error,
				"Failed to create FileDictionary for first time '{0}'. Possible security access violation",
				Filename
			);
		}
	}

	public virtual void Load() {

		#region Pre-conditions

		Debug.Assert(Filename != null);

		#endregion

		try {
			if (File.Exists(Filename)) {
				var surrogate = _useSimpleXmlSerialization ? Tools.Xml.ReadFromFile<SerializableDictionarySurrogate<T1, T2>>(Filename) : Tools.Xml.DeepReadFromFile<SerializableDictionarySurrogate<T1, T2>>(Filename);
				surrogate.ToDictionary(this);
			}
		} catch (Exception error) {

			#region Attempt to recreate file

			try {
				File.Delete(Filename);
				CreateFileForFirstTime();
				Save();
				if (File.Exists(Filename)) {
					var surrogate = _useSimpleXmlSerialization ? Tools.Xml.ReadFromFile<SerializableDictionarySurrogate<T1, T2>>(Filename) : Tools.Xml.DeepReadFromFile<SerializableDictionarySurrogate<T1, T2>>(Filename);
					if (surrogate != null) {
						surrogate.ToDictionary(this);
					}
				}
			} catch (Exception innerError) {
				throw new SoftwareException(
					innerError,
					"Failed to load and recreate FileDictionary '{0}'.",
					Filename
				);
			}

			#endregion

		}
	}

	public virtual void Save() {
		try {
			if (!File.Exists(Filename)) {
				CreateFileForFirstTime();
			}
			if (_useSimpleXmlSerialization)
				Tools.Xml.WriteToFile(Filename, new SerializableDictionarySurrogate<T1, T2>(this));
			else
				Tools.Xml.DeepWriteToFile(Filename, new SerializableDictionarySurrogate<T1, T2>(this));
		} catch (Exception error) {
			throw new SoftwareException(
				error,
				"Failed to write FileDictionary '{0}'.",
				Filename
			);
		}
	}

	public void Delete() {
		if (File.Exists(Filename))
			File.Delete(Filename);
	}
}
