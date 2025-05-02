using UnityEngine;

namespace Infrastructure.Factory
{
    public interface IAssetLoader
    {
        GameObject LoadPrefab(string path);
    }
}