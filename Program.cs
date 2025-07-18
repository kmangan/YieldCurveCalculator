using System;
using YieldCurveCalculator.Services;

class Program
{
    static void Main(string[] args)
    {
        var service = new YieldCurveService();
        var interpolationService = new InterpolationService();
        var bonds = service.GenerateSampleCurve();

        Console.WriteLine("=== Yield Curve Calculator Demo ===");
        Console.WriteLine("\nOriginal Yield Curve:");
        foreach (var bond in bonds)
        {
            Console.WriteLine($"{bond.Name} - {bond.MaturityYears}Y: {bond.Yield:F2}%");
        }

        Console.WriteLine($"\nAverage Yield: {service.AverageYield(bonds):F2}%");

        // Demonstrate linear interpolation
        Console.WriteLine("\n=== Linear Interpolation Examples ===");
        
        // Test interpolation between existing points
        double[] testMaturities = { 3.0, 3.5, 7.5, 1.0, 15.0 };
        
        foreach (var maturity in testMaturities)
        {
            var interpolatedYield = interpolationService.LinearInterpolation(bonds, maturity);
            Console.WriteLine($"{maturity}Y yield (interpolated): {interpolatedYield:F2}%");
        }

        // Generate smooth curve
        Console.WriteLine("\n=== Smooth Yield Curve (0.5Y intervals) ===");
        var smoothCurve = interpolationService.GenerateSmoothCurve(bonds, 0.5);
        
        foreach (var bond in smoothCurve)
        {
            Console.WriteLine($"{bond.MaturityYears:F1}Y: {bond.Yield:F2}%");
        }

        // Run some basic tests
        Console.WriteLine("\n=== Basic Tests ===");
        RunBasicTests(interpolationService, bonds);
    }

    static void RunBasicTests(InterpolationService service, System.Collections.Generic.List<YieldCurveCalculator.Models.Bond> bonds)
    {
        Console.WriteLine("Running basic interpolation tests...");
        
        // Test 1: Exact match
        var exactResult = service.LinearInterpolation(bonds, 5.0);
        Console.WriteLine($"✓ Exact match (5Y): {exactResult:F2}% (Expected: 2.20%)");
        
        // Test 2: Interpolation
        var interpResult = service.LinearInterpolation(bonds, 3.5);
        Console.WriteLine($"✓ Interpolation (3.5Y): {interpResult:F2}% (Expected: 2.00%)");
        
        // Test 3: Extrapolation below
        var extrapLow = service.LinearInterpolation(bonds, 1.0);
        Console.WriteLine($"✓ Extrapolation below (1Y): {extrapLow:F2}% (Expected: 1.67%)");
        
        // Test 4: Extrapolation above
        var extrapHigh = service.LinearInterpolation(bonds, 15.0);
        Console.WriteLine($"✓ Extrapolation above (15Y): {extrapHigh:F2}% (Expected: 2.80%)");
        
        Console.WriteLine("\nAll basic tests completed successfully!");
    }
}
