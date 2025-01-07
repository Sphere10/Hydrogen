// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;
using NUnit.Framework;
using NUnit.Framework.Legacy;

namespace Hydrogen.Tests;

[TestFixture]
[Parallelizable(ParallelScope.Children)]
public class ThroughputOptimizerTests {

	// Paramters are: MinSize, MaxSize, Adjustment, Tolerance
	public static object[][] TestParameters = {
		new object[] { 1000, 10000, 0.1, 0.10 },
		new object[] { 1000, 10000, 0.25, 0.15 },
		new object[] { 1000, 10000, 0.5, 0.15 },
	};

	[Test]
	[TestCaseSource("TestParameters")]
	public void InitialDirection_1(long minSize, long maxSize, double adjustment, double tolerance) {
		var op = new ThroughputOptimizer(minSize, maxSize, adjustment, tolerance, ThroughputOptimizer.SamplingAdjustmentDirection.Increasing);
		ClassicAssert.AreEqual(ThroughputOptimizer.SamplingAdjustmentDirection.Increasing, op.AdjustmentDirection);
	}

	[Test]
	[TestCaseSource("TestParameters")]
	public void InitialDirection_2(long minSize, long maxSize, double adjustment, double tolerance) {
		var op = new ThroughputOptimizer(minSize, maxSize, adjustment, tolerance, ThroughputOptimizer.SamplingAdjustmentDirection.Decreasing);
		ClassicAssert.AreEqual(ThroughputOptimizer.SamplingAdjustmentDirection.Decreasing, op.AdjustmentDirection);
	}

	[Test]
	[TestCaseSource("TestParameters")]
	public void InitialDirection_Throws(long minSize, long maxSize, double adjustment, double tolerance) {
		Assert.Catch<Exception>(() => new ThroughputOptimizer(minSize, maxSize, adjustment, tolerance, ThroughputOptimizer.SamplingAdjustmentDirection.Stablized));
	}

	[Test]
	[TestCaseSource("TestParameters")]
	public void InitialSize_1(long minSize, long maxSize, double adjustment, double tolerance) {
		var op = new ThroughputOptimizer(minSize, maxSize, adjustment, tolerance, ThroughputOptimizer.SamplingAdjustmentDirection.Increasing);
		ClassicAssert.AreEqual(1000, op.SuggestedBatchSize);
	}

	[Test]
	[TestCaseSource("TestParameters")]
	public void InitialSize_2(long minSize, long maxSize, double adjustment, double tolerance) {
		var op = new ThroughputOptimizer(minSize, maxSize, adjustment, tolerance, ThroughputOptimizer.SamplingAdjustmentDirection.Decreasing);
		ClassicAssert.AreEqual(10000, op.SuggestedBatchSize);
	}

	[Test]
	[TestCaseSource("TestParameters")]
	public void IgnoreSample_Simple_1(long minSize, long maxSize, double adjustment, double tolerance) {
		const double breach = 0.01;
		var op = new ThroughputOptimizer(minSize, maxSize, adjustment, tolerance, ThroughputOptimizer.SamplingAdjustmentDirection.Increasing);
		op.RegisterSample((long)Math.Round(minSize * (1 - (adjustment + tolerance + breach)).ClipTo(0, double.MaxValue), 0), TimeSpan.FromSeconds(1));
		op.RegisterSample((long)Math.Round(minSize * (1 + (adjustment + tolerance + breach)).ClipTo(0, double.MaxValue), 0), TimeSpan.FromSeconds(1));
		ClassicAssert.AreEqual(op.SampleCount, 0);
	}

