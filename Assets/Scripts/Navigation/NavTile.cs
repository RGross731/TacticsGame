﻿using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
[RequireComponent(typeof(BoxCollider))]
public class NavTile : MonoBehaviour
{
    [SerializeField]
    [HideInInspector]
    private List<Edge> edges;
    public List<Edge> Edges { get => edges; set => edges = value; }

    private void OnMouseOver()
    {
        PlayerInput.HandleTileMouseOver(this);
    }

    private void OnMouseDown()
    {
        PlayerInput.HandleTileMouseDown(this);
    }

    [System.Serializable]
    public class Edge
    {
        public NavTile tile;
        public float weight;

        public Edge(NavTile tile, float weight)
        {
            this.tile = tile;
            this.weight = weight;
        }
    }
}