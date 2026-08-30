using Microsoft.SemanticKernel;
using Soenneker.Dtos.HttpClientOptions;
using Soenneker.Extensions.ValueTask;
using Soenneker.SemanticKernel.Dtos.Options;
using Soenneker.Utils.HttpClientCache.Abstract;
using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Soenneker.SemanticKernel.Enums.KernelType;
using Soenneker.SemanticKernel.Pool.Abstract;

namespace Soenneker.SemanticKernel.Pool.Gemini;

/// <summary>
/// Provides Gemini connector registration extensions for <see cref="ISemanticKernelPool"/>.
/// </summary>
public static class SemanticKernelPoolGeminiExtension
{
    /// <summary>
    /// Registers a Gemini model in the kernel pool with optional rate and token limits.
    /// </summary>
    /// <param name="pool">The kernel pool manager to register the model with.</param>
    /// <param name="poolId">The sub-pool that receives the entry.</param>
    /// <param name="key">A unique identifier used to register and later reference the model.</param>
    /// <param name="type">The connector type. Chat and embedding are supported.</param>
    /// <param name="modelId">The Gemini model ID to be used for chat completion.</param>
    /// <param name="apiKey">The Google AI API key.</param>
    /// <param name="endpoint">Endpoint metadata stored with the entry. This adapter does not pass it to the Google connector.</param>
    /// <param name="httpClientCache">An HTTP client cache used to manage reusable <see cref="HttpClient"/> instances.</param>
    /// <param name="rps">Optional maximum number of requests allowed per second.</param>
    /// <param name="rpm">Optional maximum number of requests allowed per minute.</param>
    /// <param name="rpd">Optional maximum number of requests allowed per day.</param>
    /// <param name="tokensPerDay">Optional maximum number of tokens allowed per day.</param>
    /// <param name="cancellationToken">A cancellation token that can be used to cancel the operation.</param>
    /// <returns>A <see cref="ValueTask"/> representing the asynchronous registration operation.</returns>
    public static ValueTask AddGemini(this ISemanticKernelPool pool, string poolId, string key, KernelType type, string modelId, string apiKey, string endpoint,
        IHttpClientCache httpClientCache, int? rps, int? rpm, int? rpd, int? tokensPerDay = null, CancellationToken cancellationToken = default)
    {
        var options = new SemanticKernelOptions
        {
            Type = type,
            ModelId = modelId,
            Endpoint = endpoint,
            ApiKey = apiKey,
            RequestsPerSecond = rps,
            RequestsPerMinute = rpm,
            RequestsPerDay = rpd,
            TokensPerDay = tokensPerDay,
            KernelFactory = async (opts, factoryCancellationToken) =>
            {
                HttpClient httpClient = await httpClientCache.Get($"gemini:{poolId}:{key}", static () => new HttpClientOptions
                {
                    Timeout = TimeSpan.FromSeconds(300)
                }, factoryCancellationToken)
                .NoSync();

#pragma warning disable SKEXP0070
                return opts.Type switch
                {
                    var t when t == KernelType.Chat => Kernel.CreateBuilder()
                                                             .AddGoogleAIGeminiChatCompletion(opts.ModelId!, opts.ApiKey!, httpClient: httpClient),

                    var t when t == KernelType.Embedding => Kernel.CreateBuilder().AddGoogleAIEmbeddingGenerator(opts.ModelId!, opts.ApiKey!),

                    _ => throw new NotSupportedException($"Unsupported kernel type: {opts.Type}")
                };
#pragma warning restore SKEXP0070
            }
        };

        return pool.Add(poolId, key, options, cancellationToken);
    }

    /// <summary>
    /// Unregisters a Gemini model from the kernel pool and removes associated HTTP client and kernel cache entries.
    /// </summary>
    /// <param name="pool">The kernel pool manager to unregister the model from.</param>
    /// <param name="poolId">The sub-pool containing the entry.</param>
    /// <param name="key">The unique identifier used during registration.</param>
    /// <param name="httpClientCache">The HTTP client cache to remove the associated client from.</param>
    /// <param name="cancellationToken">A cancellation token that can be used to cancel the operation.</param>
    /// <returns>A <see cref="ValueTask"/> representing the asynchronous unregistration operation.</returns>
    public static async ValueTask RemoveGemini(this ISemanticKernelPool pool, string poolId, string key, IHttpClientCache httpClientCache,
        CancellationToken cancellationToken = default)
    {
        await pool.Remove(poolId, key, cancellationToken).NoSync();
        await httpClientCache.Remove($"gemini:{poolId}:{key}").NoSync();
    }
}