	[Test]
	[TestCaseSource("TestParameters")]
	public void IgnoreSample_Simple_2(long minSize, long maxSize, double adjustment, double tolerance) {
		const double breach = 0.01;
		var op = new ThroughputOptimizer(minSize, maxSize, adjustment, tolerance, ThroughputOptimizer.SamplingAdjustmentDirection.Decreasing);
		op.RegisterSample((long)Math.Round(maxSize * (1 - (adjustment + tolerance + breach)).ClipTo(0, double.MaxValue), 0), TimeSpan.FromSeconds(1));
		op.RegisterSample((long)Math.Round(maxSize * (1 + (adjustment + tolerance + breach)).ClipTo(0, double.MaxValue), 0), TimeSpan.FromSeconds(1));
		ClassicAssert.AreEqual(op.SampleCount, 0);
	}

	[Test]
	[TestCaseSource("TestParameters")]
	public void Increase_Simple(long minSize, long maxSize, double adjustment, double tolerance) {
		var op = new ThroughputOptimizer(minSize, maxSize, adjustment, tolerance, ThroughputOptimizer.SamplingAdjustmentDirection.Increasing);
		op.RegisterSample(minSize, TimeSpan.FromSeconds(1));
		ClassicAssert.AreEqual(1, op.SampleCount);
		ClassicAssert.AreEqual((long)Math.Round(minSize * (1 + adjustment), 0), op.SuggestedBatchSize);
	}

	[Test]
	[TestCaseSource("TestParameters")]
	public void Decrease_Simple(long minSize, long maxSize, double adjustment, double tolerance) {
		var op = new ThroughputOptimizer(minSize, maxSize, adjustment, tolerance, ThroughputOptimizer.SamplingAdjustmentDirection.Decreasing);
		op.RegisterSample(maxSize, TimeSpan.FromSeconds(1));
		ClassicAssert.AreEqual(1, op.SampleCount);
		ClassicAssert.AreEqual((long)Math.Round(maxSize * (1 - adjustment), 0), op.SuggestedBatchSize);
	}

	[Test]
	[TestCaseSource("TestParameters")]
	public void Increase_TwoSamples(long minSize, long maxSize, double adjustment, double tolerance) {
		var samples = new[] {
			CreateSample(minSize * Math.Pow(1 + adjustment, 0), 1),
			CreateSample(minSize * Math.Pow(1 + adjustment, 1), 2),
		};
		var op = new ThroughputOptimizer(minSize, maxSize, adjustment, tolerance, ThroughputOptimizer.SamplingAdjustmentDirection.Increasing);
		op.RegisterSample(samples[0].Item1, samples[0].Item2);
		ClassicAssert.AreEqual(1, op.SampleCount);
		op.RegisterSample(samples[1].Item1, samples[1].Item2);
		ClassicAssert.AreEqual(2, op.SampleCount);
		ClassicAssert.AreEqual((long)Math.Round(samples[1].Item1 * (1 + adjustment), 0), op.SuggestedBatchSize);
	}

	[Test]
	[TestCaseSource("TestParameters")]
	public void Decrease_TwoSamples(long minSize, long maxSize, double adjustment, double tolerance) {
		var samples = new[] {
			CreateSample(maxSize * Math.Pow(1 - adjustment, 0), 1),
			CreateSample(maxSize * Math.Pow(1 - adjustment, 1), 2),
		};

		var op = new ThroughputOptimizer(minSize, maxSize, adjustment, tolerance, ThroughputOptimizer.SamplingAdjustmentDirection.Decreasing);
		op.RegisterSample(samples[0].Item1, samples[0].Item2);
		ClassicAssert.AreEqual(1, op.SampleCount);
		op.RegisterSample(samples[1].Item1, samples[1].Item2);
		ClassicAssert.AreEqual(2, op.SampleCount);
		ClassicAssert.AreEqual((long)Math.Round(samples[1].Item1 * (1 - adjustment), 0), op.SuggestedBatchSize);
	}

