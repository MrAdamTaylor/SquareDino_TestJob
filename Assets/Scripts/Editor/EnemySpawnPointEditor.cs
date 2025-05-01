using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(EnemySpawnPoint))]
public class EnemySpawnPointEditor : Editor
{
    public const float DEBUG_RADIUS = 0.5f;
        
    [DrawGizmo(GizmoType.Active | GizmoType.Pickable | GizmoType.NonSelected)]
    public static void RenderCustomGizmo(EnemySpawnPoint spawnPoint, GizmoType gizmo)
    {
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(spawnPoint.transform.position, DEBUG_RADIUS);
    }
}
