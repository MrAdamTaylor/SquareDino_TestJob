using System.Collections.Generic;
using UnityEngine;

namespace Extensions
{
    public static class TransformExtensions
    {
        public static List<Transform> GetAllChildren(this Transform parent)
        {
            List<Transform> children = new List<Transform>();

            for (int i = 0; i < parent.childCount; i++)
            {
                children.Add(parent.GetChild(i));
            }

            return children;
        }
    }
}