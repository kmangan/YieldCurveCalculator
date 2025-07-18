# Let's calculate the expected values manually
# Test bonds: 2Y@1.8%, 5Y@2.2%, 10Y@2.5%

# For 12Y (extrapolation above using last two points: 5Y and 10Y):
# y = 2.2 + (12 - 5) * (2.5 - 2.2) / (10 - 5)
# y = 2.2 + 7 * 0.3 / 5
# y = 2.2 + 0.42 = 2.62
print("12Y expected:", 2.2 + (12 - 5) * (2.5 - 2.2) / (10 - 5))

# For 0.5Y (extrapolation below using first two points: 2Y and 5Y):
# y = 1.8 + (0.5 - 2) * (2.2 - 1.8) / (5 - 2)
# y = 1.8 + (-1.5) * 0.4 / 3
# y = 1.8 - 0.2 = 1.6
print("0.5Y expected:", 1.8 + (0.5 - 2) * (2.2 - 1.8) / (5 - 2))

# For 3Y (interpolation between 2Y and 5Y):
# y = 1.8 + (3 - 2) * (2.2 - 1.8) / (5 - 2)
# y = 1.8 + 1 * 0.4 / 3
# y = 1.8 + 0.1333 = 1.9333
print("3Y expected:", 1.8 + (3 - 2) * (2.2 - 1.8) / (5 - 2))

# For 7.5Y (interpolation between 5Y and 10Y):
# y = 2.2 + (7.5 - 5) * (2.5 - 2.2) / (10 - 5)
# y = 2.2 + 2.5 * 0.3 / 5
# y = 2.2 + 0.15 = 2.35
print("7.5Y expected:", 2.2 + (7.5 - 5) * (2.5 - 2.2) / (10 - 5))
