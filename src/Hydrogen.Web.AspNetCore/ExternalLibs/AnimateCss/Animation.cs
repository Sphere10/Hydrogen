// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System.ComponentModel;

// ReSharper disable InconsistentNaming

namespace Hydrogen.Web.AspNetCore.AnimateCss;

public enum Animation {
	// Attenion Seekers
	[Description("bounce")] bounce,

	[Description("flash")] flash,

	[Description("pulse")] pulse,

	[Description("rubberBand")] rubberBand,

	[Description("shake")] shake,

	[Description("swing")] swing,

	[Description("tada")] tada,

	[Description("wobble")] wobble,

	[Description("jello")] jello,

	// Bouncing Entrances
	[Description("bounceIn")] bounceIn,

	[Description("bounceInDown")] bounceInDown,

	[Description("bounceInLeft")] bounceInLeft,

	[Description("bounceInRight")] bounceInRight,

	[Description("bounceInUp")] bounceInUp,

	// Bouncing Exits   
	[Description("bounceOut")] bounceOut,

	[Description("bounceOutDown")] bounceOutDown,

	[Description("bounceOutLeft")] bounceOutLeft,

	[Description("bounceOutRight")] bounceOutRight,

	[Description("bounceOutUp")] bounceOutUp,

	// Fading Entrances
	[Description("fadeIn")] fadeIn,

	[Description("fadeInDown")] fadeInDown,

	[Description("fadeInDownBig")] fadeInDownBig,

	[Description("fadeInLeft")] fadeInLeft,

	[Description("fadeInLeftBig")] fadeInLeftBig,

	[Description("fadeInRight")] fadeInRight,

	[Description("fadeInRightBig")] fadeInRightBig,

	[Description("fadeInUp")] fadeInUp,

	[Description("fadeInUpBig")] fadeInUpBig,

	// Fading Exits
	[Description("fadeOut")] fadeOut,

	[Description("fadeOutDown")] fadeOutDown,

	[Description("fadeOutDownBig")] fadeOutDownBig,

	[Description("fadeOutLeft")] fadeOutLeft,

	[Description("fadeOutLeftBig")] fadeOutLeftBig,

	[Description("fadeOutRight")] fadeOutRight,

	[Description("fadeOutRightBig")] fadeOutRightBig,

	[Description("fadeOutUp")] fadeOutUp,

	[Description("fadeOutUpBig")] fadeOutUpBig,

	// Flippers
	[Description("flip")] flip,

	[Description("flipInX")] flipInX,

	[Description("flipInY")] flipInY,

	[Description("flipOutX")] flipOutX,

	[Description("flipOutY")] flipOutY,

	// Lightspeed
	[Description("lightSpeedIn")] lightSpeedIn,

	[Description("lightSpeedOut")] lightSpeedOut,

	// Rotating Entraces
	[Description("rotateIn")] rotateIn,

	[Description("rotateInDownLeft")] rotateInDownLeft,

	[Description("rotateInDownRight")] rotateInDownRight,

	[Description("rotateInUpLeft")] rotateInUpLeft,

	[Description("rotateInUpRight")] rotateInUpRight,

	// Rotating Exits
	[Description("rotateOut")] rotateOut,

	[Description("rotateOutDownLeft")] rotateOutDownLeft,

	[Description("rotateOutDownRight")] rotateOutDownRight,

	[Description("rotateOutUpLeft")] rotateOutUpLeft,

	[Description("rotateOutUpRight")] rotateOutUpRight,

	// Sliding Entrances
	[Description("slideInUp")] slideInUp,

	[Description("slideInDown")] slideInDown,

	[Description("slideInLeft")] slideInLeft,

	[Description("slideInRight")] slideInRight,

	// Sliding Exits

	[Description("slideOutUp")] slideOutUp,

	[Description("slideOutDown")] slideOutDown,

	[Description("slideOutLeft")] slideOutLeft,

	[Description("slideOutRight")] slideOutRight,

	// Specials
	[Description("hinge")] hinge,

	[Description("rollIn")] rollIn,

	[Description("rollOut")] rollOut,

	// Zoom Entrances 
	[Description("zoomIn")] zoomIn,

	[Description("zoomInDown")] zoomInDown,

	[Description("zoomInLeft")] zoomInLeft,

	[Description("zoomInRight")] zoomInRight,

	[Description("zoomInUp")] zoomInUp,

	// Zoom Exists
	[Description("zoomOut")] zoomOut,

	[Description("zoomOutDown")] zoomOutDown,

	[Description("zoomOutLeft")] zoomOutLeft,

	[Description("zoomOutRight")] zoomOutRight,

	[Description("zoomOutUp")] zoomOutUp
}
