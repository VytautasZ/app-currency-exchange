using CurrencyExchange.Shared.Extencions;

namespace CurrencyExchange.Shared.UnitTests;

public class DecimalExtensionsTests
{
    [Theory]
    [InlineData(743.9400, 2, 743.94)]
    [InlineData(1245.296283, 2, 1245.30)]
    [InlineData(1.146903, 2, 1.15)]
    [InlineData(0.134419, 2, 0.13)]
    public void RoundToGivenDecimalPlaces_IfValueHasMoreDecimals_RoundsToGivenPlaces(decimal value, int decimalPlaces, decimal expectedResult)
    {
        //Arange

        //Act
        var result = value.RoundToGivenDecimalPlaces(decimalPlaces);

        //Assert
        Assert.Equal(expectedResult, result);
    }

    [Theory]
    [InlineData(2.345, 2, 2.35)]
    [InlineData(2.355, 2, 2.36)]
    [InlineData(0.125, 2, 0.13)]
    [InlineData(1.005, 2, 1.01)]
    public void RoundToGivenDecimalPlaces_IfValueIsExactlyHalfway_RoundsAwayFromZero(decimal value, int decimalPlaces, decimal expectedResult)
    {
        //Arange

        //Act
        var result = value.RoundToGivenDecimalPlaces(decimalPlaces);

        //Assert
        Assert.Equal(expectedResult, result);
    }

    [Theory]
    [InlineData(-2.345, 2, -2.35)]
    [InlineData(-1.146903, 2, -1.15)]
    [InlineData(-0.125, 2, -0.13)]
    public void RoundToGivenDecimalPlaces_IfValueIsNegative_RoundsAwayFromZero(decimal value, int decimalPlaces, decimal expectedResult)
    {
        //Arange

        //Act
        var result = value.RoundToGivenDecimalPlaces(decimalPlaces);

        //Assert
        Assert.Equal(expectedResult, result);
    }

    [Theory]
    [InlineData(1245.296283, 0, 1245)]
    [InlineData(1245.5, 0, 1246)]
    [InlineData(0.059740, 6, 0.059740)]
    [InlineData(7.4394, 4, 7.4394)]
    public void RoundToGivenDecimalPlaces_IfDecimalPlacesVary_RoundsToRequestedPrecision(decimal value, int decimalPlaces, decimal expectedResult)
    {
        //Arange

        //Act
        var result = value.RoundToGivenDecimalPlaces(decimalPlaces);

        //Assert
        Assert.Equal(expectedResult, result);
    }

    [Theory]
    [InlineData(100, 2)]
    [InlineData(0, 2)]
    [InlineData(-50, 2)]
    public void RoundToGivenDecimalPlaces_IfValueHasNoFractionalPart_ReturnsSameValue(decimal value, int decimalPlaces)
    {
        //Arange

        //Act
        var result = value.RoundToGivenDecimalPlaces(decimalPlaces);

        //Assert
        Assert.Equal(value, result);
    }

    [Fact]
    public void RoundToGivenDecimalPlaces_IfDecimalPlacesIsNegative_Throws()
    {
        //Arange
        var value = 1.234m;

        //Act
        var exception = Record.Exception(() => value.RoundToGivenDecimalPlaces(-1));

        //Assert
        Assert.IsType<ArgumentOutOfRangeException>(exception);
    }

    [Fact]
    public void RoundToGivenDecimalPlaces_IfDecimalPlacesExceedsMaximum_Throws()
    {
        //Arange
        var value = 1.234m;

        //Act
        var exception = Record.Exception(() => value.RoundToGivenDecimalPlaces(29));

        //Assert
        Assert.IsType<ArgumentOutOfRangeException>(exception);
    }
}
