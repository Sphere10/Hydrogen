// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Dev Age
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;
using System.IO;
using System.IO.IsolatedStorage;

namespace DevAge.IO.IsolatedStorage;

/// <summary>
/// Abstract class that help to save settings in the isolated streams
/// </summary>
public abstract class IsolatedStorageSettingBase {
	private string m_FileName;

	public virtual string StorageFileName {
		get { return m_FileName; }
		set { m_FileName = value; }
	}

	public IsolatedStorageSettingBase() {
		m_FileName = null;
	}

	protected virtual IsolatedStorageFile GetStorage() {
		return IsolatedStorageFile.GetUserStoreForAssembly();
	}

	public virtual void Load() {
		if (m_FileName == null)
			throw new ApplicationException("Invalid filename");

		//carico le impostazioni
		using (IsolatedStorageFile l_Storage = GetStorage()) {
			IsolatedStorageFileStream l_File = null;
			try {
				l_File = new IsolatedStorageFileStream(StorageFileName, FileMode.Open, FileAccess.Read, l_Storage);
			} catch (FileNotFoundException) {
				l_File = null;
			}

			if (l_File == null) //file non esiste
			{
				OnCreate();
			} else //file esiste
			{
				try {
					OnLoad(l_File);
				} finally {
					l_File.Close();
				}
			}

			l_Storage.Close();
		}
	}

	public virtual void Reset() {
		if (m_FileName == null)
			throw new ApplicationException("Invalid filename");

		using (IsolatedStorageFile l_Storage = GetStorage()) {
			try {
				l_Storage.DeleteFile(StorageFileName);
			} catch (Exception) {
			} finally {
				OnCreate();
			}

			l_Storage.Close();
		}
	}

	public virtual void Save() {
		if (m_FileName == null)
			throw new ApplicationException("Invalid filename");

		using (IsolatedStorageFile l_Storage = GetStorage()) {
			using (IsolatedStorageFileStream l_File = new IsolatedStorageFileStream(StorageFileName, FileMode.Create, FileAccess.Write, l_Storage)) {
				OnSave(l_File);

				l_File.Close();
			}

			l_Storage.Close();
		}
	}

	protected abstract void OnCreate();

	protected abstract void OnLoad(IsolatedStorageFileStream p_File);

	protected abstract void OnSave(IsolatedStorageFileStream p_File);
}
