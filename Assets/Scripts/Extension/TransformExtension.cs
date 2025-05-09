using System.Collections.Generic;
using UnityEngine;

public static class TransformExtensions
{
    public static List<Transform> GetTransformsFromNames(this List<string> names)
    {
        var result = new List<Transform>();
        foreach (var name in names)
        {
            var obj = GameObject.Find(name);
            if (obj != null)
                result.Add(obj.transform);
            else
                Debug.LogWarning($"[TransformExtensions] Object with name '{name}' not found in scene.");
        }
        return result;
    }
}