using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(NavGrid))]
public class NavGridEditor : Editor
{
    private bool creating;
    private Vector3 dragStart;
    private Vector3 dragEnd;

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        NavGrid navGrid = target as NavGrid;
        if (GUILayout.Button("Create NavTiles"))
        {
            creating = true;
        }

        if (GUILayout.Button("Clear NavGrid"))
        {
            navGrid.Clear();
        }
    }

    private void OnSceneGUI()
    {
        HandleUtility.AddDefaultControl(GUIUtility.GetControlID(FocusType.Passive));

        if (creating)
        {
            if (Event.current.type == EventType.MouseDown && Event.current.button == 1)
            {
                creating = false;
                return;
            }

            if (Event.current.type == EventType.MouseDown && Event.current.button == 0)
            {
                if (Physics.SphereCast(HandleUtility.GUIPointToWorldRay(Event.current.mousePosition), 0.25f, out RaycastHit hitInfo))
                {
                    dragStart = hitInfo.point;
                    dragStart.x = Mathf.RoundToInt(dragStart.x);
                    dragStart.y = Mathf.RoundToInt(dragStart.y);
                    dragStart.z = Mathf.RoundToInt(dragStart.z);
                } else
                {
                    creating = false;
                    return;
                }
            }

            if (Event.current.type == EventType.MouseDrag /*&& Event.current.button == 0*/)
            {
                if (Physics.SphereCast(HandleUtility.GUIPointToWorldRay(Event.current.mousePosition), 0.25f, out RaycastHit hitInfo))
                {
                    dragEnd = hitInfo.point;
                    dragEnd.x = Mathf.RoundToInt(dragEnd.x);
                    dragEnd.y = Mathf.RoundToInt(dragEnd.y);
                    dragEnd.z = Mathf.RoundToInt(dragEnd.z);
                }
            }

            if (Event.current.type == EventType.MouseUp && Event.current.button == 0)
            {
                if (Physics.SphereCast(HandleUtility.GUIPointToWorldRay(Event.current.mousePosition), 0.25f, out RaycastHit hitInfo))
                {
                    dragEnd = hitInfo.point;
                    dragEnd.x = Mathf.RoundToInt(dragEnd.x);
                    dragEnd.y = Mathf.RoundToInt(dragEnd.y);
                    dragEnd.z = Mathf.RoundToInt(dragEnd.z);

                    NavGrid navGrid = target as NavGrid;
                    navGrid.Build(dragStart, dragEnd);
                }

                dragStart = Vector3.zero;
                dragEnd = Vector3.zero;
            }

            /*
            if (dragStart != Vector3.zero && dragEnd != Vector3.zero)
            {
                Handles.color = Gizmos.color = new Color(1f, 0.75f, 0.5f, 1f);
                Vector3[] lineSegments = { dragStart, dragEnd };
                Handles.DrawDottedLines(lineSegments, 1);
            }
            */
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
        }
    }
}