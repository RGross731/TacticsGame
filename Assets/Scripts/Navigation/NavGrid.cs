using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[System.Serializable]
public class NavGrid : MonoBehaviour
{
    private const float voxelSize = 1f;
    private const float halfVoxelSize = 0.5f;
    private const float voxelSkinWidth = 0.1f;
    private const float stepHeight = 0.2f;
    private const float maxWalkableAngle = 30f;
    private readonly string[] ignoreLayers = { "Agent" };

    
    [SerializeField]
    [HideInInspector]
    private AStarPathfinder pathfinder = new AStarPathfinder();

    public List<NavTile> FindPath(NavTile start, NavTile goal)
    {
        return pathfinder.FindPath(start, goal);
    }

    public void Bake()
    {
        Clear();
        
        //Calcuate environment extents
        Vector3 min = new Vector3(float.MaxValue, float.MaxValue, float.MaxValue);
        Vector3 max = new Vector3(float.MinValue, float.MinValue, float.MinValue);
        foreach (Collider c in FindObjectsOfType<Collider>())
        {
            min = Vector3.Min(min, c.bounds.min);
            max = Vector3.Max(max, c.bounds.max);
        }

        //Generate nav tiles
        for (int y = Mathf.RoundToInt(min.y); y < Mathf.RoundToInt(max.y) + voxelSize; y++)
        {
            for (int z = Mathf.RoundToInt(min.z); z < Mathf.RoundToInt(max.z); z++)
            {
                for (int x = Mathf.RoundToInt(min.x); x < Mathf.RoundToInt(max.x); x++)
                {
                    if (EmptyVoxel(new Vector3(x, y, z)) && VoxelGrounded(new Vector3(x, y, z)))
                    {
                        GameObject navTile = new GameObject("NavTile", typeof(NavTile));
                        navTile.transform.SetParent(transform);
                        navTile.transform.position = new Vector3(x + halfVoxelSize, y, z + halfVoxelSize);
                        navTile.layer = LayerMask.NameToLayer("NavGrid");
                        navTile.hideFlags = HideFlags.HideInHierarchy;

                        BoxCollider c = navTile.GetComponent<BoxCollider>();
                        c.isTrigger = true;
                        c.center = Vector3.zero;
                        c.size = new Vector3(voxelSize, 0, voxelSize);
                    }
                }
            }
        }

        //Create graph
        foreach (NavTile t in GetComponentsInChildren<NavTile>())
        {
            t.Edges = new List<NavTile.Edge>();
            foreach (Collider c in Physics.OverlapBox(t.transform.position, new Vector3(voxelSize, stepHeight, voxelSize), Quaternion.identity, LayerMask.GetMask("NavGrid")))
            {
                Vector3 o = t.transform.position + Vector3.up * halfVoxelSize;
                Vector3 d = c.transform.position - t.transform.position;
                if (c.GetComponent<NavTile>() != t && EmptyVoxelSweep(o, d))
                {
                    t.Edges.Add(new NavTile.Edge(c.GetComponent<NavTile>(), Mathf.Round(Vector3.Distance(t.transform.position, c.transform.position) * 2) / 2));
                }                
            } 
        }
    }

    public void Clear()
    {
        foreach (NavTile t in GetComponentsInChildren<NavTile>())
        {
            DestroyImmediate(t.gameObject);
        }
    }

    private bool EmptyVoxel(Vector3 o)
    {
        return Physics.OverlapBox( 
            new Vector3(o.x + halfVoxelSize, o.y + halfVoxelSize, o.z + halfVoxelSize),
            new Vector3(halfVoxelSize - voxelSkinWidth, halfVoxelSize - voxelSkinWidth, halfVoxelSize - voxelSkinWidth),
            Quaternion.identity, 
            ~LayerMask.GetMask(ignoreLayers)
        )
        .Length == 0;
    }

    private bool VoxelGrounded(Vector3 o)
    {
        return Physics.Raycast(    
            new Vector3(o.x + halfVoxelSize, o.y + halfVoxelSize, o.z + halfVoxelSize),
            Vector3.down, 
            out RaycastHit hitInfo, 
            voxelSize, 
            ~LayerMask.GetMask(ignoreLayers)
        ) 
        && Vector3.Angle(Vector3.up, hitInfo.normal) < maxWalkableAngle;
    }

    private bool EmptyVoxelSweep(Vector3 o, Vector3 d)
    {
        return !Physics.BoxCast(
            o,
            new Vector3(halfVoxelSize - voxelSkinWidth, halfVoxelSize - voxelSkinWidth, halfVoxelSize - voxelSkinWidth),
            d,
            Quaternion.identity,
            d.magnitude,
            ~LayerMask.GetMask(ignoreLayers));
    }
}
