using System;
using System.Collections.Generic;
using Xunit;
using YieldCurveCalculator.Models;
using YieldCurveCalculator.Services;

namespace YieldCurveCalculator.Tests
{
    public class InterpolationServiceTests
    {
        private readonly InterpolationService _interpolationService;
        private readonly List<Bond> _testBonds;

        public InterpolationServiceTests()
        {
            _interpolationService = new InterpolationService();
            _testBonds = new List<Bond>
            {
                new Bond { Name = "2Y Bond", MaturityYears = 2, Yield = 1.8 },
                new Bond { Name = "5Y Bond", MaturityYears = 5, Yield = 2.2 },
                new Bond { Name = "10Y Bond", MaturityYears = 10, Yield = 2.5 }
            };
        }

        [Fact]
        public void LinearInterpolation_ExactMatch_ReturnsExactYield()
        {
            // Arrange
            double targetMaturity = 5.0;
            double expectedYield = 2.2;

            // Act
            double actualYield = _interpolationService.LinearInterpolation(_testBonds, targetMaturity);

            // Assert
            Assert.Equal(expectedYield, actualYield, 2); // 2 decimal places precision
        }

        [Fact]
        public void LinearInterpolation_InterpolationBetweenPoints_ReturnsCorrectYield()
        {
            // Arrange
            double targetMaturity = 3.5;
            // Expected: 1.8 + (3.5 - 2) * (2.2 - 1.8) / (5 - 2) = 1.8 + 1.5 * 0.4 / 3 = 2.0
            double expectedYield = 2.0;

            // Act
            double actualYield = _interpolationService.LinearInterpolation(_testBonds, targetMaturity);

            // Assert
            Assert.Equal(expectedYield, actualYield, 2);
        }

        [Fact]
        public void LinearInterpolation_ExtrapolationBelowRange_ReturnsCorrectYield()
        {
            // Arrange
            double targetMaturity = 1.0;
            // Expected: 1.8 + (1 - 2) * (2.2 - 1.8) / (5 - 2) = 1.8 - 0.4/3 = 1.67
            double expectedYield = 1.67;

            // Act
            double actualYield = _interpolationService.LinearInterpolation(_testBonds, targetMaturity);

            // Assert
            Assert.Equal(expectedYield, actualYield, 2);
        }

        [Fact]
        public void LinearInterpolation_ExtrapolationAboveRange_ReturnsCorrectYield()
        {
            // Arrange
            double targetMaturity = 15.0;
            // Expected: 2.2 + (15 - 5) * (2.5 - 2.2) / (10 - 5) = 2.2 + 10 * 0.3 / 5 = 2.8
            double expectedYield = 2.8;

            // Act
            double actualYield = _interpolationService.LinearInterpolation(_testBonds, targetMaturity);

            // Assert
            Assert.Equal(expectedYield, actualYield, 2);
        }

        [Fact]
        public void LinearInterpolation_NullBondList_ThrowsArgumentException()
        {
            // Arrange
            List<Bond>? nullBonds = null;
            double targetMaturity = 5.0;

            // Act & Assert
            var exception = Assert.Throws<ArgumentException>(() => 
                _interpolationService.LinearInterpolation(nullBonds!, targetMaturity));
            
            Assert.Contains("Bond list cannot be null or empty", exception.Message);
        }

        [Fact]
        public void LinearInterpolation_EmptyBondList_ThrowsArgumentException()
        {
            // Arrange
            var emptyBonds = new List<Bond>();
            double targetMaturity = 5.0;

            // Act & Assert
            var exception = Assert.Throws<ArgumentException>(() => 
                _interpolationService.LinearInterpolation(emptyBonds, targetMaturity));
            
            Assert.Contains("Bond list cannot be null or empty", exception.Message);
        }

        [Fact]
        public void LinearInterpolation_SingleBond_ThrowsArgumentException()
        {
            // Arrange
            var singleBond = new List<Bond>
            {
                new Bond { Name = "5Y Bond", MaturityYears = 5, Yield = 2.2 }
            };
            double targetMaturity = 3.0;

            // Act & Assert
            var exception = Assert.Throws<ArgumentException>(() => 
                _interpolationService.LinearInterpolation(singleBond, targetMaturity));
            
            Assert.Contains("Cannot interpolate with only one bond", exception.Message);
        }

        [Fact]
        public void InterpolateMultiple_ValidMaturities_ReturnsCorrectResults()
        {
            // Arrange
            double[] maturities = { 1.0, 3.0, 7.5, 12.0 };
            var expectedResults = new Dictionary<double, double>
            {
                { 1.0, 1.67 },
                { 3.0, 1.93 },
                { 7.5, 2.35 },
                { 12.0, 2.62 }  // Corrected value
            };

            // Act
            var actualResults = _interpolationService.InterpolateMultiple(_testBonds, maturities);

            // Assert
            Assert.Equal(expectedResults.Count, actualResults.Count);
            
            foreach (var expected in expectedResults)
            {
                Assert.True(actualResults.ContainsKey(expected.Key));
                Assert.Equal(expected.Value, actualResults[expected.Key], 2);
            }
        }

        [Fact]
        public void GenerateSmoothCurve_ValidParameters_ReturnsCorrectNumberOfPoints()
        {
            // Arrange
            double stepSize = 1.0;
            // From 2Y to 10Y with 1Y steps = 9 points (2,3,4,5,6,7,8,9,10)
            int expectedPointCount = 9;

            // Act
            var smoothCurve = _interpolationService.GenerateSmoothCurve(_testBonds, stepSize);

            // Assert
            Assert.Equal(expectedPointCount, smoothCurve.Count);
            Assert.Equal(2.0, smoothCurve[0].MaturityYears);
            Assert.Equal(10.0, smoothCurve[^1].MaturityYears);
        }

        [Fact]
        public void GenerateSmoothCurve_NullBondList_ThrowsArgumentException()
        {
            // Arrange
            List<Bond>? nullBonds = null;

            // Act & Assert
            var exception = Assert.Throws<ArgumentException>(() => 
                _interpolationService.GenerateSmoothCurve(nullBonds!));
            
            Assert.Contains("Bond list cannot be null or empty", exception.Message);
        }

        [Theory]
        [InlineData(2.5, 1.87)] // Between 2Y and 5Y
        [InlineData(7.5, 2.35)] // Between 5Y and 10Y
        [InlineData(0.5, 1.6)]  // Extrapolation below (corrected)
        [InlineData(12.0, 2.62)] // Extrapolation above (corrected)
        public void LinearInterpolation_VariousMaturityPoints_ReturnsExpectedYields(double maturity, double expectedYield)
        {
            // Act
            double actualYield = _interpolationService.LinearInterpolation(_testBonds, maturity);

            // Assert
            Assert.Equal(expectedYield, actualYield, 2);
        }
    }
}
