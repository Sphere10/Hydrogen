// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT NON-AI software license, see the accompanying file
// LICENSE or visit https://sphere10.com/legal/NON-AI-MIT.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;

namespace Hydrogen;

public class ThroughputOptimizer {
	private readonly long _minimumBatchSize;
	private readonly long _maximumBatchSize;
	private readonly double _adjustmentFactor;
	private readonly double _toleranceFactor;
	private readonly SamplingAdjustmentDirection _initialAdjustmentDirection;
	private long _optimalBatchSize;
	private double _optimalBatchVelocity;

	public ThroughputOptimizer(long minimumBatchSize, long maximumBatchSize, double adjustmentPercentage, double stabilityTolerancePercentage, SamplingAdjustmentDirection initialAdjustmentDirection = SamplingAdjustmentDirection.Increasing)
		: this(minimumBatchSize, maximumBatchSize, (int)Math.Round(adjustmentPercentage * 100.0, 0), (int)Math.Round(stabilityTolerancePercentage * 100.0, 0), initialAdjustmentDirection) {

	}

	public ThroughputOptimizer(long minimumBatchSize, long maximumBatchSize, int adjustmentPercentage = 25, int stabilityTolerancePercentage = 10, SamplingAdjustmentDirection initialAdjustmentDirection = SamplingAdjustmentDirection.Increasing) {
		if (adjustmentPercentage < 0 || adjustmentPercentage > 100) throw new ArgumentOutOfRangeException("adjustmentPercentage", adjustmentPercentage, "Needs to be between 0 and 100");
		if (stabilityTolerancePercentage < 0 || stabilityTolerancePercentage > 100) throw new ArgumentOutOfRangeException("stabilityTolerancePercentage", stabilityTolerancePercentage, "Needs to be between 0 and 100");
		if (stabilityTolerancePercentage < 0 || stabilityTolerancePercentage > 100) throw new ArgumentOutOfRangeException("stabilityTolerancePercentage", stabilityTolerancePercentage, "Needs to be between 0 and 100");
		if (initialAdjustmentDirection == SamplingAdjustmentDirection.Stablized) throw new ArgumentException("Must be Increasing or Decreasing", "initialAdjustmentDirection");
		if (minimumBatchSize < 0) throw new ArgumentOutOfRangeException("minimumBatchSize", minimumBatchSize, "Must be greater than 0");
		if (maximumBatchSize < minimumBatchSize) throw new ArgumentException("maximumBatchSize must be greater than minimumBatchSize");

		_minimumBatchSize = minimumBatchSize;
		_maximumBatchSize = maximumBatchSize;
		_adjustmentFactor = adjustmentPercentage / 100.0;
		_toleranceFactor = stabilityTolerancePercentage / 100.0;
		_initialAdjustmentDirection = initialAdjustmentDirection;
		AdjustmentDirection = initialAdjustmentDirection;
		SampleCount = 0;
		_optimalBatchSize = 0;
		_optimalBatchVelocity = 0;
	}

	public SamplingAdjustmentDirection AdjustmentDirection { get; private set; }

	public char AdjustDirectionSymbol {
		get {
			switch (AdjustmentDirection) {
				case SamplingAdjustmentDirection.Increasing:
					return '?';

				case SamplingAdjustmentDirection.Decreasing:
					return '?';

				case SamplingAdjustmentDirection.Stablized:
				default:
					return '?';
			}

		}
	}

	public long SampleCount { get; private set; }

	public long SuggestedBatchSize {
		get {
			if (SampleCount == 0) {
				return AdjustmentDirection == SamplingAdjustmentDirection.Increasing ? _minimumBatchSize : _maximumBatchSize;
			}
			switch (AdjustmentDirection) {
				case SamplingAdjustmentDirection.Stablized:
					return _optimalBatchSize;
				case SamplingAdjustmentDirection.Increasing:
					return (long)Math.Round(_optimalBatchSize * (1 + _adjustmentFactor), 0);
				case SamplingAdjustmentDirection.Decreasing:
					return (long)Math.Round(_optimalBatchSize * (1 - _adjustmentFactor), 0);
				default:
					throw new NotSupportedException();
			}
		}
	}