	[Test]
	[TestCaseSource("TestParameters")]
	public void FindOptimal_Increasing(long minSize, long maxSize, double adjustment, double tolerance) {
		var samples = new[] {
			CreateSample(minSize * Math.Pow(1 + adjustment, 0), 1),
			CreateSample(minSize * Math.Pow(1 + adjustment, 1), 2),
			CreateSample(minSize * Math.Pow(1 + adjustment, 2), 3), // (optimal)
			CreateSample(minSize * Math.Pow(1 + adjustment, 3), 2),
		};

		var op = new ThroughputOptimizer(minSize, maxSize, adjustment, tolerance, ThroughputOptimizer.SamplingAdjustmentDirection.Increasing);
		op.RegisterSample(samples[0].Item1, samples[0].Item2);
		ClassicAssert.AreEqual(1, op.SampleCount);
		ClassicAssert.AreEqual(ThroughputOptimizer.SamplingAdjustmentDirection.Increasing, op.AdjustmentDirection);

		op.RegisterSample(samples[1].Item1, samples[1].Item2);
		ClassicAssert.AreEqual(2, op.SampleCount);
		ClassicAssert.AreEqual(ThroughputOptimizer.SamplingAdjustmentDirection.Increasing, op.AdjustmentDirection);

		op.RegisterSample(samples[2].Item1, samples[2].Item2);
		ClassicAssert.AreEqual(3, op.SampleCount);
		ClassicAssert.AreEqual(ThroughputOptimizer.SamplingAdjustmentDirection.Increasing, op.AdjustmentDirection);

		op.RegisterSample(samples[3].Item1, samples[3].Item2);
		ClassicAssert.AreEqual(4, op.SampleCount);
		ClassicAssert.AreEqual(ThroughputOptimizer.SamplingAdjustmentDirection.Stablized, op.AdjustmentDirection);

		ClassicAssert.AreEqual(samples[2].Item1, op.SuggestedBatchSize);

	}

	[Test]
	[TestCaseSource("TestParameters")]
	public void FindOptimal_Decreasing(long minSize, long maxSize, double adjustment, double tolerance) {
		var samples = new[] {
			CreateSample(maxSize * Math.Pow(1 - adjustment, 0), 1),
			CreateSample(maxSize * Math.Pow(1 - adjustment, 1), 2),
			CreateSample(maxSize * Math.Pow(1 - adjustment, 2), 3), // (optimal)
			CreateSample(maxSize * Math.Pow(1 - adjustment, 3), 2),
		};

		var op = new ThroughputOptimizer(minSize, maxSize, adjustment, tolerance, ThroughputOptimizer.SamplingAdjustmentDirection.Decreasing);
		op.RegisterSample(samples[0].Item1, samples[0].Item2);
		ClassicAssert.AreEqual(1, op.SampleCount);
		ClassicAssert.AreEqual(ThroughputOptimizer.SamplingAdjustmentDirection.Decreasing, op.AdjustmentDirection);

		op.RegisterSample(samples[1].Item1, samples[1].Item2);
		ClassicAssert.AreEqual(2, op.SampleCount);
		ClassicAssert.AreEqual(ThroughputOptimizer.SamplingAdjustmentDirection.Decreasing, op.AdjustmentDirection);

		op.RegisterSample(samples[2].Item1, samples[2].Item2);
		ClassicAssert.AreEqual(3, op.SampleCount);
		ClassicAssert.AreEqual(ThroughputOptimizer.SamplingAdjustmentDirection.Decreasing, op.AdjustmentDirection);

		op.RegisterSample(samples[3].Item1, samples[3].Item2);
		ClassicAssert.AreEqual(4, op.SampleCount);
		ClassicAssert.AreEqual(ThroughputOptimizer.SamplingAdjustmentDirection.Stablized, op.AdjustmentDirection);

		ClassicAssert.AreEqual(samples[2].Item1, op.SuggestedBatchSize);

	}

