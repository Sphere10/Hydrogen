//-----------------------------------------------------------------------
// <copyright file="iOSAPI.cs" company="Sphere 10 Software">
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
using System.Runtime.InteropServices;
using System.Collections.Generic;
using Foundation;
using ObjCRuntime;


namespace Hydrogen.iOS {
    public static class iOSAPI {
        [DllImport(Constants.ObjectiveCLibrary)]
        public static extern void objc_setAssociatedObject(IntPtr @object, IntPtr key, IntPtr value, AssociationPolicy policy);

        [DllImport(Constants.ObjectiveCLibrary)]
        public static extern IntPtr objc_getAssociatedObject(IntPtr @object, IntPtr key);

        [DllImport(Constants.ObjectiveCLibrary, EntryPoint = "objc_msgSend")]
        public static extern void void_objc_msgSend_int(IntPtr deviceHandle, IntPtr setterHandle, nint val);

        [DllImport(Constants.ObjectiveCLibrary, EntryPoint = "objc_msgSend")]
        public static extern void void_objc_msgSend_float(IntPtr deviceHandle, IntPtr setterHandle, nfloat val);

        [DllImport(Constants.ObjectiveCLibrary, EntryPoint = "objc_msgSend")]
        public static extern IntPtr intptr_objc_msgSend(IntPtr tokenHandle, IntPtr selectorHandle);
    }
}

