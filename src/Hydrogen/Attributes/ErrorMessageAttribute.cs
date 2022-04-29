//-----------------------------------------------------------------------
// <copyright file="ErrorMessageAttribute.cs" company="Sphere 10 Software">
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
using System.Text;
using System.Reflection;

namespace Hydrogen {
	public class ErrorMessageAttribute  : Attribute {
        private string _errMsg;

         public ErrorMessageAttribute(string errMsg) {
            ErrorMessage = errMsg;
        }

         public string ErrorMessage {
             get { return _errMsg; }
             set { _errMsg = value; }
         }

         public static bool HasErrorMessage(Enum enumeration) {
             return enumeration.GetType().GetCustomAttributes(typeof(ErrorMessageAttribute), true).Length > 0;
        }

        public static string GetErrorMessage(Enum enumeration) {
            StringBuilder retval = new StringBuilder();
            FieldInfo enumDecl = enumeration.GetType().GetField(enumeration.ToString());
            if (enumDecl != null) {
                object[] attrs = enumDecl.GetCustomAttributes(typeof(ErrorMessageAttribute), true);
                for (int i = 0; i < attrs.Length; i++) {
                    if (i > 0) {
                        retval.Append(Environment.NewLine);
                    }
                    retval.Append(((ErrorMessageAttribute)attrs[i]).ErrorMessage);
                }
            }
            return retval.ToString();
        }

    }
}
