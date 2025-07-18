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

    }

}
