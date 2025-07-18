# Yield Curve Calculator with Linear Interpolation

A C# console application that demonstrates yield curve calculations and linear interpolation for bond pricing.

## Features

- Bond data model with maturity and yield properties
- Linear interpolation service for calculating yields at any maturity
- Supports interpolation between existing points and extrapolation beyond the curve
- Batch processing for multiple maturity points
- Smooth curve generation at regular intervals

## Architecture

The application uses a dedicated `InterpolationService` class that handles all interpolation logic using the standard linear interpolation formula:

```
y = y1 + (x - x1) * (y2 - y1) / (x2 - x1)
```

## Usage

```csharp
var interpolationService = new InterpolationService();
var bonds = service.GenerateSampleCurve(); // 2Y@1.8%, 5Y@2.2%, 10Y@2.5%

// Interpolate yield for 3.5Y maturity
double yield = interpolationService.LinearInterpolation(bonds, 3.5);
// Returns: 2.00%
```

## Running the Application

```bash
dotnet run
```

## Running Tests

To run the unit tests:

```bash
dotnet test
```

For more detailed test output:

```bash
dotnet test --verbosity detailed
```

## Requirements

- .NET 9.0 or later
