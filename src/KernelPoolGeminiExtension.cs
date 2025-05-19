using Microsoft.SemanticKernel;
using Soenneker.Dtos.HttpClientOptions;
using Soenneker.Extensions.ValueTask;
using Soenneker.SemanticKernel.Dtos.Options;
using Soenneker.Utils.HttpClientCache.Abstract;
using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Soenneker.SemanticKernel.Pool.Abstract;

namespace Soenneker.SemanticKernel.Pool.Gemini;

/// <summary>
/// Provides Gemini-specific registration extensions for KernelPoolManager, enabling integration with local LLMs via Semantic Kernel.
/// </summary>
public static class KernelPoolGeminiExtension
{
    /// <summary>
    /// Registers a Gemini model in the kernel pool with optional rate and token limits.
    /// </summary>
    /// <param name="pool">The kernel pool manager to register the model with.</param>
    /// <param name="key">A unique identifier used to register and later reference the model.</param>
    /// <param name="modelId">The Gemini model ID to be used for chat completion.</param>
    /// <param name="apiKey"></param>
    /// <param name="endpoint">The base URI endpoint for the Gemini service.</param>
    /// <param name="httpClientCache">An HTTP client cache used to manage reusable <see cref="HttpClient"/> instances.</param>
    /// <param name="rps">Optional maximum number of requests allowed per second.</param>
    /// <param name="rpm">Optional maximum number of requests allowed per minute.</param>
    /// <param name="rpd">Optional maximum number of requests allowed per day.</param>
    /// <param name="tokensPerDay">Optional maximum number of tokens allowed per day.</param>
    /// <param name="cancellationToken">A cancellation token that can be used to cancel the operation.</param>
    /// <returns>A <see cref="ValueTask"/> representing the asynchronous registration operation.</returns>
    public static ValueTask RegisterGemini(this IKernelPoolManager pool, string key, string modelId, string apiKey, string endpoint, IHttpClientCache httpClientCache, int? rps,
        int? rpm, int? rpd, int? tokensPerDay = null, CancellationToken cancellationToken = default)
    {
        var options = new SemanticKernelOptions
        {
            ModelId = modelId,
            Endpoint = endpoint,
            RequestsPerSecond = rps,
            RequestsPerMinute = rpm,
            RequestsPerDay = rpd,
            TokensPerDay = tokensPerDay,
            ApiKey = apiKey,
            KernelFactory = async (opts, _) =>
            {
                HttpClient httpClient = await httpClientCache.Get($"gemini:{modelId}", () => new HttpClientOptions
                {
                    Timeout = TimeSpan.FromSeconds(600),
                    BaseAddress = opts.Endpoint
                }, cancellationToken);

#pragma warning disable SKEXP0070
                return Kernel.CreateBuilder().AddGoogleAIGeminiChatCompletion(modelId: opts.ModelId!, apiKey, httpClient: httpClient);
#pragma warning restore SKEXP0070
            }
        };

        return pool.Register(key, options, cancellationToken);
    }

    /// <summary>
    /// Unregisters a Gemini model from the kernel pool and removes associated HTTP client and kernel cache entries.
    /// </summary>
    /// <param name="pool">The kernel pool manager to unregister the model from.</param>
    /// <param name="key">The unique identifier used during registration.</param>
    /// <param name="httpClientCache">The HTTP client cache to remove the associated client from.</param>
    /// <param name="cancellationToken">A cancellation token that can be used to cancel the operation.</param>
    /// <returns>A <see cref="ValueTask"/> representing the asynchronous unregistration operation.</returns>
    public static async ValueTask UnregisterGemini(this IKernelPoolManager pool, string key, IHttpClientCache httpClientCache, CancellationToken cancellationToken = default)
    {
        await pool.Unregister(key, cancellationToken).NoSync();
        await httpClientCache.Remove($"gemini:{key}", cancellationToken).NoSync();
    }
}