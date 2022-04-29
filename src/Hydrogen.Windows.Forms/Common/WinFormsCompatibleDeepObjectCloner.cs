//-----------------------------------------------------------------------
// <copyright file="WinFormsCompatibleDeepObjectCloner.cs" company="Sphere 10 Software">
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
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sphere10.Framework;

namespace Sphere10.Framework.Windows.Forms {
    public class WinFormsCompatibleDeepObjectCloner : DeepObjectCloner {
        public WinFormsCompatibleDeepObjectCloner() {
            base.DontCloneTypes.AddRange(new[] { typeof(Font), typeof(Color)});
        }

        protected override object DeepClone(object source, IDictionary<Reference<object>, object> clones) {
            if (source is Bitmap) {
                return new Bitmap((Bitmap) source);
            }
            return base.DeepClone(source, clones);
        }

    }
}
