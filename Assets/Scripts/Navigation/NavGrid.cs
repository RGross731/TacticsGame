using UnityEngine;
using System.Linq;
using System;
using System.Collections.Generic;

[Serializable]
public class NavGrid : MonoBehaviour
{
    [HideInInspector]
    public NavTile navTilePrefab;

    [SerializeField]
    [HideInInspector]
    private readonly AStarPathfinder pathfinder = new AStarPathfinder();


    public List<NavTile> FindPath(NavTile start, NavTile goal)
    {
        return pathfinder.FindPath(start, goal);
    }

    public void Build(Vector3 corner1, Vector3 corner2)
    {
        //Create new nav tiles
        Vector2 zRange = corner1.z < corner2.z ? new Vector2(corner1.z, corner2.z) : new Vector2(corner2.z, corner1.z);
        Vector2 xRange = corner1.x < corner2.x ? new Vector2(corner1.x, corner2.x) : new Vector2(corner2.x, corner1.x);
        float y = corner1.y;

        for (int z = Mathf.RoundToInt(zRange.x); z < Mathf.RoundToInt(zRange.y); z++)
        {
            for (int x = Mathf.RoundToInt(xRange.x); x < Mathf.RoundToInt(xRange.y); x++)
            {
                if (Physics.OverlapBox(new Vector3(x + 0.5f, y + 0.5f, z + 0.5f), new Vector3(0.45f, 0.45f, 0.45f), Quaternion.identity, ~LayerMask.GetMask("Agent")).Length == 0)
                {
                    Instantiate(navTilePrefab, new Vector3(x + 0.5f, y, z + 0.5f), Quaternion.identity, transform);
                }
            }
        }

        //Assign edge information for nav tiles
        foreach (NavTile t in GetComponentsInChildren<NavTile>())
        {
            t.name = "NavTile";

            BoxCollider c = t.GetComponent<BoxCollider>();
            c.isTrigger = true;
            c.center = Vector3.zero;
            c.size = new Vector3(1, 0, 1);
            t.gameObject.hideFlags = HideFlags.HideInHierarchy;


            t.Edges = Physics.OverlapBox(t.transform.position, new Vector3(1f, 0.2f, 1f), Quaternion.identity, LayerMask.GetMask("NavGrid"))
                .Select(col => col.GetComponent<NavTile>()).Where(n => n != null && n != t)
                .Select(n => new NavTile.Edge(n, Mathf.Round(Vector3.Distance(t.transform.position, n.transform.position) * 2) / 2)).ToList();
        }

        //Remove diagonal connections if adjacent connections aren't both existant
        foreach (NavTile t in GetComponentsInChildren<NavTile>())
        {
            foreach (NavTile.Edge diagonalEdge in t.Edges.Where(n => n.weight == 1.5f).ToList())
            {
                if (diagonalEdge.tile.Edges.Select(n => n.tile).Intersect(t.Edges.Select(n => n.tile)).ToList().Count != 2)
                {
                    t.Edges.Remove(diagonalEdge);
                }
            }
        }
    }

    public void Clear()
    {
        //Destroy old nav tiles
        foreach (NavTile t in GetComponentsInChildren<NavTile>())
        {
            DestroyImmediate(t.gameObject);
        }
    }

}