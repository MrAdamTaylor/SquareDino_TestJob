using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(PathSpawnPoint))]
public class PathSpawnPointEditor : MonoBehaviour
{
    public const float DEBUG_RADIUS = 0.5f;
        
    [DrawGizmo(GizmoType.Active | GizmoType.Pickable | GizmoType.NonSelected)]
    public static void RenderCustomGizmo(PathSpawnPoint spawnPoint, GizmoType gizmo)
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawSphere(spawnPoint.transform.position, DEBUG_RADIUS);
    }
}
