using System.Runtime.CompilerServices;
using Anthropic.ApiModels.RequestModels;
using Anthropic.ApiModels.ResponseModels;
using Anthropic.Extensions;

namespace Anthropic.Services;

public partial class AnthropicService : IMessagesService
{
    public IMessagesService Messages => this;

    /// <inheritdoc />
    public async Task<MessageResponse> Create(MessageRequest request, CancellationToken cancellationToken = default)
    {
        request.ProcessModelId(_defaultModelId);
        return await _httpClient.PostAndReadAsAsync<MessageResponse>(_endpointProvider.CreateMessage(), request, cancellationToken);
    }

    /// <inheritdoc />
    public async IAsyncEnumerable<IStreamResponse> CreateAsStream(MessageRequest request, [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        // Mark the request as streaming
        request.Stream = true;

        request.ProcessModelId(_defaultModelId);

        using var response = _httpClient.PostAsStreamAsync(_endpointProvider.CreateMessage(), request, cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            yield return await response.HandleResponseContent<BaseResponse>(cancellationToken);
            yield break;
        }

        await foreach (var streamResponse in response.AsStream(cancellationToken)) yield return streamResponse;
    }
}

/// <summary>
///     Send a structured list of input messages with text and/or image content, and the model will generate the next
///     message in the conversation.
///     The Messages API can be used for either single queries or stateless multi-turn conversations.
/// </summary>
public interface IMessagesService
{
    /// <summary>
    ///     Send a structured list of input messages with text and/or image content, and the model will generate the next
    ///     message in the conversation.
    ///     The Messages API can be used for either single queries or stateless multi-turn conversations.
    /// </summary>
    /// <param name="request"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public Task<MessageResponse> Create(MessageRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    ///     Send a structured list of input messages with text and/or image content, and the model will generate the next
    ///     message in the conversation.
    ///     The Messages API can be used for either single queries or stateless multi-turn conversations.
    /// </summary>
    /// <param name="request"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public IAsyncEnumerable<IStreamResponse> CreateAsStream(MessageRequest request, [EnumeratorCancellation] CancellationToken cancellationToken = default);
}