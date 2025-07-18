using System;
using System.Collections.Generic;
using System.Linq;
using YieldCurveCalculator.Models;

namespace YieldCurveCalculator.Services
{
    public class InterpolationService
    {
        /// <summary>
        /// Performs linear interpolation on yield curve data to find the yield for a given maturity
        /// </summary>
        /// <param name="bonds">List of bonds representing the yield curve</param>
        /// <param name="targetMaturity">The maturity years for which to interpolate the yield</param>
        /// <returns>The interpolated yield value</returns>
        public double LinearInterpolation(List<Bond> bonds, double targetMaturity)
        {
            if (bonds == null || bonds.Count == 0)
                throw new ArgumentException($"Bond list cannot be null or empty. bonds: {bonds}");

            if (bonds.Count == 1)
                throw new ArgumentException("Cannot interpolate with only one bond. At least two bonds are required.");

            // Sort bonds by maturity to ensure proper interpolation
            var sortedBonds = bonds.OrderBy(b => b.MaturityYears).ToList();

            // Check if target maturity exactly matches an existing bond
            var exactMatch = sortedBonds.FirstOrDefault(b => Math.Abs(b.MaturityYears - targetMaturity) < 1e-9);
            if (exactMatch != null)
                return exactMatch.Yield;

            // Find the two bonds that bracket the target maturity
            Bond? lowerBond = null;
            Bond? upperBond = null;

            for (int i = 0; i < sortedBonds.Count - 1; i++)
            {
                if (sortedBonds[i].MaturityYears <= targetMaturity && 
                    sortedBonds[i + 1].MaturityYears >= targetMaturity)
                {
                    lowerBond = sortedBonds[i];
                    upperBond = sortedBonds[i + 1];
                    break;
                }
            }

            // Handle extrapolation cases
            if (lowerBond == null && upperBond == null)
            {
                // Target is outside the range - use linear extrapolation
                if (targetMaturity < sortedBonds.First().MaturityYears)
                {
                    // Extrapolate using first two points
                    return ExtrapolateLinear(sortedBonds[0], sortedBonds[1], targetMaturity);
                }
                else
                {
                    // Extrapolate using last two points
                    var lastIndex = sortedBonds.Count - 1;
                    return ExtrapolateLinear(sortedBonds[lastIndex - 1], sortedBonds[lastIndex], targetMaturity);
                }
            }

            // Perform linear interpolation
            return InterpolateLinear(lowerBond!, upperBond!, targetMaturity);
        }

        /// <summary>
        /// Performs linear interpolation between two bonds
        /// </summary>
        private double InterpolateLinear(Bond bond1, Bond bond2, double targetMaturity)
        {
            double x1 = bond1.MaturityYears;
            double y1 = bond1.Yield;
            double x2 = bond2.MaturityYears;
            double y2 = bond2.Yield;

            // Linear interpolation formula: y = y1 + (x - x1) * (y2 - y1) / (x2 - x1)
            return y1 + (targetMaturity - x1) * (y2 - y1) / (x2 - x1);
        }

        /// <summary>
        /// Performs linear extrapolation using two bonds
        /// </summary>
        private double ExtrapolateLinear(Bond bond1, Bond bond2, double targetMaturity)
        {
            // Same formula as interpolation, but allows extrapolation outside the range
            return InterpolateLinear(bond1, bond2, targetMaturity);
        }

        /// <summary>
        /// Gets multiple interpolated yields for a range of maturities
        /// </summary>
        /// <param name="bonds">List of bonds representing the yield curve</param>
        /// <param name="maturities">Array of maturity years to interpolate</param>
        /// <returns>Dictionary mapping maturity to interpolated yield</returns>
        public Dictionary<double, double> InterpolateMultiple(List<Bond> bonds, double[] maturities)
        {
            var results = new Dictionary<double, double>();
            
            foreach (var maturity in maturities)
            {
                results[maturity] = LinearInterpolation(bonds, maturity);
            }
            
            return results;
        }

        /// <summary>
        /// Generates a smooth yield curve by interpolating at regular intervals
        /// </summary>
        /// <param name="bonds">List of bonds representing the yield curve</param>
        /// <param name="stepSize">Step size in years (default: 0.5)</param>
        /// <returns>List of interpolated bonds representing a smooth curve</returns>
        public List<Bond> GenerateSmoothCurve(List<Bond> bonds, double stepSize = 0.5)
        {
            if (bonds == null || bonds.Count == 0)
                throw new ArgumentException($"Bond list cannot be null or empty. bonds: {bonds}");

            var sortedBonds = bonds.OrderBy(b => b.MaturityYears).ToList();
            var smoothCurve = new List<Bond>();

            double minMaturity = sortedBonds.First().MaturityYears;
            double maxMaturity = sortedBonds.Last().MaturityYears;

            for (double maturity = minMaturity; maturity <= maxMaturity; maturity += stepSize)
            {
                var interpolatedYield = LinearInterpolation(bonds, maturity);
                smoothCurve.Add(new Bond 
                { 
                    Name = $"{maturity:F1}Y Interpolated", 
                    MaturityYears = maturity, 
                    Yield = interpolatedYield 
                });
            }

            // Ensure we include the final maturity if it wasn't included in the loop
            if (Math.Abs(smoothCurve.Last().MaturityYears - maxMaturity) > 1e-9)
            {
                var finalYield = LinearInterpolation(bonds, maxMaturity);
                smoothCurve.Add(new Bond 
                { 
                    Name = $"{maxMaturity:F1}Y Interpolated", 
                    MaturityYears = maxMaturity, 
                    Yield = finalYield 
                });
            }

            return smoothCurve;
        }
    }
}
