using SideLoader;
using UnityEngine;

namespace PopulatedWorld
{
    public static class DialogEffectLogger
    {
        private const string PREFIX = "[DialogEffect]";

        public static void Log(string message)
        {
            Debug.Log($"{PREFIX} {message}");
        }

        public static void Error(string message)
        {
            Debug.LogError($"{PREFIX} {message}");
        }

        public static void LogEffectTransform(SL_EffectTransform transform, int index)
        {
            if (transform == null)
            {
                Error($"Effect transform at index {index} is null");
                return;
            }

            Log($"Transform {index}: {transform.TransformName}");
            Log($"- Effects: {transform.Effects?.Length ?? 0}");
            Log($"- Conditions: {transform.EffectConditions?.Length ?? 0}");
            Log($"- Children: {transform.ChildEffects?.Length ?? 0}");
        }
    }
}
