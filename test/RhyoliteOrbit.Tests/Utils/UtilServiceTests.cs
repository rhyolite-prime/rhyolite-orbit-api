using RhyoliteOrbit.Utils;
using Xunit;

namespace RhyoliteOrbit.Tests.Utils;

public class UtilServiceTests
{
    private readonly UtilService _utilService = new UtilService();

    [Theory]
    [InlineData("0", "Zero")]
    [InlineData("-123", "Minus One Hundred Twenty Three")]
    [InlineData("123", "One Hundred Twenty Three")]
    [InlineData("123.45", "One Hundred Twenty Three and Fourty Five Pesewa(s)")]
    [InlineData("1000", "One Thousand")]
    [InlineData("InvalidInput", "Invalid number")]
    public void ConvertToWords_ShouldReturnExpectedResult(string input, string expected)
    {
        // Act
        var result = _utilService.ConvertToWords(input);

        // Assert
        Assert.Equal(expected, result);
    }
}