	[Test]
	[TestCaseSource("TestParameters")]
	public void OptimalChanged_FasterBigger(long minSize, long maxSize, double adjustment, double tolerance) {
		//Sample Velocity	Sample Size  	    Action
		//==================================================================
		//Faster			Bigger		        Keep sample, Increase 
		var samples = new[] {
			CreateSample(minSize * Math.Pow(1 + adjustment, 0), 1),
			CreateSample(minSize * Math.Pow(1 + adjustment, 1), 2),
			CreateSample(minSize * Math.Pow(1 + adjustment, 2), 3), // (optimal)
			CreateSample(minSize * Math.Pow(1 + adjustment, 3), 2)
		};
		var lastSample = CreateSample(minSize * Math.Pow(1 + adjustment, 4), 4); // (faster bigger)

		var op = new ThroughputOptimizer(minSize, maxSize, adjustment, tolerance, ThroughputOptimizer.SamplingAdjustmentDirection.Increasing);
		samples.ForEach(s => op.RegisterSample(s.Item1, s.Item2));
		ClassicAssert.AreEqual(samples.Length, op.SampleCount);
		ClassicAssert.AreEqual(ThroughputOptimizer.SamplingAdjustmentDirection.Stablized, op.AdjustmentDirection);
		ClassicAssert.AreEqual(samples[samples.Length - 2].Item1, op.SuggestedBatchSize);


		op.RegisterSample(lastSample.Item1, lastSample.Item2);
		ClassicAssert.AreEqual(ThroughputOptimizer.SamplingAdjustmentDirection.Increasing, op.AdjustmentDirection);
		Assert.That(Math.Round(lastSample.Item1 * (1.0 + adjustment), 0), Is.EqualTo((double)op.SuggestedBatchSize).Within(tolerance));
	}

	[Test]
	[TestCaseSource("TestParameters")]
	public void OptimalChanged_FasterSame(long minSize, long maxSize, double adjustment, double tolerance) {
		//Sample Velocity	Sample Size  	    Action
		//==================================================================
		//Faster            Same                Keep sample, Increase    
		var samples = new[] {
			CreateSample(minSize * Math.Pow(1 + adjustment, 0), 1),
			CreateSample(minSize * Math.Pow(1 + adjustment, 1), 2),
			CreateSample(minSize * Math.Pow(1 + adjustment, 2), 3), // (optimal)
			CreateSample(minSize * Math.Pow(1 + adjustment, 3), 2)
		};
		var lastSample = CreateSample(minSize * Math.Pow(1 + adjustment, 2), 4); // (faster same)

		var op = new ThroughputOptimizer(minSize, maxSize, adjustment, tolerance, ThroughputOptimizer.SamplingAdjustmentDirection.Increasing);
		samples.ForEach(s => op.RegisterSample(s.Item1, s.Item2));
		ClassicAssert.AreEqual(samples.Length, op.SampleCount);
		ClassicAssert.AreEqual(ThroughputOptimizer.SamplingAdjustmentDirection.Stablized, op.AdjustmentDirection);
		ClassicAssert.AreEqual(samples[samples.Length - 2].Item1, op.SuggestedBatchSize);


		op.RegisterSample(lastSample.Item1, lastSample.Item2);
		ClassicAssert.AreEqual(ThroughputOptimizer.SamplingAdjustmentDirection.Increasing, op.AdjustmentDirection);
		Assert.That(Math.Round(lastSample.Item1 * (1.0 + adjustment), 0), Is.EqualTo((double)op.SuggestedBatchSize).Within(tolerance));
	}

