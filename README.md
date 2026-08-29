[![](https://img.shields.io/nuget/v/soenneker.semantickernel.pool.gemini.svg?style=for-the-badge)](https://www.nuget.org/packages/soenneker.semantickernel.pool.gemini/)
[![](https://img.shields.io/github/actions/workflow/status/soenneker/soenneker.semantickernel.pool.gemini/publish-package.yml?style=for-the-badge)](https://github.com/soenneker/soenneker.semantickernel.pool.gemini/actions/workflows/publish-package.yml)
[![](https://img.shields.io/nuget/dt/soenneker.semantickernel.pool.gemini.svg?style=for-the-badge)](https://www.nuget.org/packages/soenneker.semantickernel.pool.gemini/)
[![](https://img.shields.io/github/actions/workflow/status/soenneker/soenneker.semantickernel.pool.gemini/codeql.yml?label=CodeQL&style=for-the-badge)](https://github.com/soenneker/soenneker.semantickernel.pool.gemini/actions/workflows/codeql.yml)

# Soenneker.SemanticKernel.Pool.Gemini

Provides Gemini-specific registration extensions for KernelPoolManager, enabling integration with local LLMs via Semantic Kernel.

## Install

```bash
dotnet add package Soenneker.SemanticKernel.Pool.Gemini
```

## Quick start

```csharp
using Soenneker.SemanticKernel.Pool.Gemini;

ISemanticKernelPool pool = /* obtain from your application */;
await pool.AddGemini("value", "value", /* supply type */ default!, "value", "value", "value", /* supply httpClientCache */ default!, 1, 1, 1, default);
```

Registers a Gemini model in the kernel pool with optional rate and token limits.

## What you get

- `SemanticKernelPoolGeminiExtension` — Provides Gemini-specific registration extensions for KernelPoolManager, enabling integration with local LLMs via Semantic Kernel.

## API at a glance

| API | What it does | Result / important behavior |
| --- | --- | --- |
| `SemanticKernelPoolGeminiExtension.AddGemini(pool, poolId, key, type, modelId, apiKey, endpoint, httpClientCache, rps, rpm, rpd, tokensPerDay, cancellationToken)` | Registers a Gemini model in the kernel pool with optional rate and token limits. | A `ValueTask` representing the asynchronous registration operation. |
| `SemanticKernelPoolGeminiExtension.RemoveGemini(pool, poolId, key, httpClientCache, cancellationToken)` | Unregisters a Gemini model from the kernel pool and removes associated HTTP client and kernel cache entries. | A `ValueTask` representing the asynchronous unregistration operation. |

## Practical notes

- Cancellation stops pending work; it does not undo work that has already completed.
