using FluentAssertions;

namespace MarlinPrintMiddleware.Integration.Tests;

public class PlaceholderTests
{
    [Fact]
    public void Project_Loads() => true.Should().BeTrue();
}
