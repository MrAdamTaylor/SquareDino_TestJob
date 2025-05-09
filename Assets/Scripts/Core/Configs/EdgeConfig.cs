using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Edge", menuName = "Edge")]
public class EdgeConfig : ScriptableObject
{
    public List<EdgeConfig> NeighbourEdges;

    public List<string> Way;
}
