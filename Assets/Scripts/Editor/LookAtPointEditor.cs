using Core.SpawnPoints;
using UnityEditor;
using UnityEngine;

namespace Editor
{
    [CustomEditor(typeof(LookAtPoint))]
    public class LookAtPointEditor : UnityEditor.Editor
    {
        public const float DEBUG_RADIUS = 0.5f;
        
        [DrawGizmo(GizmoType.Active | GizmoType.Pickable | GizmoType.NonSelected)]
        public static void RenderCustomGizmo(LookAtPoint spawnPoint, GizmoType gizmo)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawSphere(spawnPoint.transform.position, DEBUG_RADIUS);
        }
    }
}