//-----------------------------------------------------------------------
// <copyright file="EnumExtensions.cs" company="Sphere 10 Software">
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
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Hydrogen.Web.AspNetCore {

    public static class EnumExtensions {

        public static SelectList ToSelectList<TEnum>(this TEnum enumObj)
              where TEnum : struct, IComparable, IFormattable, IConvertible {
            return Tools.AspNetCore.ToSelectList<TEnum>(enumObj);
        }

	}
}