	public void RegisterSample(long sampleBatchSize, TimeSpan sampleDuration) {
		if (sampleBatchSize < 0) throw new ArgumentOutOfRangeException("sampleBatchSize", "Must be greater than or equal to 0");
		if (sampleDuration <= TimeSpan.Zero) throw new ArgumentOutOfRangeException("sampleDuration", "Must be greater than or equal to TimeSpan.Zero");

		var recommendedSize = SuggestedBatchSize;
		if (AdjustmentDirection != SamplingAdjustmentDirection.Stablized && 1.0 - (double)Math.Min(sampleBatchSize, SuggestedBatchSize) / Math.Max(sampleBatchSize, SuggestedBatchSize) > _toleranceFactor) {
			// Ignore samples out of range if we're searching for optimal value, consider it if we've already found optimal value 
			return;
		}

		var sampleVelocity = sampleBatchSize / sampleDuration.TotalMilliseconds;

		if (SampleCount++ == 0) {
			// If sample not within suggestion +/- tolerance, ignore it                
			_optimalBatchSize = sampleBatchSize;
			_optimalBatchVelocity = sampleVelocity;
		} else {
			switch (AdjustmentDirection) {
				case SamplingAdjustmentDirection.Stablized:

					//Sample Velocity	Sample Size  	    Action
					//==================================================================
					//Faster			Bigger		        Keep sample, Increase 
					//Faster            Same                Keep sample, Increase    
					//Faster			Smaller			    Keep sample, Decrease
					//Same              Bigger              Keep sample, Increase
					//Same              Same                Ignore sample, Stable
					//Same              Smaller             Ignore sample, Stable
					//Slower			Bigger			    Ignore sample, Stable
					//Slower            Same                Ignore sample, Initial increase/decrease
					//Slower			Smaller		        Ignore sample, Stable
					int sampleVelocityCondition = 0, sampleSizeCondition = 0;
					if (1.0 - (double)Math.Min(sampleVelocity, _optimalBatchVelocity) / Math.Max(sampleVelocity, _optimalBatchVelocity) > _toleranceFactor) {
						sampleVelocityCondition = sampleVelocity > _optimalBatchVelocity ? 1 : -1;
					}
					if (1.0 - (double)Math.Min(sampleBatchSize, recommendedSize) / Math.Max(sampleBatchSize, recommendedSize) > _toleranceFactor) {
						sampleSizeCondition = sampleBatchSize > recommendedSize ? 1 : -1;
					}

					switch (sampleVelocityCondition) {
						case 1:
							switch (sampleSizeCondition) {
								case 1:
									_optimalBatchSize = sampleBatchSize;
									_optimalBatchVelocity = sampleVelocity;
									AdjustmentDirection = SamplingAdjustmentDirection.Increasing;
									break;
								case 0:
									_optimalBatchSize = sampleBatchSize;
									_optimalBatchVelocity = sampleVelocity;
									AdjustmentDirection = SamplingAdjustmentDirection.Increasing;
									break;
								case -1:
									_optimalBatchSize = sampleBatchSize;
									_optimalBatchVelocity = sampleVelocity;
									AdjustmentDirection = SamplingAdjustmentDirection.Decreasing;
									break;
							}

							break;
						case 0:
							switch (sampleSizeCondition) {
								case 1:
									_optimalBatchSize = sampleBatchSize;
									_optimalBatchVelocity = sampleVelocity;
									AdjustmentDirection = SamplingAdjustmentDirection.Increasing;
									break;
								case 0:
									break;
								case -1:
									break;
							}
							break;
						case -1:
							switch (sampleSizeCondition) {
								case 1:
									break;
								case 0:
									AdjustmentDirection = _initialAdjustmentDirection;
									break;
								case -1:
									break;
							}
							break;
					}
					break;
				case SamplingAdjustmentDirection.Increasing:
				case SamplingAdjustmentDirection.Decreasing:
					// Searching via increasing sizes, so
					// 1. Ignore sample if size is not within recommended size (bad sample)
					// 2. If sample velocity is faster than previous sample velocity, keep searching higher
					// 3. If sample velocity is slower than previous sample velocity, optimal velocity found (previous sample)

					if (1.0 - (double)Math.Min(sampleBatchSize, recommendedSize) / Math.Max(sampleBatchSize, recommendedSize) <= _toleranceFactor) {
						if (sampleVelocity >= _optimalBatchVelocity) {
							_optimalBatchSize = sampleBatchSize;
							_optimalBatchVelocity = sampleVelocity;
						} else {
							AdjustmentDirection = SamplingAdjustmentDirection.Stablized;
						}
					}
					break;
			}
		}
	}


	public enum SamplingAdjustmentDirection {
		Stablized,
		Increasing,
		Decreasing,
	}
}
