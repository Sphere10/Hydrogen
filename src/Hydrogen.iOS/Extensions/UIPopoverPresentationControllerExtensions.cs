//-----------------------------------------------------------------------
// <copyright file="UIPopoverPresentationControllerExtensions.cs" company="Sphere 10 Software">
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
using Hydrogen;
using Foundation;
using System.Linq;
using System.Collections.Generic;
using Hydrogen.iOS;

namespace Hydrogen.iOS {
    public static class UIPopoverPresentationControllerExtensions {

        public static void SetPresentationAnchor(this UIPopoverPresentationController ppc, object anchorObject) {
            if (ppc == null)
                throw new ArgumentNullException("ppc");

            if (anchorObject == null)
                throw new ArgumentNullException("anchorObject");

            if (!(anchorObject is NSObject) && !(anchorObject is UITargetReference))
                throw new ArgumentException("Not NSObject or UITargetReference", "anchorObject");

            if (anchorObject is UITargetReference) {
                var target = (UITargetReference)anchorObject;
                SetPresentationAnchorInternal(ppc, (NSObject)target.Target, (NSObject)target.TargetOwner);
            } else {
                SetPresentationAnchorInternal(ppc, (NSObject)anchorObject, null);
            }
        }


        private static void SetPresentationAnchorInternal(UIPopoverPresentationController ppc, NSObject target, NSObject targetOwner) {
            TypeSwitch.Do(target,
                TypeSwitch.Case<UIToolbar>(b => {
                    ppc.SourceView = b;
                    ppc.SourceRect = b.Bounds;
                    ppc.PermittedArrowDirections = UIPopoverArrowDirection.Any;
                }),

                TypeSwitch.Case<UITabBar>(b => {
                    ppc.SourceView = b;
                    ppc.SourceRect = b.Bounds;
                    ppc.PermittedArrowDirections = UIPopoverArrowDirection.Any;
                }),
                TypeSwitch.Case<UITableViewCell>(cvc => {
                    if (targetOwner == null || !(targetOwner is UITableView))
                        throw new ArgumentNullException("UITargetReference to UITableViewCell did not contain the UITableView in TargetOwner field ", "targetOwner");
                    ppc.SourceView = targetOwner as UITableView;
                    ppc.SourceRect = cvc.Frame;
                    ppc.PermittedArrowDirections = UIPopoverArrowDirection.Any;
                }),
                TypeSwitch.Case<UICollectionViewCell>(cvc => {
                    if (targetOwner == null || !(targetOwner is UICollectionView))
                        throw new ArgumentNullException("UITargetReference to UICollectionViewCell did not contain the UICollectionView in TargetOwner field ", "targetOwner");
                    ppc.SourceView = targetOwner as UICollectionView;
                    ppc.SourceRect = cvc.Frame;
                    ppc.PermittedArrowDirections = UIPopoverArrowDirection.Any;
                }),
                TypeSwitch.Case<UIView>(v => {
                    ppc.SourceView = v;
                    ppc.SourceRect = v.Bounds;
                    ppc.PermittedArrowDirections = UIPopoverArrowDirection.Any;
                }),
                TypeSwitch.Case<UIBarButtonItem>((Action<UIBarButtonItem>)(b => {
                    ppc.BarButtonItem = b;
                    ppc.PermittedArrowDirections = UIPopoverArrowDirection.Any;
                })),
                TypeSwitch.Default(() => { throw new NotSupportedException(target.GetType().ToString()); })
            );
        }
    }
}

