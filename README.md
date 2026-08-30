[![](https://img.shields.io/nuget/v/soenneker.semantickernel.pool.gemini.svg?style=for-the-badge)](https://www.nuget.org/packages/soenneker.semantickernel.pool.gemini/)
[![](https://img.shields.io/github/actions/workflow/status/soenneker/soenneker.semantickernel.pool.gemini/publish-package.yml?style=for-the-badge)](https://github.com/soenneker/soenneker.semantickernel.pool.gemini/actions/workflows/publish-package.yml)
[![](https://img.shields.io/nuget/dt/soenneker.semantickernel.pool.gemini.svg?style=for-the-badge)](https://www.nuget.org/packages/soenneker/soenneker.semantickernel.pool.gemini/)
[![](https://img.shields.io/github/actions/workflow/status/soenneker/soenneker.semantickernel.pool.gemini/codeql.yml?label=CodeQL&style=for-the-badge)](https://github.com/soenneker/soenneker.semantickernel.pool.gemini/actions/workflows/codeql.yml)

# Soenneker.SemanticKernel.Pool.Gemini

Gemini connector registration helpers for `Soenneker.SemanticKernel.Pool`.

## Installation

```bash
dotnet add package Soenneker.SemanticKernel.Pool.Gemini
```

## Add a Gemini entry

Resolve the pool and HTTP client cache from dependency injection, then register a chat or embedding entry:

```csharp
using Soenneker.SemanticKernel.Enums.KernelType;
using Soenneker.SemanticKernel.Pool.Abstract;
using Soenneker.SemanticKernel.Pool.Gemini;
using Soenneker.Utils.HttpClientCache.Abstract;

await pool.AddGemini(
    poolId: "chat",
    key: "gemini-primary",
    type: KernelType.Chat,
    modelId: "gemini-model-id",
    apiKey: configuration["Gemini:ApiKey"]!,
    endpoint: "https://generativelanguage.googleapis.com",
    httpClientCache: httpClientCache,
    rps: 2,
    rpm: 60,
    rpd: 1_000,
    tokensPerDay: null,
    cancellationToken);
```

Use `KernelType.Chat` for Google AI chat completion or `KernelType.Embedding` for the Google AI embedding generator. Other kernel types throw `NotSupportedException` when the pool first constructs the kernel.

The adapter caches chat HTTP clients under `gemini:{poolId}:{key}` with a five-minute timeout. The `endpoint` argument is retained in the entry's `SemanticKernelOptions`, but this adapter does not pass it to the Google connector; connector endpoint behavior therefore comes from the Google Semantic Kernel package.

Pool quota values are reservations made when `GetAvailable` selects the entry. `tokensPerDay` counts one unit per acquisition; it is not populated from provider token usage.

## Remove the entry

Use the matching helper so both the pool entry and its cached HTTP client are removed:

```csharp
await pool.RemoveGemini(
    "chat",
    "gemini-primary",
    httpClientCache,
    cancellationToken);
```

Keep the API key in a protected configuration provider and avoid logging or serializing the generated `SemanticKernelOptions`.