	[Test]
	[TestCaseSource("TestParameters")]
	public void OptimalChanged_FasterSmaller(long minSize, long maxSize, double adjustment, double tolerance) {
		//Sample Velocity	Sample Size  	    Action
		//==================================================================
		//Faster			Smaller			    Keep sample, Decrease  
		var samples = new[] {
			CreateSample(minSize * Math.Pow(1 + adjustment, 0), 1),
			CreateSample(minSize * Math.Pow(1 + adjustment, 1), 2),
			CreateSample(minSize * Math.Pow(1 + adjustment, 2), 3), // (optimal)
			CreateSample(minSize * Math.Pow(1 + adjustment, 3), 2)
		};
		var lastSample = CreateSample(minSize * Math.Pow(1 + adjustment, 0.5), 5); // (faster smaller)

		var op = new ThroughputOptimizer(minSize, maxSize, adjustment, tolerance, ThroughputOptimizer.SamplingAdjustmentDirection.Increasing);
		samples.ForEach(s => op.RegisterSample(s.Item1, s.Item2));
		ClassicAssert.AreEqual(samples.Length, op.SampleCount);
		ClassicAssert.AreEqual(ThroughputOptimizer.SamplingAdjustmentDirection.Stablized, op.AdjustmentDirection);
		ClassicAssert.AreEqual(samples[samples.Length - 2].Item1, op.SuggestedBatchSize);


		op.RegisterSample(lastSample.Item1, lastSample.Item2);
		ClassicAssert.AreEqual(ThroughputOptimizer.SamplingAdjustmentDirection.Decreasing, op.AdjustmentDirection);
		Assert.That(Math.Round(lastSample.Item1 * (1.0 - adjustment), 0), Is.EqualTo((double)op.SuggestedBatchSize).Within(tolerance));
	}

	[Test]
	[TestCaseSource("TestParameters")]
	public void OptimalChanged_SameBigger(long minSize, long maxSize, double adjustment, double tolerance) {
		//Sample Velocity	Sample Size  	    Action
		//==================================================================
		//Same              Bigger              Keep sample, Increase    
		var samples = new[] {
			CreateSample(minSize * Math.Pow(1 + adjustment, 0), 1),
			CreateSample(minSize * Math.Pow(1 + adjustment, 1), 2),
			CreateSample(minSize * Math.Pow(1 + adjustment, 2), 3), // (optimal)
			CreateSample(minSize * Math.Pow(1 + adjustment, 3), 2)
		};
		var lastSample = CreateSample(minSize * Math.Pow(1 + adjustment, 4), 3); // (faster same)

		var op = new ThroughputOptimizer(minSize, maxSize, adjustment, tolerance, ThroughputOptimizer.SamplingAdjustmentDirection.Increasing);
		samples.ForEach(s => op.RegisterSample(s.Item1, s.Item2));
		ClassicAssert.AreEqual(samples.Length, op.SampleCount);
		ClassicAssert.AreEqual(ThroughputOptimizer.SamplingAdjustmentDirection.Stablized, op.AdjustmentDirection);
		ClassicAssert.AreEqual(samples[samples.Length - 2].Item1, op.SuggestedBatchSize);


		op.RegisterSample(lastSample.Item1, lastSample.Item2);
		ClassicAssert.AreEqual(ThroughputOptimizer.SamplingAdjustmentDirection.Increasing, op.AdjustmentDirection);
		Assert.That(Math.Round(lastSample.Item1 * (1.0 + adjustment), 0), Is.EqualTo((double)op.SuggestedBatchSize).Within(tolerance));
	}

	[Test]
	[TestCaseSource("TestParameters")]
	public void OptimalChanged_SameSame(long minSize, long maxSize, double adjustment, double tolerance) {
		//Sample Velocity	Sample Size  	    Action
		//==================================================================
		//Same              Same                Ignore sample, Stable   
		var samples = new[] {
			CreateSample(minSize * Math.Pow(1 + adjustment, 0), 1),
			CreateSample(minSize * Math.Pow(1 + adjustment, 1), 2),
			CreateSample(minSize * Math.Pow(1 + adjustment, 2), 3), // (optimal)
			CreateSample(minSize * Math.Pow(1 + adjustment, 3), 2)
		};
		var lastSample = CreateSample(minSize * Math.Pow(1 + adjustment, 2), 3); // (same same)

		var op = new ThroughputOptimizer(minSize, maxSize, adjustment, tolerance, ThroughputOptimizer.SamplingAdjustmentDirection.Increasing);
		samples.ForEach(s => op.RegisterSample(s.Item1, s.Item2));
		ClassicAssert.AreEqual(samples.Length, op.SampleCount);
		ClassicAssert.AreEqual(ThroughputOptimizer.SamplingAdjustmentDirection.Stablized, op.AdjustmentDirection);
		ClassicAssert.AreEqual(samples[samples.Length - 2].Item1, op.SuggestedBatchSize);


		op.RegisterSample(lastSample.Item1, lastSample.Item2);
		ClassicAssert.AreEqual(ThroughputOptimizer.SamplingAdjustmentDirection.Stablized, op.AdjustmentDirection);
		ClassicAssert.AreEqual(samples[samples.Length - 2].Item1, op.SuggestedBatchSize);
	}

