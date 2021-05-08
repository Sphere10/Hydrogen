//-----------------------------------------------------------------------
// <copyright file="Statistics.cs" company="Sphere 10 Software">
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

namespace Sphere10.Framework {

	/// <summary>
	/// Summary description for Statistics.
	/// </summary>
	[Serializable]
    public class Statistics {

        public const double EPSILON = 0.00001;

        public Statistics() {
            Reset();
        }

        #region Properties

        public uint SampleCount { get; private set; }

        public double Minimum { get; private set; }

        public double Maximum { get; private set; }

        public double Mean {
            get {
                var mean = double.NaN;
                if (SampleCount > 0) {
                    mean = Sum / SampleCount;
                }
                return mean;
            }
        }

        public double PopulationStandardDeviation {
            get {
                var stdDev = double.NaN;
                stdDev = Math.Sqrt(PopulationVariance);
                return stdDev;
            }
        }

        public double PopulationVariance {
            get {
                var variance = double.NaN;
                if (SampleCount > 0) {
                    variance = ((SampleCount * SquaredSum) - Sum * Sum) / (SampleCount * SampleCount);
                }
                return variance;
            }
        }

        public double PopulationVariationCoefficient {
            get {
                var varCoeff = double.NaN;
                if (SampleCount > 0) {
                    varCoeff = (PopulationVariance / Mean) * 100;
                }
                return varCoeff;
            }
        }

        public double GeometricMean {
            get {
                var gmean = double.NaN;
                if (SampleCount > 0) {
                    gmean = Math.Pow(Product, 1.0 / SampleCount);
                }
                return gmean;
            }
        }

        public double HarmonicMean {
            get {
                var hmean = double.NaN;
                if (SampleCount > 0) {
                    hmean = SampleCount / ReciprocalSum;
                }
                return hmean;
            }
        }

        /// <summary>
        /// Return the error (%) for the minimum datum.
        /// </summary>
        public double MinimumError {
            get {
                var error = double.NaN;
                if ((Mean * Mean) > (EPSILON * EPSILON)) {
                    error = 100.0 * (Minimum - Mean) / Mean;
                }
                return error;
            }
        }


        /// <summary>
        /// Return the error (%) for the maximum datum.
        /// </summary>
        public double MaximumError {
            get {
                var error = double.NaN;
                if ((Mean * Mean) > (EPSILON * EPSILON)) {
                    error = 100.0 * (Maximum - Mean) / Mean;
                }
                return error;
            }
        }

        public double Sum { get; private set; }

        public double ReciprocalSum { get; private set; }

        public double SquaredSum { get; private set; }

        public double Product { get; private set; }

        public double SampleStandardDeviation {
            get {
                var stdDev = double.NaN;
                if (SampleCount >= 2) {
                    stdDev = Math.Sqrt(SampleVariance);
                }
                return stdDev;
            }
        }

        public double SampleVariance {
            get {
                var variance = double.NaN;
                if (SampleCount > 0) {
                    variance = ((SampleCount * SquaredSum) - Sum * Sum) /
                        ((SampleCount - 1) * (SampleCount - 1));
                }
                return variance;
            }
        }

        public double SampleVariationCoefficient {
            get {
                var varCoeff = double.NaN;
                if (SampleCount >= 2) {
                    varCoeff = 100 * (SampleStandardDeviation / Mean);
                }
                return varCoeff;
            }
        }



        #endregion

        public void Reset() {
            SampleCount = 0;
            Minimum = 0.0;
            Maximum = 0.0;
            Sum = 0.0;
            SquaredSum = 0.0;
            ReciprocalSum = 0.0;
            Product = 1.0;
        }

        public void AddDatum(double datum) {
            SampleCount++;
            Sum += datum;
            SquaredSum += datum * datum;
            if (double.IsNaN(ReciprocalSum) || datum * datum < EPSILON * EPSILON) {
                ReciprocalSum = double.NaN;
            } else {
                ReciprocalSum += (1 / datum);
            }

            Product *= datum;

            if (SampleCount == 1) {
                // first data so set _min/_max
                Minimum = datum;
                Maximum = datum;
            } else {
                // adjust _min/_max boundaries if necessary
                if (datum < Minimum) {
                    Minimum = datum;
                }
                if (datum > Maximum) {
                    Maximum = datum;
                }
            }
        }

        public void AddDatum(double datum, uint numTimes) {
            SampleCount += numTimes;
            Sum += datum * numTimes;
            SquaredSum += datum * datum * numTimes;
            if (double.IsNaN(ReciprocalSum) || datum * datum < EPSILON * EPSILON) {
                ReciprocalSum = double.NaN;
            } else {
                ReciprocalSum += (1 / datum) * numTimes;
            }

            Product *= Math.Pow(datum, numTimes);

            if (SampleCount == 1) {
                // first data so set _min/_max
                Minimum = datum;
                Maximum = datum;
            } else {
                // adjust _min/_max boundaries if necessary
                if (datum < Minimum) {
                    Minimum = datum;
                }
                if (datum > Maximum) {
                    Maximum = datum;
                }
            }
        }

        public void RemoveDatum(double datum) {
            SampleCount--;
            Sum -= datum;
            SquaredSum -= datum * datum;
            ReciprocalSum -= 1.0 / datum;
            Product /= datum;
        }

    }
}
