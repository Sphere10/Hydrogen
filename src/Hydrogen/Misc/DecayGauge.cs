// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT NON-AI software license, see the accompanying file
// LICENSE or visit https://sphere10.com/legal/NON-AI-MIT.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;

namespace Hydrogen;

/// <summary>
/// A gauge who's level decays with time.
/// 
/// Level = ((DecayOffset + ElapsedSeconds) * DecayCoefficient ) ^ DecayExponent
/// 
/// You can top-up the level by calling RegisterEvent
/// </summary>
public sealed class DecayGauge {
	private double _decayCoefficient;
	private double _decayExponent;
	private double _decayOffset;
	private double _level;
	private DateTime _lastEventTime;

	//key strength=0.040
	//mouse act = 0.020
	//mouse click = 0.1
	public static readonly DecayGauge RSIGauge = new DecayGauge(0, 0, 0.04, 2.0, 0, 100);

	public static readonly DecayGauge ScreenMouse = new DecayGauge(0, 0.13, 4, 3, 0, 50);

	public DecayGauge() : this(0.0d, 0.0d, 1.0d, 0.0d) {
	}

	public DecayGauge(double startLevel, double decayOffset, double decayCoefficient, double decayExponent, double minValue = 0, double maxValue = double.MaxValue) {
		Level = startLevel;
		DecayOffset = decayOffset;
		DecayCoefficient = decayCoefficient;
		DecayExponent = decayExponent;
		MaxValue = maxValue;
		MinValue = minValue;
	}

	public void RegisterEvent(double eventStrength) {
		Level = (Level + eventStrength).ClipTo(MinValue, MaxValue);
		_lastEventTime = DateTime.Now;
	}

	public double Level {
		get {
			var decay = Math.Pow((DecayOffset + DateTime.Now.Subtract(_lastEventTime).TotalSeconds) * DecayCoefficient, DecayExponent);
			return (_level - decay).ClipTo(MinValue, MaxValue);
		}
		private set {
			_level = value;
			_lastEventTime = DateTime.Now;
		}
	}

	public void Reset() {
		Level = 0;
	}

	public double DecayCoefficient {
		get { return _decayCoefficient; }
		set {
			_level = Level;
			_decayCoefficient = value;
		}
	}

	public double DecayExponent {
		get { return _decayExponent; }
		set {
			_level = Level;
			_decayExponent = value;
		}
	}

	public double DecayOffset {
		get { return _decayOffset; }
		set {
			_level = Level;
			_decayOffset = value;
		}
	}

	public double MinValue { get; set; }

	public double MaxValue { get; set; }
}