	[Test]
	[TestCaseSource("TestParameters")]
	public void OptimalChanged_SameSmaller(long minSize, long maxSize, double adjustment, double tolerance) {
		//Sample Velocity	Sample Size  	    Action
		//==================================================================
		//Same              Smaller             Ignore sample, Stable
		var samples = new[] {
			CreateSample(minSize * Math.Pow(1 + adjustment, 0), 1),
			CreateSample(minSize * Math.Pow(1 + adjustment, 1), 2),
			CreateSample(minSize * Math.Pow(1 + adjustment, 2), 3), // (optimal)
			CreateSample(minSize * Math.Pow(1 + adjustment, 3), 2)
		};
		var lastSample = CreateSample(minSize * Math.Pow(1 + adjustment, 1), 3); // (same smaller)

		var op = new ThroughputOptimizer(minSize, maxSize, adjustment, tolerance, ThroughputOptimizer.SamplingAdjustmentDirection.Increasing);
		samples.ForEach(s => op.RegisterSample(s.Item1, s.Item2));
		ClassicAssert.AreEqual(samples.Length, op.SampleCount);
		ClassicAssert.AreEqual(ThroughputOptimizer.SamplingAdjustmentDirection.Stablized, op.AdjustmentDirection);
		ClassicAssert.AreEqual(samples[samples.Length - 2].Item1, op.SuggestedBatchSize);


		op.RegisterSample(lastSample.Item1, lastSample.Item2);
		ClassicAssert.AreEqual(ThroughputOptimizer.SamplingAdjustmentDirection.Stablized, op.AdjustmentDirection);
		ClassicAssert.AreEqual(samples[samples.Length - 2].Item1, op.SuggestedBatchSize);
	}

	[Test]
	[TestCaseSource("TestParameters")]
	public void OptimalChanged_SlowerBigger(long minSize, long maxSize, double adjustment, double tolerance) {
		//Sample Velocity	Sample Size  	    Action
		//==================================================================
		//Slower			Bigger			    Ignore sample, Stable   
		var samples = new[] {
			CreateSample(minSize * Math.Pow(1 + adjustment, 0), 1),
			CreateSample(minSize * Math.Pow(1 + adjustment, 1), 2),
			CreateSample(minSize * Math.Pow(1 + adjustment, 2), 3), // (optimal)
			CreateSample(minSize * Math.Pow(1 + adjustment, 3), 2)
		};
		var lastSample = CreateSample(minSize * Math.Pow(1 + adjustment, 4), 2); // (slower bigger)

		var op = new ThroughputOptimizer(minSize, maxSize, adjustment, tolerance, ThroughputOptimizer.SamplingAdjustmentDirection.Increasing);
		samples.ForEach(s => op.RegisterSample(s.Item1, s.Item2));
		ClassicAssert.AreEqual(samples.Length, op.SampleCount);
		ClassicAssert.AreEqual(ThroughputOptimizer.SamplingAdjustmentDirection.Stablized, op.AdjustmentDirection);
		ClassicAssert.AreEqual(samples[samples.Length - 2].Item1, op.SuggestedBatchSize);


		op.RegisterSample(lastSample.Item1, lastSample.Item2);
		ClassicAssert.AreEqual(ThroughputOptimizer.SamplingAdjustmentDirection.Stablized, op.AdjustmentDirection);
		ClassicAssert.AreEqual(samples[samples.Length - 2].Item1, op.SuggestedBatchSize);
	}


