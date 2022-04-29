//-----------------------------------------------------------------------
// <copyright file="UIScrollViewExtensions.cs" company="Sphere 10 Software">
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
using System.Drawing;
using System.Linq;
using CoreGraphics;
using Foundation;
using Hydrogen;
using UIKit;

namespace Hydrogen.iOS {
    public static class UIScrollViewExtensions {

        /// <summary>
        /// Adds the view into the scroll view that is auto-zoomed, best-fit, orientable and re-entrant.
        /// </summary>
        /// <param name="scrollView">Scroll view.</param>
        /// <param name="contentView">Content view.</param>
        public static void SetZoomableContentView(this UIScrollView scrollView, UIView contentView) {

            // Remove fixer if has one
            if (Tools.iOSTool.HasRuntimeProperty(scrollView, "ScrollViewFixer")) {
                scrollView.UnsetZoomableContentView();
            }

            scrollView.RemoveAndDisposeChildSubViews();
            var scrollViewFixer = new ScrollViewObserver(contentView);
            scrollView.AddObserver(
                observer: scrollViewFixer,
                keyPath: new NSString("bounds"),
                options: NSKeyValueObservingOptions.OldNew,
                context: IntPtr.Zero
            );

            if (scrollView.Frame != RectangleF.Empty) {
                ScrollViewObserver.ConfigureScrollView(scrollView, contentView, Guid.Empty);
            }

            Tools.iOSTool.AddRuntimeProperty(scrollView, scrollViewFixer, "ScrollViewFixer", AssociationPolicy.RETAIN_NONATOMIC);
        }

        public static void UnsetZoomableContentView(this UIScrollView scrollView) {
            NSObject fixer;
            if (Tools.iOSTool.TryGetRuntimeProperty(scrollView, "ScrollViewFixer", out fixer)) {
                var scrollViewFixer = (ScrollViewObserver)fixer;
                scrollView.RemoveObserver(
                    observer: scrollViewFixer,
                    keyPath: new NSString("bounds")
                );
                Tools.iOSTool.RemoveRuntimeProperty(scrollView, "ScrollViewFixer");
                scrollViewFixer.Dispose();
            }
        }

        public static void SetTouchesForScrollGesture(this UIScrollView scrollView, uint numTouches) {
            foreach (var panGestureRecognizer in scrollView.GestureRecognizers.Where(gr => gr is UIPanGestureRecognizer).Cast<UIPanGestureRecognizer>())
                panGestureRecognizer.MinimumNumberOfTouches = numTouches;
        }





    }

    [Register("ScrollViewObserver")]
    public class ScrollViewObserver : NSObject {
        private Guid ID = Guid.NewGuid();
        private WeakReference<UIView> _contentView;

        public ScrollViewObserver(IntPtr x)
            : base(x) {
            var xxx = 1;
        }

        public ScrollViewObserver(UIView contentView) {
            _contentView = new WeakReference<UIView>(contentView);
        }

        protected override void Dispose(bool disposing) {
            if (disposing) {
            }
            base.Dispose(disposing);
        }


        public override void ObserveValue(NSString keyPath, NSObject ofObject, NSDictionary change, IntPtr context) {
            if (!change.ContainsKey("old".ToNSString()))
                return;

            if (!change.ContainsKey("new".ToNSString()))
                return;

            var oldRect = ((NSValue)change.ValueForKey("old".ToNSString())).CGRectValue;
            var newRect = ((NSValue)change.ValueForKey("new".ToNSString())).CGRectValue;

            if (oldRect.Size == newRect.Size)
                return; // zoom/scroll event

            var scrollView = ofObject as UIScrollView;
            if (scrollView == null)
                return;

            if (scrollView.Handle == IntPtr.Zero)
                return;

            UIView contentView;
            if (_contentView.TryGetTarget(out contentView)) {
                if (contentView.Handle == IntPtr.Zero) {
                    SystemLog.Error("Content view was invalid");
                    return;
                }
                if (scrollView.Subviews.Length == 0) {
                    ConfigureScrollView(scrollView, contentView, ID);
                    //ClearContentViewReference();
                } else {
                    AdjustScrollView(scrollView, contentView, ID);
                }
            }
        }

        public static void ConfigureScrollView(UIScrollView scrollView, UIView contentView, Guid fixerID) {
            contentView.LayoutIfNeeded();
            scrollView.RemoveAndDisposeChildSubViews();
            scrollView.TranslatesAutoresizingMaskIntoConstraints = false;
            contentView.TranslatesAutoresizingMaskIntoConstraints = false;
            scrollView.AddSubviewDockFull(contentView);
            scrollView.ViewForZoomingInScrollView = (x) => contentView;
            AdjustScrollView(scrollView, contentView, fixerID);
        }

        public static void AdjustScrollView(UIScrollView scrollView, UIView contentView, Guid fixerID) {
            var scrollViewFrameSize = scrollView.Frame.Size;
            var scaleWidth = scrollViewFrameSize.Width / contentView.IntrinsicContentSize.Width;
            var scaleHeight = scrollViewFrameSize.Height / contentView.IntrinsicContentSize.Height;
            // Unnecessary, if uncommenting note that these get added every orientation change, which is a bug
           /* scrollView.AddConstraints(new[] {
				NSLayoutConstraint.Create(scrollView, NSLayoutAttribute.Width, NSLayoutRelation.LessThanOrEqual, null, NSLayoutAttribute.NoAttribute, 1.0f, contentView.IntrinsicContentSize.Width),
				NSLayoutConstraint.Create(scrollView, NSLayoutAttribute.Height, NSLayoutRelation.LessThanOrEqual, null, NSLayoutAttribute.NoAttribute, 1.0f, contentView.IntrinsicContentSize.Height)
			});*/
            var minScale = (nfloat) Math.Max(scaleWidth, scaleHeight);
            scrollView.MinimumZoomScale = minScale;
            scrollView.MaximumZoomScale = 4.0f;
            SystemLog.Debug("[{0}] ScrollView.Handle = {1}", fixerID, scrollView.Handle);
            scrollView.ZoomScale = (nfloat) minScale.ClipTo(scrollView.MinimumZoomScale, scrollView.MaximumZoomScale);
        }
    }
}

