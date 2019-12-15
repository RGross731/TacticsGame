using System.Linq;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(NavGrid))]
public class NavGridEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        NavGrid navGrid = target as NavGrid;
        if (GUILayout.Button("Bake NavGrid"))
        {
            navGrid.Bake();
        }
    }

    [DrawGizmo(GizmoType.InSelectionHierarchy | GizmoType.Active)]
    private static void DrawGizmo(NavGrid navGrid, GizmoType gizmoType)
    {
        foreach (NavTile t in navGrid.GetComponentsInChildren<NavTile>())
        {
            Gizmos.color = new Color(0.5f, 0.75f, 1f, 0.5f);
            Gizmos.DrawCube(t.transform.position + Vector3.up * 0.001f, t.GetComponent<BoxCollider>().size);
            Gizmos.color = new Color(0.5f, 0.5f, 0.5f, 0.5f);
            Gizmos.DrawWireCube(t.transform.position, t.GetComponent<BoxCollider>().size);
            foreach (NavTile n in t.Edges.Select(e => e.tile).ToList())
            {
                Gizmos.color = Color.red;
                Gizmos.DrawLine(t.transform.position + Vector3.up * 0.002f, n.transform.position + Vector3.up * 0.002f);
            }
        }
    }
}