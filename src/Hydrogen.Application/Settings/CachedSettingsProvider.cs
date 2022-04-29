//-----------------------------------------------------------------------
// <copyright file="ISettingsProvider.cs" company="Sphere 10 Software">
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

using System;

namespace Hydrogen.Application {
    public class CachedSettingsProvider : SettingsProviderDecorator {
        SynchronizedDictionary<(Type, object), SettingsObject> _cache;
       
        public CachedSettingsProvider(ISettingsProvider internalSettingsProvider)
            : base(internalSettingsProvider) {
            _cache = new SynchronizedDictionary<(Type, object), SettingsObject>();
        }

        public override void ClearSettings() {
            base.ClearSettings();
            _cache.Clear();
        }

        public override bool ContainsSetting(Type settingsObjectType, object id = null) {
            var key = (settingsObjectType, id);
            if (_cache.ContainsKey(key)) {
                return true;
            }
            return base.ContainsSetting(settingsObjectType, id);
        }

        public override void DeleteSetting(SettingsObject setting) {
            var key = (setting.GetType(), setting.ID);
            _cache.Remove(key);
            base.DeleteSetting(setting);
        }

        public override SettingsObject LoadSetting(Type settingsObjectType, object id = null) {
            var key = (settingsObjectType, id);
            if (!_cache.TryGetValue(key, out var setting)) {
                setting = base.LoadSetting(settingsObjectType, id);
                _cache[key] = setting;
            }
            return setting;
        }

        public override SettingsObject NewSetting(Type settingsObjectType, object id = null) {
            var key = (settingsObjectType, id);
            var setting = base.NewSetting(settingsObjectType, id);
            _cache[key] = setting;
            return setting;
        }

        public override void SaveSetting(SettingsObject setting) {
            var key = (setting.GetType(), setting.ID);
            _cache[key] = setting;
            base.SaveSetting(setting);
        }


    }
}