	[Test]
	[TestCaseSource("TestParameters")]
	public void OptimalChanged_SlowerSame_1(long minSize, long maxSize, double adjustment, double tolerance) {
		//Sample Velocity	Sample Size  	    Action
		//==================================================================
		//Slower            Same                Ignore sample, Initial increase/decrease
		var samples = new[] {
			CreateSample(minSize * Math.Pow(1 + adjustment, 0), 1),
			CreateSample(minSize * Math.Pow(1 + adjustment, 1), 2),
			CreateSample(minSize * Math.Pow(1 + adjustment, 2), 3), // (optimal)
			CreateSample(minSize * Math.Pow(1 + adjustment, 3), 2)
		};
		var lastSample = CreateSample(minSize * Math.Pow(1 + adjustment, 2), 2); // (slower same)

		var op = new ThroughputOptimizer(minSize, maxSize, adjustment, tolerance, ThroughputOptimizer.SamplingAdjustmentDirection.Increasing);
		samples.ForEach(s => op.RegisterSample(s.Item1, s.Item2));
		ClassicAssert.AreEqual(samples.Length, op.SampleCount);
		ClassicAssert.AreEqual(ThroughputOptimizer.SamplingAdjustmentDirection.Stablized, op.AdjustmentDirection);
		ClassicAssert.AreEqual(samples[samples.Length - 2].Item1, op.SuggestedBatchSize);


		op.RegisterSample(lastSample.Item1, lastSample.Item2);
		ClassicAssert.AreEqual(ThroughputOptimizer.SamplingAdjustmentDirection.Increasing, op.AdjustmentDirection);
		Assert.That(Math.Round(samples[samples.Length - 2].Item1 * (1.0 + adjustment), 0), Is.EqualTo((double)op.SuggestedBatchSize).Within(tolerance));
	}


	[Test]
	[TestCaseSource("TestParameters")]
	public void OptimalChanged_SlowerSmaller(long minSize, long maxSize, double adjustment, double tolerance) {
		//Sample Velocity	Sample Size  	    Action
		//==================================================================
		//Slower			Smaller		        Ignore sample, Stable
		var samples = new[] {
			CreateSample(minSize * Math.Pow(1 + adjustment, 0), 1),
			CreateSample(minSize * Math.Pow(1 + adjustment, 1), 2),
			CreateSample(minSize * Math.Pow(1 + adjustment, 2), 3), // (optimal)
			CreateSample(minSize * Math.Pow(1 + adjustment, 3), 2)
		};
		var lastSample = CreateSample(minSize * Math.Pow(1 + adjustment, 0.5), 1); // (slower smaller)

		var op = new ThroughputOptimizer(minSize, maxSize, adjustment, tolerance, ThroughputOptimizer.SamplingAdjustmentDirection.Increasing);
		samples.ForEach(s => op.RegisterSample(s.Item1, s.Item2));
		ClassicAssert.AreEqual(samples.Length, op.SampleCount);
		ClassicAssert.AreEqual(ThroughputOptimizer.SamplingAdjustmentDirection.Stablized, op.AdjustmentDirection);
		ClassicAssert.AreEqual(samples[samples.Length - 2].Item1, op.SuggestedBatchSize);


		op.RegisterSample(lastSample.Item1, lastSample.Item2);
		ClassicAssert.AreEqual(ThroughputOptimizer.SamplingAdjustmentDirection.Stablized, op.AdjustmentDirection);
		ClassicAssert.AreEqual(samples[samples.Length - 2].Item1, op.SuggestedBatchSize);
	}


	private Tuple<long, TimeSpan> CreateSample(double size, double velocity) {
		return Tuple.Create((long)Math.Round(size, 0), TimeSpan.FromSeconds(size / velocity));
	}

}
