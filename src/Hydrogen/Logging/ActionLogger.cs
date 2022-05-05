//-----------------------------------------------------------------------
// <copyright file="TextWriterLogger.cs" company="Sphere 10 Software">
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

namespace Hydrogen {

	public class ActionLogger : LoggerBase {

		private readonly Action<string> _debugAction;
		private readonly Action<string> _infoAction;
		private readonly Action<string> _warningAction;
		private readonly Action<string> _errorAction;

		/// <summary>
		/// 
		/// </summary>
		/// <param name="action"></param>
	    public ActionLogger(Action<string> action) 
			: this(action, action, action, action) {
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="debugAction"></param>
		/// <param name="infoAction"></param>
		/// <param name="warningAction"></param>
		/// <param name="errorAction"></param>
		public ActionLogger(Action<string> debugAction, Action<string> infoAction, Action<string> warningAction, Action<string> errorAction) {
			_debugAction = debugAction;
			_infoAction = infoAction;
			_warningAction = warningAction;
			_errorAction = errorAction;
			this.Options = LogOptions.DebugBuildDefaults;
		}


	    protected override void Log(LogLevel level, string message) {
			try {
                switch (level) {
                    case LogLevel.Debug:
						_debugAction(message);
                        break;
                    case LogLevel.Info:
						_infoAction(message);
                        break;
                    case LogLevel.Warning:
						_warningAction(message);
                        break;
                    case LogLevel.Error:
						_errorAction(message);
                        break;
                }
            } catch {
				// errors do not propagate outside logging framework
			}
        }

    }
}
