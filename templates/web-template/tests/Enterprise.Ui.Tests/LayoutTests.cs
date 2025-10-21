using FluentAssertions;
using Xunit;
using System.Linq;

namespace Enterprise.Ui.Tests;

/// <summary>
/// Basic unit tests demonstrating test infrastructure setup
/// </summary>
public class BasicTests
{
    [Fact]
    public void Test_Infrastructure_Should_Work()
    {
        // Arrange
        var value = "Hello World";

        // Act & Assert
        value.Should().NotBeNull();
        value.Should().Be("Hello World");
    }

    [Fact]
    public void String_Operations_Should_Work_As_Expected()
    {
        // Arrange
        var input = "Enterprise UI Template";

        // Act
        var result = input.ToUpperInvariant().Replace(" ", "_");

        // Assert
        result.Should().Be("ENTERPRISE_UI_TEMPLATE");
    }

    [Fact]
    public void Collections_Should_Support_LINQ_Operations()
    {
        // Arrange
        var numbers = new[] { 1, 2, 3, 4, 5 };

        // Act
        var evenNumbers = numbers.Where(n => n % 2 == 0).ToArray();

        // Assert
        evenNumbers.Should().HaveCount(2);
        evenNumbers.Should().BeEquivalentTo(new[] { 2, 4 });
    }
}
