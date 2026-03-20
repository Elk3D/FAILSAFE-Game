using UnityEngine;

namespace FAILSAFE.Utilities
{
    /// <summary>
    /// Handy extension methods used throughout the project.
    /// </summary>
    public static class Extensions
    {
        // Transform
        public static void DestroyChildren(this Transform parent)
        {
            foreach (Transform child in parent)
                Object.Destroy(child.gameObject);
        }

        // Float
        public static float Remap(this float value, float fromMin, float fromMax, float toMin, float toMax)
            => Mathf.Lerp(toMin, toMax, Mathf.InverseLerp(fromMin, fromMax, value));

        // GameObject
        public static T GetOrAddComponent<T>(this GameObject go) where T : Component
            => go.TryGetComponent(out T comp) ? comp : go.AddComponent<T>();

        // String
        public static bool IsNullOrEmpty(this string s) => string.IsNullOrEmpty(s);
    }
}
