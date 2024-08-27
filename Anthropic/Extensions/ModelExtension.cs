using Betalgo.Anthropic.ApiModels;

namespace Betalgo.Anthropic.Extensions;

internal static class ModelExtension
{
    internal static void ProcessModelId(this IObjectInterfaces.IModel modelFromObject, string? defaultModelId, bool allowNull = false)
    {
        if (allowNull)
        {
            modelFromObject.Model ??= defaultModelId;
        }
        else
        {
            modelFromObject.Model ??= defaultModelId ?? throw new ArgumentNullException("Model Id");
        }
    }
}