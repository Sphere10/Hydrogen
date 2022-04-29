//-----------------------------------------------------------------------
// <copyright file="UICollectionViewExtensions.cs" company="Sphere 10 Software">
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
using CoreGraphics;
using UIKit;

namespace Hydrogen.iOS {

    public static class UICollectionViewExtensions {

        public static void DeselectAll(this UICollectionView collectionView, bool animated) {
            foreach (var item in collectionView.GetIndexPathsForSelectedItems()) {
                collectionView.DeselectItem(item, animated);
            }
        }

    }
}

