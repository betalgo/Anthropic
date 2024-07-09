using System;
using Anthropic.ObjectModels;

namespace Anthropic.Extensions;

public static class ModelExtension
{
    public static void ProcessModelId(this IObjectModels.IModel modelFromObject, string? defaultModelId, bool allowNull = false)
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