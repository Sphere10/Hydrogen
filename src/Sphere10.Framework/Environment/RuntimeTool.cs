//-----------------------------------------------------------------------
// <copyright file="RuntimeTool.cs" company="Sphere 10 Software">
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
using System.Diagnostics;
using System.Reflection;
using Sphere10.Framework;

// ReSharper disable CheckNamespace
namespace Tools {

    public static class Runtime {
        private static readonly object _threadLock = new object();
        private static bool _hasDeterminedDesignMode = false;
        private static bool _isDesignMode = false;
        private static Assembly _entryAssembly = null;
        private static bool? _isWebApp = null;

        public static bool BreakCondition { get; set; }
        public static string GetExecutablePath() {
            // https://stackoverflow.com/questions/64581054/how-do-i-get-the-name-of-the-current-executable-in-c-net-5-edition
            return Process.GetCurrentProcess().MainModule.FileName;
        }

        public static bool IsDebugBuild {
            get {
#if DEBUG
                return true;
#else
                return false;
#endif
            }
        }

        public static bool IsReleaseBuild {
            get {
#if RELEASE
                return true;
#else
                return false;
#endif
            }
        }


        public static bool IsWebApp {
            get {
#warning IsWebApp needs testing for NET_CORE and NET STANDARD apps
                return false;

                //#if __MOBILE__
                //            return false;
                //#else
                //                if (_isWebApp == null) {
                //                    lock (_threadLock) {
                //                        if (_isWebApp == null) {
                //                            _isWebApp = Path.GetFileName(AppDomain.CurrentDomain.SetupInformation.ConfigurationFile).EndsWith("web.config", true, CultureInfo.InvariantCulture);
                //                        }
                //                    }
                //                }
                //#endif
                //                return _isWebApp.Value;
            }
        }

        public static bool IsDesignMode {
            get {
                if (!_hasDeterminedDesignMode) {
                    lock (_threadLock) {
                        if (!_hasDeterminedDesignMode) {
                            var processName = Process.GetCurrentProcess().ProcessName.ToUpperInvariant();
                            _isDesignMode = processName.IsIn("DEVENV", "SHARPDEVELOP");
                            _hasDeterminedDesignMode = true;
                        }
                    }
                }
                return _isDesignMode;
            }
        }

        public static Assembly GetEntryAssembly() {
            if (_entryAssembly == null) {
                lock (_threadLock) {
                    if (_entryAssembly == null) {
                        _entryAssembly = IsWebApp ? GetWebEntryAssembly() : Assembly.GetEntryAssembly();
                        if (_entryAssembly == null)
                            throw new SoftwareException("Unable to determine entry assembly");
                    }
                }
            }
            return _entryAssembly;
        }

        private static Assembly GetWebEntryAssembly() {
            throw new NotImplementedException();
            ////return Assembly.GetExecutingAssembly();
            //var httpContextType = TypeResolver.Resolve("System.Web.HttpContext");
            //var httpContextCurrent = httpContextType.GetProperty("Current").FastGetValue(null);
            ////var httpContextCurrentHandler = httpContextCurrent.GetType().GetProperty("Handler").FastGetValue(httpContextCurrent);
            //var httpContextCurrentApplicationInstance = httpContextCurrent.GetType().GetProperty("ApplicationInstance").FastGetValue(httpContextCurrent);
            //return httpContextCurrentApplicationInstance.GetType().BaseType.Assembly;
            ////if ((System.Web.HttpContext.Current == null) || (System.Web.HttpContext.Current.Handler == null))
            ////    return Tools.Runtime.GetEntryAssembly(); // Not a web application
            ////return System.Web.HttpContext.Current.Handler.GetType().BaseType.Assembly;
        }

    }
}




