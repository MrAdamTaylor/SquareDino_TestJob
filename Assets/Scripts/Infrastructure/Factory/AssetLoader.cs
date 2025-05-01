using UnityEngine;

public class AssetLoader : IAssetLoader
{
    public GameObject LoadPrefab(string path)
    {
        var prefab = Resources.Load<GameObject>(path);
        if (prefab == null)
        {
            Debug.LogError($"Not found prefab by path '{path}'");
            return null;
        }
        return prefab;
    }
}