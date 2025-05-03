using UnityEngine;

namespace Core.Configs
{
    [CreateAssetMenu(fileName = "Player", menuName = "Player")]
    public class PlayerConfig : ScriptableObject
    {
        public int Speed;
    }
}