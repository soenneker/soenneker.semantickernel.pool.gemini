using Soenneker.Tests.HostedUnit;

namespace Soenneker.SemanticKernel.Pool.Gemini.Tests;

[ClassDataSource<Host>(Shared = SharedType.PerTestSession)]
public class KernelPoolGeminiExtensionTests : HostedUnitTest
{
    public KernelPoolGeminiExtensionTests(Host host) : base(host)
    {
    }

    [Test]
    public void Default()
    {

    }
}
