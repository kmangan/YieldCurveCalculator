namespace YieldCurveCalculator.Models
{
    public class Bond
    {
        public string Name { get; set; } = string.Empty;
        public double MaturityYears { get; set; }
        public double Yield { get; set; }
    }
}
