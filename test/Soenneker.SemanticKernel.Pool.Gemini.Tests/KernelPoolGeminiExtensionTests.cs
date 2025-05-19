using Soenneker.Tests.FixturedUnit;
using Xunit;

namespace Soenneker.SemanticKernel.Pool.Gemini.Tests;

[Collection("Collection")]
public class KernelPoolGeminiExtensionTests : FixturedUnitTest
{
    public KernelPoolGeminiExtensionTests(Fixture fixture, ITestOutputHelper output) : base(fixture, output)
    {
    }

    [Fact]
    public void Default()
    {

    }
}
