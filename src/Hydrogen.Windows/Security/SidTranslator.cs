// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System.Collections.Generic;

namespace Hydrogen.Windows.Security;

/// <summary>
/// Translates remote account names into a local equivalent
/// </summary>
public class SidTranslator {
	private IDictionary<string, string> _translations;
	private bool _importObject;
	private IActionObserver _actionObserver;
	private string _defaultPassword;

	public SidTranslator()
		: this(false, string.Empty) {
	}

	public SidTranslator(bool automaticallyImport, string defaultPassword)
		: this(automaticallyImport, defaultPassword, SecurityTool.DefaultActionObserver) {
	}


	public SidTranslator(bool automaticallyImport, string defaultPassword, IActionObserver actionObserver) {
		_importObject = automaticallyImport;
		_actionObserver = actionObserver;
		_defaultPassword = defaultPassword;
		_translations = new Dictionary<string, string>();
	}

	public IActionObserver ActionObserver {
		get { return _actionObserver; }
		set { _actionObserver = value; }
	}

	public bool TryTranslate(
		string remoteHost,
		string remoteName,
		WinAPI.ADVAPI32.SidNameUse remoteNameUse,
		out string translatedAccountName
	) {
		ActionObserver.NotifyAction("Translating", remoteNameUse.ToString(), remoteName, string.Empty);

		bool retval = false;
		translatedAccountName = string.Empty;
		string key = string.Format("{0}\\{1}", remoteHost, remoteName);
		if (_translations.ContainsKey(key)) {
			translatedAccountName = _translations[key];
			retval = true;
		} else {
			// attempt to resolve with local user/group of same name
			if (AccountExistsLocally(remoteName)) {
				_translations[key] = remoteName;
				translatedAccountName = _translations[key];
				retval = true;
				ActionObserver.NotifyInformation("Translated remote account '{0}\\{1}' to already existing local account '{2}'", remoteHost, remoteName, translatedAccountName);
			} else if (_importObject) {

				#region Import remote object

				NTHost host = new NTHost(remoteHost);
				NTLocalObject obj;
				if (host.TryGetLocalObject(remoteName, out obj)) {
					if (obj is NTLocalUser) {
						NTLocalUser remoteUser = (NTLocalUser)obj;
						UserCopier userCopier = new UserCopier(
							true,
							false,
							_defaultPassword,
							false,
							ActionObserver
						);
						NTLocalUser localUser =
							userCopier.CopyRemoteUserToLocalMachine(remoteUser);
						translatedAccountName = localUser.Name;

						ActionObserver.NotifyInformation("Copied and translated remote user '{0}\\{1}' to local group '{2}'", remoteHost, remoteName, translatedAccountName);

					} else if (obj is NTLocalGroup) {

						NTLocalGroup remoteGroup = (NTLocalGroup)obj;
						GroupCopier groupCopier = new GroupCopier(
							true,
							false,
							true,
							_defaultPassword,
							false,
							ActionObserver
						);

						NTLocalGroup localGroup =
							groupCopier.CopyRemoteGroupToLocalMachine(remoteGroup);
						translatedAccountName = remoteGroup.Name;

						ActionObserver.NotifyInformation("Copied and translated remote group '{0}\\{1}' to local group '{2}'", remoteHost, remoteName, translatedAccountName);
					}
				}

				#endregion

				_translations[key] = translatedAccountName;
				retval = true;
			}
		}

		if (!retval) {
			ActionObserver.NotifyWarning("Failed to translate '{0}\\{1}' into a local object.", remoteHost, remoteName);
		}
		return retval;
	}

	public static bool AccountExistsLocally(string name) {
		NTLocalObject obj;
		return NTHost.CurrentMachine.TryGetLocalObject(name, out obj);
	}
}
