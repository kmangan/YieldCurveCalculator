using YieldCurveCalculator.Models;
using System.Collections.Generic;
using System.Linq;

namespace YieldCurveCalculator.Services
{
    public class YieldCurveService
    {
        public List<Bond> GenerateSampleCurve()
        {
            return new List<Bond>
            {
                new Bond { Name = "2Y Bond", MaturityYears = 2, Yield = 1.8 },
                new Bond { Name = "5Y Bond", MaturityYears = 5, Yield = 2.2 },
                new Bond { Name = "10Y Bond", MaturityYears = 10, Yield = 2.5 }
            };
        }

        public double AverageYield(List<Bond> bonds)
        {
            return bonds.Average(b => b.Yield);
        }
    }
}
