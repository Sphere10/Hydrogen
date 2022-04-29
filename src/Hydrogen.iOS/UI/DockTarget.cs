//-----------------------------------------------------------------------
// <copyright file="DockTarget.cs" company="Sphere 10 Software">
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

namespace Hydrogen.iOS {
    public class DockTarget {
        private readonly UIView _view;
        private readonly float _percentage;
        private readonly float _fixedLength;

        private DockTarget() {
            TargetType = DockTargetType.DockToContainer;
        }

        private DockTarget(UIView view) {
            TargetType = DockTargetType.DockToView;
            _view = view;
        }

        private DockTarget(float? percentage, float? fixedLength) {
            if (percentage.HasValue && fixedLength.HasValue)
                throw new ArgumentException("Percentage or fixedLength are mutually exclusive, one must be null");

            if (percentage.HasValue) {
                TargetType = DockTargetType.PercentageOfContainer;
                _percentage = percentage.Value;
            } else if (fixedLength.HasValue) {
                TargetType = DockTargetType.FixedLength;
                _fixedLength = fixedLength.Value;
            }
        }

        public static DockTarget ToContainer() {
            return new DockTarget();
        }

        public static DockTarget ToView(UIView view) {
            return new DockTarget(view);
        }

        public static DockTarget ToFixedLength(float length) {
            return new DockTarget(null, length);
        }

        public static DockTarget ToContainerPercentage(float percentage) {
            return new DockTarget(percentage, null);
        }



        public DockTargetType TargetType { get; private set; }

        public UIView View {
            get {
                if (TargetType != DockTargetType.DockToView)
                    throw new Exception("Not docked to a view");
                return _view;
            }
        }

        public float Percentage {
            get {
                if (TargetType != DockTargetType.PercentageOfContainer)
                    throw new Exception("Not docked to a view");
                return _percentage;
            }
        }

        public float FixedLength {
            get {
                if (TargetType != DockTargetType.FixedLength)
                    throw new Exception("Not docked to a view");
                return _fixedLength;
            }
        }

    }
}

