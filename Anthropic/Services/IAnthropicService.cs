namespace Anthropic.Services;

public interface IAnthropicService
{
    public IMessagesService Messages { get; }

    /// <summary>
    ///     Set default model
    /// </summary>
    /// <param name="modelId"></param>
    void SetDefaultModelId(string modelId);
}