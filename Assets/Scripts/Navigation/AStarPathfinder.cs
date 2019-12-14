using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[Serializable]
public class AStarPathfinder
{
    public List<NavTile> FindPath(NavTile startNode, NavTile goalNode)
    {
        return FindPath(startNode, goalNode, float.MaxValue);
    }

    public List<NavTile> FindPath(NavTile startNode, NavTile goalNode, float range)
    {
        HashSet<NavTile> closedSet = new HashSet<NavTile>();
        HashSet<NavTile> openSet = new HashSet<NavTile>() { startNode };
        Dictionary<NavTile, float> gCost = new Dictionary<NavTile, float>
        {
            [startNode] = 0
        };
        Dictionary<NavTile, float> fCost = new Dictionary<NavTile, float>
        {
            [startNode] = HeuristicFunction(startNode, goalNode)
        };
        Dictionary<NavTile, NavTile> cameFrom = new Dictionary<NavTile, NavTile>();

        while (openSet.Count > 0)
        {
            NavTile current = openSet.OrderBy(n => fCost[n]).First();

            if (gCost[current] > range)
            {
                return null;
            }

            if (current == goalNode)
            {
                return ReconstructPath(cameFrom, goalNode);
            }

            openSet.Remove(current);
            closedSet.Add(current);

            foreach (NavTile.Edge edge in current.Edges)
            {
                NavTile neighbor = edge.tile;
                float edgeWeight = edge.weight;

                if (closedSet.Contains(neighbor))
                {
                    continue;
                }

                float gCostTemp = gCost[current] + edgeWeight;
                if (!openSet.Contains(neighbor))
                {
                    openSet.Add(neighbor);
                }
                else if (gCostTemp >= gCost[neighbor])
                {
                    continue;
                }
                gCost[neighbor] = gCostTemp;
                fCost[neighbor] = gCost[neighbor] + HeuristicFunction(neighbor, goalNode);
                cameFrom[neighbor] = current;
            }
        }

        return null;
    }

    public List<NavTile> FindNodesInRange(NavTile startNode, float range)
    {
        HashSet<NavTile> closedSet = new HashSet<NavTile>();
        HashSet<NavTile> openSet = new HashSet<NavTile>() { startNode };
        Dictionary<NavTile, float> gCost = new Dictionary<NavTile, float>
        {
            [startNode] = 0
        };

        while (openSet.Count > 0)
        {
            NavTile current = openSet.OrderBy(n => gCost[n]).First();

            if (gCost[current] > range)
            {
                return new List<NavTile>(closedSet);
            }
            openSet.Remove(current);
            closedSet.Add(current);

            foreach (NavTile.Edge edge in current.Edges)
            {
                NavTile neighbor = edge.tile;
                float edgeWeight = edge.weight;

                if (closedSet.Contains(neighbor))
                {
                    continue;
                }

                float gCostTemp = gCost[current] + edgeWeight;
                if (!openSet.Contains(neighbor))
                {
                    openSet.Add(neighbor);
                }
                else if (gCostTemp >= gCost[neighbor])
                {
                    continue;
                }
                gCost[neighbor] = gCostTemp;
            }
        }

        return new List<NavTile>(closedSet);
    }

    private float HeuristicFunction(NavTile source, NavTile dest)
    {
        return Vector3.Distance(source.transform.position, dest.transform.position);
    }

    private List<NavTile> ReconstructPath(Dictionary<NavTile, NavTile> cameFrom, NavTile goal)
    {
        List<NavTile> path = new List<NavTile>();
        NavTile current = goal;
        path.Add(current);

        while (cameFrom.ContainsKey(current))
        {
            current = cameFrom[current];
            path.Add(current);
        }

        path.Reverse();
        return path;
    }
}